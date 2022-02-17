using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Corporeal
{
    [Serializable]
    internal class CorporealSpellSeeMagic : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DetectObjectsMagic();
        }

        public override void Initialise(int characterClass)
        {
            Name = "See Magic";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 10;
                    ManaCost = 10;
                    BaseFailure = 70;
                    FirstCastExperience = 40;
                    break;

                case CharacterClass.Priest:
                    Level = 12;
                    ManaCost = 12;
                    BaseFailure = 70;
                    FirstCastExperience = 40;
                    break;

                case CharacterClass.Ranger:
                    Level = 15;
                    ManaCost = 20;
                    BaseFailure = 70;
                    FirstCastExperience = 25;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                case CharacterClass.Cultist:
                    Level = 12;
                    ManaCost = 12;
                    BaseFailure = 70;
                    FirstCastExperience = 40;
                    break;

                case CharacterClass.HighMage:
                    Level = 7;
                    ManaCost = 7;
                    BaseFailure = 60;
                    FirstCastExperience = 40;
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