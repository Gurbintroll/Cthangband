﻿using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationBeak : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "Your mouth turns into a sharp, powerful beak!";
            HaveMessage = "You have a beak (dam. 2d4).";
            LoseMessage = "Your mouth reverts to normal!";
            Group = MutationGroup.Mouth;
            DamageDiceSize = 2;
            DamageDiceNumber = 4;
            EquivalentWeaponWeight = 5;
            AttackDescription = "beak";
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