using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Show the previous message
    /// </summary>
    [Serializable]
    internal class MessageOneStoreCommand : IStoreCommand
    {
        public char Key => 'O';

        public bool IsEnabled(Store store) => true;

        public bool RequiresRerendering => false;

        public void Execute(Player player, Store store)
        {
            DoCmdMessageOne();
        }

        public static void DoCmdMessageOne()
        {
            Gui.PrintLine($"> {Profile.Instance.MessageStr(0)}", 0, 0);
        }
    }
}
