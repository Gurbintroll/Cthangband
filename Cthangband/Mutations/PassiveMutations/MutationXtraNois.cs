using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationXtraNois : Mutation
    {
        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "You start making strange noise!";
            HaveMessage = "You make a lot of strange noise (-3 stealth).";
            LoseMessage = "You stop making strange noise!";
        }

        public override void OnGain(Genome genome)
        {
            genome.StealthBonus -= 3;
        }

        public override void OnLose(Genome genome)
        {
            genome.StealthBonus += 3;
        }
    }
}