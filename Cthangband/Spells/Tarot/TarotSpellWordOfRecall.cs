using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Tarot
{
    [Serializable]
    internal class TarotSpellWordOfRecall : Spell
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
                    Level = 40;
                    ManaCost = 35;
                    BaseFailure = 80;
                    FirstCastExperience = 15;
                    break;

                case CharacterClass.Priest:
                case CharacterClass.Monk:
                    Level = 42;
                    ManaCost = 40;
                    BaseFailure = 80;
                    FirstCastExperience = 15;
                    break;

                case CharacterClass.Rogue:
                    Level = 46;
                    ManaCost = 44;
                    BaseFailure = 80;
                    FirstCastExperience = 15;
                    break;

                case CharacterClass.Ranger:
                    Level = 45;
                    ManaCost = 42;
                    BaseFailure = 80;
                    FirstCastExperience = 15;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 44;
                    ManaCost = 42;
                    BaseFailure = 80;
                    FirstCastExperience = 15;
                    break;

                case CharacterClass.HighMage:
                    Level = 35;
                    ManaCost = 30;
                    BaseFailure = 70;
                    FirstCastExperience = 15;
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