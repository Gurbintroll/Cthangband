using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellHerbalHealing : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.RestoreHealth(1000);
            player.SetTimedStun(0);
            player.SetTimedBleeding(0);
            player.SetTimedPoison(0);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Herbal Healing";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 40;
                    ManaCost = 100;
                    BaseFailure = 95;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Priest:
                    Level = 42;
                    ManaCost = 100;
                    BaseFailure = 95;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Ranger:
                    Level = 40;
                    ManaCost = 100;
                    BaseFailure = 95;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 45;
                    ManaCost = 100;
                    BaseFailure = 95;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 35;
                    ManaCost = 80;
                    BaseFailure = 85;
                    FirstCastExperience = 50;
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
            return "heal 1000";
        }
    }
}