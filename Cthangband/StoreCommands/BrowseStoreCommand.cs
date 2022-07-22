using Cthangband.StaticData;
using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Browse a book
    /// </summary>
    [Serializable]
    internal class BrowseStoreCommand : IStoreCommand
    {
        public char Key => 'b';

        public bool RequiresRerendering => false;

        public bool IsEnabled(Store store) => true;

        public void Execute(Player player, Store store)
        {
            DoCmdBrowse(player);
        }

        public static void DoCmdBrowse(Player player)
        {
            int spell;
            int spellIndex = 0;
            int[] spells = new int[64];
            // Make sure we can read
            if (player.Realm1 == 0 && player.Realm2 == 0)
            {
                Profile.Instance.MsgPrint("You cannot read books!");
                return;
            }
            // Get a book to read if we don't already have one
            Inventory.ItemFilterUseableSpellBook = true;
            if (!SaveGame.Instance.GetItem(out int itemIndex, "Browse which book? ", false, true, true))
            {
                if (itemIndex == -2)
                {
                    Profile.Instance.MsgPrint("You have no books that you can read.");
                }
                Inventory.ItemFilterUseableSpellBook = false;
                return;
            }
            Item item = itemIndex >= 0 ? player.Inventory[itemIndex] : SaveGame.Instance.Level.Items[0 - itemIndex]; // TODO: Remove access to Level
            // Check that the book is useable by the player
            Inventory.ItemFilterUseableSpellBook = true;
            if (!player.Inventory.ItemMatchesFilter(item))
            {
                Profile.Instance.MsgPrint("You can't read that.");
                Inventory.ItemFilterUseableSpellBook = false;
                return;
            }
            Inventory.ItemFilterUseableSpellBook = false;
            int bookSubCategory = item.ItemSubCategory;
            SaveGame.Instance.HandleStuff();
            // Find all spells in the book and add them to the array
            for (spell = 0; spell < 32; spell++)
            {
                if ((GlobalData.BookSpellFlags[bookSubCategory] & (1u << spell)) != 0)
                {
                    spells[spellIndex++] = spell;
                }
            }
            // Save the screen and overprint the spells in the book
            Gui.Save();
            player.PrintSpells(spells, spellIndex, 1, 20, item.Category.SpellBookToToRealm());
            Gui.PrintLine("", 0, 0);
            // Wait for a keypress and re-load the screen
            Gui.Print("[Press any key to continue]", 0, 23);
            Gui.Inkey();
            Gui.Load();
        }
    }
}
