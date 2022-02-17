using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Corporeal
{
    [Serializable]
    internal class CorporealSpellDetoxify : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedPoison(0);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Detoxify";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 10;
                    ManaCost = 7;
                    BaseFailure = 75;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Priest:
                    Level = 14;
                    ManaCost = 12;
                    BaseFailure = 75;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Ranger:
                    Level = 23;
                    ManaCost = 25;
                    BaseFailure = 90;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                case CharacterClass.Cultist:
                    Level = 11;
                    ManaCost = 10;
                    BaseFailure = 75;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.HighMage:
                    Level = 7;
                    ManaCost = 5;
                    BaseFailure = 65;
                    FirstCastExperience = 8;
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