namespace Cthangband.Commands
{
    internal interface IStoreCommand
    {
        char Key { get; }
        bool IsEnabled(Store store);
        bool RequiresRerendering { get; }
        void Execute(Player player, Store store);
    }
}
