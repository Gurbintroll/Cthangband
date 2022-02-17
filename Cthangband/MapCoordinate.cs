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