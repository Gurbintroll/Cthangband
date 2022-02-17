using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellBlizzard : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            SaveGame.Instance.SpellEffects.FireBall(new ProjectCold(SaveGame.Instance.SpellEffects), dir, 70 + player.Level, (player.Level / 12) + 1);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Blizzard";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 25;
                    ManaCost = 25;
                    BaseFailure = 75;
                    FirstCastExperience = 29;
                    break;

                case CharacterClass.Priest:
                    Level = 27;
                    ManaCost = 27;
                    BaseFailure = 75;
                    FirstCastExperience = 29;
                    break;

                case CharacterClass.Ranger:
                    Level = 30;
                    ManaCost = 35;
                    BaseFailure = 75;
                    FirstCastExperience = 125;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 28;
                    ManaCost = 28;
                    BaseFailure = 75;
                    FirstCastExperience = 29;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 22;
                    ManaCost = 22;
                    BaseFailure = 65;
                    FirstCastExperience = 29;
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
            return $"dam {70 + player.Level}";
        }
    }
}