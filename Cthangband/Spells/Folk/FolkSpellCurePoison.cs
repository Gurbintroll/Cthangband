using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellCurePoison : Spell
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
                    Level = 11;
                    ManaCost = 10;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Priest:
                    Level = 13;
                    ManaCost = 12;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Rogue:
                    Level = 15;
                    ManaCost = 14;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Ranger:
                    Level = 15;
                    ManaCost = 14;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 14;
                    ManaCost = 13;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.HighMage:
                    Level = 10;
                    ManaCost = 9;
                    BaseFailure = 40;
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
            return string.Empty;
        }
    }
}