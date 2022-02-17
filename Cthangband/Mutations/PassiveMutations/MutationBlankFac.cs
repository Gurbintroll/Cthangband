using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationBlankFac : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "Your face becomes completely featureless!";
            HaveMessage = "Your face is featureless (-1 CHR).";
            LoseMessage = "Your facial features return.";
        }

        public override void OnGain(Genome genome)
        {
            genome.CharismaBonus -= 1;
        }

        public override void OnLose(Genome genome)
        {
            genome.CharismaBonus += 1;
        }
    }
}