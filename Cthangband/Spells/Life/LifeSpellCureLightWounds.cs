using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellCureLightWounds : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.RestoreHealth(Program.Rng.DiceRoll(2, 10));
            player.SetTimedBleeding(player.TimedBleeding - 10);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Cure Light Wounds";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 3;
                    ManaCost = 2;
                    BaseFailure = 35;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Priest:
                    Level = 1;
                    ManaCost = 2;
                    BaseFailure = 15;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Paladin:
                    Level = 2;
                    ManaCost = 2;
                    BaseFailure = 35;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 4;
                    ManaCost = 4;
                    BaseFailure = 35;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.HighMage:
                    Level = 2;
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
            return "heal 2d10";
        }
    }
}