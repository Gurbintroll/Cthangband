using Cthangband.Enumerations;
using Cthangband.StaticData;
using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Wield/wear an item
    /// </summary>
    [Serializable]
    internal class WieldStoreCommand : IStoreCommand
    {
        public char Key => 'w';

        public bool IsEnabled(Store store) => true;

        public bool RequiresRerendering => false;

        public void Execute(Player player, Store store)
        {
            DoCmdWield(player);
        }

        public static void DoCmdWield(Player player)
        {
            string weildPhrase;
            string itemName;
            // Only interested in wearable items
            SaveGame.Instance.ItemFilter = SaveGame.Instance.CommandEngine.ItemFilterWearable;
            if (!SaveGame.Instance.GetItem(out int itemIndex, "Wear/Wield which item? ", false, true, true))
            {
                if (itemIndex == -2)
                {
                    Profile.Instance.MsgPrint("You have nothing you can wear or wield.");
                }
                return;
            }
            Item item = itemIndex >= 0 ? player.Inventory[itemIndex] : SaveGame.Instance.Level.Items[0 - itemIndex]; // TODO: Remove access to Level
            // Find the correct item slot
            int slot = player.Inventory.WieldSlot(item);
            // Can't replace a cursed item
            if (player.Inventory[slot].IsCursed())
            {
                itemName = player.Inventory[slot].Description(false, 0);
                Profile.Instance.MsgPrint($"The {itemName} you are {player.DescribeWieldLocation(slot)} appears to be cursed.");
                return;
            }
            // If we know the item to be cursed, confirm its wearing
            if (item.IsCursed() && (item.IsKnown() || item.IdentifyFlags.IsSet(Constants.IdentSense)))
            {
                itemName = item.Description(false, 0);
                string dummy = $"Really use the {itemName} {{cursed}}? ";
                if (!Gui.GetCheck(dummy))
                {
                    return;
                }
            }
            // Use some energy
            SaveGame.Instance.EnergyUse = 100;
            // Pull one item out of the item stack
            Item wornItem = new Item(item) { Count = 1 };
            // Reduce the count of the item stack accordingly
            if (itemIndex >= 0)
            {
                player.Inventory.InvenItemIncrease(itemIndex, -1);
                player.Inventory.InvenItemOptimize(itemIndex);
            }
            else
            {
                SaveGame.Instance.Level.FloorItemIncrease(0 - itemIndex, -1);
                SaveGame.Instance.Level.FloorItemOptimize(0 - itemIndex);
            }
            // Take off the old item
            item = player.Inventory[slot];
            if (item.ItemType != null)
            {
                player.Inventory.InvenTakeoff(slot, 255);
            }
            // Put the item into the wield slot
            player.Inventory[slot] = wornItem;
            // Add the weight of the item
            player.WeightCarried += wornItem.Weight;
            // Inform us what we did
            if (slot == InventorySlot.MeleeWeapon)
            {
                weildPhrase = "You are wielding";
            }
            else if (slot == InventorySlot.RangedWeapon)
            {
                weildPhrase = "You are shooting with";
            }
            else if (slot == InventorySlot.Lightsource)
            {
                weildPhrase = "Your light source is";
            }
            else if (slot == InventorySlot.Digger)
            {
                weildPhrase = "You are digging with";
            }
            else
            {
                weildPhrase = "You are wearing";
            }
            itemName = wornItem.Description(true, 3);
            Profile.Instance.MsgPrint($"{weildPhrase} {itemName} ({slot.IndexToLabel()}).");
            // Let us know if it's cursed
            if (wornItem.IsCursed())
            {
                Profile.Instance.MsgPrint("Oops! It feels deathly cold!");
                wornItem.IdentifyFlags.Set(Constants.IdentSense);
            }
            player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            player.UpdatesNeeded.Set(UpdateFlags.UpdateTorchRadius);
            player.UpdatesNeeded.Set(UpdateFlags.UpdateMana);
            player.RedrawNeeded.Set(RedrawFlag.PrEquippy);
        }
    }
}
