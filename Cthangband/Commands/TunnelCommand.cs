using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Tunnel into the wall (or whatever is in front of us
    /// </summary>
    [Serializable]
    internal class TunnelCommand : ICommand
    {
        public char Key => 'T';

        public int? Repeat => 99;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            bool disturb = false;
            // Get the direction in which we wish to tunnel
            if (targetEngine.GetDirectionNoAim(out int dir))
            {
                // Pick up the tile that the player wishes to tunnel through
                int tileY = player.MapY + level.KeypadDirectionYOffset[dir];
                int tileX = player.MapX + level.KeypadDirectionXOffset[dir];
                GridTile tile = level.Grid[tileY][tileX];
                // Check if it can be tunneled through
                if (tile.FeatureType.IsPassable || tile.FeatureType.Name == "YellowSign")
                {
                    Profile.Instance.MsgPrint("You cannot tunnel through air.");
                }
                else if (tile.FeatureType.IsClosedDoor)
                {
                    Profile.Instance.MsgPrint("You cannot tunnel through doors.");
                }
                // Can't tunnel if there's a monster there - so attack the monster instead
                else if (tile.MonsterIndex != 0)
                {
                    SaveGame.Instance.EnergyUse = 100;
                    Profile.Instance.MsgPrint("There is a monster in the way!");
                    SaveGame.Instance.CommandEngine.PlayerAttackMonster(tileY, tileX);
                }
                else
                {
                    // Tunnel through the tile
                    disturb = SaveGame.Instance.CommandEngine.TunnelThroughTile(tileY, tileX);
                }
            }
            // Something might have disturbed us
            if (!disturb)
            {
                SaveGame.Instance.Disturb(false);
            }
        }
    }
}
