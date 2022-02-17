using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellWordOfRecall : Spell
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
                    Level = 45;
                    ManaCost = 50;
                    BaseFailure = 70;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Priest:
                    Level = 47;
                    ManaCost = 55;
                    BaseFailure = 70;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Rogue:
                    Level = 49;
                    ManaCost = 65;
                    BaseFailure = 70;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Ranger:
                    Level = 49;
                    ManaCost = 65;
                    BaseFailure = 70;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 48;
                    ManaCost = 65;
                    BaseFailure = 70;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.HighMage:
                    Level = 43;
                    ManaCost = 40;
                    BaseFailure = 60;
                    FirstCastExperience = 50;
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