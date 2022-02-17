using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationScales : Mutation
    {
        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "Your skin turns into black scales!";
            HaveMessage = "Your skin has turned into scales (-1 CHR, +10 AC).";
            LoseMessage = "Your scales vanish!";
            Group = MutationGroup.Skin;
        }

        public override void OnGain(Genome genome)
        {
            genome.CharismaBonus -= 1;
            genome.ArmourClassBonus += 10;
        }

        public override void OnLose(Genome genome)
        {
            genome.CharismaBonus += 1;
            genome.ArmourClassBonus -= 10;
        }
    }
}