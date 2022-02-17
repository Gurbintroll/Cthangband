using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellClairvoyance : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            level.WizLight();
            if (!player.HasTelepathy)
            {
                player.SetTimedTelepathy(player.TimedTelepathy + Program.Rng.DieRoll(30) + 25);
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Clairvoyance";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 49;
                    ManaCost = 100;
                    BaseFailure = 80;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.Priest:
                    Level = 50;
                    ManaCost = 120;
                    BaseFailure = 80;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.Rogue:
                    Level = 99;
                    ManaCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;

                case CharacterClass.Ranger:
                    Level = 99;
                    ManaCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 50;
                    ManaCost = 140;
                    BaseFailure = 80;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.HighMage:
                    Level = 46;
                    ManaCost = 80;
                    BaseFailure = 70;
                    FirstCastExperience = 200;
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