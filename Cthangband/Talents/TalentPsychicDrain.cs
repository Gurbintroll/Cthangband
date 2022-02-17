using Cthangband.Projection;
using System;

namespace Cthangband.Talents
{
    [Serializable]
    internal class TalentPsychicDrain : Talent
    {
        public override void Initialise(int characterClass)
        {
            Name = "Psychic Drain";
            Level = 25;
            ManaCost = 10;
            BaseFailure = 40;
        }

        public override void Use(Player player, Level level, SaveGame saveGame)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            int i = Program.Rng.DiceRoll(player.Level / 2, 6);
            if (SaveGame.Instance.SpellEffects.FireBall(new ProjectPsiDrain(SaveGame.Instance.SpellEffects), dir, i, 0 + ((player.Level - 25) / 10)))
            {
                player.Energy -= Program.Rng.DieRoll(150);
            }
        }

        protected override string Comment(Player player)
        {
            return $"dam {player.Level / 2}d6";
        }
    }
}