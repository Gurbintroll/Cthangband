using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationVulnElem : Mutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "You feel strangely exposed.";
            HaveMessage = "You are susceptible to damage from the elements.";
            LoseMessage = "You feel less exposed.";
        }

        public override void OnGain(Genome genome)
        {
            genome.Vulnerable = true;
        }

        public override void OnLose(Genome genome)
        {
            genome.Vulnerable = false;
        }
    }
}