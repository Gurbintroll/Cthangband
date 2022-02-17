using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellHeroism : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedHeroism(player.TimedHeroism + Program.Rng.DieRoll(25) + 25);
            player.RestoreHealth(10);
            player.SetTimedFear(0);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Heroism";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 9;
                    ManaCost = 9;
                    BaseFailure = 50;
                    FirstCastExperience = 40;
                    break;

                case CharacterClass.Priest:
                    Level = 5;
                    ManaCost = 5;
                    BaseFailure = 80;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Paladin:
                    Level = 9;
                    ManaCost = 9;
                    BaseFailure = 50;
                    FirstCastExperience = 40;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 10;
                    ManaCost = 10;
                    BaseFailure = 50;
                    FirstCastExperience = 40;
                    break;

                case CharacterClass.HighMage:
                    Level = 5;
                    ManaCost = 5;
                    BaseFailure = 50;
                    FirstCastExperience = 40;
                    break;

                default:
                    Level = 99;
                    ManaCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;
            }
        }

        protected override string Comment(Player player)
        {
            return "dur 25 + d25";
        }
    }
}