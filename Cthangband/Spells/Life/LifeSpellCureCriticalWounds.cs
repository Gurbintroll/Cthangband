using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellCureCriticalWounds : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.RestoreHealth(Program.Rng.DiceRoll(8, 10));
            player.SetTimedStun(0);
            player.SetTimedBleeding(0);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Cure Critical Wounds";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 18;
                    ManaCost = 18;
                    BaseFailure = 50;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Priest:
                    Level = 9;
                    ManaCost = 7;
                    BaseFailure = 40;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Paladin:
                    Level = 17;
                    ManaCost = 15;
                    BaseFailure = 50;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 22;
                    ManaCost = 22;
                    BaseFailure = 50;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.HighMage:
                    Level = 15;
                    ManaCost = 15;
                    BaseFailure = 40;
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
            return "heal 8d10";
        }
    }
}