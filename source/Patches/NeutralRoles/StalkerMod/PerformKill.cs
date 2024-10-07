using AmongUs.GameOptions;
using HarmonyLib;
using System;
using TownOfUs.Patches.Roles;
using TownOfUs.Roles;

namespace TownOfUs.Patches.NeutralRoles.StalkerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Stalker);
            if (!flag) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = Role.GetRole<Stalker>(PlayerControl.LocalPlayer);
            if (role.Player.inVent) return false;

            //Change Player Logic
            if (__instance == role.TargetChangeButton && role.ClosestChangePlayer != null)
            {
                if (role.StalkerChangeTimer() != 0) return false;

                role.ChangeTarget(role.ClosestChangePlayer);
                return false;
            }

            //Kill logic
            if (role.StalkerKillTimer() != 0) return false;

            if (role.ClosestPlayer == null) return false;
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers <
                        GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (!flag3) return false;


            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, true);
            if (interact[4] == true) return false;
            else if (interact[0] == true)
            {
                role.LastKilled = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastKilled = DateTime.UtcNow;
                return false;
            }
            else if (interact[2] == true)
            {
                role.LastKilled = DateTime.UtcNow;
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}
