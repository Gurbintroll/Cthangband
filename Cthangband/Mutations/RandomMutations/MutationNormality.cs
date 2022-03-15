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
    internal class MutationNormality : Mutation
    {
        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "You feel strangely normal.";
            HaveMessage = "You may be chaotic, but you're recovering.";
            LoseMessage = "You feel normally strange.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (Program.Rng.DieRoll(5000) == 1)
            {
                player.Dna.LoseMutation();
            }
        }
    }
}