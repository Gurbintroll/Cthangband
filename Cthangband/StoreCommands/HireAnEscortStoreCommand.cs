using Cthangband.Commands;
using Cthangband.Enumerations;
using System;

namespace Cthangband.StoreCommands
{
    internal class HireAnEscortStoreCommand : IStoreCommand

    {
        public char Key => 'r';

        public bool RequiresRerendering => false;

        public void Execute(Player player, Store store)
        {
            store.HireAnEscort();
        }

        public bool IsEnabled(Store store)
        {
            return (store.StoreType == StoreType.StoreGeneral);
        }
    }
}
