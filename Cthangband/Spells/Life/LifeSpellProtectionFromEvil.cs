using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellProtectionFromEvil : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedProtectionFromEvil(player.TimedProtectionFromEvil + Program.Rng.DieRoll(25) + (3 * player.Level));
        }

        public override void Initialise(int characterClass)
        {
            Name = "Protection from Evil";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 23;
                    ManaCost = 23;
                    BaseFailure = 50;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Priest:
                    Level = 11;
                    ManaCost = 8;
                    BaseFailure = 42;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Paladin:
                    Level = 19;
                    ManaCost = 15;
                    BaseFailure = 50;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 28;
                    ManaCost = 28;
                    BaseFailure = 50;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.HighMage:
                    Level = 21;
                    ManaCost = 19;
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
            return $"dur d25+{3 * player.Level}";
        }
    }
}