using Cthangband.Projection;
using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationFlatulent : Mutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "You become subject to uncontrollable flatulence.";
            HaveMessage = "You are subject to uncontrollable flatulence.";
            LoseMessage = "You are no longer subject to uncontrollable flatulence.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (Program.Rng.DieRoll(3000) == 13)
            {
                saveGame.Disturb(false);
                Profile.Instance.MsgPrint("BRRAAAP! Oops.");
                Profile.Instance.MsgPrint(null);
                saveGame.SpellEffects.FireBall(new ProjectPois(SaveGame.Instance.SpellEffects), 0, player.Level, 3);
            }
        }
    }
}