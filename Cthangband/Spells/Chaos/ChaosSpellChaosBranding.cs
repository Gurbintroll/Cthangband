using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellChaosBranding : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.CommandEngine.BrandWeapon(1);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Chaos Branding";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 45;
                    ManaCost = 90;
                    BaseFailure = 80;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Priest:
                    Level = 47;
                    ManaCost = 90;
                    BaseFailure = 90;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Ranger:
                    Level = 48;
                    ManaCost = 100;
                    BaseFailure = 90;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 49;
                    ManaCost = 95;
                    BaseFailure = 80;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Fanatic:
                    Level = 45;
                    ManaCost = 90;
                    BaseFailure = 80;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 42;
                    ManaCost = 80;
                    BaseFailure = 70;
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