using HarmonyLib;
using Reactor.Utilities.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.JailorMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))] // BBFDNCCEJHI
    public static class VotingComplete
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Jailor))
            {
                var jailor = Role.GetRole<Jailor>(PlayerControl.LocalPlayer);
                jailor.ExecuteButton.Destroy();
                jailor.UsesText.Destroy();
            }
        }
    }
}