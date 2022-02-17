using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Corporeal
{
    [Serializable]
    internal class CorporealSpellWraithform : Spell
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
                    Level = 25;
                    ManaCost = 25;
                    BaseFailure = 75;
                    FirstCastExperience = 70;
                    break;

                case CharacterClass.Priest:
                    Level = 24;
                    ManaCost = 24;
                    BaseFailure = 75;
                    FirstCastExperience = 70;
                    break;

                case CharacterClass.Ranger:
                    Level = 27;
                    ManaCost = 27;
                    BaseFailure = 70;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                case CharacterClass.Cultist:
                    Level = 25;
                    ManaCost = 25;
                    BaseFailure = 75;
                    FirstCastExperience = 70;
                    break;

                case CharacterClass.HighMage:
                    Level = 20;
                    ManaCost = 20;
                    BaseFailure = 65;
                    FirstCastExperience = 70;
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