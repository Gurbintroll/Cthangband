using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationXtraLegs : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You grow an extra pair of legs!";
            HaveMessage = "You have an extra pair of legs (+3 speed).";
            LoseMessage = "Your extra legs disappear!";
        }

        public override void OnGain(Genome genome)
        {
            genome.SpeedBonus += 3;
        }

        public override void OnLose(Genome genome)
        {
            genome.SpeedBonus -= 3;
        }
    }
}