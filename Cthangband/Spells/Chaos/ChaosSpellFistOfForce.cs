using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellFistOfForce : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBall(new ProjectDisintegrate(SaveGame.Instance.SpellEffects), dir,
                Program.Rng.DiceRoll(8 + ((player.Level - 5) / 4), 8), 0);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Fist of Force";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 14;
                    ManaCost = 9;
                    BaseFailure = 45;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Priest:
                    Level = 16;
                    ManaCost = 11;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Ranger:
                    Level = 25;
                    ManaCost = 21;
                    BaseFailure = 60;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 17;
                    ManaCost = 15;
                    BaseFailure = 45;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Fanatic:
                    Level = 15;
                    ManaCost = 9;
                    BaseFailure = 45;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 12;
                    ManaCost = 6;
                    BaseFailure = 35;
                    FirstCastExperience = 6;
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
            return $"dam {8 + ((player.Level - 5) / 4)}d8";
        }
    }
}