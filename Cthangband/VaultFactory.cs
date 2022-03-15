// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;

namespace Cthangband
{
    internal class VaultFactory
    {
        private readonly Level _level;
        private int _templateRace;
        private uint _vaultAuxDragonMask4;

        public VaultFactory(Level level)
        {
            _level = level;
        }

        public void BuildType1(int yval, int xval)
        {
            int y, x;
            GridTile cPtr;
            bool light = SaveGame.Instance.Difficulty <= Program.Rng.DieRoll(25);
            int y1 = yval - Program.Rng.DieRoll(4);
            int y2 = yval + Program.Rng.DieRoll(3);
            int x1 = xval - Program.Rng.DieRoll(11);
            int x2 = xval + Program.Rng.DieRoll(11);
            for (y = y1 - 1; y <= y2 + 1; y++)
            {
                for (x = x1 - 1; x <= x2 + 1; x++)
                {
                    cPtr = _level.Grid[y][x];
                    cPtr.RevertToBackground();
                    cPtr.TileFlags.Set(GridTile.InRoom);
                    if (light)
                    {
                        cPtr.TileFlags.Set(GridTile.SelfLit);
                    }
                }
            }
            for (y = y1 - 1; y <= y2 + 1; y++)
            {
                cPtr = _level.Grid[y][x1 - 1];
                cPtr.SetFeature("WallOuter");
                cPtr = _level.Grid[y][x2 + 1];
                cPtr.SetFeature("WallOuter");
            }
            for (x = x1 - 1; x <= x2 + 1; x++)
            {
                cPtr = _level.Grid[y1 - 1][x];
                cPtr.SetFeature("WallOuter");
                cPtr = _level.Grid[y2 + 1][x];
                cPtr.SetFeature("WallOuter");
            }
            if (Program.Rng.RandomLessThan(20) == 0)
            {
                for (y = y1; y <= y2; y += 2)
                {
                    for (x = x1; x <= x2; x += 2)
                    {
                        cPtr = _level.Grid[y][x];
                        cPtr.SetFeature("Pillar");
                    }
                }
            }
            else if (Program.Rng.RandomLessThan(50) == 0)
            {
                for (y = y1 + 2; y <= y2 - 2; y += 2)
                {
                    cPtr = _level.Grid[y][x1];
                    cPtr.SetFeature("Pillar");
                    cPtr = _level.Grid[y][x2];
                    cPtr.SetFeature("Pillar");
                }
                for (x = x1 + 2; x <= x2 - 2; x += 2)
                {
                    cPtr = _level.Grid[y1][x];
                    cPtr.SetFeature("Pillar");
                    cPtr = _level.Grid[y2][x];
                    cPtr.SetFeature("Pillar");
                }
            }
        }

        public void BuildType2(int yval, int xval)
        {
            int y, x;
            GridTile cPtr;
            bool light = SaveGame.Instance.Difficulty <= Program.Rng.DieRoll(25);
            int y1A = yval - Program.Rng.DieRoll(4);
            int y2A = yval + Program.Rng.DieRoll(3);
            int x1A = xval - Program.Rng.DieRoll(11);
            int x2A = xval + Program.Rng.DieRoll(10);
            int y1B = yval - Program.Rng.DieRoll(3);
            int y2B = yval + Program.Rng.DieRoll(4);
            int x1B = xval - Program.Rng.DieRoll(10);
            int x2B = xval + Program.Rng.DieRoll(11);
            for (y = y1A - 1; y <= y2A + 1; y++)
            {
                for (x = x1A - 1; x <= x2A + 1; x++)
                {
                    cPtr = _level.Grid[y][x];
                    cPtr.RevertToBackground();
                    cPtr.TileFlags.Set(GridTile.InRoom);
                    if (light)
                    {
                        cPtr.TileFlags.Set(GridTile.SelfLit);
                    }
                }
            }
            for (y = y1B - 1; y <= y2B + 1; y++)
            {
                for (x = x1B - 1; x <= x2B + 1; x++)
                {
                    cPtr = _level.Grid[y][x];
                    cPtr.RevertToBackground();
                    cPtr.TileFlags.Set(GridTile.InRoom);
                    if (light)
                    {
                        cPtr.TileFlags.Set(GridTile.SelfLit);
                    }
                }
            }
            for (y = y1A - 1; y <= y2A + 1; y++)
            {
                cPtr = _level.Grid[y][x1A - 1];
                cPtr.SetFeature("WallOuter");
                cPtr = _level.Grid[y][x2A + 1];
                cPtr.SetFeature("WallOuter");
            }
            for (x = x1A - 1; x <= x2A + 1; x++)
            {
                cPtr = _level.Grid[y1A - 1][x];
                cPtr.SetFeature("WallOuter");
                cPtr = _level.Grid[y2A + 1][x];
                cPtr.SetFeature("WallOuter");
            }
            for (y = y1B - 1; y <= y2B + 1; y++)
            {
                cPtr = _level.Grid[y][x1B - 1];
                cPtr.SetFeature("WallOuter");
                cPtr = _level.Grid[y][x2B + 1];
                cPtr.SetFeature("WallOuter");
            }
            for (x = x1B - 1; x <= x2B + 1; x++)
            {
                cPtr = _level.Grid[y1B - 1][x];
                cPtr.SetFeature("WallOuter");
                cPtr = _level.Grid[y2B + 1][x];
                cPtr.SetFeature("WallOuter");
            }
            for (y = y1A; y <= y2A; y++)
            {
                for (x = x1A; x <= x2A; x++)
                {
                    cPtr = _level.Grid[y][x];
                    cPtr.RevertToBackground();
                }
            }
            for (y = y1B; y <= y2B; y++)
            {
                for (x = x1B; x <= x2B; x++)
                {
                    cPtr = _level.Grid[y][x];
                    cPtr.RevertToBackground();
                }
            }
        }

        public void BuildType3(int yval, int xval)
        {
            int y, x, wy;
            GridTile cPtr;
            bool light = SaveGame.Instance.Difficulty <= Program.Rng.DieRoll(25);
            int wx = wy = 1;
            int dy = Program.Rng.RandomBetween(3, 4);
            int dx = Program.Rng.RandomBetween(3, 11);
            int y1A = yval - dy;
            int y2A = yval + dy;
            int x1A = xval - wx;
            int x2A = xval + wx;
            int y1B = yval - wy;
            int y2B = yval + wy;
            int x1B = xval - dx;
            int x2B = xval + dx;
            for (y = y1A - 1; y <= y2A + 1; y++)
            {
                for (x = x1A - 1; x <= x2A + 1; x++)
                {
                    cPtr = _level.Grid[y][x];
                    cPtr.RevertToBackground();
                    cPtr.TileFlags.Set(GridTile.InRoom);
                    if (light)
                    {
                        cPtr.TileFlags.Set(GridTile.SelfLit);
                    }
                }
            }
            for (y = y1B - 1; y <= y2B + 1; y++)
            {
                for (x = x1B - 1; x <= x2B + 1; x++)
                {
                    cPtr = _level.Grid[y][x];
                    cPtr.RevertToBackground();
                    cPtr.TileFlags.Set(GridTile.InRoom);
                    if (light)
                    {
                        cPtr.TileFlags.Set(GridTile.SelfLit);
                    }
                }
            }
            for (y = y1A - 1; y <= y2A + 1; y++)
            {
                cPtr = _level.Grid[y][x1A - 1];
                cPtr.SetFeature("WallOuter");
                cPtr = _level.Grid[y][x2A + 1];
                cPtr.SetFeature("WallOuter");
            }
            for (x = x1A - 1; x <= x2A + 1; x++)
            {
                cPtr = _level.Grid[y1A - 1][x];
                cPtr.SetFeature("WallOuter");
                cPtr = _level.Grid[y2A + 1][x];
                cPtr.SetFeature("WallOuter");
            }
            for (y = y1B - 1; y <= y2B + 1; y++)
            {
                cPtr = _level.Grid[y][x1B - 1];
                cPtr.SetFeature("WallOuter");
                cPtr = _level.Grid[y][x2B + 1];
                cPtr.SetFeature("WallOuter");
            }
            for (x = x1B - 1; x <= x2B + 1; x++)
            {
                cPtr = _level.Grid[y1B - 1][x];
                cPtr.SetFeature("WallOuter");
                cPtr = _level.Grid[y2B + 1][x];
                cPtr.SetFeature("WallOuter");
            }
            for (y = y1A; y <= y2A; y++)
            {
                for (x = x1A; x <= x2A; x++)
                {
                    cPtr = _level.Grid[y][x];
                    cPtr.RevertToBackground();
                }
            }
            for (y = y1B; y <= y2B; y++)
            {
                for (x = x1B; x <= x2B; x++)
                {
                    cPtr = _level.Grid[y][x];
                    cPtr.RevertToBackground();
                }
            }
            switch (Program.Rng.RandomLessThan(4))
            {
                case 1:
                    {
                        for (y = y1B; y <= y2B; y++)
                        {
                            for (x = x1A; x <= x2A; x++)
                            {
                                cPtr = _level.Grid[y][x];
                                cPtr.SetFeature("WallInner");
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        for (y = y1B; y <= y2B; y++)
                        {
                            cPtr = _level.Grid[y][x1A];
                            cPtr.SetFeature("WallInner");
                            cPtr = _level.Grid[y][x2A];
                            cPtr.SetFeature("WallInner");
                        }
                        for (x = x1A; x <= x2A; x++)
                        {
                            cPtr = _level.Grid[y1B][x];
                            cPtr.SetFeature("WallInner");
                            cPtr = _level.Grid[y2B][x];
                            cPtr.SetFeature("WallInner");
                        }
                        switch (Program.Rng.RandomLessThan(4))
                        {
                            case 0:
                                PlaceSecretDoor(y1B, xval);
                                break;

                            case 1:
                                PlaceSecretDoor(y2B, xval);
                                break;

                            case 2:
                                PlaceSecretDoor(yval, x1A);
                                break;

                            case 3:
                                PlaceSecretDoor(yval, x2A);
                                break;
                        }
                        _level.PlaceObject(yval, xval, false, false);
                        VaultMonsters(yval, xval, Program.Rng.RandomLessThan(2) + 3);
                        VaultTraps(yval, xval, 4, 4, Program.Rng.RandomLessThan(3) + 2);
                        break;
                    }
                case 3:
                    {
                        if (Program.Rng.RandomLessThan(3) == 0)
                        {
                            for (y = y1B; y <= y2B; y++)
                            {
                                if (y == yval)
                                {
                                    continue;
                                }
                                cPtr = _level.Grid[y][x1A - 1];
                                cPtr.SetFeature("WallInner");
                                cPtr = _level.Grid[y][x2A + 1];
                                cPtr.SetFeature("WallInner");
                            }
                            for (x = x1A; x <= x2A; x++)
                            {
                                if (x == xval)
                                {
                                    continue;
                                }
                                cPtr = _level.Grid[y1B - 1][x];
                                cPtr.SetFeature("WallInner");
                                cPtr = _level.Grid[y2B + 1][x];
                                cPtr.SetFeature("WallInner");
                            }
                            if (Program.Rng.RandomLessThan(3) == 0)
                            {
                                PlaceSecretDoor(yval, x1A - 1);
                                PlaceSecretDoor(yval, x2A + 1);
                                PlaceSecretDoor(y1B - 1, xval);
                                PlaceSecretDoor(y2B + 1, xval);
                            }
                        }
                        else if (Program.Rng.RandomLessThan(3) == 0)
                        {
                            cPtr = _level.Grid[yval][xval];
                            cPtr.SetFeature("WallInner");
                            cPtr = _level.Grid[y1B][xval];
                            cPtr.SetFeature("WallInner");
                            cPtr = _level.Grid[y2B][xval];
                            cPtr.SetFeature("WallInner");
                            cPtr = _level.Grid[yval][x1A];
                            cPtr.SetFeature("WallInner");
                            cPtr = _level.Grid[yval][x2A];
                            cPtr.SetFeature("WallInner");
                        }
                        else if (Program.Rng.RandomLessThan(3) == 0)
                        {
                            cPtr = _level.Grid[yval][xval];
                            cPtr.SetFeature("Pillar");
                        }
                        break;
                    }
            }
        }

        public void BuildType4(int yval, int xval)
        {
            int y, x;
            GridTile cPtr;
            bool light = SaveGame.Instance.Difficulty <= Program.Rng.DieRoll(25);
            int y1 = yval - 4;
            int y2 = yval + 4;
            int x1 = xval - 11;
            int x2 = xval + 11;
            for (y = y1 - 1; y <= y2 + 1; y++)
            {
                for (x = x1 - 1; x <= x2 + 1; x++)
                {
                    cPtr = _level.Grid[y][x];
                    cPtr.RevertToBackground();
                    cPtr.TileFlags.Set(GridTile.InRoom);
                    if (light)
                    {
                        cPtr.TileFlags.Set(GridTile.SelfLit);
                    }
                }
            }
            for (y = y1 - 1; y <= y2 + 1; y++)
            {
                cPtr = _level.Grid[y][x1 - 1];
                cPtr.SetFeature("WallOuter");
                cPtr = _level.Grid[y][x2 + 1];
                cPtr.SetFeature("WallOuter");
            }
            for (x = x1 - 1; x <= x2 + 1; x++)
            {
                cPtr = _level.Grid[y1 - 1][x];
                cPtr.SetFeature("WallOuter");
                cPtr = _level.Grid[y2 + 1][x];
                cPtr.SetFeature("WallOuter");
            }
            y1 += 2;
            y2 -= 2;
            x1 += 2;
            x2 -= 2;
            for (y = y1 - 1; y <= y2 + 1; y++)
            {
                cPtr = _level.Grid[y][x1 - 1];
                cPtr.SetFeature("WallInner");
                cPtr = _level.Grid[y][x2 + 1];
                cPtr.SetFeature("WallInner");
            }
            for (x = x1 - 1; x <= x2 + 1; x++)
            {
                cPtr = _level.Grid[y1 - 1][x];
                cPtr.SetFeature("WallInner");
                cPtr = _level.Grid[y2 + 1][x];
                cPtr.SetFeature("WallInner");
            }
            switch (Program.Rng.DieRoll(5))
            {
                case 1:
                    switch (Program.Rng.DieRoll(4))
                    {
                        case 1:
                            PlaceSecretDoor(y1 - 1, xval);
                            break;

                        case 2:
                            PlaceSecretDoor(y2 + 1, xval);
                            break;

                        case 3:
                            PlaceSecretDoor(yval, x1 - 1);
                            break;

                        case 4:
                            PlaceSecretDoor(yval, x2 + 1);
                            break;
                    }
                    VaultMonsters(yval, xval, 1);
                    break;

                case 2:
                    switch (Program.Rng.DieRoll(4))
                    {
                        case 1:
                            PlaceSecretDoor(y1 - 1, xval);
                            break;

                        case 2:
                            PlaceSecretDoor(y2 + 1, xval);
                            break;

                        case 3:
                            PlaceSecretDoor(yval, x1 - 1);
                            break;

                        case 4:
                            PlaceSecretDoor(yval, x2 + 1);
                            break;
                    }
                    for (y = yval - 1; y <= yval + 1; y++)
                    {
                        for (x = xval - 1; x <= xval + 1; x++)
                        {
                            if (x == xval && y == yval)
                            {
                                continue;
                            }
                            cPtr = _level.Grid[y][x];
                            cPtr.SetFeature("WallInner");
                        }
                    }
                    switch (Program.Rng.DieRoll(4))
                    {
                        case 1:
                            PlaceLockedDoor(yval - 1, xval);
                            break;

                        case 2:
                            PlaceLockedDoor(yval + 1, xval);
                            break;

                        case 3:
                            PlaceLockedDoor(yval, xval - 1);
                            break;

                        case 4:
                            PlaceLockedDoor(yval, xval + 1);
                            break;
                    }
                    VaultMonsters(yval, xval, Program.Rng.DieRoll(3) + 2);
                    if (Program.Rng.RandomLessThan(100) < 80)
                    {
                        _level.PlaceObject(yval, xval, false, false);
                    }
                    else
                    {
                        PlaceRandomStairs(yval, xval);
                    }
                    VaultTraps(yval, xval, 4, 10, 2 + Program.Rng.DieRoll(3));
                    break;

                case 3:
                    switch (Program.Rng.DieRoll(4))
                    {
                        case 1:
                            PlaceSecretDoor(y1 - 1, xval);
                            break;

                        case 2:
                            PlaceSecretDoor(y2 + 1, xval);
                            break;

                        case 3:
                            PlaceSecretDoor(yval, x1 - 1);
                            break;

                        case 4:
                            PlaceSecretDoor(yval, x2 + 1);
                            break;
                    }
                    for (y = yval - 1; y <= yval + 1; y++)
                    {
                        for (x = xval - 1; x <= xval + 1; x++)
                        {
                            cPtr = _level.Grid[y][x];
                            cPtr.SetFeature("WallInner");
                        }
                    }
                    if (Program.Rng.RandomLessThan(2) == 0)
                    {
                        int tmp = Program.Rng.DieRoll(2);
                        for (y = yval - 1; y <= yval + 1; y++)
                        {
                            for (x = xval - 5 - tmp; x <= xval - 3 - tmp; x++)
                            {
                                cPtr = _level.Grid[y][x];
                                cPtr.SetFeature("WallInner");
                            }
                            for (x = xval + 3 + tmp; x <= xval + 5 + tmp; x++)
                            {
                                cPtr = _level.Grid[y][x];
                                cPtr.SetFeature("WallInner");
                            }
                        }
                    }
                    if (Program.Rng.RandomLessThan(3) == 0)
                    {
                        for (x = xval - 5; x <= xval + 5; x++)
                        {
                            cPtr = _level.Grid[yval - 1][x];
                            cPtr.SetFeature("WallInner");
                            cPtr = _level.Grid[yval + 1][x];
                            cPtr.SetFeature("WallInner");
                        }
                        cPtr = _level.Grid[yval][xval - 5];
                        cPtr.SetFeature("WallInner");
                        cPtr = _level.Grid[yval][xval + 5];
                        cPtr.SetFeature("WallInner");
                        PlaceSecretDoor(yval - 3 + (Program.Rng.DieRoll(2) * 2), xval - 3);
                        PlaceSecretDoor(yval - 3 + (Program.Rng.DieRoll(2) * 2), xval + 3);
                        VaultMonsters(yval, xval - 2, Program.Rng.DieRoll(2));
                        VaultMonsters(yval, xval + 2, Program.Rng.DieRoll(2));
                        if (Program.Rng.RandomLessThan(3) == 0)
                        {
                            _level.PlaceObject(yval, xval - 2, false, false);
                        }
                        if (Program.Rng.RandomLessThan(3) == 0)
                        {
                            _level.PlaceObject(yval, xval + 2, false, false);
                        }
                    }
                    break;

                case 4:
                    switch (Program.Rng.DieRoll(4))
                    {
                        case 1:
                            PlaceSecretDoor(y1 - 1, xval);
                            break;

                        case 2:
                            PlaceSecretDoor(y2 + 1, xval);
                            break;

                        case 3:
                            PlaceSecretDoor(yval, x1 - 1);
                            break;

                        case 4:
                            PlaceSecretDoor(yval, x2 + 1);
                            break;
                    }
                    for (y = y1; y <= y2; y++)
                    {
                        for (x = x1; x <= x2; x++)
                        {
                            if ((1 & (x + y)) != 0)
                            {
                                cPtr = _level.Grid[y][x];
                                cPtr.SetFeature("WallInner");
                            }
                        }
                    }
                    VaultMonsters(yval, xval - 5, Program.Rng.DieRoll(3));
                    VaultMonsters(yval, xval + 5, Program.Rng.DieRoll(3));
                    VaultTraps(yval, xval - 3, 2, 8, Program.Rng.DieRoll(3));
                    VaultTraps(yval, xval + 3, 2, 8, Program.Rng.DieRoll(3));
                    VaultObjects(yval, xval, 3);
                    break;

                case 5:
                    for (y = y1; y <= y2; y++)
                    {
                        cPtr = _level.Grid[y][xval];
                        cPtr.SetFeature("WallInner");
                    }
                    for (x = x1; x <= x2; x++)
                    {
                        cPtr = _level.Grid[yval][x];
                        cPtr.SetFeature("WallInner");
                    }
                    if (Program.Rng.RandomLessThan(100) < 50)
                    {
                        int i = Program.Rng.DieRoll(10);
                        PlaceSecretDoor(y1 - 1, xval - i);
                        PlaceSecretDoor(y1 - 1, xval + i);
                        PlaceSecretDoor(y2 + 1, xval - i);
                        PlaceSecretDoor(y2 + 1, xval + i);
                    }
                    else
                    {
                        int i = Program.Rng.DieRoll(3);
                        PlaceSecretDoor(yval + i, x1 - 1);
                        PlaceSecretDoor(yval - i, x1 - 1);
                        PlaceSecretDoor(yval + i, x2 + 1);
                        PlaceSecretDoor(yval - i, x2 + 1);
                    }
                    VaultObjects(yval, xval, 2 + Program.Rng.DieRoll(2));
                    VaultMonsters(yval + 1, xval - 4, Program.Rng.DieRoll(4));
                    VaultMonsters(yval + 1, xval + 4, Program.Rng.DieRoll(4));
                    VaultMonsters(yval - 1, xval - 4, Program.Rng.DieRoll(4));
                    VaultMonsters(yval - 1, xval + 4, Program.Rng.DieRoll(4));
                    break;
            }
        }

        public void BuildType5(int yval, int xval)
        {
            int y, x;
            int[] what = new int[64];
            GridTile cPtr;
            bool empty = false;
            int y1 = yval - 4;
            int y2 = yval + 4;
            int x1 = xval - 11;
            int x2 = xval + 11;
            for (y = y1 - 1; y <= y2 + 1; y++)
            {
                for (x = x1 - 1; x <= x2 + 1; x++)
                {
                    cPtr = _level.Grid[y][x];
                    cPtr.RevertToBackground();
                    cPtr.TileFlags.Set(GridTile.InRoom);
                }
            }
            for (y = y1 - 1; y <= y2 + 1; y++)
            {
                cPtr = _level.Grid[y][x1 - 1];
                cPtr.SetFeature("WallOuter");
                cPtr = _level.Grid[y][x2 + 1];
                cPtr.SetFeature("WallOuter");
            }
            for (x = x1 - 1; x <= x2 + 1; x++)
            {
                cPtr = _level.Grid[y1 - 1][x];
                cPtr.SetFeature("WallOuter");
                cPtr = _level.Grid[y2 + 1][x];
                cPtr.SetFeature("WallOuter");
            }
            y1 += 2;
            y2 -= 2;
            x1 += 2;
            x2 -= 2;
            for (y = y1 - 1; y <= y2 + 1; y++)
            {
                cPtr = _level.Grid[y][x1 - 1];
                cPtr.SetFeature("WallInner");
                cPtr = _level.Grid[y][x2 + 1];
                cPtr.SetFeature("WallInner");
            }
            for (x = x1 - 1; x <= x2 + 1; x++)
            {
                cPtr = _level.Grid[y1 - 1][x];
                cPtr.SetFeature("WallInner");
                cPtr = _level.Grid[y2 + 1][x];
                cPtr.SetFeature("WallInner");
            }
            switch (Program.Rng.DieRoll(4))
            {
                case 1:
                    PlaceSecretDoor(y1 - 1, xval);
                    break;

                case 2:
                    PlaceSecretDoor(y2 + 1, xval);
                    break;

                case 3:
                    PlaceSecretDoor(yval, x1 - 1);
                    break;

                case 4:
                    PlaceSecretDoor(yval, x2 + 1);
                    break;
            }
            int tmp = Program.Rng.DieRoll(SaveGame.Instance.Difficulty);
            if (tmp < 25 && Program.Rng.DieRoll(2) != 1)
            {
                do
                {
                    _templateRace = Program.Rng.DieRoll(Profile.Instance.MonsterRaces.Count - 2);
                } while ((Profile.Instance.MonsterRaces[_templateRace].Flags1 & MonsterFlag1.Unique) != 0 ||
                         Profile.Instance.MonsterRaces[_templateRace].Level + Program.Rng.DieRoll(5) >
                         SaveGame.Instance.Difficulty + Program.Rng.DieRoll(5));
                if (Program.Rng.DieRoll(2) != 1 && SaveGame.Instance.Difficulty >= 25 + Program.Rng.DieRoll(15))
                {
                    _level.Monsters.GetMonNumHook = VaultAuxSymbol;
                }
                else
                {
                    _level.Monsters.GetMonNumHook = VaultAuxClone;
                }
            }
            else if (tmp < 25)
            {
                _level.Monsters.GetMonNumHook = VaultAuxJelly;
            }
            else if (tmp < 50)
            {
                _level.Monsters.GetMonNumHook = VaultAuxTreasure;
            }
            else if (tmp < 65)
            {
                if (Program.Rng.DieRoll(3) == 1)
                {
                    _level.Monsters.GetMonNumHook = VaultAuxKennel;
                }
                else
                {
                    _level.Monsters.GetMonNumHook = VaultAuxAnimal;
                }
            }
            else
            {
                if (Program.Rng.DieRoll(3) == 1)
                {
                    _level.Monsters.GetMonNumHook = VaultAuxChapel;
                }
                else
                {
                    _level.Monsters.GetMonNumHook = VaultAuxUndead;
                }
            }
            _level.Monsters.GetMonNumPrep();
            for (int i = 0; i < 64; i++)
            {
                what[i] = _level.Monsters.GetMonNum(SaveGame.Instance.Difficulty + 10);
                if (what[i] == 0)
                {
                    empty = true;
                }
            }
            _level.Monsters.GetMonNumHook = null;
            _level.Monsters.GetMonNumPrep();
            if (empty)
            {
                return;
            }
            _level.DangerRating += 10;
            if (SaveGame.Instance.Difficulty <= 40 &&
                Program.Rng.DieRoll((SaveGame.Instance.Difficulty * SaveGame.Instance.Difficulty) + 50) < 300)
            {
                _level.SpecialDanger = true;
            }
            for (y = yval - 2; y <= yval + 2; y++)
            {
                for (x = xval - 9; x <= xval + 9; x++)
                {
                    int rIdx = what[Program.Rng.RandomLessThan(64)];
                    MonsterRace race = Profile.Instance.MonsterRaces[rIdx];
                    _level.Monsters.PlaceMonsterAux(y, x, race, false, false, false);
                }
            }
        }

        public void BuildType6(int yval, int xval)
        {
            int[] what = new int[16];
            int i, y, x;
            bool empty = false;
            GridTile cPtr;
            int y1 = yval - 4;
            int y2 = yval + 4;
            int x1 = xval - 11;
            int x2 = xval + 11;
            for (y = y1 - 1; y <= y2 + 1; y++)
            {
                for (x = x1 - 1; x <= x2 + 1; x++)
                {
                    cPtr = _level.Grid[y][x];
                    cPtr.RevertToBackground();
                    cPtr.TileFlags.Set(GridTile.InRoom);
                }
            }
            for (y = y1 - 1; y <= y2 + 1; y++)
            {
                cPtr = _level.Grid[y][x1 - 1];
                cPtr.SetFeature("WallOuter");
                cPtr = _level.Grid[y][x2 + 1];
                cPtr.SetFeature("WallOuter");
            }
            for (x = x1 - 1; x <= x2 + 1; x++)
            {
                cPtr = _level.Grid[y1 - 1][x];
                cPtr.SetFeature("WallOuter");
                cPtr = _level.Grid[y2 + 1][x];
                cPtr.SetFeature("WallOuter");
            }
            y1 += 2;
            y2 -= 2;
            x1 += 2;
            x2 -= 2;
            for (y = y1 - 1; y <= y2 + 1; y++)
            {
                cPtr = _level.Grid[y][x1 - 1];
                cPtr.SetFeature("WallInner");
                cPtr = _level.Grid[y][x2 + 1];
                cPtr.SetFeature("WallInner");
            }
            for (x = x1 - 1; x <= x2 + 1; x++)
            {
                cPtr = _level.Grid[y1 - 1][x];
                cPtr.SetFeature("WallInner");
                cPtr = _level.Grid[y2 + 1][x];
                cPtr.SetFeature("WallInner");
            }
            switch (Program.Rng.DieRoll(4))
            {
                case 1:
                    PlaceSecretDoor(y1 - 1, xval);
                    break;

                case 2:
                    PlaceSecretDoor(y2 + 1, xval);
                    break;

                case 3:
                    PlaceSecretDoor(yval, x1 - 1);
                    break;

                case 4:
                    PlaceSecretDoor(yval, x2 + 1);
                    break;
            }
            int tmp = Program.Rng.DieRoll(SaveGame.Instance.Difficulty);
            if (tmp < 20)
            {
                _level.Monsters.GetMonNumHook = VaultAuxOrc;
            }
            else if (tmp < 40)
            {
                _level.Monsters.GetMonNumHook = VaultAuxTroll;
            }
            else if (tmp < 55)
            {
                _level.Monsters.GetMonNumHook = VaultAuxGiant;
            }
            else if (tmp < 70)
            {
                if (Program.Rng.DieRoll(4) != 1)
                {
                    do
                    {
                        _templateRace = Program.Rng.DieRoll(Profile.Instance.MonsterRaces.Count - 2);
                    } while ((Profile.Instance.MonsterRaces[_templateRace].Flags1 & MonsterFlag1.Unique) != 0 ||
                             Profile.Instance.MonsterRaces[_templateRace].Level + Program.Rng.DieRoll(5) >
                             SaveGame.Instance.Difficulty + Program.Rng.DieRoll(5));
                    _level.Monsters.GetMonNumHook = VaultAuxSymbol;
                }
                else
                {
                    if (Program.Rng.DieRoll(2) == 1)
                    {
                        _level.Monsters.GetMonNumHook = VaultAuxCult;
                    }
                    else
                    {
                        _level.Monsters.GetMonNumHook = VaultAuxChapel;
                    }
                }
            }
            else if (tmp < 80)
            {
                switch (Program.Rng.RandomLessThan(6))
                {
                    case 0:
                        {
                            _vaultAuxDragonMask4 = MonsterFlag4.BreatheAcid;
                            break;
                        }
                    case 1:
                        {
                            _vaultAuxDragonMask4 = MonsterFlag4.BreatheLightning;
                            break;
                        }
                    case 2:
                        {
                            _vaultAuxDragonMask4 = MonsterFlag4.BreatheFire;
                            break;
                        }
                    case 3:
                        {
                            _vaultAuxDragonMask4 = MonsterFlag4.BreatheCold;
                            break;
                        }
                    case 4:
                        {
                            _vaultAuxDragonMask4 = MonsterFlag4.BreathePoison;
                            break;
                        }
                    default:
                        {
                            _vaultAuxDragonMask4 = MonsterFlag4.BreatheAcid | MonsterFlag4.BreatheLightning |
                                                   MonsterFlag4.BreatheFire | MonsterFlag4.BreatheCold | MonsterFlag4.BreathePoison;
                            break;
                        }
                }
                _level.Monsters.GetMonNumHook = VaultAuxDragon;
            }
            else
            {
                _level.Monsters.GetMonNumHook = VaultAuxDemon;
            }
            _level.Monsters.GetMonNumPrep();
            for (i = 0; i < 16; i++)
            {
                what[i] = _level.Monsters.GetMonNum(SaveGame.Instance.Difficulty + 10);
                if (what[i] == 0)
                {
                    empty = true;
                }
            }
            _level.Monsters.GetMonNumHook = null;
            _level.Monsters.GetMonNumPrep();
            if (empty)
            {
                return;
            }
            for (i = 0; i < 16 - 1; i++)
            {
                for (int j = 0; j < 16 - 1; j++)
                {
                    int i1 = j;
                    int i2 = j + 1;
                    int p1 = Profile.Instance.MonsterRaces[what[i1]].Level;
                    int p2 = Profile.Instance.MonsterRaces[what[i2]].Level;
                    if (p1 > p2)
                    {
                        tmp = what[i1];
                        what[i1] = what[i2];
                        what[i2] = tmp;
                    }
                }
            }
            for (i = 0; i < 8; i++)
            {
                what[i] = what[i * 2];
            }
            _level.DangerRating += 10;
            if (SaveGame.Instance.Difficulty <= 40 &&
                Program.Rng.DieRoll((SaveGame.Instance.Difficulty * SaveGame.Instance.Difficulty) + 50) < 300)
            {
                _level.SpecialDanger = true;
            }
            for (x = xval - 9; x <= xval + 9; x++)
            {
                _level.Monsters.PlaceMonsterByIndex(yval - 2, x, what[0], false, false, false);
                _level.Monsters.PlaceMonsterByIndex(yval + 2, x, what[0], false, false, false);
            }
            for (y = yval - 1; y <= yval + 1; y++)
            {
                _level.Monsters.PlaceMonsterByIndex(y, xval - 9, what[0], false, false, false);
                _level.Monsters.PlaceMonsterByIndex(y, xval + 9, what[0], false, false, false);
                _level.Monsters.PlaceMonsterByIndex(y, xval - 8, what[1], false, false, false);
                _level.Monsters.PlaceMonsterByIndex(y, xval + 8, what[1], false, false, false);
                _level.Monsters.PlaceMonsterByIndex(y, xval - 7, what[1], false, false, false);
                _level.Monsters.PlaceMonsterByIndex(y, xval + 7, what[1], false, false, false);
                _level.Monsters.PlaceMonsterByIndex(y, xval - 6, what[2], false, false, false);
                _level.Monsters.PlaceMonsterByIndex(y, xval + 6, what[2], false, false, false);
                _level.Monsters.PlaceMonsterByIndex(y, xval - 5, what[2], false, false, false);
                _level.Monsters.PlaceMonsterByIndex(y, xval + 5, what[2], false, false, false);
                _level.Monsters.PlaceMonsterByIndex(y, xval - 4, what[3], false, false, false);
                _level.Monsters.PlaceMonsterByIndex(y, xval + 4, what[3], false, false, false);
                _level.Monsters.PlaceMonsterByIndex(y, xval - 3, what[3], false, false, false);
                _level.Monsters.PlaceMonsterByIndex(y, xval + 3, what[3], false, false, false);
                _level.Monsters.PlaceMonsterByIndex(y, xval - 2, what[4], false, false, false);
                _level.Monsters.PlaceMonsterByIndex(y, xval + 2, what[4], false, false, false);
            }
            for (x = xval - 1; x <= xval + 1; x++)
            {
                _level.Monsters.PlaceMonsterByIndex(yval + 1, x, what[5], false, false, false);
                _level.Monsters.PlaceMonsterByIndex(yval - 1, x, what[5], false, false, false);
            }
            _level.Monsters.PlaceMonsterByIndex(yval, xval + 1, what[6], false, false, false);
            _level.Monsters.PlaceMonsterByIndex(yval, xval - 1, what[6], false, false, false);
            _level.Monsters.PlaceMonsterByIndex(yval, xval, what[7], false, false, false);
        }

        public void BuildType7(int yval, int xval)
        {
            VaultType vPtr = Profile.Instance.VaultTypes[0];
            int dummy = 0;
            while (dummy < LevelFactory.SafeMaxAttempts)
            {
                dummy++;
                vPtr = Profile.Instance.VaultTypes[Program.Rng.RandomLessThan(Profile.Instance.VaultTypes.Count)];
                if (vPtr.Category == 7)
                {
                    var minX = xval - (vPtr.Width / 2);
                    var maxX = xval + (vPtr.Width / 2);
                    var minY = yval - (vPtr.Height / 2);
                    var maxY = yval + (vPtr.Height / 2);
                    if (minX >= 1 && minY >= 1 && maxX < _level.CurWid - 1 && maxY < _level.CurHgt - 1)
                    {
                        break;
                    }
                }
            }
            if (dummy >= LevelFactory.SafeMaxAttempts)
            {
                return;
            }
            _level.DangerRating += vPtr.Rating;
            if (SaveGame.Instance.Difficulty <= 50 ||
                Program.Rng.DieRoll(((SaveGame.Instance.Difficulty - 40) * (SaveGame.Instance.Difficulty - 40)) + 50) <
                400)
            {
                _level.SpecialDanger = true;
            }
            BuildVault(yval, xval, vPtr.Height, vPtr.Width, vPtr.Text);
        }

        public void BuildType8(int yval, int xval)
        {
            VaultType vPtr = Profile.Instance.VaultTypes[0];
            int dummy = 0;
            while (dummy < LevelFactory.SafeMaxAttempts)
            {
                dummy++;
                vPtr = Profile.Instance.VaultTypes[Program.Rng.RandomLessThan(Profile.Instance.VaultTypes.Count)];
                if (vPtr.Category == 8)
                {
                    var minX = xval - (vPtr.Width / 2);
                    var maxX = xval + (vPtr.Width / 2);
                    var minY = yval - (vPtr.Height / 2);
                    var maxY = yval + (vPtr.Height / 2);
                    if (minX >= 1 && minY >= 1 && maxX < _level.CurWid - 1 && maxY < _level.CurHgt - 1)
                    {
                        break;
                    }
                }
            }
            if (dummy >= LevelFactory.SafeMaxAttempts)
            {
                return;
            }
            _level.DangerRating += vPtr.Rating;
            if (SaveGame.Instance.Difficulty <= 50 ||
                Program.Rng.DieRoll(((SaveGame.Instance.Difficulty - 40) * (SaveGame.Instance.Difficulty - 40)) + 50) <
                400)
            {
                _level.SpecialDanger = true;
            }
            BuildVault(yval, xval, vPtr.Height, vPtr.Width, vPtr.Text);
        }

        private void BuildVault(int yval, int xval, int ymax, int xmax, string data)
        {
            int dx, dy, x, y;
            char t;
            int index = 0;
            for (dy = 0; dy < ymax; dy++)
            {
                for (dx = 0; dx < xmax; dx++)
                {
                    t = data[index];
                    index++;
                    x = xval - (xmax / 2) + dx;
                    y = yval - (ymax / 2) + dy;
                    if (t == ' ')
                    {
                        continue;
                    }
                    GridTile cPtr = _level.Grid[y][x];
                    cPtr.RevertToBackground();
                    cPtr.TileFlags.Set(GridTile.InRoom | GridTile.InVault);
                    switch (t)
                    {
                        case '%':
                            cPtr.SetFeature("WallOuter");
                            break;

                        case '#':
                            cPtr.SetFeature("WallInner");
                            break;

                        case 'X':
                            cPtr.SetFeature("WallPermInner");
                            break;

                        case '*':
                            if (Program.Rng.RandomLessThan(100) < 75)
                            {
                                _level.PlaceObject(y, x, false, false);
                            }
                            else
                            {
                                _level.PlaceTrap(y, x);
                            }
                            break;

                        case '+':
                            PlaceSecretDoor(y, x);
                            break;

                        case '^':
                            _level.PlaceTrap(y, x);
                            break;
                    }
                }
            }
            index = 0;
            for (dy = 0; dy < ymax; dy++)
            {
                for (dx = 0; dx < xmax; dx++)
                {
                    t = data[index];
                    index++;
                    x = xval - (xmax / 2) + dx;
                    y = yval - (ymax / 2) + dy;
                    if (t == ' ')
                    {
                        continue;
                    }
                    switch (t)
                    {
                        case '&':
                            {
                                _level.MonsterLevel = SaveGame.Instance.Difficulty + 5;
                                _level.Monsters.PlaceMonster(y, x, true, true);
                                _level.MonsterLevel = SaveGame.Instance.Difficulty;
                                break;
                            }
                        case '@':
                            {
                                _level.MonsterLevel = SaveGame.Instance.Difficulty + 11;
                                _level.Monsters.PlaceMonster(y, x, true, true);
                                _level.MonsterLevel = SaveGame.Instance.Difficulty;
                                break;
                            }
                        case '9':
                            {
                                _level.MonsterLevel = SaveGame.Instance.Difficulty + 9;
                                _level.Monsters.PlaceMonster(y, x, true, true);
                                _level.MonsterLevel = SaveGame.Instance.Difficulty;
                                _level.ObjectLevel = SaveGame.Instance.Difficulty + 7;
                                _level.PlaceObject(y, x, true, false);
                                _level.ObjectLevel = SaveGame.Instance.Difficulty;
                                break;
                            }
                        case '8':
                            {
                                _level.MonsterLevel = SaveGame.Instance.Difficulty + 40;
                                _level.Monsters.PlaceMonster(y, x, true, true);
                                _level.MonsterLevel = SaveGame.Instance.Difficulty;
                                _level.ObjectLevel = SaveGame.Instance.Difficulty + 20;
                                _level.PlaceObject(y, x, true, true);
                                _level.ObjectLevel = SaveGame.Instance.Difficulty;
                                break;
                            }
                        case ',':
                            {
                                if (Program.Rng.RandomLessThan(100) < 50)
                                {
                                    _level.MonsterLevel = SaveGame.Instance.Difficulty + 3;
                                    _level.Monsters.PlaceMonster(y, x, true, true);
                                    _level.MonsterLevel = SaveGame.Instance.Difficulty;
                                }
                                if (Program.Rng.RandomLessThan(100) < 50)
                                {
                                    _level.ObjectLevel = SaveGame.Instance.Difficulty + 7;
                                    _level.PlaceObject(y, x, false, false);
                                    _level.ObjectLevel = SaveGame.Instance.Difficulty;
                                }
                                break;
                            }
                        case 'A':
                            {
                                _level.ObjectLevel = SaveGame.Instance.Difficulty + 12;
                                _level.PlaceObject(y, x, true, false);
                                _level.ObjectLevel = SaveGame.Instance.Difficulty;
                            }
                            break;
                    }
                }
            }
        }

        private void PlaceDownStairs(int y, int x)
        {
            GridTile cPtr = _level.Grid[y][x];
            cPtr.SetFeature("DownStair");
        }

        private void PlaceLockedDoor(int y, int x)
        {
            GridTile cPtr = _level.Grid[y][x];
            cPtr.SetFeature($"LockedDoor{Program.Rng.DieRoll(7)}");
        }

        private void PlaceRandomStairs(int y, int x)
        {
            if (!_level.GridOpenNoItem(y, x))
            {
                return;
            }
            if (SaveGame.Instance.DunLevel <= 0)
            {
            }
            if (SaveGame.Instance.Quests.IsQuest(SaveGame.Instance.DunLevel) ||
                SaveGame.Instance.DunLevel == SaveGame.Instance.CurDungeon.MaxLevel)
            {
                if (SaveGame.Instance.CurDungeon.Tower)
                {
                    PlaceDownStairs(y, x);
                }
                else
                {
                    PlaceUpStairs(y, x);
                }
            }
            else if (Program.Rng.RandomLessThan(100) < 50)
            {
                PlaceDownStairs(y, x);
            }
            else
            {
                PlaceUpStairs(y, x);
            }
        }

        private void PlaceSecretDoor(int y, int x)
        {
            GridTile cPtr = _level.Grid[y][x];
            cPtr.SetFeature("SecretDoor");
        }

        private void PlaceUpStairs(int y, int x)
        {
            GridTile cPtr = _level.Grid[y][x];
            cPtr.SetFeature("UpStair");
        }

        private bool VaultAuxAnimal(int rIdx)
        {
            MonsterRace rPtr = Profile.Instance.MonsterRaces[rIdx];
            if ((rPtr.Flags1 & MonsterFlag1.Unique) != 0)
            {
                return false;
            }
            if ((rPtr.Flags3 & MonsterFlag3.Animal) == 0)
            {
                return false;
            }
            return true;
        }

        private bool VaultAuxChapel(int rIdx)
        {
            MonsterRace rPtr = Profile.Instance.MonsterRaces[rIdx];
            if ((rPtr.Flags1 & MonsterFlag1.Unique) != 0)
            {
                return false;
            }
            if (!rPtr.Name.Contains("haman"))
            {
                return false;
            }
            return true;
        }

        private bool VaultAuxClone(int rIdx)
        {
            return rIdx == _templateRace;
        }

        private bool VaultAuxCult(int rIdx)
        {
            MonsterRace rPtr = Profile.Instance.MonsterRaces[rIdx];
            if ((rPtr.Flags1 & MonsterFlag1.Unique) != 0)
            {
                return false;
            }
            if (!rPtr.Name.Contains("Cult"))
            {
                return false;
            }
            return true;
        }

        private bool VaultAuxDemon(int rIdx)
        {
            MonsterRace rPtr = Profile.Instance.MonsterRaces[rIdx];
            if ((rPtr.Flags1 & MonsterFlag1.Unique) != 0)
            {
                return false;
            }
            if (rPtr.Character != 'U')
            {
                return false;
            }
            return true;
        }

        private bool VaultAuxDragon(int rIdx)
        {
            MonsterRace rPtr = Profile.Instance.MonsterRaces[rIdx];
            if ((rPtr.Flags1 & MonsterFlag1.Unique) != 0)
            {
                return false;
            }
            if (!"Dd".Contains(rPtr.Character.ToString()))
            {
                return false;
            }
            if (rPtr.Flags4 != _vaultAuxDragonMask4)
            {
                return false;
            }
            return true;
        }

        private bool VaultAuxGiant(int rIdx)
        {
            MonsterRace rPtr = Profile.Instance.MonsterRaces[rIdx];
            if ((rPtr.Flags1 & MonsterFlag1.Unique) != 0)
            {
                return false;
            }
            if (rPtr.Character != 'P')
            {
                return false;
            }
            return true;
        }

        private bool VaultAuxJelly(int rIdx)
        {
            MonsterRace rPtr = Profile.Instance.MonsterRaces[rIdx];
            if ((rPtr.Flags1 & MonsterFlag1.Unique) != 0)
            {
                return false;
            }
            if ((rPtr.Flags3 & MonsterFlag3.Evil) != 0)
            {
                return false;
            }
            if (!"ijm,".Contains(rPtr.Character.ToString()))
            {
                return false;
            }
            return true;
        }

        private bool VaultAuxKennel(int rIdx)
        {
            MonsterRace rPtr = Profile.Instance.MonsterRaces[rIdx];
            if ((rPtr.Flags1 & MonsterFlag1.Unique) != 0)
            {
                return false;
            }
            return rPtr.Character == 'Z' || rPtr.Character == 'C';
        }

        private bool VaultAuxOrc(int rIdx)
        {
            MonsterRace rPtr = Profile.Instance.MonsterRaces[rIdx];
            if ((rPtr.Flags1 & MonsterFlag1.Unique) != 0)
            {
                return false;
            }
            if (rPtr.Character != 'o')
            {
                return false;
            }
            return true;
        }

        private bool VaultAuxSymbol(int rIdx)
        {
            return Profile.Instance.MonsterRaces[rIdx].Character == Profile.Instance.MonsterRaces[_templateRace].Character &&
                   (Profile.Instance.MonsterRaces[rIdx].Flags1 & MonsterFlag1.Unique) == 0;
        }

        private bool VaultAuxTreasure(int rIdx)
        {
            MonsterRace rPtr = Profile.Instance.MonsterRaces[rIdx];
            if ((rPtr.Flags1 & MonsterFlag1.Unique) != 0)
            {
                return false;
            }
            if (!(rPtr.Character == '!' || rPtr.Character == '|' || rPtr.Character == '$' || rPtr.Character == '?' ||
                  rPtr.Character == '='))
            {
                return false;
            }
            return true;
        }

        private bool VaultAuxTroll(int rIdx)
        {
            MonsterRace rPtr = Profile.Instance.MonsterRaces[rIdx];
            if ((rPtr.Flags1 & MonsterFlag1.Unique) != 0)
            {
                return false;
            }
            if (rPtr.Character != 'T')
            {
                return false;
            }
            return true;
        }

        private bool VaultAuxUndead(int rIdx)
        {
            MonsterRace rPtr = Profile.Instance.MonsterRaces[rIdx];
            if ((rPtr.Flags1 & MonsterFlag1.Unique) != 0)
            {
                return false;
            }
            if ((rPtr.Flags3 & MonsterFlag3.Undead) == 0)
            {
                return false;
            }
            return true;
        }

        private void VaultMonsters(int y1, int x1, int num)
        {
            for (int k = 0; k < num; k++)
            {
                for (int i = 0; i < 9; i++)
                {
                    const int d = 1;
                    _level.Scatter(out int y, out int x, y1, x1, d);
                    if (!_level.GridPassableNoCreature(y, x))
                    {
                        continue;
                    }
                    _level.MonsterLevel = SaveGame.Instance.Difficulty + 2;
                    _level.Monsters.PlaceMonster(y, x, true, true);
                    _level.MonsterLevel = SaveGame.Instance.Difficulty;
                }
            }
        }

        private void VaultObjects(int y, int x, int num)
        {
            int dummy = 0;
            int j = y, k = x;
            for (; num > 0; --num)
            {
                int i;
                for (i = 0; i < 11; ++i)
                {
                    while (dummy < LevelFactory.SafeMaxAttempts)
                    {
                        j = Program.Rng.RandomSpread(y, 2);
                        k = Program.Rng.RandomSpread(x, 3);
                        dummy++;
                        if (!_level.InBounds(j, k))
                        {
                            continue;
                        }
                        break;
                    }
                    if (!_level.GridOpenNoItem(j, k))
                    {
                        continue;
                    }
                    if (Program.Rng.RandomLessThan(100) < 75)
                    {
                        _level.PlaceObject(j, k, false, false);
                    }
                    else
                    {
                        _level.PlaceGold(j, k);
                    }
                    break;
                }
            }
        }

        private void VaultTrapAux(int y, int x, int yd, int xd)
        {
            int count, y1 = y, x1 = x;
            int dummy = 0;
            for (count = 0; count <= 5; count++)
            {
                while (dummy < LevelFactory.SafeMaxAttempts)
                {
                    y1 = Program.Rng.RandomSpread(y, yd);
                    x1 = Program.Rng.RandomSpread(x, xd);
                    dummy++;
                    if (!_level.InBounds(y1, x1))
                    {
                        continue;
                    }
                    break;
                }
                if (!_level.GridOpenNoItemOrCreature(y1, x1))
                {
                    continue;
                }
                _level.PlaceTrap(y1, x1);
                break;
            }
        }

        private void VaultTraps(int y, int x, int yd, int xd, int num)
        {
            for (int i = 0; i < num; i++)
            {
                VaultTrapAux(y, x, yd, xd);
            }
        }
    }
}