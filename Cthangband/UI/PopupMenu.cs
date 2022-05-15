// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.StaticData;
using Cthangband.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cthangband.UI
{
    internal class PopupMenu
    {
        private readonly List<string> _chosenItems;
        private readonly List<string> _items;
        private readonly int _menuWidth;
        private readonly List<string> _text;

        internal PopupMenu(List<string> items, List<string> text = null, int width = 22)
        {
            _menuWidth = width;
            _text = new List<string>();
            if (text != null)
            {
                foreach (var line in text)
                {
                    _text.Add(line.PadCenter(_menuWidth));
                }
            }
            _items = new List<string>();
            _chosenItems = new List<string>();
            foreach (var item in items)
            {
                _items.Add(item.PadCenter(_menuWidth));
                _chosenItems.Add(("* " + item + " *").PadCenter(_menuWidth));
            }
        }

        public int DisplayMenu()
        {
            var top = Constants.ConsoleHeight / 2 - (_items.Count + _text.Count) / 2;
            var left = Constants.ConsoleWidth / 2 - _menuWidth / 2;
            var topBottomBorder = "+" + new string('-', _menuWidth) + "+";
            var leftRightBorder = "|" + new string(' ', _menuWidth) + "|";
            var chosenItem = 0;
            Gui.Print(Colour.White, topBottomBorder, top, left);
            for (int i = 0; i < _text.Count + _items.Count; i++)
            {
                Gui.Print(Colour.White, leftRightBorder, top + i + 1, left);
            }
            Gui.Print(Colour.White, topBottomBorder, top + (_items.Count + _text.Count) + 1, left);
            for (int i = 0; i < _text.Count; i++)
            {
                Gui.Print(Colour.White, _text[i], top + i + 1, left + 1);
            }
            while (true)
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    if (i == chosenItem)
                    {
                        Gui.Print(Colour.BrightPurple, _chosenItems[i], top + _text.Count + i + 1, left + 1);
                    }
                    else
                    {
                        Gui.Print(Colour.White, _items[i], top + _text.Count + i + 1, left + 1);
                    }
                }
                Gui.HideCursorOnFullScreenInkey = true;
                char k = Gui.Inkey();
                switch (k)
                {
                    case '\x1b':
                        return -1;

                    case '\r':
                    case ' ':
                        return chosenItem;

                    case '2':
                        chosenItem++;
                        if (chosenItem == _items.Count)
                        {
                            chosenItem = 0;
                        }
                        break;

                    case '8':
                        chosenItem--;
                        if (chosenItem == -1)
                        {
                            chosenItem = _items.Count - 1;
                        }
                        break;
                }
            }
        }

        public int Show()
        {
            Gui.InPopupMenu = true;
            Gui.FullScreenOverlay = true;
            Gui.Save();
            var result = DisplayMenu();
            Gui.Load();
            Gui.InPopupMenu = false;
            Gui.FullScreenOverlay = false;
            return result;
        }
    }
}