using Cthangband.StaticData;
using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Show the player what a particular symbol represents
    /// </summary>
    [Serializable]
    internal class QuerySymbolStoreCommand : IStoreCommand
    {
        public char Key => '/';

        public bool IsEnabled(Store store) => true;

        public bool RequiresRerendering => false;

        public void Execute(Player player, Store store)
        {
            DoCmdQuerySymbol();
        }

        public static void DoCmdQuerySymbol()
        {
            int index;
            // Get the symbol
            if (!Gui.GetCom("Enter character to be identified: ", out char symbol))
            {
                return;
            }
            // Run through the identification array till we find the symbol
            for (index = 0; GlobalData.SymbolIdentification[index] != null; ++index)
            {
                if (symbol == GlobalData.SymbolIdentification[index][0])
                {
                    break;
                }
            }
            // Display the symbol and its idenfitication
            string buf = GlobalData.SymbolIdentification[index] != null
                ? $"{symbol} - {GlobalData.SymbolIdentification[index].Substring(2)}."
                : $"{symbol} - Unknown Symbol";
            Profile.Instance.MsgPrint(buf);
        }
    }
}
