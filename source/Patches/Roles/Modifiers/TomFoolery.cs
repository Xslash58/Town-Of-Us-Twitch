using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles.Modifiers
{
    public class TomFoolery : Modifier, IVisualAlteration
    {
        public TomFoolery(PlayerControl player) : base(player)
        {
            Name = "TomFoolery";
            TaskText = () => "Your chat baited you.";
            Color = Patches.Colors.UpsideDown;
            ModifierType = ModifierEnum.TomFoolery;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            appearance = Player.GetDefaultAppearance();

            if (Player == PlayerControl.LocalPlayer)
                Camera.main.transform.Rotate(new Vector3(0, 0, 1));

            return true;
        }
    }
}
