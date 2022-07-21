using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Open a door or chest
    /// </summary>
    [Serializable]
    internal class OpenCommand : ICommand
    {
        public char Key => 'o';

        public int? Repeat => 99;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            bool disturb = false;
            // Check if there's only one thing we can open
            MapCoordinate coord = new MapCoordinate();
            int numDoors =
                SaveGame.Instance.CommandEngine.CountClosedDoors(coord);
            int numChests = SaveGame.Instance.CommandEngine.CountChests(coord, false);
            if (numDoors != 0 || numChests != 0)
            {
                bool tooMany = (numDoors != 0 && numChests != 0) || numDoors > 1 || numChests > 1;
                if (!tooMany)
                {
                    // There's only one thing we can open, so assume we mean that thing
                    Gui.CommandDirection = level.CoordsToDir(coord.Y, coord.X);
                }
            }
            // If we don't already have a direction, prompt for one
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (targetEngine.GetDirectionNoAim(out int dir))
            {
                int y = player.MapY + level.KeypadDirectionYOffset[dir];
                int x = player.MapX + level.KeypadDirectionXOffset[dir];
                GridTile tile = level.Grid[y][x];
                int itemIndex = level.ChestCheck(y, x);
                // Make sure there is something to open in the direction we chose
                if (!tile.FeatureType.IsClosedDoor &&
                    itemIndex == 0)
                {
                    Profile.Instance.MsgPrint("You see nothing there to open.");
                }
                // Can't open something if there's a monster in the way
                else if (tile.MonsterIndex != 0)
                {
                    SaveGame.Instance.EnergyUse = 100;
                    Profile.Instance.MsgPrint("There is a monster in the way!");
                    SaveGame.Instance.CommandEngine.PlayerAttackMonster(y, x);
                }
                // Open the chest or door
                else if (itemIndex != 0)
                {
                    disturb = SaveGame.Instance.CommandEngine.OpenChest(y, x, itemIndex);
                }
                else
                {
                    disturb = SaveGame.Instance.CommandEngine.OpenDoor(y, x);
                }
            }
            if (!disturb)
            {
                SaveGame.Instance.Disturb(false);
            }
        }

    }
}
