using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellDetectEvil : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DetectMonstersEvil();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Detect Evil";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 1;
                    ManaCost = 1;
                    BaseFailure = 30;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Priest:
                    Level = 1;
                    ManaCost = 1;
                    BaseFailure = 10;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Paladin:
                    Level = 1;
                    ManaCost = 1;
                    BaseFailure = 30;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 2;
                    ManaCost = 2;
                    BaseFailure = 30;
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