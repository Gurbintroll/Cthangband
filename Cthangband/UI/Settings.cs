using Cthangband.StaticData;
using Cthangband.Terminal;
using System;

namespace Cthangband.UI
{
    [Serializable]
    internal class Settings
    {
        public bool Bold = true;
        public string Font = "Consolas";
        public bool Italic = false;
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