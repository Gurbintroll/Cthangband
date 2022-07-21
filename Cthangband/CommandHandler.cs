// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Commands;
using Cthangband.UI;
using System;

namespace Cthangband
{
    /// <summary>
    /// Class for handling player commands - simple commands are handled directly and complex
    /// commands use routines from the command engine
    /// </summary>
    [Serializable]
    internal class CommandHandler
    {
        public int CommandRepeat;
        public Level Level;
        public Player Player;

        /// <summary>
        /// Process the player's latest command
        /// </summary>
        public void ProcessCommand(bool isRepeated)
        {
            // Get the current command
            char c = Gui.CurrentCommand;

            // Process commands
            foreach (ICommand command in CommandManager.GameCommands)
            {
                // TODO: The IF statement below can be converted into a dictionary with the applicable object 
                // attached for improved performance.
                if (command.IsEnabled && command.Key == c)
                {
                    command.Execute(Player, Level);

                    // Apply the default repeat value.  This handles the 0, for no repeat and default repeat count (TBDocs+ ... count = 99).
                    if (!isRepeated && command.Repeat.HasValue)
                    {
                        // Only apply the default once.
                        Gui.CommandArgument = command.Repeat.Value;
                    }

                    if (Gui.CommandArgument > 0)
                    {
                        CommandRepeat = Gui.CommandArgument - 1;
                        Player.RedrawNeeded.Set(RedrawFlag.PrState);
                        Gui.CommandArgument = 0;
                    }

                    // The command was processed.  Skip the SWITCH statement.
                    return;
                }
            }
            Gui.PrintLine("Type '?' for a list of commands.", 0, 0);
        }
    }
}