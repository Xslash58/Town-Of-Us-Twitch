using System;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using AmongUs.GameOptions;

namespace TownOfUs.CrewmateRoles.JailorMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Jailor);
            if (!flag) return true;
            var role = Role.GetRole<Jailor>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            var flag2 = role.JailTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;

            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[4] == true)
            {
                role.Jailed = role.ClosestPlayer;
                Utils.Rpc(CustomRPC.Jail, PlayerControl.LocalPlayer.PlayerId, (byte)0, role.Jailed.PlayerId);
            }
            if (interact[0] == true)
            {
                role.LastJailed = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastJailed = DateTime.UtcNow;
                role.LastJailed = role.LastJailed.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.JailCd);
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}
