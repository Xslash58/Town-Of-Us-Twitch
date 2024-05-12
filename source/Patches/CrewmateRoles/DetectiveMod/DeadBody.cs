using System;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.DetectiveMod
{
    public class BodyReport
    {
        public PlayerControl Killer { get; set; }
        public PlayerControl Reporter { get; set; }
        public PlayerControl Body { get; set; }
        public float KillAge { get; set; }

        public static string ParseBodyReport(BodyReport br)
        {
            if (br.KillAge > CustomGameOptions.DetectiveFactionDuration * 1000)
                return CustomGameOptions.PolishTranslations ? $"Raport: Miejsce zbrodni jest zbyt stare, aby uzyskac informacje. (Utworzone {Math.Round(br.KillAge / 1000)}s temu)" :
            $"Crime Scene Report: The crime scene is too old to gain information from. (Created {Math.Round(br.KillAge / 1000)}s ago)";

            if (br.Killer.PlayerId == br.Body.PlayerId)
                return CustomGameOptions.PolishTranslations ? $"Raport: Miejsce zbrodni wyglada na samobojstwo! (Utworzone {Math.Round(br.KillAge / 1000)}s temu)" :
                    $"Crime Scene Report: The crime scene appears to have been a suicide! (Created {Math.Round(br.KillAge / 1000)}s ago)";

            var role = Role.GetRole(br.Killer);

            if (br.KillAge < CustomGameOptions.DetectiveRoleDuration * 1000)
                return CustomGameOptions.PolishTranslations ? $"Raport: Miejsce zbrodni zdaje sie miec cialo nalezace do {role.Name}! (Utworzone {Math.Round(br.KillAge / 1000)}s temu)" :
                    $"Crime Scene Report: The crime scene appears to have a body of a {role.Name}! (Created {Math.Round(br.KillAge / 1000)}s ago)";

            if (br.Killer.Is(Faction.Crewmates))
                return CustomGameOptions.PolishTranslations ? $"Raport: Zabojca zdaje sie byc Crewmate! (Utworzone {Math.Round(br.KillAge / 1000)}s temu)" :
                    $"Crime Scene Report: The killer appears to be a Crewmate! (Created {Math.Round(br.KillAge / 1000)}s ago)";

            else if (br.Killer.Is(Faction.NeutralKilling) || br.Killer.Is(Faction.NeutralBenign))
                return CustomGameOptions.PolishTranslations ? $"Raport: Zabojca zdaje sie byc Rola Neutralna (Utworzone {Math.Round(br.KillAge / 1000)}s temu)" :
                    $"Crime Scene Report: The killer appears to be a Neutral Role! (Created {Math.Round(br.KillAge / 1000)}s ago)";

            else
                return CustomGameOptions.PolishTranslations ? $"Raport: Cialo na miejscu zbrodni zdaje sie nalezec do Impostor (Utworzone {Math.Round(br.KillAge / 1000)}s temu)" :
                    $"Crime Scene Report: The crime scene body appears to be an Impostor! (Created {Math.Round(br.KillAge / 1000)}s ago)";
        }
    }
}
