using Cthangband.Enumerations;
using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Drop an item
    /// </summary>
    [Serializable]
    internal class DropCommand : ICommand
    {
        public char Key => 'd';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            int amount = 1;
            // Get an item from the inventory/equipment
            if (!SaveGame.Instance.GetItem(out int itemIndex, "Drop which item? ", true, true, false))
            {
                if (itemIndex == -2)
                {
                    Profile.Instance.MsgPrint("You have nothing to drop.");
                }
                return;
            }
            Item item = itemIndex >= 0 ? player.Inventory[itemIndex] : level.Items[0 - itemIndex];
            // Can't drop a cursed item
            if (itemIndex >= InventorySlot.MeleeWeapon && item.IsCursed())
            {
                Profile.Instance.MsgPrint("Hmmm, it seems to be cursed.");
                return;
            }
            // It's a stack, so find out how many to drop
            if (item.Count > 1)
            {
                amount = Gui.GetQuantity(null, item.Count, true);
                if (amount <= 0)
                {
                    return;
                }
            }
            // Dropping things takes half a turn
            SaveGame.Instance.EnergyUse = 50;
            // Drop it
            player.Inventory.InvenDrop(itemIndex, amount);
            player.RedrawNeeded.Set(RedrawFlag.PrEquippy);
        }
    }
}