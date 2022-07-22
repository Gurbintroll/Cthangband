using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Look in the player's journal for any one of a number of different reasons
    /// </summary>
    [Serializable]
    internal class JournalStoreCommand : IStoreCommand
    {
        public char Key => 'J';

        public bool IsEnabled(Store store) => true;

        public bool RequiresRerendering => true;

        public void Execute(Player player, Store store)
        {
            DoCmdJournal(player);
        }

        public static void DoCmdJournal(Player player)
        {
            // Let the journal itself handle it from here
            Journal journal = new Journal(player);
            journal.ShowMenu();
        }
    }
}
