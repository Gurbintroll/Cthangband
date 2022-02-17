using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Sorcery
{
    [Serializable]
    internal class SorcerySpellMassSleep : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.SleepMonsters();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Mass Sleep";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 13;
                    ManaCost = 7;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Rogue:
                    Level = 30;
                    ManaCost = 20;
                    BaseFailure = 80;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 13;
                    ManaCost = 12;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.HighMage:
                    Level = 9;
                    ManaCost = 5;
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