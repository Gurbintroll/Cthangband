using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Corporeal
{
    [Serializable]
    internal class CorporealSpellMindVision : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedTelepathy(player.TimedTelepathy + Program.Rng.DieRoll(30) + 25);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Mind Vision";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 5;
                    ManaCost = 5;
                    BaseFailure = 35;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Priest:
                    Level = 7;
                    ManaCost = 6;
                    BaseFailure = 35;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Ranger:
                    Level = 11;
                    ManaCost = 8;
                    BaseFailure = 45;
                    FirstCastExperience = 2;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                case CharacterClass.Cultist:
                    Level = 6;
                    ManaCost = 6;
                    BaseFailure = 35;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.HighMage:
                    Level = 4;
                    ManaCost = 3;
                    BaseFailure = 25;
                    FirstCastExperience = 5;
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