using Cthangband.Enumerations;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Toggle whether we're automatically searching while moving
    /// </summary>
    [Serializable]
    internal class ToggleSearchCommand : ICommand
    {
        public char Key => 'S';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            if (player.IsSearching)
            {
                player.IsSearching = false;
                player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
                player.RedrawNeeded.Set(RedrawFlag.PrState);
            }
            else
            {
                player.IsSearching = true;
                player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
                player.RedrawNeeded.Set(RedrawFlag.PrState | RedrawFlag.PrSpeed);
            }
        }
    }
}
