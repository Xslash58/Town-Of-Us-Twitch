using TownOfUs.Extensions;
using UnityEngine;
using System.Collections;
using Reactor.Utilities;

namespace TownOfUs.Roles.Modifiers
{
    public class Mini : Modifier, IVisualAlteration
    {
        bool isTaskAdjust = false;
        public Mini(PlayerControl player) : base(player)
        {
            var slowText = CustomGameOptions.MiniSpeed >= 1.50 ? " and fast!" : "!";
            Name = "Mini";
            TaskText = () => "You are tiny" + slowText;
            Color = Patches.Colors.Mini;
            ModifierType = ModifierEnum.Mini;

            isTaskAdjust = false;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            appearance = Player.GetDefaultAppearance();
            appearance.SpeedFactor = CustomGameOptions.MiniSpeed;
            appearance.SizeFactor = new Vector3(0.40f, 0.40f, 1f);

            if (!isTaskAdjust && Player == PlayerControl.LocalPlayer)
            {
                isTaskAdjust = true;
                Coroutines.Start(DelayedTaskPatch());
            }

            return true;
        }

        IEnumerator DelayedTaskPatch()
        {
            yield return new WaitForSeconds(5);
            foreach (var task in GameObject.FindObjectsOfType<NormalPlayerTask>())
                foreach (var console in task.FindConsoles())
                    console.usableDistance += 0.2f;
        }
    }
}
