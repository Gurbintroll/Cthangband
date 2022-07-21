using Cthangband.Enumerations;
using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Enter a store
    /// </summary>
    [Serializable]
    internal class StoreCommand : ICommand
    {
        public char Key => '_';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            DoCmdStore(player, level);
        }

        public static void DoCmdStore(Player player, Level level)
        {
            GridTile tile = level.Grid[player.MapY][player.MapX];
            // Make sure we're actually on a shop tile
            if (!tile.FeatureType.IsShop)
            {
                Profile.Instance.MsgPrint("You see no Stores here.");
                return;
            }
            Store which = SaveGame.Instance.GetWhichStore();
            // We can't enter a house unless we own it
            if (which.StoreType == StoreType.StoreHome && player.TownWithHouse != SaveGame.Instance.CurTown.Index)
            {
                Profile.Instance.MsgPrint("The door is locked.");
                return;
            }
            // Switch from the normal game interface to the store interface
            level.ForgetLight();
            level.ForgetView();
            Gui.FullScreenOverlay = true;
            Gui.CommandArgument = 0;
            //            CommandRepeat = 0; TODO: Confirm this is not needed
            Gui.QueuedCommand = '\0';
            which.EnterStore(player);
        }
    }
}
