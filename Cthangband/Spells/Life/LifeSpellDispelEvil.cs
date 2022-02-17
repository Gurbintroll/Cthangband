using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellDispelEvil : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DispelEvil(player.Level * 4);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Dispel Evil";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 35;
                    ManaCost = 35;
                    BaseFailure = 70;
                    FirstCastExperience = 75;
                    break;

                case CharacterClass.Priest:
                    Level = 25;
                    ManaCost = 20;
                    BaseFailure = 70;
                    FirstCastExperience = 120;
                    break;

                case CharacterClass.Paladin:
                    Level = 35;
                    ManaCost = 32;
                    BaseFailure = 70;
                    FirstCastExperience = 75;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 38;
                    ManaCost = 38;
                    BaseFailure = 70;
                    FirstCastExperience = 75;
                    break;

                case CharacterClass.HighMage:
                    Level = 32;
                    ManaCost = 30;
                    BaseFailure = 60;
                    FirstCastExperience = 75;
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
            return $"dam {4 * player.Level}";
        }
    }
}