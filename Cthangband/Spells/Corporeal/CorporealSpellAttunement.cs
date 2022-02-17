using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Corporeal
{
    [Serializable]
    internal class CorporealSpellAttunement : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.IdentifyFully();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Attunement";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 25;
                    ManaCost = 30;
                    BaseFailure = 95;
                    FirstCastExperience = 160;
                    break;

                case CharacterClass.Priest:
                    Level = 27;
                    ManaCost = 30;
                    BaseFailure = 95;
                    FirstCastExperience = 160;
                    break;

                case CharacterClass.Ranger:
                    Level = 37;
                    ManaCost = 60;
                    BaseFailure = 95;
                    FirstCastExperience = 120;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                case CharacterClass.Cultist:
                    Level = 30;
                    ManaCost = 35;
                    BaseFailure = 95;
                    FirstCastExperience = 160;
                    break;

                case CharacterClass.HighMage:
                    Level = 20;
                    ManaCost = 25;
                    BaseFailure = 85;
                    FirstCastExperience = 160;
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