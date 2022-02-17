using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationWings : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You grow a pair of wings.";
            HaveMessage = "You have wings.";
            LoseMessage = "Your wings fall off.";
        }

        public override void OnGain(Genome genome)
        {
            genome.FeatherFall = true;
        }

        public override void OnLose(Genome genome)
        {
            genome.FeatherFall = false;
        }
    }
}