// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationHallu : Mutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "You are afflicted by a hallucinatory insanity!";
            HaveMessage = "You have a hallucinatory insanity.";
            LoseMessage = "You are no longer afflicted by a hallucinatory insanity!";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (Program.Rng.DieRoll(6400) != 42)
            {
                return;
            }
            if (player.HasChaosResistance)
            {
                return;
            }
            saveGame.Disturb(false);
            player.RedrawNeeded.Set(RedrawFlag.PrExtra);
            player.SetTimedHallucinations(player.TimedHallucinations + Program.Rng.RandomLessThan(50) + 20);
        }
    }
}