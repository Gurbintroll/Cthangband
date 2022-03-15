// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using System;

namespace Cthangband.Talents
{
    [Serializable]
    internal class TalentAdrenalineChanneling : Talent
    {
        public override void Initialise(int characterClass)
        {
            Name = "Adrenaline Channeling";
            Level = 23;
            ManaCost = 15;
            BaseFailure = 50;
        }

        public override void Use(Player player, Level level, SaveGame saveGame)
        {
            player.SetTimedFear(0);
            player.SetTimedStun(0);
            player.RestoreHealth(player.Level);
            int i = 10 + Program.Rng.DieRoll(player.Level * 3 / 2);
            if (player.Level < 35)
            {
                player.SetTimedHeroism(player.TimedHeroism + i);
            }
            else
            {
                player.SetTimedSuperheroism(player.TimedSuperheroism + i);
            }
            if (player.TimedHaste == 0)
            {
                player.SetTimedHaste(i);
            }
            else
            {
                player.SetTimedHaste(player.TimedHaste + i);
            }
        }

        protected override string Comment(Player player)
        {
            return $"dur 10+d{player.Level * 3 / 2}";
        }
    }
}