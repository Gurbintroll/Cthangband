using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationInvuln : Mutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "You are blessed with fits of invulnerability.";
            HaveMessage = "You occasionally feel invincible.";
            LoseMessage = "You are no longer blessed with fits of invulnerability.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (!player.HasAntiMagic && Program.Rng.DieRoll(5000) == 1)
            {
                saveGame.Disturb(false);
                Profile.Instance.MsgPrint("You feel invincible!");
                Profile.Instance.MsgPrint(null);
                player.SetTimedInvulnerability(player.TimedInvulnerability + Program.Rng.DieRoll(8) + 8);
            }
        }
    }
}