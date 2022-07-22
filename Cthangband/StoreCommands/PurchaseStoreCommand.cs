using Cthangband.Commands;

namespace Cthangband.StoreCommands
{
    internal class PurchaseStoreCommand : IStoreCommand
    {
        public char Key => 'g';

        public bool RequiresRerendering => false;

        public void Execute(Player player, Store store)
        {
            store.StorePurchase();
        }

        public bool IsEnabled(Store store) => true;
    }
}
