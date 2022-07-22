using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// View the character sheet
    /// </summary>
    [Serializable]
    internal class ViewCharacterCommand : ICommand
    {
        public char Key => 'C';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            ViewCharacterStoreCommand.DoCmdViewCharacter(player);
        }
    }
}
