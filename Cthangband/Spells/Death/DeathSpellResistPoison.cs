using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellResistPoison : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedPoisonResistance(player.TimedPoisonResistance + Program.Rng.DieRoll(20) + 20);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Resist Poison";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 7;
                    ManaCost = 10;
                    BaseFailure = 75;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Priest:
                    Level = 9;
                    ManaCost = 11;
                    BaseFailure = 75;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Rogue:
                    Level = 17;
                    ManaCost = 15;
                    BaseFailure = 80;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.Ranger:
                    Level = 17;
                    ManaCost = 25;
                    BaseFailure = 75;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Paladin:
                    Level = 10;
                    ManaCost = 11;
                    BaseFailure = 75;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 9;
                    ManaCost = 10;
                    BaseFailure = 75;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.HighMage:
                    Level = 5;
                    ManaCost = 9;
                    BaseFailure = 55;
                    FirstCastExperience = 6;
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