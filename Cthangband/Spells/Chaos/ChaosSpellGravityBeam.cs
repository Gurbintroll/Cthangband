using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellGravityBeam : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBeam(new ProjectGravity(SaveGame.Instance.SpellEffects), dir,
                Program.Rng.DiceRoll(9 + ((player.Level - 5) / 4), 8));
        }

        public override void Initialise(int characterClass)
        {
            Name = "Gravity Beam";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 20;
                    ManaCost = 20;
                    BaseFailure = 66;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Priest:
                    Level = 25;
                    ManaCost = 25;
                    BaseFailure = 66;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Ranger:
                    Level = 33;
                    ManaCost = 33;
                    BaseFailure = 66;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 24;
                    ManaCost = 20;
                    BaseFailure = 66;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Fanatic:
                    Level = 23;
                    ManaCost = 23;
                    BaseFailure = 66;
                    FirstCastExperience = 10;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 16;
                    ManaCost = 16;
                    BaseFailure = 55;
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
            return $"dam {9 + ((player.Level - 5) / 4)}d8";
        }
    }
}