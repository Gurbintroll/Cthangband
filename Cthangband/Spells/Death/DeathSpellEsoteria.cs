using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellEsoteria : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            if (Program.Rng.DieRoll(50) > player.Level)
            {
                saveGame.SpellEffects.IdentifyItem();
            }
            else
            {
                saveGame.SpellEffects.IdentifyFully();
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Esoteria";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 30;
                    ManaCost = 40;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Priest:
                    Level = 35;
                    ManaCost = 45;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Rogue:
                    Level = 32;
                    ManaCost = 40;
                    BaseFailure = 90;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Ranger:
                    Level = 40;
                    ManaCost = 45;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Paladin:
                    Level = 38;
                    ManaCost = 45;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 35;
                    ManaCost = 45;
                    BaseFailure = 90;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.HighMage:
                    Level = 26;
                    ManaCost = 35;
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