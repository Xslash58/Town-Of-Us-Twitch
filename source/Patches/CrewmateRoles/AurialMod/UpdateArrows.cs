using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.AurialMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class UpdateArrows
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Aurial)) return;

            var role = Role.GetRole<Aurial>(PlayerControl.LocalPlayer);

            if (PlayerControl.LocalPlayer.Data.IsDead)
            {
                foreach (var arrow in role.SenseArrows)
                {
                    role.DestroyArrow(arrow.Key.Item1, arrow.Key.Item2);
                }
                return;
            }

            foreach (var arrow in role.SenseArrows)
            {
                if (RainbowUtils.IsRainbow(arrow.Key.Item2))
                {
                    arrow.Value.image.color = RainbowUtils.Rainbow;
                }
            }
        }
    }
}