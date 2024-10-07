using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.InvestigatorMod
{
    public class EndGame
    {
        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.ExitGame))]
        [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]

        public static class EndGamePatch
        {
            public static void Prefix()
            {
                foreach (var role in Role.GetRoles(RoleEnum.Investigator)) ((Investigator)role).AllPrints.Clear();
            }
        }
    }
}