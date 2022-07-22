using Cthangband.StaticData;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Examine an item from the player's inventory
    /// </summary>
    [Serializable]
    internal class ExamineInventoryStoreCommand : IStoreCommand
    {
        public char Key => 'I';

        public bool RequiresRerendering => false;

        public bool IsEnabled(Store store) => true;

        public void Execute(Player player, Store store)
        {
            DoCmdExamine(player);
        }

        public static void DoCmdExamine(Player player)
        {
            // Get the item to examine
            if (!SaveGame.Instance.GetItem(out int itemIndex, "Examine which item? ", true, true, true))
            {
                if (itemIndex == -2)
                {
                    Profile.Instance.MsgPrint("You have nothing to examine.");
                }
                return;
            }
            Item item = itemIndex >= 0 ? player.Inventory[itemIndex] : SaveGame.Instance.Level.Items[0 - itemIndex]; // TODO: Remove access to Level
            // Do we know anything about it?
            if (item.IdentifyFlags.IsClear(Constants.IdentMental))
            {
                Profile.Instance.MsgPrint("You have no special knowledge about that item.");
                return;
            }
            string itemName = item.Description(true, 3);
            Profile.Instance.MsgPrint($"Examining {itemName}...");
            // We're not actually identifying it, because it's already itentified, but we want to
            // repeat the identification text
            if (!item.IdentifyFully())
            {
                Profile.Instance.MsgPrint("You see nothing special.");
            }
        }
    }
}
