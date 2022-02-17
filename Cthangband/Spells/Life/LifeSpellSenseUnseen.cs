using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellSenseUnseen : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedSeeInvisibility(player.TimedSeeInvisibility + Program.Rng.DieRoll(24) + 24);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Sense Unseen";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 19;
                    ManaCost = 19;
                    BaseFailure = 50;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Priest:
                    Level = 10;
                    ManaCost = 8;
                    BaseFailure = 38;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Paladin:
                    Level = 18;
                    ManaCost = 15;
                    BaseFailure = 50;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 24;
                    ManaCost = 24;
                    BaseFailure = 50;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.HighMage:
                    Level = 17;
                    ManaCost = 15;
                    BaseFailure = 40;
                    FirstCastExperience = 4;
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