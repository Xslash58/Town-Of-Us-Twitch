using HarmonyLib;

namespace TownOfUs.CrewmateRoles.ImitatorMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class OverrideKillText
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (StartImitate.ImitatingPlayer == null) return;
            if (PlayerControl.LocalPlayer != StartImitate.ImitatingPlayer) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Sheriff) && !PlayerControl.LocalPlayer.Is(RoleEnum.Hunter)) __instance.KillButton.OverrideText("");
            return;
        }
    }
}