using Cthangband.Enumerations;
using Cthangband.StaticData;
using Cthangband.UI;
using System.Collections.Generic;

namespace Cthangband
{
    internal class TargetEngine
    {
        private readonly Level _level;
        private readonly Player _player;

        public TargetEngine(Player player, Level level)
        {
            _player = player;
            _level = level;
        }

        public bool GetAimDir(out int dp)
        {
            dp = 0;
            int dir = Gui.CommandDir;
            if (TargetOkay())
            {
                dir = 5;
            }
            while (dir == 0)
            {
                string p = !TargetOkay()
                    ? "Direction ('*' to choose a target, Escape to cancel)? "
                    : "Direction ('5' for target, '*' to re-target, Escape to cancel)? ";
                if (!Gui.GetCom(p, out char command))
                {
                    break;
                }
                switch (command)
                {
                    case 'T':
                    case 't':
                    case '.':
                    case '5':
                    case '0':
                        {
                            dir = 5;
                            break;
                        }
                    case '*':
                        {
                            if (TargetSet(Constants.TargetKill))
                            {
                                dir = 5;
                            }
                            break;
                        }
                    default:
                        {
                            dir = Gui.GetKeymapDir(command);
                            break;
                        }
                }
                if (dir == 5 && !TargetOkay())
                {
                    dir = 0;
                }
            }
            if (dir == 0)
            {
                return false;
            }
            Gui.CommandDir = dir;
            if (_player.TimedConfusion != 0)
            {
                dir = _level.OrderedDirection[Program.Rng.RandomLessThan(8)];
            }
            if (Gui.CommandDir != dir)
            {
                Profile.Instance.MsgPrint("You are confused.");
            }
            dp = dir;
            return true;
        }

        public void GetHackDir(out int dp)
        {
            dp = 0;
            int dir = 0;
            while (dir == 0)
            {
                string p = !TargetOkay()
                    ? "Direction ('*' to choose a target, Escape to cancel)? "
                    : "Direction ('5' for target, '*' to re-target, Escape to cancel)? ";
                if (!Gui.GetCom(p, out char command))
                {
                    break;
                }
                switch (command)
                {
                    case 'T':
                    case 't':
                    case '.':
                    case '5':
                    case '0':
                        {
                            dir = 5;
                            break;
                        }
                    case '*':
                        {
                            if (TargetSet(Constants.TargetKill))
                            {
                                dir = 5;
                            }
                            break;
                        }
                    default:
                        {
                            dir = Gui.GetKeymapDir(command);
                            break;
                        }
                }
                if (dir == 5 && !TargetOkay())
                {
                    dir = 0;
                }
            }
            if (dir == 0)
            {
                return;
            }
            Gui.CommandDir = dir;
            if (_player.TimedConfusion != 0)
            {
                dir = _level.OrderedDirection[Program.Rng.RandomLessThan(8)];
            }
            if (Gui.CommandDir != dir)
            {
                Profile.Instance.MsgPrint("You are confused.");
            }
            dp = dir;
        }

        public bool GetRepDir(out int dp)
        {
            dp = 0;
            int dir = Gui.CommandDir;
            while (dir == 0)
            {
                if (!Gui.GetCom("Direction (Escape to cancel)? ", out char ch))
                {
                    break;
                }
                dir = Gui.GetKeymapDir(ch);
            }
            if (dir == 5)
            {
                dir = 0;
            }
            if (dir == 0)
            {
                return false;
            }
            Gui.CommandDir = dir;
            if (_player.TimedConfusion != 0)
            {
                if (Program.Rng.RandomLessThan(100) < 75)
                {
                    dir = _level.OrderedDirection[Program.Rng.RandomLessThan(8)];
                }
            }
            if (Gui.CommandDir != dir)
            {
                Profile.Instance.MsgPrint("You are confused.");
            }
            dp = dir;
            return true;
        }

        public void PanelBounds()
        {
            _level.PanelRowMin = _level.PanelRow * (Constants.ScreenHgt / 2);
            _level.PanelRowMax = _level.PanelRowMin + Constants.ScreenHgt - 1;
            _level.PanelRowPrt = _level.PanelRowMin - 1;
            _level.PanelColMin = _level.PanelCol * (Constants.ScreenWid / 2);
            _level.PanelColMax = _level.PanelColMin + Constants.ScreenWid - 1;
            _level.PanelColPrt = _level.PanelColMin - 13;
        }

        public void PanelBoundsCenter()
        {
            _level.PanelRow = _level.PanelRowMin / (Constants.ScreenHgt / 2);
            _level.PanelRowMax = _level.PanelRowMin + Constants.ScreenHgt - 1;
            _level.PanelRowPrt = _level.PanelRowMin - 1;
            _level.PanelCol = _level.PanelColMin / (Constants.ScreenWid / 2);
            _level.PanelColMax = _level.PanelColMin + Constants.ScreenWid - 1;
            _level.PanelColPrt = _level.PanelColMin - 13;
        }

        public void RecenterScreenAroundPlayer()
        {
            int y = _player.MapY;
            int x = _player.MapX;
            int maxProwMin = _level.MaxPanelRows * (Constants.ScreenHgt / 2);
            int maxPcolMin = _level.MaxPanelCols * (Constants.ScreenWid / 2);
            int prowMin = y - (Constants.ScreenHgt / 2);
            if (prowMin > maxProwMin)
            {
                prowMin = maxProwMin;
            }
            else if (prowMin < 0)
            {
                prowMin = 0;
            }
            int pcolMin = x - (Constants.ScreenWid / 2);
            if (pcolMin > maxPcolMin)
            {
                pcolMin = maxPcolMin;
            }
            else if (pcolMin < 0)
            {
                pcolMin = 0;
            }
            if (prowMin == _level.PanelRowMin && pcolMin == _level.PanelColMin)
            {
                return;
            }
            _level.PanelRowMin = prowMin;
            _level.PanelColMin = pcolMin;
            PanelBoundsCenter();
            _player.UpdatesNeeded.Set(UpdateFlags.UpdateMonsters);
            _player.RedrawNeeded.Set(RedrawFlag.PrMap);
        }

        public bool TargetOkay()
        {
            if (SaveGame.Instance.TargetWho < 0)
            {
                return true;
            }
            if (SaveGame.Instance.TargetWho <= 0)
            {
                return false;
            }
            if (!TargetAble(SaveGame.Instance.TargetWho))
            {
                return false;
            }
            Monster mPtr = _level.Monsters[SaveGame.Instance.TargetWho];
            SaveGame.Instance.TargetRow = mPtr.MapY;
            SaveGame.Instance.TargetCol = mPtr.MapX;
            return true;
        }

        public bool TargetSet(int mode)
        {
            int y = _player.MapY;
            int x = _player.MapX;
            bool done = false;
            SaveGame.Instance.TargetWho = 0;
            TargetSetPrepare(mode);
            int m = 0;
            if (_level.TempN != 0)
            {
                y = _level.TempY[m];
                x = _level.TempX[m];
            }
            while (!done)
            {
                GridTile cPtr = _level.Grid[y][x];
                string info = TargetAble(cPtr.MonsterIndex) ? "t,T,*" : "T,*";
                char query = TargetSetAux(y, x, mode | Constants.TargetLook, info);
                switch (query)
                {
                    case '\x1b':
                        {
                            done = true;
                            break;
                        }
                    case 't':
                        {
                            if (TargetAble(cPtr.MonsterIndex))
                            {
                                SaveGame.Instance.HealthTrack(cPtr.MonsterIndex);
                                SaveGame.Instance.TargetWho = cPtr.MonsterIndex;
                                SaveGame.Instance.TargetRow = y;
                                SaveGame.Instance.TargetCol = x;
                                done = true;
                            }
                            break;
                        }
                    case 'T':
                        SaveGame.Instance.TargetWho = -1;
                        SaveGame.Instance.TargetRow = y;
                        SaveGame.Instance.TargetCol = x;
                        done = true;
                        break;

                    case '*':
                        {
                            if (x == _level.TempX[m] && y == _level.TempY[m])
                            {
                                if (++m >= _level.TempN)
                                {
                                    m = 0;
                                    done = true;
                                }
                                else
                                {
                                    y = _level.TempY[m];
                                    x = _level.TempX[m];
                                }
                            }
                            else
                            {
                                y = _level.TempY[m];
                                x = _level.TempX[m];
                            }
                            break;
                        }
                    default:
                        {
                            int d = Gui.GetKeymapDir(query);
                            if (d != 0)
                            {
                                x += _level.KeypadDirectionXOffset[d];
                                y += _level.KeypadDirectionYOffset[d];
                                if (x >= _level.CurWid - 1 || x > _level.PanelColMax)
                                {
                                    x--;
                                }
                                else if (x <= 0 || x < _level.PanelColMin)
                                {
                                    x++;
                                }
                                if (y >= _level.CurHgt - 1 || y > _level.PanelRowMax)
                                {
                                    y--;
                                }
                                else if (y <= 0 || y < _level.PanelRowMin)
                                {
                                    y++;
                                }
                            }
                            break;
                        }
                }
            }
            _level.TempN = 0;
            Gui.PrintLine("", 0, 0);
            return SaveGame.Instance.TargetWho != 0;
        }

        public bool TgtPt(out int x, out int y)
        {
            char ch = '\0';
            bool success = false;
            x = _player.MapX;
            y = _player.MapY;
            bool cv = Gui.CursorVisible;
            Gui.CursorVisible = true;
            Profile.Instance.MsgPrint("Select a point and press space.");
            while (ch != 27 && ch != ' ')
            {
                _level.MoveCursorRelative(y, x);
                ch = Gui.Inkey();
                switch (ch)
                {
                    case '\x1b':
                        break;

                    case ' ':
                        success = true;
                        break;

                    default:
                        {
                            int d = Gui.GetKeymapDir(ch);
                            if (d == 0)
                            {
                                break;
                            }
                            x += _level.KeypadDirectionXOffset[d];
                            y += _level.KeypadDirectionYOffset[d];
                            if (x >= _level.CurWid - 1 || x >= _level.PanelColMin + Constants.ScreenWid)
                            {
                                x--;
                            }
                            else if (x <= 0 || x <= _level.PanelColMin)
                            {
                                x++;
                            }
                            if (y >= _level.CurHgt - 1 || y >= _level.PanelRowMin + Constants.ScreenHgt)
                            {
                                y--;
                            }
                            else if (y <= 0 || y <= _level.PanelRowMin)
                            {
                                y++;
                            }
                            break;
                        }
                }
            }
            Gui.CursorVisible = cv;
            Gui.Refresh();
            return success;
        }

        private string LookMonDesc(int mIdx)
        {
            Monster mPtr = _level.Monsters[mIdx];
            MonsterRace rPtr = mPtr.Race;
            bool living = (rPtr.Flags3 & MonsterFlag3.Undead) == 0;
            if ((rPtr.Flags3 & MonsterFlag3.Demon) != 0)
            {
                living = false;
            }
            if ((rPtr.Flags3 & MonsterFlag3.Cthuloid) != 0)
            {
                living = false;
            }
            if ((rPtr.Flags3 & MonsterFlag3.Nonliving) != 0)
            {
                living = false;
            }
            if ("Egv".Contains(rPtr.Character.ToString()))
            {
                living = false;
            }
            if (mPtr.Health >= mPtr.MaxHealth)
            {
                return living ? "unhurt" : "undamaged";
            }
            int perc = 100 * mPtr.Health / mPtr.MaxHealth;
            if (perc >= 60)
            {
                return living ? "somewhat wounded" : "somewhat damaged";
            }
            if (perc >= 25)
            {
                return living ? "wounded" : "damaged";
            }
            if (perc >= 10)
            {
                return living ? "badly wounded" : "badly damaged";
            }
            return living ? "almost dead" : "almost destroyed";
        }

        private bool TargetAble(int mIdx)
        {
            Monster mPtr = _level.Monsters[mIdx];
            if (mPtr.Race == null)
            {
                return false;
            }
            if (!mPtr.IsVisible)
            {
                return false;
            }
            if (!_level.Projectable(_player.MapY, _player.MapX, mPtr.MapY, mPtr.MapX))
            {
                return false;
            }
            if (_player.TimedHallucinations != 0)
            {
                return false;
            }
            return true;
        }

        private bool TargetSetAccept(int y, int x)
        {
            int nextOIdx;
            if (y == _player.MapY && x == _player.MapX)
            {
                return true;
            }
            if (_player.TimedHallucinations != 0)
            {
                return false;
            }
            GridTile cPtr = _level.Grid[y][x];
            if (cPtr.MonsterIndex != 0)
            {
                Monster mPtr = _level.Monsters[cPtr.MonsterIndex];
                if (mPtr.IsVisible)
                {
                    return true;
                }
            }
            for (int thisOIdx = cPtr.ItemIndex; thisOIdx != 0; thisOIdx = nextOIdx)
            {
                Item oPtr = _level.Items[thisOIdx];
                nextOIdx = oPtr.NextInStack;
                if (oPtr.Marked)
                {
                    return true;
                }
            }
            if (cPtr.TileFlags.IsSet(GridTile.PlayerMemorised))
            {
                return cPtr.FeatureType.IsInteresting;
            }
            return false;
        }

        private char TargetSetAux(int y, int x, int mode, string info)
        {
            GridTile cPtr = _level.Grid[y][x];
            char query;
            do
            {
                query = ' ';
                bool boring = true;
                string s1 = "You see ";
                string s2 = "";
                string s3 = "";
                if (y == _player.MapY && x == _player.MapX)
                {
                    s1 = "You are ";
                    s2 = "on ";
                }
                string outVal;
                if (_player.TimedHallucinations != 0)
                {
                    const string name = "something strange";
                    outVal = $"{s1}{s2}{s3}{name} [{info}]";
                    Gui.PrintLine(outVal, 0, 0);
                    _level.MoveCursorRelative(y, x);
                    query = Gui.Inkey();
                    if (query != '\r' && query != '\n')
                    {
                        break;
                    }
                    continue;
                }
                int thisOIdx;
                int nextOIdx;
                if (cPtr.MonsterIndex != 0)
                {
                    Monster mPtr = _level.Monsters[cPtr.MonsterIndex];
                    MonsterRace rPtr = mPtr.Race;
                    if (mPtr.IsVisible)
                    {
                        bool recall = false;
                        boring = false;
                        string mName = mPtr.MonsterDesc(0x08);
                        SaveGame.Instance.HealthTrack(cPtr.MonsterIndex);
                        SaveGame.Instance.HandleStuff();
                        while (true)
                        {
                            if (recall)
                            {
                                Gui.Save();
                                rPtr.Knowledge.Display();
                                Gui.Print(Colour.White, $"  [r,{info}]", -1);
                                query = Gui.Inkey();
                                Gui.Load();
                            }
                            else
                            {
                                string c = (mPtr.Mind & Constants.SmCloned) != 0 ? " (clone)" : "";
                                string a = (mPtr.Mind & Constants.SmFriendly) != 0 ? " (allied) " : " ";
                                outVal = $"{s1}{s2}{s3}{mName} ({LookMonDesc(cPtr.MonsterIndex)}){c}{a}[r,{info}]";
                                Gui.PrintLine(outVal, 0, 0);
                                _level.MoveCursorRelative(y, x);
                                query = Gui.Inkey();
                            }
                            if (query != 'r')
                            {
                                break;
                            }
                            recall = !recall;
                        }
                        if (query != '\r' && query != '\n' && query != ' ')
                        {
                            break;
                        }
                        if (query == ' ' && (mode & Constants.TargetLook) == 0)
                        {
                            break;
                        }
                        s1 = "It is ";
                        if ((rPtr.Flags1 & MonsterFlag1.Female) != 0)
                        {
                            s1 = "She is ";
                        }
                        else if ((rPtr.Flags1 & MonsterFlag1.Male) != 0)
                        {
                            s1 = "He is ";
                        }
                        s2 = "carrying ";
                        for (thisOIdx = mPtr.FirstHeldItemIndex; thisOIdx != 0; thisOIdx = nextOIdx)
                        {
                            Item oPtr = _level.Items[thisOIdx];
                            nextOIdx = oPtr.NextInStack;
                            string oName = oPtr.Description(true, 3);
                            outVal = $"{s1}{s2}{s3}{oName} [{info}]";
                            Gui.PrintLine(outVal, 0, 0);
                            _level.MoveCursorRelative(y, x);
                            query = Gui.Inkey();
                            if (query != '\r' && query != '\n' && query != ' ')
                            {
                                break;
                            }
                            if (query == ' ' && (mode & Constants.TargetLook) == 0)
                            {
                                break;
                            }
                            s2 = "also carrying ";
                        }
                        if (thisOIdx != 0)
                        {
                            break;
                        }
                        s2 = "on ";
                    }
                }
                for (thisOIdx = cPtr.ItemIndex; thisOIdx != 0; thisOIdx = nextOIdx)
                {
                    Item oPtr = _level.Items[thisOIdx];
                    nextOIdx = oPtr.NextInStack;
                    if (oPtr.Marked)
                    {
                        boring = false;
                        string oName = oPtr.Description(true, 3);
                        outVal = $"{s1}{s2}{s3}{oName} [{info}]";
                        Gui.PrintLine(outVal, 0, 0);
                        _level.MoveCursorRelative(y, x);
                        query = Gui.Inkey();
                        if (query != '\r' && query != '\n' && query != ' ')
                        {
                            break;
                        }
                        if (query == ' ' && (mode & Constants.TargetLook) == 0)
                        {
                            break;
                        }
                        s1 = "It is ";
                        if (oPtr.Count != 1)
                        {
                            s1 = "They are ";
                        }
                        s2 = "on ";
                    }
                }
                if (thisOIdx != 0)
                {
                    break;
                }
                string feat = string.IsNullOrEmpty(cPtr.FeatureType.AppearAs)
                    ? StaticResources.Instance.FloorTileTypes[cPtr.BackgroundFeature.AppearAs].Name
                    : StaticResources.Instance.FloorTileTypes[cPtr.FeatureType.AppearAs].Name;
                if (cPtr.TileFlags.IsClear(GridTile.PlayerMemorised) && !_level.PlayerCanSeeBold(y, x))
                {
                    feat = string.Empty;
                }
                if (boring || (!cPtr.FeatureType.IsOpenFloor))
                {
                    string name = "unknown grid";
                    if (feat != string.Empty)
                    {
                        name = StaticResources.Instance.FloorTileTypes[feat].Description;
                        if (s2 != "" && cPtr.FeatureType.BlocksLos)
                        {
                            s2 = "in ";
                        }
                    }
                    s3 = name[0].IsVowel() ? "an " : "a ";
                    if (cPtr.FeatureType.IsShop)
                    {
                        s3 = name[0].IsVowel() ? "the entrance to an " : "the entrance to a ";
                    }
                    outVal = $"{s1}{s2}{s3}{name} [{info}]";
                    Gui.PrintLine(outVal, 0, 0);
                    _level.MoveCursorRelative(y, x);
                    query = Gui.Inkey();
                    if (query != '\r' && query != '\n' && query != ' ')
                    {
                        break;
                    }
                }
            }
            while (query == '\r' || query == '\n');
            return query;
        }

        private void TargetSetPrepare(int mode)
        {
            int y;
            _level.TempN = 0;
            for (y = _level.PanelRowMin; y <= _level.PanelRowMax; y++)
            {
                int x;
                for (x = _level.PanelColMin; x <= _level.PanelColMax; x++)
                {
                    GridTile cPtr = _level.Grid[y][x];
                    if (!_level.PlayerHasLosBold(y, x))
                    {
                        continue;
                    }
                    if (!TargetSetAccept(y, x))
                    {
                        continue;
                    }
                    if ((mode & Constants.TargetKill) != 0 && !TargetAble(cPtr.MonsterIndex))
                    {
                        continue;
                    }
                    _level.TempX[_level.TempN] = x;
                    _level.TempY[_level.TempN] = y;
                    _level.TempN++;
                }
            }
            List<TargetLocation> list = new List<TargetLocation>();
            for (int i = 0; i < _level.TempN; i++)
            {
                list.Add(new TargetLocation(_level.TempY[i], _level.TempX[i],
                    _level.Distance(_level.TempY[i], _level.TempX[i], _player.MapY, _player.MapX)));
            }
            list.Sort();
            for (int i = 0; i < _level.TempN; i++)
            {
                _level.TempX[i] = list[i].X;
                _level.TempY[i] = list[i].Y;
            }
        }
    }
}