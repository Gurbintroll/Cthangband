using Cthangband.Enumerations;
using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Show the player's inventory
    /// </summary>
    [Serializable]
    internal class InventoryCommand : ICommand, IStoreCommand
    {
        public char Key => 'i';

        public bool RequiresRerendering => false;

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            Execute(player);
        }

        public void Execute(Player player)
        {
            // We're not viewing equipment
            SaveGame.Instance.ViewingEquipment = false;
            Gui.Save();
            // We want to see everything
            SaveGame.Instance.ItemFilterAll = true;
            player.Inventory.ShowInven();
            SaveGame.Instance.ItemFilterAll = false;
            // Get a new command
            string outVal =
                $"Inventory: carrying {player.WeightCarried / 10}.{player.WeightCarried % 10} pounds ({player.WeightCarried * 100 / (player.AbilityScores[Ability.Strength].StrCarryingCapacity * 100 / 2)}% of capacity). Command: ";
            Gui.PrintLine(outVal, 0, 0);
            Gui.QueuedCommand = Gui.Inkey();
            Gui.Load();
            // Display details if the player wants
            if (Gui.QueuedCommand == '\x1b')
            {
                Gui.QueuedCommand = (char)0;
                SaveGame.Instance.ItemDisplayColumn = 50;
            }
            else
            {
                SaveGame.Instance.ViewingItemList = true;
            }
        }
    }
}
