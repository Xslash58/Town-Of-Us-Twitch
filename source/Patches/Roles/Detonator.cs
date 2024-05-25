using UnityEngine;
using System;
using TownOfUs.ImpostorRoles.BomberMod;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Patches;
using TownOfUs.CrewmateRoles.TimeLordMod;

namespace TownOfUs.Roles
{
    public class Detonator : Role

    {
        public KillButton _plantButton;
        public KillButton _detonateButton;
        public bool Detonated = false;
        public bool BombAttached = false;
        public PlayerControl ClosestPlayer;
        public PlayerControl BombPlayer;
        public DateTime LastPlanted;
        public DateTime StartingCooldown { get; set; }

        public Detonator(PlayerControl player) : base(player)
        {
            Name = "Detonator";
            ImpostorText = () => "Attach Bombs To Kill Multiple Crewmates At Once";
            TaskText = () => "Detonate crewmates";
            Color = Palette.ImpostorRed;
            LastPlanted = DateTime.UtcNow;
            StartingCooldown = DateTime.UtcNow;
            RoleType = RoleEnum.Detonator;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
        }
        public KillButton PlantButton
        {
            get => _plantButton;
            set
            {
                _plantButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public KillButton DetonateButton
        {
            get => _detonateButton;
            set
            {
                _detonateButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public float PlantTimer(bool detonate)
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastPlanted;
            var num = !detonate ? CustomGameOptions.DetonatorPlantCooldown * 1000f
                : CustomGameOptions.DetonatorDetonateCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Detonate(PlayerControl playerToDetonate)
        {
            var playersToDie = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            
            if (CustomGameOptions.DetonatorMaxKillsInDetonation == 1) playersToDie.Add(playerToDetonate);
            else playersToDie = Utils.GetClosestPlayers(playerToDetonate.transform.position, CustomGameOptions.DetonatorRadius, false);

            playersToDie = Shuffle(playersToDie);
            
            while (playersToDie.Count > CustomGameOptions.DetonatorMaxKillsInDetonation) playersToDie.Remove(playersToDie[playersToDie.Count - 1]);
            foreach (var player in playersToDie)
            {
                if (!player.Is(RoleEnum.Pestilence) && !player.IsShielded() && !player.IsProtected() && player != ShowRoundOneShield.FirstRoundShielded)
                {
                    Utils.RpcMultiMurderPlayer(Player, player);
                }
                else if (player.IsShielded())
                {
                    var medic = player.GetMedic().Player.PlayerId;
                    Utils.Rpc(CustomRPC.AttemptSound, medic, player.PlayerId);
                    StopKill.BreakShield(medic, player.PlayerId, CustomGameOptions.ShieldBreaks);
                }
            }
        }
        public static Il2CppSystem.Collections.Generic.List<PlayerControl> Shuffle(Il2CppSystem.Collections.Generic.List<PlayerControl> playersToDie)
        {
            var count = playersToDie.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = UnityEngine.Random.Range(i, count);
                var tmp = playersToDie[i];
                playersToDie[i] = playersToDie[r];
                playersToDie[r] = tmp;
            }
            return playersToDie;
        }
    }
}