using HarmonyLib;
using Reactor.Utilities;
using static TownOfUs.Roles.Glitch;

namespace TownOfUs.NeutralRoles.GlitchMod
{
    [HarmonyPatch(typeof(ReportButton), nameof(ReportButton.DoClick))]
    public class StopReport
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(ReportButton __instance)
        {
            if (PlayerControl.LocalPlayer.IsHacked())
            {
                Coroutines.Start(AbilityCoroutine.Hack(PlayerControl.LocalPlayer));
                return false;
            }
            return true;
        }
    }
}
