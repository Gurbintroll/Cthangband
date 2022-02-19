using Cthangband.Enumerations;
using Cthangband.StaticData;
using Cthangband.UI;
using System;
using System.Collections.Generic;

namespace Cthangband
{
    [Serializable]
    internal class Level
    {
        public const int MaxHgt = 126;
        public const int MaxWid = 198;
        public readonly GridTile[][] Grid = new GridTile[MaxHgt][];
        public readonly Item[] Items = new Item[Constants.MaxOIdx];
        public readonly int[] KeypadDirectionXOffset = { 0, -1, 0, 1, -1, 0, 1, -1, 0, 1 };
        public readonly int[] KeypadDirectionYOffset = { 0, 1, 1, 1, 0, 0, 0, -1, -1, -1 };
        public readonly MonsterList Monsters;
        public readonly int[] OrderedDirection = { 2, 8, 6, 4, 3, 1, 9, 7, 5 };
        public readonly int[] OrderedDirectionXOffset = { 0, 0, 1, -1, 1, -1, 1, -1, 0 };
        public readonly int[] OrderedDirectionYOffset = { 1, -1, 0, 0, 1, 1, -1, -1, 0 };
        public readonly int[] TempX = new int[Constants.TempMax];
        public readonly int[] TempY = new int[Constants.TempMax];
        public int CurHgt;
        public int CurWid;
        public int DangerFeeling;
        public int DangerRating;
        public int MaxPanelCols;
        public int MaxPanelRows;
        public int MCnt;
        public int MMax = 1;
        public int MonsterLevel;
        public int ObjectLevel;
        public int OCnt;
        public int OMax = 1;
        public bool OpeningChest;
        public int PanelCol;
        public int PanelColMax;
        public int PanelColMin;
        public int PanelColPrt;
        public int PanelRow;
        public int PanelRowMax;
        public int PanelRowMin;
        public int PanelRowPrt;
        public bool SpecialDanger;
        public bool SpecialTreasure;
        public int TempN;
        public int TreasureFeeling;
        public int TreasureRating;
        private const string _imageMonsterHack = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string _imageObjectHack = "?/|\\\"!$()_-=[]{},~";
        private const int _mapHgt = MaxHgt / _ratio;
        private const int _mapWid = MaxWid / _ratio;
        private const int _ratio = 3;
        private readonly int[] _lightX = new int[Constants.LightMax];
        private readonly int[] _lightY = new int[Constants.LightMax];
        private readonly Player _player;

        private readonly int[] _viewX = new int[Constants.ViewMax];
        private readonly int[] _viewY = new int[Constants.ViewMax];
        private int _flowHead;
        private int _flowN;
        private int _flowTail;
        private int _lightN;
        private int _viewN;

        public Level()
        {
            // Get a local reference to the player for efficiency
            _player = SaveGame.Instance.Player;
            for (int i = 0; i < MaxHgt; i++)
            {
                Grid[i] = new GridTile[MaxWid];
                for (int j = 0; j < MaxWid; j++)
                {
                    Grid[i][j] = new GridTile();
                }
            }
            for (int j = 0; j < Constants.MaxOIdx; j++)
            {
                Items[j] = new Item();
            }
            Monsters = new MonsterList(this);
        }

        public void Acquirement(int y1, int x1, int num, bool great)
        {
            while (num-- != 0)
            {
                Item qPtr = new Item();
                if (!qPtr.MakeObject(true, great))
                {
                    continue;
                }
                DropNear(qPtr, -1, y1, x1);
            }
        }

        public void CaveRemoveFeat(int y, int x)
        {
            GridTile cPtr = Grid[y][x];
            cPtr.RevertToBackground();
            NoteSpot(y, x);
            RedrawSingleLocation(y, x);
        }

        public void CaveSetBackground(int y, int x, string feat)
        {
            GridTile cPtr = Grid[y][x];
            cPtr.BackgroundFeature = StaticResources.Instance.FloorTileTypes[feat];
        }

        public void CaveSetFeat(int y, int x, string feat)
        {
            GridTile cPtr = Grid[y][x];
            cPtr.FeatureType = StaticResources.Instance.FloorTileTypes[feat];
            NoteSpot(y, x);
            RedrawSingleLocation(y, x);
        }

        public bool CaveValidBold(int y, int x)
        {
            GridTile cPtr = Grid[y][x];
            int nextOIdx;
            if (cPtr.FeatureType.IsPermanent)
            {
                return false;
            }
            for (int thisOIdx = cPtr.Item; thisOIdx != 0; thisOIdx = nextOIdx)
            {
                Item oPtr = Items[thisOIdx];
                nextOIdx = oPtr.NextInStack;
                if (!string.IsNullOrEmpty(oPtr.RandartName) || oPtr.IsFixedArtifact())
                {
                    return false;
                }
            }
            return true;
        }

        public int ChestCheck(int y, int x)
        {
            GridTile cPtr = Grid[y][x];
            int nextOIdx;
            for (int thisOIdx = cPtr.Item; thisOIdx != 0; thisOIdx = nextOIdx)
            {
                Item oPtr = Items[thisOIdx];
                nextOIdx = oPtr.NextInStack;
                if (oPtr.Category == ItemCategory.Chest)
                {
                    return thisOIdx;
                }
            }
            return 0;
        }

        public void CompactObjects(int size)
        {
            int i;
            int num, cnt;
            if (size != 0)
            {
                Profile.Instance.MsgPrint("Compacting objects...");
                SaveGame.Instance.Player.RedrawNeeded.Set(RedrawFlag.PrMap);
            }
            for (num = 0, cnt = 1; num < size; cnt++)
            {
                int curLev = 5 * cnt;
                int curDis = 5 * (20 - cnt);
                for (i = 1; i < OMax; i++)
                {
                    Item oPtr = Items[i];
                    ItemType kPtr = oPtr.ItemType;
                    if (oPtr.ItemType == null)
                    {
                        continue;
                    }
                    if (kPtr.Level > curLev)
                    {
                        continue;
                    }
                    int y;
                    int x;
                    if (oPtr.HoldingMonsterIndex != 0)
                    {
                        Monster mPtr = Monsters[oPtr.HoldingMonsterIndex];
                        y = mPtr.MapY;
                        x = mPtr.MapX;
                        if (Program.Rng.RandomLessThan(100) < 90)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        y = oPtr.Y;
                        x = oPtr.X;
                    }
                    if (curDis > 0 && Distance(SaveGame.Instance.Player.MapY, SaveGame.Instance.Player.MapX, y, x) < curDis)
                    {
                        continue;
                    }
                    int chance = 90;
                    if ((oPtr.IsFixedArtifact() || !string.IsNullOrEmpty(oPtr.RandartName)) && cnt < 1000)
                    {
                        chance = 100;
                    }
                    if (Program.Rng.RandomLessThan(100) < chance)
                    {
                        continue;
                    }
                    DeleteObjectIdx(i);
                    num++;
                }
            }
            for (i = OMax - 1; i >= 1; i--)
            {
                Item oPtr = Items[i];
                if (oPtr.ItemType != null)
                {
                    continue;
                }
                CompactObjectsAux(OMax - 1, i);
                OMax--;
            }
        }

        public int CoordsToDir(int y, int x)
        {
            int[][] d = { new[] { 7, 4, 1 }, new[] { 8, 5, 2 }, new[] { 9, 6, 3 } };
            int dy = y - _player.MapY;
            int dx = x - _player.MapX;
            if (Math.Abs(dx) > 1 || Math.Abs(dy) > 1)
            {
                return 0;
            }
            return d[dx + 1][dy + 1];
        }

        public void DeleteMonster(int y, int x)
        {
            if (!InBounds(y, x))
            {
                return;
            }
            GridTile cPtr = Grid[y][x];
            if (cPtr.Monster != 0)
            {
                Monsters.DeleteMonsterByIndex(cPtr.Monster, true);
            }
        }

        public void DeleteObject(int y, int x)
        {
            int nextOIdx;
            if (!InBounds(y, x))
            {
                return;
            }
            GridTile cPtr = Grid[y][x];
            for (int thisOIdx = cPtr.Item; thisOIdx != 0; thisOIdx = nextOIdx)
            {
                Item oPtr = Items[thisOIdx];
                nextOIdx = oPtr.NextInStack;
                Items[thisOIdx] = new Item();
                OCnt--;
            }
            cPtr.Item = 0;
            RedrawSingleLocation(y, x);
        }

        public void DeleteObjectIdx(int oIdx)
        {
            ExciseObjectIdx(oIdx);
            Item jPtr = Items[oIdx];
            if (jPtr.HoldingMonsterIndex == 0)
            {
                int y = jPtr.Y;
                int x = jPtr.X;
                RedrawSingleLocation(y, x);
            }
            Items[oIdx] = new Item();
            OCnt--;
        }

        public void DisplayMap(out int cy, out int cx)
        {
            int x, y, maxy;
            Colour ta;
            char tc;
            Colour[][] ma = new Colour[_mapHgt + 2][];
            for (int i = 0; i < _mapHgt + 2; i++)
            {
                ma[i] = new Colour[_mapWid + 2];
            }
            char[][] mc = new char[_mapHgt + 2][];
            for (int i = 0; i < _mapHgt + 2; i++)
            {
                mc[i] = new char[_mapWid + 2];
            }
            int[][] mp = new int[_mapHgt + 2][];
            for (int i = 0; i < _mapHgt + 2; i++)
            {
                mp[i] = new int[_mapWid + 2];
            }
            for (y = 0; y < _mapHgt + 2; ++y)
            {
                for (x = 0; x < _mapWid + 2; ++x)
                {
                    ma[y][x] = Colour.White;
                    mc[y][x] = ' ';
                    mp[y][x] = 0;
                }
            }
            int maxx = maxy = 0;
            for (int i = 0; i < CurWid; ++i)
            {
                for (int j = 0; j < CurHgt; ++j)
                {
                    x = (i / _ratio) + 1;
                    y = (j / _ratio) + 1;
                    if (x > maxx)
                    {
                        maxx = x;
                    }
                    if (y > maxy)
                    {
                        maxy = y;
                    }
                    MapInfo(j, i, out ta, out tc);
                    int tp = Grid[j][i].FeatureType.MapPriority;
                    if (ta == Colour.Background)
                    {
                        tp = 0;
                    }
                    if (mp[y][x] < tp)
                    {
                        mc[y][x] = tc;
                        ma[y][x] = ta;
                        mp[y][x] = tp;
                    }
                }
            }
            x = maxx + 1;
            y = maxy + 1;
            int xOffset = (80 - x) / 2;
            int yOffset = (44 - y) / 2;
            mc[0][0] = '+';
            ma[0][0] = Colour.Purple;
            mc[0][x] = '+';
            ma[0][x] = Colour.Purple;
            mc[y][0] = '+';
            ma[y][0] = Colour.Purple;
            mc[y][x] = '+';
            ma[y][x] = Colour.Purple;
            for (x = 1; x <= maxx; x++)
            {
                mc[0][x] = '-';
                ma[0][x] = Colour.Purple;
                mc[maxy + 1][x] = '-';
                ma[maxy + 1][x] = Colour.Purple;
            }
            for (y = 1; y <= maxy; y++)
            {
                mc[y][0] = '|';
                ma[y][0] = Colour.Purple;
                mc[y][maxx + 1] = '|';
                ma[y][maxx + 1] = Colour.Purple;
            }
            for (y = 0; y < maxy + 2; ++y)
            {
                Gui.Goto(yOffset + y, xOffset);
                for (x = 0; x < maxx + 2; ++x)
                {
                    ta = ma[y][x];
                    tc = mc[y][x];
                    if (_player.TimedInvulnerability != 0)
                    {
                        ta = Colour.White;
                    }
                    else if (_player.TimedEtherealness != 0)
                    {
                        ta = Colour.Black;
                    }
                    Gui.Print(ta, tc);
                }
            }
            cy = yOffset + (_player.MapY / _ratio) + 1;
            cx = xOffset + (_player.MapX / _ratio) + 1;
        }

        public int Distance(int y1, int x1, int y2, int x2)
        {
            int dy = y1 > y2 ? y1 - y2 : y2 - y1;
            int dx = x1 > x2 ? x1 - x2 : x2 - x1;
            int d = dy > dx ? dy + (dx >> 1) : dx + (dy >> 1);
            return d;
        }

        public void DropNear(Item jPtr, int chance, int y, int x)
        {
            int ty, tx;
            int thisOIdx, nextOIdx;
            GridTile cPtr;
            bool flag = false;
            bool done = false;
            bool plural = jPtr.Count != 1;
            string oName = jPtr.Description(false, 0);
            if (!(!string.IsNullOrEmpty(jPtr.RandartName) || jPtr.IsFixedArtifact()) &&
                Program.Rng.RandomLessThan(100) < chance)
            {
                string p = plural ? "" : "s";
                Profile.Instance.MsgPrint($"The {oName} disappear{p}.");
                return;
            }
            int bs = -1;
            int bn = 0;
            int by = y;
            int bx = x;
            for (int dy = -3; dy <= 3; dy++)
            {
                for (int dx = -3; dx <= 3; dx++)
                {
                    bool comb = false;
                    int d = (dy * dy) + (dx * dx);
                    if (d > 10)
                    {
                        continue;
                    }
                    ty = y + dy;
                    tx = x + dx;
                    if (!InBounds(ty, tx))
                    {
                        continue;
                    }
                    if (!Los(y, x, ty, tx))
                    {
                        continue;
                    }
                    cPtr = Grid[ty][tx];
                    if (!cPtr.FeatureType.IsOpenFloor)
                    {
                        continue;
                    }
                    int k = 0;
                    for (thisOIdx = cPtr.Item; thisOIdx != 0; thisOIdx = nextOIdx)
                    {
                        Item oPtr = Items[thisOIdx];
                        nextOIdx = oPtr.NextInStack;
                        if (oPtr.CanAbsorb(jPtr))
                        {
                            comb = true;
                        }
                        k++;
                    }
                    if (!comb)
                    {
                        k++;
                    }
                    if (k > 99)
                    {
                        continue;
                    }
                    int s = 1000 - (d + (k * 5));
                    if (s < bs)
                    {
                        continue;
                    }
                    if (s > bs)
                    {
                        bn = 0;
                    }
                    if (++bn >= 2 && Program.Rng.RandomLessThan(bn) != 0)
                    {
                        continue;
                    }
                    bs = s;
                    by = ty;
                    bx = tx;
                    flag = true;
                }
            }
            if (!flag && !(jPtr.IsFixedArtifact() || !string.IsNullOrEmpty(jPtr.RandartName)))
            {
                string p = plural ? "" : "s";
                Profile.Instance.MsgPrint($"The {oName} disappear{p}.");
                return;
            }
            for (int i = 0; !flag; i++)
            {
                if (i < 1000)
                {
                    ty = Program.Rng.RandomSpread(by, 1);
                    tx = Program.Rng.RandomSpread(bx, 1);
                }
                else
                {
                    ty = Program.Rng.RandomLessThan(CurHgt);
                    tx = Program.Rng.RandomLessThan(CurWid);
                }
                cPtr = Grid[ty][tx];
                if (!cPtr.FeatureType.IsOpenFloor)
                {
                    continue;
                }
                by = ty;
                bx = tx;
                if (!GridOpenNoItem(by, bx))
                {
                    continue;
                }
                flag = true;
            }
            cPtr = Grid[by][bx];
            for (thisOIdx = cPtr.Item; thisOIdx != 0; thisOIdx = nextOIdx)
            {
                Item oPtr = Items[thisOIdx];
                nextOIdx = oPtr.NextInStack;
                if (oPtr.CanAbsorb(jPtr))
                {
                    oPtr.Absorb(jPtr);
                    done = true;
                    break;
                }
            }
            int oIdx = OPop();
            if (!done && oIdx == 0)
            {
                string p = plural ? "" : "s";
                Profile.Instance.MsgPrint($"The {oName} disappear{p}.");
                if (jPtr.FixedArtifactIndex != 0)
                {
                    Profile.Instance.FixedArtifacts[jPtr.FixedArtifactIndex].CurNum = 0;
                }
                return;
            }
            if (!done)
            {
                Items[oIdx] = new Item(jPtr);
                jPtr = Items[oIdx];
                jPtr.Y = by;
                jPtr.X = bx;
                jPtr.HoldingMonsterIndex = 0;
                jPtr.NextInStack = cPtr.Item;
                cPtr.Item = oIdx;
            }
            NoteSpot(by, bx);
            RedrawSingleLocation(by, bx);
            Gui.PlaySound(SoundEffect.Drop);
            if (chance != 0 && by == SaveGame.Instance.Player.MapY && bx == SaveGame.Instance.Player.MapX)
            {
                Profile.Instance.MsgPrint("You feel something roll beneath your feet.");
            }
        }

        public void ExciseObjectIdx(int oIdx)
        {
            int thisOIdx, nextOIdx;
            int prevOIdx = 0;
            Item jPtr = Items[oIdx];
            if (jPtr.HoldingMonsterIndex != 0)
            {
                Monster mPtr = Monsters[jPtr.HoldingMonsterIndex];
                for (thisOIdx = mPtr.FirstHeldItemIndex; thisOIdx != 0; thisOIdx = nextOIdx)
                {
                    Item oPtr = Items[thisOIdx];
                    nextOIdx = oPtr.NextInStack;
                    if (thisOIdx == oIdx)
                    {
                        if (prevOIdx == 0)
                        {
                            mPtr.FirstHeldItemIndex = nextOIdx;
                        }
                        else
                        {
                            Item kPtr = Items[prevOIdx];
                            kPtr.NextInStack = nextOIdx;
                        }
                        oPtr.NextInStack = 0;
                        break;
                    }
                    prevOIdx = thisOIdx;
                }
            }
            else
            {
                int y = jPtr.Y;
                int x = jPtr.X;
                GridTile cPtr = Grid[y][x];
                for (thisOIdx = cPtr.Item; thisOIdx != 0; thisOIdx = nextOIdx)
                {
                    Item oPtr = Items[thisOIdx];
                    nextOIdx = oPtr.NextInStack;
                    if (thisOIdx == oIdx)
                    {
                        if (prevOIdx == 0)
                        {
                            cPtr.Item = nextOIdx;
                        }
                        else
                        {
                            Item kPtr = Items[prevOIdx];
                            kPtr.NextInStack = nextOIdx;
                        }
                        oPtr.NextInStack = 0;
                        break;
                    }
                    prevOIdx = thisOIdx;
                }
            }
        }

        public void FloorItemDescribe(int item)
        {
            Item oPtr = Items[item];
            string oName = oPtr.Description(true, 3);
            Profile.Instance.MsgPrint($"You see {oName}.");
        }

        public void FloorItemIncrease(int item, int num)
        {
            Item oPtr = Items[item];
            num += oPtr.Count;
            if (num > 255)
            {
                num = 255;
            }
            else if (num < 0)
            {
                num = 0;
            }
            num -= oPtr.Count;
            oPtr.Count += num;
        }

        public void FloorItemOptimize(int item)
        {
            Item oPtr = Items[item];
            if (oPtr.ItemType == null)
            {
                return;
            }
            if (oPtr.Count != 0)
            {
                return;
            }
            DeleteObjectIdx(item);
        }

        public void ForgetLight()
        {
            if (_lightN == 0)
            {
                return;
            }
            for (int i = 0; i < _lightN; i++)
            {
                int y = _lightY[i];
                int x = _lightX[i];
                Grid[y][x].TileFlags.Clear(GridTile.PlayerLit);
                RedrawSingleLocation(y, x);
            }
            _lightN = 0;
        }

        public void ForgetView()
        {
            if (_viewN == 0)
            {
                return;
            }
            for (int i = 0; i < _viewN; i++)
            {
                int y = _viewY[i];
                int x = _viewX[i];
                GridTile cPtr = Grid[y][x];
                cPtr.TileFlags.Clear(GridTile.IsVisible);
                RedrawSingleLocation(y, x);
            }
            _viewN = 0;
        }

        public bool GridOpenNoItem(int y, int x)
        {
            return Grid[y][x].FeatureType.IsOpenFloor && Grid[y][x].Item == 0;
        }

        public bool GridOpenNoItemOrCreature(int y, int x)
        {
            return Grid[y][x].FeatureType.IsOpenFloor && Grid[y][x].Item == 0 && Grid[y][x].Monster == 0 &&
                   !(y == _player.MapY && x == _player.MapX);
        }

        public bool GridPassable(int y, int x)
        {
            return Grid[y][x].FeatureType.IsPassable;
        }

        public bool GridPassableNoCreature(int y, int x)
        {
            return GridPassable(y, x) && Grid[y][x].Monster == 0 && !(y == _player.MapY && x == _player.MapX);
        }

        public bool InBounds(int y, int x)
        {
            return y > 0 && x > 0 && y < CurHgt - 1 && x < CurWid - 1;
        }

        public bool InBounds2(int y, int x)
        {
            return y >= 0 && x >= 0 && y < CurHgt && x < CurWid;
        }

        public bool Los(int y1, int x1, int y2, int x2)
        {
            int tx, ty;
            int m;
            int dy = y2 - y1;
            int dx = x2 - x1;
            int ay = Math.Abs(dy);
            int ax = Math.Abs(dx);
            if (ax < 2 && ay < 2)
            {
                return true;
            }
            if (dx == 0)
            {
                if (dy > 0)
                {
                    for (ty = y1 + 1; ty < y2; ty++)
                    {
                        if (GridBlocksLos(ty, x1))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    for (ty = y1 - 1; ty > y2; ty--)
                    {
                        if (GridBlocksLos(ty, x1))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            if (dy == 0)
            {
                if (dx > 0)
                {
                    for (tx = x1 + 1; tx < x2; tx++)
                    {
                        if (GridBlocksLos(y1, tx))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    for (tx = x1 - 1; tx > x2; tx--)
                    {
                        if (GridBlocksLos(y1, tx))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            int sx = dx < 0 ? -1 : 1;
            int sy = dy < 0 ? -1 : 1;
            if (ax == 1)
            {
                if (ay == 2)
                {
                    if (!GridBlocksLos(y1 + sy, x1))
                    {
                        return true;
                    }
                }
            }
            else if (ay == 1)
            {
                if (ax == 2)
                {
                    if (!GridBlocksLos(y1, x1 + sx))
                    {
                        return true;
                    }
                }
            }
            int f2 = ax * ay;
            int f1 = f2 << 1;
            if (ax >= ay)
            {
                int qy = ay * ay;
                m = qy << 1;
                tx = x1 + sx;
                if (qy == f2)
                {
                    ty = y1 + sy;
                    qy -= f1;
                }
                else
                {
                    ty = y1;
                }
                while (x2 - tx != 0)
                {
                    if (GridBlocksLos(ty, tx))
                    {
                        return false;
                    }
                    qy += m;
                    if (qy < f2)
                    {
                        tx += sx;
                    }
                    else if (qy > f2)
                    {
                        ty += sy;
                        if (GridBlocksLos(ty, tx))
                        {
                            return false;
                        }
                        qy -= f1;
                        tx += sx;
                    }
                    else
                    {
                        ty += sy;
                        qy -= f1;
                        tx += sx;
                    }
                }
            }
            else
            {
                int qx = ax * ax;
                m = qx << 1;
                ty = y1 + sy;
                if (qx == f2)
                {
                    tx = x1 + sx;
                    qx -= f1;
                }
                else
                {
                    tx = x1;
                }
                while (y2 - ty != 0)
                {
                    if (GridBlocksLos(ty, tx))
                    {
                        return false;
                    }
                    qx += m;
                    if (qx < f2)
                    {
                        ty += sy;
                    }
                    else if (qx > f2)
                    {
                        tx += sx;
                        if (GridBlocksLos(ty, tx))
                        {
                            return false;
                        }
                        qx -= f1;
                        ty += sy;
                    }
                    else
                    {
                        tx += sx;
                        qx -= f1;
                        ty += sy;
                    }
                }
            }
            return true;
        }

        public void MapArea()
        {
            int y1 = PanelRowMin - Program.Rng.DieRoll(10);
            int y2 = PanelRowMax + Program.Rng.DieRoll(10);
            int x1 = PanelColMin - Program.Rng.DieRoll(20);
            int x2 = PanelColMax + Program.Rng.DieRoll(20);
            if (y1 < 1)
            {
                y1 = 1;
            }
            if (y2 > CurHgt - 2)
            {
                y2 = CurHgt - 2;
            }
            if (x1 < 1)
            {
                x1 = 1;
            }
            if (x2 > CurWid - 2)
            {
                x2 = CurWid - 2;
            }
            for (int y = y1; y <= y2; y++)
            {
                for (int x = x1; x <= x2; x++)
                {
                    GridTile cPtr = Grid[y][x];
                    if (!cPtr.FeatureType.IsWall)
                    {
                        if (!cPtr.FeatureType.IsOpenFloor)
                        {
                            cPtr.TileFlags.Set(GridTile.PlayerMemorised);
                        }
                        for (int i = 0; i < 8; i++)
                        {
                            cPtr = Grid[y + OrderedDirectionYOffset[i]][x + OrderedDirectionXOffset[i]];
                            if (cPtr.FeatureType.IsWall)
                            {
                                cPtr.TileFlags.Set(GridTile.PlayerMemorised);
                            }
                        }
                    }
                }
            }
            _player.RedrawNeeded.Set(RedrawFlag.PrMap);
        }

        public void MoveCursorRelative(int row, int col)
        {
            row -= PanelRowPrt;
            col -= PanelColPrt;
            Gui.Goto(row, col);
        }

        public void MoveOneStepTowards(out int newY, out int newX, int currentY, int currentX, int startY, int startX, int targetY, int targetX)
        {
            newY = currentY;
            newX = currentX;
            int shift;
            int dY = newY < startY ? startY - newY : newY - startY;
            int dX = newX < startX ? startX - newX : newX - startX;
            int distance = dY > dX ? dY : dX;
            distance++;
            dY = targetY < startY ? startY - targetY : targetY - startY;
            dX = targetX < startX ? startX - targetX : targetX - startX;
            if (dY == 0 && dX == 0)
            {
                return;
            }
            if (dY > dX)
            {
                shift = ((distance * dX) + ((dY - 1) / 2)) / dY;
                newX = targetX < startX ? startX - shift : startX + shift;
                newY = targetY < startY ? startY - distance : startY + distance;
            }
            else
            {
                shift = ((distance * dY) + ((dX - 1) / 2)) / dX;
                newY = targetY < startY ? startY - shift : startY + shift;
                newX = targetX < startX ? startX - distance : startX + distance;
            }
        }

        public bool NoLight()
        {
            return !PlayerCanSeeBold(_player.MapY, _player.MapX);
        }

        public void NoteSpot(int y, int x)
        {
            GridTile cPtr = Grid[y][x];
            int nextOIdx;
            if (_player.TimedBlindness != 0)
            {
                return;
            }
            if (cPtr.TileFlags.IsClear(GridTile.PlayerLit))
            {
                if (cPtr.TileFlags.IsClear(GridTile.IsVisible))
                {
                    return;
                }
                if (cPtr.TileFlags.IsClear(GridTile.SelfLit))
                {
                    return;
                }
            }
            for (int thisOIdx = cPtr.Item; thisOIdx != 0; thisOIdx = nextOIdx)
            {
                Item oPtr = Items[thisOIdx];
                nextOIdx = oPtr.NextInStack;
                oPtr.Marked = true;
            }
            if (cPtr.TileFlags.IsClear(GridTile.PlayerMemorised))
            {
                if (cPtr.FeatureType.IsOpenFloor)
                {
                    cPtr.TileFlags.Set(GridTile.PlayerMemorised);
                }
                else if (cPtr.FeatureType.IsPassable)
                {
                    cPtr.TileFlags.Set(GridTile.PlayerMemorised);
                }
                else if (cPtr.TileFlags.IsSet(GridTile.PlayerLit))
                {
                    cPtr.TileFlags.Set(GridTile.PlayerMemorised);
                }
                else
                {
                    int yy = y < _player.MapY ? y + 1 : y > _player.MapY ? y - 1 : y;
                    int xx = x < _player.MapX ? x + 1 : x > _player.MapX ? x - 1 : x;
                    if (Grid[yy][xx].TileFlags.IsSet(GridTile.SelfLit))
                    {
                        cPtr.TileFlags.Set(GridTile.PlayerMemorised);
                    }
                }
            }
        }

        public int OPop()
        {
            int i;
            if (OMax < Constants.MaxOIdx)
            {
                i = OMax;
                OMax++;
                OCnt++;
                return i;
            }
            for (i = 1; i < OMax; i++)
            {
                Item oPtr = Items[i];
                if (oPtr.ItemType != null)
                {
                    continue;
                }
                OCnt++;
                return i;
            }
            return 0;
        }

        public bool PanelContains(int y, int x)
        {
            return y >= PanelRowMin && y <= PanelRowMax && x >= PanelColMin && x <= PanelColMax;
        }

        public void PickTrap(int y, int x)
        {
            string feat;
            GridTile cPtr = Grid[y][x];
            if (cPtr.FeatureType.Name != "Invis")
            {
                return;
            }
            int trapType = Program.Rng.DieRoll(16);
            if (SaveGame.Instance.Quests.IsQuest(SaveGame.Instance.DunLevel))
            {
                trapType = Program.Rng.DieRoll(15);
            }
            if (SaveGame.Instance.DunLevel >= SaveGame.Instance.CurDungeon.MaxLevel)
            {
                trapType = Program.Rng.DieRoll(15);
            }
            switch (trapType)
            {
                case 1:
                    feat = "AcidTrap";
                    break;

                case 2:
                    feat = "FireTrap";
                    break;

                case 3:
                    feat = "StrDart";
                    break;

                case 4:
                    feat = "ConDart";
                    break;

                case 5:
                    feat = "DexDart";
                    break;

                case 6:
                    feat = "SlowDart";
                    break;

                case 7:
                    feat = "PoisonGas";
                    break;

                case 8:
                    feat = "ConfuseGas";
                    break;

                case 9:
                    feat = "SleepGas";
                    break;

                case 10:
                    feat = "BlindGas";
                    break;

                case 11:
                    feat = "SummonRune";
                    break;

                case 12:
                    feat = "TeleportRune";
                    break;

                case 13:
                    feat = "Pit";
                    break;

                case 14:
                    feat = "SpikedPit";
                    break;

                case 15:
                    feat = "PoisonPit";
                    break;

                default:
                    feat = "TrapDoor";
                    break;
            }
            CaveSetFeat(y, x, feat);
        }

        public void PlaceGold(int y, int x)
        {
            if (!InBounds(y, x))
            {
                return;
            }
            if (!GridOpenNoItem(y, x))
            {
                return;
            }
            Item qPtr = new Item();
            if (!qPtr.MakeGold())
            {
                return;
            }
            int oIdx = OPop();
            if (oIdx != 0)
            {
                Items[oIdx] = new Item(qPtr);
                Item oPtr = Items[oIdx];
                oPtr.Y = y;
                oPtr.X = x;
                GridTile cPtr = Grid[y][x];
                oPtr.NextInStack = cPtr.Item;
                cPtr.Item = oIdx;
                NoteSpot(y, x);
                RedrawSingleLocation(y, x);
            }
        }

        public void PlaceObject(int y, int x, bool good, bool great)
        {
            if (!InBounds(y, x))
            {
                return;
            }
            if (!GridOpenNoItem(y, x))
            {
                return;
            }
            Item qPtr = new Item();
            if (!qPtr.MakeObject(good, great))
            {
                return;
            }
            int oIdx = OPop();
            if (oIdx != 0)
            {
                Items[oIdx] = new Item(qPtr);
                Item oPtr = Items[oIdx];
                oPtr.Y = y;
                oPtr.X = x;
                GridTile cPtr = Grid[y][x];
                oPtr.NextInStack = cPtr.Item;
                cPtr.Item = oIdx;
                NoteSpot(y, x);
                RedrawSingleLocation(y, x);
            }
            else
            {
                if (qPtr.FixedArtifactIndex != 0)
                {
                    Profile.Instance.FixedArtifacts[qPtr.FixedArtifactIndex].CurNum = 0;
                }
            }
        }

        public void PlaceTrap(int y, int x)
        {
            if (!InBounds(y, x))
            {
                return;
            }
            if (!GridOpenNoItemOrCreature(y, x))
            {
                return;
            }
            CaveSetFeat(y, x, "Invis");
        }

        public bool PlayerCanSeeBold(int y, int x)
        {
            if (_player.TimedBlindness != 0)
            {
                return false;
            }
            GridTile cPtr = Grid[y][x];
            if (cPtr.TileFlags.IsSet(GridTile.PlayerLit))
            {
                return true;
            }
            if (!PlayerHasLosBold(y, x))
            {
                return false;
            }
            if (cPtr.TileFlags.IsClear(GridTile.SelfLit))
            {
                return false;
            }
            if (GridPassable(y, x))
            {
                return true;
            }
            int yy = y < _player.MapY ? y + 1 : y > _player.MapY ? y - 1 : y;
            int xx = x < _player.MapX ? x + 1 : x > _player.MapX ? x - 1 : x;
            return Grid[yy][xx].TileFlags.IsSet(GridTile.SelfLit);
        }

        public bool PlayerHasLosBold(int y, int x)
        {
            return Grid[y][x].TileFlags.IsSet(GridTile.IsVisible);
        }

        public void PrintCharacterAtMapLocation(char c, Colour a, int y, int x)
        {
            if (PanelContains(y, x))
            {
                if (_player.TimedInvulnerability != 0)
                {
                    a = Colour.White;
                }
                else if (_player.TimedEtherealness != 0)
                {
                    a = Colour.Black;
                }
                Gui.Place(a, c, y - PanelRowPrt, x - PanelColPrt);
            }
        }

        public bool Projectable(int y1, int x1, int y2, int x2)
        {
            int y = y1;
            int x = x1;
            for (int dist = 0; dist <= Constants.MaxRange; dist++)
            {
                if (x == x2 && y == y2)
                {
                    return true;
                }
                if (dist != 0 && !GridPassable(y, x) && Grid[y][x].FeatureType.Name != "YellowSign")
                {
                    break;
                }
                MoveOneStepTowards(out y, out x, y, x, y1, x1, y2, x2);
            }
            return false;
        }

        public void PrtMap()
        {
            bool v = Gui.CursorVisible;
            Gui.CursorVisible = false;
            for (int y = PanelRowMin; y <= PanelRowMax; y++)
            {
                for (int x = PanelColMin; x <= PanelColMax; x++)
                {
                    MapInfo(y, x, out Colour a, out char c);
                    if (_player.TimedInvulnerability != 0)
                    {
                        a = Colour.White;
                    }
                    else if (_player.TimedEtherealness != 0)
                    {
                        a = Colour.Black;
                    }
                    Gui.Print(a, c, y - PanelRowPrt, x - PanelColPrt);
                }
            }
            RedrawSingleLocation(_player.MapY, _player.MapX);
            Gui.CursorVisible = v;
        }

        public void PutQuestMonster(int rIdx)
        {
            int y, x;
            if (Profile.Instance.MonsterRaces[rIdx].MaxNum == 0)
            {
                Profile.Instance.MonsterRaces[rIdx].MaxNum++;
                Profile.Instance.MsgPrint("Resurrecting guardian to fix corrupted savefile...");
            }
            do
            {
                while (true)
                {
                    y = Program.Rng.RandomLessThan(MaxHgt);
                    x = Program.Rng.RandomLessThan(MaxWid);
                    if (!GridOpenNoItemOrCreature(y, x))
                    {
                        continue;
                    }
                    {
                        if (Distance(y, x, SaveGame.Instance.Player.MapY, SaveGame.Instance.Player.MapX) > 15)
                        {
                            break;
                        }
                    }
                }
            } while (!Monsters.PlaceMonsterByIndex(y, x, rIdx, false, false, false));
        }

        public void RedrawSingleLocation(int y, int x)
        {
            if (PanelContains(y, x))
            {
                Colour a;
                char c;
                {
                    MapInfo(y, x, out a, out c);
                }
                if (_player.TimedInvulnerability != 0)
                {
                    a = Colour.White;
                }
                else if (_player.TimedEtherealness != 0)
                {
                    a = Colour.Black;
                }
                Gui.Print(a, c, y - PanelRowPrt, x - PanelColPrt);
            }
        }

        public void ReplacePets(int y, int x, List<Monster> petList)
        {
            foreach (Monster monster in petList)
            {
                Monsters.ReplacePet(y, x, monster);
            }
        }

        public void ReplaceSecretDoor(int y, int x)
        {
            int tmp = Program.Rng.RandomLessThan(400);
            if (tmp < 300)
            {
                CaveSetFeat(y, x, "LockedDoor0");
            }
            else if (tmp < 999)
            {
                CaveSetFeat(y, x, $"LockedDoor{Program.Rng.DieRoll(7)}");
            }
            else
            {
                CaveSetFeat(y, x, $"JammedDoor{Program.Rng.RandomLessThan(8)}");
            }
        }

        public void ReportChargeUsageFromFloor(int item)
        {
            Item oPtr = Items[item];
            if (oPtr.Category != ItemCategory.Staff && oPtr.Category != ItemCategory.Wand)
            {
                return;
            }
            if (!oPtr.IsKnown())
            {
                return;
            }
            Profile.Instance.MsgPrint(oPtr.TypeSpecificValue != 1
                ? $"There are {oPtr.TypeSpecificValue} charges remaining."
                : $"There is {oPtr.TypeSpecificValue} charge remaining.");
        }

        public void Scatter(out int yp, out int xp, int y, int x, int d)
        {
            int nx = 0;
            int ny = 0;
            yp = y;
            xp = x;
            int attemptsLeft = 5000;
            while (--attemptsLeft != 0)
            {
                ny = Program.Rng.RandomSpread(y, d);
                nx = Program.Rng.RandomSpread(x, d);
                if (!InBounds(y, x))
                {
                    continue;
                }
                if (d > 1 && Distance(y, x, ny, nx) > d)
                {
                    continue;
                }
                if (Los(y, x, ny, nx))
                {
                    break;
                }
            }
            if (attemptsLeft > 0)
            {
                yp = ny;
                xp = nx;
            }
        }

        public void UpdateFlow()
        {
            int x, y;
            if (TempN != 0)
            {
                return;
            }
            if (_flowN == 255)
            {
                for (y = 0; y < CurHgt; y++)
                {
                    for (x = 0; x < CurWid; x++)
                    {
                        int w = Grid[y][x].ScentAge;
                        Grid[y][x].ScentAge = w > 128 ? w - 128 : 0;
                    }
                }
                _flowN = 127;
            }
            _flowN++;
            _flowHead = 0;
            _flowTail = 0;
            UpdateFlowAux(_player.MapY, _player.MapX, 0);
            while (_flowHead != _flowTail)
            {
                y = TempY[_flowTail];
                x = TempX[_flowTail];
                if (++_flowTail == Constants.TempMax)
                {
                    _flowTail = 0;
                }
                for (int d = 0; d < 8; d++)
                {
                    UpdateFlowAux(y + OrderedDirectionYOffset[d], x + OrderedDirectionXOffset[d], Grid[y][x].ScentStrength + 1);
                }
            }
            _flowHead = 0;
            _flowTail = 0;
        }

        public void UpdateLight()
        {
            int i, x, y;
            if (_player.LightLevel <= 0)
            {
                ForgetLight();
                RedrawSingleLocation(_player.MapY, _player.MapX);
                return;
            }
            for (i = 0; i < _lightN; i++)
            {
                y = _lightY[i];
                x = _lightX[i];
                Grid[y][x].TileFlags.Clear(GridTile.PlayerLit);
                Grid[y][x].TileFlags.Set(GridTile.TempFlag);
                TempY[TempN] = y;
                TempX[TempN] = x;
                TempN++;
            }
            _lightN = 0;
            CaveLightHack(_player.MapY, _player.MapX);
            if (_player.LightLevel >= 1)
            {
                CaveLightHack(_player.MapY + 1, _player.MapX);
                CaveLightHack(_player.MapY - 1, _player.MapX);
                CaveLightHack(_player.MapY, _player.MapX + 1);
                CaveLightHack(_player.MapY, _player.MapX - 1);
                CaveLightHack(_player.MapY + 1, _player.MapX + 1);
                CaveLightHack(_player.MapY + 1, _player.MapX - 1);
                CaveLightHack(_player.MapY - 1, _player.MapX + 1);
                CaveLightHack(_player.MapY - 1, _player.MapX - 1);
            }
            if (_player.LightLevel >= 2)
            {
                if (GridPassable(_player.MapY + 1, _player.MapX))
                {
                    CaveLightHack(_player.MapY + 2, _player.MapX);
                    CaveLightHack(_player.MapY + 2, _player.MapX + 1);
                    CaveLightHack(_player.MapY + 2, _player.MapX - 1);
                }
                if (GridPassable(_player.MapY - 1, _player.MapX))
                {
                    CaveLightHack(_player.MapY - 2, _player.MapX);
                    CaveLightHack(_player.MapY - 2, _player.MapX + 1);
                    CaveLightHack(_player.MapY - 2, _player.MapX - 1);
                }
                if (GridPassable(_player.MapY, _player.MapX + 1))
                {
                    CaveLightHack(_player.MapY, _player.MapX + 2);
                    CaveLightHack(_player.MapY + 1, _player.MapX + 2);
                    CaveLightHack(_player.MapY - 1, _player.MapX + 2);
                }
                if (GridPassable(_player.MapY, _player.MapX - 1))
                {
                    CaveLightHack(_player.MapY, _player.MapX - 2);
                    CaveLightHack(_player.MapY + 1, _player.MapX - 2);
                    CaveLightHack(_player.MapY - 1, _player.MapX - 2);
                }
            }
            if (_player.LightLevel >= 3)
            {
                int p = _player.LightLevel;
                if (p > 5)
                {
                    p = 5;
                }
                if (GridPassable(_player.MapY + 1, _player.MapX + 1))
                {
                    CaveLightHack(_player.MapY + 2, _player.MapX + 2);
                }
                if (GridPassable(_player.MapY + 1, _player.MapX - 1))
                {
                    CaveLightHack(_player.MapY + 2, _player.MapX - 2);
                }
                if (GridPassable(_player.MapY - 1, _player.MapX + 1))
                {
                    CaveLightHack(_player.MapY - 2, _player.MapX + 2);
                }
                if (GridPassable(_player.MapY - 1, _player.MapX - 1))
                {
                    CaveLightHack(_player.MapY - 2, _player.MapX - 2);
                }
                int minY = _player.MapY - p;
                if (minY < 0)
                {
                    minY = 0;
                }
                int maxY = _player.MapY + p;
                if (maxY > CurHgt - 1)
                {
                    maxY = CurHgt - 1;
                }
                int minX = _player.MapX - p;
                if (minX < 0)
                {
                    minX = 0;
                }
                int maxX = _player.MapX + p;
                if (maxX > CurWid - 1)
                {
                    maxX = CurWid - 1;
                }
                for (y = minY; y <= maxY; y++)
                {
                    for (x = minX; x <= maxX; x++)
                    {
                        int dy = _player.MapY > y ? _player.MapY - y : y - _player.MapY;
                        int dx = _player.MapX > x ? _player.MapX - x : x - _player.MapX;
                        if (dy <= 2 && dx <= 2)
                        {
                            continue;
                        }
                        int d = dy > dx ? dy + (dx >> 1) : dx + (dy >> 1);
                        if (d > p)
                        {
                            continue;
                        }
                        if (PlayerHasLosBold(y, x))
                        {
                            CaveLightHack(y, x);
                        }
                    }
                }
            }
            for (i = 0; i < _lightN; i++)
            {
                y = _lightY[i];
                x = _lightX[i];
                if (Grid[y][x].TileFlags.IsSet(GridTile.TempFlag))
                {
                    continue;
                }
                NoteSpot(y, x);
                RedrawSingleLocation(y, x);
            }
            for (i = 0; i < TempN; i++)
            {
                y = TempY[i];
                x = TempX[i];
                Grid[y][x].TileFlags.Clear(GridTile.TempFlag);
                if (Grid[y][x].TileFlags.IsSet(GridTile.PlayerLit))
                {
                    continue;
                }
                RedrawSingleLocation(y, x);
            }
            TempN = 0;
        }

        public void UpdateMonsters(bool full)
        {
            for (int i = 1; i < MMax; i++)
            {
                Monster mPtr = Monsters[i];
                if (mPtr.Race == null)
                {
                    continue;
                }
                Monsters.UpdateMonsterVisibility(i, full);
            }
        }

        public void UpdateView()
        {
            int n;
            int d;
            int y, x;
            int yMax = CurHgt - 1;
            int xMax = CurWid - 1;
            GridTile cPtr;
            const int full = Constants.MaxSight;
            const int over = Constants.MaxSight * 3 / 2;
            for (n = 0; n < _viewN; n++)
            {
                y = _viewY[n];
                x = _viewX[n];
                cPtr = Grid[y][x];
                cPtr.TileFlags.Clear(GridTile.IsVisible);
                cPtr.TileFlags.Set(GridTile.TempFlag);
                TempY[TempN] = y;
                TempX[TempN] = x;
                TempN++;
            }
            _viewN = 0;
            y = _player.MapY;
            x = _player.MapX;
            cPtr = Grid[y][x];
            cPtr.TileFlags.Set(GridTile.EasyVisibility);
            CaveViewHack(cPtr, y, x);
            int z = full * 2 / 3;
            for (d = 1; d <= z; d++)
            {
                cPtr = Grid[y + d][x + d];
                cPtr.TileFlags.Set(GridTile.EasyVisibility);
                CaveViewHack(cPtr, y + d, x + d);
                if (cPtr.FeatureType.BlocksLos)
                {
                    break;
                }
                if (!InBounds2(y + d, x + d))
                {
                    break;
                }
            }
            for (d = 1; d <= z; d++)
            {
                cPtr = Grid[y + d][x - d];
                cPtr.TileFlags.Set(GridTile.EasyVisibility);
                CaveViewHack(cPtr, y + d, x - d);
                if (cPtr.FeatureType.BlocksLos)
                {
                    break;
                }
                if (!InBounds2(y + d, x - d))
                {
                    break;
                }
            }
            for (d = 1; d <= z; d++)
            {
                cPtr = Grid[y - d][x + d];
                cPtr.TileFlags.Set(GridTile.EasyVisibility);
                CaveViewHack(cPtr, y - d, x + d);
                if (cPtr.FeatureType.BlocksLos)
                {
                    break;
                }
                if (!InBounds2(y - d, x + d))
                {
                    break;
                }
            }
            for (d = 1; d <= z; d++)
            {
                cPtr = Grid[y - d][x - d];
                cPtr.TileFlags.Set(GridTile.EasyVisibility);
                CaveViewHack(cPtr, y - d, x - d);
                if (cPtr.FeatureType.BlocksLos)
                {
                    break;
                }
                if (!InBounds2(y - d, x - d))
                {
                    break;
                }
            }
            for (d = 1; d <= full; d++)
            {
                cPtr = Grid[y + d][x];
                cPtr.TileFlags.Set(GridTile.EasyVisibility);
                CaveViewHack(cPtr, y + d, x);
                if (cPtr.FeatureType.BlocksLos)
                {
                    break;
                }
                if (!InBounds2(y + d, x))
                {
                    break;
                }
            }
            int se = d;
            int sw = d;
            for (d = 1; d <= full; d++)
            {
                cPtr = Grid[y - d][x];
                cPtr.TileFlags.Set(GridTile.EasyVisibility);
                CaveViewHack(cPtr, y - d, x);
                if (cPtr.FeatureType.BlocksLos)
                {
                    break;
                }
                if (!InBounds2(y - d, x))
                {
                    break;
                }
            }
            int ne = d;
            int nw = d;
            for (d = 1; d <= full; d++)
            {
                cPtr = Grid[y][x + d];
                cPtr.TileFlags.Set(GridTile.EasyVisibility);
                CaveViewHack(cPtr, y, x + d);
                if (cPtr.FeatureType.BlocksLos)
                {
                    break;
                }
                if (!InBounds2(y, x + d))
                {
                    break;
                }
            }
            int es = d;
            int en = d;
            for (d = 1; d <= full; d++)
            {
                cPtr = Grid[y][x - d];
                cPtr.TileFlags.Set(GridTile.EasyVisibility);
                CaveViewHack(cPtr, y, x - d);
                if (cPtr.FeatureType.BlocksLos)
                {
                    break;
                }
                if (!InBounds2(y, x - d))
                {
                    break;
                }
            }
            int ws = d;
            int wn = d;
            for (n = 1; n <= over / 2; n++)
            {
                z = over - n - n;
                if (z > full - n)
                {
                    z = full - n;
                }
                while (z + n + (n >> 1) > full)
                {
                    z--;
                }
                int ypn = y + n;
                int ymn = y - n;
                int xpn = x + n;
                int xmn = x - n;
                int m;
                int k;
                if (ypn < yMax)
                {
                    m = Math.Min(z, yMax - ypn);
                    if (xpn <= xMax && n < se)
                    {
                        for (k = n, d = 1; d <= m; d++)
                        {
                            if (UpdateViewAux(ypn + d, xpn, ypn + d - 1, xpn - 1, ypn + d - 1, xpn))
                            {
                                if (n + d >= se)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                k = n + d;
                            }
                        }
                        se = k + 1;
                    }
                    if (xmn >= 0 && n < sw)
                    {
                        for (k = n, d = 1; d <= m; d++)
                        {
                            if (UpdateViewAux(ypn + d, xmn, ypn + d - 1, xmn + 1, ypn + d - 1, xmn))
                            {
                                if (n + d >= sw)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                k = n + d;
                            }
                        }
                        sw = k + 1;
                    }
                }
                if (ymn > 0)
                {
                    m = Math.Min(z, ymn);
                    if (xpn <= xMax && n < ne)
                    {
                        for (k = n, d = 1; d <= m; d++)
                        {
                            if (UpdateViewAux(ymn - d, xpn, ymn - d + 1, xpn - 1, ymn - d + 1, xpn))
                            {
                                if (n + d >= ne)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                k = n + d;
                            }
                        }
                        ne = k + 1;
                    }
                    if (xmn >= 0 && n < nw)
                    {
                        for (k = n, d = 1; d <= m; d++)
                        {
                            if (UpdateViewAux(ymn - d, xmn, ymn - d + 1, xmn + 1, ymn - d + 1, xmn))
                            {
                                if (n + d >= nw)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                k = n + d;
                            }
                        }
                        nw = k + 1;
                    }
                }
                if (xpn < xMax)
                {
                    m = Math.Min(z, xMax - xpn);
                    if (ypn <= yMax && n < es)
                    {
                        for (k = n, d = 1; d <= m; d++)
                        {
                            if (UpdateViewAux(ypn, xpn + d, ypn - 1, xpn + d - 1, ypn, xpn + d - 1))
                            {
                                if (n + d >= es)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                k = n + d;
                            }
                        }
                        es = k + 1;
                    }
                    if (ymn >= 0 && n < en)
                    {
                        for (k = n, d = 1; d <= m; d++)
                        {
                            if (UpdateViewAux(ymn, xpn + d, ymn + 1, xpn + d - 1, ymn, xpn + d - 1))
                            {
                                if (n + d >= en)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                k = n + d;
                            }
                        }
                        en = k + 1;
                    }
                }
                if (xmn > 0)
                {
                    m = Math.Min(z, xmn);
                    if (ypn <= yMax && n < ws)
                    {
                        for (k = n, d = 1; d <= m; d++)
                        {
                            if (UpdateViewAux(ypn, xmn - d, ypn - 1, xmn - d + 1, ypn, xmn - d + 1))
                            {
                                if (n + d >= ws)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                k = n + d;
                            }
                        }
                        ws = k + 1;
                    }
                    if (ymn >= 0 && n < wn)
                    {
                        for (k = n, d = 1; d <= m; d++)
                        {
                            if (UpdateViewAux(ymn, xmn - d, ymn + 1, xmn - d + 1, ymn, xmn - d + 1))
                            {
                                if (n + d >= wn)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                k = n + d;
                            }
                        }
                        wn = k + 1;
                    }
                }
            }
            for (n = 0; n < _viewN; n++)
            {
                y = _viewY[n];
                x = _viewX[n];
                cPtr = Grid[y][x];
                cPtr.TileFlags.Clear(GridTile.EasyVisibility);
                if (cPtr.TileFlags.IsSet(GridTile.TempFlag))
                {
                    continue;
                }
                NoteSpot(y, x);
                RedrawSingleLocation(y, x);
            }
            for (n = 0; n < TempN; n++)
            {
                y = TempY[n];
                x = TempX[n];
                cPtr = Grid[y][x];
                cPtr.TileFlags.Clear(GridTile.TempFlag);
                if (cPtr.TileFlags.IsSet(GridTile.IsVisible))
                {
                    continue;
                }
                RedrawSingleLocation(y, x);
            }
            TempN = 0;
        }

        public void WipeOList()
        {
            for (int i = 1; i < OMax; i++)
            {
                Item oPtr = Items[i];
                if (oPtr.ItemType == null)
                {
                    continue;
                }
                if (oPtr.HoldingMonsterIndex != 0)
                {
                    Monster mPtr = Monsters[oPtr.HoldingMonsterIndex];
                    mPtr.FirstHeldItemIndex = 0;
                }
                else
                {
                    int y = oPtr.Y;
                    int x = oPtr.X;
                    GridTile cPtr = Grid[y][x];
                    cPtr.Item = 0;
                }
                Items[i] = new Item();
            }
            OMax = 1;
            OCnt = 0;
        }

        public void WizDark()
        {
            for (int y = 0; y < CurHgt; y++)
            {
                for (int x = 0; x < CurWid; x++)
                {
                    GridTile cPtr = Grid[y][x];
                    cPtr.TileFlags.Clear(GridTile.PlayerMemorised);
                }
            }
            for (int i = 1; i < OMax; i++)
            {
                Item oPtr = Items[i];
                if (oPtr.ItemType == null)
                {
                    continue;
                }
                if (oPtr.HoldingMonsterIndex != 0)
                {
                    continue;
                }
                oPtr.Marked = false;
            }
            _player.UpdatesNeeded.Set(UpdateFlags.UpdateRemoveView | UpdateFlags.UpdateRemoveLight);
            _player.UpdatesNeeded.Set(UpdateFlags.UpdateView | UpdateFlags.UpdateLight);
            _player.UpdatesNeeded.Set(UpdateFlags.UpdateMonsters);
            _player.RedrawNeeded.Set(RedrawFlag.PrMap);
        }

        public void WizLight()
        {
            int i;
            for (i = 1; i < OMax; i++)
            {
                Item oPtr = Items[i];
                if (oPtr.ItemType != null)
                {
                    continue;
                }
                if (oPtr.HoldingMonsterIndex != 0)
                {
                    continue;
                }
                oPtr.Marked = true;
            }
            for (int y = 1; y < CurHgt - 1; y++)
            {
                for (int x = 1; x < CurWid - 1; x++)
                {
                    GridTile cPtr = Grid[y][x];
                    if (!cPtr.FeatureType.IsWall)
                    {
                        for (i = 0; i < 9; i++)
                        {
                            int yy = y + OrderedDirectionYOffset[i];
                            int xx = x + OrderedDirectionXOffset[i];
                            cPtr = Grid[yy][xx];
                            cPtr.TileFlags.Set(GridTile.SelfLit);
                            if (!cPtr.FeatureType.IsOpenFloor)
                            {
                                cPtr.TileFlags.Set(GridTile.PlayerMemorised);
                            }
                            cPtr.TileFlags.Set(GridTile.PlayerMemorised);
                        }
                    }
                }
            }
            _player.UpdatesNeeded.Set(UpdateFlags.UpdateMonsters);
            _player.RedrawNeeded.Set(RedrawFlag.PrMap);
        }

        private void CaveLightHack(int y, int x)
        {
            Grid[y][x].TileFlags.Set(GridTile.PlayerLit);
            _lightY[_lightN] = y;
            _lightX[_lightN] = x;
            _lightN++;
        }

        private void CaveViewHack(GridTile c, int y, int x)
        {
            c.TileFlags.Set(GridTile.IsVisible);
            _viewY[_viewN] = y;
            _viewX[_viewN] = x;
            _viewN++;
        }

        private void CompactObjectsAux(int i1, int i2)
        {
            Item oPtr;
            if (i1 == i2)
            {
                return;
            }
            for (int i = 1; i < OMax; i++)
            {
                oPtr = Items[i];
                if (oPtr.ItemType == null)
                {
                    continue;
                }
                if (oPtr.NextInStack == i1)
                {
                    oPtr.NextInStack = i2;
                }
            }
            oPtr = Items[i1];
            if (oPtr.HoldingMonsterIndex != 0)
            {
                Monster mPtr = Monsters[oPtr.HoldingMonsterIndex];
                if (mPtr.FirstHeldItemIndex == i1)
                {
                    mPtr.FirstHeldItemIndex = i2;
                }
            }
            else
            {
                int y = oPtr.Y;
                int x = oPtr.X;
                GridTile cPtr = Grid[y][x];
                if (cPtr.Item == i1)
                {
                    cPtr.Item = i2;
                }
            }
            Items[i2] = Items[i1];
            Items[i1] = new Item();
        }

        private Colour DimColour(Colour a)
        {
            switch (a)
            {
                case Colour.BrightBlue:
                    return Colour.Blue;

                case Colour.BrightGreen:
                    return Colour.Green;

                case Colour.BrightRed:
                    return Colour.Red;

                case Colour.BrightWhite:
                    return Colour.White;

                case Colour.BrightBeige:
                    return Colour.Beige;

                case Colour.BrightChartreuse:
                    return Colour.Chartreuse;

                case Colour.BrightGrey:
                    return Colour.Grey;

                case Colour.BrightOrange:
                    return Colour.Orange;

                case Colour.BrightYellow:
                    return Colour.Yellow;

                case Colour.BrightBrown:
                    return Colour.Brown;

                case Colour.BrightTurquoise:
                    return Colour.Turquoise;

                case Colour.BrightPink:
                    return Colour.Pink;

                case Colour.Grey:
                    return Colour.Black;

                case Colour.White:
                    return Colour.Grey;

                case Colour.Yellow:
                    return Colour.BrightBrown;

                default:
                    return a;
            }
        }

        private bool GridBlocksLos(int y, int x)
        {
            return Grid[y][x].FeatureType.BlocksLos;
        }

        private void ImageMonster(out Colour ap, out char cp)
        {
            cp = Profile.Instance.MonsterRaces[Program.Rng.DieRoll(Profile.Instance.MonsterRaces.Count - 2)].Character;
            ap = Profile.Instance.MonsterRaces[Program.Rng.DieRoll(Profile.Instance.MonsterRaces.Count - 2)].Colour;
        }

        private void ImageObject(out Colour ap, out char cp)
        {
            cp = Profile.Instance.ItemTypes[Program.Rng.DieRoll(Profile.Instance.ItemTypes.Count - 1)].Character;
            ap = Profile.Instance.ItemTypes[Program.Rng.DieRoll(Profile.Instance.ItemTypes.Count - 1)].Colour;
        }

        private void ImageRandom(out Colour ap, out char cp)
        {
            if (Program.Rng.RandomLessThan(100) < 75)
            {
                ImageMonster(out ap, out cp);
            }
            else
            {
                ImageObject(out ap, out cp);
            }
        }

        private void MapInfo(int y, int x, out Colour ap, out char cp)
        {
            int nextOIdx;
            Colour a;
            char c;
            GridTile cPtr = Grid[y][x];
            FloorTileType feat = cPtr.FeatureType;
            if (feat.IsOpenFloor)
            {
                if (cPtr.TileFlags.IsSet(GridTile.PlayerMemorised) ||
                    ((cPtr.TileFlags.IsSet(GridTile.PlayerLit) || (cPtr.TileFlags.IsSet(GridTile.SelfLit) &&
                     cPtr.TileFlags.IsSet(GridTile.IsVisible))) && _player.TimedBlindness == 0))
                {
                    c = feat.Character;
                    a = feat.Colour;
                    if (feat.DimsOutsideLOS)
                    {
                        if (_player.TimedBlindness != 0)
                        {
                            a = Colour.Black;
                        }
                        else if (cPtr.TileFlags.IsSet(GridTile.PlayerLit))
                        {
                            if (feat.YellowInTorchlight)
                            {
                                a = Colour.BrightYellow;
                            }
                        }
                        else if (cPtr.TileFlags.IsClear(GridTile.SelfLit))
                        {
                            a = Colour.Black;
                        }
                        else if (cPtr.TileFlags.IsClear(GridTile.IsVisible))
                        {
                            a = DimColour(a);
                        }
                        if (cPtr.TileFlags.IsSet(GridTile.TrapsDetected))
                        {
                            int count = 0;
                            if (Grid[y - 1][x].TileFlags.IsSet(GridTile.TrapsDetected))
                            {
                                count++;
                            }
                            if (Grid[y + 1][x].TileFlags.IsSet(GridTile.TrapsDetected))
                            {
                                count++;
                            }
                            if (Grid[y][x - 1].TileFlags.IsSet(GridTile.TrapsDetected))
                            {
                                count++;
                            }
                            if (Grid[y][x + 1].TileFlags.IsSet(GridTile.TrapsDetected))
                            {
                                count++;
                            }
                            if (count != 4)
                            {
                                a = Colour.BrightChartreuse;
                            }
                        }
                    }
                }
                else
                {
                    a = StaticResources.Instance.FloorTileTypes["Nothing"].Colour;
                    c = StaticResources.Instance.FloorTileTypes["Nothing"].Character;
                }
            }
            else
            {
                if (cPtr.TileFlags.IsSet(GridTile.PlayerMemorised))
                {
                    feat = string.IsNullOrEmpty(feat.AppearAs)
                        ? StaticResources.Instance.FloorTileTypes[cPtr.BackgroundFeature.AppearAs]
                        : StaticResources.Instance.FloorTileTypes[feat.AppearAs];
                    c = feat.Character;
                    a = feat.Colour;
                    if (feat.DimsOutsideLOS)
                    {
                        if (_player.TimedBlindness != 0)
                        {
                            a = Colour.Black;
                        }
                        else if (cPtr.TileFlags.IsSet(GridTile.PlayerLit))
                        {
                            if (feat.YellowInTorchlight)
                            {
                                a = Colour.Yellow;
                            }
                        }
                        else
                        {
                            if (cPtr.TileFlags.IsClear(GridTile.IsVisible))
                            {
                                a = DimColour(a);
                            }
                            else if (cPtr.TileFlags.IsClear(GridTile.SelfLit))
                            {
                                a = DimColour(a);
                            }
                            else
                            {
                                int yy = y < _player.MapY ? y + 1 : y > _player.MapY ? y - 1 : y;
                                int xx = x < _player.MapX ? x + 1 : x > _player.MapX ? x - 1 : x;
                                if (Grid[yy][xx].TileFlags.IsClear(GridTile.SelfLit))
                                {
                                    a = DimColour(a);
                                }
                            }
                        }
                    }
                }
                else
                {
                    a = StaticResources.Instance.FloorTileTypes["Nothing"].Colour;
                    c = StaticResources.Instance.FloorTileTypes["Nothing"].Character;
                }
            }
            if (_player.TimedHallucinations != 0 && Program.Rng.RandomLessThan(256) == 0 && (!cPtr.FeatureType.IsWall))
            {
                ImageRandom(out ap, out cp);
            }
            else
            {
                ap = a;
                cp = c;
            }
            for (int thisOIdx = cPtr.Item; thisOIdx != 0; thisOIdx = nextOIdx)
            {
                Item oPtr = Items[thisOIdx];
                nextOIdx = oPtr.NextInStack;
                if (oPtr.Marked)
                {
                    cp = oPtr.ItemType.Character;
                    ap = oPtr.ItemType.Colour;
                    if (_player.TimedHallucinations != 0)
                    {
                        ImageObject(out ap, out cp);
                    }
                    break;
                }
            }
            if (cPtr.Monster != 0)
            {
                Monster mPtr = Monsters[cPtr.Monster];
                if (mPtr.IsVisible)
                {
                    MonsterRace rPtr = mPtr.Race;
                    a = rPtr.Colour;
                    c = rPtr.Character;
                    if ((rPtr.Flags1 & MonsterFlag1.AttrMulti) != 0)
                    {
                        if ((rPtr.Flags2 & MonsterFlag2.Shapechanger) != 0)
                        {
                            cp = Program.Rng.DieRoll(25) == 1
                                ? _imageObjectHack[Program.Rng.RandomLessThan(_imageObjectHack.Length)]
                                : _imageMonsterHack[Program.Rng.RandomLessThan(_imageMonsterHack.Length)];
                        }
                        else
                        {
                            cp = c;
                        }
                        if ((rPtr.Flags2 & MonsterFlag2.AttrAny) != 0)
                        {
                            ap = (Colour)Program.Rng.DieRoll(15);
                        }
                        else
                        {
                            switch (Program.Rng.DieRoll(7))
                            {
                                case 1:
                                    ap = Colour.Red;
                                    break;

                                case 2:
                                    ap = Colour.BrightRed;
                                    break;

                                case 3:
                                    ap = Colour.White;
                                    break;

                                case 4:
                                    ap = Colour.BrightGreen;
                                    break;

                                case 5:
                                    ap = Colour.Blue;
                                    break;

                                case 6:
                                    ap = Colour.Black;
                                    break;

                                case 7:
                                    ap = Colour.Green;
                                    break;
                            }
                        }
                    }
                    else if ((rPtr.Flags1 & (MonsterFlag1.AttrClear | MonsterFlag1.CharClear)) == 0)
                    {
                        cp = c;
                        ap = a;
                    }
                    else
                    {
                        if ((rPtr.Flags1 & MonsterFlag1.CharClear) == 0)
                        {
                            cp = c;
                        }
                        else if ((rPtr.Flags1 & MonsterFlag1.AttrClear) == 0)
                        {
                            ap = a;
                        }
                    }
                    if (_player.TimedHallucinations != 0)
                    {
                        ImageMonster(out ap, out cp);
                    }
                }
            }
            if (y == _player.MapY && x == _player.MapX)
            {
                MonsterRace rPtr = Profile.Instance.MonsterRaces[0];
                a = rPtr.Colour;
                c = rPtr.Character;
                ap = a;
                cp = c;
            }
        }

        private void UpdateFlowAux(int y, int x, int n)
        {
            int oldHead = _flowHead;
            GridTile cPtr = Grid[y][x];
            if (cPtr.ScentAge == _flowN)
            {
                return;
            }
            if (cPtr.FeatureType.BlocksLos && cPtr.FeatureType.Name != "SecretDoor")
            {
                return;
            }
            cPtr.ScentAge = _flowN;
            cPtr.ScentStrength = n;
            if (n == Constants.MonsterFlowDepth)
            {
                return;
            }
            TempY[_flowHead] = y;
            TempX[_flowHead] = x;
            if (++_flowHead == Constants.TempMax)
            {
                _flowHead = 0;
            }
            if (_flowHead == _flowTail)
            {
                _flowHead = oldHead;
            }
        }

        private bool UpdateViewAux(int y, int x, int y1, int x1, int y2, int x2)
        {
            GridTile g1CPtr = Grid[y1][x1];
            GridTile g2CPtr = Grid[y2][x2];
            bool f1 = !g1CPtr.FeatureType.BlocksLos;
            bool f2 = !g2CPtr.FeatureType.BlocksLos;
            if (!f1 && !f2)
            {
                return true;
            }
            bool v1 = f1 && g1CPtr.TileFlags.IsSet(GridTile.IsVisible);
            bool v2 = f2 && g2CPtr.TileFlags.IsSet(GridTile.IsVisible);
            if (!v1 && !v2)
            {
                return true;
            }
            GridTile cPtr = Grid[y][x];
            bool wall = cPtr.FeatureType.BlocksLos;
            bool z1 = v1 && g1CPtr.TileFlags.IsSet(GridTile.EasyVisibility);
            bool z2 = v2 && g2CPtr.TileFlags.IsSet(GridTile.EasyVisibility);
            if (z1 && z2)
            {
                cPtr.TileFlags.Set(GridTile.EasyVisibility);
                CaveViewHack(cPtr, y, x);
                return wall;
            }
            if (z1)
            {
                CaveViewHack(cPtr, y, x);
                return wall;
            }
            if (v1 && v2)
            {
                CaveViewHack(cPtr, y, x);
                return wall;
            }
            if (wall)
            {
                CaveViewHack(cPtr, y, x);
                return true;
            }
            if (Los(_player.MapY, _player.MapX, y, x))
            {
                CaveViewHack(cPtr, y, x);
                return false;
            }
            return true;
        }
    }
}