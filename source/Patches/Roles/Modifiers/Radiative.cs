using Reactor.Utilities;
using System.Collections;
using TownOfUs.CrewmateRoles.DetectiveMod;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Extensions;
using TownOfUs.Patches;
using UnityEngine;
using static Epic.OnlineServices.Helper;

namespace TownOfUs.Roles.Modifiers
{
    public class Radiative : Modifier, IVisualAlteration
    {
        bool isTimerSet = false;
        GameObject radiationVisual;
        public static Material bombMaterial = TownOfUs.bundledAssets.Get<Material>("bomb");
        public Radiative(PlayerControl player) : base(player)
        {
            Name = "Radiative";
            TaskText = () => "You are radioactive... radioactive catJAM";
            Color = Patches.Colors.Radiative;
            ModifierType = ModifierEnum.Radiative;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            appearance = Player.GetDefaultAppearance();

            string playerName = Player.nameText().text;
            Player.nameText().text = "<color=#39FF14>RADIATIVE</color>\n" + playerName;

            if (ShipStatus.Instance && radiationVisual == null)
            {
                radiationVisual = GameObject.CreatePrimitive(PrimitiveType.Sphere); 
                radiationVisual.name = "RadiationZone";
                radiationVisual.transform.position = Player.transform.position;
                radiationVisual.layer = LayerMask.NameToLayer("Players");

                radiationVisual.transform.localScale = new Vector3(
                    0.2f * ShipStatus.Instance.MaxLightRadius * 2f,
                    0.2f * ShipStatus.Instance.MaxLightRadius * 2f,
                    0.2f * ShipStatus.Instance.MaxLightRadius * 2f);

                radiationVisual.GetComponent<MeshRenderer>().material = Roles.Bomber.bombMaterial;
                radiationVisual.GetComponent<MeshRenderer>().material.color = Color.green;

                //SpriteRenderer render = radiationVisual.AddComponent<SpriteRenderer>();
                //render.sprite = TownOfUs.bo;
            }

            radiationVisual.transform.position = new Vector3(Player.transform.position.x,
                Player.transform.position.y,
                0);

            if (!isTimerSet && Player == PlayerControl.LocalPlayer)
            {
                isTimerSet = true;
                Coroutines.Start(RadiateNearby());
            }

            return true;
        }

        IEnumerator RadiateNearby()
        {
            yield return new WaitForSeconds(10);

            if (Player != null)
            {
                var playersToDie = Utils.GetClosestPlayers(Player.transform.position, 0.2f, false);

                foreach (var player in playersToDie)
                {
                    if (player == Player) continue;

                    if (!player.Is(RoleEnum.Pestilence) && !player.IsShielded() && !player.IsProtected() && player != ShowRoundOneShield.FirstRoundShielded)
                    {
                        Utils.RpcMultiMurderPlayer(Player, player);
                    }
                    else if (player.IsShielded())
                    {
                        var medic = player.GetMedic().Player.PlayerId;
                        Utils.Rpc(CustomRPC.AttemptSound, medic, player.PlayerId);
                        StopKill.BreakShield(medic, player.PlayerId, CustomGameOptions.ShieldBreaks);
                    }
                }

                Coroutines.Start(RadiateNearby());
            }
        }
    }
}