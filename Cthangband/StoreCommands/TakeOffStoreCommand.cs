using Cthangband.Enumerations;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Take off an item
    /// </summary>
    [Serializable]
    internal class TakeOffStoreCommand : IStoreCommand
    {
        public char Key => 't';

        public bool IsEnabled(Store store) => true;

        public bool RequiresRerendering => false;

        public void Execute(Player player, Store store)
        {
            DoCmdTakeOff(player);
        }

        public static void DoCmdTakeOff(Player player)
        {
            // Get the item to take off
            if (!SaveGame.Instance.GetItem(out int itemIndex, "Take off which item? ", true, false, false))
            {
                if (itemIndex == -2)
                {
                    Profile.Instance.MsgPrint("You are not wearing anything to take off.");
                }
                return;
            }
            Item item = itemIndex >= 0 ? player.Inventory[itemIndex] : SaveGame.Instance.Level.Items[0 - itemIndex]; // TODO: Remove access to Level
            // Can't take of cursed items
            if (item.IsCursed())
            {
                Profile.Instance.MsgPrint("Hmmm, it seems to be cursed.");
                return;
            }
            // Take off the item
            SaveGame.Instance.EnergyUse = 50;
            player.Inventory.InvenTakeoff(itemIndex, 255);
            player.RedrawNeeded.Set(RedrawFlag.PrEquippy);
        }
    }
}
