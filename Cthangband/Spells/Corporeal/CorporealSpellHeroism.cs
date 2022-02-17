using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Corporeal
{
    [Serializable]
    internal class CorporealSpellHeroism : Spell
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
                    Level = 10;
                    ManaCost = 10;
                    BaseFailure = 40;
                    FirstCastExperience = 20;
                    break;

                case CharacterClass.Priest:
                    Level = 13;
                    ManaCost = 13;
                    BaseFailure = 40;
                    FirstCastExperience = 20;
                    break;

                case CharacterClass.Ranger:
                    Level = 20;
                    ManaCost = 20;
                    BaseFailure = 45;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                case CharacterClass.Cultist:
                    Level = 12;
                    ManaCost = 12;
                    BaseFailure = 40;
                    FirstCastExperience = 20;
                    break;

                case CharacterClass.HighMage:
                    Level = 8;
                    ManaCost = 8;
                    BaseFailure = 30;
                    FirstCastExperience = 20;
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
            return "dur 25+d25";
        }
    }
}