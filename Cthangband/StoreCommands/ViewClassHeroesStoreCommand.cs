using Cthangband.Commands;
using Cthangband.Enumerations;
using Cthangband.UI;
using System;

namespace Cthangband.StoreCommands
{
    internal class ViewClassHeroesStoreCommand : IStoreCommand
    {
        public char Key => 'c';

        public bool RequiresRerendering => false;

        public void Execute(Player player, Store store)
        {
            Gui.Save();
            Program.HiScores.ClassFilter = player.ProfessionIndex;
            Program.HiScores.DisplayScores(new HighScore(player));
            Program.HiScores.ClassFilter = -1;
            Gui.Load();
        }

        public bool IsEnabled(Store store)
        {
            return (store.StoreType == StoreType.StoreHall);
        }
    }
}
