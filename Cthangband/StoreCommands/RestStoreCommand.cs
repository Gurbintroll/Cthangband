using Cthangband.Commands;
using Cthangband.Enumerations;

namespace Cthangband.StoreCommands
{
    internal class RestStoreCommand : IStoreCommand

    {
        public char Key => 'r';

        public bool RequiresRerendering => false;

        public void Execute(Player player, Store store)
        {
            store.Rest();
        }

        public bool IsEnabled(Store store)
        {
            return (store.StoreType == StoreType.StoreHome);
        }
    }
}
