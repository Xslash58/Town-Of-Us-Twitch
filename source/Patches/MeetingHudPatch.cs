using HarmonyLib;
using UnityEngine;

namespace TownOfUs.Patches
{
    [HarmonyPatch]
    public class MeetingHudPatch
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        [HarmonyPostfix]

        public static void Postfix(MeetingHud __instance)
        {
            if (!CustomGameOptions.HideNames) return;

            __instance.HostIcon.enabled = false;
            foreach (var state in __instance.playerStates)
                state.NameText.text = string.Empty;
            foreach(var bubble in GameObject.FindObjectsOfType<ChatBubble>())
                bubble.NameText.text = string.Empty;
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CoIntro))]
        [HarmonyPostfix]

        public static void Postfix(GameData.PlayerInfo reporter, GameData.PlayerInfo reportedBody, Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<GameData.PlayerInfo> deadBodies)
        {
            if (!CustomGameOptions.HideNames) return;

            if(reporter != null) reporter.PlayerName = string.Empty;
            if(reportedBody != null) reportedBody.PlayerName = string.Empty;
        }
    }
}
