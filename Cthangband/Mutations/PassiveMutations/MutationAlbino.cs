using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationAlbino : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You turn into an albino! You feel frail...";
            HaveMessage = "You are albino (-4 CON).";
            LoseMessage = "You are no longer an albino!";
        }

        public override void OnGain(Genome genome)
        {
            genome.ConstitutionBonus -= 4;
        }

        public override void OnLose(Genome genome)
        {
            genome.ConstitutionBonus += 4;
        }
    }
}