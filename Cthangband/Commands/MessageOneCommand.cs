using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Show the previous message
    /// </summary>
    [Serializable]
    internal class MessageOneCommand : ICommand
    {
        public char Key => 'O';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            MessageOneStoreCommand.DoCmdMessageOne();
        }
    }
}
