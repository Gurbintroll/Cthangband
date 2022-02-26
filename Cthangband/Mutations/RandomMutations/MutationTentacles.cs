using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationTentacles : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "Evil-looking tentacles sprout from your sides.";
            HaveMessage = "You have evil looking tentacles (dam 2d5).";
            LoseMessage = "Your tentacles vanish from your sides.";
            DamageDiceSize = 2;
            DamageDiceNumber = 5;
            EquivalentWeaponWeight = 5;
            AttackDescription = "tentacles";
            MutationAttackType = MutationAttackType.Hellfire;
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