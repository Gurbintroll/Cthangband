using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Equip an item
    /// </summary>
    [Serializable]
    internal class EquipCommand : ICommand
    {
        public char Key => 'e';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            EquipStoreCommand.DoCmdEquip(player);
        }
    }
}
