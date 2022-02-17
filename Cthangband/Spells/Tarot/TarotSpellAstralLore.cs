using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Tarot
{
    [Serializable]
    internal class TarotSpellAstralLore : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.IdentifyFully();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Astral Lore";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 35;
                    ManaCost = 50;
                    BaseFailure = 90;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.Priest:
                case CharacterClass.Monk:
                    Level = 38;
                    ManaCost = 55;
                    BaseFailure = 90;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.Rogue:
                    Level = 42;
                    ManaCost = 65;
                    BaseFailure = 90;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.Ranger:
                    Level = 40;
                    ManaCost = 65;
                    BaseFailure = 90;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 40;
                    ManaCost = 60;
                    BaseFailure = 90;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.HighMage:
                    Level = 32;
                    ManaCost = 45;
                    BaseFailure = 80;
                    FirstCastExperience = 100;
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