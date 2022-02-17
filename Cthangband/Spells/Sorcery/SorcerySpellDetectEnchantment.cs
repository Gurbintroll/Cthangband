using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Sorcery
{
    [Serializable]
    internal class SorcerySpellDetectEnchantment : Spell
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
                    Level = 10;
                    ManaCost = 10;
                    BaseFailure = 70;
                    FirstCastExperience = 40;
                    break;

                case CharacterClass.Rogue:
                    Level = 13;
                    ManaCost = 10;
                    BaseFailure = 70;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 12;
                    ManaCost = 12;
                    BaseFailure = 70;
                    FirstCastExperience = 40;
                    break;

                case CharacterClass.HighMage:
                    Level = 7;
                    ManaCost = 7;
                    BaseFailure = 60;
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