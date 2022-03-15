// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationPuny : Mutation
    {
        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "Your muscles wither away...";
            HaveMessage = "You are puny (-4 STR).";
            LoseMessage = "Your muscles revert to normal.";
            Group = MutationGroup.Strength;
        }

        public override void OnGain(Genome genome)
        {
            genome.StrengthBonus -= 4;
        }

        public override void OnLose(Genome genome)
        {
            genome.StrengthBonus += 4;
        }
    }
}