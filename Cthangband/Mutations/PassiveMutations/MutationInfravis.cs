using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationInfravis : Mutation
    {
        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "Your infravision is improved.";
            HaveMessage = "You have remarkable infravision (+3).";
            LoseMessage = "Your infravision is degraded.";
        }

        public override void OnGain(Genome genome)
        {
            genome.InfravisionBonus += 3;
        }

        public override void OnLose(Genome genome)
        {
            genome.InfravisionBonus -= 3;
        }
    }
}