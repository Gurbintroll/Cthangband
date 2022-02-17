using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Corporeal
{
    [Serializable]
    internal class CorporealSpellBatsSense : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            level.MapArea();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Bat's Sense";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 3;
                    ManaCost = 3;
                    BaseFailure = 30;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.Priest:
                    Level = 5;
                    ManaCost = 4;
                    BaseFailure = 30;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.Ranger:
                    Level = 7;
                    ManaCost = 4;
                    BaseFailure = 40;
                    FirstCastExperience = 2;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                case CharacterClass.Cultist:
                    Level = 4;
                    ManaCost = 4;
                    BaseFailure = 30;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.HighMage:
                    Level = 2;
                    ManaCost = 2;
                    BaseFailure = 20;
                    FirstCastExperience = 1;
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