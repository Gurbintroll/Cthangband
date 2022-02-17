using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellRestoreLife : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.RestoreLevel();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Restore Life";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 45;
                    ManaCost = 50;
                    BaseFailure = 95;
                    FirstCastExperience = 150;
                    break;

                case CharacterClass.Priest:
                    Level = 50;
                    ManaCost = 52;
                    BaseFailure = 95;
                    FirstCastExperience = 150;
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
                    Level = 50;
                    ManaCost = 52;
                    BaseFailure = 95;
                    FirstCastExperience = 150;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 48;
                    ManaCost = 55;
                    BaseFailure = 95;
                    FirstCastExperience = 150;
                    break;

                case CharacterClass.HighMage:
                    Level = 40;
                    ManaCost = 40;
                    BaseFailure = 85;
                    FirstCastExperience = 150;
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