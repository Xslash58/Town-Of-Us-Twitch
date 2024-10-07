using TownOfUs.Roles;
using HarmonyLib;
using UnityEngine;

namespace TownOfUs.Patches
{
    [HarmonyPatch]
    public class LocalSettings
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        [HarmonyPostfix]

        public static void HideGhosts()
        {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
            if (!PlayerControl.LocalPlayer.Data.IsDead) return;
            if (MeetingHud.Instance) return;
            if (!Utils.ShowDeadBodies) return;

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player == PlayerControl.LocalPlayer) continue;
                if (!player.Data.IsDead) continue;
                if (player.Is(RoleEnum.Haunter) && !Role.GetRole<Haunter>(player).Caught) continue;
                if (player.Is(RoleEnum.Phantom) && !Role.GetRole<Phantom>(player).Caught) continue;

                bool show = TownOfUs.DeadSeeGhosts.Value;
                var bodyforms = player.gameObject.transform.GetChild(1).gameObject;

                foreach (var form in bodyforms.GetAllChilds())
                {
                    if (form.activeSelf)
                    {
                        form.GetComponent<SpriteRenderer>().color = new(1f, 1f, 1f, show ? 1f : 0f);
                    }
                }

                if (player.cosmetics.HasPetEquipped()) player.cosmetics.CurrentPet.Visible = show;
                player.cosmetics.gameObject.SetActive(show);
                player.gameObject.transform.GetChild(3).gameObject.SetActive(show);
            }
        }
    }
}