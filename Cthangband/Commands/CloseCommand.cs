using Cthangband.Enumerations;
using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Close a door
    /// </summary>
    [Serializable]
    internal class CloseCommand : ICommand
    {
        public char Key => 'c';

        public int? Repeat => 99;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            MapCoordinate coord = new MapCoordinate();
            bool disturb = false;
            // If there's only one door, assume we mean that one and don't ask for a direction
            if (SaveGame.Instance.CommandEngine.CountOpenDoors(coord) == 1)
            {
                Gui.CommandDirection = level.CoordsToDir(coord.Y, coord.X);
            }
            // Get the location to close
            if (targetEngine.GetDirectionNoAim(out int dir))
            {
                int y = player.MapY + level.KeypadDirectionYOffset[dir];
                int x = player.MapX + level.KeypadDirectionXOffset[dir];
                GridTile tile = level.Grid[y][x];
                // Can only close actual open doors
                if (tile.FeatureType.Category != FloorTileTypeCategory.OpenDoorway)
                {
                    Profile.Instance.MsgPrint("You see nothing there to close.");
                }
                // Can't close if there's a monster in the way
                else if (tile.MonsterIndex != 0)
                {
                    SaveGame.Instance.EnergyUse = 100;
                    Profile.Instance.MsgPrint("There is a monster in the way!");
                    SaveGame.Instance.CommandEngine.PlayerAttackMonster(y, x);
                }
                // Actually close the door
                else
                {
                    disturb = SaveGame.Instance.CommandEngine.CloseDoor(y, x);
                }
            }
            if (!disturb)
            {
                SaveGame.Instance.Disturb(false);
            }
        }
    }
}
