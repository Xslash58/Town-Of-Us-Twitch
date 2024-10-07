using HarmonyLib;
using UnityEngine;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(ChatNotification), nameof(ChatNotification.SetUp))]
    public class ChatColourPatch
    {
        public static bool Prefix(ChatNotification __instance, PlayerControl sender, string text)
        {
            if (ShipStatus.Instance)
            {
                return false;
            }
            __instance.timeOnScreen = 5f;
            __instance.gameObject.SetActive(true);
            __instance.SetCosmetics(sender.Data);
            string htmlString;
            Color colour;
            try
            {
                htmlString = ColorUtility.ToHtmlStringRGB(Palette.TextColors[__instance.player.ColorId]);
                colour = Palette.TextOutlineColors[__instance.player.ColorId];
            }
            catch
            {
                Color32 customColour = Palette.PlayerColors[__instance.player.ColorId];
                htmlString = ColorUtility.ToHtmlStringRGB(customColour);

                colour = customColour.r + customColour.g + customColour.b > 180 ? Color.black : Color.white;
            }
            __instance.playerColorText.text = __instance.player.ColorBlindName;
            __instance.playerNameText.text = "<color=#" + htmlString + ">" + (string.IsNullOrEmpty(sender.Data.PlayerName) ? "..." : sender.Data.PlayerName);
            __instance.playerNameText.outlineColor = colour;
            __instance.chatText.text = text;
            return false;
        }
    }
}