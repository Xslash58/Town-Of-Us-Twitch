using AmongUs.GameOptions;
using HarmonyLib;
using System;
using System.Linq;
using TownOfUs.Patches.Roles;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.Patches.NeutralRoles.StalkerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static Sprite defaultSprite = null;
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Stalker)) return;
            var role = Role.GetRole<Stalker>(PlayerControl.LocalPlayer);

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            __instance.KillButton.SetCoolDown(role.StalkerKillTimer(), CustomGameOptions.StalkerKillCd);

            if (role.target != null && Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(), role.target.GetTruePosition()) <=
            GameOptionsData.KillDistances[CustomGameOptions.StalkerStalkRadius])
                role.isNearTarget = true;
            else
                role.isNearTarget = false;

            if (defaultSprite == null)
                defaultSprite = __instance.KillButton.graphic.sprite;

            if (role.canKill)
            {
                __instance.KillButton.graphic.sprite = defaultSprite;
                __instance.KillButton.buttonLabelText.text = "KILL";
                Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, new() { role.target });
            }
            else
            {
                __instance.KillButton.graphic.sprite = TownOfUs.ImitateDeselectSprite;
                __instance.KillButton.buttonLabelText.text = "TOO FAR";
                Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, new());
            }

            if (role.isNearTarget && !role.wasNearTarget)
            {
                role.wasNearTarget = true;
                role.LastKilled = DateTime.UtcNow; //set cooldown
                role.canKill = true;
            }
            else if (!role.isNearTarget && role.wasNearTarget)
            {
                role.wasNearTarget = false;
                role.LastKilled = DateTime.UtcNow.AddSeconds(-CustomGameOptions.StalkerKillCd); //remove cooldown
                role.canKill = false;
            }

            if (role.TargetChangeButton == null)
            {
                role.TargetChangeButton = GameObject.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.TargetChangeButton.graphic.enabled = true;
                role.TargetChangeButton.gameObject.SetActive(true);
            }

            role.TargetChangeButton.graphic.sprite = TownOfUs.StalkSprite;
            role.TargetChangeButton.transform.localPosition = new Vector3(-2f, 0f, 0f);

            role.TargetChangeButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            role.TargetChangeButton.SetCoolDown(role.StalkerChangeTimer(), CustomGameOptions.StalkerChangeCd);

            var playerTargets = PlayerControl.AllPlayerControls.ToArray().Where(x => x != role.target).ToList();
            Utils.SetTarget(ref role.ClosestChangePlayer, role.TargetChangeButton, float.NaN, playerTargets);
        }
    }
}
