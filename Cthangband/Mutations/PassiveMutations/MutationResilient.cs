using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationResilient : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You become extraordinarily resilient.";
            HaveMessage = "You are very resilient (+4 CON).";
            LoseMessage = "You become ordinarily resilient again.";
        }

        public override void OnGain(Genome genome)
        {
            genome.ConstitutionBonus += 4;
        }

        public override void OnLose(Genome genome)
        {
            genome.ConstitutionBonus -= 4;
        }
    }
}