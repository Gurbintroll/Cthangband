using Cthangband.Enumerations;
using Cthangband.StaticData;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellForaging : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetFood(Constants.PyFoodMax - 1);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Foraging";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 4;
                    ManaCost = 4;
                    BaseFailure = 35;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Priest:
                    Level = 6;
                    ManaCost = 5;
                    BaseFailure = 35;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Ranger:
                    Level = 5;
                    ManaCost = 7;
                    BaseFailure = 55;
                    FirstCastExperience = 2;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 5;
                    ManaCost = 5;
                    BaseFailure = 35;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 3;
                    ManaCost = 2;
                    BaseFailure = 25;
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