using System;

namespace Cthangband
{
    [Serializable]
    internal class WildernessRegion
    {
        public Dungeon Dungeon;
        public int RoadMap;
        public int Seed;
        public Town Town;

        public WildernessRegion()
        {
            Dungeon = null;
            Town = null;
            Seed = 0;
            RoadMap = 0;
        }
    }
}