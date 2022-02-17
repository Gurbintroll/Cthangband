using Cthangband.StaticData;
using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationAttAnimal : Mutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "You start attracting animals.";
            HaveMessage = "You attract animals.";
            LoseMessage = "You stop attracting animals.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (player.HasAntiMagic || Program.Rng.DieRoll(7000) != 1)
            {
                return;
            }
            bool aSummon;
            if (Program.Rng.DieRoll(3) == 1)
            {
                aSummon = level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, saveGame.Difficulty,
                    Constants.SummonAnimal, true);
            }
            else
            {
                aSummon = level.Monsters.SummonSpecific(player.MapY, player.MapX, saveGame.Difficulty,
                    Constants.SummonAnimal);
            }
            if (!aSummon)
            {
                return;
            }
            Profile.Instance.MsgPrint("You have attracted an animal!");
            saveGame.Disturb(false);
        }
    }
}