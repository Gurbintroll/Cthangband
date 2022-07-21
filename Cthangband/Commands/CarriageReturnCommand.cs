namespace Cthangband.Commands
{
    internal class CarriageReturnCommand : ICommand
    {
        public char Key => '\r';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
        }
    }
}
