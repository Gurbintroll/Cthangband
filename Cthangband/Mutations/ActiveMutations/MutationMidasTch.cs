using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationMidasTch : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.RacialAux(10, 5, Ability.Intelligence, 12))
            {
                saveGame.SpellEffects.Alchemy();
            }
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 10 ? "midas touch      (unusable until level 10)" : "midas touch      (cost 5, INT based)";
        }

        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You gain the Midas touch.";
            HaveMessage = "You can turn ordinary items to gold.";
            LoseMessage = "You lose the Midas touch.";
        }
    }
}