using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Examine an item
    /// </summary>
    [Serializable]
    internal class ExamineCommand : ICommand
    {
        public char Key => 'x';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            ExamineInventoryStoreCommand.DoCmdExamine(player);
        }
    }
}
