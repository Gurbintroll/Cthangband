using System;

namespace Cthangband
{
    [Serializable]
    internal class NavigationState
    {
        public bool _findBreakleft;
        public bool _findBreakright;
        public int _findCurrent;
        public bool _findOpenarea;
        public int _findPrevdir;
        private readonly int[] _chome = { 0, 8, 9, 10, 7, 0, 11, 6, 5, 4 };
        private readonly int[] _cycle = { 1, 2, 3, 6, 9, 8, 7, 4, 1, 2, 3, 6, 9, 8, 7, 4, 1 };

        public int[] Chome
        {
            get => _chome;
        }

        public int[] Cycle
        {
            get => _cycle;
        }
    }
}