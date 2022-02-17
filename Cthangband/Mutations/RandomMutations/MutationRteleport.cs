using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationRteleport : Mutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "Your position seems very uncertain...";
            HaveMessage = "You are teleporting randomly.";
            LoseMessage = "Your position seems more certain.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (Program.Rng.DieRoll(5000) != 88)
            {
                return;
            }
            if (player.HasNexusResistance || player.HasAntiTeleport)
            {
                return;
            }
            saveGame.Disturb(false);
            Profile.Instance.MsgPrint("Your position suddenly seems very uncertain...");
            Profile.Instance.MsgPrint(null);
            saveGame.SpellEffects.TeleportPlayer(40);
        }
    }
}