using Cthangband.Enumerations;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Alter a tile in a 'sensibe' way given the tile type
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"> </exception>
    [Serializable]
    internal class AlterCommand : ICommand
    {
        public char Key => '+';

        public int? Repeat => 99;
        
        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            // Assume we won't disturb the player
            bool disturb = false;
            TargetEngine targetEngine = new TargetEngine(player, level);

            // Get the direction in which to alter something
            if (targetEngine.GetDirectionNoAim(out int dir))
            {
                int y = player.MapY + level.KeypadDirectionYOffset[dir];
                int x = player.MapX + level.KeypadDirectionXOffset[dir];
                GridTile tile = level.Grid[y][x];
                // Altering a tile will take a turn
                SaveGame.Instance.EnergyUse = 100;
                // We 'alter' a tile by attacking it
                if (tile.MonsterIndex != 0)
                {
                    SaveGame.Instance.CommandEngine.PlayerAttackMonster(y, x);
                }
                else
                {
                    // Check the action based on the type of tile
                    switch (tile.FeatureType.AlterAction)
                    {
                        case FloorTileAlterAction.Nothing:
                            Profile.Instance.MsgPrint("You're not sure what you can do with that...");
                            break;

                        case FloorTileAlterAction.Tunnel:
                            disturb = SaveGame.Instance.CommandEngine.TunnelThroughTile(y, x);
                            break;

                        case FloorTileAlterAction.Disarm:
                            disturb = SaveGame.Instance.CommandEngine.DisarmTrap(y, x, dir);
                            break;

                        case FloorTileAlterAction.Open:
                            disturb = SaveGame.Instance.CommandEngine.OpenDoor(y, x);
                            break;

                        case FloorTileAlterAction.Close:
                            disturb = SaveGame.Instance.CommandEngine.CloseDoor(y, x);
                            break;

                        case FloorTileAlterAction.Bash:
                            disturb = SaveGame.Instance.CommandEngine.BashClosedDoor(y, x, dir);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            if (!disturb)
            {
                SaveGame.Instance.Disturb(false);
            }
        }
    }
}
