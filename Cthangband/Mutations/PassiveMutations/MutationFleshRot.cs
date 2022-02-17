using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationFleshRot : Mutation
    {
        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "Your flesh is afflicted by a rotting disease!";
            HaveMessage = "Your flesh is rotting (-2 CON, -1 CHR).";
            LoseMessage = "Your flesh is no longer afflicted by a rotting disease!";
            Group = MutationGroup.Skin;
        }

        public override void OnGain(Genome genome)
        {
            genome.ConstitutionBonus -= 2;
            genome.CharismaBonus -= 1;
            genome.SuppressRegen = true;
        }

        public override void OnLose(Genome genome)
        {
            genome.ConstitutionBonus += 2;
            genome.CharismaBonus += 1;
            genome.SuppressRegen = false;
        }
    }
}