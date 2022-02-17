using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationChaosGift : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You attract the notice of a chaos deity!";
            HaveMessage = "Chaos deities give you gifts.";
            LoseMessage = "You lose the attention of the chaos deities.";
        }

        public override void OnGain(Genome genome)
        {
            genome.ChaosGift = true;
        }

        public override void OnLose(Genome genome)
        {
            genome.ChaosGift = false;
        }
    }
}