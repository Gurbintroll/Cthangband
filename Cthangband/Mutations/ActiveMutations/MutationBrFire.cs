using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationBrFire : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.CheckIfRacialPowerWorks(20, player.Level, Ability.Constitution, 18))
            {
                return;
            }
            TargetEngine targetEngine = new TargetEngine(player, level);
            Profile.Instance.MsgPrint("You breathe fire...");
            if (targetEngine.GetAimDir(out int dir))
            {
                saveGame.SpellEffects.FireBall(new ProjectFire(SaveGame.Instance.SpellEffects), dir, player.Level * 2, -(1 + (player.Level / 20)));
            }
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 20
                ? "fire breath      (unusable until level 20)"
                : $"fire breath      (cost {lvl}, dam {lvl * 2}, CON based)";
        }

        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "You gain the ability to breathe fire.";
            HaveMessage = "You can breathe fire (dam lvl * 2).";
            LoseMessage = "You lose the ability to breathe fire.";
        }
    }
}