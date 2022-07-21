using Cthangband.Enumerations;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Take off an item
    /// </summary>
    [Serializable]
    internal class TakeOffCommand : ICommand, IStoreCommand
    {
        public char Key => 't';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public bool RequiresRerendering => false;

        public void Execute(Player player, Level level)
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
            Item item = itemIndex >= 0 ? player.Inventory[itemIndex] : level.Items[0 - itemIndex];
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

        public void Execute(Player player)
        {
            // TODO: This is a deviation because the browse command didn't have the store in mind when designed.
            // We need to inject the level but ideally, the functionality can be tweaked to remove this
            // unoptimized code.
            Execute(player, SaveGame.Instance.Level);
        }
    }
}
