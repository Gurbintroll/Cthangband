using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellDetectUnlife : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DetectMonstersNonliving();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Detect Unlife";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 1;
                    ManaCost = 1;
                    BaseFailure = 25;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Priest:
                    Level = 1;
                    ManaCost = 1;
                    BaseFailure = 25;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Rogue:
                    Level = 5;
                    ManaCost = 3;
                    BaseFailure = 40;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.Ranger:
                    Level = 5;
                    ManaCost = 2;
                    BaseFailure = 40;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.Paladin:
                    Level = 1;
                    ManaCost = 1;
                    BaseFailure = 24;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 1;
                    ManaCost = 1;
                    BaseFailure = 25;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.HighMage:
                    Level = 1;
                    ManaCost = 1;
                    BaseFailure = 20;
                    FirstCastExperience = 4;
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
            return string.Empty;
        }
    }
}