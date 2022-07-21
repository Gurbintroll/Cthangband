using Cthangband.StaticData;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Look around (using the target code) stopping on anything interesting rather than just
    /// things that can be targeted
    /// </summary>
    [Serializable]
    internal class LookCommand : ICommand
    {
        public char Key => 'l';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (targetEngine.TargetSet(Constants.TargetLook))
            {
                Profile.Instance.MsgPrint(SaveGame.Instance.TargetWho > 0 ? "Target Selected." : "Location Targeted.");
            }
        }
    }
}
