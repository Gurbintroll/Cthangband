using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Show the player's inventory
    /// </summary>
    [Serializable]
    internal class InventoryCommand : ICommand
    {
        public char Key => 'i';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            InventoryStoreCommand.DoCmdInventory(player);
        }
    }
}
