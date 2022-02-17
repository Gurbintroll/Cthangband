using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellCureLightWounds : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.RestoreHealth(Program.Rng.DiceRoll(2, 8));
            player.SetTimedBleeding(player.TimedBleeding - 10);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Cure Light Wounds";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 6;
                    ManaCost = 5;
                    BaseFailure = 44;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Priest:
                    Level = 7;
                    ManaCost = 6;
                    BaseFailure = 44;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Rogue:
                    Level = 9;
                    ManaCost = 8;
                    BaseFailure = 44;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Ranger:
                    Level = 8;
                    ManaCost = 8;
                    BaseFailure = 44;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 7;
                    ManaCost = 7;
                    BaseFailure = 44;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.HighMage:
                    Level = 5;
                    ManaCost = 4;
                    BaseFailure = 33;
                    FirstCastExperience = 5;
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