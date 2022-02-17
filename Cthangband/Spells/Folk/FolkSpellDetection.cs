using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellDetection : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DetectAll();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Detection";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 42;
                    ManaCost = 30;
                    BaseFailure = 80;
                    FirstCastExperience = 40;
                    break;

                case CharacterClass.Priest:
                    Level = 46;
                    ManaCost = 40;
                    BaseFailure = 80;
                    FirstCastExperience = 40;
                    break;

                case CharacterClass.Rogue:
                    Level = 48;
                    ManaCost = 44;
                    BaseFailure = 80;
                    FirstCastExperience = 40;
                    break;

                case CharacterClass.Ranger:
                    Level = 48;
                    ManaCost = 44;
                    BaseFailure = 80;
                    FirstCastExperience = 40;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 47;
                    ManaCost = 45;
                    BaseFailure = 80;
                    FirstCastExperience = 40;
                    break;

                case CharacterClass.HighMage:
                    Level = 41;
                    ManaCost = 28;
                    BaseFailure = 70;
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
            return string.Empty;
        }
    }
}