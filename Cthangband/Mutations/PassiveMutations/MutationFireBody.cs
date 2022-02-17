using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationFireBody : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "Your body is enveloped in flames!";
            HaveMessage = "Your body is enveloped in flames.";
            LoseMessage = "Your body is no longer enveloped in flames.";
        }

        public override void OnGain(Genome genome)
        {
            genome.FireHit = true;
        }

        public override void OnLose(Genome genome)
        {
            genome.FireHit = false;
        }
    }
}