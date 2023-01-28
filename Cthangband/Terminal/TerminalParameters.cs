// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using System;

namespace Cthangband.Terminal
{
    /// <summary>
    /// The set of parameters with which to make a new terminal
    /// </summary>
    [Serializable]
    internal class TerminalParameters
    {
        /// <summary>
        /// Sets or returns whether window text should be bold
        /// </summary>
        public bool FontBold = true;

        /// <summary>
        /// Sets or returns whether window text should be italic
        /// </summary>
        public bool FontItalic = false;

        /// <summary>
        /// The font to use in the terminal window
        /// </summary>
        public string FontName = "Consolas";

        /// <summary>
        /// Sets or returns whether the terminal should be fullscreen
        /// </summary>
        public bool Fullscreen = true;

        /// <summary>
        /// The height of the terminal window when in windowed mode
        /// </summary>
        public int WindowedHeight = 900;

        /// <summary>
        /// The number of rows in the terminal window
        /// </summary>
        public int WindowedWidth = 1600;

        /// <summary>
        /// The title for the terminal window
        /// </summary>
        public string WindowTitle = string.Empty;
    }
}