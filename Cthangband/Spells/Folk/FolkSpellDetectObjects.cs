using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellDetectObjects : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DetectObjectsNormal();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Detect Objects";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 9;
                    ManaCost = 9;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Priest:
                    Level = 12;
                    ManaCost = 11;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Rogue:
                    Level = 14;
                    ManaCost = 13;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Ranger:
                    Level = 14;
                    ManaCost = 13;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 13;
                    ManaCost = 12;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.HighMage:
                    Level = 9;
                    ManaCost = 8;
                    BaseFailure = 40;
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
            return string.Empty;
        }
    }
}