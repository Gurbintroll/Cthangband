using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellDoorCreation : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DoorCreation();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Door Creation";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 7;
                    ManaCost = 7;
                    BaseFailure = 20;
                    FirstCastExperience = 28;
                    break;

                case CharacterClass.Priest:
                    Level = 9;
                    ManaCost = 9;
                    BaseFailure = 20;
                    FirstCastExperience = 28;
                    break;

                case CharacterClass.Ranger:
                    Level = 10;
                    ManaCost = 10;
                    BaseFailure = 50;
                    FirstCastExperience = 25;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 9;
                    ManaCost = 9;
                    BaseFailure = 20;
                    FirstCastExperience = 28;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 5;
                    ManaCost = 5;
                    BaseFailure = 10;
                    FirstCastExperience = 28;
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