using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationIllumine : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.RacialAux(3, 2, Ability.Intelligence, 10))
            {
                saveGame.SpellEffects.LightArea(Program.Rng.DiceRoll(2, player.Level / 2), (player.Level / 10) + 1);
            }
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 3 ? "illuminate       (unusable until level 3)" : "illuminate       (cost 2, INT based)";
        }

        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "You can light up rooms with your presence.";
            HaveMessage = "You can emit bright light.";
            LoseMessage = "You can no longer light up rooms with your presence.";
        }
    }
}