using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationWraith : Mutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "You start to fade in and out of the physical world.";
            HaveMessage = "You fade in and out of physical reality.";
            LoseMessage = "You are firmly in the physical world.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (player.HasAntiMagic || Program.Rng.DieRoll(3000) != 13)
            {
                return;
            }
            saveGame.Disturb(false);
            Profile.Instance.MsgPrint("You feel insubstantial!");
            Profile.Instance.MsgPrint(null);
            player.SetTimedEtherealness(player.TimedEtherealness + Program.Rng.DieRoll(player.Level / 2) + player.Level / 2);
        }
    }
}