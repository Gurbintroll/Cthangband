using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationWeirdMind : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "Your thoughts suddenly take off in strange directions.";
            HaveMessage = "Your mind randomly expands and contracts.";
            LoseMessage = "Your thoughts return to boring paths.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (player.HasAntiMagic || Program.Rng.DieRoll(3000) != 1)
            {
                return;
            }
            if (player.TimedTelepathy > 0)
            {
                Profile.Instance.MsgPrint("Your mind feels cloudy!");
                player.SetTimedTelepathy(0);
            }
            else
            {
                Profile.Instance.MsgPrint("Your mind expands!");
                player.SetTimedTelepathy(player.Level);
            }
        }
    }
}