using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationBersRage : Mutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "You become subject to fits of berserk rage!";
            HaveMessage = "You are subject to berserker fits.";
            LoseMessage = "You are no longer subject to fits of berserk rage!";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (Program.Rng.DieRoll(3000) != 1)
            {
                return;
            }
            saveGame.Disturb(false);
            Profile.Instance.MsgPrint("RAAAAGHH!");
            Profile.Instance.MsgPrint("You feel a fit of rage coming over you!");
            player.SetTimedSuperheroism(player.TimedSuperheroism + 10 + Program.Rng.DieRoll(player.Level));
        }
    }
}