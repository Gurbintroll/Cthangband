using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellFireBall : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBall(new ProjectFire(SaveGame.Instance.SpellEffects), dir, 55 + player.Level, 2);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Fire Ball";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 25;
                    ManaCost = 16;
                    BaseFailure = 50;
                    FirstCastExperience = 12;
                    break;

                case CharacterClass.Priest:
                    Level = 27;
                    ManaCost = 20;
                    BaseFailure = 65;
                    FirstCastExperience = 12;
                    break;

                case CharacterClass.Ranger:
                    Level = 37;
                    ManaCost = 35;
                    BaseFailure = 75;
                    FirstCastExperience = 15;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 33;
                    ManaCost = 33;
                    BaseFailure = 50;
                    FirstCastExperience = 12;
                    break;

                case CharacterClass.Fanatic:
                    Level = 30;
                    ManaCost = 20;
                    BaseFailure = 50;
                    FirstCastExperience = 12;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 22;
                    ManaCost = 13;
                    BaseFailure = 40;
                    FirstCastExperience = 12;
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
            return $"dam {55 + player.Level}";
        }
    }
}