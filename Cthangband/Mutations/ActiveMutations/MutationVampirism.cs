using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationVampirism : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.RacialAux(13, player.Level, Ability.Constitution, 14))
            {
                return;
            }
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            if (SaveGame.Instance.SpellEffects.DrainLife(dir, player.Level * 2))
            {
                player.RestoreHealth(player.Level + Program.Rng.DieRoll(player.Level));
            }
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 13
                ? "vampiric drain   (unusable until level 13)"
                : $"vampiric drain   (cost {lvl}, CON based)";
        }

        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You become vampiric.";
            HaveMessage = "You can drain life from a foe like a vampire.";
            LoseMessage = "You are no longer vampiric.";
        }
    }
}