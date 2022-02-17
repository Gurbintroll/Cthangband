using Cthangband.Debug;
using Cthangband.Enumerations;
using Cthangband.UI;
using System;

namespace Cthangband.StaticData
{
    [Serializable]
    internal class Animation : EntityType
    {
        public Colour AlternateColour
        {
            get;
            set;
        }

        public string Sequence
        {
            get;
            set;
        }

        public void Animate(Level level, int y, int x)
        {
            int msec = GlobalData.DelayFactor * GlobalData.DelayFactor * GlobalData.DelayFactor;
            bool drawn = false;
            bool oddFrame = true;
            foreach (char character in Sequence)
            {
                if (level.PlayerHasLosBold(y, x) && level.PanelContains(y, x))
                {
                    Colour colour = oddFrame ? Colour : AlternateColour;
                    level.PrintCharacterAtMapLocation(character, colour, y, x);
                    drawn = true;
                }
                if (drawn)
                {
                    Gui.Refresh();
                    Gui.Pause(msec);
                }
                oddFrame = !oddFrame;
            }
            if (drawn)
            {
                if (level.PlayerHasLosBold(y, x) && level.PanelContains(y, x))
                {
                    level.LightSpot(y, x);
                }
                Gui.Refresh();
            }
        }

        public void Animate(Level level, int[] y, int[] x)
        {
            int msec = GlobalData.DelayFactor * GlobalData.DelayFactor * GlobalData.DelayFactor;
            int grids = x.Length;
            bool drawn = false;
            bool oddFrame = true;
            foreach (char character in Sequence)
            {
                for (int j = 0; j < grids; j++)
                {
                    if (level.PlayerHasLosBold(y[j], x[j]) && level.PanelContains(y[j], x[j]))
                    {
                        Colour colour = oddFrame ? Colour : AlternateColour;
                        level.PrintCharacterAtMapLocation(character, colour, y[j], x[j]);
                        drawn = true;
                    }
                }
                if (drawn)
                {
                    Gui.Refresh();
                    Gui.Pause(msec);
                }
                oddFrame = !oddFrame;
            }
            if (drawn)
            {
                for (int j = 0; j < grids; j++)
                {
                    if (level.PlayerHasLosBold(y[j], x[j]) && level.PanelContains(y[j], x[j]))
                    {
                        level.LightSpot(y[j], x[j]);
                    }
                }
                Gui.Refresh();
            }
        }
    }
}