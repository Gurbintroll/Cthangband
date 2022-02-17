using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellHolyVision : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.IdentifyFully();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Holy Vision";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 48;
                    ManaCost = 50;
                    BaseFailure = 80;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.Priest:
                    Level = 40;
                    ManaCost = 40;
                    BaseFailure = 80;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.Paladin:
                    Level = 45;
                    ManaCost = 45;
                    BaseFailure = 80;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 48;
                    ManaCost = 50;
                    BaseFailure = 80;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.HighMage:
                    Level = 45;
                    ManaCost = 60;
                    BaseFailure = 60;
                    FirstCastExperience = 100;
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