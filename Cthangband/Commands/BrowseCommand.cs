using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Browse a book
    /// </summary>
    [Serializable]
    internal class BrowseCommand : ICommand
    {
        public char Key => 'b';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            BrowseStoreCommand.DoCmdBrowse(player);
        }
    }
}
