using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationMagicRes : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You become resistant to magic.";
            HaveMessage = "You are resistant to magic.";
            LoseMessage = "You become susceptible to magic again.";
        }

        public override void OnGain(Genome genome)
        {
            genome.MagicResistance = true;
        }

        public override void OnLose(Genome genome)
        {
            genome.MagicResistance = false;
        }
    }
}