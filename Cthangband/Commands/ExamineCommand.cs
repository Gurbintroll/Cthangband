using Cthangband.StaticData;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Examine an item
    /// </summary>
    [Serializable]
    internal class ExamineCommand : ICommand, IStoreCommand
    {
        char IStoreCommand.Key => 'I';

        public bool RequiresRerendering => false;

        char ICommand.Key => 'x';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player)
        {
            // TODO: This is a deviation because the browse command didn't have the store in mind when designed.
            // We need to inject the level but ideally, the functionality can be tweaked to remove this
            // unoptimized code.
            Execute(player, SaveGame.Instance.Level);
        }

        public void Execute(Player player, Level level)
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
            Item item = itemIndex >= 0 ? player.Inventory[itemIndex] : level.Items[0 - itemIndex];
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
