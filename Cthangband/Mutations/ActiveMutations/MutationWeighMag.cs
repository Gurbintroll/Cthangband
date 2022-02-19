using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationWeighMag : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(6, 6, Ability.Intelligence, 10))
            {
                saveGame.SpellEffects.ReportMagics();
            }
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 6 ? "weigh magic      (unusable until level 6)" : "weigh magic      (cost 6, INT based)";
        }

        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You feel you can better understand the magic around you.";
            HaveMessage = "You can feel the strength of the magics affecting you.";
            LoseMessage = "You no longer sense magic.";
        }
    }
}