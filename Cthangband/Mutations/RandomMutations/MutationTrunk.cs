// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
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