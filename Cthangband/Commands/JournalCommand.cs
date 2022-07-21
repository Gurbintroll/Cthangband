using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Look in the player's journal for any one of a number of different reasons
    /// </summary>
    [Serializable]
    internal class JournalCommand : ICommand, IStoreCommand
    {
        public char Key => 'J';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public bool RequiresRerendering => true;

        public void Execute(Player player, Level level)
        {
            Execute(player);
        }

        public void Execute(Player player)
        {
            // Let the journal itself handle it from here
            Journal journal = new Journal(player);
            journal.ShowMenu();
        }
    }
}
