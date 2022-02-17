using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationRecall : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.RacialAux(17, 50, Ability.Intelligence, 16))
            {
                return;
            }
            player.ToggleRecall();
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 17 ? "recall           (unusable until level 17)" : "recall           (cost 50, INT based)";
        }

        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You feel briefly homesick, but it passes.";
            HaveMessage = "You can travel between town and the depths.";
            LoseMessage = "You feel briefly homesick.";
        }
    }
}