using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationPanicHit : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!SaveGame.Instance.CommandEngine.CheckIfRacialPowerWorks(10, 12, Ability.Dexterity, 14))
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
            if (level.Grid[y][x].Monster != 0)
            {
                SaveGame.Instance.CommandEngine.PlayerAttackMonster(y, x);
                SaveGame.Instance.SpellEffects.TeleportPlayer(30);
            }
            else
            {
                Profile.Instance.MsgPrint("You don't see any monster in this direction");
                Profile.Instance.MsgPrint(null);
            }
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 10 ? "panic hit        (unusable until level 10)" : "panic hit        (cost 12, DEX based)";
        }

        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You suddenly understand how thieves feel.";
            HaveMessage = "You can run for your life after hitting something.";
            LoseMessage = "You no longer feel jumpy.";
        }
    }
}