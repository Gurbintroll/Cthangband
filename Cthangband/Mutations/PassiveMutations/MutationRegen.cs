// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
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
    internal class MutationRegen : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You start regenerating.";
            HaveMessage = "You are regenerating.";
            LoseMessage = "You stop regenerating.";
        }

        public override void OnGain(Genome genome)
        {
            genome.Regen = true;
        }

        public override void OnLose(Genome genome)
        {
            genome.Regen = false;
        }
    }
}