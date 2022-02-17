using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Sorcery
{
    [Serializable]
    internal class SorcerySpellDetectMonsters : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DetectMonstersNormal();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Detect Monsters";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 1;
                    ManaCost = 1;
                    BaseFailure = 23;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Rogue:
                    Level = 5;
                    ManaCost = 1;
                    BaseFailure = 50;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 1;
                    ManaCost = 1;
                    BaseFailure = 23;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.HighMage:
                    Level = 1;
                    ManaCost = 1;
                    BaseFailure = 15;
                    FirstCastExperience = 4;
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