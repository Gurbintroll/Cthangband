using Cthangband.StaticData;
using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationAttDragon : Mutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "You start attracting dragons.";
            HaveMessage = "You attract dragons.";
            LoseMessage = "You stop attracting dragons.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (player.HasAntiMagic || Program.Rng.DieRoll(3000) != 13)
            {
                return;
            }
            bool dSummon;
            if (Program.Rng.DieRoll(5) == 1)
            {
                dSummon = level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, saveGame.Difficulty,
                    Constants.SummonDragon, true);
            }
            else
            {
                dSummon = level.Monsters.SummonSpecific(player.MapY, player.MapX, saveGame.Difficulty,
                    Constants.SummonDragon);
            }
            if (!dSummon)
            {
                return;
            }
            Profile.Instance.MsgPrint("You have attracted a dragon!");
            saveGame.Disturb(false);
        }
    }
}