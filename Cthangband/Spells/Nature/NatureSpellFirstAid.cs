using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellFirstAid : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.RestoreHealth(Program.Rng.DiceRoll(2, 8));
            player.SetTimedBleeding(player.TimedBleeding - 15);
        }

        public override void Initialise(int characterClass)
        {
            Name = "First Aid";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 3;
                    ManaCost = 3;
                    BaseFailure = 25;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.Priest:
                    Level = 5;
                    ManaCost = 3;
                    BaseFailure = 25;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Ranger:
                    Level = 4;
                    ManaCost = 3;
                    BaseFailure = 40;
                    FirstCastExperience = 2;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 3;
                    ManaCost = 3;
                    BaseFailure = 25;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 2;
                    ManaCost = 1;
                    BaseFailure = 15;
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
            return "heal 2d8";
        }
    }
}