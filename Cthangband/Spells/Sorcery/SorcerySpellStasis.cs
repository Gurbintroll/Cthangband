using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Sorcery
{
    [Serializable]
    internal class SorcerySpellStasis : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.StasisMonster(dir);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Stasis";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 10;
                    ManaCost = 10;
                    BaseFailure = 40;
                    FirstCastExperience = 20;
                    break;

                case CharacterClass.Rogue:
                    Level = 15;
                    ManaCost = 15;
                    BaseFailure = 40;
                    FirstCastExperience = 10;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 12;
                    ManaCost = 12;
                    BaseFailure = 40;
                    FirstCastExperience = 20;
                    break;

                case CharacterClass.HighMage:
                    Level = 8;
                    ManaCost = 8;
                    BaseFailure = 30;
                    FirstCastExperience = 20;
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