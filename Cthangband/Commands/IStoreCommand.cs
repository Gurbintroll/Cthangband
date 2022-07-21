namespace Cthangband.Commands
{
    internal interface IStoreCommand
    {
        char Key { get; }
        bool IsEnabled { get; }
        bool RequiresRerendering { get; }
        void Execute(Player player);
    }
}
