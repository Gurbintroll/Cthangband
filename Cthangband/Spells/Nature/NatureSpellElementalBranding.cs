using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellElementalBranding : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.CommandEngine.BrandWeapon(0);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Elemental Branding";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 40;
                    ManaCost = 90;
                    BaseFailure = 95;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.Priest:
                    Level = 42;
                    ManaCost = 90;
                    BaseFailure = 95;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.Ranger:
                    Level = 41;
                    ManaCost = 80;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 45;
                    ManaCost = 95;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 36;
                    ManaCost = 80;
                    BaseFailure = 85;
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