using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Sorcery
{
    [Serializable]
    internal class SorcerySpellEnchantWeapon : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.EnchantSpell(Program.Rng.RandomLessThan(4) + 1, Program.Rng.RandomLessThan(4) + 1, 0);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Enchant Weapon";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 40;
                    ManaCost = 80;
                    BaseFailure = 95;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.Rogue:
                    Level = 43;
                    ManaCost = 80;
                    BaseFailure = 80;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 42;
                    ManaCost = 85;
                    BaseFailure = 95;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.HighMage:
                    Level = 30;
                    ManaCost = 65;
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