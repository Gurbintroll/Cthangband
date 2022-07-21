using Cthangband.StaticData;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Select a target in advance for attacks. Note that this does not cost any in-game time
    /// </summary>
    [Serializable]
    internal class TargetCommand : ICommand
    {
        public char Key => '*';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (targetEngine.TargetSet(Constants.TargetKill))
            {
                Profile.Instance.MsgPrint(SaveGame.Instance.TargetWho > 0 ? "Target Selected." : "Location Targeted.");
            }
            else
            {
                Profile.Instance.MsgPrint("Target Aborted.");
            }
        }
    }
}
