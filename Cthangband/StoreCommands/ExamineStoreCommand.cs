using Cthangband.Commands;

namespace Cthangband.StoreCommands
{
    internal class ExamineStoreCommand : IStoreCommand
    {
        public char Key => 'x';

        public bool RequiresRerendering => false;

        public void Execute(Player player, Store store)
        {
            store.StoreExamine();
        }

        public bool IsEnabled(Store store) => true;
    }
}
