using Il2CppSystem.Collections.Generic;
using System;
using System.Linq;
using TownOfUs.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.Patches.Roles
{
    public class Stalker : Role
    {
        public bool StalkerWins { get; set; }

        public PlayerControl ClosestPlayer;
        public PlayerControl ClosestChangePlayer;
        public DateTime LastKilled { get; set; }
        public DateTime LastChanged { get; set; }
        public PlayerControl target;
        public KillButton TargetChangeButton;

        public bool wasNearTarget = false;
        public bool isNearTarget = false;
        public bool canKill = false;
        public Stalker(PlayerControl player) : base(player)
        {
            Name = "Stalker";
            ImpostorText = () => $"Stalk Your Victim";
            TaskText = () => $"Stalk your victims to kill them!\nFake Tasks:";
            Color = Patches.Colors.Stalker;
            LastKilled = DateTime.UtcNow;
            RoleType = RoleEnum.Stalker;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralKilling;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var stalkTeam = new List<PlayerControl>();
            stalkTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = stalkTeam;
        }
        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 2 &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Data.IsImpostor() || x.Is(Faction.NeutralKilling))) == 1)
            {
                Utils.Rpc(CustomRPC.StalkerWin, Player.PlayerId);
                Wins();
                Utils.EndGame();
                return false;
            }

            return false;
        }
        public void Wins()
        {
            StalkerWins = true;
        }

        public void ChangeTarget(PlayerControl newTarget)
        {
            target = newTarget;
            LastKilled = DateTime.UtcNow;
            LastChanged = DateTime.UtcNow;
            Utils.Rpc(CustomRPC.SetStalk, Player.PlayerId, target.PlayerId);
        }

        public float StalkerKillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = CustomGameOptions.StalkerKillCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
        public float StalkerChangeTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastChanged;
            var num = CustomGameOptions.StalkerChangeCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}
