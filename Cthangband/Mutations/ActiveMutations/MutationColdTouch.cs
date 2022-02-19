using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationColdTouch : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.CheckIfRacialPowerWorks(2, 2, Ability.Constitution, 11))
            {
                return;
            }
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetRepDir(out int dir))
            {
                return;
            }
            int y = player.MapY + level.KeypadDirectionYOffset[dir];
            int x = player.MapX + level.KeypadDirectionXOffset[dir];
            GridTile cPtr = level.Grid[y][x];
            if (cPtr.Monster == 0)
            {
                Profile.Instance.MsgPrint("You wave your hands in the air.");
                return;
            }
            saveGame.SpellEffects.FireBolt(new ProjectCold(SaveGame.Instance.SpellEffects), dir, 2 * player.Level);
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 2 ? "cold touch       (unusable until level 2)" : "cold touch       (cost 2, CON based)";
        }

        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "Your hands get very cold.";
            HaveMessage = "You can freeze things with a touch.";
            LoseMessage = "Your hands warm up.";
        }
    }
}