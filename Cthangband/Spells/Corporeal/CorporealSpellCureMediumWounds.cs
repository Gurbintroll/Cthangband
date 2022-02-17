using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Corporeal
{
    [Serializable]
    internal class CorporealSpellCureMediumWounds : Spell
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
                    Level = 6;
                    ManaCost = 5;
                    BaseFailure = 30;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Priest:
                    Level = 9;
                    ManaCost = 7;
                    BaseFailure = 30;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Ranger:
                    Level = 13;
                    ManaCost = 8;
                    BaseFailure = 40;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                case CharacterClass.Cultist:
                    Level = 7;
                    ManaCost = 7;
                    BaseFailure = 30;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.HighMage:
                    Level = 5;
                    ManaCost = 4;
                    BaseFailure = 20;
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
            return "heal 4d10";
        }
    }
}