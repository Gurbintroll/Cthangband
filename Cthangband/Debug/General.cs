using Cthangband.Enumerations;
using Cthangband.UI;
using System.Diagnostics;

namespace Cthangband.Debug
{
    internal static class General
    {
        [Conditional("DEBUG")]
        public static void PrintDebugOption()
        {
            Gui.Print(Colour.Yellow, "                            Press 'd' for debug menu.", 28, 0);
        }

        [Conditional("DEBUG")]
        public static void ShowDebugMenu()
        {
            DebugMenu menu = new DebugMenu();
            menu.ShowDialog();
        }

        [Conditional("DEBUG")]
        internal static void CheckDebugStatus()
        {
            if (!Debugger.IsAttached)
            {
                Program.Quit("Debug version running outside IDE!");
            }
        }
    }
}