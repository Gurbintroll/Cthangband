using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationScorTail : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You grow a scorpion tail!";
            HaveMessage = "You have a scorpion tail (poison, 3d7).";
            LoseMessage = "You lose your scorpion tail!";
            DamageDiceSize = 3;
            DamageDiceNumber = 7;
            EquivalentWeaponWeight = 5;
            AttackDescription = "tail";
            MutationAttackType = MutationAttackType.Poison;
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