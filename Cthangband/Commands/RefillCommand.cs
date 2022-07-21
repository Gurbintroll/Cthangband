using Cthangband.Enumerations;
using Cthangband.StaticData;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Refill a light source with fuel
    /// </summary>
    [Serializable]
    internal class RefillCommand : ICommand
    {
        public char Key => 'F';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            int itemIndex = -999;

            // Make sure we actually have a light source to refuel
            Item lightSource = player.Inventory[InventorySlot.Lightsource];
            if (lightSource.Category != ItemCategory.Light)
            {
                Profile.Instance.MsgPrint("You are not wielding a light.");
            }
            else if (lightSource.ItemSubCategory == LightType.Lantern)
            {
                RefillLamp(itemIndex, player, level);
            }
            else if (lightSource.ItemSubCategory == LightType.Torch)
            {
                RefillTorch(itemIndex, player, level);
            }
            else
            {
                Profile.Instance.MsgPrint("Your light cannot be refilled.");
            }
        }

        /// <summary>
        /// Refill a lamp
        /// </summary>
        /// <param name="itemIndex"> The inventory index of the fuel </param>
        private void RefillLamp(int itemIndex, Player player, Level level)
        {
            // Get an item if we don't already have one
            SaveGame.Instance.ItemFilter = SaveGame.Instance.CommandEngine.ItemFilterLanternFuel;
            if (itemIndex == -999)
            {
                if (!SaveGame.Instance.GetItem(out itemIndex, "Refill with which flask? ", true, true, true))
                {
                    if (itemIndex == -2)
                    {
                        Profile.Instance.MsgPrint("You have no flasks of oil.");
                    }
                    return;
                }
            }
            Item fuelSource = itemIndex >= 0 ? player.Inventory[itemIndex] : level.Items[0 - itemIndex];
            // Make sure our item is suitable fuel
            SaveGame.Instance.ItemFilter = SaveGame.Instance.CommandEngine.ItemFilterLanternFuel;
            if (!player.Inventory.ItemMatchesFilter(fuelSource))
            {
                Profile.Instance.MsgPrint("You can't refill a lantern from that!");
                SaveGame.Instance.ItemFilter = null;
                return;
            }
            SaveGame.Instance.ItemFilter = null;
            // Refilling takes half a turn
            SaveGame.Instance.EnergyUse = 50;
            Item lamp = player.Inventory[InventorySlot.Lightsource];
            // Add the fuel
            lamp.TypeSpecificValue += fuelSource.TypeSpecificValue;
            Profile.Instance.MsgPrint("You fuel your lamp.");
            // Check for overfilling
            if (lamp.TypeSpecificValue >= Constants.FuelLamp)
            {
                lamp.TypeSpecificValue = Constants.FuelLamp;
                Profile.Instance.MsgPrint("Your lamp is full.");
            }
            // Update the inventory
            if (itemIndex >= 0)
            {
                player.Inventory.InvenItemIncrease(itemIndex, -1);
                player.Inventory.InvenItemDescribe(itemIndex);
                player.Inventory.InvenItemOptimize(itemIndex);
            }
            else
            {
                SaveGame.Instance.Level.FloorItemIncrease(0 - itemIndex, -1);
                SaveGame.Instance.Level.FloorItemDescribe(0 - itemIndex);
                SaveGame.Instance.Level.FloorItemOptimize(0 - itemIndex);
            }
            player.UpdatesNeeded.Set(UpdateFlags.UpdateTorchRadius);
        }

        /// <summary>
        /// Refill a torch from another torch
        /// </summary>
        /// <param name="itemIndex"> The inventory index of the fuel </param>
        private void RefillTorch(int itemIndex, Player player, Level level)
        {
            // Get an item if we don't already have one
            SaveGame.Instance.ItemFilter = SaveGame.Instance.CommandEngine.ItemFilterTorchFuel;
            if (itemIndex == -999)
            {
                if (!SaveGame.Instance.GetItem(out itemIndex, "Refuel with which torch? ", false, true, true))
                {
                    if (itemIndex == -2)
                    {
                        Profile.Instance.MsgPrint("You have no extra torches.");
                    }
                    return;
                }
            }
            Item fuelSource = itemIndex >= 0 ? player.Inventory[itemIndex] : level.Items[0 - itemIndex];
            // Check that our fuel is suitable
            SaveGame.Instance.ItemFilter = SaveGame.Instance.CommandEngine.ItemFilterTorchFuel;
            if (!player.Inventory.ItemMatchesFilter(fuelSource))
            {
                Profile.Instance.MsgPrint("You can't refill a torch with that!");
                SaveGame.Instance.ItemFilter = null;
                return;
            }
            SaveGame.Instance.ItemFilter = null;
            // Refueling takes half a turn
            SaveGame.Instance.EnergyUse = 50;
            Item torch = player.Inventory[InventorySlot.Lightsource];
            // Add the fuel
            torch.TypeSpecificValue += fuelSource.TypeSpecificValue + 5;
            Profile.Instance.MsgPrint("You combine the torches.");
            // Check for overfilling
            if (torch.TypeSpecificValue >= Constants.FuelTorch)
            {
                torch.TypeSpecificValue = Constants.FuelTorch;
                Profile.Instance.MsgPrint("Your torch is fully fueled.");
            }
            else
            {
                Profile.Instance.MsgPrint("Your torch glows more brightly.");
            }
            // Update the player's inventory
            if (itemIndex >= 0)
            {
                player.Inventory.InvenItemIncrease(itemIndex, -1);
                player.Inventory.InvenItemDescribe(itemIndex);
                player.Inventory.InvenItemOptimize(itemIndex);
            }
            else
            {
                SaveGame.Instance.Level.FloorItemIncrease(0 - itemIndex, -1);
                SaveGame.Instance.Level.FloorItemDescribe(0 - itemIndex);
                SaveGame.Instance.Level.FloorItemOptimize(0 - itemIndex);
            }
            player.UpdatesNeeded.Set(UpdateFlags.UpdateTorchRadius);
        }
    }
}
