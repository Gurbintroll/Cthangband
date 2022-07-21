using Cthangband.Enumerations;
using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Locate the player on the level and let them scroll the map around
    /// </summary>
    [Serializable]
    internal class LocateCommand : ICommand
    {
        public char Key => 'L';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            int startRow = level.PanelRow;
            int startCol = level.PanelCol;
            int currentRow = startRow;
            int currentCol = startCol;
            TargetEngine targetEngine = new TargetEngine(player, level);
            // Enter a loop so the player can browse the level
            while (true)
            {
                // Describe the location being viewed
                string offsetText;
                if (currentRow == startRow && currentCol == startCol)
                {
                    offsetText = "";
                }
                else
                {
                    string northSouth = currentRow < startRow ? " North" : currentRow > startRow ? " South" : "";
                    string eastWest = currentCol < startCol ? " West" : currentCol > startCol ? " East" : "";
                    offsetText = $"{northSouth}{eastWest} of";
                }
                string message = $"Map sector [{currentRow},{currentCol}], which is{offsetText} your sector. Direction?";
                // Get a direction command or escape
                int dir = 0;
                while (dir == 0)
                {
                    if (!Gui.GetCom(message, out char command))
                    {
                        break;
                    }
                    dir = Gui.GetKeymapDir(command);
                }
                if (dir == 0)
                {
                    break;
                }
                // Move the view based on the direction
                currentRow += level.KeypadDirectionYOffset[dir];
                currentCol += level.KeypadDirectionXOffset[dir];
                if (currentRow > level.MaxPanelRows)
                {
                    currentRow = level.MaxPanelRows;
                }
                else if (currentRow < 0)
                {
                    currentRow = 0;
                }
                if (currentCol > level.MaxPanelCols)
                {
                    currentCol = level.MaxPanelCols;
                }
                else if (currentCol < 0)
                {
                    currentCol = 0;
                }
                // Update the view if necessary
                if (currentRow != level.PanelRow || currentCol != level.PanelCol)
                {
                    level.PanelRow = currentRow;
                    level.PanelCol = currentCol;
                    targetEngine.PanelBounds();
                    player.UpdatesNeeded.Set(UpdateFlags.UpdateMonsters);
                    player.RedrawNeeded.Set(RedrawFlag.PrMap);
                    SaveGame.Instance.HandleStuff();
                }
            }
            // We've finished, so snap back to the player's location
            targetEngine.RecenterScreenAroundPlayer();
            player.UpdatesNeeded.Set(UpdateFlags.UpdateMonsters);
            player.RedrawNeeded.Set(RedrawFlag.PrMap);
            SaveGame.Instance.HandleStuff();
        }
    }
}
