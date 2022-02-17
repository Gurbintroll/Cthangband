using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationXtraFat : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You become sickeningly fat!";
            HaveMessage = "You are extremely fat (+2 CON, -2 speed).";
            LoseMessage = "You benefit from a miracle diet!";
        }

        public override void OnGain(Genome genome)
        {
            genome.ConstitutionBonus += 2;
            genome.SpeedBonus -= 2;
        }

        public override void OnLose(Genome genome)
        {
            genome.ConstitutionBonus -= 2;
            genome.SpeedBonus += 2;
        }
    }
}