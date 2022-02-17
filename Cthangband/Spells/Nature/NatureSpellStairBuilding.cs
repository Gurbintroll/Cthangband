using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellStairBuilding : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.StairCreation();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Stair Building";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 9;
                    ManaCost = 12;
                    BaseFailure = 40;
                    FirstCastExperience = 44;
                    break;

                case CharacterClass.Priest:
                    Level = 11;
                    ManaCost = 12;
                    BaseFailure = 40;
                    FirstCastExperience = 44;
                    break;

                case CharacterClass.Ranger:
                    Level = 12;
                    ManaCost = 12;
                    BaseFailure = 50;
                    FirstCastExperience = 25;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 12;
                    ManaCost = 12;
                    BaseFailure = 40;
                    FirstCastExperience = 44;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 7;
                    ManaCost = 7;
                    BaseFailure = 20;
                    FirstCastExperience = 44;
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