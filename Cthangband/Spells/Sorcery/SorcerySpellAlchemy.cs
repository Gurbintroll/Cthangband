using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Sorcery
{
    [Serializable]
    internal class SorcerySpellAlchemy : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.Alchemy();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Alchemy";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 42;
                    ManaCost = 50;
                    BaseFailure = 90;
                    FirstCastExperience = 175;
                    break;

                case CharacterClass.Rogue:
                    Level = 45;
                    ManaCost = 50;
                    BaseFailure = 70;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 46;
                    ManaCost = 55;
                    BaseFailure = 90;
                    FirstCastExperience = 175;
                    break;

                case CharacterClass.HighMage:
                    Level = 40;
                    ManaCost = 40;
                    BaseFailure = 80;
                    FirstCastExperience = 175;
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