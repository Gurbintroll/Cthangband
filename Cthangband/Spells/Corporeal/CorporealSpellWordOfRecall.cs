using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Corporeal
{
    [Serializable]
    internal class CorporealSpellWordOfRecall : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.ToggleRecall();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Word of Recall";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 25;
                    ManaCost = 25;
                    BaseFailure = 75;
                    FirstCastExperience = 19;
                    break;

                case CharacterClass.Priest:
                    Level = 27;
                    ManaCost = 27;
                    BaseFailure = 75;
                    FirstCastExperience = 19;
                    break;

                case CharacterClass.Ranger:
                    Level = 35;
                    ManaCost = 35;
                    BaseFailure = 75;
                    FirstCastExperience = 13;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                case CharacterClass.Cultist:
                    Level = 28;
                    ManaCost = 28;
                    BaseFailure = 75;
                    FirstCastExperience = 19;
                    break;

                case CharacterClass.HighMage:
                    Level = 20;
                    ManaCost = 20;
                    BaseFailure = 65;
                    FirstCastExperience = 19;
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
            return "delay 15+d21";
        }
    }
}