using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Sorcery
{
    [Serializable]
    internal class SorcerySpellMagicMapping : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            level.MapArea();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Magic Mapping";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 9;
                    ManaCost = 7;
                    BaseFailure = 75;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Rogue:
                    Level = 25;
                    ManaCost = 14;
                    BaseFailure = 80;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 10;
                    ManaCost = 9;
                    BaseFailure = 75;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.HighMage:
                    Level = 7;
                    ManaCost = 5;
                    BaseFailure = 65;
                    FirstCastExperience = 8;
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