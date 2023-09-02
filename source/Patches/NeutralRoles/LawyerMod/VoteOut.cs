using HarmonyLib;
using System.Linq;
using TownOfUs.Roles;
using TownOfUs.Modifiers.AssassinMod;

namespace TownOfUs.NeutralRoles.LawyerMod
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    internal class MeetingExiledEnd
    {
        private static void Postfix(ExileController __instance)
        {
            var exiled = __instance.exiled;
            if (exiled == null) return;
            var player = exiled.Object;

            foreach (var role in Role.GetRoles(RoleEnum.Lawyer))
            {
                if (player.PlayerId == ((Lawyer)role).target.PlayerId && CustomGameOptions.LawyerDies)
                {
                    Utils.RpcMurderPlayer(role.Player, role.Player);
                    role.IncorrectAssassinKills -= 1;
                }
            }
        }
    }
}