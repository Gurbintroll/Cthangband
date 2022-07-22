using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Show the game manual
    /// </summary>
    [Serializable]
    internal class ManualStoreCommand : IStoreCommand
    {
        public char Key => 'h';

        public bool IsEnabled(Store store) => true;

        public bool RequiresRerendering => false;

        public void Execute(Player player, Store store)
        {
            DoCmdManual();
        }

        public static void DoCmdManual()
        {
            Gui.ShowManual();
        }
    }
}
