using System;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using AmongUs.GameOptions;

namespace TownOfUs.CrewmateRoles.WardenMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Warden);
            if (!flag) return true;
            var role = Role.GetRole<Warden>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            var flag2 = role.FortifyTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null || role.Fortified != null) return false;

            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[4] == true)
            {
                role.Fortified = role.ClosestPlayer;
                Utils.Rpc(CustomRPC.Fortify, (byte)0, PlayerControl.LocalPlayer.PlayerId, role.Fortified.PlayerId);
                return false;
            }
            if (interact[0] == true)
            {
                role.LastFortified = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastFortified = DateTime.UtcNow;
                role.LastFortified = role.LastFortified.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.FortifyCd);
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}
