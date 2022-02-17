using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Sorcery
{
    [Serializable]
    internal class SorcerySpellEnchantArmour : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.EnchantSpell(0, 0, Program.Rng.RandomLessThan(3) + 2);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Enchant Armour";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 40;
                    ManaCost = 100;
                    BaseFailure = 95;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.Rogue:
                    Level = 44;
                    ManaCost = 100;
                    BaseFailure = 80;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 45;
                    ManaCost = 100;
                    BaseFailure = 95;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.HighMage:
                    Level = 35;
                    ManaCost = 80;
                    BaseFailure = 85;
                    FirstCastExperience = 200;
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