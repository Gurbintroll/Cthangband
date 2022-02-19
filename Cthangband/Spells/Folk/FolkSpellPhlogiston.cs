using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellPhlogiston : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.CommandEngine.CreatePhlogiston();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Phlogiston";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 8;
                    ManaCost = 8;
                    BaseFailure = 60;
                    FirstCastExperience = 7;
                    break;

                case CharacterClass.Priest:
                    Level = 9;
                    ManaCost = 8;
                    BaseFailure = 60;
                    FirstCastExperience = 7;
                    break;

                case CharacterClass.Rogue:
                    Level = 10;
                    ManaCost = 10;
                    BaseFailure = 60;
                    FirstCastExperience = 7;
                    break;

                case CharacterClass.Ranger:
                    Level = 10;
                    ManaCost = 10;
                    BaseFailure = 60;
                    FirstCastExperience = 7;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 9;
                    ManaCost = 9;
                    BaseFailure = 60;
                    FirstCastExperience = 7;
                    break;

                case CharacterClass.HighMage:
                    Level = 7;
                    ManaCost = 7;
                    BaseFailure = 50;
                    FirstCastExperience = 7;
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