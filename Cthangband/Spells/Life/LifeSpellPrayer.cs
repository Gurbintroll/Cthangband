using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellPrayer : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedBlessing(player.TimedBlessing + Program.Rng.DieRoll(48) + 48);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Prayer";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 25;
                    ManaCost = 25;
                    BaseFailure = 50;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Priest:
                    Level = 15;
                    ManaCost = 14;
                    BaseFailure = 50;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.Paladin:
                    Level = 25;
                    ManaCost = 20;
                    BaseFailure = 50;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 28;
                    ManaCost = 28;
                    BaseFailure = 50;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.HighMage:
                    Level = 20;
                    ManaCost = 20;
                    BaseFailure = 40;
                    FirstCastExperience = 40;
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
            return "dur 48+d48";
        }
    }
}