using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellBlessWeapon : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.BlessWeapon();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Bless Weapon";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 35;
                    ManaCost = 85;
                    BaseFailure = 80;
                    FirstCastExperience = 115;
                    break;

                case CharacterClass.Priest:
                    Level = 30;
                    ManaCost = 50;
                    BaseFailure = 80;
                    FirstCastExperience = 130;
                    break;

                case CharacterClass.Paladin:
                    Level = 35;
                    ManaCost = 65;
                    BaseFailure = 80;
                    FirstCastExperience = 115;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 38;
                    ManaCost = 85;
                    BaseFailure = 80;
                    FirstCastExperience = 115;
                    break;

                case CharacterClass.HighMage:
                    Level = 30;
                    ManaCost = 70;
                    BaseFailure = 60;
                    FirstCastExperience = 115;
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