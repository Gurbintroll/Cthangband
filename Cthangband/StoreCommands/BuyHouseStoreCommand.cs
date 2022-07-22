using Cthangband.Commands;
using Cthangband.Enumerations;

namespace Cthangband.StoreCommands
{
    internal class BuyHouseStoreCommand : IStoreCommand

    {
        public char Key => 'r';

        public bool RequiresRerendering => false;

        public void Execute(Player player, Store store)
        {
            store.BuyHouse();
        }

        public bool IsEnabled(Store store)
        {
            return (store.StoreType == StoreType.StoreHome);
        }
    }
}
