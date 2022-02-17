using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationCowardice : Mutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "You become an incredible coward!";
            HaveMessage = "You are subject to cowardice.";
            LoseMessage = "You are no longer an incredible coward!";
            Group = MutationGroup.Bravery;
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (Program.Rng.DieRoll(3000) != 13)
            {
                return;
            }
            if (player.HasFearResistance || player.TimedHeroism != 0 || player.TimedSuperheroism != 0)
            {
                return;
            }
            saveGame.Disturb(false);
            Profile.Instance.MsgPrint("It's so dark... so scary!");
            player.RedrawFlags |= RedrawFlag.PrAfraid;
            player.TimedFear = player.TimedFear + 13 + Program.Rng.DieRoll(26);
        }
    }
}