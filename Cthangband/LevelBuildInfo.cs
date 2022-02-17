using System;

namespace Cthangband
{
    [Serializable]
    internal class LevelBuildInfo
    {
        public readonly MapCoordinate[] Cent;
        public readonly MapCoordinate[] Door;
        public readonly bool[][] RoomMap;
        public readonly MapCoordinate[] Tunn;
        public readonly MapCoordinate[] Wall;
        public int CentN;
        public int ColRooms;
        public bool Crowded;
        public int DoorN;
        public int RowRooms;
        public int TunnN;
        public int WallN;

        public LevelBuildInfo()
        {
            Cent = new MapCoordinate[LevelFactory.CentMax];
            for (int i = 0; i < LevelFactory.CentMax; i++)
            {
                Cent[i] = new MapCoordinate();
            }
            Door = new MapCoordinate[LevelFactory.DoorMax];
            for (int i = 0; i < LevelFactory.DoorMax; i++)
            {
                Door[i] = new MapCoordinate();
            }
            Wall = new MapCoordinate[LevelFactory.WallMax];
            for (int i = 0; i < LevelFactory.WallMax; i++)
            {
                Wall[i] = new MapCoordinate();
            }
            Tunn = new MapCoordinate[LevelFactory.TunnMax];
            for (int i = 0; i < LevelFactory.TunnMax; i++)
            {
                Tunn[i] = new MapCoordinate();
            }
            RoomMap = new bool[LevelFactory.MaxRoomsRow][];
            for (int i = 0; i < LevelFactory.MaxRoomsRow; i++)
            {
                RoomMap[i] = new bool[LevelFactory.MaxRoomsCol];
            }
        }
    }
}