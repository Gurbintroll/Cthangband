﻿// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationMagicRes : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You become resistant to magic.";
            HaveMessage = "You are resistant to magic.";
            LoseMessage = "You become susceptible to magic again.";
        }

        public override void OnGain(Genome genome)
        {
            genome.MagicResistance = true;
        }

        public override void OnLose(Genome genome)
        {
            genome.MagicResistance = false;
        }
    }
}