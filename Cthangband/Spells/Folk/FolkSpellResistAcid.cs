using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellResistAcid : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedAcidResistance(player.TimedColdResistance + Program.Rng.DieRoll(20) + 20);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Resist Acid";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 15;
                    ManaCost = 12;
                    BaseFailure = 50;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Priest:
                    Level = 17;
                    ManaCost = 16;
                    BaseFailure = 50;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Rogue:
                    Level = 19;
                    ManaCost = 18;
                    BaseFailure = 50;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Ranger:
                    Level = 19;
                    ManaCost = 18;
                    BaseFailure = 50;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 18;
                    ManaCost = 17;
                    BaseFailure = 50;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.HighMage:
                    Level = 13;
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