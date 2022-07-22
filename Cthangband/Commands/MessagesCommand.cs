using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Let the player scroll through previous messages
    /// </summary>
    [Serializable]
    internal class MessagesCommand : ICommand
    {
        public char Key => 'P';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            MessagesStoreCommand.DoCmdMessages();
        }
    }
}
