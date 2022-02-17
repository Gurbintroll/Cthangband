using System;

namespace Cthangband
{
    [Serializable]
    internal class Quest
    {
        public bool Discovered;
        public int Dungeon;
        public int Killed;
        public int Level;
        public int RIdx;
        public int ToKill;

        public bool IsActive => (Level != 0 && Killed < ToKill);
    }
}