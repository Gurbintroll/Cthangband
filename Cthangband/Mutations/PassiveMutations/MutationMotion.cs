using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationMotion : Mutation
    {
        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "You move with new assurance.";
            HaveMessage = "Your movements are precise and forceful (+1 STL).";
            LoseMessage = "You move with less assurance.";
        }

        public override void OnGain(Genome genome)
        {
            genome.StealthBonus += 1;
            genome.FreeAction = true;
        }

        public override void OnLose(Genome genome)
        {
            genome.StealthBonus -= 1;
            genome.FreeAction = false;
        }
    }
}