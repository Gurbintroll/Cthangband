using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Tarot
{
    [Serializable]
    internal class TarotSpellSummonObject : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            saveGame.CommandEngine.Fetch(dir, player.Level * 15, true);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Summon Object";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 20;
                    ManaCost = 20;
                    BaseFailure = 80;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Priest:
                case CharacterClass.Monk:
                    Level = 22;
                    ManaCost = 22;
                    BaseFailure = 80;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Rogue:
                    Level = 25;
                    ManaCost = 22;
                    BaseFailure = 80;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Ranger:
                    Level = 24;
                    ManaCost = 22;
                    BaseFailure = 80;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 24;
                    ManaCost = 23;
                    BaseFailure = 80;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.HighMage:
                    Level = 16;
                    ManaCost = 16;
                    BaseFailure = 70;
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
            return $"max wgt {player.Level * 15 / 10}";
        }
    }
}