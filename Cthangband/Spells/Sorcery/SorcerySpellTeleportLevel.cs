using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Sorcery
{
    [Serializable]
    internal class SorcerySpellTeleportLevel : Spell
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
                    Level = 20;
                    ManaCost = 18;
                    BaseFailure = 60;
                    FirstCastExperience = 25;
                    break;

                case CharacterClass.Rogue:
                    Level = 18;
                    ManaCost = 17;
                    BaseFailure = 60;
                    FirstCastExperience = 30;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 24;
                    ManaCost = 22;
                    BaseFailure = 60;
                    FirstCastExperience = 25;
                    break;

                case CharacterClass.HighMage:
                    Level = 17;
                    ManaCost = 12;
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