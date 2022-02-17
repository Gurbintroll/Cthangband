using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Sorcery
{
    [Serializable]
    internal class SorcerySpellClairvoyance : Spell
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
                    Level = 30;
                    ManaCost = 40;
                    BaseFailure = 80;
                    FirstCastExperience = 120;
                    break;

                case CharacterClass.Rogue:
                    Level = 37;
                    ManaCost = 40;
                    BaseFailure = 80;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 35;
                    ManaCost = 45;
                    BaseFailure = 80;
                    FirstCastExperience = 120;
                    break;

                case CharacterClass.HighMage:
                    Level = 25;
                    ManaCost = 30;
                    BaseFailure = 70;
                    FirstCastExperience = 120;
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