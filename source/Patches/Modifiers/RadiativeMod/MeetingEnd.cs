using HarmonyLib;
using TownOfUs.Roles.Modifiers;
using UnityEngine;

namespace TownOfUs.Patches.Modifiers.RadiativeMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
    public class MeetingEnd
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(ModifierEnum.Radiative)) return;
            var modifier = Modifier.GetModifier<Radiative>(PlayerControl.LocalPlayer);

            modifier.StartRadiation();
        }
    }
}
