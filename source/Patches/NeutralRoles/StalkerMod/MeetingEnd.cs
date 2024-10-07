using HarmonyLib;
using System;
using TownOfUs.Patches.Roles;
using TownOfUs.Roles;

namespace TownOfUs.Patches.NeutralRoles.StalkerMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
    public class MeetingEnd
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Stalker)) return;
            var role = Role.GetRole<Stalker>(PlayerControl.LocalPlayer);

            role.LastChanged = DateTime.UtcNow;
            role.LastKilled = DateTime.UtcNow;
        }
    }
}
