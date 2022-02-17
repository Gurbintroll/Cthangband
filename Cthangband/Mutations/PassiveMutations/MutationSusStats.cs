using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationSusStats : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You feel like you can recover from anything.";
            HaveMessage = "Your body resists serious damage.";
            LoseMessage = "You no longer feel like you can recover from anything.";
        }

        public override void OnGain(Genome genome)
        {
            genome.SustainAll = true;
        }

        public override void OnLose(Genome genome)
        {
            genome.SustainAll = false;
        }
    }
}