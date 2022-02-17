using Cthangband.Projection;
using System;

namespace Cthangband.Talents
{
    [Serializable]
    internal class TalentPulverise : Talent
    {
        public override void Initialise(int characterClass)
        {
            Name = "Pulverise";
            Level = 11;
            ManaCost = 7;
            BaseFailure = 30;
        }

        public override void Use(Player player, Level level, SaveGame saveGame)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBall(new ProjectSound(SaveGame.Instance.SpellEffects), dir,
                Program.Rng.DiceRoll(8 + ((player.Level - 5) / 4), 8), player.Level > 20 ? ((player.Level - 20) / 8) + 1 : 0);
        }

        protected override string Comment(Player player)
        {
            return $"dam {8 + ((player.Level - 5) / 4)}d8";
            ;
        }
    }
}