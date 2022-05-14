// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.StaticData;
using Cthangband.Terminal;
using System;
using System.Drawing;

namespace Cthangband.UI
{
    [Serializable]
    internal class Settings
    {
        public bool Bold = true;
        public string Font = FontFamily.GenericMonospace.Name;
        public bool Italic = false;
        public int LastProfileUsed = 0;
        public int MusicVolume = 100;
        public int Resolution = 0;
        public int SoundVolume = 100;

        public TerminalParameters Parameters()
        {
            return new TerminalParameters
            {
                FontBold = Bold,
                FontItalic = Italic,
                FontName = Font,
                Fullscreen = Resolution == 0,
                WindowTitle = Constants.VersionName + " " + Constants.VersionStamp,
                WindowedHeight = Resolution == 0 ? 450 : (Resolution + 2) * 90,
                WindowedWidth = Resolution == 0 ? 800 : (Resolution + 2) * 160
            };
        }
    }
}