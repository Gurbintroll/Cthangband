using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellIdentify : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.IdentifyItem();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Identify";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 38;
                    ManaCost = 30;
                    BaseFailure = 60;
                    FirstCastExperience = 25;
                    break;

                case CharacterClass.Priest:
                    Level = 40;
                    ManaCost = 37;
                    BaseFailure = 60;
                    FirstCastExperience = 25;
                    break;

                case CharacterClass.Rogue:
                    Level = 44;
                    ManaCost = 38;
                    BaseFailure = 60;
                    FirstCastExperience = 25;
                    break;

                case CharacterClass.Ranger:
                    Level = 44;
                    ManaCost = 38;
                    BaseFailure = 60;
                    FirstCastExperience = 25;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 41;
                    ManaCost = 40;
                    BaseFailure = 60;
                    FirstCastExperience = 25;
                    break;

                case CharacterClass.HighMage:
                    Level = 32;
                    ManaCost = 28;
                    BaseFailure = 50;
                    FirstCastExperience = 25;
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