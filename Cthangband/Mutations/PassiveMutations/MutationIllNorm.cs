using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationIllNorm : Mutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "You start projecting a reassuring image.";
            HaveMessage = "Your appearance is masked with illusion.";
            LoseMessage = "You stop projecting a reassuring image.";
        }

        public override void OnGain(Genome genome)
        {
            genome.CharismaOverride = true;
        }

        public override void OnLose(Genome genome)
        {
            genome.CharismaOverride = false;
        }
    }
}