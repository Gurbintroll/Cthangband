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