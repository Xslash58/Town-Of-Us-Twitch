using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;

namespace TownOfUs.CrewmateRoles.SnitchMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HighlightImpostors
    {
        private static void UpdateMeeting(MeetingHud __instance)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                foreach (var state in __instance.playerStates)
                {
                    if (player.PlayerId != state.TargetPlayerId) continue;
                    var role = Role.GetRole(player);
                    if (player.Is(Faction.Impostors) && !player.Is(RoleEnum.Traitor))
                        state.NameText.color = Palette.ImpostorRed;
                    else if (player.Is(RoleEnum.Traitor) && CustomGameOptions.SnitchSeesTraitor)
                        state.NameText.color = Palette.ImpostorRed;
                    if (player.Is(Faction.NeutralKilling) && CustomGameOptions.SnitchSeesNeutrals)
                        state.NameText.color = role.Color;
                }
            }
        }

        public static void Postfix(HudManager __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Snitch)) return;
            var role = Role.GetRole<Snitch>(PlayerControl.LocalPlayer);
            if (!role.TasksDone) return;
            if (MeetingHud.Instance && CustomGameOptions.SnitchSeesImpInMeeting) UpdateMeeting(MeetingHud.Instance);

            if (!PlayerControl.LocalPlayer.IsHypnotised())
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Data.IsImpostor() && !player.Is(RoleEnum.Traitor))
                    {
                        var colour = Palette.ImpostorRed;
                        if (player.Is(ModifierEnum.Shy)) colour.a = Modifier.GetModifier<Shy>(player).Opacity;
                        player.nameText().color = colour;
                    }
                    else if (player.Is(RoleEnum.Traitor) && CustomGameOptions.SnitchSeesTraitor)
                    {
                        var colour = Palette.ImpostorRed;
                        if (player.Is(ModifierEnum.Shy)) colour.a = Modifier.GetModifier<Shy>(player).Opacity;
                        player.nameText().color = colour;
                    }
                    var playerRole = Role.GetRole(player);
                    if (playerRole.Faction == Faction.NeutralKilling && CustomGameOptions.SnitchSeesNeutrals)
                    {
                        var colour = playerRole.Color;
                        if (player.Is(ModifierEnum.Shy)) colour.a = Modifier.GetModifier<Shy>(player).Opacity;
                        player.nameText().color = colour;
                    }
                }
            }
        }
    }
}