using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationSpeedFlux : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You have become unstuck in time.";
            HaveMessage = "You move faster or slower randomly.";
            LoseMessage = "You are firmly anchored in time.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (Program.Rng.DieRoll(6000) == 1)
            {
                saveGame.Disturb(false);
                if (Program.Rng.DieRoll(2) == 1)
                {
                    Profile.Instance.MsgPrint("Everything around you speeds up.");
                    if (player.TimedHaste > 0)
                    {
                        player.SetTimedHaste(0);
                    }
                    else
                    {
                        player.SetTimedSlow(player.TimedSlow + Program.Rng.DieRoll(30) + 10);
                    }
                }
                else
                {
                    Profile.Instance.MsgPrint("Everything around you slows down.");
                    if (player.TimedSlow > 0)
                    {
                        player.SetTimedSlow(0);
                    }
                    else
                    {
                        player.SetTimedHaste(player.TimedHaste + Program.Rng.DieRoll(30) + 10);
                    }
                }
                Profile.Instance.MsgPrint(null);
            }
        }
    }
}