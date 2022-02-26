using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationTrunk : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "Your nose grows into an elephant-like trunk.";
            HaveMessage = "You have an elephantine trunk (dam 1d4).";
            LoseMessage = "Your nose returns to a normal length.";
            Group = MutationGroup.Mouth;
            DamageDiceSize = 1;
            DamageDiceNumber = 4;
            EquivalentWeaponWeight = 35;
            AttackDescription = "trunk";
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