using Cthangband.Enumerations;
using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Let the player scroll through previous messages
    /// </summary>
    [Serializable]
    internal class MessagesStoreCommand : IStoreCommand
    {
        public char Key => 'P';

        public bool IsEnabled(Store store) => true;

        public bool RequiresRerendering => false;

        public void Execute(Player player, Store store)
        {
            DoCmdMessages();
        }

        public static void DoCmdMessages()
        {
            int messageNumber = Profile.Instance.MessageNum();
            int index = 0;
            int horizontalOffset = 0;
            Gui.FullScreenOverlay = true;
            Gui.Save();
            Gui.SetBackground(Terminal.BackgroundImage.Normal);
            // Infinite loop showing a page of messages from the index
            while (true)
            {
                // Clear the screen
                Gui.Clear();
                int row;
                // Print the messages
                for (row = 0; row < 40 && index + row < messageNumber; row++)
                {
                    string msg = Profile.Instance.MessageStr((short)(index + row));
                    msg = msg.Length >= horizontalOffset ? msg.Substring(horizontalOffset) : "";
                    Gui.Print(Colour.White, msg, 41 - row, 0);
                }
                // Get a command
                Gui.PrintLine($"Message Recall ({index}-{index + row - 1} of {messageNumber}), Offset {horizontalOffset}", 0, 0);
                Gui.PrintLine("[Press 'p' for older, 'n' for newer, <dir> to scroll, or ESCAPE]", 43, 0);
                int keyCode = Gui.Inkey();
                if (keyCode == '\x1b')
                {
                    // Break out of the infinite loop
                    break;
                }
                if (keyCode == '4')
                {
                    horizontalOffset = horizontalOffset >= 40 ? horizontalOffset - 40 : 0;
                    continue;
                }
                if (keyCode == '6')
                {
                    horizontalOffset += 40;
                    continue;
                }
                if (keyCode == '8' || keyCode == '\n' || keyCode == '\r')
                {
                    if (index + 1 < messageNumber)
                    {
                        index++;
                    }
                }
                if (keyCode == 'p' || keyCode == ' ')
                {
                    if (index + 40 < messageNumber)
                    {
                        index += 40;
                    }
                }
                if (keyCode == 'n')
                {
                    index = index >= 40 ? index - 40 : 0;
                }
                if (keyCode == '2')
                {
                    index = index >= 1 ? index - 1 : 0;
                }
            }
            // Tidy up after ourselves
            Gui.Load();
            Gui.FullScreenOverlay = false;
        }
    }
}
