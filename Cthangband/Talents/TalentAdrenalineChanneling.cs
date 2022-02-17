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