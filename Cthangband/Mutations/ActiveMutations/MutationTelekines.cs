using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationTelekines : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.CheckIfRacialPowerWorks(9, 9, Ability.Wisdom, 14))
            {
                return;
            }
            TargetEngine targetEngine = new TargetEngine(player, level);
            Profile.Instance.MsgPrint("You concentrate...");
            if (targetEngine.GetAimDir(out int dir))
            {
                saveGame.CommandEngine.SummonObject(dir, player.Level * 10, true);
            }
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 9 ? "telekinesis      (unusable until level 9)" : "telekinesis      (cost 9, WIS based)";
        }

        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You gain the ability to move objects telekinetically.";
            HaveMessage = "You are telekinetic.";
            LoseMessage = "You lose the ability to move objects telekinetically.";
        }
    }
}