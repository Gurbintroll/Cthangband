using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Corporeal
{
    [Serializable]
    internal class CorporealSpellHaste : Spell
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
            Name = "Haste";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 22;
                    ManaCost = 12;
                    BaseFailure = 60;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Priest:
                    Level = 27;
                    ManaCost = 17;
                    BaseFailure = 65;
                    FirstCastExperience = 10;
                    break;

                case CharacterClass.Ranger:
                    Level = 34;
                    ManaCost = 35;
                    BaseFailure = 70;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                case CharacterClass.Cultist:
                    Level = 27;
                    ManaCost = 18;
                    BaseFailure = 60;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.HighMage:
                    Level = 17;
                    ManaCost = 10;
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
            return $"dur {player.Level}+d{20 + player.Level}";
        }
    }
}