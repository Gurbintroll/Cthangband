using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellWardingTrue : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.ElderSign();
            saveGame.SpellEffects.ElderSignCreation();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Warding True";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 46;
                    ManaCost = 70;
                    BaseFailure = 80;
                    FirstCastExperience = 150;
                    break;

                case CharacterClass.Priest:
                    Level = 44;
                    ManaCost = 44;
                    BaseFailure = 80;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Paladin:
                    Level = 46;
                    ManaCost = 60;
                    BaseFailure = 80;
                    FirstCastExperience = 150;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 50;
                    ManaCost = 70;
                    BaseFailure = 80;
                    FirstCastExperience = 150;
                    break;

                case CharacterClass.HighMage:
                    Level = 40;
                    ManaCost = 70;
                    BaseFailure = 70;
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