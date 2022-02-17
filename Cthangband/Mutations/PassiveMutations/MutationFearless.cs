using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationFearless : Mutation
    {
        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "You become completely fearless.";
            HaveMessage = "You are completely fearless.";
            LoseMessage = "You begin to feel fear again.";
            Group = MutationGroup.Bravery;
        }

        public override void OnGain(Genome genome)
        {
            genome.ResFear = true;
        }

        public override void OnLose(Genome genome)
        {
            genome.ResFear = false;
        }
    }
}