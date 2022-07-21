using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Attempt to disarm a trap on a door or chest
    /// </summary>
    [Serializable]
    internal class DisarmCommand : ICommand
    {
        public char Key => 'D';

        public int? Repeat => 99;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            bool disturb = false;
            MapCoordinate coord = new MapCoordinate();
            int numTraps =
                SaveGame.Instance.CommandEngine.CountKnownTraps(coord);
            int numChests = SaveGame.Instance.CommandEngine.CountChests(coord, true);
            // Count the possible traps and chests we might want to disarm
            if (numTraps != 0 || numChests != 0)
            {
                bool tooMany = (numTraps != 0 && numChests != 0) || numTraps > 1 || numChests > 1;
                // If only one then we have our target
                if (!tooMany)
                {
                    Gui.CommandDirection = level.CoordsToDir(coord.Y, coord.X);
                }
            }
            // Get a direction if we don't already have one
            if (targetEngine.GetDirectionNoAim(out int dir))
            {
                int y = player.MapY + level.KeypadDirectionYOffset[dir];
                int x = player.MapX + level.KeypadDirectionXOffset[dir];
                GridTile tile = level.Grid[y][x];
                // Check for a chest
                int itemIndex = level.ChestCheck(y, x);
                if (!tile.FeatureType.IsTrap &&
                    itemIndex == 0)
                {
                    Profile.Instance.MsgPrint("You see nothing there to disarm.");
                }
                // Can't disarm with a monster in the way
                else if (tile.MonsterIndex != 0)
                {
                    Profile.Instance.MsgPrint("There is a monster in the way!");
                    SaveGame.Instance.CommandEngine.PlayerAttackMonster(y, x);
                }
                // Disarm the chest or trap
                else if (itemIndex != 0)
                {
                    disturb = SaveGame.Instance.CommandEngine.DisarmChest(y, x, itemIndex);
                }
                else
                {
                    disturb = SaveGame.Instance.CommandEngine.DisarmTrap(y, x, dir);
                }
            }
            if (!disturb)
            {
                SaveGame.Instance.Disturb(false);
            }
        }
    }
}
