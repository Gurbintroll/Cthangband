using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationHorns : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "Horns pop forth into your forehead!";
            HaveMessage = "You have horns (dam. 2d6).";
            LoseMessage = "Your horns vanish from your forehead!";
            Dss = 2;
            Ddd = 6;
            NWeight = 15;
            AtkDesc = "horns";
        }

        public override void OnGain(Genome genome)
        {
            genome.NaturalAttacks.Add(this);
        }

        public override void OnLose(Genome genome)
        {
            genome.NaturalAttacks.Remove(this);
        }
    }
}