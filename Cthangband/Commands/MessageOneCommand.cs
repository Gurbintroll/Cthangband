using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Show the previous message
    /// </summary>
    [Serializable]
    internal class MessageOneCommand : ICommand, IStoreCommand
    {
        public char Key => 'O';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public bool RequiresRerendering => false;

        public void Execute(Player player, Level level)
        {
            Execute(player);
        }

        public void Execute(Player player)
        {
            Gui.PrintLine($"> {Profile.Instance.MessageStr(0)}", 0, 0);
        }
    }
}
