using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationElecTouc : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "Electricity starts running through you!";
            HaveMessage = "Electricity is running through your veins.";
            LoseMessage = "Electricity stops running through you.";
        }

        public override void OnGain(Genome genome)
        {
            genome.ElecHit = true;
        }

        public override void OnLose(Genome genome)
        {
            genome.ElecHit = false;
        }
    }
}