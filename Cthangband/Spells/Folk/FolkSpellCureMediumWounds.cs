using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellCureMediumWounds : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.RestoreHealth(Program.Rng.DiceRoll(4, 8));
            player.SetTimedBleeding((player.TimedBleeding / 2) - 50);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Cure Medium Wounds";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 16;
                    ManaCost = 14;
                    BaseFailure = 33;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Priest:
                    Level = 18;
                    ManaCost = 17;
                    BaseFailure = 33;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Rogue:
                    Level = 20;
                    ManaCost = 19;
                    BaseFailure = 33;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Ranger:
                    Level = 20;
                    ManaCost = 19;
                    BaseFailure = 33;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 19;
                    ManaCost = 18;
                    BaseFailure = 33;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.HighMage:
                    Level = 14;
                    ManaCost = 11;
                    BaseFailure = 22;
                    FirstCastExperience = 6;
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
            return "heal 4d8";
        }
    }
}