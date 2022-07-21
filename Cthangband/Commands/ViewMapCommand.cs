using Cthangband.Enumerations;
using Cthangband.Terminal;
using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Display a map of the area on screen
    /// </summary>
    [Serializable]
    internal class ViewMapCommand : ICommand
    {
        public char Key => 'M';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            int cy = -1;
            int cx = -1;
            Gui.FullScreenOverlay = true;
            Gui.Save();
            Gui.Clear();
            // If we're on the surface, display the island map
            if (SaveGame.Instance.CurrentDepth == 0)
            {
                Gui.SetBackground(BackgroundImage.WildMap);
                SaveGame.Instance.DisplayWildMap();
            }
            else
            {
                // We're not on the surface, so draw the level map
                Gui.SetBackground(BackgroundImage.Map);
                level.DisplayMap(out cy, out cx);
            }
            // Give us a prompt, and display the cursor in the player's location
            Gui.Print(Colour.Orange, "[Press any key to continue]", 43, 26);
            if (SaveGame.Instance.CurrentDepth == 0)
            {
                Gui.Goto(player.WildernessY + 2, player.WildernessX + 2);
            }
            else
            {
                Gui.Goto(cy, cx);
            }
            // Wait for a keypress, and restore the screen (looking at the map takes no time)
            Gui.Inkey();
            Gui.Load();
            Gui.FullScreenOverlay = false;
            Gui.SetBackground(BackgroundImage.Overhead);
        }
    }
}
