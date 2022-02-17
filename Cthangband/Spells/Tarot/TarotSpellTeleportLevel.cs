using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Tarot
{
    [Serializable]
    internal class TarotSpellTeleportLevel : Spell
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
                    ManaCost = 30;
                    BaseFailure = 70;
                    FirstCastExperience = 10;
                    break;

                case CharacterClass.Priest:
                case CharacterClass.Monk:
                    Level = 38;
                    ManaCost = 35;
                    BaseFailure = 70;
                    FirstCastExperience = 10;
                    break;

                case CharacterClass.Rogue:
                    Level = 42;
                    ManaCost = 38;
                    BaseFailure = 70;
                    FirstCastExperience = 10;
                    break;

                case CharacterClass.Ranger:
                    Level = 42;
                    ManaCost = 38;
                    BaseFailure = 70;
                    FirstCastExperience = 10;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 40;
                    ManaCost = 38;
                    BaseFailure = 70;
                    FirstCastExperience = 10;
                    break;

                case CharacterClass.HighMage:
                    Level = 30;
                    ManaCost = 28;
                    BaseFailure = 60;
                    FirstCastExperience = 10;
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