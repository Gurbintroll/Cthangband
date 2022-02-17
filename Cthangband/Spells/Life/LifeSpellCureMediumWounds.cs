using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellCureMediumWounds : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.RestoreHealth(Program.Rng.DiceRoll(4, 10));
            player.SetTimedBleeding((player.TimedBleeding / 2) - 20);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Cure Medium Wounds";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 12;
                    ManaCost = 12;
                    BaseFailure = 40;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.Priest:
                    Level = 5;
                    ManaCost = 4;
                    BaseFailure = 32;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Paladin:
                    Level = 11;
                    ManaCost = 9;
                    BaseFailure = 40;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 14;
                    ManaCost = 14;
                    BaseFailure = 40;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.HighMage:
                    Level = 9;
                    ManaCost = 9;
                    BaseFailure = 30;
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
            return "heal 4d10";
        }
    }
}