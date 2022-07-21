using Cthangband.Enumerations;
using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Equip an item
    /// </summary>
    [Serializable]
    internal class EquipCommand : ICommand, IStoreCommand
    {
        public char Key => 'e';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public bool RequiresRerendering => false;

        public void Execute(Player player)
        {
            // We're viewing equipment
            SaveGame.Instance.ViewingEquipment = true;
            Gui.Save();
            // We're interested in seeing everything
            SaveGame.Instance.ItemFilterAll = true;
            player.Inventory.ShowEquip();
            SaveGame.Instance.ItemFilterAll = false;
            // Get a command
            string outVal =
                $"Equipment: carrying {player.WeightCarried / 10}.{player.WeightCarried % 10} pounds ({player.WeightCarried * 100 / (player.AbilityScores[Ability.Strength].StrCarryingCapacity * 100 / 2)}% of capacity). Command: ";
            Gui.PrintLine(outVal, 0, 0);
            Gui.QueuedCommand = Gui.Inkey();
            Gui.Load();
            // Display details if the player wants
            if (Gui.QueuedCommand == '\x1b')
            {
                Gui.QueuedCommand = (char)0;
                SaveGame.Instance.ItemDisplayColumn = 50;
            }
            else
            {
                SaveGame.Instance.ViewingItemList = true;
            }
        }

        public void Execute(Player player, Level level)
        {
            Execute(player);
        }
    }
}
