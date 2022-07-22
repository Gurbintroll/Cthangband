using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Take off an item
    /// </summary>
    [Serializable]
    internal class TakeOffCommand : ICommand
    {
        public char Key => 't';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            TakeOffStoreCommand.DoCmdTakeOff(player);
        }
    }
}
