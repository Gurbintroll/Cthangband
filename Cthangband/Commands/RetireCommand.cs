using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Retire (if a winner) or give up (if not a winner)
    /// </summary>
    [Serializable]
    internal class RetireCommand : ICommand
    {
        public char Key => 'Q';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            // If we're a winner it's a simple question with a more positive connotation
            if (player.IsWinner)
            {
                if (!Gui.GetCheck("Do you want to retire? "))
                {
                    return;
                }
            }
            else
            {
                // If we're not a winner, only ask if we're not also a wizard - giving up a wizard
                // character doesn't need a prompt/confirmation
                if (!player.IsWizard)
                {
                    if (!Gui.GetCheck("Do you really want to give up? "))
                    {
                        return;
                    }
                    // Require a confirmation to make sure the player doesn't accidentally give up a
                    // long-running character
                    Gui.PrintLine("Type the '@' sign to give up (this character will no longer be playable): ", 0, 0);
                    int i = Gui.Inkey();
                    Gui.PrintLine("", 0, 0);
                    if (i != '@')
                    {
                        return;
                    }
                }
            }
            // Assuming whe player didn't give up, "kill" the character by quitting
            SaveGame.Instance.Playing = false;
            player.IsDead = true;
            SaveGame.Instance.DiedFrom = "quitting";
        }
    }
}
