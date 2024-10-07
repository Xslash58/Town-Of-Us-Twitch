using HarmonyLib;
using System.Linq;
using TownOfUs.Extensions;
using TownOfUs.Patches.Roles;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.NeutralRoles.StalkerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class StalkerTargetColor
    {
        private static void UpdateMeeting(MeetingHud __instance, Stalker role)
        {
            foreach (var player in __instance.playerStates)
                if (player.TargetPlayerId == role.target.PlayerId)
                    player.NameText.color = new Color(0.25f, 0.25f, 0.25f, 1f);
        }

        private static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Stalker)) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;

            var role = Role.GetRole<Stalker>(PlayerControl.LocalPlayer);

            if (role.target == null) return;

            if (MeetingHud.Instance != null) UpdateMeeting(MeetingHud.Instance, role);

            role.target.nameText().color = new Color(0.25f, 0.25f, 0.25f, 1f);

            if (!role.target.Data.IsDead && !role.target.Data.Disconnected && !role.target.Is(RoleEnum.Vampire)) return;

            SelectNewTarget(role);
        }

        private static void SelectNewTarget(Stalker role)
        {
            var stalkerTargets = PlayerControl.AllPlayerControls.ToArray().Where(x => x != role.Player).ToList();
            stalkerTargets.Shuffle();
            if (stalkerTargets.Count > 0)
            {
                role.ChangeTarget(stalkerTargets.FirstOrDefault());
            }
        }
    }
}