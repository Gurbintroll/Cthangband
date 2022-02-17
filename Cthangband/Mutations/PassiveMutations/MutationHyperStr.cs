using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationHyperStr : Mutation
    {
        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "You turn into a superhuman he-man!";
            HaveMessage = "You are superhumanly strong (+4 STR).";
            LoseMessage = "Your muscles revert to normal.";
            Group = MutationGroup.Strength;
        }

        public override void OnGain(Genome genome)
        {
            genome.StrengthBonus += 4;
        }

        public override void OnLose(Genome genome)
        {
            genome.StrengthBonus -= 4;
        }
    }
}