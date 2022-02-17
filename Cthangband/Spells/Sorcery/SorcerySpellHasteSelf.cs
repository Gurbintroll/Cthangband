using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Sorcery
{
    [Serializable]
    internal class SorcerySpellHasteSelf : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            if (player.TimedHaste == 0)
            {
                player.SetTimedHaste(Program.Rng.DieRoll(20 + player.Level) + player.Level);
            }
            else
            {
                player.SetTimedHaste(player.TimedHaste + Program.Rng.DieRoll(5));
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Haste Self";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 18;
                    ManaCost = 12;
                    BaseFailure = 60;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Rogue:
                    Level = 31;
                    ManaCost = 23;
                    BaseFailure = 80;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 20;
                    ManaCost = 15;
                    BaseFailure = 60;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.HighMage:
                    Level = 13;
                    ManaCost = 8;
                    BaseFailure = 50;
                    FirstCastExperience = 8;
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
            return $"dur {player.Level}+d{player.Level + 20}";
        }
    }
}