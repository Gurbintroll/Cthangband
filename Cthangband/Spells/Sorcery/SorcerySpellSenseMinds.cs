using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Sorcery
{
    [Serializable]
    internal class SorcerySpellSenseMinds : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedTelepathy(player.TimedTelepathy + Program.Rng.DieRoll(30) + 25);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Sense Minds";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 14;
                    ManaCost = 10;
                    BaseFailure = 60;
                    FirstCastExperience = 25;
                    break;

                case CharacterClass.Rogue:
                    Level = 16;
                    ManaCost = 10;
                    BaseFailure = 60;
                    FirstCastExperience = 10;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 16;
                    ManaCost = 14;
                    BaseFailure = 60;
                    FirstCastExperience = 25;
                    break;

                case CharacterClass.HighMage:
                    Level = 12;
                    ManaCost = 9;
                    BaseFailure = 50;
                    FirstCastExperience = 25;
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
            return "dur 25+d30";
        }
    }
}