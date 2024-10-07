using HarmonyLib;
using System.Linq;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.JuggernautMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut)) return;
            var role = Role.GetRole<Juggernaut>(PlayerControl.LocalPlayer);

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            if (CustomGameOptions.JuggKCd - CustomGameOptions.ReducedKCdPerKill * role.JuggKills <= 0) __instance.KillButton.SetCoolDown(role.KillTimer(), 0.01f);
            else __instance.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.JuggKCd - CustomGameOptions.ReducedKCdPerKill * role.JuggKills);

            if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);
            else if (role.Player.IsLover()) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover()).ToList());
            else Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);
        }
    }
}