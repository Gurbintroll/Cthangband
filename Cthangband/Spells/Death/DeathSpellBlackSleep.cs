using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellBlackSleep : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.SleepMonster(dir);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Black Sleep";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 5;
                    ManaCost = 5;
                    BaseFailure = 30;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Priest:
                    Level = 7;
                    ManaCost = 7;
                    BaseFailure = 30;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Rogue:
                    Level = 15;
                    ManaCost = 7;
                    BaseFailure = 80;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.Ranger:
                    Level = 11;
                    ManaCost = 8;
                    BaseFailure = 40;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.Paladin:
                    Level = 8;
                    ManaCost = 8;
                    BaseFailure = 30;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 7;
                    ManaCost = 7;
                    BaseFailure = 30;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.HighMage:
                    Level = 3;
                    ManaCost = 3;
                    BaseFailure = 20;
                    FirstCastExperience = 4;
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