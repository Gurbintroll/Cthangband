// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
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