using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Tarot
{
    [Serializable]
    internal class TarotSpellAstralSpying : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedTelepathy(player.TimedTelepathy + Program.Rng.DieRoll(30) + 25);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Astral Spying";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 14;
                    ManaCost = 12;
                    BaseFailure = 60;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Priest:
                case CharacterClass.Monk:
                    Level = 17;
                    ManaCost = 14;
                    BaseFailure = 60;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Rogue:
                    Level = 19;
                    ManaCost = 15;
                    BaseFailure = 60;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Ranger:
                    Level = 20;
                    ManaCost = 17;
                    BaseFailure = 60;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 18;
                    ManaCost = 15;
                    BaseFailure = 60;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.HighMage:
                    Level = 10;
                    ManaCost = 10;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
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