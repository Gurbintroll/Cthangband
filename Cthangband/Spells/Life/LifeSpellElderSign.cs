using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellElderSign : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.ElderSign();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Elder Sign";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 35;
                    ManaCost = 70;
                    BaseFailure = 75;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Priest:
                    Level = 33;
                    ManaCost = 55;
                    BaseFailure = 90;
                    FirstCastExperience = 15;
                    break;

                case CharacterClass.Paladin:
                    Level = 35;
                    ManaCost = 70;
                    BaseFailure = 75;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 40;
                    ManaCost = 70;
                    BaseFailure = 75;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.HighMage:
                    Level = 30;
                    ManaCost = 50;
                    BaseFailure = 55;
                    FirstCastExperience = 5;
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