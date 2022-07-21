using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Bash a door to open it
    /// </summary>
    [Serializable]
    internal class BashCommand : ICommand
    {
        public char Key => 'B';

        public int? Repeat => 99;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            // Assume it won't disturb us
            bool disturb = false;
            TargetEngine targetEngine = new TargetEngine(player, level);

            // Get the direction to bash
            if (targetEngine.GetDirectionNoAim(out int dir))
            {
                int y = player.MapY + level.KeypadDirectionYOffset[dir];
                int x = player.MapX + level.KeypadDirectionXOffset[dir];
                GridTile tile = level.Grid[y][x];
                // Can only bash closed doors
                if (!tile.FeatureType.IsClosedDoor)
                {
                    Profile.Instance.MsgPrint("You see nothing there to bash.");
                }
                else if (tile.MonsterIndex != 0)
                {
                    // Oops - a montser got in the way
                    SaveGame.Instance.EnergyUse = 100;
                    Profile.Instance.MsgPrint("There is a monster in the way!");
                    SaveGame.Instance.CommandEngine.PlayerAttackMonster(y, x);
                }
                else
                {
                    // Bash the door
                    disturb = SaveGame.Instance.CommandEngine.BashClosedDoor(y, x, dir);
                }
            }
            if (!disturb)
            {
                SaveGame.Instance.Disturb(false);
            }
        }
    }
}
