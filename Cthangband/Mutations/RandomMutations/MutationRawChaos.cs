using Cthangband.Projection;
using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationRawChaos : Mutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "You feel the universe is less stable around you.";
            HaveMessage = "You occasionally are surrounded with raw chaos.";
            LoseMessage = "You feel the universe is more stable around you.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (player.HasAntiMagic || Program.Rng.DieRoll(8000) != 1)
            {
                return;
            }
            saveGame.Disturb(false);
            Profile.Instance.MsgPrint("You feel the world warping around you!");
            Profile.Instance.MsgPrint(null);
            saveGame.SpellEffects.FireBall(new ProjectChaos(SaveGame.Instance.SpellEffects), 0, player.Level, 8);
        }
    }
}