using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellDispelCurse : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.RemoveAllCurse();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Dispel Curse";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 28;
                    ManaCost = 25;
                    BaseFailure = 70;
                    FirstCastExperience = 150;
                    break;

                case CharacterClass.Priest:
                    Level = 16;
                    ManaCost = 14;
                    BaseFailure = 80;
                    FirstCastExperience = 60;
                    break;

                case CharacterClass.Paladin:
                    Level = 28;
                    ManaCost = 24;
                    BaseFailure = 70;
                    FirstCastExperience = 150;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 30;
                    ManaCost = 30;
                    BaseFailure = 70;
                    FirstCastExperience = 150;
                    break;

                case CharacterClass.HighMage:
                    Level = 24;
                    ManaCost = 24;
                    BaseFailure = 60;
                    FirstCastExperience = 150;
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