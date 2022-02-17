using Cthangband.Projection;
using System;

namespace Cthangband.Talents
{
    [Serializable]
    internal class TalentNeuralBlast : Talent
    {
        public override void Initialise(int characterClass)
        {
            Name = "Neural Blast";
            Level = 2;
            ManaCost = 1;
            BaseFailure = 20;
        }

        public override void Use(Player player, Level level, SaveGame saveGame)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            if (Program.Rng.DieRoll(100) < player.Level * 2)
            {
                saveGame.SpellEffects.FireBeam(new ProjectPsi(SaveGame.Instance.SpellEffects), dir,
                    Program.Rng.DiceRoll(3 + ((player.Level - 1) / 4), 3 + (player.Level / 15)));
            }
            else
            {
                saveGame.SpellEffects.FireBall(new ProjectPsi(SaveGame.Instance.SpellEffects), dir,
                    Program.Rng.DiceRoll(3 + ((player.Level - 1) / 4), 3 + (player.Level / 15)), 0);
            }
        }

        protected override string Comment(Player player)
        {
            return $"dam {3 + ((player.Level - 1) / 4)}d{3 + (player.Level / 15)}";
        }
    }
}