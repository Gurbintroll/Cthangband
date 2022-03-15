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
    internal class MutationXtraFat : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You become sickeningly fat!";
            HaveMessage = "You are extremely fat (+2 CON, -2 speed).";
            LoseMessage = "You benefit from a miracle diet!";
        }

        public override void OnGain(Genome genome)
        {
            genome.ConstitutionBonus += 2;
            genome.SpeedBonus -= 2;
        }

        public override void OnLose(Genome genome)
        {
            genome.ConstitutionBonus -= 2;
            genome.SpeedBonus += 2;
        }
    }
}