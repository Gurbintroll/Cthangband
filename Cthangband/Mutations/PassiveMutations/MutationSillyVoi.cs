using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationSillyVoi : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "Your voice turns into a ridiculous squeak!";
            HaveMessage = "Your voice is a silly squeak (-4 CHR).";
            LoseMessage = "Your voice returns to normal.";
        }

        public override void OnGain(Genome genome)
        {
            genome.CharismaBonus -= 4;
        }

        public override void OnLose(Genome genome)
        {
            genome.CharismaBonus += 4;
        }
    }
}