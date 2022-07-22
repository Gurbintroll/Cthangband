using Cthangband.Commands;

namespace Cthangband.StoreCommands
{
    internal class SellStoreCommand : IStoreCommand
    {
        public char Key => 'd';

        public bool RequiresRerendering => false;

        public void Execute(Player player, Store store)
        {
            store.StoreSell();
        }

        public bool IsEnabled(Store store) => true;
    }
}
