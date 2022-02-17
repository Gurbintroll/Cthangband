using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationWartSkin : Mutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "Disgusting warts appear everywhere on you!";
            HaveMessage = "Your skin is covered with warts (-2 CHR, +5 AC).";
            LoseMessage = "Your warts disappear!";
            Group = MutationGroup.Skin;
        }

        public override void OnGain(Genome genome)
        {
            genome.CharismaBonus -= 2;
            genome.ArmourClassBonus += 5;
        }

        public override void OnLose(Genome genome)
        {
            genome.CharismaBonus += 2;
            genome.ArmourClassBonus -= 5;
        }
    }
}