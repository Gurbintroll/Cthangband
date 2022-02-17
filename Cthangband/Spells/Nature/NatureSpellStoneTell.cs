using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellStoneTell : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.IdentifyFully();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Stone Tell";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 37;
                    ManaCost = 40;
                    BaseFailure = 90;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.Priest:
                    Level = 39;
                    ManaCost = 40;
                    BaseFailure = 90;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.Ranger:
                    Level = 38;
                    ManaCost = 40;
                    BaseFailure = 90;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 40;
                    ManaCost = 42;
                    BaseFailure = 90;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 33;
                    ManaCost = 35;
                    BaseFailure = 80;
                    FirstCastExperience = 200;
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