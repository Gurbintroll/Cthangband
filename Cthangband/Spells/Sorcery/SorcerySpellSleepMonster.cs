using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Sorcery
{
    [Serializable]
    internal class SorcerySpellSleepMonster : Spell
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
            Name = "Sleep Monster";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 6;
                    ManaCost = 5;
                    BaseFailure = 35;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Rogue:
                    Level = 17;
                    ManaCost = 9;
                    BaseFailure = 75;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 7;
                    ManaCost = 7;
                    BaseFailure = 30;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.HighMage:
                    Level = 5;
                    ManaCost = 4;
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