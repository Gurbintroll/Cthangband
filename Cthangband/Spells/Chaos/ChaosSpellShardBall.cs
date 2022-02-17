using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellShardBall : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBall(new ProjectShard(SaveGame.Instance.SpellEffects), dir, 120 + player.Level, 2);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Shard Ball";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 43;
                    ManaCost = 44;
                    BaseFailure = 80;
                    FirstCastExperience = 150;
                    break;

                case CharacterClass.Priest:
                    Level = 45;
                    ManaCost = 47;
                    BaseFailure = 90;
                    FirstCastExperience = 150;
                    break;

                case CharacterClass.Ranger:
                    Level = 50;
                    ManaCost = 50;
                    BaseFailure = 90;
                    FirstCastExperience = 150;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 48;
                    ManaCost = 48;
                    BaseFailure = 80;
                    FirstCastExperience = 150;
                    break;

                case CharacterClass.Fanatic:
                    Level = 43;
                    ManaCost = 44;
                    BaseFailure = 80;
                    FirstCastExperience = 150;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 38;
                    ManaCost = 38;
                    BaseFailure = 70;
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
            return $"dam {120 + player.Level}";
        }
    }
}