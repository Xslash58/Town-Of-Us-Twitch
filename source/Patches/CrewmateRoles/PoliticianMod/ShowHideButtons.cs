using HarmonyLib;
using TownOfUs.Roles;
using Reactor.Utilities.Extensions;

namespace TownOfUs.CrewmateRoles.PoliticianMod
{
    public class ShowHideButtonsPolitician
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
        public static class Confirm
        {
            public static bool Prefix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Politician)) return true;
                var politician = Role.GetRole<Politician>(PlayerControl.LocalPlayer);
                politician.RevealButton.Destroy();
                return true;
            }
        }
    }
}