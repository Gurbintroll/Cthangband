using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationEsp : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You develop a telepathic ability!";
            HaveMessage = "You are telepathic.";
            LoseMessage = "You lose your telepathic ability!";
        }

        public override void OnGain(Genome genome)
        {
            genome.Esp = true;
        }

        public override void OnLose(Genome genome)
        {
            genome.Esp = false;
        }
    }
}