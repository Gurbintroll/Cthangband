using Cthangband.Enumerations;
using Cthangband.StaticData;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellSatisfyHunger : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetFood(Constants.PyFoodMax - 1);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Satisfy Hunger";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 15;
                    ManaCost = 14;
                    BaseFailure = 45;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.Priest:
                    Level = 7;
                    ManaCost = 5;
                    BaseFailure = 38;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Paladin:
                    Level = 13;
                    ManaCost = 10;
                    BaseFailure = 45;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 16;
                    ManaCost = 16;
                    BaseFailure = 45;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.HighMage:
                    Level = 12;
                    ManaCost = 10;
                    BaseFailure = 35;
                    FirstCastExperience = 3;
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