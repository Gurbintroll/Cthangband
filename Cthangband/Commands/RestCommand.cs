using Cthangband.Enumerations;
using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Rest for either a fixed amount of time or until back to max health and mana
    /// </summary>
    [Serializable]
    internal class RestCommand : ICommand
    {
        public char Key => 'R';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            if (Gui.CommandArgument <= 0)
            {
                const string prompt = "Rest (0-9999, '*' for HP/SP, '&' as needed): ";
                if (!Gui.GetString(prompt, out string choice, "&", 4))
                {
                    return;
                }
                // Default to resting until we're fine
                if (string.IsNullOrEmpty(choice))
                {
                    choice = "&";
                }
                // -2 means rest until we're fine
                if (choice[0] == '&')
                {
                    Gui.CommandArgument = -2;
                }
                // -1 means rest until we're at full health, but don't worry about waiting for other
                // status effects to go away
                else if (choice[0] == '*')
                {
                    Gui.CommandArgument = -1;
                }
                else
                {
                    // A number means rest for that many turns
                    if (int.TryParse(choice, out int i))
                    {
                        Gui.CommandArgument = i;
                    }
                    // The player might not have put a number in - so abandon if they didn't
                    if (Gui.CommandArgument <= 0)
                    {
                        return;
                    }
                }
            }
            // Can't rest for more than 9999 turns
            if (Gui.CommandArgument > 9999)
            {
                Gui.CommandArgument = 9999;
            }
            // Resting takes at least one turn (we'll also be skipping future turns)
            SaveGame.Instance.EnergyUse = 100;
            SaveGame.Instance.Resting = Gui.CommandArgument;
            player.IsSearching = false;
            player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            player.RedrawNeeded.Set(RedrawFlag.PrState);
            SaveGame.Instance.HandleStuff();
            Gui.Refresh();
        }
    }
}
