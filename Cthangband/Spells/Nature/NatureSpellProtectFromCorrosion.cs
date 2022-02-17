using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellProtectFromCorrosion : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.CommandEngine.Rustproof();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Protection from Corrosion";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 40;
                    ManaCost = 90;
                    BaseFailure = 90;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Priest:
                    Level = 42;
                    ManaCost = 90;
                    BaseFailure = 90;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Ranger:
                    Level = 42;
                    ManaCost = 80;
                    BaseFailure = 90;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 49;
                    ManaCost = 95;
                    BaseFailure = 90;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 37;
                    ManaCost = 65;
                    BaseFailure = 80;
                    FirstCastExperience = 250;
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