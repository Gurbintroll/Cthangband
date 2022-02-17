using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationArthritis : Mutation
    {
        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "Your joints suddenly hurt.";
            HaveMessage = "Your joints ache constantly (-3 DEX).";
            LoseMessage = "Your joints stop hurting.";
            Group = MutationGroup.Joints;
        }

        public override void OnGain(Genome genome)
        {
            genome.DexterityBonus -= 3;
        }

        public override void OnLose(Genome genome)
        {
            genome.DexterityBonus += 3;
        }
    }
}