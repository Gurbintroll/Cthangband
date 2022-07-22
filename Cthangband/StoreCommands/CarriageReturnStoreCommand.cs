using Cthangband.Commands;

namespace Cthangband.StoreCommands
{
    internal class CarriageReturnStoreCommand : IStoreCommand
    {
        public char Key => '\r';

        public bool RequiresRerendering => false;

        public void Execute(Player player, Store store)
        {
        }

        public bool IsEnabled(Store store) => true;
    }
}
