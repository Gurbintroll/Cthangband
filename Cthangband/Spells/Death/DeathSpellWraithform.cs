using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellWraithform : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedEtherealness(player.TimedEtherealness + Program.Rng.DieRoll(player.Level / 2) + (player.Level / 2));
        }

        public override void Initialise(int characterClass)
        {
            Name = "Wraithform";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 47;
                    ManaCost = 100;
                    BaseFailure = 90;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Priest:
                    Level = 50;
                    ManaCost = 111;
                    BaseFailure = 90;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Rogue:
                    Level = 50;
                    ManaCost = 125;
                    BaseFailure = 90;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Ranger:
                    Level = 99;
                    ManaCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;

                case CharacterClass.Paladin:
                    Level = 50;
                    ManaCost = 111;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 50;
                    ManaCost = 123;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.HighMage:
                    Level = 44;
                    ManaCost = 75;
                    BaseFailure = 80;
                    FirstCastExperience = 250;
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
            return $"dur {player.Level / 2}+d{player.Level / 2}";
        }
    }
}