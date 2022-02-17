using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Sorcery
{
    [Serializable]
    internal class SorcerySpellDetectObjectsAndTreasure : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DetectObjectsNormal();
            saveGame.SpellEffects.DetectTreasure();
            saveGame.SpellEffects.DetectObjectsGold();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Detect Objects and Treasure";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 3;
                    ManaCost = 3;
                    BaseFailure = 25;
                    FirstCastExperience = 15;
                    break;

                case CharacterClass.Rogue:
                    Level = 9;
                    ManaCost = 3;
                    BaseFailure = 65;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 4;
                    ManaCost = 4;
                    BaseFailure = 25;
                    FirstCastExperience = 15;
                    break;

                case CharacterClass.HighMage:
                    Level = 2;
                    ManaCost = 2;
                    BaseFailure = 20;
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
            return string.Empty;
        }
    }
}