using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellTeleportLevel : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.TeleportPlayerLevel();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Teleport Level";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 35;
                    ManaCost = 35;
                    BaseFailure = 80;
                    FirstCastExperience = 25;
                    break;

                case CharacterClass.Priest:
                    Level = 37;
                    ManaCost = 36;
                    BaseFailure = 80;
                    FirstCastExperience = 25;
                    break;

                case CharacterClass.Rogue:
                    Level = 42;
                    ManaCost = 38;
                    BaseFailure = 80;
                    FirstCastExperience = 25;
                    break;

                case CharacterClass.Ranger:
                    Level = 42;
                    ManaCost = 38;
                    BaseFailure = 80;
                    FirstCastExperience = 25;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 39;
                    ManaCost = 38;
                    BaseFailure = 80;
                    FirstCastExperience = 25;
                    break;

                case CharacterClass.HighMage:
                    Level = 30;
                    ManaCost = 30;
                    BaseFailure = 70;
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