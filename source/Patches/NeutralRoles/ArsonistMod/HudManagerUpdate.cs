using System.Linq;
using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnityEngine;

namespace TownOfUs.NeutralRoles.ArsonistMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static Sprite IgniteSprite => TownOfUs.IgniteSprite;
        
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist)) return;
            var role = Role.GetRole<Arsonist>(PlayerControl.LocalPlayer);

            if (!PlayerControl.LocalPlayer.IsHypnotised())
            {
                foreach (var playerId in role.DousedPlayers)
                {
                    var player = Utils.PlayerById(playerId);
                    var data = player?.Data;
                    if (data == null || data.Disconnected || data.IsDead || PlayerControl.LocalPlayer.Data.IsDead)
                        continue;

                    player.myRend().material.SetColor("_VisorColor", role.Color);

                    var colour = Color.black;
                    if (player.Is(ModifierEnum.Shy)) colour.a = Modifier.GetModifier<Shy>(player).Opacity;
                    player.nameText().color = colour;
                }
            }

            if (role.IgniteButton == null)
            {
                role.IgniteButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.IgniteButton.graphic.enabled = true;
                role.IgniteButton.gameObject.SetActive(false);
            }

            role.IgniteButton.graphic.sprite = IgniteSprite;
            role.IgniteButton.transform.localPosition = new Vector3(-2f, 0f, 0f);

            if (PlayerControl.LocalPlayer.Data.IsDead) role.IgniteButton.SetTarget(null);

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            role.IgniteButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            if (!role.LastKiller || !CustomGameOptions.IgniteCdRemoved) role.IgniteButton.SetCoolDown(role.DouseTimer(), CustomGameOptions.DouseCd);
            else role.IgniteButton.SetCoolDown(0f, CustomGameOptions.DouseCd);
            if (role.DousedAlive < CustomGameOptions.MaxDoused)
            {
                __instance.KillButton.SetCoolDown(role.DouseTimer(), CustomGameOptions.DouseCd);
            }

            var notDoused = PlayerControl.AllPlayerControls.ToArray().Where(
                player => !role.DousedPlayers.Contains(player.PlayerId)
            ).ToList();
            var doused = PlayerControl.AllPlayerControls.ToArray().Where(
                player => role.DousedPlayers.Contains(player.PlayerId)
            ).ToList();

            if (role.DousedAlive < CustomGameOptions.MaxDoused)
            {
                if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTarget(ref role.ClosestPlayerDouse, __instance.KillButton, float.NaN, notDoused);
                else if (role.Player.IsLover()) Utils.SetTarget(ref role.ClosestPlayerDouse, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && !role.DousedPlayers.Contains(x.PlayerId)).ToList());
                else Utils.SetTarget(ref role.ClosestPlayerDouse, __instance.KillButton, float.NaN, notDoused);
            }
            else __instance.KillButton.SetTarget(null);

            if (role.DousedAlive > 0)
            {
                if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTarget(ref role.ClosestPlayerIgnite, role.IgniteButton, float.NaN, doused);
                else if (role.Player.IsLover()) Utils.SetTarget(ref role.ClosestPlayerIgnite, role.IgniteButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover() && role.DousedPlayers.Contains(x.PlayerId)).ToList());
                else Utils.SetTarget(ref role.ClosestPlayerIgnite, role.IgniteButton, float.NaN, doused);
            }
            else role.IgniteButton.SetTarget(null);

            return;
        }
    }
}
