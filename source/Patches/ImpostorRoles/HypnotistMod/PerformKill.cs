using HarmonyLib;
using TownOfUs.Roles;
using System;

namespace TownOfUs.ImpostorRoles.HypnotistMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Hypnotist)) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Hypnotist>(PlayerControl.LocalPlayer);
            var target = role.ClosestPlayer;
            if (__instance == role.HypnotiseButton)
            {
                if (role.Player.inVent) return false;
                if (!__instance.isActiveAndEnabled || role.ClosestPlayer == null) return false;
                if (__instance.isCoolingDown) return false;
                if (role.HypnotiseTimer() != 0) return false;

                var interact = Utils.Interact(PlayerControl.LocalPlayer, target);
                if (interact[4] == true)
                {
                    role.HypnotisedPlayers.Add(target.PlayerId);
                    Utils.Rpc(CustomRPC.Hypnotise, PlayerControl.LocalPlayer.PlayerId, (byte)0, target.PlayerId);
                }
                if (interact[0] == true)
                {
                    role.LastHypnotised = DateTime.UtcNow;
                    return false;
                }
                else if (interact[1] == true)
                {
                    role.LastHypnotised = DateTime.UtcNow;
                    role.LastHypnotised.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.HypnotiseCd);
                    return false;
                }
                else if (interact[3] == true) return false;
                return false;
            }
            return true;
        }
    }
}