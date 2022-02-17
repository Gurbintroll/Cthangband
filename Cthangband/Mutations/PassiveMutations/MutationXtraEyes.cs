using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationXtraEyes : Mutation
    {
        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "You grow an extra pair of eyes!";
            HaveMessage = "You have an extra pair of eyes (+15 search).";
            LoseMessage = "Your extra eyes vanish!";
        }

        public override void OnGain(Genome genome)
        {
            genome.SearchBonus += 15;
        }

        public override void OnLose(Genome genome)
        {
            genome.SearchBonus -= 15;
        }
    }
}