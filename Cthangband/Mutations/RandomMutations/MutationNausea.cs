using Cthangband.StaticData;
using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationNausea : Mutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "Your stomach starts to roil nauseously.";
            HaveMessage = "You have a seriously upset stomach.";
            LoseMessage = "Your stomach stops roiling.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (player.HasSlowDigestion || Program.Rng.DieRoll(9000) != 1)
            {
                return;
            }
            saveGame.Disturb(false);
            Profile.Instance.MsgPrint("Your stomach roils, and you lose your lunch!");
            Profile.Instance.MsgPrint(null);
            player.SetFood(Constants.PyFoodWeak);
        }
    }
}