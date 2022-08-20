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

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationScales : BaseMutation
    {
        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "Your skin turns into black scales!";
            HaveMessage = "Your skin has turned into scales (-1 CHR, +10 AC).";
            LoseMessage = "Your scales vanish!";
            Group = MutationGroup.Skin;
        }

        public override void OnGain(Genome genome)
        {
            genome.CharismaBonus -= 1;
            genome.ArmourClassBonus += 10;
        }

        public override void OnLose(Genome genome)
        {
            genome.CharismaBonus += 1;
            genome.ArmourClassBonus -= 10;
        }
    }
}