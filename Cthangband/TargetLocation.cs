using System;

namespace Cthangband
{
    [Serializable]
    internal class TargetLocation : IComparable<TargetLocation>
    {
        public readonly int X;
        public readonly int Y;
        private readonly int _distance;

        public TargetLocation(int y, int x, int distance)
        {
            X = x;
            Y = y;
            _distance = distance;
        }

        public int CompareTo(TargetLocation other)
        {
            return _distance.CompareTo(other._distance);
        }
    }
}