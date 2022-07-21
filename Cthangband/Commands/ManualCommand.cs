using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Show the game manual
    /// </summary>
    [Serializable]
    internal class ManualCommand : ICommand, IStoreCommand
    {
        public char Key => 'h';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public bool RequiresRerendering => false;

        public void Execute(Player player, Level level)
        {
            Execute(player);
        }

        public void Execute(Player player)
        {
            Gui.ShowManual();
        }
    }
}
