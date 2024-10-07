using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace TownOfUs
{
    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Initialize))]
    public class EnableMapImps
    {
        private static void Prefix(ref GameOptionsMenu __instance)
        {
            __instance.HideForOnline = new Il2CppReferenceArray<Transform>(0);
        }
    }
}