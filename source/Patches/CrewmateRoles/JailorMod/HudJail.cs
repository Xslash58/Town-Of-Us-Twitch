using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.JailorMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudJail
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Jailor)) return;
            var jailButton = __instance.KillButton;

            var role = Role.GetRole<Jailor>(PlayerControl.LocalPlayer);

            jailButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            if (role.CanJail)
            {
                jailButton.SetCoolDown(role.JailTimer(), CustomGameOptions.JailCd);
                Utils.SetTarget(ref role.ClosestPlayer, jailButton, float.NaN);
            }
            else
            {
                jailButton.SetCoolDown(0f, CustomGameOptions.JailCd);
                jailButton.SetTarget(null);
            }
        }
    }
}
