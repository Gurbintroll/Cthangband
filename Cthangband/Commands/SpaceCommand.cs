namespace Cthangband.Commands
{
    internal class SpaceCommand : ICommand
    {
        public char Key => ' ';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
        }
    }
}
