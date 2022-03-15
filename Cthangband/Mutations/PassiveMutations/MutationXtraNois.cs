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
    internal class MutationXtraNois : Mutation
    {
        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "You start making strange noise!";
            HaveMessage = "You make a lot of strange noise (-3 stealth).";
            LoseMessage = "You stop making strange noise!";
        }

        public override void OnGain(Genome genome)
        {
            genome.StealthBonus -= 3;
        }

        public override void OnLose(Genome genome)
        {
            genome.StealthBonus += 3;
        }
    }
}