using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationSmellMet : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(3, 2, Ability.Intelligence, 12))
            {
                saveGame.SpellEffects.DetectTreasure();
            }
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 3 ? "smell metal      (unusable until level 3)" : "smell metal      (cost 2, INT based)";
        }

        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "You smell a metallic odor.";
            HaveMessage = "You can smell nearby precious metal.";
            LoseMessage = "You no longer smell a metallic odor.";
        }
    }
}