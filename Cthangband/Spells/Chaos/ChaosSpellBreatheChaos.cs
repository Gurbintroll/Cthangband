using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellBreatheChaos : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBall(new ProjectChaos(SaveGame.Instance.SpellEffects), dir, player.Health, -2);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Breathe Chaos";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 47;
                    ManaCost = 75;
                    BaseFailure = 80;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.Priest:
                    Level = 49;
                    ManaCost = 95;
                    BaseFailure = 80;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.Ranger:
                    Level = 99;
                    ManaCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 50;
                    ManaCost = 100;
                    BaseFailure = 80;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Fanatic:
                    Level = 48;
                    ManaCost = 100;
                    BaseFailure = 80;
                    FirstCastExperience = 220;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 43;
                    ManaCost = 55;
                    BaseFailure = 70;
                    FirstCastExperience = 200;
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
            return $"dam {player.Health}";
        }
    }
}