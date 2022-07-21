using Cthangband.Enumerations;
using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// View the character sheet
    /// </summary>
    [Serializable]
    internal class ViewCharacterCommand : ICommand, IStoreCommand
    {
        public char Key => 'C';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public bool RequiresRerendering => true;

        public void Execute(Player player)
        {
            // Save the current screen
            Gui.FullScreenOverlay = true;
            Gui.Save();
            Gui.SetBackground(Terminal.BackgroundImage.Paper);
            // Load the character viewer
            CharacterViewer characterViewer = new CharacterViewer(player);
            while (true)
            {
                characterViewer.DisplayPlayer();
                Gui.Print(Colour.Orange, "[Press 'c' to change name, or ESC]", 43, 23);
                char keyPress = Gui.Inkey();
                // Escape breaks us out of the loop
                if (keyPress == '\x1b')
                {
                    break;
                }
                // 'c' changes name
                if (keyPress == 'c' || keyPress == 'C')
                {
                    player.InputPlayerName();
                }
                Profile.Instance.MsgPrint(null);
            }
            // Restore the screen
            Gui.SetBackground(Terminal.BackgroundImage.Overhead);
            Gui.Load();
            Gui.FullScreenOverlay = false;
            player.RedrawNeeded.Set(RedrawFlag.PrWipe | RedrawFlag.PrBasic | RedrawFlag.PrExtra | RedrawFlag.PrMap |
                             RedrawFlag.PrEquippy);
            SaveGame.Instance.HandleStuff();
        }

        public void Execute(Player player, Level level)
        {
            Execute(player); ;
        }
    }
}
