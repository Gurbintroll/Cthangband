using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationResTime : Mutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "You feel immortal.";
            HaveMessage = "You are protected from the ravages of time.";
            LoseMessage = "You feel all too mortal.";
        }

        public override void OnGain(Genome genome)
        {
            genome.ResTime = true;
        }

        public override void OnLose(Genome genome)
        {
            genome.ResTime = false;
        }
    }
}