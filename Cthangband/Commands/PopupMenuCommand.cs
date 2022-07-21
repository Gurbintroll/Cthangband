using Cthangband.Terminal;
using Cthangband.UI;
using System;
using System.Collections.Generic;

namespace Cthangband.Commands
{
    /// <summary>
    /// Fire the popup menu for quitting and changing options
    /// </summary>
    [Serializable]
    internal class PopupMenuCommand : ICommand
    {
        public char Key => '\x1b';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            var menuItems = new List<string>() { "Resume Game", "Options", "Quit to Menu", "Quit to Desktop" };
            var menu = new PopupMenu(menuItems);
            var result = menu.Show();
            switch (result)
            {
                // Escape or Resume Game
                case -1:
                case 0:
                    return;
                // Options
                case 1:
                    Program.ChangeOptions();
                    Gui.SetBackground(BackgroundImage.Overhead);
                    break;
                // Quit to Menu
                case 2:
                    SaveGame.Instance.Playing = false;
                    break;
                // Quit to Desktop
                case 3:
                    SaveGame.Instance.Playing = false;
                    Program.ExitToDesktop = true;
                    break;
            }
        }
    }
}
