using HarmonyLib;
using TownOfUs.Extensions;

namespace TownOfUs.Patches
{
    [HarmonyPatch]
    public class PlayerUpdate
    {
        public static bool NameHide = false;
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (CustomGameOptions.HideNames)
            {
                NameHide = true;
                foreach (var player in PlayerControl.AllPlayerControls)
                    player.nameText().enabled = false;
            } else if (NameHide)
            {
                NameHide = false;
                foreach (var player in PlayerControl.AllPlayerControls)
                    player.nameText().enabled = true;
            }
        }
    }
}
