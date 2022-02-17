using System;

namespace Cthangband
{
    [Serializable]
    internal class Room
    {
        public readonly int Dx1;
        public readonly int Dx2;
        public readonly int Dy1;
        public readonly int Dy2;
        public readonly int Level;

        public Room(int dy1, int dy2, int dx1, int dx2, int level)
        {
            Dy1 = dy1;
            Dy2 = dy2;
            Dx1 = dx1;
            Dx2 = dx2;
            Level = level;
        }
    }
}