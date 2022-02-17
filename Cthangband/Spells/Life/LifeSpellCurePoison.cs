using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellCurePoison : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedPoison(0);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Cure Poison";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 17;
                    ManaCost = 17;
                    BaseFailure = 50;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Priest:
                    Level = 9;
                    ManaCost = 6;
                    BaseFailure = 38;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Paladin:
                    Level = 15;
                    ManaCost = 15;
                    BaseFailure = 50;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 20;
                    ManaCost = 20;
                    BaseFailure = 50;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.HighMage:
                    Level = 15;
                    ManaCost = 14;
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
            return string.Empty;
        }
    }
}