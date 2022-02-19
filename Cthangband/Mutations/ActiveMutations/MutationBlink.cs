using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationBlink : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(3, 3, Ability.Wisdom, 12))
            {
                saveGame.SpellEffects.TeleportPlayer(10);
            }
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 3 ? "blink            (unusable until level 3)" : "blink            (cost 3, WIS based)";
        }

        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "You gain the power of minor teleportation.";
            HaveMessage = "You can teleport yourself short distances.";
            LoseMessage = "You lose the power of minor teleportation.";
        }
    }
}