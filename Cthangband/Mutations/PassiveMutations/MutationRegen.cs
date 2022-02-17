using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationRegen : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You start regenerating.";
            HaveMessage = "You are regenerating.";
            LoseMessage = "You stop regenerating.";
        }

        public override void OnGain(Genome genome)
        {
            genome.Regen = true;
        }

        public override void OnLose(Genome genome)
        {
            genome.Regen = false;
        }
    }
}