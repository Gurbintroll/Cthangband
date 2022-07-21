// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.StaticData;
using Cthangband.Terminal;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace Cthangband.UI
{
    /// <summary>
    /// Static class for user interaction
    /// </summary>
    internal static class Gui
    {
        public static readonly Dictionary<Colour, Color> ColorData = new Dictionary<Colour, Color>();
        public static int CommandArgument;
        public static int CommandDirection;
        public static char CurrentCommand;

        /// <summary>
        /// Set to skip waiting for a keypress in the Inkey() function
        /// </summary>
        public static bool DoNotWaitOnInkey;

        /// <summary>
        /// Set to indicate that there is a full screen overlay covering the normally updated locations
        /// </summary>
        public static bool FullScreenOverlay;

        /// <summary>
        /// Set to indicate that the cursor should be hidden while waiting for a keypress with a
        /// full screen overlay
        /// </summary>
        public static bool HideCursorOnFullScreenInkey;

        public static bool InPopupMenu;
        public static Mixer Mixer = new Mixer();
        public static char QueuedCommand;
        private static Display _display;

        /// <summary>
        /// A buffer of artificial keypresses
        /// </summary>
        private static string _keyBuffer;

        private static string[][] _keymapAct;
        private static string _requestCommandBuffer;
        private static Terminal.Terminal _terminal;
        private static Manual.ManualViewer ManualViewer;

        /// <summary>
        /// Sets or returns whether the cursor is visible
        /// </summary>
        public static bool CursorVisible
        {
            get => _display.Scr.Cv;
            set => _display.Scr.Cv = value;
        }

        public static Terminal.Terminal Terminal
        {
            get
            {
                return _terminal;
            }
        }

        /// <summary>
        /// Prints a 'press any key' message and waits for a key press
        /// </summary>
        /// <param name="row"> The row on which to print the message </param>
        public static void AnyKey(int row)
        {
            PrintLine("", row, 0);
            Print(Colour.Orange, "[Press any key to continue]", row, 27);
            Inkey();
            PrintLine("", row, 0);
        }

        public static bool AskforAux(out string buf, string initial, int len)
        {
            buf = initial;
            char i = '\0';
            int k = 0;
            bool done = false;
            Locate(out int y, out int x);
            if (len < 1)
            {
                len = 1;
            }
            if (x < 0 || x >= Constants.ConsoleWidth)
            {
                x = 0;
            }
            if (x + len > Constants.ConsoleWidth)
            {
                len = Constants.ConsoleWidth - x;
            }
            Erase(y, x, len);
            Print(Colour.Grey, buf, y, x);
            while (!done)
            {
                Goto(y, x + k);
                i = Inkey();
                switch (i)
                {
                    case '\x1b':
                        k = 0;
                        done = true;
                        break;

                    case '\n':
                    case '\r':
                        k = buf.Length;
                        done = true;
                        break;

                    case (char)8:
                        if (k > 0)
                        {
                            k--;
                        }
                        buf = buf.Substring(0, k);
                        break;

                    default:
                        if (k < len && (char.IsLetterOrDigit(i) || i == ' ' || char.IsPunctuation(i)))
                        {
                            buf = buf.Substring(0, k) + i;
                            k++;
                        }
                        break;
                }
                Erase(y, x, len);
                Print(Colour.Black, buf, y, x);
            }
            return i != '\x1b';
        }

        /// <summary>
        /// Clears the screen from the specified row downwards
        /// </summary>
        /// <param name="row"> The first row to clear </param>
        public static void Clear(int row)
        {
            for (int y = row; y < _display.Height; y++)
            {
                Erase(y, 0, 255);
            }
        }

        /// <summary>
        /// Clears the entire screen
        /// </summary>
        public static void Clear()
        {
            int w = _display.Width;
            int h = _display.Height;
            Colour na = _display.AttrBlank;
            char nc = _display.CharBlank;
            _display.Scr.Cu = false;
            _display.Scr.Cx = 0;
            _display.Scr.Cy = 0;
            for (int y = 0; y < h; y++)
            {
                int scrAa = _display.Scr.A[y];
                int scrCc = _display.Scr.C[y];
                for (int x = 0; x < w; x++)
                {
                    _display.Scr.Va[scrAa + x] = na;
                    _display.Scr.Vc[scrCc + x] = nc;
                }
                _display.X1[y] = 0;
                _display.X2[y] = w - 1;
            }
            _display.Y1 = 0;
            _display.Y2 = h - 1;
            _display.TotalErase = true;
        }

        /// <summary>
        /// Erases a number of characters on the screen
        /// </summary>
        /// <param name="row"> The row position of the first character </param>
        /// <param name="col"> The column position of the first character </param>
        /// <param name="length"> The number of characters to erase </param>
        public static void Erase(int row, int col, int length)
        {
            int w = _display.Width;
            int x1 = -1;
            int x2 = -1;
            Colour na = _display.AttrBlank;
            char nc = _display.CharBlank;
            Goto(row, col);
            if (col + length > w)
            {
                length = w - col;
            }
            int scrAa = _display.Scr.A[row];
            int scrCc = _display.Scr.C[row];
            for (int i = 0; i < length; i++, col++)
            {
                Colour oa = _display.Scr.Va[scrAa + col];
                int oc = _display.Scr.Vc[scrCc + col];
                if (oa == na && oc == nc)
                {
                    continue;
                }
                _display.Scr.Va[scrAa + col] = na;
                _display.Scr.Vc[scrCc + col] = nc;
                if (x1 < 0)
                {
                    x1 = col;
                }
                x2 = col;
            }
            if (x1 >= 0)
            {
                if (row < _display.Y1)
                {
                    _display.Y1 = row;
                }
                if (row > _display.Y2)
                {
                    _display.Y2 = row;
                }
                if (x1 < _display.X1[row])
                {
                    _display.X1[row] = x1;
                }
                if (x2 > _display.X2[row])
                {
                    _display.X2[row] = x2;
                }
            }
        }

        public static bool GetCheck(string prompt)
        {
            int i;
            Profile.Instance.MsgPrint(null);
            string buf = $"{prompt}[Y/n]";
            PrintLine(buf, 0, 0);
            while (true)
            {
                i = Inkey();
                switch (i)
                {
                    case 'y':
                    case 'Y':
                    case 'n':
                    case 'N':
                    case 13:
                    case 27:
                        break;

                    default:
                        continue;
                }
                break;
            }
            PrintLine("", 0, 0);
            return i == 'Y' || i == 'y' || i == 13;
        }

        public static bool GetCom(string prompt, out char command)
        {
            Profile.Instance.MsgPrint(null);
            if (prompt.Length > 1)
            {
                prompt = char.ToUpper(prompt[0]) + prompt.Substring(1);
            }
            PrintLine(prompt, 0, 0);
            command = Inkey();
            PrintLine("", 0, 0);
            return command != '\x1b';
        }

        public static int GetKeymapDir(char ch)
        {
            int d = 0;
            string act = _keymapAct[Constants.KeymapModeOrig][ch];
            while (true)
            {
                if (act.Length == 0)
                {
                    return 0;
                }
                if (act[0] >= '1' && act[0] <= '9')
                {
                    break;
                }
                act = act.Remove(0, 1);
            }
            if (!string.IsNullOrEmpty(act))
            {
                if (!int.TryParse(act, out d))
                {
                    d = 0;
                }
            }
            return d;
        }

        public static int GetQuantity(string prompt, int max, bool allbydefault)
        {
            int amt;
            if (CommandArgument != 0)
            {
                amt = CommandArgument;
                CommandArgument = 0;
                if (amt > max)
                {
                    amt = max;
                }
                return amt;
            }
            if (string.IsNullOrEmpty(prompt))
            {
                string tmp = $"Quantity (1-{max}): ";
                prompt = tmp;
            }
            amt = 1;
            if (allbydefault)
            {
                amt = max;
            }
            string def = amt.ToString();
            if (!GetString(prompt, out string buf, def, 6))
            {
                return 0;
            }
            if (int.TryParse(buf, out int test))
            {
                amt = test;
            }
            if (string.IsNullOrEmpty(buf))
            {
                amt = max;
            }
            else if (char.IsLetter(buf[0]))
            {
                amt = max;
            }
            if (amt > max)
            {
                amt = max;
            }
            if (amt < 0)
            {
                amt = 0;
            }
            return amt;
        }

        public static bool GetString(string prompt, out string buf, string initial, int len)
        {
            Profile.Instance.MsgPrint(null);
            PrintLine(prompt, 0, 0);
            bool res = AskforAux(out buf, initial, len);
            PrintLine("", 0, 0);
            return res;
        }

        /// <summary>
        /// Moves the cursor and print location to a new position
        /// </summary>
        /// <param name="row"> The row at which to print </param>
        /// <param name="col"> The column at which to print </param>
        public static void Goto(int row, int col)
        {
            int w = _display.Width;
            int h = _display.Height;
            if (col < 0 || col >= w)
            {
                return;
            }
            if (row < 0 || row >= h)
            {
                return;
            }
            _display.Scr.Cx = col;
            _display.Scr.Cy = row;
            _display.Scr.Cu = false;
        }

        /// <summary>
        /// Initialises the Gui
        /// </summary>
        public static void Initialise(Settings settings)
        {
            Mixer.Initialise(settings.MusicVolume / 100.0f, settings.SoundVolume / 100.0f);
            TerminalParameters startupParameters = settings.Parameters();
            _terminal = new Terminal.Terminal(startupParameters);
            _terminal.Refresh();
            _terminal.CursorColour = Color.SkyBlue;
            ColorData.Add(Colour.Background, Color.Black);
            ColorData.Add(Colour.Black, Color.DarkSlateGray);
            ColorData.Add(Colour.Grey, Color.DimGray);
            ColorData.Add(Colour.BrightGrey, Color.DarkGray);
            ColorData.Add(Colour.Silver, Color.LightSlateGray);
            ColorData.Add(Colour.Beige, Color.Moccasin);
            ColorData.Add(Colour.BrightBeige, Color.Beige);
            ColorData.Add(Colour.White, Color.LightGray);
            ColorData.Add(Colour.BrightWhite, Color.White);
            ColorData.Add(Colour.Red, Color.DarkRed);
            ColorData.Add(Colour.BrightRed, Color.Red);
            ColorData.Add(Colour.Copper, Color.Chocolate);
            ColorData.Add(Colour.Orange, Color.OrangeRed);
            ColorData.Add(Colour.BrightOrange, Color.Orange);
            ColorData.Add(Colour.Brown, Color.SaddleBrown);
            ColorData.Add(Colour.BrightBrown, Color.BurlyWood);
            ColorData.Add(Colour.Gold, Color.Gold);
            ColorData.Add(Colour.Yellow, Color.Khaki);
            ColorData.Add(Colour.BrightYellow, Color.Yellow);
            ColorData.Add(Colour.Chartreuse, Color.YellowGreen);
            ColorData.Add(Colour.BrightChartreuse, Color.Chartreuse);
            ColorData.Add(Colour.Green, Color.DarkGreen);
            ColorData.Add(Colour.BrightGreen, Color.LimeGreen);
            ColorData.Add(Colour.Turquoise, Color.DarkTurquoise);
            ColorData.Add(Colour.BrightTurquoise, Color.Cyan);
            ColorData.Add(Colour.Blue, Color.MediumBlue);
            ColorData.Add(Colour.BrightBlue, Color.DeepSkyBlue);
            ColorData.Add(Colour.Diamond, Color.LightCyan);
            ColorData.Add(Colour.Purple, Color.Purple);
            ColorData.Add(Colour.BrightPurple, Color.Violet);
            ColorData.Add(Colour.Pink, Color.DeepPink);
            ColorData.Add(Colour.BrightPink, Color.HotPink);
            _display = new Display(Constants.ConsoleWidth, Constants.ConsoleHeight, 256);
            MapMovementKeys();
        }

        /// <summary>
        /// Returns the next keypress. The behaviour of this function is modified by other class properties
        /// </summary>
        /// <returns> The next key pressed </returns>
        public static char Inkey()
        {
            char ch = '\0';
            bool done = false;
            if (!string.IsNullOrEmpty(_keyBuffer))
            {
                ch = _keyBuffer[0];
                _keyBuffer = _keyBuffer.Remove(0, 1);
                HideCursorOnFullScreenInkey = false;
                DoNotWaitOnInkey = false;
                return ch;
            }
            _keyBuffer = null;
            bool v = CursorVisible;
            if (!DoNotWaitOnInkey && (!HideCursorOnFullScreenInkey || FullScreenOverlay))
            {
                CursorVisible = true;
            }
            if (InPopupMenu)
            {
                CursorVisible = false;
            }
            while (ch == 0)
            {
                if (DoNotWaitOnInkey && GetKeypress(out char kk, false, false))
                {
                    ch = kk;
                    break;
                }
                if (!done && GetKeypress(out _, false, false))
                {
                    Refresh();
                    done = true;
                }
                if (DoNotWaitOnInkey)
                {
                    break;
                }
                GetKeypress(out ch, true, true);
                if (ch == 29)
                {
                    ch = '\0';
                    continue;
                }
                if (ch == '`')
                {
                    ch = '\x1b';
                }
                if (ch == 30)
                {
                    ch = '\0';
                }
            }
            CursorVisible = v;
            HideCursorOnFullScreenInkey = false;
            DoNotWaitOnInkey = false;
            return ch;
        }

        /// <summary>
        /// Re-loads the saved screen to the GUI
        /// </summary>
        public static void Load()
        {
            int w = _display.Width;
            int h = _display.Height;
            if (_display.Mem == null)
            {
                _display.Mem = new Screen(w, h);
            }
            _display.Scr.Copy(_display.Mem, w, h);
            for (int y = 0; y < h; y++)
            {
                _display.X1[y] = 0;
                _display.X2[y] = w - 1;
            }
            _display.Y1 = 0;
            _display.Y2 = h - 1;
        }

        /// <summary>
        /// Pauses for a duration
        /// </summary>
        /// <param name="duration"> The duration of the pause, in milliseconds </param>
        public static void Pause(int duration)
        {
            Thread.Sleep(duration);
        }

        /// <summary>
        /// Places a character without moving the text position
        /// </summary>
        /// <param name="attr"> The colour to use for the character </param>
        /// <param name="ch"> The character to place </param>
        /// <param name="row"> The row at which to place the character </param>
        /// <param name="col"> The column at which to place the character </param>
        public static void Place(Colour attr, char ch, int row, int col)
        {
            int w = _display.Width;
            int h = _display.Height;
            if (col < 0 || col >= w)
            {
                return;
            }
            if (row < 0 || row >= h)
            {
                return;
            }
            if (ch == 0)
            {
                return;
            }
            QueueCharacter(col, row, attr, ch);
        }

        /// <summary>
        /// Plays a sound
        /// </summary>
        /// <param name="val"> The sound to play </param>
        public static void PlaySound(SoundEffect sound)
        {
            if (Mixer.SoundVolume > 0)
            {
                Mixer.Play(sound);
            }
        }

        /// <summary>
        /// Prints a character at the current cursor position
        /// </summary>
        /// <param name="attr"> The colour in which to print the character </param>
        /// <param name="ch"> The character to print </param>
        public static void Print(Colour attr, char ch)
        {
            int w = _display.Width;
            if (_display.Scr.Cu)
            {
                return;
            }
            if (ch == 0)
            {
                return;
            }
            QueueCharacter(_display.Scr.Cx, _display.Scr.Cy, attr, ch);
            _display.Scr.Cx++;
            if (_display.Scr.Cx < w)
            {
                return;
            }
            _display.Scr.Cu = true;
        }

        /// <summary>
        /// Prints a character at a given location
        /// </summary>
        /// <param name="attr"> The colour in which to print </param>
        /// <param name="ch"> The character to print </param>
        /// <param name="row"> The y position at which to print </param>
        /// <param name="col"> The x position at which to print </param>
        public static void Print(Colour attr, char ch, int row, int col)
        {
            Goto(row, col);
            Print(attr, ch);
        }

        /// <summary>
        /// Prints a string at the current location
        /// </summary>
        /// <param name="attr"> The colour in which to print the string </param>
        /// <param name="str"> The string to print </param>
        /// <param name="length"> The number of characters to print (-1 for all) </param>
        public static void Print(Colour attr, string str, int length)
        {
            if (string.IsNullOrEmpty(str))
            {
                return;
            }
            int w = _display.Width;
            int res = 0;
            int len = str.Length;
            if (_display.Scr.Cu)
            {
                return;
            }
            int k = length < 0 ? w + 1 : length;
            for (length = 0; length < k && length < len; length++)
            {
            }
            if (_display.Scr.Cx + length >= w)
            {
                res = w - _display.Scr.Cx;
                length = w - _display.Scr.Cx;
            }
            QueueCharacters(_display.Scr.Cx, _display.Scr.Cy, length, attr, str);
            _display.Scr.Cx += length;
            if (res != 0)
            {
                _display.Scr.Cu = true;
            }
        }

        /// <summary>
        /// Prints a string at a given location
        /// </summary>
        /// <param name="attr"> The colour in which to print </param>
        /// <param name="str"> The string to print </param>
        /// <param name="row"> The y position at which to print the string </param>
        /// <param name="col"> The x position at which to print the string </param>
        /// <param name="length"> The number of characters to print (-1 for all) </param>
        public static void Print(Colour attr, string str, int row, int col, int length)
        {
            Goto(row, col);
            Print(attr, str, length);
        }

        /// <summary>
        /// Prints a string at a given location
        /// </summary>
        /// <param name="str"> The string to print </param>
        /// <param name="row"> The row at which to print </param>
        /// <param name="col"> The column at which to print </param>
        public static void Print(string str, int row, int col)
        {
            Goto(row, col);
            Print(Colour.White, str, -1);
        }

        /// <summary>
        /// Prints a string
        /// </summary>
        /// <param name="attr"> The colour in which to print </param>
        /// <param name="str"> The string to print </param>
        /// <param name="row"> The row at which to print </param>
        /// <param name="col"> The column at which to print </param>
        public static void Print(Colour attr, string str, int row, int col)
        {
            Goto(row, col);
            Print(attr, str, -1);
        }

        /// <summary>
        /// Print a string, wiping the rest of the line
        /// </summary>
        /// <param name="attr"> The colour in which to print </param>
        /// <param name="str"> The string to print </param>
        /// <param name="row"> The row at which to print </param>
        /// <param name="col"> The column at which to print </param>
        public static void PrintLine(Colour attr, string str, int row, int col)
        {
            Erase(row, col, 255);
            Print(attr, str, -1);
        }

        /// <summary>
        /// Print a string, wiping to the end of the line
        /// </summary>
        /// <param name="str"> The string to print </param>
        /// <param name="row"> The row at which to print </param>
        /// <param name="col"> The column at which to print </param>
        public static void PrintLine(string str, int row, int col)
        {
            PrintLine(Colour.White, str, row, col);
        }

        /// <summary>
        /// Prints a string, word-wrapping it onto successive lines
        /// </summary>
        /// <param name="a"> The colour in which to print </param>
        /// <param name="str"> The string to print </param>
        public static void PrintWrap(Colour a, string str)
        {
            GetSize(out int w, out _);
            Locate(out int y, out int x);
            string[] split = str.Split(' ');
            for (int i = 0; i < split.Length; i++)
            {
                string s = split[i];
                if (i > 0)
                {
                    s = " " + s;
                }
                if (x + s.Length > w)
                {
                    x = 0;
                    y++;
                    if (y > 22)
                    {
                        return;
                    }
                    Erase(y, x, 255);
                }
                foreach (char c in s)
                {
                    if (c == ' ' && x == 0)
                    {
                    }
                    else if (c == '\n')
                    {
                        x = 0;
                        y++;
                        Erase(y, x, 255);
                    }
                    else if (c >= ' ')
                    {
                        Print(a, c, y, x);
                        x++;
                    }
                    else
                    {
                        Print(a, ' ', y, x);
                        x++;
                    }
                }
            }
        }

        /// <summary>
        /// Redraw the entire window
        /// </summary>
        public static void Redraw()
        {
            _display.TotalErase = true;
            Refresh();
        }

        /// <summary>
        /// Refresh the window, drawing all queued print and erase requests
        /// </summary>
        public static void Refresh()
        {
            int y;
            int w = _display.Width;
            int h = _display.Height;
            int y1 = _display.Y1;
            int y2 = _display.Y2;
            Screen old = _display.Old;
            Screen scr = _display.Scr;
            if (y1 > y2 && scr.Cu == old.Cu && scr.Cv == old.Cv && scr.Cx == old.Cx && scr.Cy == old.Cy &&
                !_display.TotalErase)
            {
                return;
            }
            if (_display.TotalErase)
            {
                Colour na = _display.AttrBlank;
                char nc = _display.CharBlank;
                _terminal.Clear();
                old.Cv = false;
                old.Cu = false;
                old.Cx = 0;
                old.Cy = 0;
                for (y = 0; y < h; y++)
                {
                    int aa = old.A[y];
                    int cc = old.C[y];
                    for (int x = 0; x < w; x++)
                    {
                        old.Va[aa++] = na;
                        old.Vc[cc++] = nc;
                    }
                }
                _display.Y1 = y1 = 0;
                _display.Y2 = y2 = h - 1;
                for (y = 0; y < h; y++)
                {
                    _display.X1[y] = 0;
                    _display.X2[y] = w - 1;
                }
                _display.TotalErase = false;
            }
            if (scr.Cu || !scr.Cv)
            {
                _terminal.CursorVisible = false;
            }
            if (y1 <= y2)
            {
                for (y = y1; y <= y2; ++y)
                {
                    int x1 = _display.X1[y];
                    int x2 = _display.X2[y];
                    if (x1 <= x2)
                    {
                        RefreshTextRow(y, x1, x2);
                        _display.X1[y] = w;
                        _display.X2[y] = 0;
                    }
                }
                _display.Y1 = h;
                _display.Y2 = 0;
            }
            {
                if (scr.Cu)
                {
                    _terminal.Goto(scr.Cy, w - 1);
                    _terminal.CursorVisible = false;
                }
                else if (!scr.Cv)
                {
                    _terminal.Goto(scr.Cy, scr.Cx);
                    _terminal.CursorVisible = false;
                }
                else
                {
                    _terminal.Goto(scr.Cy, scr.Cx);
                    _terminal.CursorVisible = true;
                }
            }
            old.Cu = scr.Cu;
            old.Cv = scr.Cv;
            old.Cx = scr.Cx;
            old.Cy = scr.Cy;
            _terminal.Refresh();
        }

        public static void RequestCommand(bool shopping)
        {
            const int mode = Constants.KeymapModeOrig;
            CurrentCommand = (char)0;
            CommandArgument = 0;
            CommandDirection = 0;
            while (true)
            {
                char cmd;
                if (QueuedCommand != 0)
                {
                    Profile.Instance.MsgPrint(null);
                    cmd = QueuedCommand;
                    QueuedCommand = (char)0;
                }
                else
                {
                    Profile.Instance.MsgFlag = false;
                    HideCursorOnFullScreenInkey = true;
                    cmd = Inkey();
                }
                PrintLine("", 0, 0);
                if (cmd == '0')
                {
                    int oldArg = CommandArgument;
                    CommandArgument = 0;
                    PrintLine("Count: ", 0, 0);
                    while (true)
                    {
                        cmd = Inkey();
                        if (cmd == 0x7F || cmd == 0x08)
                        {
                            CommandArgument /= 10;
                            PrintLine($"Count: {CommandArgument}", 0, 0);
                        }
                        else if (cmd >= '0' && cmd <= '9')
                        {
                            if (CommandArgument >= 1000)
                            {
                                CommandArgument = 9999;
                            }
                            else
                            {
                                CommandArgument = (CommandArgument * 10) + cmd.ToString().ToIntSafely();
                            }
                            PrintLine($"Count: {CommandArgument}", 0, 0);
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (CommandArgument == 0)
                    {
                        CommandArgument = 99;
                        PrintLine($"Count: {CommandArgument}", 0, 0);
                    }
                    if (oldArg != 0)
                    {
                        CommandArgument = oldArg;
                        PrintLine($"Count: {CommandArgument}", 0, 0);
                    }
                    if (cmd == ' ' || cmd == '\n' || cmd == '\r')
                    {
                        if (!GetCom("Command: ", out cmd))
                        {
                            CommandArgument = 0;
                            continue;
                        }
                    }
                }
                if (cmd == '\\')
                {
                    GetCom("Command: ", out cmd);
                    if (string.IsNullOrEmpty(_keyBuffer))
                    {
                        _keyBuffer = "";
                    }
                }
                string act = _keymapAct[mode][cmd];
                if (!string.IsNullOrEmpty(act) && _keyBuffer == null)
                {
                    _requestCommandBuffer = act;
                    _keyBuffer = _requestCommandBuffer;
                    continue;
                }
                if (cmd == 0)
                {
                    continue;
                }
                CurrentCommand = cmd;
                break;
            }
            if (shopping)
            {
                switch (CurrentCommand)
                {
                    case 'p':
                        CurrentCommand = 'g';
                        break;

                    case 'm':
                        CurrentCommand = 'g';
                        break;

                    case 's':
                        CurrentCommand = 'd';
                        break;
                }
            }
            PrintLine("", 0, 0);
        }

        /// <summary>
        /// Save the screen to memory for later re-loading
        /// </summary>
        public static void Save()
        {
            int w = _display.Width;
            int h = _display.Height;
            (_display.Mem ?? (_display.Mem = new Screen(w, h))).Copy(_display.Scr, w, h);
        }

        public static void ShowManual()
        {
            if (ManualViewer == null)
            {
                ManualViewer = new Manual.ManualViewer();
            }
            if (ManualViewer.IsDisposed)
            {
                ManualViewer = new Manual.ManualViewer();
            }
            ManualViewer.Show();
            ManualViewer.Activate();
        }

        internal static void SetBackground(BackgroundImage image)
        {
            _terminal.SetBackground(image);
        }

        /// <summary>
        /// Adds a keypress to the internal queue
        /// </summary>
        /// <param name="k"> The keypress to add </param>
        private static void EnqueueKey(char k)
        {
            if (k == 0)
            {
                return;
            }
            _display.KeyQueue[_display.KeyHead++] = k;
            if (_display.KeyHead == _display.KeySize)
            {
                _display.KeyHead = 0;
            }
        }

        /// <summary>
        /// Gets a keypress from the internal queue, waiting until one is added if necessary
        /// </summary>
        /// <param name="ch"> The next key from the queue </param>
        /// <param name="wait"> Whether to wait for a key if one isn't already available </param>
        /// <param name="take"> Whether to take the keypress out of the queue </param>
        /// <returns> True if a keypress was available, false otherwise </returns>
        private static bool GetKeypress(out char ch, bool wait, bool take)
        {
            ch = '\0';
            if (wait)
            {
                Refresh();
                while (_display.KeyHead == _display.KeyTail)
                {
                    EnqueueKey(_terminal.WaitForKey());
                }
            }
            if (_display.KeyHead == _display.KeyTail)
            {
                return false;
            }
            ch = _display.KeyQueue[_display.KeyTail];
            if (take && ++_display.KeyTail == _display.KeySize)
            {
                _display.KeyTail = 0;
            }
            return true;
        }

        /// <summary>
        /// Gets the size of the GUI display
        /// </summary>
        /// <param name="w"> The width of the display </param>
        /// <param name="h"> The height of the display </param>
        private static void GetSize(out int w, out int h)
        {
            w = _display.Width;
            h = _display.Height;
        }

        /// <summary>
        /// Retrieves the cursor location
        /// </summary>
        /// <param name="row"> The row of the cursor </param>
        /// <param name="col"> The column of the cursor </param>
        private static void Locate(out int row, out int col)
        {
            col = _display.Scr.Cx;
            row = _display.Scr.Cy;
        }

        private static void MapMovementKeys()
        {
            _keymapAct = new string[Constants.KeymapModes][];
            for (int i = 0; i < Constants.KeymapModes; i++)
            {
                _keymapAct[i] = new string[256];
                for (int j = 0; j < 256; j++)
                {
                    _keymapAct[i][j] = string.Empty;
                }
            }
            _keymapAct[0]['1'] = ";1";
            _keymapAct[0]['2'] = ";2";
            _keymapAct[0]['3'] = ";3";
            _keymapAct[0]['4'] = ";4";
            _keymapAct[0]['5'] = ",";
            _keymapAct[0]['6'] = ";6";
            _keymapAct[0]['7'] = ";7";
            _keymapAct[0]['8'] = ";8";
            _keymapAct[0]['9'] = ";9";
        }

        /// <summary>
        /// Queue up a printed character to be displayed when the screen is refreshed
        /// </summary>
        /// <param name="x"> The x location to display the character </param>
        /// <param name="y"> The y location to display the character </param>
        /// <param name="a"> The colour in which to display the character </param>
        /// <param name="c"> The character to display </param>
        private static void QueueCharacter(int x, int y, Colour a, char c)
        {
            int scrAa = _display.Scr.A[y];
            int scrCc = _display.Scr.C[y];
            Colour oa = _display.Scr.Va[scrAa + x];
            int oc = _display.Scr.Vc[scrCc + x];
            if (oa == a && oc == c)
            {
                return;
            }
            _display.Scr.Va[scrAa + x] = a;
            _display.Scr.Vc[scrCc + x] = c;
            if (y < _display.Y1)
            {
                _display.Y1 = y;
            }
            if (y > _display.Y2)
            {
                _display.Y2 = y;
            }
            if (x < _display.X1[y])
            {
                _display.X1[y] = x;
            }
            if (x > _display.X2[y])
            {
                _display.X2[y] = x;
            }
        }

        /// <summary>
        /// Queue up a printed string to be displayed when the screen is refreshed
        /// </summary>
        /// <param name="x"> The x location to display the string </param>
        /// <param name="y"> The y location to display the string </param>
        /// <param name="n"> The number of characters to display (-1 for all) </param>
        /// <param name="a"> The colour in which to display the string </param>
        /// <param name="s"> The string to print </param>
        private static void QueueCharacters(int x, int y, int n, Colour a, string s)
        {
            int x1 = -1;
            int x2 = -1;
            int scrAa = _display.Scr.A[y];
            int scrCc = _display.Scr.C[y];
            int index = 0;
            for (; n != 0 && index < s.Length; n--)
            {
                Colour oa = _display.Scr.Va[scrAa + x];
                int oc = _display.Scr.Vc[scrCc + x];
                if (oa == a && oc == s[index])
                {
                    x++;
                    index++;
                    continue;
                }
                _display.Scr.Va[scrAa + x] = a;
                _display.Scr.Vc[scrCc + x] = s[index];
                if (x1 < 0)
                {
                    x1 = x;
                }
                x2 = x;
                x++;
                index++;
            }
            if (x1 >= 0)
            {
                if (y < _display.Y1)
                {
                    _display.Y1 = y;
                }
                if (y > _display.Y2)
                {
                    _display.Y2 = y;
                }
                if (x1 < _display.X1[y])
                {
                    _display.X1[y] = x1;
                }
                if (x2 > _display.X2[y])
                {
                    _display.X2[y] = x2;
                }
            }
        }

        /// <summary>
        /// Refresh a single text row
        /// </summary>
        /// <param name="y"> The row to refresh </param>
        /// <param name="x1"> The first character to refresh </param>
        /// <param name="x2"> The last character to refresh </param>
        private static void RefreshTextRow(int y, int x1, int x2)
        {
            int oldAa = _display.Old.A[y];
            int oldCc = _display.Old.C[y];
            int scrAa = _display.Scr.A[y];
            int scrCc = _display.Scr.C[y];
            int fn = 0;
            int fx = 0;
            Colour fa = _display.AttrBlank;
            for (int x = x1; x <= x2; x++)
            {
                Colour oa = _display.Old.Va[oldAa + x];
                char oc = _display.Old.Vc[oldCc + x];
                Colour na = _display.Scr.Va[scrAa + x];
                char nc = _display.Scr.Vc[scrCc + x];
                if (na == oa && nc == oc)
                {
                    if (fn != 0)
                    {
                        _terminal.Print(y, fx, new string(_display.Scr.Vc, scrCc + fx, fn), ColorData[fa]);
                        fn = 0;
                    }
                    continue;
                }
                _display.Old.Va[oldAa + x] = na;
                _display.Old.Vc[oldCc + x] = nc;
                if (fa != na)
                {
                    if (fn != 0)
                    {
                        _terminal.Print(y, fx, new string(_display.Scr.Vc, scrCc + fx, fn), ColorData[fa]);
                        fn = 0;
                    }
                    fa = na;
                }
                if (fn++ == 0)
                {
                    fx = x;
                }
            }
            if (fn != 0)
            {
                _terminal.Print(y, fx, new string(_display.Scr.Vc, scrCc + fx, fn), ColorData[fa]);
            }
        }
    }
}