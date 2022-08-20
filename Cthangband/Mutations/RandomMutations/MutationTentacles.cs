﻿// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Mutations.Base;
using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationTentacles : BaseMutation
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