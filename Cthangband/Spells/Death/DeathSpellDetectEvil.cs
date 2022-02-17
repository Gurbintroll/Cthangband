using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellDetectEvil : Spell
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
                    Level = 2;
                    ManaCost = 2;
                    BaseFailure = 25;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Priest:
                    Level = 3;
                    ManaCost = 3;
                    BaseFailure = 25;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Rogue:
                    Level = 9;
                    ManaCost = 5;
                    BaseFailure = 50;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.Ranger:
                    Level = 7;
                    ManaCost = 4;
                    BaseFailure = 50;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.Paladin:
                    Level = 4;
                    ManaCost = 3;
                    BaseFailure = 25;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 3;
                    ManaCost = 3;
                    BaseFailure = 25;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.HighMage:
                    Level = 2;
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