using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellInvokeChaos : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBall(new ProjectChaos(SaveGame.Instance.SpellEffects), dir, 66 + player.Level, player.Level / 5);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Invoke Chaos";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 35;
                    ManaCost = 40;
                    BaseFailure = 85;
                    FirstCastExperience = 40;
                    break;

                case CharacterClass.Priest:
                    Level = 37;
                    ManaCost = 42;
                    BaseFailure = 85;
                    FirstCastExperience = 40;
                    break;

                case CharacterClass.Ranger:
                    Level = 48;
                    ManaCost = 50;
                    BaseFailure = 85;
                    FirstCastExperience = 30;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 48;
                    ManaCost = 50;
                    BaseFailure = 85;
                    FirstCastExperience = 40;
                    break;

                case CharacterClass.Fanatic:
                    Level = 40;
                    ManaCost = 45;
                    BaseFailure = 85;
                    FirstCastExperience = 40;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 30;
                    ManaCost = 35;
                    BaseFailure = 75;
                    FirstCastExperience = 40;
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
            return $"dam {66 + player.Level}";
        }
    }
}