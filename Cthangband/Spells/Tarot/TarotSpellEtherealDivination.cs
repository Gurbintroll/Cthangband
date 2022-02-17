using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Tarot
{
    [Serializable]
    internal class TarotSpellEtherealDivination : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DetectAll();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Ethereal Divination";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 30;
                    ManaCost = 30;
                    BaseFailure = 60;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Priest:
                case CharacterClass.Monk:
                    Level = 32;
                    ManaCost = 30;
                    BaseFailure = 60;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Rogue:
                    Level = 35;
                    ManaCost = 30;
                    BaseFailure = 60;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Ranger:
                    Level = 35;
                    ManaCost = 33;
                    BaseFailure = 60;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 33;
                    ManaCost = 30;
                    BaseFailure = 60;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.HighMage:
                    Level = 25;
                    ManaCost = 25;
                    BaseFailure = 50;
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
            return string.Empty;
        }
    }
}