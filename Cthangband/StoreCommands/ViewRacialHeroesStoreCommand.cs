using Cthangband.Commands;
using Cthangband.Enumerations;
using Cthangband.UI;

namespace Cthangband.StoreCommands
{
    internal class ViewRacialHeroesStoreCommand : IStoreCommand
    {
        public char Key => 'v';

        public bool RequiresRerendering => false;

        public void Execute(Player player, Store store)
        {
            Gui.Save();
            Program.HiScores.RaceFilter = player.RaceIndex;
            Program.HiScores.DisplayScores(new HighScore(player));
            Program.HiScores.RaceFilter = -1;
            Gui.Load();
        }

        public bool IsEnabled(Store store)
        {
            return (store.StoreType == StoreType.StoreHall);
        }
    }
}
