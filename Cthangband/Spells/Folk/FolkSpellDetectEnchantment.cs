using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellDetectEnchantment : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DetectObjectsMagic();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Detect Enchantment";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 9;
                    ManaCost = 9;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Priest:
                    Level = 11;
                    ManaCost = 10;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Rogue:
                    Level = 12;
                    ManaCost = 12;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Ranger:
                    Level = 12;
                    ManaCost = 12;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 12;
                    ManaCost = 11;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.HighMage:
                    Level = 8;
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