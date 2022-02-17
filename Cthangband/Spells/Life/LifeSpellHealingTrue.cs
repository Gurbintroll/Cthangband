using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellHealingTrue : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.RestoreHealth(2000);
            player.SetTimedStun(0);
            player.SetTimedBleeding(0);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Healing True";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 45;
                    ManaCost = 90;
                    BaseFailure = 80;
                    FirstCastExperience = 115;
                    break;

                case CharacterClass.Priest:
                    Level = 40;
                    ManaCost = 50;
                    BaseFailure = 80;
                    FirstCastExperience = 130;
                    break;

                case CharacterClass.Paladin:
                    Level = 45;
                    ManaCost = 80;
                    BaseFailure = 80;
                    FirstCastExperience = 115;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 46;
                    ManaCost = 90;
                    BaseFailure = 80;
                    FirstCastExperience = 115;
                    break;

                case CharacterClass.HighMage:
                    Level = 42;
                    ManaCost = 75;
                    BaseFailure = 60;
                    FirstCastExperience = 115;
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
            return "heal 2000";
        }
    }
}