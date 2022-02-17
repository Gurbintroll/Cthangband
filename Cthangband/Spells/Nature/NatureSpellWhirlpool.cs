using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellWhirlpool : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            SaveGame.Instance.SpellEffects.FireBall(new ProjectWater(SaveGame.Instance.SpellEffects), dir, 100 + player.Level, (player.Level / 12) + 1);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Whirlpool";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 35;
                    ManaCost = 30;
                    BaseFailure = 85;
                    FirstCastExperience = 65;
                    break;

                case CharacterClass.Priest:
                    Level = 37;
                    ManaCost = 32;
                    BaseFailure = 85;
                    FirstCastExperience = 65;
                    break;

                case CharacterClass.Ranger:
                    Level = 36;
                    ManaCost = 33;
                    BaseFailure = 75;
                    FirstCastExperience = 45;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 38;
                    ManaCost = 38;
                    BaseFailure = 85;
                    FirstCastExperience = 65;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 32;
                    ManaCost = 28;
                    BaseFailure = 75;
                    FirstCastExperience = 65;
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
            return $"dam {100 + player.Level}";
        }
    }
}