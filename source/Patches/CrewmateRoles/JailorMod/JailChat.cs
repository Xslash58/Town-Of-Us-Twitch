using HarmonyLib;
using TownOfUs.Patches;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.JailorMod
{
    [HarmonyPatch]
    public static class JailChat
    {
        public static bool JailorMessage = false;

        [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
        public static class PrivateJaileeChat
        {
            public static bool Prefix(ChatController __instance, [HarmonyArgument(0)] ref PlayerControl sourcePlayer, ref string chatText)
            {
                if ((chatText.ToLower().StartsWith("/jail") || chatText.ToLower().StartsWith("/ jail")) && sourcePlayer.Is(RoleEnum.Jailor) && MeetingHud.Instance)
                {
                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Jailor) || PlayerControl.LocalPlayer.IsJailed())
                    {
                        if (chatText.ToLower().StartsWith("/jail")) chatText = chatText[5..];
                        else if (chatText.ToLower().StartsWith("/jail ")) chatText = chatText[6..];
                        else if (chatText.ToLower().StartsWith("/ jail")) chatText = chatText[6..];
                        else if (chatText.ToLower().StartsWith("/ jail ")) chatText = chatText[7..];
                        JailorMessage = true;
                        if (sourcePlayer != PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.IsJailed()) sourcePlayer = PlayerControl.LocalPlayer;
                        return true;
                    }
                    else return false;
                }
                if (sourcePlayer.IsJailed() && MeetingHud.Instance)
                {
                    if (PlayerControl.LocalPlayer == sourcePlayer || PlayerControl.LocalPlayer.Is(RoleEnum.Jailor)) return true;
                    else return false;
                }
                if (PlayerControl.LocalPlayer.IsJailed() && MeetingHud.Instance) return false;
                return true;
            }
        }

        [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
        public static class SetName
        {
            public static void Postfix(ChatBubble __instance, [HarmonyArgument(0)] string playerName)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Jailor) && MeetingHud.Instance)
                {
                    var jailor = Role.GetRole<Jailor>(PlayerControl.LocalPlayer);
                    if (jailor.Jailed != null && jailor.Jailed.Data.PlayerName == playerName)
                    {
                        __instance.NameText.color = jailor.Color;
                        __instance.NameText.text = playerName + " (Jailed)";
                    }
                    else if (JailorMessage)
                    {
                        __instance.NameText.color = jailor.Color;
                        __instance.NameText.text = "Jailor";
                        JailorMessage = false;
                    }
                }
                if (PlayerControl.LocalPlayer.IsJailed() && MeetingHud.Instance)
                {
                    if (JailorMessage)
                    {
                        __instance.NameText.color = Colors.Jailor;
                        __instance.NameText.text = "Jailor";
                        JailorMessage = false;
                    }
                }
            }
        }
    }
}