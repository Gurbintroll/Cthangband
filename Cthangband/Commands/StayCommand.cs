using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Stand still for a turn and pick up any items
    /// </summary>
    [Serializable]
    internal class StayAndPickupCommand : ICommand
    {
        public char Key => ',';

        public int? Repeat => null;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            StayCommand.DoCmdStay(player, level, true);
        }
    }

    /// <summary>
    /// Stand still for a turn without picking up any items
    /// </summary>
    [Serializable]
    internal class StayCommand : ICommand
    {
        public char Key => 'g';

        public int? Repeat => null;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            DoCmdStay(player, level, false);
        }

        /// <param name="pickup"> Whether or not we should pick up an object in our location </param>
        public static void DoCmdStay(Player player, Level level, bool pickup)
        {
            // Standing still takes a turn
            SaveGame.Instance.EnergyUse = 100;
            // Periodically search if we're not actively in search mode
            if (player.SkillSearchFrequency >= 50 || 0 == Program.Rng.RandomLessThan(50 - player.SkillSearchFrequency))
            {
                SaveGame.Instance.CommandEngine.Search();
            }
            // Always search if we are actively in search mode
            if (player.IsSearching)
            {
                SaveGame.Instance.CommandEngine.Search();
            }
            // Pick up items if we should
            SaveGame.Instance.CommandEngine.PickUpItems(pickup);
            // If we're in a shop doorway, enter the shop
            GridTile tile = level.Grid[player.MapY][player.MapX];
            if (tile.FeatureType.IsShop)
            {
                SaveGame.Instance.Disturb(false);
                Gui.QueuedCommand = '_';
            }
        }
    }
}
