using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellSeeInvisible : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedSeeInvisibility(player.TimedSeeInvisibility + Program.Rng.DieRoll(24) + 24);
        }

        public override void Initialise(int characterClass)
        {
            Name = "See Invisible";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 25;
                    ManaCost = 20;
                    BaseFailure = 60;
                    FirstCastExperience = 13;
                    break;

                case CharacterClass.Priest:
                    Level = 29;
                    ManaCost = 26;
                    BaseFailure = 60;
                    FirstCastExperience = 13;
                    break;

                case CharacterClass.Rogue:
                    Level = 33;
                    ManaCost = 30;
                    BaseFailure = 60;
                    FirstCastExperience = 13;
                    break;

                case CharacterClass.Ranger:
                    Level = 33;
                    ManaCost = 30;
                    BaseFailure = 60;
                    FirstCastExperience = 13;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 30;
                    ManaCost = 27;
                    BaseFailure = 60;
                    FirstCastExperience = 13;
                    break;

                case CharacterClass.HighMage:
                    Level = 22;
                    ManaCost = 18;
                    BaseFailure = 50;
                    FirstCastExperience = 13;
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
            return "dur 24+d24";
        }
    }
}