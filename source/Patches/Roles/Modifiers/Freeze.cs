namespace TownOfUs.Roles.Modifiers
{
    public class Freeze : Modifier
    {
        public Freeze(PlayerControl player) : base(player)
        {
            Name = "Freeze";
            TaskText = () => "Killing you causes killer to freeze";
            Color = Patches.Colors.Bait;
            ModifierType = ModifierEnum.Freeze;
        }
    }
}