﻿// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Mutations.Base;
using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationMotion : BaseMutation
    {
        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "You move with new assurance.";
            HaveMessage = "Your movements are precise and forceful (+1 STL).";
            LoseMessage = "You move with less assurance.";
        }

        public override void OnGain(Genome genome)
        {
            genome.StealthBonus += 1;
            genome.FreeAction = true;
        }

        public override void OnLose(Genome genome)
        {
            genome.StealthBonus -= 1;
            genome.FreeAction = false;
        }
    }
}