using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellVampiricBranding : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.CommandEngine.BrandWeapon(3);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Vampiric Branding";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 33;
                    ManaCost = 90;
                    BaseFailure = 70;
                    FirstCastExperience = 90;
                    break;

                case CharacterClass.Priest:
                    Level = 35;
                    ManaCost = 95;
                    BaseFailure = 70;
                    FirstCastExperience = 90;
                    break;

                case CharacterClass.Rogue:
                    Level = 48;
                    ManaCost = 100;
                    BaseFailure = 90;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.Ranger:
                    Level = 46;
                    ManaCost = 100;
                    BaseFailure = 90;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.Paladin:
                    Level = 42;
                    ManaCost = 100;
                    BaseFailure = 70;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 42;
                    ManaCost = 90;
                    BaseFailure = 70;
                    FirstCastExperience = 90;
                    break;

                case CharacterClass.HighMage:
                    Level = 30;
                    ManaCost = 80;
                    BaseFailure = 60;
                    FirstCastExperience = 90;
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