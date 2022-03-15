// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationChaosGift : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You attract the notice of a chaos deity!";
            HaveMessage = "Chaos deities give you gifts.";
            LoseMessage = "You lose the attention of the chaos deities.";
        }

        public override void OnGain(Genome genome)
        {
            genome.ChaosGift = true;
        }

        public override void OnLose(Genome genome)
        {
            genome.ChaosGift = false;
        }
    }
}