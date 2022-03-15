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