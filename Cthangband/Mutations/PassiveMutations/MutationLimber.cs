using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationLimber : Mutation
    {
        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "Your muscles become limber.";
            HaveMessage = "Your body is very limber (+3 DEX).";
            LoseMessage = "Your muscles stiffen.";
            Group = MutationGroup.Joints;
        }

        public override void OnGain(Genome genome)
        {
            genome.DexterityBonus += 3;
        }

        public override void OnLose(Genome genome)
        {
            genome.DexterityBonus -= 3;
        }
    }
}