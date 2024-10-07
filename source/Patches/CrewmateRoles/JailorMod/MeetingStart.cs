using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.JailorrMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (PlayerControl.LocalPlayer.IsJailed())
            {
                if (PlayerControl.LocalPlayer.Is(Faction.Crewmates))
                {
                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "You are jailed, provide relevant information to the Jailor to prove you are Crew");
                }
                else
                {
                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "You are jailed, convince the Jailor that you are Crew to avoid being executed");
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Jailor))
            {
                var jailor = Role.GetRole<Jailor>(PlayerControl.LocalPlayer);
                if (jailor.Jailed.Data.IsDead || jailor.Jailed.Data.Disconnected) return;
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "Use /jail to communicate with your jailee");
            }
        }
    }
}
