using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellResistFire : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedFireResistance(player.TimedColdResistance + Program.Rng.DieRoll(20) + 20);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Resist Fire";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 13;
                    ManaCost = 12;
                    BaseFailure = 50;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Priest:
                    Level = 15;
                    ManaCost = 14;
                    BaseFailure = 50;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Rogue:
                    Level = 17;
                    ManaCost = 16;
                    BaseFailure = 50;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Ranger:
                    Level = 17;
                    ManaCost = 16;
                    BaseFailure = 50;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 16;
                    ManaCost = 15;
                    BaseFailure = 50;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.HighMage:
                    Level = 11;
                    ManaCost = 10;
                    BaseFailure = 40;
                    FirstCastExperience = 5;
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
            return "dur 20+d20";
        }
    }
}