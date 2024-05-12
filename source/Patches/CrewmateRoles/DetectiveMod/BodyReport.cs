using System;
using System.Linq;
using HarmonyLib;
using TownOfUs.CrewmateRoles.MedicMod;

namespace TownOfUs.CrewmateRoles.DetectiveMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    internal class BodyReportPatch
    {
        private static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] GameData.PlayerInfo info)
        {
            if (info == null) return;
            var matches = Murder.KilledPlayers.Where(x => x.PlayerId == info.PlayerId).ToArray();
            DeadPlayer killer = null;

            if (matches.Length > 0)
                killer = matches[0];

            if (killer == null)
                return;

            var isDetectiveAlive = __instance.Is(RoleEnum.Detective);
            var areReportsEnabled = CustomGameOptions.DetectiveReportOn;

            if (!isDetectiveAlive || !areReportsEnabled)
                return;

            var isUserDetective = PlayerControl.LocalPlayer.Is(RoleEnum.Detective);
            if (!isUserDetective)
                return;
            var br = new BodyReport
            {
                Killer = Utils.PlayerById(killer.KillerId),
                Reporter = __instance,
                Body = Utils.PlayerById(killer.PlayerId),
                KillAge = (float) (DateTime.UtcNow - killer.KillTime).TotalMilliseconds
            };

            var reportMsg = BodyReport.ParseBodyReport(br);

            if (string.IsNullOrWhiteSpace(reportMsg))
                return;

            if (DestroyableSingleton<HudManager>.Instance)
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, reportMsg, censor: false);
        }
    }
}