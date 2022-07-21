using Cthangband.Enumerations;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Destroy all worthless items in your pack
    /// </summary>
    [Serializable]
    internal class DestroyAllCommand : ICommand, IStoreCommand
    {
        public char Key => 'K';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public bool RequiresRerendering => false;

        public void Execute(Player player)
        {
            int count = 0;
            // Look for worthless items
            for (int i = InventorySlot.Pack - 1; i >= 0; i--)
            {
                Item item = player.Inventory[i];
                if (item.ItemType == null)
                {
                    continue;
                }
                // Only destroy if it's stompable (i.e. worthless or marked as unwanted)
                if (!item.Stompable())
                {
                    continue;
                }
                string itemName = item.Description(true, 3);
                Profile.Instance.MsgPrint($"You destroy {itemName}.");
                count++;
                int amount = item.Count;
                player.Inventory.InvenItemIncrease(i, -amount);
                player.Inventory.InvenItemOptimize(i);
            }
            if (count == 0)
            {
                Profile.Instance.MsgPrint("You are carrying nothing worth destroying.");
                SaveGame.Instance.EnergyUse = 0;
            }
            else
            {
                // If we destroyed at least one thing, take a turn
                SaveGame.Instance.EnergyUse = 100;
            }

        }

        public void Execute(Player player, Level level)
        {
            Execute(player);
        }
    }
}
