using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellCureWoundsAndPoison : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedBleeding(0);
            player.SetTimedPoison(0);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Cure Wounds and Poison";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 5;
                    ManaCost = 5;
                    BaseFailure = 35;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Priest:
                    Level = 7;
                    ManaCost = 7;
                    BaseFailure = 35;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Ranger:
                    Level = 9;
                    ManaCost = 7;
                    BaseFailure = 45;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 9;
                    ManaCost = 9;
                    BaseFailure = 35;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 4;
                    ManaCost = 4;
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