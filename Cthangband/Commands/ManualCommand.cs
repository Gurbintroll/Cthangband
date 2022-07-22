using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Show the game manual
    /// </summary>
    [Serializable]
    internal class ManualCommand : ICommand
    {
        public char Key => 'h';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            ManualStoreCommand.DoCmdManual();
        }
    }
}
