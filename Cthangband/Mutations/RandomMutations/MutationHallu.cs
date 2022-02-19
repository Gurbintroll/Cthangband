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