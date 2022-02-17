using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationLauncher : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.RacialAux(1, player.Level, Ability.Strength, 6))
            {
                return;
            }
            ActivationHandler handler = new ActivationHandler(level, player);
            handler.DoCmdThrow(2 + (player.Level / 16));
        }

        public override string ActivationSummary(int lvl)
        {
            return "throw object     (cost lev, STR based)";
        }

        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "Your throwing arm feels much stronger.";
            HaveMessage = "You can hurl objects with great force.";
            LoseMessage = "Your throwing arm feels much weaker.";
        }
    }
}