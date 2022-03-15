// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using System;

namespace Cthangband
{
    /// <summary>
    /// A simple co-ordinate, stored in a reference type so it can be passed from function to
    /// function and have its properties updated
    /// </summary>
    [Serializable]
    internal class MapCoordinate
    {
        public int X;
        public int Y;

        public MapCoordinate()
        {
        }

        public MapCoordinate(MapCoordinate original)
        {
            X = original.X;
            Y = original.Y;
        }

        public MapCoordinate(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}