using Cthangband.StaticData;
using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationAttDemon : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You start attracting demons.";
            HaveMessage = "You attract demons.";
            LoseMessage = "You stop attracting demons.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (player.HasAntiMagic || Program.Rng.DieRoll(6666) != 666)
            {
                return;
            }
            bool dSummon;
            if (Program.Rng.DieRoll(6) == 1)
            {
                dSummon = level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, saveGame.Difficulty,
                    Constants.SummonDemon, true);
            }
            else
            {
                dSummon = level.Monsters.SummonSpecific(player.MapY, player.MapX, saveGame.Difficulty,
                    Constants.SummonDemon);
            }
            if (!dSummon)
            {
                return;
            }
            Profile.Instance.MsgPrint("You have attracted a demon!");
            saveGame.Disturb(false);
        }
    }
}