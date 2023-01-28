// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.StaticData;
using System;

namespace Cthangband
{
    internal class LevelFactory
    {
        public const int CentMax = 100;
        public const int DoorMax = 200;
        public const int MaxRoomsCol = Level.MaxWid / _blockWid;
        public const int MaxRoomsRow = Level.MaxHgt / _blockHgt;
        public const int SafeMaxAttempts = 5000;
        public const int TunnMax = 900;
        public const int WallMax = 500;

        private const int _allocSetBoth = 3;
        private const int _allocSetCorr = 1;
        private const int _allocSetRoom = 2;
        private const int _allocTypGold = 4;
        private const int _allocTypObject = 5;
        private const int _allocTypRubble = 1;
        private const int _allocTypTrap = 3;
        private const int _blockHgt = 21;
        private const int _blockWid = 11;
        private const int _darkEmpty = 5;
        private const int _dunAmtGold = 3;
        private const int _dunAmtItem = 3;
        private const int _dunAmtRoom = 9;
        private const int _dunDest = 18;
        private const int _dunRooms = 50;
        private const int _dunStrDen = 5;
        private const int _dunStrMag = 3;
        private const int _dunStrMc = 90;
        private const int _dunStrQc = 40;
        private const int _dunStrQua = 2;
        private const int _dunStrRng = 2;
        private const int _dunTunChg = 30;
        private const int _dunTunCon = 15;
        private const int _dunTunJct = 90;
        private const int _dunTunPen = 25;
        private const int _dunTunRnd = 10;
        private const int _dunUnusual = 194;
        private const int _emptyLevel = 15;
        private const int _smallLevel = 3;

        private readonly Room[] _room =
        {
            new Room(0, 0, 0, 0, 0), new Room(0, 0, -1, 1, 1), new Room(0, 0, -1, 1, 1), new Room(0, 0, -1, 1, 3),
            new Room(0, 0, -1, 1, 3), new Room(0, 0, -1, 1, 5), new Room(0, 0, -1, 1, 5), new Room(0, 1, -1, 1, 5),
            new Room(-1, 2, -2, 3, 10), new Room(0, 1, -1, 1, 1)
        };

        private LevelBuildInfo _dun;
        private Level _level;

        public LevelFactory(Level level)
        {
            _level = level;
        }

        public void GenerateNewLevel()
        {
            for (var num = 0; ; num++)
            {
                var okay = true;
                _level.OMax = 1;
                int i;
                for (i = 0; i < Level.MaxHgt; i++)
                {
                    _level.Grid[i] = new GridTile[Level.MaxWid];
                    for (var j = 0; j < Level.MaxWid; j++)
                    {
                        _level.Grid[i][j] = new GridTile();
                        if (SaveGame.Instance.CurrentDepth == 0)
                        {
                            _level.Grid[i][j].SetBackgroundFeature("Grass");
                        }
                        else if (SaveGame.Instance.Wilderness[SaveGame.Instance.Player.WildernessY][SaveGame.Instance.Player.WildernessX].Dungeon.Tower)
                        {
                            _level.Grid[i][j].SetBackgroundFeature("TowerFloor");
                        }
                        else
                        {
                            _level.Grid[i][j].SetBackgroundFeature("DungeonFloor");
                        }
                    }
                }
                _level.PanelRowMin = 0;
                _level.PanelRowMax = 0;
                _level.PanelColMin = 0;
                _level.PanelColMax = 0;
                if (SaveGame.Instance.CurrentDepth == 0)
                {
                    if (SaveGame.Instance.Wilderness[SaveGame.Instance.Player.WildernessY][SaveGame.Instance.Player.WildernessX]
                            .Town != null)
                    {
                        SaveGame.Instance.CurTown =
                            SaveGame.Instance.Wilderness[SaveGame.Instance.Player.WildernessY][SaveGame.Instance.Player.WildernessX]
                                .Town;
                        SaveGame.Instance.DungeonDifficulty = 0;
                        _level.Monsters.DunBias = 0;
                        if (SaveGame.Instance.Wilderness[SaveGame.Instance.Player.WildernessY][SaveGame.Instance.Player.WildernessX]
                                .Town.Char == 'K')
                        {
                            SaveGame.Instance.DungeonDifficulty = 35;
                            _level.Monsters.DunBias = Constants.SummonCthuloid;
                        }
                    }
                    else if (SaveGame.Instance.Wilderness[SaveGame.Instance.Player.WildernessY][
                                 SaveGame.Instance.Player.WildernessX].Dungeon != null)
                    {
                        SaveGame.Instance.DungeonDifficulty =
                            SaveGame.Instance.Wilderness[SaveGame.Instance.Player.WildernessY][SaveGame.Instance.Player.WildernessX]
                                .Dungeon.Offset / 2;
                        if (SaveGame.Instance.DungeonDifficulty < 4)
                        {
                            SaveGame.Instance.DungeonDifficulty = 4;
                        }
                        _level.Monsters.DunBias =
                            SaveGame.Instance.Wilderness[SaveGame.Instance.Player.WildernessY][SaveGame.Instance.Player.WildernessX]
                                .Dungeon.Bias;
                    }
                    else
                    {
                        SaveGame.Instance.DungeonDifficulty = 2;
                        _level.Monsters.DunBias = Constants.SummonAnimal;
                    }
                }
                else
                {
                    SaveGame.Instance.DungeonDifficulty = SaveGame.Instance.CurDungeon.Offset;
                    _level.Monsters.DunBias = SaveGame.Instance.CurDungeon.Bias;
                }
                _level.MonsterLevel = SaveGame.Instance.Difficulty;
                _level.ObjectLevel = SaveGame.Instance.Difficulty;
                _level.SpecialTreasure = false;
                _level.SpecialDanger = false;
                _level.TreasureRating = 0;
                _level.DangerRating = 0;
                if (SaveGame.Instance.CurrentDepth == 0)
                {
                    _level.CurHgt = Constants.ScreenHgt;
                    _level.CurWid = Constants.ScreenWid;
                    _level.MaxPanelRows = (_level.CurHgt / Constants.ScreenHgt * 2) - 2;
                    _level.MaxPanelCols = (_level.CurWid / Constants.ScreenWid * 2) - 2;
                    _level.PanelRow = _level.MaxPanelRows;
                    _level.PanelCol = _level.MaxPanelCols;
                    if (SaveGame.Instance.Wilderness[SaveGame.Instance.Player.WildernessY][SaveGame.Instance.Player.WildernessX]
                            .Town != null)
                    {
                        TownGen();
                    }
                    else
                    {
                        WildernessGen();
                    }
                }
                else
                {
                    if (SaveGame.Instance.CurDungeon.Tower)
                    {
                        _level.CurHgt = Constants.ScreenHgt;
                        _level.CurWid = Constants.ScreenWid;
                        _level.MaxPanelRows = 0;
                        _level.MaxPanelCols = 0;
                        _level.PanelRow = 0;
                        _level.PanelCol = 0;
                    }
                    else
                    {
                        if (Program.Rng.DieRoll(_smallLevel) == 1)
                        {
                            var tester1 = Program.Rng.DieRoll(Level.MaxHgt / Constants.ScreenHgt);
                            var tester2 = Program.Rng.DieRoll(Level.MaxWid / Constants.ScreenWid);
                            _level.CurHgt = tester1 * Constants.ScreenHgt;
                            _level.CurWid = tester2 * Constants.ScreenWid;
                            _level.MaxPanelRows = (_level.CurHgt / Constants.ScreenHgt * 2) - 2;
                            _level.MaxPanelCols = (_level.CurWid / Constants.ScreenWid * 2) - 2;
                            _level.PanelRow = _level.MaxPanelRows;
                            _level.PanelCol = _level.MaxPanelCols;
                        }
                        else
                        {
                            _level.CurHgt = Level.MaxHgt;
                            _level.CurWid = Level.MaxWid;
                            _level.MaxPanelRows = (_level.CurHgt / Constants.ScreenHgt * 2) - 2;
                            _level.MaxPanelCols = (_level.CurWid / Constants.ScreenWid * 2) - 2;
                            _level.PanelRow = _level.MaxPanelRows;
                            _level.PanelCol = _level.MaxPanelCols;
                        }
                    }
                    if (!UndergroundGen())
                    {
                        okay = false;
                    }
                }
                if (_level.TreasureRating > 100)
                {
                    _level.TreasureFeeling = 2;
                }
                else if (_level.TreasureRating > 80)
                {
                    _level.TreasureFeeling = 3;
                }
                else if (_level.TreasureRating > 60)
                {
                    _level.TreasureFeeling = 4;
                }
                else if (_level.TreasureRating > 40)
                {
                    _level.TreasureFeeling = 5;
                }
                else if (_level.TreasureRating > 30)
                {
                    _level.TreasureFeeling = 6;
                }
                else if (_level.TreasureRating > 20)
                {
                    _level.TreasureFeeling = 7;
                }
                else if (_level.TreasureRating > 10)
                {
                    _level.TreasureFeeling = 8;
                }
                else if (_level.TreasureRating > 0)
                {
                    _level.TreasureFeeling = 9;
                }
                else
                {
                    _level.TreasureFeeling = 10;
                }
                if (_level.SpecialTreasure)
                {
                    _level.TreasureRating = 1;
                }
                if (_level.DangerRating > 100)
                {
                    _level.DangerFeeling = 2;
                }
                else if (_level.DangerRating > 80)
                {
                    _level.DangerFeeling = 3;
                }
                else if (_level.DangerRating > 60)
                {
                    _level.DangerFeeling = 4;
                }
                else if (_level.DangerRating > 40)
                {
                    _level.DangerFeeling = 5;
                }
                else if (_level.DangerRating > 30)
                {
                    _level.DangerFeeling = 6;
                }
                else if (_level.DangerRating > 20)
                {
                    _level.DangerFeeling = 7;
                }
                else if (_level.DangerRating > 10)
                {
                    _level.DangerFeeling = 8;
                }
                else if (_level.DangerRating > 0)
                {
                    _level.DangerFeeling = 9;
                }
                else
                {
                    _level.DangerFeeling = 10;
                }
                if (_level.SpecialDanger)
                {
                    _level.DangerFeeling = 1;
                }
                if (SaveGame.Instance.CurrentDepth <= 0)
                {
                    _level.TreasureFeeling = 0;
                    _level.DangerFeeling = 0;
                }
                if (_level.OMax >= Constants.MaxOIdx)
                {
                    okay = false;
                }
                if (_level.MMax >= Constants.MaxMIdx)
                {
                    okay = false;
                }
                if (num < 100)
                {
                    var totalFeeling = _level.TreasureFeeling + _level.DangerFeeling;
                    if (totalFeeling > 18 ||
                        (SaveGame.Instance.Difficulty >= 5 && totalFeeling > 16) ||
                        (SaveGame.Instance.Difficulty >= 10 && totalFeeling > 14) ||
                        (SaveGame.Instance.Difficulty >= 20 && totalFeeling > 12) ||
                        (SaveGame.Instance.Difficulty >= 40 && totalFeeling > 10))
                    {
                        okay = false;
                    }
                }
                if (okay)
                {
                    break;
                }
                _level.WipeOList();
                _level.Monsters.WipeMList();
            }
            SaveGame.Instance.Player.GameTime.MarkLevelEntry();
        }

        private void AllocObject(int set, int typ, int num)
        {
            var y = 0;
            var x = 0;
            var dummy = 0;
            for (var k = 0; k < num; k++)
            {
                while (dummy < SafeMaxAttempts)
                {
                    dummy++;
                    y = Program.Rng.RandomLessThan(_level.CurHgt);
                    x = Program.Rng.RandomLessThan(_level.CurWid);
                    if (!_level.GridOpenNoItemOrCreature(y, x))
                    {
                        continue;
                    }
                    var isRoom = _level.Grid[y][x].TileFlags.IsSet(GridTile.InRoom);
                    if (set == _allocSetCorr && isRoom)
                    {
                        continue;
                    }
                    if (set == _allocSetRoom && !isRoom)
                    {
                        continue;
                    }
                    break;
                }
                if (dummy >= SafeMaxAttempts)
                {
                    return;
                }
                switch (typ)
                {
                    case _allocTypRubble:
                        {
                            PlaceRubble(y, x);
                            break;
                        }
                    case _allocTypTrap:
                        {
                            _level.PlaceTrap(y, x);
                            break;
                        }
                    case _allocTypGold:
                        {
                            _level.PlaceGold(y, x);
                            break;
                        }
                    case _allocTypObject:
                        {
                            _level.PlaceObject(y, x, false, false);
                            break;
                        }
                }
            }
        }

        private void AllocStairs(string feat, int num, int walls)
        {
            for (var i = 0; i < num; i++)
            {
                for (var flag = false; !flag;)
                {
                    for (var j = 0; !flag && j <= 3000; j++)
                    {
                        var y = Program.Rng.RandomLessThan(_level.CurHgt);
                        var x = Program.Rng.RandomLessThan(_level.CurWid);
                        if (!_level.GridOpenNoItemOrCreature(y, x))
                        {
                            continue;
                        }
                        if (NextToWalls(y, x) < walls)
                        {
                            continue;
                        }
                        var cPtr = _level.Grid[y][x];
                        if (SaveGame.Instance.CurrentDepth <= 0)
                        {
                            cPtr.SetFeature("DownStair");
                        }
                        else if (SaveGame.Instance.Quests.IsQuest(SaveGame.Instance.CurrentDepth) ||
                                 SaveGame.Instance.CurrentDepth == SaveGame.Instance.CurDungeon.MaxLevel)
                        {
                            cPtr.SetFeature(SaveGame.Instance.CurDungeon.Tower ? "DownStair" : "UpStair");
                        }
                        else
                        {
                            cPtr.SetFeature(feat);
                        }
                        flag = true;
                    }
                    if (walls != 0)
                    {
                        walls--;
                    }
                }
            }
        }

        private void BuildField(int yy, int xx)
        {
            var y0 = (yy * 9) + 8;
            var x0 = (xx * 15) + 10;
            var y1 = y0 - Program.Rng.DieRoll(2) - 1;
            var y2 = y0 + Program.Rng.DieRoll(2) + 1;
            var x1 = x0 - Program.Rng.DieRoll(3) - 2;
            var x2 = x0 + Program.Rng.DieRoll(3) + 2;
            const string feature = "Field";
            for (var x = x1; x < x2; x++)
            {
                for (var y = y1; y < y2; y++)
                {
                    _level.Grid[y][x].SetFeature(feature);
                    _level.Grid[y][x].SetBackgroundFeature(feature);
                }
            }
            if (Program.Rng.DieRoll(5) == 4)
            {
                var x = Program.Rng.RandomBetween(x1, x2);
                var y = Program.Rng.RandomBetween(y1, y2);
                _level.Grid[y][x].SetFeature("Scarecrow");
            }
        }

        private void BuildGraveyard(int yy, int xx)
        {
            var y0 = (yy * 9) + 8;
            var x0 = (xx * 15) + 10;
            var y1 = y0 - Program.Rng.DieRoll(2) - 1;
            var y2 = y0 + Program.Rng.DieRoll(2) + 1;
            var x1 = x0 - Program.Rng.DieRoll(3) - 2;
            var x2 = x0 + Program.Rng.DieRoll(3) + 2;
            for (var i = 0; i < Program.Rng.RandomBetween(10, 20); i++)
            {
                var x = (Program.Rng.RandomBetween(x1, x2) / 2 * 2) + 1;
                var y = (Program.Rng.RandomBetween(y1, y2) / 2 * 2) + 1;
                _level.Grid[y][x].SetFeature("Grave");
            }
        }

        private void BuildStore(Store store, int yy, int xx)
        {
            int y, x;
            GridTile cPtr;
            if (SaveGame.Instance.CurTown.Char != 'K')
            {
                if (store.StoreType == StoreType.StoreEmptyLot)
                {
                    switch (Program.Rng.DieRoll(10))
                    {
                        case 3:
                        case 7:
                        case 9:
                            break;

                        case 6:
                            BuildGraveyard(yy, xx);
                            break;

                        default:
                            BuildField(yy, xx);
                            break;
                    }
                    return;
                }
            }
            var y0 = (yy * 9) + 6;
            var x0 = (xx * 15) + 10;
            var y1 = y0 - Program.Rng.DieRoll(2);
            var y2 = y0 + Program.Rng.DieRoll(2) + 1;
            var x1 = x0 - Program.Rng.DieRoll(3) - 2;
            var x2 = x0 + Program.Rng.DieRoll(3) + 2;
            if ((y2 - y1) % 2 == 0)
            {
                y2++;
            }
            for (y = y1; y <= y2; y++)
            {
                for (x = x1; x <= x2; x++)
                {
                    cPtr = _level.Grid[y][x];
                    if (SaveGame.Instance.CurTown.Char == 'K')
                    {
                        switch (Program.Rng.DieRoll(6))
                        {
                            case 1:
                                cPtr.SetFeature("WallInner");
                                break;

                            case 2:
                            case 3:
                            case 4:
                                cPtr.SetFeature("Rubble");
                                break;
                        }
                    }
                    else
                    {
                        if (y == y2)
                        {
                            cPtr.SetFeature("WallPermBuilding");
                        }
                        else
                        {
                            cPtr.SetFeature("Roof");
                        }
                    }
                    cPtr.TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                }
            }
            y = y2;
            x = Program.Rng.RandomBetween(x1 + 1, x2 - 2);
            cPtr = _level.Grid[y][x];
            if (SaveGame.Instance.CurTown.Char == 'K')
            {
                if (Program.Rng.DieRoll(8) == 6)
                {
                    PlaceRandomDoor(y, x);
                }
            }
            else
            {
                cPtr.SetFeature(store.FeatureType);
            }
            store.X = x;
            store.Y = y;
            for (++y; y < y0 + 7; y++)
            {
                cPtr = _level.Grid[y][x];
                cPtr.SetFeature("PathBase");
            }
            y--;
            var dX = Math.Sign((_level.CurWid / 2) - x);
            for (x += dX; x != _level.CurWid / 2; x += dX)
            {
                cPtr = _level.Grid[y][x];
                cPtr.SetFeature("PathBase");
            }
        }

        private void BuildStreamer(string feat, int chance)
        {
            var dummy = 0;
            var y = Program.Rng.RandomSpread(_level.CurHgt / 2, 10);
            var x = Program.Rng.RandomSpread(_level.CurWid / 2, 15);
            var dir = _level.OrderedDirection[Program.Rng.RandomLessThan(8)];
            while (dummy < SafeMaxAttempts)
            {
                dummy++;
                for (var i = 0; i < _dunStrDen; i++)
                {
                    const int d = _dunStrRng;
                    int tx;
                    int ty;
                    while (true)
                    {
                        ty = Program.Rng.RandomSpread(y, d);
                        tx = Program.Rng.RandomSpread(x, d);
                        if (!_level.InBounds2(ty, tx))
                        {
                            continue;
                        }
                        break;
                    }
                    var cPtr = _level.Grid[ty][tx];

                    if (!cPtr.FeatureType.IsBasicWall)
                    {
                        continue;
                    }
                    cPtr.SetFeature(feat);
                    if (Program.Rng.RandomLessThan(chance) == 0)
                    {
                        cPtr.SetFeature(cPtr.FeatureType.Name + "VisTreas");
                    }
                }
                if (dummy >= SafeMaxAttempts)
                {
                    return;
                }
                y += _level.KeypadDirectionYOffset[dir];
                x += _level.KeypadDirectionXOffset[dir];
                if (!_level.InBounds(y, x))
                {
                    break;
                }
            }
        }

        private void BuildTunnel(int row1, int col1, int row2, int col2)
        {
            int i, y, x;
            var mainLoopCount = 0;
            var doorFlag = false;
            GridTile cPtr;
            _dun.TunnN = 0;
            _dun.WallN = 0;
            var startRow = row1;
            var startCol = col1;
            CorrectDir(out var rowDir, out var colDir, row1, col1, row2, col2);
            while (row1 != row2 || col1 != col2)
            {
                if (mainLoopCount++ > 2000)
                {
                    break;
                }
                if (Program.Rng.RandomLessThan(100) < _dunTunChg)
                {
                    CorrectDir(out rowDir, out colDir, row1, col1, row2, col2);
                    if (Program.Rng.RandomLessThan(100) < _dunTunRnd)
                    {
                        RandDir(out rowDir, out colDir);
                    }
                }
                var tmpRow = row1 + rowDir;
                var tmpCol = col1 + colDir;
                while (!_level.InBounds(tmpRow, tmpCol))
                {
                    CorrectDir(out rowDir, out colDir, row1, col1, row2, col2);
                    if (Program.Rng.RandomLessThan(100) < _dunTunRnd)
                    {
                        RandDir(out rowDir, out colDir);
                    }
                    tmpRow = row1 + rowDir;
                    tmpCol = col1 + colDir;
                }
                cPtr = _level.Grid[tmpRow][tmpCol];
                if (cPtr.FeatureType.Name == "WallPermSolid")
                {
                    continue;
                }
                if (cPtr.FeatureType.Name == "WallPermOuter")
                {
                    continue;
                }
                if (cPtr.FeatureType.Name == "WallSolid")
                {
                    continue;
                }
                if (cPtr.FeatureType.Name == "WallOuter")
                {
                    y = tmpRow + rowDir;
                    x = tmpCol + colDir;
                    if (_level.Grid[y][x].FeatureType.Name == "WallPermSolid")
                    {
                        continue;
                    }
                    if (_level.Grid[y][x].FeatureType.Name == "WallPermOuter")
                    {
                        continue;
                    }
                    if (_level.Grid[y][x].FeatureType.Name == "WallOuter")
                    {
                        continue;
                    }
                    if (_level.Grid[y][x].FeatureType.Name == "WallSolid")
                    {
                        continue;
                    }
                    row1 = tmpRow;
                    col1 = tmpCol;
                    if (_dun.WallN < WallMax)
                    {
                        _dun.Wall[_dun.WallN].Y = row1;
                        _dun.Wall[_dun.WallN].X = col1;
                        _dun.WallN++;
                    }
                    for (y = row1 - 1; y <= row1 + 1; y++)
                    {
                        for (x = col1 - 1; x <= col1 + 1; x++)
                        {
                            if (_level.Grid[y][x].FeatureType.Name == "WallOuter")
                            {
                                _level.Grid[y][x].SetFeature("WallSolid");
                            }
                        }
                    }
                }
                else if (cPtr.TileFlags.IsSet(GridTile.InRoom))
                {
                    row1 = tmpRow;
                    col1 = tmpCol;
                }
                else if (cPtr.FeatureType.IsWall)
                {
                    row1 = tmpRow;
                    col1 = tmpCol;
                    if (_dun.TunnN < TunnMax)
                    {
                        _dun.Tunn[_dun.TunnN].Y = row1;
                        _dun.Tunn[_dun.TunnN].X = col1;
                        _dun.TunnN++;
                    }
                    doorFlag = false;
                }
                else
                {
                    row1 = tmpRow;
                    col1 = tmpCol;
                    if (!doorFlag)
                    {
                        if (_dun.DoorN < DoorMax)
                        {
                            _dun.Door[_dun.DoorN].Y = row1;
                            _dun.Door[_dun.DoorN].X = col1;
                            _dun.DoorN++;
                        }
                        doorFlag = true;
                    }
                    if (Program.Rng.RandomLessThan(100) >= _dunTunCon)
                    {
                        tmpRow = row1 - startRow;
                        if (tmpRow < 0)
                        {
                            tmpRow = -tmpRow;
                        }
                        tmpCol = col1 - startCol;
                        if (tmpCol < 0)
                        {
                            tmpCol = -tmpCol;
                        }
                        if (tmpRow > 10 || tmpCol > 10)
                        {
                            break;
                        }
                    }
                }
            }
            for (i = 0; i < _dun.TunnN; i++)
            {
                y = _dun.Tunn[i].Y;
                x = _dun.Tunn[i].X;
                cPtr = _level.Grid[y][x];
                cPtr.RevertToBackground();
            }
            for (i = 0; i < _dun.WallN; i++)
            {
                y = _dun.Wall[i].Y;
                x = _dun.Wall[i].X;
                cPtr = _level.Grid[y][x];
                cPtr.RevertToBackground();
                if (Program.Rng.RandomLessThan(100) < _dunTunPen)
                {
                    PlaceRandomDoor(y, x);
                }
            }
        }

        private void CorrectDir(out int rdir, out int cdir, int y1, int x1, int y2, int x2)
        {
            rdir = y1 == y2 ? 0 : y1 < y2 ? 1 : -1;
            cdir = x1 == x2 ? 0 : x1 < x2 ? 1 : -1;
            if (rdir != 0 && cdir != 0)
            {
                if (Program.Rng.RandomLessThan(100) < 50)
                {
                    rdir = 0;
                }
                else
                {
                    cdir = 0;
                }
            }
        }

        private void DestroyLevel()
        {
            for (var n = 0; n < Program.Rng.DieRoll(5); n++)
            {
                var x1 = Program.Rng.RandomBetween(5, _level.CurWid - 1 - 5);
                var y1 = Program.Rng.RandomBetween(5, _level.CurHgt - 1 - 5);
                int y;
                for (y = y1 - 15; y <= y1 + 15; y++)
                {
                    int x;
                    for (x = x1 - 15; x <= x1 + 15; x++)
                    {
                        if (!_level.InBounds(y, x))
                        {
                            continue;
                        }
                        var k = _level.Distance(y1, x1, y, x);
                        if (k >= 16)
                        {
                            continue;
                        }
                        _level.DeleteMonster(y, x);
                        if (_level.CaveValidBold(y, x))
                        {
                            _level.DeleteObject(y, x);
                            var cPtr = _level.Grid[y][x];
                            var t = Program.Rng.RandomLessThan(200);
                            if (t < 20)
                            {
                                cPtr.SetFeature("WallBasic");
                            }
                            else if (t < 70)
                            {
                                cPtr.SetFeature("Quartz");
                            }
                            else if (t < 100)
                            {
                                cPtr.SetFeature("Magma");
                            }
                            else
                            {
                                cPtr.RevertToBackground();
                            }
                            cPtr.TileFlags.Clear(GridTile.InRoom | GridTile.InVault);
                            cPtr.TileFlags.Clear(GridTile.PlayerMemorised | GridTile.SelfLit);
                        }
                    }
                }
            }
        }

        private void MakeCavernLevel()
        {
            var perlinNoise = new PerlinNoise(Program.Rng.RandomBetween(0, int.MaxValue - 1));
            var widthDivisor = 1 / (double)_level.CurWid;
            var heightDivisor = 1 / (double)_level.CurHgt;
            for (var y = 0; y < _level.CurHgt; y++)
            {
                for (var x = 0; x < _level.CurWid; x++)
                {
                    var cPtr = _level.Grid[y][x];
                    var v = perlinNoise.Noise(10 * x * widthDivisor, 10 * y * heightDivisor, -0.5);
                    v = (v + 1) / 2;
                    var dX = Math.Abs(x - (_level.CurWid / 2)) * widthDivisor;
                    var dY = Math.Abs(y - (_level.CurHgt / 2)) * heightDivisor;
                    var d = Math.Max(dX, dY);
                    const double elevation = 0.05;
                    const double steepness = 6.0;
                    const double dropoff = 50.0;
                    v += elevation - (dropoff * Math.Pow(d, steepness));
                    v = Math.Min(1, Math.Max(0, v));
                    var rounded = (int)(v * 10);
                    if (rounded < 2 || rounded > 5)
                    {
                        cPtr.SetFeature("WallBasic");
                    }
                    else
                    {
                        cPtr.SetFeature("DungeonFloor");
                    }
                }
            }
            for (var i = 0; i < _dunStrMag; i++)
            {
                BuildStreamer("Magma", _dunStrMc);
            }
            for (var i = 0; i < _dunStrQua; i++)
            {
                BuildStreamer("Quartz", _dunStrQc);
            }
            for (var x = 0; x < _level.CurWid; x++)
            {
                var cPtr = _level.Grid[0][x];
                cPtr.SetFeature("WallPermSolid");
            }
            for (var x = 0; x < _level.CurWid; x++)
            {
                var cPtr = _level.Grid[_level.CurHgt - 1][x];
                cPtr.SetFeature("WallPermSolid");
            }
            for (var y = 0; y < _level.CurHgt; y++)
            {
                var cPtr = _level.Grid[y][0];
                cPtr.SetFeature("WallPermSolid");
            }
            for (var y = 0; y < _level.CurHgt; y++)
            {
                var cPtr = _level.Grid[y][_level.CurWid - 1];
                cPtr.SetFeature("WallPermSolid");
            }
            if (Program.Rng.DieRoll(_darkEmpty) != 1 || Program.Rng.DieRoll(100) > SaveGame.Instance.Difficulty)
            {
                _level.WizLight();
            }
        }

        private void MakeCornerTowers(int wildX, int wildY)
        {
            var wilderness = SaveGame.Instance.Wilderness;
            var height = _level.CurHgt;
            var width = _level.CurWid;
            if ((wilderness[wildY][wildX].Town != null) || (wilderness[wildY - 1][wildX].Town != null) ||
                (wilderness[wildY][wildX - 1].Town != null) || (wilderness[wildY - 1][wildX - 1].Town != null))
            {
                _level.Grid[0][0].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[0][0].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[1][0].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[1][0].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[1][1].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[1][1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[0][1].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[0][1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[0][0].RevertToBackground();
                _level.Grid[0][0].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[1][0].SetFeature("TownWall");
                _level.Grid[1][0].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[1][1].SetFeature("TownWall");
                _level.Grid[1][1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[0][1].SetFeature("TownWall");
                _level.Grid[0][1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            }
            if ((wilderness[wildY][wildX].Town != null) || (wilderness[wildY - 1][wildX].Town != null) ||
                (wilderness[wildY][wildX + 1].Town != null) || (wilderness[wildY - 1][wildX + 1].Town != null))
            {
                _level.Grid[0][width - 1].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[0][width - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[1][width - 1].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[1][width - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[1][width - 2].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[1][width - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[0][width - 2].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[0][width - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[0][width - 1].RevertToBackground();
                _level.Grid[0][width - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[1][width - 1].SetFeature("TownWall");
                _level.Grid[1][width - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[1][width - 2].SetFeature("TownWall");
                _level.Grid[1][width - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[0][width - 2].SetFeature("TownWall");
                _level.Grid[0][width - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            }
            if ((wilderness[wildY][wildX].Town != null) || (wilderness[wildY + 1][wildX].Town != null) ||
                (wilderness[wildY][wildX + 1].Town != null) || (wilderness[wildY + 1][wildX + 1].Town != null))
            {
                _level.Grid[height - 1][width - 1].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[height - 1][width - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 2][width - 1].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[height - 2][width - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 2][width - 2].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[height - 2][width - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 1][width - 2].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[height - 1][width - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 1][width - 1].RevertToBackground();
                _level.Grid[height - 1][width - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 2][width - 1].SetFeature("TownWall");
                _level.Grid[height - 2][width - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 2][width - 2].SetFeature("TownWall");
                _level.Grid[height - 2][width - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 1][width - 2].SetFeature("TownWall");
                _level.Grid[height - 1][width - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            }
            if ((wilderness[wildY][wildX].Town != null) || (wilderness[wildY + 1][wildX].Town != null) ||
                (wilderness[wildY][wildX - 1].Town != null) || (wilderness[wildY + 1][wildX - 1].Town != null))
            {
                _level.Grid[height - 1][0].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[height - 1][0].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 2][0].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[height - 2][0].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 2][1].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[height - 2][1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 1][1].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[height - 1][1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 1][0].RevertToBackground();
                _level.Grid[height - 1][0].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 2][0].SetFeature("TownWall");
                _level.Grid[height - 2][0].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 2][1].SetFeature("TownWall");
                _level.Grid[height - 2][1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 1][1].SetFeature("TownWall");
                _level.Grid[height - 1][1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            }
        }

        private void MakeDungeonEntrance(int left, int top, int width, int height, out int stairX, out int stairY)
        {
            var dummy = 0;
            var x = 1;
            var y = 1;
            while (dummy < SafeMaxAttempts)
            {
                dummy++;
                y = Program.Rng.RandomBetween(top, top + height);
                x = Program.Rng.RandomBetween(left, left + width);
                if (_level.GridOpenNoItemOrCreature(y, x))
                {
                    break;
                }
            }
            _level.Grid[y - 2][x].RevertToBackground();
            _level.Grid[y - 1][x - 1].RevertToBackground();
            _level.Grid[y - 1][x].RevertToBackground();
            _level.Grid[y - 1][x + 1].RevertToBackground();
            _level.Grid[y][x - 2].RevertToBackground();
            _level.Grid[y][x - 1].RevertToBackground();
            _level.Grid[y][x].SetFeature("DownStair");
            stairX = x;
            stairY = y;
            _level.Grid[y][x + 1].RevertToBackground();
            _level.Grid[y][x + 2].RevertToBackground();
            _level.Grid[y + 1][x - 1].RevertToBackground();
            _level.Grid[y + 1][x].RevertToBackground();
            _level.Grid[y + 1][x + 1].RevertToBackground();
            _level.Grid[y + 2][x].RevertToBackground();
        }

        private void MakeDungeonLevel()
        {
            int i;
            int k;
            int y;
            int x;
            var maxVaultOk = 2;
            var destroyed = false;
            var emptyLevel = false;
            _dun = new LevelBuildInfo();
            if (_level.MaxPanelRows == 0)
            {
                maxVaultOk--;
            }
            if (_level.MaxPanelCols == 0)
            {
                maxVaultOk--;
            }
            if (Program.Rng.DieRoll(_emptyLevel) == 1)
            {
                emptyLevel = true;
            }
            for (y = 0; y < _level.CurHgt; y++)
            {
                for (x = 0; x < _level.CurWid; x++)
                {
                    var cPtr = _level.Grid[y][x];
                    if (emptyLevel)
                    {
                        cPtr.RevertToBackground();
                    }
                    else
                    {
                        cPtr.SetFeature("WallBasic");
                    }
                }
            }
            if (SaveGame.Instance.Difficulty > 10 && Program.Rng.RandomLessThan(_dunDest) == 0)
            {
                destroyed = true;
            }
            if (SaveGame.Instance.Quests.IsQuest(SaveGame.Instance.CurrentDepth))
            {
                destroyed = false;
            }
            _dun.RowRooms = _level.CurHgt / _blockHgt;
            _dun.ColRooms = _level.CurWid / _blockWid;
            for (y = 0; y < _dun.RowRooms; y++)
            {
                for (x = 0; x < _dun.ColRooms; x++)
                {
                    _dun.RoomMap[y][x] = false;
                }
            }
            _dun.Crowded = false;
            _dun.CentN = 0;
            for (i = 0; i < _dunRooms; i++)
            {
                y = Program.Rng.RandomLessThan(_dun.RowRooms);
                x = Program.Rng.RandomLessThan(_dun.ColRooms);
                if (x % 3 == 0)
                {
                    x++;
                }
                if (x % 3 == 2)
                {
                    x--;
                }
                if (destroyed)
                {
                    if (RoomBuild(y, x, 1))
                    {
                    }
                    continue;
                }
                if (Program.Rng.RandomLessThan(_dunUnusual) < SaveGame.Instance.Difficulty)
                {
                    k = Program.Rng.RandomLessThan(100);
                    if (Program.Rng.RandomLessThan(_dunUnusual) < SaveGame.Instance.Difficulty)
                    {
                        if (k < 10)
                        {
                            if (maxVaultOk > 1)
                            {
                                if (RoomBuild(y, x, 8))
                                {
                                    continue;
                                }
                            }
                        }
                        if (k < 25)
                        {
                            if (maxVaultOk > 0)
                            {
                                if (RoomBuild(y, x, 7))
                                {
                                    continue;
                                }
                            }
                        }
                        if (k < 40 && RoomBuild(y, x, 5))
                        {
                            continue;
                        }
                        if (k < 55 && RoomBuild(y, x, 6))
                        {
                            continue;
                        }
                    }
                    if (k < 25 && RoomBuild(y, x, 4))
                    {
                        continue;
                    }
                    if (k < 50 && RoomBuild(y, x, 3))
                    {
                        continue;
                    }
                    if (k < 100 && RoomBuild(y, x, 2))
                    {
                        continue;
                    }
                }
                if (RoomBuild(y, x, 1))
                {
                }
            }
            for (x = 0; x < _level.CurWid; x++)
            {
                var cPtr = _level.Grid[0][x];
                cPtr.SetFeature("WallPermSolid");
            }
            for (x = 0; x < _level.CurWid; x++)
            {
                var cPtr = _level.Grid[_level.CurHgt - 1][x];
                cPtr.SetFeature("WallPermSolid");
            }
            for (y = 0; y < _level.CurHgt; y++)
            {
                var cPtr = _level.Grid[y][0];
                cPtr.SetFeature("WallPermSolid");
            }
            for (y = 0; y < _level.CurHgt; y++)
            {
                var cPtr = _level.Grid[y][_level.CurWid - 1];
                cPtr.SetFeature("WallPermSolid");
            }
            for (i = 0; i < _dun.CentN; i++)
            {
                var pick1 = Program.Rng.RandomLessThan(_dun.CentN);
                var pick2 = Program.Rng.RandomLessThan(_dun.CentN);
                var y1 = _dun.Cent[pick1].Y;
                var x1 = _dun.Cent[pick1].X;
                _dun.Cent[pick1].Y = _dun.Cent[pick2].Y;
                _dun.Cent[pick1].X = _dun.Cent[pick2].X;
                _dun.Cent[pick2].Y = y1;
                _dun.Cent[pick2].X = x1;
            }
            _dun.DoorN = 0;
            y = _dun.Cent[_dun.CentN - 1].Y;
            x = _dun.Cent[_dun.CentN - 1].X;
            for (i = 0; i < _dun.CentN; i++)
            {
                BuildTunnel(_dun.Cent[i].Y, _dun.Cent[i].X, y, x);
                y = _dun.Cent[i].Y;
                x = _dun.Cent[i].X;
            }
            for (i = 0; i < _dun.DoorN; i++)
            {
                y = _dun.Door[i].Y;
                x = _dun.Door[i].X;
                TryDoor(y, x - 1);
                TryDoor(y, x + 1);
                TryDoor(y - 1, x);
                TryDoor(y + 1, x);
            }
            for (i = 0; i < _dunStrMag; i++)
            {
                BuildStreamer("Magma", _dunStrMc);
            }
            for (i = 0; i < _dunStrQua; i++)
            {
                BuildStreamer("Quartz", _dunStrQc);
            }
            if (destroyed)
            {
                DestroyLevel();
            }
            if (emptyLevel && (Program.Rng.DieRoll(_darkEmpty) != 1 ||
                               Program.Rng.DieRoll(100) > SaveGame.Instance.Difficulty))
            {
                _level.WizLight();
            }
        }

        private void MakeHenge(int left, int top, int width, int height)
        {
            var midX = left + (width / 2);
            var midY = top + (height / 2);
            for (var y = midY - 3; y < midY + 3; y++)
            {
                _level.Grid[y][midX - 7].SetBackgroundFeature("Grass");
                _level.Grid[y][midX - 7].SetFeature("Grass");
            }
            for (var y = midY - 5; y < midY + 5; y++)
            {
                _level.Grid[y][midX - 6].SetBackgroundFeature("Grass");
                _level.Grid[y][midX - 6].SetFeature("Grass");
            }
            for (var y = midY - 6; y < midY + 6; y++)
            {
                _level.Grid[y][midX - 5].SetBackgroundFeature("Grass");
                _level.Grid[y][midX - 5].SetFeature("Grass");
            }
            for (var y = midY - 6; y < midY + 6; y++)
            {
                _level.Grid[y][midX - 4].SetBackgroundFeature("Grass");
                _level.Grid[y][midX - 4].SetFeature("Grass");
            }
            for (var y = midY - 7; y < midY + 6; y++)
            {
                _level.Grid[y][midX - 3].SetBackgroundFeature("Grass");
                _level.Grid[y][midX - 3].SetFeature("Grass");
            }
            for (var y = midY - 7; y < midY + 6; y++)
            {
                _level.Grid[y][midX - 2].SetBackgroundFeature("Grass");
                _level.Grid[y][midX - 2].SetFeature("Grass");
            }
            for (var y = midY - 6; y < midY + 6; y++)
            {
                _level.Grid[y][midX - 1].SetBackgroundFeature("Grass");
                _level.Grid[y][midX - 1].SetFeature("Grass");
            }
            for (var y = midY - 7; y < midY + 6; y++)
            {
                _level.Grid[y][midX].SetBackgroundFeature("Grass");
                _level.Grid[y][midX].SetFeature("Grass");
            }
            for (var y = midY - 7; y < midY + 6; y++)
            {
                _level.Grid[y][midX + 1].SetBackgroundFeature("Grass");
                _level.Grid[y][midX + 1].SetFeature("Grass");
            }
            for (var y = midY - 6; y < midY + 6; y++)
            {
                _level.Grid[y][midX + 2].SetBackgroundFeature("Grass");
                _level.Grid[y][midX + 2].SetFeature("Grass");
            }
            for (var y = midY - 7; y < midY + 6; y++)
            {
                _level.Grid[y][midX + 3].SetBackgroundFeature("Grass");
                _level.Grid[y][midX + 3].SetFeature("Grass");
            }
            for (var y = midY - 6; y < midY + 6; y++)
            {
                _level.Grid[y][midX + 4].SetBackgroundFeature("Grass");
                _level.Grid[y][midX + 4].SetFeature("Grass");
            }
            for (var y = midY - 5; y < midY + 5; y++)
            {
                _level.Grid[y][midX + 5].SetBackgroundFeature("Grass");
                _level.Grid[y][midX + 5].SetFeature("Grass");
            }
            for (var y = midY - 3; y < midY + 3; y++)
            {
                _level.Grid[y][midX + 6].SetBackgroundFeature("Grass");
                _level.Grid[y][midX + 6].SetFeature("Grass");
            }
            _level.Grid[midY - 6][midX].SetFeature("Rock");
            _level.Grid[midY - 6][midX - 1].SetFeature("Rock");
            _level.Grid[midY - 5][midX - 4].SetFeature("Rock");
            _level.Grid[midY - 4][midX - 5].SetFeature("Rock");
            _level.Grid[midY - 1][midX - 6].SetFeature("Rock");
            _level.Grid[midY][midX - 6].SetFeature("Rock");
            _level.Grid[midY + 3][midX - 5].SetFeature("Rock");
            _level.Grid[midY + 4][midX - 4].SetFeature("Rock");
            _level.Grid[midY + 5][midX - 1].SetFeature("Rock");
            _level.Grid[midY + 5][midX].SetFeature("Rock");
            _level.Grid[midY + 4][midX + 3].SetFeature("Rock");
            _level.Grid[midY + 3][midX + 4].SetFeature("Rock");
            _level.Grid[midY][midX + 5].SetFeature("Rock");
            _level.Grid[midY - 1][midX + 5].SetFeature("Rock");
            _level.Grid[midY - 4][midX + 4].SetFeature("Rock");
            _level.Grid[midY - 5][midX + 3].SetFeature("Rock");
        }

        private void MakeLake(int minX, int minY, int width, int height)
        {
            var perlinNoise = new PerlinNoise(Program.Rng.RandomBetween(0, int.MaxValue - 1));
            var widthDivisor = 1 / (double)width;
            var heightDivisor = 1 / (double)height;
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var cPtr = _level.Grid[minY + y][minX + x];
                    var v = perlinNoise.Noise(10 * x * widthDivisor, 10 * y * heightDivisor, -0.5);
                    v = (v + 1) / 2;
                    var dX = Math.Abs(x - (width / 2)) * widthDivisor;
                    var dY = Math.Abs(y - (height / 2)) * heightDivisor;
                    var d = Math.Max(dX, dY);
                    const double elevation = 0.05;
                    const double steepness = 6.0;
                    const double dropoff = 50.0;
                    v += elevation - (dropoff * Math.Pow(d, steepness));
                    v = Math.Min(1, Math.Max(0, v));
                    var rounded = (int)(v * 10);
                    if (rounded > 3)
                    {
                        cPtr.SetBackgroundFeature("Water");
                        cPtr.SetFeature("Water");
                    }
                    else if (rounded == 3)
                    {
                        cPtr.SetBackgroundFeature("Grass");
                        cPtr.SetFeature("Grass");
                    }
                }
            }
        }

        private void MakeTower(int left, int top, int width, int height, out int stairX, out int stairY)
        {
            int i;
            var y = top + height;
            var x = left + (width / 2);
            stairX = x;
            stairY = y;
            for (i = -2; i < 3; i++)
            {
                _level.Grid[y][x + i].SetFeature("WallPermBuilding");
                _level.Grid[y][x + i].TileFlags.Set(GridTile.PlayerMemorised | GridTile.SelfLit);
            }
            for (i = -4; i < 5; i++)
            {
                _level.Grid[y - 1][x + i].SetFeature("WallPermBuilding");
                _level.Grid[y - 1][x + i].TileFlags.Set(GridTile.PlayerMemorised | GridTile.SelfLit);
            }
            for (i = -5; i < 6; i++)
            {
                _level.Grid[y - 2][x + i].SetFeature("WallPermBuilding");
                _level.Grid[y - 2][x + i].TileFlags.Set(GridTile.PlayerMemorised | GridTile.SelfLit);
            }
            for (i = -6; i < 7; i++)
            {
                _level.Grid[y - 3][x + i].SetFeature("WallPermBuilding");
                _level.Grid[y - 3][x + i].TileFlags.Set(GridTile.PlayerMemorised | GridTile.SelfLit);
            }
            for (i = -6; i < 7; i++)
            {
                _level.Grid[y - 4][x + i].SetFeature("WallPermBuilding");
                _level.Grid[y - 4][x + i].TileFlags.Set(GridTile.PlayerMemorised | GridTile.SelfLit);
            }
            for (i = -7; i < 8; i++)
            {
                _level.Grid[y - 5][x + i].SetFeature("WallPermBuilding");
                _level.Grid[y - 5][x + i].TileFlags.Set(GridTile.PlayerMemorised | GridTile.SelfLit);
            }
            for (i = -7; i < 8; i++)
            {
                _level.Grid[y - 6][x + i].SetFeature("WallPermBuilding");
                _level.Grid[y - 6][x + i].TileFlags.Set(GridTile.PlayerMemorised | GridTile.SelfLit);
            }
            for (i = -7; i < 8; i++)
            {
                _level.Grid[y - 7][x + i].SetFeature("WallPermBuilding");
                _level.Grid[y - 7][x + i].TileFlags.Set(GridTile.PlayerMemorised | GridTile.SelfLit);
            }
            for (i = -7; i < 8; i++)
            {
                _level.Grid[y - 8][x + i].SetFeature("WallPermBuilding");
                _level.Grid[y - 8][x + i].TileFlags.Set(GridTile.PlayerMemorised | GridTile.SelfLit);
            }
            for (i = -7; i < 8; i++)
            {
                _level.Grid[y - 9][x + i].SetFeature("WallPermBuilding");
                _level.Grid[y - 9][x + i].TileFlags.Set(GridTile.PlayerMemorised | GridTile.SelfLit);
            }
            for (i = -6; i < 7; i++)
            {
                _level.Grid[y - 10][x + i].SetFeature("WallPermBuilding");
                _level.Grid[y - 10][x + i].TileFlags.Set(GridTile.PlayerMemorised | GridTile.SelfLit);
            }
            for (i = -6; i < 7; i++)
            {
                _level.Grid[y - 11][x + i].SetFeature("WallPermBuilding");
                _level.Grid[y - 11][x + i].TileFlags.Set(GridTile.PlayerMemorised | GridTile.SelfLit);
            }
            for (i = -5; i < 6; i++)
            {
                _level.Grid[y - 12][x + i].SetFeature("WallPermBuilding");
                _level.Grid[y - 12][x + i].TileFlags.Set(GridTile.PlayerMemorised | GridTile.SelfLit);
            }
            for (i = -4; i < 5; i++)
            {
                _level.Grid[y - 13][x + i].SetFeature("WallPermBuilding");
                _level.Grid[y - 13][x + i].TileFlags.Set(GridTile.PlayerMemorised | GridTile.SelfLit);
            }
            for (i = -2; i < 4; i++)
            {
                _level.Grid[y - 14][x + i].SetFeature("WallPermBuilding");
                _level.Grid[y - 14][x + i].TileFlags.Set(GridTile.PlayerMemorised | GridTile.SelfLit);
            }
            _level.Grid[y][x].SetFeature("UpStair");
            _level.Grid[y][x].TileFlags.Set(GridTile.PlayerMemorised | GridTile.SelfLit);
            for (i = -3; i < 4; i++)
            {
                if (_level.Grid[y + 1][x + i].FeatureType.Category == FloorTileTypeCategory.Tree)
                {
                    _level.Grid[y + 1][x + i].RevertToBackground();
                }
            }
            for (i = -2; i < 3; i++)
            {
                if (_level.Grid[y + 2][x + i].FeatureType.Category == FloorTileTypeCategory.Tree)
                {
                    _level.Grid[y + 2][x + i].RevertToBackground();
                }
            }
        }

        private void MakeTownCentre()
        {
            var xx = _level.CurWid / 2;
            var yy = _level.CurHgt / 2;
            switch (Program.Rng.DieRoll(12))
            {
                case 1:
                case 3:
                    _level.Grid[yy - 1][xx - 1].SetFeature("PathBase");
                    _level.Grid[yy][xx - 1].SetFeature("PathBase");
                    _level.Grid[yy + 1][xx - 1].SetFeature("PathBase");
                    _level.Grid[yy - 1][xx].SetFeature("PathBase");
                    _level.Grid[yy + 1][xx].SetFeature("PathBase");
                    _level.Grid[yy - 1][xx + 1].SetFeature("PathBase");
                    _level.Grid[yy][xx + 1].SetFeature("PathBase");
                    _level.Grid[yy + 1][xx + 1].SetFeature("PathBase");
                    switch (Program.Rng.DieRoll(6))
                    {
                        case 4:
                        case 1:
                            _level.Grid[yy][xx].RevertToBackground();
                            break;

                        case 2:
                            _level.Grid[yy][xx].SetFeature("Statue");
                            break;

                        default:
                            _level.Grid[yy][xx].SetFeature("Fountain");
                            break;
                    }
                    return;

                case 2:
                case 8:
                case 9:
                case 12:
                    var x = xx - 1;
                    if (Program.Rng.DieRoll(2) == 1)
                    {
                        x = xx + 1;
                    }
                    var y = yy - 1;
                    if (Program.Rng.DieRoll(2) == 1)
                    {
                        y = yy + 1;
                    }
                    _level.Grid[y][x].SetFeature("Signpost");
                    break;

                default:
                    return;
            }
        }

        private void MakeTownContents()
        {
            int x, n;
            var dummy = 0;
            GridTile cPtr;
            var rooms = new int[12];
            Program.Rng.UseFixed = true;
            Program.Rng.FixedSeed = SaveGame.Instance.CurTown.Seed;
            for (n = 0; n < 12; n++)
            {
                rooms[n] = n;
            }
            var y = _level.CurHgt / 2;
            for (x = 1; x < _level.CurWid - 1; x++)
            {
                _level.Grid[y][x].SetFeature("PathBase");
            }
            x = _level.CurWid / 2;
            for (y = 1; y < _level.CurHgt - 1; y++)
            {
                _level.Grid[y][x].SetFeature("PathBase");
            }
            for (y = 0; y < 4; y++)
            {
                for (x = 0; x < 4; x++)
                {
                    if (x == 1 || x == 2 || y == 1 || y == 2)
                    {
                        var k = Program.Rng.RandomLessThan(n);
                        BuildStore(SaveGame.Instance.CurTown.Stores[rooms[k]], y, x);
                        rooms[k] = rooms[--n];
                    }
                    else
                    {
                        switch (Program.Rng.DieRoll(10))
                        {
                            case 3:
                            case 7:
                            case 9:
                                break;

                            default:
                                if (SaveGame.Instance.CurTown.Char == 'K')
                                {
                                    BuildGraveyard(y, x);
                                }
                                else
                                {
                                    BuildField(y, x);
                                }
                                break;
                        }
                    }
                }
            }
            for (n = 0; n < Program.Rng.RandomBetween(1, 10) - 6; n++)
            {
                x = Program.Rng.RandomBetween(1, _level.CurWid - 2);
                y = Program.Rng.RandomBetween(1, _level.CurHgt - 2);
                cPtr = _level.Grid[y][x];
                if (cPtr.FeatureType.Name == cPtr.BackgroundFeature.Name)
                {
                    cPtr.SetFeature("Rock");
                    cPtr.TileFlags.Set(GridTile.PlayerMemorised);
                }
            }
            for (n = 0; n < Program.Rng.RandomBetween(5, 10); n++)
            {
                x = Program.Rng.RandomBetween(1, _level.CurWid - 2);
                y = Program.Rng.RandomBetween(1, _level.CurHgt - 2);
                cPtr = _level.Grid[y][x];
                if (cPtr.FeatureType.Name == cPtr.BackgroundFeature.Name)
                {
                    cPtr.SetFeature("Tree");
                    cPtr.TileFlags.Set(GridTile.PlayerMemorised);
                }
            }
            for (n = 0; n < Program.Rng.RandomBetween(5, 10); n++)
            {
                x = Program.Rng.RandomBetween(1, _level.CurWid - 2);
                y = Program.Rng.RandomBetween(1, _level.CurHgt - 2);
                cPtr = _level.Grid[y][x];
                if (cPtr.FeatureType.Name == cPtr.BackgroundFeature.Name)
                {
                    cPtr.SetFeature("Bush");
                    cPtr.TileFlags.Set(GridTile.PlayerMemorised);
                }
            }
            MakeTownCentre();
            x = _level.CurWid / 2;
            cPtr = _level.Grid[0][x];
            cPtr.SetFeature("PathBorderNS");
            cPtr.TileFlags.Set(GridTile.PlayerMemorised);
            x = _level.CurWid - 2;
            y = _level.CurHgt / 2;
            cPtr = _level.Grid[y][x + 1];
            cPtr.SetFeature("PathBorderEW");
            cPtr.TileFlags.Set(GridTile.PlayerMemorised);
            x = _level.CurWid / 2;
            y = _level.CurHgt - 2;
            cPtr = _level.Grid[y + 1][x];
            cPtr.SetFeature("PathBorderNS");
            cPtr.TileFlags.Set(GridTile.PlayerMemorised);
            x = 1;
            y = _level.CurHgt / 2;
            cPtr = _level.Grid[y][0];
            cPtr.SetFeature("PathBorderEW");
            cPtr.TileFlags.Set(GridTile.PlayerMemorised);
            while (dummy < SafeMaxAttempts)
            {
                dummy++;
                y = Program.Rng.RandomBetween(12, 29);
                x = Program.Rng.RandomBetween(17, 46);
                if (_level.GridOpenNoItemOrCreature(y, x))
                {
                    break;
                }
            }
            cPtr = _level.Grid[y][x];
            cPtr.SetFeature("DownStair");
            cPtr.TileFlags.Set(GridTile.PlayerMemorised);
            Program.Rng.UseFixed = false;
            switch (SaveGame.Instance.CameFrom)
            {
                case LevelStart.StartRandom:
                    NewPlayerSpot();
                    break;

                case LevelStart.StartStairs:
                    SaveGame.Instance.Player.MapY = y;
                    SaveGame.Instance.Player.MapX = x;
                    break;

                case LevelStart.StartWalk:
                    break;

                case LevelStart.StartHouse:
                    foreach (var store in SaveGame.Instance.CurTown.Stores)
                    {
                        if (store.StoreType != StoreType.StoreHome)
                        {
                            continue;
                        }
                        SaveGame.Instance.Player.MapY = store.Y;
                        SaveGame.Instance.Player.MapX = store.X;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void MakeTownWalls()
        {
            int x;
            int y;
            GridTile cPtr;
            for (x = 0; x < _level.CurWid; x++)
            {
                cPtr = _level.Grid[0][x];
                cPtr.SetFeature("TownWall");
                cPtr.TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                cPtr = _level.Grid[_level.CurHgt - 1][x];
                cPtr.SetFeature("TownWall");
                cPtr.TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            }
            for (y = 0; y < _level.CurHgt; y++)
            {
                cPtr = _level.Grid[y][0];
                cPtr.SetFeature("TownWall");
                cPtr.TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                cPtr = _level.Grid[y][_level.CurWid - 1];
                cPtr.SetFeature("TownWall");
                cPtr.TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            }
            _level.Grid[0][(_level.CurWid / 2) - 2].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[0][(_level.CurWid / 2) - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[0][(_level.CurWid / 2) - 1].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[0][(_level.CurWid / 2) - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[0][(_level.CurWid / 2) + 1].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[0][(_level.CurWid / 2) + 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[0][(_level.CurWid / 2) + 2].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[0][(_level.CurWid / 2) + 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[1][(_level.CurWid / 2) - 2].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[1][(_level.CurWid / 2) - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[1][(_level.CurWid / 2) - 1].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[1][(_level.CurWid / 2) - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[1][(_level.CurWid / 2) + 1].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[1][(_level.CurWid / 2) + 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[1][(_level.CurWid / 2) + 2].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[1][(_level.CurWid / 2) + 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[0][(_level.CurWid / 2) - 2].SetFeature("TownWall");
            _level.Grid[0][(_level.CurWid / 2) - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[0][(_level.CurWid / 2) - 1].SetFeature("TownWall");
            _level.Grid[0][(_level.CurWid / 2) - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[0][_level.CurWid / 2].SetFeature("PathBorderNS");
            _level.Grid[0][_level.CurWid / 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[0][(_level.CurWid / 2) + 1].SetFeature("TownWall");
            _level.Grid[0][(_level.CurWid / 2) + 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[0][(_level.CurWid / 2) + 2].SetFeature("TownWall");
            _level.Grid[0][(_level.CurWid / 2) + 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[1][(_level.CurWid / 2) - 2].SetFeature("TownWall");
            _level.Grid[1][(_level.CurWid / 2) - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[1][(_level.CurWid / 2) - 1].SetFeature("TownWall");
            _level.Grid[1][(_level.CurWid / 2) - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[1][_level.CurWid / 2].SetFeature("PathBase");
            _level.Grid[1][_level.CurWid / 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[1][(_level.CurWid / 2) + 1].SetFeature("TownWall");
            _level.Grid[1][(_level.CurWid / 2) + 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[1][(_level.CurWid / 2) + 2].SetFeature("TownWall");
            _level.Grid[1][(_level.CurWid / 2) + 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[_level.CurHgt - 1][(_level.CurWid / 2) - 2].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[_level.CurHgt - 1][(_level.CurWid / 2) - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[_level.CurHgt - 1][(_level.CurWid / 2) - 1].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[_level.CurHgt - 1][(_level.CurWid / 2) - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[_level.CurHgt - 1][(_level.CurWid / 2) + 1].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[_level.CurHgt - 1][(_level.CurWid / 2) + 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[_level.CurHgt - 1][(_level.CurWid / 2) + 2].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[_level.CurHgt - 1][(_level.CurWid / 2) + 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[_level.CurHgt - 2][(_level.CurWid / 2) - 2].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[_level.CurHgt - 2][(_level.CurWid / 2) - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[_level.CurHgt - 2][(_level.CurWid / 2) - 1].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[_level.CurHgt - 2][(_level.CurWid / 2) - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[_level.CurHgt - 2][(_level.CurWid / 2) + 1].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[_level.CurHgt - 2][(_level.CurWid / 2) + 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[_level.CurHgt - 2][(_level.CurWid / 2) + 2].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[_level.CurHgt - 2][(_level.CurWid / 2) + 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[_level.CurHgt - 1][(_level.CurWid / 2) - 2].SetFeature("TownWall");
            _level.Grid[_level.CurHgt - 1][(_level.CurWid / 2) - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[_level.CurHgt - 1][(_level.CurWid / 2) - 1].SetFeature("TownWall");
            _level.Grid[_level.CurHgt - 1][(_level.CurWid / 2) - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[_level.CurHgt - 1][_level.CurWid / 2].SetFeature("PathBorderNS");
            _level.Grid[_level.CurHgt - 1][_level.CurWid / 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[_level.CurHgt - 1][(_level.CurWid / 2) + 1].SetFeature("TownWall");
            _level.Grid[_level.CurHgt - 1][(_level.CurWid / 2) + 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[_level.CurHgt - 1][(_level.CurWid / 2) + 2].SetFeature("TownWall");
            _level.Grid[_level.CurHgt - 1][(_level.CurWid / 2) + 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[_level.CurHgt - 2][(_level.CurWid / 2) - 2].SetFeature("TownWall");
            _level.Grid[_level.CurHgt - 2][(_level.CurWid / 2) - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[_level.CurHgt - 2][(_level.CurWid / 2) - 1].SetFeature("TownWall");
            _level.Grid[_level.CurHgt - 2][(_level.CurWid / 2) - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[_level.CurHgt - 2][_level.CurWid / 2].SetFeature("PathBase");
            _level.Grid[_level.CurHgt - 2][_level.CurWid / 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[_level.CurHgt - 2][(_level.CurWid / 2) + 1].SetFeature("TownWall");
            _level.Grid[_level.CurHgt - 2][(_level.CurWid / 2) + 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[_level.CurHgt - 2][(_level.CurWid / 2) + 2].SetFeature("TownWall");
            _level.Grid[_level.CurHgt - 2][(_level.CurWid / 2) + 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) - 2][0].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[(_level.CurHgt / 2) - 2][0].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) - 1][0].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[(_level.CurHgt / 2) - 1][0].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) + 1][0].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[(_level.CurHgt / 2) + 1][0].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) + 2][0].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[(_level.CurHgt / 2) + 2][0].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) - 2][1].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[(_level.CurHgt / 2) - 2][1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) - 1][1].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[(_level.CurHgt / 2) - 1][1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) + 1][1].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[(_level.CurHgt / 2) + 1][1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) + 2][1].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[(_level.CurHgt / 2) + 2][1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) - 2][0].SetFeature("TownWall");
            _level.Grid[(_level.CurHgt / 2) - 2][0].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) - 1][0].SetFeature("TownWall");
            _level.Grid[(_level.CurHgt / 2) - 1][0].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[_level.CurHgt / 2][0].SetFeature("PathBorderEW");
            _level.Grid[_level.CurHgt / 2][0].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) + 1][0].SetFeature("TownWall");
            _level.Grid[(_level.CurHgt / 2) + 1][0].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) + 2][0].SetFeature("TownWall");
            _level.Grid[(_level.CurHgt / 2) + 2][0].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) - 2][1].SetFeature("TownWall");
            _level.Grid[(_level.CurHgt / 2) - 2][1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) - 1][1].SetFeature("TownWall");
            _level.Grid[(_level.CurHgt / 2) - 1][1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[_level.CurHgt / 2][1].SetFeature("PathBase");
            _level.Grid[_level.CurHgt / 2][1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) + 1][1].SetFeature("TownWall");
            _level.Grid[(_level.CurHgt / 2) + 1][1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) + 2][1].SetFeature("TownWall");
            _level.Grid[(_level.CurHgt / 2) + 2][1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) - 2][_level.CurWid - 1].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[(_level.CurHgt / 2) - 2][_level.CurWid - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) - 1][_level.CurWid - 1].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[(_level.CurHgt / 2) - 1][_level.CurWid - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) + 1][_level.CurWid - 1].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[(_level.CurHgt / 2) + 1][_level.CurWid - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) + 2][_level.CurWid - 1].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[(_level.CurHgt / 2) + 2][_level.CurWid - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) - 2][_level.CurWid - 2].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[(_level.CurHgt / 2) - 2][_level.CurWid - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) - 1][_level.CurWid - 2].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[(_level.CurHgt / 2) - 1][_level.CurWid - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) + 1][_level.CurWid - 2].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[(_level.CurHgt / 2) + 1][_level.CurWid - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) + 2][_level.CurWid - 2].SetBackgroundFeature("InsideGatehouse");
            _level.Grid[(_level.CurHgt / 2) + 2][_level.CurWid - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) - 2][_level.CurWid - 1].SetFeature("TownWall");
            _level.Grid[(_level.CurHgt / 2) - 2][_level.CurWid - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) - 1][_level.CurWid - 1].SetFeature("TownWall");
            _level.Grid[(_level.CurHgt / 2) - 1][_level.CurWid - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[_level.CurHgt / 2][_level.CurWid - 1].SetFeature("PathBorderEW");
            _level.Grid[_level.CurHgt / 2][_level.CurWid - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) + 1][_level.CurWid - 1].SetFeature("TownWall");
            _level.Grid[(_level.CurHgt / 2) + 1][_level.CurWid - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) + 2][_level.CurWid - 1].SetFeature("TownWall");
            _level.Grid[(_level.CurHgt / 2) + 2][_level.CurWid - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) - 2][_level.CurWid - 2].SetFeature("TownWall");
            _level.Grid[(_level.CurHgt / 2) - 2][_level.CurWid - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) - 1][_level.CurWid - 2].SetFeature("TownWall");
            _level.Grid[(_level.CurHgt / 2) - 1][_level.CurWid - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[_level.CurHgt / 2][_level.CurWid - 2].SetFeature("PathBase");
            _level.Grid[_level.CurHgt / 2][_level.CurWid - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) + 1][_level.CurWid - 2].SetFeature("TownWall");
            _level.Grid[(_level.CurHgt / 2) + 1][_level.CurWid - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            _level.Grid[(_level.CurHgt / 2) + 2][_level.CurWid - 2].SetFeature("TownWall");
            _level.Grid[(_level.CurHgt / 2) + 2][_level.CurWid - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
        }

        private void MakeWildernessFeatures(int wildx, int wildy, out int stairX, out int stairY)
        {
            stairX = _level.CurWid / 2;
            stairY = _level.CurHgt / 2;
            if (wildx == 1 || wildx == 10 || wildy == 1 || wildy == 10)
            {
                return;
            }
            var dungeonX = 0;
            var dungeonY = 0;
            switch (Program.Rng.DieRoll(4))
            {
                case 1:
                    dungeonX = 0;
                    dungeonY = 0;
                    break;

                case 2:
                    dungeonX = _level.CurWid / 2;
                    dungeonY = 0;
                    break;

                case 3:
                    dungeonX = 0;
                    dungeonY = _level.CurHgt / 2;
                    break;

                case 4:
                    dungeonX = _level.CurWid / 2;
                    dungeonY = _level.CurHgt / 2;
                    break;
            }
            for (var offsetX = 0; offsetX < _level.CurWid - 1; offsetX += _level.CurWid / 2)
            {
                for (var offsetY = 0; offsetY < _level.CurHgt - 1; offsetY += _level.CurHgt / 2)
                {
                    if (offsetX == dungeonX && offsetY == dungeonY)
                    {
                        if (SaveGame.Instance.Wilderness[wildy][wildx].Dungeon != null)
                        {
                            if (SaveGame.Instance.Wilderness[wildy][wildx].Dungeon.Tower)
                            {
                                MakeTower(offsetX + 4, offsetY + 4, (_level.CurWid / 2) - 8, (_level.CurHgt / 2) - 8, out var x, out var y);
                                stairX = x;
                                stairY = y;
                            }
                            else
                            {
                                MakeDungeonEntrance(offsetX + 4, offsetY + 4, (_level.CurWid / 2) - 8, (_level.CurHgt / 2) - 8, out var x, out var y);
                                stairX = x;
                                stairY = y;
                            }
                        }
                    }
                    else
                    {
                        switch (Program.Rng.DieRoll(30))
                        {
                            case 7:
                            case 22:
                                MakeLake(offsetX + 4, offsetY + 4, (_level.CurWid / 2) - 8, (_level.CurHgt / 2) - 8);
                                break;

                            case 15:
                                MakeHenge(offsetX + 4, offsetY + 4, (_level.CurWid / 2) - 8, (_level.CurHgt / 2) - 8);
                                break;
                        }
                    }
                }
            }
        }

        private void MakeWildernessPaths(int wildx, int wildy)
        {
            int x;
            int y;

            var midX = _level.CurWid / 2;
            var midY = _level.CurHgt / 2;
            if (SaveGame.Instance.Wilderness[wildy][wildx].RoadMap == 0)
            {
                return;
            }
            _level.Grid[midY - 1][midX - 1].SetFeature("Grass");
            _level.Grid[midY - 1][midX].SetFeature("Grass");
            _level.Grid[midY - 1][midX + 1].SetFeature("Grass");
            _level.Grid[midY][midX - 1].SetFeature("Grass");
            _level.Grid[midY][midX].SetFeature("PathBase");
            _level.Grid[midY][midX + 1].SetFeature("Grass");
            _level.Grid[midY + 1][midX - 1].SetFeature("Grass");
            _level.Grid[midY + 1][midX].SetFeature("Grass");
            _level.Grid[midY + 1][midX + 1].SetFeature("Grass");
            if ((SaveGame.Instance.Wilderness[wildy][wildx].RoadMap & Constants.RoadUp) != 0)
            {
                x = 0;
                _level.Grid[0][midX].SetFeature("PathBorderNS");
                _level.Grid[1][midX].SetFeature("PathBase");
                _level.Grid[midY - 1][midX].SetFeature("PathBase");
                for (y = 2; y < midY - 1; y++)
                {
                    x += Program.Rng.RandomBetween(-2, 2) / 2;
                    if (x > midY - 1 - y)
                    {
                        x = midY - 1 - y;
                    }
                    if (x < -(midY - 1 - y))
                    {
                        x = -(midY - 1 - y);
                    }
                    if (!_level.Grid[y][midX - 1 + x].FeatureType.Name.StartsWith("WildPath"))
                    {
                        _level.Grid[y][midX - 1 + x].SetFeature("Grass");
                    }
                    _level.Grid[y][midX + x].SetFeature("WildPathNS");
                    if (!_level.Grid[y][midX + 1 + x].FeatureType.Name.StartsWith("WildPath"))
                    {
                        _level.Grid[y][midX + 1 + x].SetFeature("Grass");
                    }
                }
            }
            if ((SaveGame.Instance.Wilderness[wildy][wildx].RoadMap & Constants.RoadDown) != 0)
            {
                x = 0;
                _level.Grid[_level.CurHgt - 1][midX].SetFeature("PathBorderNS");
                _level.Grid[_level.CurHgt - 2][midX].SetFeature("PathBase");
                _level.Grid[midY + 1][midX].SetFeature("PathBase");
                for (y = _level.CurHgt - 3; y > midY + 1; y--)
                {
                    x += Program.Rng.RandomBetween(-2, 2) / 2;
                    if (x > y - (midY + 1))
                    {
                        x = y - (midY + 1);
                    }
                    if (x < -(y - (midY + 1)))
                    {
                        x = -(y - (midY + 1));
                    }
                    if (!_level.Grid[y][midX - 1 + x].FeatureType.Name.StartsWith("WildPath"))
                    {
                        _level.Grid[y][midX - 1 + x].SetFeature("Grass");
                    }
                    _level.Grid[y][midX + x].SetFeature("WildPathNS");
                    if (!_level.Grid[y][midX + 1 + x].FeatureType.Name.StartsWith("WildPath"))
                    {
                        _level.Grid[y][midX + 1 + x].SetFeature("Grass");
                    }
                }
            }
            if ((SaveGame.Instance.Wilderness[wildy][wildx].RoadMap & Constants.RoadLeft) != 0)
            {
                y = 0;
                _level.Grid[midY][0].SetFeature("PathBorderEW");
                _level.Grid[midY][1].SetFeature("PathBase");
                _level.Grid[midY][midX - 1].SetFeature("PathBase");
                for (x = 2; x < midX - 1; x++)
                {
                    y += Program.Rng.RandomBetween(-2, 2) / 2;
                    if (y > midX - 1 - x)
                    {
                        y = midX - 1 - x;
                    }
                    if (y < -(midX - 1 - x))
                    {
                        y = -(midX - 1 - x);
                    }
                    if (!_level.Grid[midY - 1 + y][x].FeatureType.Name.StartsWith("WildPath"))
                    {
                        _level.Grid[midY - 1 + y][x].SetFeature("Grass");
                    }
                    _level.Grid[midY + y][x].SetFeature("WildPathEW");
                    if (!_level.Grid[midY + 1 + y][x].FeatureType.Name.StartsWith("WildPath"))
                    {
                        _level.Grid[midY + 1 + y][x].SetFeature("Grass");
                    }
                }
            }
            if ((SaveGame.Instance.Wilderness[wildy][wildx].RoadMap & Constants.RoadRight) != 0)
            {
                y = 0;
                _level.Grid[midY][_level.CurWid - 1].SetFeature("PathBorderEW");
                _level.Grid[midY][_level.CurWid - 2].SetFeature("PathBase");
                _level.Grid[midY][midX + 1].SetFeature("PathBase");
                for (x = _level.CurWid - 3; x > midX + 1; x--)
                {
                    y += Program.Rng.RandomBetween(-2, 2) / 2;
                    if (y > x - (midX + 1))
                    {
                        y = x - (midX + 1);
                    }
                    if (y < -(x - (midX + 1)))
                    {
                        y = -(x - (midX + 1));
                    }
                    if (!_level.Grid[midY - 1 + y][x].FeatureType.Name.StartsWith("WildPath"))
                    {
                        _level.Grid[midY - 1 + y][x].SetFeature("Grass");
                    }
                    _level.Grid[midY + y][x].SetFeature("WildPathEW");
                    if (!_level.Grid[midY + 1 + y][x].FeatureType.Name.StartsWith("WildPath"))
                    {
                        _level.Grid[midY + 1 + y][x].SetFeature("Grass");
                    }
                }
            }
        }

        private void MakeWildernessWalls(int wildX, int wildY)
        {
            var wilderness = SaveGame.Instance.Wilderness;
            var height = _level.CurHgt;
            var width = _level.CurWid;
            if (wilderness[wildY - 1][wildX].Town != null)
            {
                for (var x = 0; x < width; x++)
                {
                    _level.Grid[0][x].SetFeature("TownWall");
                    _level.Grid[0][x].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                }
                _level.Grid[0][(width / 2) - 2].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[0][(width / 2) - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[0][(width / 2) - 1].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[0][(width / 2) - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[0][(width / 2) + 1].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[0][(width / 2) + 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[0][(width / 2) + 2].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[0][(width / 2) + 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[1][(width / 2) - 2].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[1][(width / 2) - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[1][(width / 2) - 1].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[1][(width / 2) - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[1][(width / 2) + 1].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[1][(width / 2) + 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[1][(width / 2) + 2].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[1][(width / 2) + 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[0][(width / 2) - 2].SetFeature("TownWall");
                _level.Grid[0][(width / 2) - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[0][(width / 2) - 1].SetFeature("TownWall");
                _level.Grid[0][(width / 2) - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[0][width / 2].SetFeature("PathBorderNS");
                _level.Grid[0][width / 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[0][(width / 2) + 1].SetFeature("TownWall");
                _level.Grid[0][(width / 2) + 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[0][(width / 2) + 2].SetFeature("TownWall");
                _level.Grid[0][(width / 2) + 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[1][(width / 2) - 2].SetFeature("TownWall");
                _level.Grid[1][(width / 2) - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[1][(width / 2) - 1].SetFeature("TownWall");
                _level.Grid[1][(width / 2) - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[1][width / 2].SetFeature("PathBase");
                _level.Grid[1][width / 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[1][(width / 2) + 1].SetFeature("TownWall");
                _level.Grid[1][(width / 2) + 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[1][(width / 2) + 2].SetFeature("TownWall");
                _level.Grid[1][(width / 2) + 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            }
            if (wilderness[wildY + 1][wildX].Town != null)
            {
                for (var x = 0; x < width; x++)
                {
                    _level.Grid[height - 1][x].SetFeature("TownWall");
                    _level.Grid[height - 1][x].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                }
                _level.Grid[height - 1][(width / 2) - 2].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[height - 1][(width / 2) - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 1][(width / 2) - 1].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[height - 1][(width / 2) - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 1][(width / 2) + 1].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[height - 1][(width / 2) + 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 1][(width / 2) + 2].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[height - 1][(width / 2) + 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 2][(width / 2) - 2].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[height - 2][(width / 2) - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 2][(width / 2) - 1].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[height - 2][(width / 2) - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 2][(width / 2) + 1].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[height - 2][(width / 2) + 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 2][(width / 2) + 2].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[height - 2][(width / 2) + 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 1][(width / 2) - 2].SetFeature("TownWall");
                _level.Grid[height - 1][(width / 2) - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 1][(width / 2) - 1].SetFeature("TownWall");
                _level.Grid[height - 1][(width / 2) - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 1][width / 2].SetFeature("PathBorderNS");
                _level.Grid[height - 1][width / 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 1][(width / 2) + 1].SetFeature("TownWall");
                _level.Grid[height - 1][(width / 2) + 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 1][(width / 2) + 2].SetFeature("TownWall");
                _level.Grid[height - 1][(width / 2) + 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 2][(width / 2) - 2].SetFeature("TownWall");
                _level.Grid[height - 2][(width / 2) - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 2][(width / 2) - 1].SetFeature("TownWall");
                _level.Grid[height - 2][(width / 2) - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 2][width / 2].SetFeature("PathBase");
                _level.Grid[height - 2][width / 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 2][(width / 2) + 1].SetFeature("TownWall");
                _level.Grid[height - 2][(width / 2) + 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height - 2][(width / 2) + 2].SetFeature("TownWall");
                _level.Grid[height - 2][(width / 2) + 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            }
            if (wilderness[wildY][wildX - 1].Town != null)
            {
                for (var y = 0; y < height; y++)
                {
                    _level.Grid[y][0].SetFeature("TownWall");
                    _level.Grid[y][0].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                }
                _level.Grid[(height / 2) - 2][0].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[(height / 2) - 2][0].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) - 1][0].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[(height / 2) - 1][0].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) + 1][0].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[(height / 2) + 1][0].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) + 2][0].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[(height / 2) + 2][0].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) - 2][1].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[(height / 2) - 2][1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) - 1][1].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[(height / 2) - 1][1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) + 1][1].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[(height / 2) + 1][1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) + 2][1].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[(height / 2) + 2][1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) - 2][0].SetFeature("TownWall");
                _level.Grid[(height / 2) - 2][0].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) - 1][0].SetFeature("TownWall");
                _level.Grid[(height / 2) - 1][0].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height / 2][0].SetFeature("PathBorderEW");
                _level.Grid[height / 2][0].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) + 1][0].SetFeature("TownWall");
                _level.Grid[(height / 2) + 1][0].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) + 2][0].SetFeature("TownWall");
                _level.Grid[(height / 2) + 2][0].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) - 2][1].SetFeature("TownWall");
                _level.Grid[(height / 2) - 2][1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) - 1][1].SetFeature("TownWall");
                _level.Grid[(height / 2) - 1][1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height / 2][1].SetFeature("PathBase");
                _level.Grid[height / 2][1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) + 1][1].SetFeature("TownWall");
                _level.Grid[(height / 2) + 1][1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) + 2][1].SetFeature("TownWall");
                _level.Grid[(height / 2) + 2][1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            }
            if (wilderness[wildY][wildX + 1].Town != null)
            {
                for (var y = 0; y < height; y++)
                {
                    _level.Grid[y][width - 1].SetFeature("TownWall");
                    _level.Grid[y][width - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                }
                _level.Grid[(height / 2) - 2][width - 1].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[(height / 2) - 2][width - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) - 1][width - 1].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[(height / 2) - 1][width - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) + 1][width - 1].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[(height / 2) + 1][width - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) + 2][width - 1].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[(height / 2) + 2][width - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) - 2][width - 2].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[(height / 2) - 2][width - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) - 1][width - 2].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[(height / 2) - 1][width - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) + 1][width - 2].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[(height / 2) + 1][width - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) + 2][width - 2].SetBackgroundFeature("InsideGatehouse");
                _level.Grid[(height / 2) + 2][width - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) - 2][width - 1].SetFeature("TownWall");
                _level.Grid[(height / 2) - 2][width - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) - 1][width - 1].SetFeature("TownWall");
                _level.Grid[(height / 2) - 1][width - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height / 2][width - 1].SetFeature("PathBorderEW");
                _level.Grid[height / 2][width - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) + 1][width - 1].SetFeature("TownWall");
                _level.Grid[(height / 2) + 1][width - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) + 2][width - 1].SetFeature("TownWall");
                _level.Grid[(height / 2) + 2][width - 1].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) - 2][width - 2].SetFeature("TownWall");
                _level.Grid[(height / 2) - 2][width - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) - 1][width - 2].SetFeature("TownWall");
                _level.Grid[(height / 2) - 1][width - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[height / 2][width - 2].SetFeature("PathBase");
                _level.Grid[height / 2][width - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) + 1][width - 2].SetFeature("TownWall");
                _level.Grid[(height / 2) + 1][width - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
                _level.Grid[(height / 2) + 2][width - 2].SetFeature("TownWall");
                _level.Grid[(height / 2) + 2][width - 2].TileFlags.Set(GridTile.SelfLit | GridTile.PlayerMemorised);
            }
        }

        private bool NewPlayerSpot()
        {
            var y = 0;
            var x = 0;
            var maxAttempts = 5000;
            while (maxAttempts-- != 0)
            {
                y = Program.Rng.RandomBetween(1, _level.CurHgt - 2);
                x = Program.Rng.RandomBetween(1, _level.CurWid - 2);
                if (!_level.GridOpenNoItemOrCreature(y, x))
                {
                    continue;
                }
                if (_level.Grid[y][x].TileFlags.IsSet(GridTile.InVault))
                {
                    continue;
                }
                break;
            }
            if (maxAttempts < 1)
            {
                return false;
            }
            SaveGame.Instance.Player.MapY = y;
            SaveGame.Instance.Player.MapX = x;
            return true;
        }

        private int NextToCorr(int y1, int x1)
        {
            var k = 0;
            for (var i = 0; i < 4; i++)
            {
                var y = y1 + _level.OrderedDirectionYOffset[i];
                var x = x1 + _level.OrderedDirectionXOffset[i];
                if (!_level.GridPassable(y, x))
                {
                    continue;
                }
                var cPtr = _level.Grid[y][x];
                if (!cPtr.FeatureType.IsOpenFloor)
                {
                    continue;
                }
                if (cPtr.TileFlags.IsSet(GridTile.InRoom))
                {
                    continue;
                }
                k++;
            }
            return k;
        }

        private int NextToWalls(int y, int x)
        {
            var k = 0;
            if (_level.Grid[y + 1][x].FeatureType.IsWall)
            {
                k++;
            }
            if (_level.Grid[y - 1][x].FeatureType.IsWall)
            {
                k++;
            }
            if (_level.Grid[y][x + 1].FeatureType.IsWall)
            {
                k++;
            }
            if (_level.Grid[y][x - 1].FeatureType.IsWall)
            {
                k++;
            }
            return k;
        }

        private void PlaceRandomDoor(int y, int x)
        {
            var cPtr = _level.Grid[y][x];
            var tmp = Program.Rng.RandomLessThan(1000);
            if (tmp < 300)
            {
                cPtr.SetFeature("OpenDoor");
            }
            else if (tmp < 400)
            {
                cPtr.SetFeature("BrokenDoor");
            }
            else if (tmp < 600)
            {
                cPtr.SetFeature("SecretDoor");
            }
            else if (tmp < 900)
            {
                cPtr.SetFeature("LockedDoor0");
            }
            else if (tmp < 999)
            {
                cPtr.SetFeature($"LockedDoor{Program.Rng.DieRoll(7)}");
            }
            else
            {
                cPtr.SetFeature($"JammedDoor{Program.Rng.RandomLessThan(8)}");
            }
        }

        private void PlaceRubble(int y, int x)
        {
            var cPtr = _level.Grid[y][x];
            cPtr.SetFeature("Rubble");
        }

        private bool PossibleDoorway(int y, int x)
        {
            if (NextToCorr(y, x) >= 2)
            {
                if (_level.Grid[y - 1][x].FeatureType.IsWall &&
                    _level.Grid[y + 1][x].FeatureType.IsWall)
                {
                    return true;
                }
                if (_level.Grid[y][x - 1].FeatureType.IsWall &&
                    _level.Grid[y][x + 1].FeatureType.IsWall)
                {
                    return true;
                }
            }
            return false;
        }

        private void RandDir(out int rdir, out int cdir)
        {
            var i = Program.Rng.RandomLessThan(4);
            rdir = _level.OrderedDirectionYOffset[i];
            cdir = _level.OrderedDirectionXOffset[i];
        }

        private void ResolvePaths()
        {
            for (var x = 1; x < _level.CurWid - 1; x++)
            {
                for (var y = 1; y < _level.CurHgt - 1; y++)
                {
                    if (_level.Grid[y][x].FeatureType.Name != "PathBase")
                    {
                        continue;
                    }
                    var map = 0;
                    if (_level.Grid[y - 1][x].FeatureType.Name.StartsWith("Path"))
                    {
                        map++;
                    }
                    if (_level.Grid[y][x + 1].FeatureType.Name.StartsWith("Path"))
                    {
                        map += 2;
                    }
                    if (_level.Grid[y + 1][x].FeatureType.Name.StartsWith("Path"))
                    {
                        map += 4;
                    }
                    if (_level.Grid[y][x - 1].FeatureType.Name.StartsWith("Path"))
                    {
                        map += 8;
                    }
                    switch (map)
                    {
                        case 1:
                        case 4:
                        case 5:
                            _level.Grid[y][x].SetFeature("PathNS");
                            break;

                        case 2:
                        case 8:
                        case 10:
                            _level.Grid[y][x].SetFeature("PathEW");
                            break;

                        default:
                            _level.Grid[y][x].SetFeature("PathJunction");
                            break;
                    }
                }
            }
        }

        private bool RoomBuild(int y0, int x0, int typ)
        {
            var vaultFactory = new VaultFactory(_level);
            if (SaveGame.Instance.Difficulty < _room[typ].Level)
            {
                return false;
            }
            if (_dun.Crowded && (typ == 5 || typ == 6))
            {
                return false;
            }
            var y1 = y0 + _room[typ].Dy1;
            var y2 = y0 + _room[typ].Dy2;
            var x1 = x0 + _room[typ].Dx1;
            var x2 = x0 + _room[typ].Dx2;
            if (y1 < 0 || y2 >= _dun.RowRooms)
            {
                return false;
            }
            if (x1 < 0 || x2 >= _dun.ColRooms)
            {
                return false;
            }
            int y;
            int x;
            for (y = y1; y <= y2; y++)
            {
                for (x = x1; x <= x2; x++)
                {
                    if (_dun.RoomMap[y][x])
                    {
                        return false;
                    }
                }
            }
            y = (y1 + y2 + 1) * _blockHgt / 2;
            x = (x1 + x2 + 1) * _blockWid / 2;
            switch (typ)
            {
                case 8:
                    vaultFactory.BuildType8(y, x);
                    break;

                case 7:
                    vaultFactory.BuildType7(y, x);
                    break;

                case 6:
                    vaultFactory.BuildType6(y, x);
                    break;

                case 5:
                    vaultFactory.BuildType5(y, x);
                    break;

                case 4:
                    vaultFactory.BuildType4(y, x);
                    break;

                case 3:
                    vaultFactory.BuildType3(y, x);
                    break;

                case 2:
                    vaultFactory.BuildType2(y, x);
                    break;

                case 1:
                    vaultFactory.BuildType1(y, x);
                    break;

                default:
                    return false;
            }
            if (_dun.CentN < CentMax)
            {
                _dun.Cent[_dun.CentN].Y = y;
                _dun.Cent[_dun.CentN].X = x;
                _dun.CentN++;
            }
            for (y = y1; y <= y2; y++)
            {
                for (x = x1; x <= x2; x++)
                {
                    _dun.RoomMap[y][x] = true;
                }
            }
            if (typ == 5 || typ == 6)
            {
                _dun.Crowded = true;
            }
            return true;
        }

        private void TownGen()
        {
            int i, y, x;
            GridTile cPtr;
            for (y = 0; y < _level.CurHgt; y++)
            {
                for (x = 0; x < _level.CurWid; x++)
                {
                    cPtr = _level.Grid[y][x];
                    cPtr.RevertToBackground();
                }
            }
            MakeTownWalls();
            MakeCornerTowers(SaveGame.Instance.Player.WildernessX, SaveGame.Instance.Player.WildernessY);
            MakeTownContents();
            ResolvePaths();
            if (SaveGame.Instance.Player.GameTime.IsLight)
            {
                for (y = 0; y < _level.CurHgt; y++)
                {
                    for (x = 0; x < _level.CurWid; x++)
                    {
                        cPtr = _level.Grid[y][x];
                        cPtr.TileFlags.Set(GridTile.SelfLit);
                        cPtr.TileFlags.Set(GridTile.PlayerMemorised);
                    }
                }
                for (i = 0; i < Constants.MinMAllocTd; i++)
                {
                    _level.Monsters.AllocMonster(3, true);
                }
            }
            else
            {
                for (i = 0; i < Constants.MinMAllocTn; i++)
                {
                    _level.Monsters.AllocMonster(3, true);
                }
            }
        }

        private void TryDoor(int y, int x)
        {
            if (!_level.InBounds(y, x))
            {
                return;
            }
            if (_level.Grid[y][x].FeatureType.IsWall)
            {
                return;
            }
            if (_level.Grid[y][x].TileFlags.IsSet(GridTile.InRoom))
            {
                return;
            }
            if (Program.Rng.RandomLessThan(100) < _dunTunJct && PossibleDoorway(y, x))
            {
                PlaceRandomDoor(y, x);
            }
        }

        private bool UndergroundGen()
        {
            int i;
            int k;
            Profile.Instance.MonsterRaces.ResetGuardians();
            if (SaveGame.Instance.Quests.IsQuest(SaveGame.Instance.CurrentDepth))
            {
                Profile.Instance.MonsterRaces[SaveGame.Instance.Quests.GetQuestMonster()].Flags1 |=
                    MonsterFlag1.Guardian;
            }
            if (Program.Rng.PercentileRoll(4) && !SaveGame.Instance.CurDungeon.Tower)
            {
                MakeCavernLevel();
            }
            else
            {
                MakeDungeonLevel();
            }
            AllocStairs("DownStair", Program.Rng.RandomBetween(3, 4), 3);
            AllocStairs("UpStair", Program.Rng.RandomBetween(1, 2), 3);
            if (!NewPlayerSpot())
            {
                return false;
            }
            k = SaveGame.Instance.Difficulty / 3;
            if (k > 10)
            {
                k = 10;
            }
            if (k < 2)
            {
                k = 2;
            }
            if (SaveGame.Instance.Quests.IsQuest(SaveGame.Instance.CurrentDepth))
            {
                var rIdx = SaveGame.Instance.Quests.GetQuestMonster();
                var qIdx = SaveGame.Instance.Quests.GetQuestNumber();
                while (Profile.Instance.MonsterRaces[rIdx].CurNum < (SaveGame.Instance.Quests[qIdx].ToKill - SaveGame.Instance.Quests[qIdx].Killed))
                {
                    _level.PutQuestMonster(SaveGame.Instance.Quests[qIdx].RIdx);
                }
            }
            i = Constants.MinMAllocLevel;
            if (_level.CurHgt < Level.MaxHgt || _level.CurWid < Level.MaxWid)
            {
                var smallTester = i;
                i = i * _level.CurHgt / Level.MaxHgt;
                i = i * _level.CurWid / Level.MaxWid;
                i++;
                if (i > smallTester)
                {
                    i = smallTester;
                }
            }
            i += Program.Rng.DieRoll(8);
            for (i += k; i > 0; i--)
            {
                _level.Monsters.AllocMonster(0, true);
            }
            AllocObject(_allocSetBoth, _allocTypTrap, Program.Rng.DieRoll(k));
            AllocObject(_allocSetCorr, _allocTypRubble, Program.Rng.DieRoll(k));
            AllocObject(_allocSetRoom, _allocTypObject, Program.Rng.RandomNormal(_dunAmtRoom, 3));
            AllocObject(_allocSetBoth, _allocTypObject, Program.Rng.RandomNormal(_dunAmtItem, 3));
            AllocObject(_allocSetBoth, _allocTypGold, Program.Rng.RandomNormal(_dunAmtGold, 3));
            return true;
        }

        private void WildernessGen()
        {
            Program.Rng.UseFixed = true;
            Program.Rng.FixedSeed =
                SaveGame.Instance.Wilderness[SaveGame.Instance.Player.WildernessY][SaveGame.Instance.Player.WildernessX].Seed;
            var island = SaveGame.Instance.Wilderness;
            var player = SaveGame.Instance.Player;
            int x;
            int y;
            for (y = 0; y < _level.CurHgt; y++)
            {
                for (x = 0; x < _level.CurWid; x++)
                {
                    var elevation = island.Elevation(player.WildernessY, player.WildernessX, y, x);
                    var floorName = "Water";
                    var featureName = "Water";
                    if (elevation > 0)
                    {
                        floorName = "Grass";
                        if (Program.Rng.DieRoll(10) < elevation)
                        {
                            if (Program.Rng.DieRoll(10) < elevation)
                            {
                                featureName = "Tree";
                            }
                            else
                            {
                                featureName = "Bush";
                            }
                        }
                        else
                        {
                            featureName = "Grass";
                        }
                    }
                    _level.Grid[y][x].SetFeature(featureName);
                    _level.Grid[y][x].SetBackgroundFeature(floorName);
                }
            }
            for (x = 0; x < _level.CurWid; x++)
            {
                var cPtr = _level.Grid[0][x];
                cPtr.SetFeature(cPtr.BackgroundFeature.Name.StartsWith("Water") ? "WaterBorder" : "WildBorder");
                cPtr = _level.Grid[_level.CurHgt - 1][x];
                cPtr.SetFeature(cPtr.BackgroundFeature.Name.StartsWith("Water") ? "WaterBorder" : "WildBorder");
            }
            for (y = 0; y < _level.CurHgt; y++)
            {
                var cPtr = _level.Grid[y][0];
                cPtr.SetFeature(cPtr.BackgroundFeature.Name.StartsWith("Water") ? "WaterBorder" : "WildBorder");
                cPtr = _level.Grid[y][_level.CurWid - 1];
                cPtr.SetFeature(cPtr.BackgroundFeature.Name.StartsWith("Water") ? "WaterBorder" : "WildBorder");
            }
            MakeWildernessWalls(SaveGame.Instance.Player.WildernessX, SaveGame.Instance.Player.WildernessY);
            MakeCornerTowers(SaveGame.Instance.Player.WildernessX, SaveGame.Instance.Player.WildernessY);
            MakeWildernessPaths(SaveGame.Instance.Player.WildernessX, SaveGame.Instance.Player.WildernessY);
            MakeWildernessFeatures(SaveGame.Instance.Player.WildernessX, SaveGame.Instance.Player.WildernessY, out var stairX, out var stairY);
            var rocks = Program.Rng.RandomBetween(1, 10);
            for (var i = 0; i < rocks; i++)
            {
                x = Program.Rng.DieRoll(_level.CurWid - 2);
                y = Program.Rng.DieRoll(_level.CurHgt - 2);
                if (_level.Grid[y][x].FeatureType.Name != "Grass")
                {
                    continue;
                }
                _level.Grid[y][x].SetFeature("Rock");
            }
            Program.Rng.UseFixed = false;
            if (SaveGame.Instance.CameFrom == LevelStart.StartRandom)
            {
                NewPlayerSpot();
            }
            else if (SaveGame.Instance.CameFrom == LevelStart.StartStairs)
            {
                SaveGame.Instance.Player.MapY = stairY;
                SaveGame.Instance.Player.MapX = stairX;
            }
            else if (SaveGame.Instance.CameFrom == LevelStart.StartWalk)
            {
                if (_level.Grid[SaveGame.Instance.Player.MapY][SaveGame.Instance.Player.MapX].FeatureType.Category == FloorTileTypeCategory.Tree ||
                    _level.Grid[SaveGame.Instance.Player.MapY][SaveGame.Instance.Player.MapX].FeatureType.Name == "Water")
                {
                    _level.Grid[SaveGame.Instance.Player.MapY][SaveGame.Instance.Player.MapX].RevertToBackground();
                }
            }
            ResolvePaths();
            for (y = 1; y < _level.CurHgt - 1; y++)
            {
                for (x = 1; x < _level.CurWid - 1; x++)
                {
                    if (_level.Grid[y][x].FeatureType.IsOpenFloor)
                    {
                        _level.Grid[y][x].TileFlags.Set(GridTile.InRoom);
                    }
                }
            }
            if (SaveGame.Instance.Player.GameTime.IsLight)
            {
                for (y = 0; y < _level.CurHgt; y++)
                {
                    for (x = 0; x < _level.CurWid; x++)
                    {
                        _level.Grid[y][x].TileFlags.Set(GridTile.SelfLit);
                        _level.Grid[y][x].TileFlags.Set(GridTile.PlayerMemorised);
                    }
                }
            }
            for (x = 0; x < Constants.MinMAllocLevel; x++)
            {
                _level.Monsters.AllocMonster(3, true);
            }
        }
    }
}