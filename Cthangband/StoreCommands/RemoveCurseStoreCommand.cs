using Cthangband.Commands;
using Cthangband.Enumerations;

namespace Cthangband.StoreCommands
{
    internal class RemoveCurseStoreCommand : IStoreCommand

    {
        public char Key => 'r';

        public bool RequiresRerendering => false;

        public void Execute(Player player, Store store)
        {
            store.RemoveCurse();
        }

        public bool IsEnabled(Store store)
        {
            return (store.StoreType == StoreType.StoreTemple);
        }
    }
}
