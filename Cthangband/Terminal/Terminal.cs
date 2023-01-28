// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.StaticData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using Application = System.Windows.Forms.Application;
using Color = System.Drawing.Color;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Cthangband.Terminal
{
    /// <summary>
    /// A pseudo-terminal
    /// </summary>
    internal class Terminal
    {
        private readonly TerminalParameters _parameters;
        private Brush _cursorBrush = Brushes.Gold;
        private int _cursorCol;
        private Color _cursorColour;
        private int _cursorRow;
        private bool _cursorVisible;
        private Color _ink;
        private Brush _inkBrush = Brushes.White;
        private MainWindow _window;

        /// <summary>
        /// Opens a terminal window using the given parameters
        /// </summary>
        /// <param name="parameters"> The parameters for the terminal window </param>
        public Terminal(TerminalParameters parameters)
        {
            _parameters = parameters;
            SetWindowParameters(_parameters);
        }

        /// <summary>
        /// Sets or returns the column in which the cursor lies
        /// </summary>
        public int CursorCol
        {
            get => _cursorCol;
            set
            {
                if (_cursorVisible)
                {
                    if (CursorRow >= 0 && CursorRow < Constants.ConsoleHeight && CursorCol >= 0 && CursorCol < Constants.ConsoleWidth)
                    {
                        _window.Cells[CursorRow][CursorCol].Background = null;
                    }
                }
                _cursorCol = value;
                if (_cursorVisible)
                {
                    if (CursorRow >= 0 && CursorRow < Constants.ConsoleHeight && CursorCol >= 0 && CursorCol < Constants.ConsoleWidth)
                    {
                        _window.Cells[CursorRow][CursorCol].Background = _cursorBrush;
                    }
                }
            }
        }

        /// <summary>
        /// Sets or returns the colour of the cursor
        /// </summary>
        public Color CursorColour
        {
            get => _cursorColour;
            set
            {
                _cursorColour = value;
                _cursorBrush = new SolidColorBrush(GetColor(value));
                if (_cursorVisible)
                {
                    if (CursorRow >= 0 && CursorRow < Constants.ConsoleHeight && CursorCol >= 0 && CursorCol < Constants.ConsoleWidth)
                    {
                        _window.Cells[CursorRow][CursorCol].Background = _cursorBrush;
                    }
                }
            }
        }

        /// <summary>
        /// Sets or returns the row in which the cursor lies
        /// </summary>
        public int CursorRow
        {
            get => _cursorRow;
            set
            {
                if (_cursorVisible)
                {
                    if (CursorRow >= 0 && CursorRow < Constants.ConsoleHeight && CursorCol >= 0 && CursorCol < Constants.ConsoleWidth)
                    {
                        _window.Cells[CursorRow][CursorCol].Background = null;
                    }
                }
                _cursorRow = value;
                if (_cursorVisible)
                {
                    if (CursorRow >= 0 && CursorRow < Constants.ConsoleHeight && CursorCol >= 0 && CursorCol < Constants.ConsoleWidth)
                    {
                        _window.Cells[CursorRow][CursorCol].Background = _cursorBrush;
                    }
                }
            }
        }

        /// <summary>
        /// Sets or returns whether the cursor is visible
        /// </summary>
        public bool CursorVisible
        {
            get => _cursorVisible;
            set
            {
                _cursorVisible = value;
                if (_cursorVisible)
                {
                    if (CursorRow >= 0 && CursorRow < Constants.ConsoleHeight && CursorCol >= 0 && CursorCol < Constants.ConsoleWidth)
                    {
                        _window.Cells[CursorRow][CursorCol].Background = _cursorBrush;
                    }
                }
                else
                {
                    if (CursorRow >= 0 && CursorRow < Constants.ConsoleHeight && CursorCol >= 0 && CursorCol < Constants.ConsoleWidth)
                    {
                        _window.Cells[CursorRow][CursorCol].Background = null;
                    }
                }
            }
        }

        /// <summary>
        /// Sets or returns the colour used for printing
        /// </summary>
        public Color Ink
        {
            get => _ink;
            set
            {
                _ink = value;
                _inkBrush = new SolidColorBrush(GetColor(value));
            }
        }

        /// <summary>
        /// Clears the entire screen
        /// </summary>
        public void Clear()
        {
            foreach (var line in _window.Cells)
            {
                foreach (var textBlock in line)
                {
                    textBlock.Text = " ";
                }
            }
        }

        /// <summary>
        /// Clears a single row of the screen
        /// </summary>
        /// <param name="row"> The row to clear </param>
        public void Clear(int row)
        {
            var line = _window.Cells[row];
            foreach (var textBlock in line)
            {
                textBlock.Text = " ";
            }
        }

        /// <summary>
        /// Clears a number of rows of the screen
        /// </summary>
        /// <param name="startRow"> The first row to clear </param>
        /// <param name="endRow"> The last row to clear </param>
        public void Clear(int startRow, int endRow)
        {
            for (var i = startRow; i <= endRow; i++)
            {
                var line = _window.Cells[i];
                foreach (var textBlock in line)
                {
                    textBlock.Text = " ";
                }
            }
        }

        /// <summary>
        /// Clears any remaining queued key presses
        /// </summary>
        public void ClearKeyQueue()
        {
            _window.KeyQueue.Clear();
        }

        /// <summary>
        /// Returns a list of the names of all the fonts that are installed and can be used
        /// </summary>
        /// <returns> A list of font names </returns>
        public List<string> EnumerateFonts()
        {
            var names = new List<string>();
            var fonts = Fonts.SystemFontFamilies;
            foreach (var font in fonts)
            {
                var typeface = new Typeface(font.ToString());
                var iWidth = new FormattedText("iiii", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface,
                    12, Brushes.Black).Width;
                var wWidth = new FormattedText("WWWW", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface,
                    12, Brushes.Black).Width;
                if (Math.Abs(wWidth - iWidth) > 0.0001)
                {
                    continue;
                }
                var percentWidth = new FormattedText("%%%%", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                    typeface, 12, Brushes.Black).Width;
                if (Math.Abs(percentWidth - iWidth) > 0.0001)
                {
                    continue;
                }
                var zWidth = new FormattedText("zzzz", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface,
                    12, Brushes.Black).Width;
                if (Math.Abs(zWidth - iWidth) > 0.0001)
                {
                    continue;
                }
                var lWidth = new FormattedText("llll", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface,
                    12, Brushes.Black).Width;
                if (Math.Abs(lWidth - iWidth) > 0.0001)
                {
                    continue;
                }
                var dotWidth = new FormattedText("....", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface,
                    12, Brushes.Black).Width;
                if (Math.Abs(dotWidth - iWidth) > 0.0001)
                {
                    continue;
                }
                var underscoreWidth = new FormattedText("____", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface,
                    12, Brushes.Black).Width;
                if (Math.Abs(underscoreWidth - iWidth) > 0.0001)
                {
                    continue;
                }
                names.Add(font.ToString());
            }
            names.Sort();
            return names;
        }

        public List<Resolution> EnumerateResolutions()
        {
            var list = new List<Resolution>();
            for (var i = 0; i < 11; i++)
            {
                list.Add(new Resolution(i));
            }
            return list;
        }

        /// <summary>
        /// Moves the cursor to the specified location
        /// </summary>
        /// <param name="row"> The row on which to place the cursor </param>
        /// <param name="col"> The column on which to place the cursor </param>
        public void Goto(int row, int col)
        {
            CursorRow = row;
            CursorCol = col;
        }

        /// <summary>
        /// Prints text on screen at the current cursor position and in the current ink colour
        /// </summary>
        /// <param name="text"> The text to print </param>
        public void Print(string text)
        {
            foreach (var c in text)
            {
                if (CursorRow >= 0 && CursorRow < Constants.ConsoleHeight && CursorCol >= 0 && CursorCol < 80)
                {
                    var printable = c;
                    if (printable < 32)
                    {
                        printable = '?';
                    }
                    var t = _window.Cells[CursorRow][CursorCol];
                    t.Foreground = _inkBrush;
                    t.Text = printable.ToString();
                    CursorCol++;
                }
            }
        }

        /// <summary>
        /// Prints text at a specific location, in the ink colour
        /// </summary>
        /// <param name="row"> The row on which to print </param>
        /// <param name="col"> The column in which to start printing </param>
        /// <param name="text"> The text to print </param>
        public void Print(int row, int col, string text)
        {
            CursorRow = row;
            CursorCol = col;
            Print(text);
        }

        /// <summary>
        /// Prints text at the current cursor position in the specified colour
        /// </summary>
        /// <param name="text"> The text to print </param>
        /// <param name="colour"> The colour in which to print it </param>
        public void Print(string text, Color colour)
        {
            Ink = colour;
            Print(text);
        }

        /// <summary>
        /// Prints text at the specified location in the specified colour
        /// </summary>
        /// <param name="row"> The row on which to print </param>
        /// <param name="col"> The column in which to start printing </param>
        /// <param name="text"> The text to print </param>
        /// <param name="colour"> The colour in which to print it </param>
        public void Print(int row, int col, string text, Color colour)
        {
            CursorRow = row;
            CursorCol = col;
            Ink = colour;
            Print(text);
        }

        /// <summary>
        /// Returns the next queued key press. If the queue is empty, it returns 0 rather than
        /// waiting for a key to be pressed.
        /// </summary>
        /// <returns> The key that was pressed, or 0 if no keys pressed </returns>
        public char QueuedKey()
        {
            if (_window.KeyQueue.Count != 0)
            {
                return _window.KeyQueue.Dequeue();
            }
            return '\0';
        }

        public void Refresh()
        {
            _window.InvalidateVisual();
            Application.DoEvents();
        }

        /// <summary>
        /// Sets a new size for the terminal window
        /// </summary>
        /// <param name="fullscreen"> Whether the terminal window should take up the whole screen </param>
        /// <param name="width"> The width of the window if not full screen </param>
        /// <param name="height"> The height of the window if not full screen </param>
        public void ResizeWindow(bool fullscreen, int width, int height)
        {
            if (fullscreen)
            {
                _window.WindowStyle = WindowStyle.None;
                _window.WindowState = WindowState.Maximized;
            }
            else
            {
                _window.Width = width;
                _window.Height = height;
                _window.WindowState = WindowState.Normal;
                _window.WindowStyle = WindowStyle.SingleBorderWindow;
            }
        }

        /// <summary>
        /// Immediately sets the window to use a new font
        /// </summary>
        /// <param name="fontName"> The name of the font to use </param>
        /// <param name="fontBold"> Whether the font should be bold </param>
        /// <param name="fontItalic"> Whether the font should be italic </param>
        public void SetNewFont(string fontName, bool fontBold, bool fontItalic)
        {
            var family = new FontFamily(fontName);
            var weight = fontBold ? FontWeights.Bold : FontWeights.Normal;
            var style = fontItalic ? FontStyles.Italic : FontStyles.Normal;
            foreach (var line in _window.Cells)
            {
                foreach (var textBlock in line)
                {
                    textBlock.FontFamily = family;
                    textBlock.FontStyle = style;
                    textBlock.FontWeight = weight;
                }
            }
        }

        /// <summary>
        /// Waits for a key to be pressed, and then returns that key
        /// </summary>
        /// <returns> The key that was pressed </returns>
        public char WaitForKey()
        {
            while (_window.KeyQueue.Count == 0)
            {
                Application.DoEvents();
                if (_window.KeyQueue.Count == 0)
                {
                    System.Threading.Thread.Sleep(5);
                }
            }
            return _window.KeyQueue.Dequeue();
        }

        internal void SetBackground(BackgroundImage image)
        {
            _window.BackgroundImage = image;
        }

        private System.Windows.Media.Color GetColor(Color value)
        {
            return System.Windows.Media.Color.FromRgb(value.R, value.G, value.B);
        }

        private void SetWindowParameters(TerminalParameters parameters)
        {
            _window?.Close();
            _window = new MainWindow();
            if (parameters.Fullscreen)
            {
                _window.WindowStyle = WindowStyle.None;
                _window.WindowState = WindowState.Maximized;
            }
            else
            {
                _window.WindowState = WindowState.Normal;
                _window.WindowStyle = WindowStyle.SingleBorderWindow;
                _window.Width = parameters.WindowedWidth;
                _window.Height = parameters.WindowedHeight;
            }
            _window.Title = parameters.WindowTitle;
            _window.InitializeGrid(_parameters);
            ElementHost.EnableModelessKeyboardInterop(_window);
            _window.Visibility = Visibility.Visible;
            CursorVisible = false;
            CursorRow = 0;
            CursorCol = 0;
            Ink = Color.White;
        }
    }
}