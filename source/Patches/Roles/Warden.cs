using System;

namespace TownOfUs.Roles
{
    public class Warden : Role
    {
        public PlayerControl ClosestPlayer;
        public PlayerControl Fortified;
        public DateTime LastFortified { get; set; }

        public Warden(PlayerControl player) : base(player)
        {
            Name = "Warden";
            ImpostorText = () => "Fortify Crewmates";
            TaskText = () => "Fortify the Crewmates";
            Color = Patches.Colors.Warden;
            LastFortified = DateTime.UtcNow;
            RoleType = RoleEnum.Warden;
            AddToRoleHistory(RoleType);
        }
        public float FortifyTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastFortified;
            var num = CustomGameOptions.FortifyCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}