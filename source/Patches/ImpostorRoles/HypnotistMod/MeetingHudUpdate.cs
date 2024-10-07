using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.HypnotistMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    public static class MeetingHudUpdate
    {
        public static void Postfix(MeetingHud __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            var _role = Role.GetRole(localPlayer);
            if (_role?.RoleType != RoleEnum.Hypnotist) return;
            if (localPlayer.Data.IsDead) return;
            var role = (Hypnotist)_role;
            foreach (var state in __instance.playerStates)
            {
                var targetId = state.TargetPlayerId;
                if (role.HypnotisedPlayers.Contains(targetId) && role.Player.PlayerId != targetId) state.NameText.color = new Color(0.6f, 0f, 0f);
            }
        }
    }
}