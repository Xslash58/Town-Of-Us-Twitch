using Reactor.Utilities;
using System.Collections;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Extensions;
using TownOfUs.Patches;
using UnityEngine;

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
            TaskText = () => "You are radioactive";
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
                    CustomGameOptions.RadiativeRadius * ShipStatus.Instance.MaxLightRadius * 2f,
                    CustomGameOptions.RadiativeRadius * ShipStatus.Instance.MaxLightRadius * 2f,
                    CustomGameOptions.RadiativeRadius * ShipStatus.Instance.MaxLightRadius * 2f);

                radiationVisual.GetComponent<MeshRenderer>().material = Roles.Trapper.trapMaterial;
                radiationVisual.GetComponent<MeshRenderer>().material.color = Color.green;
            }

            radiationVisual.transform.position = new Vector3(Player.transform.position.x,
                Player.transform.position.y,
                0);

            if (!isTimerSet && Player == PlayerControl.LocalPlayer)
            {
                isTimerSet = true;
                Coroutines.Start(DelayRadiate());
            }

            return true;
        }

        public void StartRadiation()
        {
            Coroutines.Start(DelayRadiate());
        }

        IEnumerator DelayRadiate()
        {
            yield return new WaitForSeconds(7);
            Coroutines.Start(RadiateNearby());
        }

        IEnumerator RadiateNearby()
        {
            yield return new WaitForSeconds(CustomGameOptions.RadiativeCooldown);

            if (Player != null && MeetingHud.Instance == null)
            {
                var playersToDie = Utils.GetClosestPlayers(Player.transform.position, CustomGameOptions.RadiativeRadius, false);

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