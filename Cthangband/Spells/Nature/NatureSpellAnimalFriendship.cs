using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellAnimalFriendship : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.CharmAnimals(player.Level * 2);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Animal Friendship";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 30;
                    ManaCost = 30;
                    BaseFailure = 90;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.Priest:
                    Level = 35;
                    ManaCost = 35;
                    BaseFailure = 80;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Ranger:
                    Level = 35;
                    ManaCost = 30;
                    BaseFailure = 80;
                    FirstCastExperience = 75;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 38;
                    ManaCost = 38;
                    BaseFailure = 85;
                    FirstCastExperience = 80;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 25;
                    ManaCost = 25;
                    BaseFailure = 80;
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