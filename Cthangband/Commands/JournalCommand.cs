using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Look in the player's journal for any one of a number of different reasons
    /// </summary>
    [Serializable]
    internal class JournalCommand : ICommand
    {
        public char Key => 'J';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            JournalStoreCommand.DoCmdJournal(player);
        }
    }
}
