using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellDetectTreasure : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DetectTreasure();
            saveGame.SpellEffects.DetectObjectsGold();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Detect Treasure";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 9;
                    ManaCost = 8;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Priest:
                    Level = 10;
                    ManaCost = 9;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Rogue:
                    Level = 11;
                    ManaCost = 11;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Ranger:
                    Level = 11;
                    ManaCost = 11;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 11;
                    ManaCost = 10;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.HighMage:
                    Level = 8;
                    ManaCost = 7;
                    BaseFailure = 40;
                    FirstCastExperience = 6;
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