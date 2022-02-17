using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationShortLeg : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "Your legs turn into short stubs!";
            HaveMessage = "Your legs are short stubs (-3 speed).";
            LoseMessage = "Your legs lengthen to normal.";
        }

        public override void OnGain(Genome genome)
        {
            genome.SpeedBonus -= 3;
        }

        public override void OnLose(Genome genome)
        {
            genome.SpeedBonus += 3;
        }
    }
}