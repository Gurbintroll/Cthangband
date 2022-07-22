using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Show the player what a particular symbol represents
    /// </summary>
    [Serializable]
    internal class QuerySymbolCommand : ICommand
    {
        public char Key => '/';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            QuerySymbolStoreCommand.DoCmdQuerySymbol();
        }
    }
}
