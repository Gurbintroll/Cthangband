using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellCarnage : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.Carnage(true);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Carnage";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 37;
                    ManaCost = 25;
                    BaseFailure = 95;
                    FirstCastExperience = 25;
                    break;

                case CharacterClass.Priest:
                    Level = 40;
                    ManaCost = 30;
                    BaseFailure = 95;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.Rogue:
                    Level = 99;
                    ManaCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;

                case CharacterClass.Ranger:
                    Level = 99;
                    ManaCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;

                case CharacterClass.Paladin:
                    Level = 45;
                    ManaCost = 35;
                    BaseFailure = 95;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 44;
                    ManaCost = 45;
                    BaseFailure = 95;
                    FirstCastExperience = 25;
                    break;

                case CharacterClass.HighMage:
                    Level = 33;
                    ManaCost = 30;
                    BaseFailure = 85;
                    FirstCastExperience = 25;
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