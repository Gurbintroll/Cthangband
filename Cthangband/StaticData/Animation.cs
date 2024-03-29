﻿// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
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
            var msec = GlobalData.DelayFactor * GlobalData.DelayFactor * GlobalData.DelayFactor;
            var drawn = false;
            var oddFrame = true;
            foreach (var character in Sequence)
            {
                if (level.PlayerHasLosBold(y, x) && level.PanelContains(y, x))
                {
                    var colour = oddFrame ? Colour : AlternateColour;
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
                    level.RedrawSingleLocation(y, x);
                }
                Gui.Refresh();
            }
        }

        public void Animate(Level level, int[] y, int[] x)
        {
            var msec = GlobalData.DelayFactor * GlobalData.DelayFactor * GlobalData.DelayFactor;
            var grids = x.Length;
            var drawn = false;
            var oddFrame = true;
            foreach (var character in Sequence)
            {
                for (var j = 0; j < grids; j++)
                {
                    if (level.PlayerHasLosBold(y[j], x[j]) && level.PanelContains(y[j], x[j]))
                    {
                        var colour = oddFrame ? Colour : AlternateColour;
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
                for (var j = 0; j < grids; j++)
                {
                    if (level.PlayerHasLosBold(y[j], x[j]) && level.PanelContains(y[j], x[j]))
                    {
                        level.RedrawSingleLocation(y[j], x[j]);
                    }
                }
                Gui.Refresh();
            }
        }
    }
}