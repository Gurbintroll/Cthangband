using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationHyperInt : Mutation
    {
        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "Your brain evolves into a living computer!";
            HaveMessage = "Your brain is a living computer (+4 INT/WIS).";
            LoseMessage = "Your brain reverts to normal.";
            Group = MutationGroup.Smarts;
        }

        public override void OnGain(Genome genome)
        {
            genome.IntelligenceBonus += 4;
            genome.WisdomBonus += 4;
        }

        public override void OnLose(Genome genome)
        {
            genome.IntelligenceBonus -= 4;
            genome.WisdomBonus -= 4;
        }
    }
}