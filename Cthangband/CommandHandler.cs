// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Spells;
using Cthangband.StaticData;
using Cthangband.UI;
using System;
using System.Reflection;

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
        /// Browse a book
        /// </summary>
        /// <param name="itemIndex"> </param>
        public void DoCmdBrowse(int itemIndex)
        {
            int spell;
            int spellIndex = 0;
            int[] spells = new int[64];
            // Make sure we can read
            if (Player.Realm1 == 0 && Player.Realm2 == 0)
            {
                Profile.Instance.MsgPrint("You cannot read books!");
                return;
            }
            // Get a book to read if we don't already have one
            Inventory.ItemFilterUseableSpellBook = true;
            if (itemIndex < 0)
            {
                if (!SaveGame.Instance.GetItem(out itemIndex, "Browse which book? ", false, true, true))
                {
                    if (itemIndex == -2)
                    {
                        Profile.Instance.MsgPrint("You have no books that you can read.");
                    }
                    Inventory.ItemFilterUseableSpellBook = false;
                    return;
                }
            }
            Item item = itemIndex >= 0 ? Player.Inventory[itemIndex] : Level.Items[0 - itemIndex];
            // Check that the book is useable by the player
            Inventory.ItemFilterUseableSpellBook = true;
            if (!Player.Inventory.ItemMatchesFilter(item))
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
            Player.PrintSpells(spells, spellIndex, 1, 20, item.Category.SpellBookToToRealm());
            Gui.PrintLine("", 0, 0);
            // Wait for a keypress and re-load the screen
            Gui.Print("[Press any key to continue]", 0, 23);
            Gui.Inkey();
            Gui.Load();
        }

        /// <summary>
        /// Destroy a single item
        /// </summary>
        public void DoCmdDestroy()
        {
            int amount = 1;
            bool force = Gui.CommandArgument > 0;
            // Get an item to destroy
            if (!SaveGame.Instance.GetItem(out int itemIndex, "Destroy which item? ", false, true, true))
            {
                if (itemIndex == -2)
                {
                    Profile.Instance.MsgPrint("You have nothing to destroy.");
                }
                return;
            }
            Item item = itemIndex >= 0 ? Player.Inventory[itemIndex] : Level.Items[0 - itemIndex];
            // If we have more than one we might not want to destroy all of them
            if (item.Count > 1)
            {
                amount = Gui.GetQuantity(null, item.Count, true);
                if (amount <= 0)
                {
                    return;
                }
            }
            int oldNumber = item.Count;
            item.Count = amount;
            string itemName = item.Description(true, 3);
            item.Count = oldNumber;
            //Only confirm if it's not a worthless item
            if (!force)
            {
                if (!item.Stompable())
                {
                    string outVal = $"Really destroy {itemName}? ";
                    if (!Gui.GetCheck(outVal))
                    {
                        return;
                    }
                    // If it was something we might want to destroy again, ask
                    if (!item.ItemType.HasQuality() && item.ItemType.Category != ItemCategory.Chest)
                    {
                        if (item.IsKnown())
                        {
                            if (Gui.GetCheck($"Always destroy {itemName}?"))
                            {
                                item.ItemType.Stompable[0] = true;
                            }
                        }
                    }
                }
            }
            // Destroying something takes a turn
            SaveGame.Instance.EnergyUse = 100;
            // Can't destroy an artifact artifact
            if (item.IsFixedArtifact() || !string.IsNullOrEmpty(item.RandartName))
            {
                string feel = "special";
                SaveGame.Instance.EnergyUse = 0;
                Profile.Instance.MsgPrint($"You cannot destroy {itemName}.");
                if (item.IsCursed() || item.IsBroken())
                {
                    feel = "terrible";
                }
                item.Inscription = feel;
                item.IdentifyFlags.Set(Constants.IdentSense);
                Player.NoticeFlags |= Constants.PnCombine;
                Player.RedrawNeeded.Set(RedrawFlag.PrEquippy);
                return;
            }
            Profile.Instance.MsgPrint($"You destroy {itemName}.");
            // Warriors and paladins get experience for destroying magic books
            if (SaveGame.Instance.CommandEngine.ItemFilterHighLevelBook(item))
            {
                bool gainExpr = false;
                if (Player.ProfessionIndex == CharacterClass.Warrior)
                {
                    gainExpr = true;
                }
                else if (Player.ProfessionIndex == CharacterClass.Paladin)
                {
                    if (Player.Realm1 == Realm.Life)
                    {
                        if (item.Category == ItemCategory.DeathBook)
                        {
                            gainExpr = true;
                        }
                    }
                    else
                    {
                        if (item.Category == ItemCategory.LifeBook)
                        {
                            gainExpr = true;
                        }
                    }
                }
                if (gainExpr && Player.ExperiencePoints < Constants.PyMaxExp)
                {
                    int testerExp = Player.MaxExperienceGained / 20;
                    if (testerExp > 10000)
                    {
                        testerExp = 10000;
                    }
                    if (item.ItemSubCategory < 3)
                    {
                        testerExp /= 4;
                    }
                    if (testerExp < 1)
                    {
                        testerExp = 1;
                    }
                    Profile.Instance.MsgPrint("You feel more experienced.");
                    Player.GainExperience(testerExp * amount);
                }
            }
            // Tidy up the player's inventory
            if (itemIndex >= 0)
            {
                Player.Inventory.InvenItemIncrease(itemIndex, -amount);
                Player.Inventory.InvenItemDescribe(itemIndex);
                Player.Inventory.InvenItemOptimize(itemIndex);
            }
            else
            {
                SaveGame.Instance.Level.FloorItemIncrease(0 - itemIndex, -amount);
                SaveGame.Instance.Level.FloorItemDescribe(0 - itemIndex);
                SaveGame.Instance.Level.FloorItemOptimize(0 - itemIndex);
            }
        }

        /// <summary>
        /// Destroy all worthless items in your pack
        /// </summary>
        public void DoCmdDestroyAll()
        {
            int count = 0;
            // Look for worthless items
            for (int i = InventorySlot.Pack - 1; i >= 0; i--)
            {
                Item item = Player.Inventory[i];
                if (item.ItemType == null)
                {
                    continue;
                }
                // Only destroy if it's stompable (i.e. worthless or marked as unwanted)
                if (!item.Stompable())
                {
                    continue;
                }
                string itemName = item.Description(true, 3);
                Profile.Instance.MsgPrint($"You destroy {itemName}.");
                count++;
                int amount = item.Count;
                Player.Inventory.InvenItemIncrease(i, -amount);
                Player.Inventory.InvenItemOptimize(i);
            }
            if (count == 0)
            {
                Profile.Instance.MsgPrint("You are carrying nothing worth destroying.");
                SaveGame.Instance.EnergyUse = 0;
            }
            else
            {
                // If we destroyed at least one thing, take a turn
                SaveGame.Instance.EnergyUse = 100;
            }
        }

        /// <summary>
        /// Equip an item
        /// </summary>
        public void DoCmdEquip()
        {
            // We're viewing equipment
            SaveGame.Instance.ViewingEquipment = true;
            Gui.Save();
            // We're interested in seeing everything
            SaveGame.Instance.ItemFilterAll = true;
            Player.Inventory.ShowEquip();
            SaveGame.Instance.ItemFilterAll = false;
            // Get a command
            string outVal =
                $"Equipment: carrying {Player.WeightCarried / 10}.{Player.WeightCarried % 10} pounds ({Player.WeightCarried * 100 / (Player.AbilityScores[Ability.Strength].StrCarryingCapacity * 100 / 2)}% of capacity). Command: ";
            Gui.PrintLine(outVal, 0, 0);
            Gui.QueuedCommand = Gui.Inkey();
            Gui.Load();
            // Display details if the player wants
            if (Gui.QueuedCommand == '\x1b')
            {
                Gui.QueuedCommand = (char)0;
                SaveGame.Instance.ItemDisplayColumn = 50;
            }
            else
            {
                SaveGame.Instance.ViewingItemList = true;
            }
        }

        /// <summary>
        /// Examine an item
        /// </summary>
        public void DoCmdExamine()
        {
            // Get the item to examine
            if (!SaveGame.Instance.GetItem(out int itemIndex, "Examine which item? ", true, true, true))
            {
                if (itemIndex == -2)
                {
                    Profile.Instance.MsgPrint("You have nothing to examine.");
                }
                return;
            }
            Item item = itemIndex >= 0 ? Player.Inventory[itemIndex] : Level.Items[0 - itemIndex];
            // Do we know anything about it?
            if (item.IdentifyFlags.IsClear(Constants.IdentMental))
            {
                Profile.Instance.MsgPrint("You have no special knowledge about that item.");
                return;
            }
            string itemName = item.Description(true, 3);
            Profile.Instance.MsgPrint($"Examining {itemName}...");
            // We're not actually identifying it, because it's already itentified, but we want to
            // repeat the identification text
            if (!item.IdentifyFully())
            {
                Profile.Instance.MsgPrint("You see nothing special.");
            }
        }

        /// <summary>
        /// Repeat the level feeling for the player
        /// </summary>
        /// <param name="feelingOnly">
        /// True to show the feeling only, false to also say where we are
        /// </param>
        public void DoCmdFeeling(bool feelingOnly)
        {
            // Some sanity checks
            if (Level.DangerFeeling < 0)
            {
                Level.DangerFeeling = 0;
            }
            if (Level.DangerFeeling > 10)
            {
                Level.DangerFeeling = 10;
            }
            if (Level.TreasureFeeling < 0)
            {
                Level.TreasureFeeling = 0;
            }
            if (Level.TreasureFeeling > 10)
            {
                Level.TreasureFeeling = 10;
            }
            if (SaveGame.Instance.DunLevel <= 0)
            {
                // If we need to say where we are, do so
                if (!feelingOnly)
                {
                    if (SaveGame.Instance.Wilderness[Player.WildernessY][Player.WildernessX].Town != null)
                    {
                        Profile.Instance.MsgPrint($"You are in {SaveGame.Instance.CurTown.Name}.");
                    }
                    else if (SaveGame.Instance.Wilderness[Player.WildernessY][Player.WildernessX].Dungeon != null)
                    {
                        Profile.Instance.MsgPrint(
                            $"You are outside {SaveGame.Instance.Wilderness[Player.WildernessY][Player.WildernessX].Dungeon.Name}.");
                    }
                    else
                    {
                        Profile.Instance.MsgPrint("You are wandering around outside.");
                    }
                }
                // If we're not in a dungeon, there's no feeling to be had
                return;
            }
            // If we need to say where we are, do so
            if (!feelingOnly)
            {
                Profile.Instance.MsgPrint($"You are in {SaveGame.Instance.CurDungeon.Name}.");
                if (SaveGame.Instance.Quests.IsQuest(SaveGame.Instance.DunLevel))
                {
                    SaveGame.Instance.Quests.PrintQuestMessage();
                }
            }
            // Special feeling overrides the normal two-part feeling
            if (Level.DangerFeeling == 1 || Level.TreasureFeeling == 1)
            {
                string message = GlobalData.DangerFeelingText[1];
                Profile.Instance.MsgPrint(Player.GameTime.LevelFeel
                    ? message : GlobalData.DangerFeelingText[0]);
            }
            else
            {
                // Make the two-part feeling make a bit more sense by using the correct conjunction
                string conjunction = ", and ";
                if ((Level.DangerFeeling > 5 && Level.TreasureFeeling < 6) || (Level.DangerFeeling < 6 && Level.TreasureFeeling > 5))
                {
                    conjunction = ", but ";
                }
                string message = GlobalData.DangerFeelingText[Level.DangerFeeling] + conjunction + GlobalData.TreasureFeelingText[Level.TreasureFeeling];
                Profile.Instance.MsgPrint(Player.GameTime.LevelFeel
                    ? message : GlobalData.DangerFeelingText[0]);
            }
        }

        /// <summary>
        /// Show the player's inventory
        /// </summary>
        public void DoCmdInven()
        {
            // We're not viewing equipment
            SaveGame.Instance.ViewingEquipment = false;
            Gui.Save();
            // We want to see everything
            SaveGame.Instance.ItemFilterAll = true;
            Player.Inventory.ShowInven();
            SaveGame.Instance.ItemFilterAll = false;
            // Get a new command
            string outVal =
                $"Inventory: carrying {Player.WeightCarried / 10}.{Player.WeightCarried % 10} pounds ({Player.WeightCarried * 100 / (Player.AbilityScores[Ability.Strength].StrCarryingCapacity * 100 / 2)}% of capacity). Command: ";
            Gui.PrintLine(outVal, 0, 0);
            Gui.QueuedCommand = Gui.Inkey();
            Gui.Load();
            // Display details if the player wants
            if (Gui.QueuedCommand == '\x1b')
            {
                Gui.QueuedCommand = (char)0;
                SaveGame.Instance.ItemDisplayColumn = 50;
            }
            else
            {
                SaveGame.Instance.ViewingItemList = true;
            }
        }

        /// <summary>
        /// Look in the player's journal for any one of a number of different reasons
        /// </summary>
        public void DoCmdJournal()
        {
            // Let the journal itself handle it from here
            Journal journal = new Journal(Player);
            journal.ShowMenu();
        }

        /// <summary>
        /// Show the game manual
        /// </summary>
        public void DoCmdManual()
        {
            using (Manual.ManualViewer manual = new Manual.ManualViewer())
            {
                manual.ShowDialog();
            }
        }

        /// <summary>
        /// Show the previous message
        /// </summary>
        public void DoCmdMessageOne()
        {
            Gui.PrintLine($"> {Profile.Instance.MessageStr(0)}", 0, 0);
        }

        /// <summary>
        /// Let the player scroll through previous messages
        /// </summary>
        public void DoCmdMessages()
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

        /// <summary>
        /// Show the player what a particular symbol represents
        /// </summary>
        public void DoCmdQuerySymbol()
        {
            int index;
            // Get the symbol
            if (!Gui.GetCom("Enter character to be identified: ", out char symbol))
            {
                return;
            }
            // Run through the identification array till we find the symbol
            for (index = 0; GlobalData.SymbolIdentification[index] != null; ++index)
            {
                if (symbol == GlobalData.SymbolIdentification[index][0])
                {
                    break;
                }
            }
            // Display the symbol and its idenfitication
            string buf = GlobalData.SymbolIdentification[index] != null
                ? $"{symbol} - {GlobalData.SymbolIdentification[index].Substring(2)}."
                : $"{symbol} - Unknown Symbol";
            Profile.Instance.MsgPrint(buf);
        }

        /// <summary>
        /// Enter a store
        /// </summary>
        public void DoCmdStore()
        {
            GridTile tile = Level.Grid[Player.MapY][Player.MapX];
            // Make sure we're actually on a shop tile
            if (!tile.FeatureType.IsShop)
            {
                Profile.Instance.MsgPrint("You see no Stores here.");
                return;
            }
            Store which = SaveGame.Instance.GetWhichStore();
            // We can't enter a house unless we own it
            if (which.StoreType == StoreType.StoreHome && Player.TownWithHouse != SaveGame.Instance.CurTown.Index)
            {
                Profile.Instance.MsgPrint("The door is locked.");
                return;
            }
            // Switch from the normal game interface to the store interface
            Level.ForgetLight();
            Level.ForgetView();
            Gui.FullScreenOverlay = true;
            Gui.CommandArgument = 0;
            CommandRepeat = 0;
            Gui.QueuedCommand = '\0';
            which.EnterStore(Player, this);
        }

        /// <summary>
        /// Study a book to learn spells from it
        /// </summary>
        public void DoCmdStudy()
        {
            string spellType = Player.Spellcasting.Type == CastingType.Arcane ? "spell" : "prayer";
            // If we don't have a realm then we can't do anything
            if (Player.Realm1 == 0)
            {
                Profile.Instance.MsgPrint("You cannot read books!");
                return;
            }
            // We can't learn spells if we're blind or confused
            if (Player.TimedBlindness != 0)
            {
                Profile.Instance.MsgPrint("You cannot see!");
                return;
            }
            if (Player.TimedConfusion != 0)
            {
                Profile.Instance.MsgPrint("You are too confused!");
                return;
            }
            // We can only learn new spells if we have spare slots
            if (Player.SpareSpellSlots == 0)
            {
                Profile.Instance.MsgPrint($"You cannot learn any new {spellType}s!");
                return;
            }
            string plural = Player.SpareSpellSlots == 1 ? "" : "s";
            Profile.Instance.MsgPrint($"You can learn {Player.SpareSpellSlots} new {spellType}{plural}.");
            Profile.Instance.MsgPrint(null);
            // Get the spell books we have
            Inventory.ItemFilterUseableSpellBook = true;
            if (!SaveGame.Instance.GetItem(out int itemIndex, "Study which book? ", false, true, true))
            {
                if (itemIndex == -2)
                {
                    Profile.Instance.MsgPrint("You have no books that you can read.");
                }
                Inventory.ItemFilterUseableSpellBook = false;
                return;
            }
            Inventory.ItemFilterUseableSpellBook = false;
            // Check each book
            Item item = itemIndex >= 0 ? Player.Inventory[itemIndex] : Level.Items[0 - itemIndex];
            int itemSubCategory = item.ItemSubCategory;
            bool useSetTwo = item.Category == Player.Realm2.ToSpellBookItemCategory();
            SaveGame.Instance.HandleStuff();
            int spellIndex;
            // Arcane casters can choose their spell
            if (Player.Spellcasting.Type != CastingType.Divine)
            {
                CastingHandler castingHandler = new CastingHandler(SaveGame.Instance.Player, SaveGame.Instance.Level);
                if (!castingHandler.GetSpell(out spellIndex, "study", itemSubCategory, false, useSetTwo) && spellIndex == -1)
                {
                    return;
                }
            }
            else
            {
                // We need to choose a spell at random
                int k = 0;
                int gift = -1;
                // Gather the potential spells from the book
                for (spellIndex = 0; spellIndex < 32; spellIndex++)
                {
                    if ((GlobalData.BookSpellFlags[itemSubCategory] & (1u << spellIndex)) != 0)
                    {
                        if (!Player.SpellOkay(spellIndex, false, useSetTwo))
                        {
                            continue;
                        }
                        k++;
                        if (Program.Rng.RandomLessThan(k) == 0)
                        {
                            gift = spellIndex;
                        }
                    }
                }
                spellIndex = gift;
            }
            // If we failed to get a spell, return
            if (spellIndex < 0)
            {
                Profile.Instance.MsgPrint($"You cannot learn any {spellType}s from that book.");
                return;
            }
            // Learning a spell takes a turn (although that's not very relevant)
            SaveGame.Instance.EnergyUse = 100;
            // Mark the spell as learned
            Spell spell = useSetTwo ? Player.Spellcasting.Spells[1][spellIndex] : Player.Spellcasting.Spells[0][spellIndex];
            spell.Learned = true;
            int i;
            // Mark the spell as the last spell learned, in case we need to start forgetting them
            for (i = 0; i < 64; i++)
            {
                if (Player.Spellcasting.SpellOrder[i] == 99)
                {
                    break;
                }
            }
            Player.Spellcasting.SpellOrder[i] = spellIndex;
            // Let the player know they've learned a spell
            Profile.Instance.MsgPrint($"You have learned the {spellType} of {spell.Name}.");
            Gui.PlaySound(SoundEffect.Study);
            Player.SpareSpellSlots--;
            if (Player.SpareSpellSlots != 0)
            {
                plural = Player.SpareSpellSlots != 1 ? "s" : "";
                Profile.Instance.MsgPrint($"You can learn {Player.SpareSpellSlots} more {spellType}{plural}.");
            }
            Player.OldSpareSpellSlots = Player.SpareSpellSlots;
            Player.RedrawNeeded.Set(RedrawFlag.PrStudy);
        }

        /// <summary>
        /// Take off an item
        /// </summary>
        public void DoCmdTakeoff()
        {
            // Get the item to take off
            if (!SaveGame.Instance.GetItem(out int itemIndex, "Take off which item? ", true, false, false))
            {
                if (itemIndex == -2)
                {
                    Profile.Instance.MsgPrint("You are not wearing anything to take off.");
                }
                return;
            }
            Item item = itemIndex >= 0 ? Player.Inventory[itemIndex] : Level.Items[0 - itemIndex];
            // Can't take of cursed items
            if (item.IsCursed())
            {
                Profile.Instance.MsgPrint("Hmmm, it seems to be cursed.");
                return;
            }
            // Take off the item
            SaveGame.Instance.EnergyUse = 50;
            Player.Inventory.InvenTakeoff(itemIndex, 255);
            Player.RedrawNeeded.Set(RedrawFlag.PrEquippy);
        }

        /// <summary>
        /// View the character sheet
        /// </summary>
        public void DoCmdViewCharacter()
        {
            // Save the current screen
            Gui.FullScreenOverlay = true;
            Gui.Save();
            Gui.SetBackground(Terminal.BackgroundImage.Paper);
            // Load the character viewer
            CharacterViewer characterViewer = new CharacterViewer(Player);
            while (true)
            {
                characterViewer.DisplayPlayer();
                Gui.Print(Colour.Orange, "[Press 'c' to change name, or ESC]", 43, 23);
                char keyPress = Gui.Inkey();
                // Escape breaks us out of the loop
                if (keyPress == '\x1b')
                {
                    break;
                }
                // 'c' changes name
                if (keyPress == 'c' || keyPress == 'C')
                {
                    Player.InputPlayerName();
                }
                Profile.Instance.MsgPrint(null);
            }
            // Restore the screen
            Gui.SetBackground(Terminal.BackgroundImage.Overhead);
            Gui.Load();
            Gui.FullScreenOverlay = false;
            Player.RedrawNeeded.Set(RedrawFlag.PrWipe | RedrawFlag.PrBasic | RedrawFlag.PrExtra | RedrawFlag.PrMap |
                             RedrawFlag.PrEquippy);
            SaveGame.Instance.HandleStuff();
        }

        /// <summary>
        /// Wield/wear an item
        /// </summary>
        public void DoCmdWield()
        {
            string weildPhrase;
            string itemName;
            // Only interested in wearable items
            SaveGame.Instance.ItemFilter = SaveGame.Instance.CommandEngine.ItemFilterWearable;
            if (!SaveGame.Instance.GetItem(out int itemIndex, "Wear/Wield which item? ", false, true, true))
            {
                if (itemIndex == -2)
                {
                    Profile.Instance.MsgPrint("You have nothing you can wear or wield.");
                }
                return;
            }
            Item item = itemIndex >= 0 ? Player.Inventory[itemIndex] : Level.Items[0 - itemIndex];
            // Find the correct item slot
            int slot = Player.Inventory.WieldSlot(item);
            // Can't replace a cursed item
            if (Player.Inventory[slot].IsCursed())
            {
                itemName = Player.Inventory[slot].Description(false, 0);
                Profile.Instance.MsgPrint($"The {itemName} you are {Player.DescribeWieldLocation(slot)} appears to be cursed.");
                return;
            }
            // If we know the item to be cursed, confirm its wearing
            if (item.IsCursed() && (item.IsKnown() || item.IdentifyFlags.IsSet(Constants.IdentSense)))
            {
                itemName = item.Description(false, 0);
                string dummy = $"Really use the {itemName} {{cursed}}? ";
                if (!Gui.GetCheck(dummy))
                {
                    return;
                }
            }
            // Use some energy
            SaveGame.Instance.EnergyUse = 100;
            // Pull one item out of the item stack
            Item wornItem = new Item(item) { Count = 1 };
            // Reduce the count of the item stack accordingly
            if (itemIndex >= 0)
            {
                Player.Inventory.InvenItemIncrease(itemIndex, -1);
                Player.Inventory.InvenItemOptimize(itemIndex);
            }
            else
            {
                SaveGame.Instance.Level.FloorItemIncrease(0 - itemIndex, -1);
                SaveGame.Instance.Level.FloorItemOptimize(0 - itemIndex);
            }
            // Take off the old item
            item = Player.Inventory[slot];
            if (item.ItemType != null)
            {
                Player.Inventory.InvenTakeoff(slot, 255);
            }
            // Put the item into the wield slot
            Player.Inventory[slot] = wornItem;
            // Add the weight of the item
            Player.WeightCarried += wornItem.Weight;
            // Inform us what we did
            if (slot == InventorySlot.MeleeWeapon)
            {
                weildPhrase = "You are wielding";
            }
            else if (slot == InventorySlot.RangedWeapon)
            {
                weildPhrase = "You are shooting with";
            }
            else if (slot == InventorySlot.Lightsource)
            {
                weildPhrase = "Your light source is";
            }
            else if (slot == InventorySlot.Digger)
            {
                weildPhrase = "You are digging with";
            }
            else
            {
                weildPhrase = "You are wearing";
            }
            itemName = wornItem.Description(true, 3);
            Profile.Instance.MsgPrint($"{weildPhrase} {itemName} ({slot.IndexToLabel()}).");
            // Let us know if it's cursed
            if (wornItem.IsCursed())
            {
                Profile.Instance.MsgPrint("Oops! It feels deathly cold!");
                wornItem.IdentifyFlags.Set(Constants.IdentSense);
            }
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateTorchRadius);
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateMana);
            Player.RedrawNeeded.Set(RedrawFlag.PrEquippy);
        }

        /// <summary>
        /// Process the player's latest command
        /// </summary>
        public void ProcessCommand()
        {
            // Get the current command
            char c = Gui.CurrentCommand;
            // Big honking switch statement to call the correct function to handle the command
            switch (c)
            {
                case ' ':
                    {
                        break;
                    }
                case '\r':
                    {
                        break;
                    }
                case '\x1b':
                    DoCmdQuit();
                    break;

                case 'a':
                    {
                        ActivationHandler handler = new ActivationHandler(Level, Player);
                        handler.DoCmdAimWand(-999);
                        break;
                    }
                case 'b':
                    {
                        DoCmdBrowse(-999);
                        break;
                    }
                case 'c':
                    {
                        DoCmdClose();
                        break;
                    }
                case 'd':
                    {
                        DoCmdDrop();
                        break;
                    }
                case 'e':
                    {
                        DoCmdEquip();
                        break;
                    }
                case 'f':
                    {
                        ActivationHandler handler = new ActivationHandler(Level, Player);
                        handler.DoCmdFire();
                        break;
                    }
                case 'g':
                    {
                        DoCmdStay(false);
                        break;
                    }
                case 'h':
                    {
                        DoCmdManual();
                        break;
                    }
                case 'i':
                    {
                        DoCmdInven();
                        break;
                    }
                case 'j':
                    {
                        ActivationHandler handler = new ActivationHandler(Level, Player);
                        handler.DoCmdSpike();
                        break;
                    }
                case 'k':
                    {
                        DoCmdDestroy();
                        break;
                    }
                case 'l':
                    {
                        DoCmdLook();
                        break;
                    }
                case 'm':
                    {
                        CastingHandler handler = new CastingHandler(Player, Level);
                        handler.Cast();
                        break;
                    }
                case 'o':
                    {
                        DoCmdOpen();
                        break;
                    }
                case 'p':
                    {
                        DoCmdRacialPower();
                        break;
                    }
                case 'q':
                    {
                        ActivationHandler handler = new ActivationHandler(Level, Player);
                        handler.DoCmdQuaffPotion(-999);
                        break;
                    }
                case 'r':
                    {
                        ActivationHandler handler = new ActivationHandler(Level, Player);
                        handler.DoCmdReadScroll(-999);
                        break;
                    }
                case 's':
                    {
                        DoCmdSearch();
                        break;
                    }
                case 't':
                    {
                        DoCmdTakeoff();
                        break;
                    }
                case 'u':
                    {
                        ActivationHandler handler = new ActivationHandler(Level, Player);
                        handler.DoCmdUseStaff(-999);
                        break;
                    }
                case 'v':
                    {
                        ActivationHandler handler = new ActivationHandler(Level, Player);
                        handler.DoCmdThrow(1);
                        break;
                    }
                case 'w':
                    {
                        DoCmdWield();
                        break;
                    }
                case 'x':
                    {
                        DoCmdExamine();
                        break;
                    }
                case 'z':
                    {
                        ActivationHandler handler = new ActivationHandler(Level, Player);
                        handler.DoCmdZapRod(-999);
                        break;
                    }
                case 'A':
                    {
                        ActivationHandler handler = new ActivationHandler(Level, Player);
                        handler.DoCmdActivate(-999);
                        break;
                    }
                case 'B':
                    {
                        DoCmdBash();
                        break;
                    }
                case 'C':
                    {
                        DoCmdViewCharacter();
                        break;
                    }
                case 'D':
                    {
                        DoCmdDisarm();
                        break;
                    }
                case 'E':
                    {
                        ActivationHandler handler = new ActivationHandler(Level, Player);
                        handler.DoCmdEatFood(-999);
                        break;
                    }
                case 'F':
                    {
                        ActivationHandler handler = new ActivationHandler(Level, Player);
                        handler.DoCmdRefill(-999);
                        break;
                    }
                case 'H':
                    DoCmdFeeling(false);
                    break;

                case 'J':
                    DoCmdJournal();
                    break;

                case 'K':
                    {
                        DoCmdDestroyAll();
                        break;
                    }
                case 'L':
                    {
                        DoCmdLocate();
                        break;
                    }
                case 'M':
                    {
                        DoCmdViewMap();
                        break;
                    }
                case 'O':
                    {
                        DoCmdMessageOne();
                        break;
                    }
                case 'P':
                    {
                        DoCmdMessages();
                        Gui.SetBackground(Terminal.BackgroundImage.Overhead);
                        break;
                    }
                case 'Q':
                    {
                        DoCmdRetire();
                        break;
                    }
                case 'R':
                    {
                        DoCmdRest();
                        break;
                    }
                case 'S':
                    {
                        DoCmdToggleSearch();
                        break;
                    }
                case 'T':
                    {
                        DoCmdTunnel();
                        break;
                    }
                case 'V':
                    {
                        DoCmdVersion();
                        break;
                    }
                case 'W':
                    {
                        if (Player.IsWizard)
                        {
                            WizardCommandHandler wizard = new WizardCommandHandler(Player, Level);
                            wizard.DoCmdWizard();
                        }
                        else
                        {
                            WizardCommandHandler wizard = new WizardCommandHandler(Player, Level);
                            wizard.DoCmdWizmode();
                        }
                        break;
                    }
                case '+':
                    {
                        DoCmdAlter();
                        break;
                    }
                case ';':
                    {
                        DoCmdWalk(false);
                        break;
                    }
                case '-':
                    {
                        DoCmdWalk(true);
                        break;
                    }
                case '.':
                    {
                        DoCmdRun();
                        break;
                    }
                case ',':
                    {
                        DoCmdStay(true);
                        break;
                    }
                case '_':
                    {
                        DoCmdStore();
                        break;
                    }
                case '<':
                    {
                        DoCmdGoUp();
                        break;
                    }
                case '>':
                    {
                        DoCmdGoDown();
                        break;
                    }
                case '*':
                    {
                        DoCmdTarget();
                        break;
                    }
                case '/':
                    {
                        DoCmdQuerySymbol();
                        break;
                    }
                case '?':
                    {
                        DoCmdCommandList();
                        break;
                    }
                default:
                    {
                        Gui.PrintLine("Type '?' for a list of commands.", 0, 0);
                        break;
                    }
            }
        }

        /// <summary>
        /// Alter a tile in a 'sensibe' way given the tile type
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"> </exception>
        private void DoCmdAlter()
        {
            // Assume we won't disturb the player
            bool disturb = false;
            TargetEngine targetEngine = new TargetEngine(Player, Level);
            // We might have a repeat command
            if (Gui.CommandArgument != 0)
            {
                CommandRepeat = Gui.CommandArgument - 1;
                Player.RedrawNeeded.Set(RedrawFlag.PrState);
                Gui.CommandArgument = 0;
            }
            // Get the direction in which to alter something
            if (targetEngine.GetRepDir(out int dir))
            {
                int y = Player.MapY + Level.KeypadDirectionYOffset[dir];
                int x = Player.MapX + Level.KeypadDirectionXOffset[dir];
                GridTile tile = Level.Grid[y][x];
                // Altering a tile will take a turn
                SaveGame.Instance.EnergyUse = 100;
                // We 'alter' a tile by attacking it
                if (tile.MonsterIndex != 0)
                {
                    SaveGame.Instance.CommandEngine.PlayerAttackMonster(y, x);
                }
                else
                {
                    // Check the action based on the type of tile
                    switch (tile.FeatureType.AlterAction)
                    {
                        case FloorTileAlterAction.Nothing:
                            Profile.Instance.MsgPrint("You're not sure what you can do with that...");
                            break;

                        case FloorTileAlterAction.Tunnel:
                            disturb = SaveGame.Instance.CommandEngine.TunnelThroughTile(y, x);
                            break;

                        case FloorTileAlterAction.Disarm:
                            disturb = SaveGame.Instance.CommandEngine.DisarmTrap(y, x, dir);
                            break;

                        case FloorTileAlterAction.Open:
                            disturb = SaveGame.Instance.CommandEngine.OpenDoor(y, x);
                            break;

                        case FloorTileAlterAction.Close:
                            disturb = SaveGame.Instance.CommandEngine.CloseDoor(y, x);
                            break;

                        case FloorTileAlterAction.Bash:
                            disturb = SaveGame.Instance.CommandEngine.BashClosedDoor(y, x, dir);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            if (!disturb)
            {
                SaveGame.Instance.Disturb(false);
            }
        }

        /// <summary>
        /// Bash a door to open it
        /// </summary>
        private void DoCmdBash()
        {
            // Assume it won't disturb us
            bool disturb = false;
            TargetEngine targetEngine = new TargetEngine(Player, Level);
            // We might have a repeat command
            if (Gui.CommandArgument != 0)
            {
                CommandRepeat = Gui.CommandArgument - 1;
                Player.RedrawNeeded.Set(RedrawFlag.PrState);
                Gui.CommandArgument = 0;
            }
            // Get the direction to bash
            if (targetEngine.GetRepDir(out int dir))
            {
                int y = Player.MapY + Level.KeypadDirectionYOffset[dir];
                int x = Player.MapX + Level.KeypadDirectionXOffset[dir];
                GridTile tile = Level.Grid[y][x];
                // Can only bash closed doors
                if (!tile.FeatureType.IsClosedDoor)
                {
                    Profile.Instance.MsgPrint("You see nothing there to bash.");
                }
                else if (tile.MonsterIndex != 0)
                {
                    // Oops - a montser got in the way
                    SaveGame.Instance.EnergyUse = 100;
                    Profile.Instance.MsgPrint("There is a monster in the way!");
                    SaveGame.Instance.CommandEngine.PlayerAttackMonster(y, x);
                }
                else
                {
                    // Bash the door
                    disturb = SaveGame.Instance.CommandEngine.BashClosedDoor(y, x, dir);
                }
            }
            if (!disturb)
            {
                SaveGame.Instance.Disturb(false);
            }
        }

        private void DoCmdClose()
        {
            TargetEngine targetEngine = new TargetEngine(Player, Level);
            MapCoordinate coord = new MapCoordinate();
            bool more = false;
            if (SaveGame.Instance.CommandEngine.CountOpenDoors(coord) == 1)
            {
                Gui.CommandDirection = Level.CoordsToDir(coord.Y, coord.X);
            }
            if (Gui.CommandArgument != 0)
            {
                CommandRepeat = Gui.CommandArgument - 1;
                Player.RedrawNeeded.Set(RedrawFlag.PrState);
                Gui.CommandArgument = 0;
            }
            if (targetEngine.GetRepDir(out int dir))
            {
                int y = Player.MapY + Level.KeypadDirectionYOffset[dir];
                int x = Player.MapX + Level.KeypadDirectionXOffset[dir];
                GridTile cPtr = Level.Grid[y][x];
                if (cPtr.FeatureType.Category != FloorTileTypeCategory.OpenDoorway)
                {
                    Profile.Instance.MsgPrint("You see nothing there to close.");
                }
                else if (cPtr.MonsterIndex != 0)
                {
                    SaveGame.Instance.EnergyUse = 100;
                    Profile.Instance.MsgPrint("There is a monster in the way!");
                    SaveGame.Instance.CommandEngine.PlayerAttackMonster(y, x);
                }
                else
                {
                    more = SaveGame.Instance.CommandEngine.CloseDoor(y, x);
                }
            }
            if (!more)
            {
                SaveGame.Instance.Disturb(false);
            }
        }

        private void DoCmdCommandList()
        {
            Gui.FullScreenOverlay = true;
            Gui.Save();
            Gui.Refresh();
            Gui.Clear();
            Gui.SetBackground(Terminal.BackgroundImage.Normal);
            Gui.Print(Colour.Yellow, "Numpad", 1, 1);
            Gui.Print("7 8 9", 3, 1);
            Gui.Print(" \\|/", 4, 1);
            Gui.Print("4- -6 = Move", 5, 1);
            Gui.Print(" /|\\    (+Shift = run)", 6, 1);
            Gui.Print("1 2 3", 7, 1);
            Gui.Print("5 = Stand still", 8, 1);
            Gui.Print(Colour.Yellow, "Other Symbols", 10, 1);
            Gui.Print(". = Run", 12, 1);
            Gui.Print("< = Go up stairs", 13, 1);
            Gui.Print("> = Go down stairs", 14, 1);
            Gui.Print("+ = Auto-alter a space", 15, 1);
            Gui.Print("* = Target a creature", 16, 1);
            Gui.Print("/ = Identify a symbol", 17, 1);
            Gui.Print("? = Command list", 18, 1);
            Gui.Print("Esc = Save and quit", 20, 1);
            Gui.Print(Colour.Yellow, "Without Shift", 1, 25);
            Gui.Print("a = Aim a wand", 3, 25);
            Gui.Print("b = Browse a book", 4, 25);
            Gui.Print("c = Close a door", 5, 25);
            Gui.Print("d = Drop object", 6, 25);
            Gui.Print("e = Show equipment", 7, 25);
            Gui.Print("f = Fire a missile weapon", 8, 25);
            Gui.Print("g = Get (pick up) object", 9, 25);
            Gui.Print("h = View game help", 10, 25);
            Gui.Print("i = Show Inventory", 11, 25);
            Gui.Print("j = Jam spike in a door", 12, 25);
            Gui.Print("k = Destroy an item", 13, 25);
            Gui.Print("l = Look around", 14, 25);
            Gui.Print("m = Spell/Mentalism power", 15, 25);
            Gui.Print("n =", 16, 25);
            Gui.Print("o = Open a door/chest", 17, 25);
            Gui.Print("p = Racial power", 18, 25);
            Gui.Print("q = Quaff a potion", 19, 25);
            Gui.Print("r = Read a scroll", 20, 25);
            Gui.Print("s = Search for traps", 21, 25);
            Gui.Print("t = Take off an item", 22, 25);
            Gui.Print("u = Use a staff", 23, 25);
            Gui.Print("v = Throw object", 24, 25);
            Gui.Print("w = Wield/wear an item", 25, 25);
            Gui.Print("x = Examine an object", 26, 25);
            Gui.Print("y =", 27, 25);
            Gui.Print("z = Zap a rod", 28, 25);
            Gui.Print(Colour.Yellow, "With Shift", 1, 52);
            Gui.Print("A = Activate an artifact", 3, 52);
            Gui.Print("B = Bash a stuck door", 4, 52);
            Gui.Print("C = View your character", 5, 52);
            Gui.Print("D = Disarm a trap", 6, 52);
            Gui.Print("E = Eat some food", 7, 52);
            Gui.Print("F = Fuel a light source", 8, 52);
            Gui.Print("H = How you feel here", 10, 52);
            Gui.Print("J = View your journal", 12, 52);
            Gui.Print("K = Destroy trash objects", 13, 52);
            Gui.Print("L = Locate player", 14, 52);
            Gui.Print("M = View the map", 15, 52);
            Gui.Print("O = Show last message", 17, 52);
            Gui.Print("P = Show previous messages", 18, 52);
            Gui.Print("Q = Quit (Retire character)", 19, 52);
            Gui.Print("R = Rest", 20, 52);
            Gui.Print("S = Auto-search on/off", 21, 52);
            Gui.Print("T = Tunnel", 22, 52);
            if (Player.IsWizard)
            {
                Gui.Print("W = Wizard command", 25, 52);
            }
            Gui.AnyKey(44);
            Gui.Load();
            Gui.SetBackground(Terminal.BackgroundImage.Overhead);
            Gui.FullScreenOverlay = false;
        }

        private void DoCmdDisarm()
        {
            TargetEngine targetEngine = new TargetEngine(Player, Level);
            bool more = false;
            MapCoordinate coord = new MapCoordinate();
            int numTraps =
                SaveGame.Instance.CommandEngine.CountKnownTraps(coord);
            int numChests = SaveGame.Instance.CommandEngine.CountChests(coord, true);
            if (numTraps != 0 || numChests != 0)
            {
                bool tooMany = (numTraps != 0 && numChests != 0) || numTraps > 1 || numChests > 1;
                if (!tooMany)
                {
                    Gui.CommandDirection = Level.CoordsToDir(coord.Y, coord.X);
                }
            }
            if (Gui.CommandArgument != 0)
            {
                CommandRepeat = Gui.CommandArgument - 1;
                Player.RedrawNeeded.Set(RedrawFlag.PrState);
                Gui.CommandArgument = 0;
            }
            if (targetEngine.GetRepDir(out int dir))
            {
                int y = Player.MapY + Level.KeypadDirectionYOffset[dir];
                int x = Player.MapX + Level.KeypadDirectionXOffset[dir];
                GridTile cPtr = Level.Grid[y][x];
                int oIdx = Level.ChestCheck(y, x);
                if (!cPtr.FeatureType.IsTrap &&
                    oIdx == 0)
                {
                    Profile.Instance.MsgPrint("You see nothing there to disarm.");
                }
                else if (cPtr.MonsterIndex != 0)
                {
                    Profile.Instance.MsgPrint("There is a monster in the way!");
                    SaveGame.Instance.CommandEngine.PlayerAttackMonster(y, x);
                }
                else if (oIdx != 0)
                {
                    more = SaveGame.Instance.CommandEngine.DisarmChest(y, x, oIdx);
                }
                else
                {
                    more = SaveGame.Instance.CommandEngine.DisarmTrap(y, x, dir);
                }
            }
            if (!more)
            {
                SaveGame.Instance.Disturb(false);
            }
        }

        private void DoCmdDrop()
        {
            int amt = 1;
            if (!SaveGame.Instance.GetItem(out int item, "Drop which item? ", true, true, false))
            {
                if (item == -2)
                {
                    Profile.Instance.MsgPrint("You have nothing to drop.");
                }
                return;
            }
            Item oPtr = item >= 0 ? Player.Inventory[item] : Level.Items[0 - item];
            if (item >= InventorySlot.MeleeWeapon && oPtr.IsCursed())
            {
                Profile.Instance.MsgPrint("Hmmm, it seems to be cursed.");
                return;
            }
            if (oPtr.Count > 1)
            {
                amt = Gui.GetQuantity(null, oPtr.Count, true);
                if (amt <= 0)
                {
                    return;
                }
            }
            SaveGame.Instance.EnergyUse = 50;
            Player.Inventory.InvenDrop(item, amt);
            Player.RedrawNeeded.Set(RedrawFlag.PrEquippy);
        }

        private void DoCmdGoDown()
        {
            bool fallTrap = false;
            GridTile cPtr = Level.Grid[Player.MapY][Player.MapX];
            if (cPtr.FeatureType.Category == FloorTileTypeCategory.TrapDoor)
            {
                fallTrap = true;
            }
            if (cPtr.FeatureType.Name != "DownStair" && !fallTrap)
            {
                Profile.Instance.MsgPrint("I see no down staircase here.");
                SaveGame.Instance.EnergyUse = 0;
                return;
            }
            SaveGame.Instance.EnergyUse = 0;
            if (fallTrap)
            {
                Profile.Instance.MsgPrint("You deliberately jump through the trap door.");
            }
            else
            {
                if (SaveGame.Instance.DunLevel == 0)
                {
                    SaveGame.Instance.CurDungeon = SaveGame.Instance.Wilderness[Player.WildernessY][Player.WildernessX].Dungeon;
                    Profile.Instance.MsgPrint($"You enter {SaveGame.Instance.CurDungeon.Name}");
                }
                else
                {
                    Profile.Instance.MsgPrint("You enter a maze of down staircases.");
                }
                SaveGame.Instance.IsAutosave = true;
                SaveGame.Instance.DoCmdSaveGame();
                SaveGame.Instance.IsAutosave = false;
            }
            if (SaveGame.Instance.CurDungeon.Tower)
            {
                int j = Program.Rng.DieRoll(5);
                if (j > SaveGame.Instance.DunLevel)
                {
                    j = 1;
                }
                SaveGame.Instance.DunLevel -= j;
                if (SaveGame.Instance.DunLevel < 0)
                {
                    SaveGame.Instance.DunLevel = 0;
                }
                if (SaveGame.Instance.DunLevel == 0)
                {
                    Player.WildernessX = SaveGame.Instance.CurDungeon.X;
                    Player.WildernessY = SaveGame.Instance.CurDungeon.Y;
                    SaveGame.Instance.CameFrom = LevelStart.StartStairs;
                }
            }
            else
            {
                int j = Program.Rng.DieRoll(5);
                if (j > SaveGame.Instance.DunLevel)
                {
                    j = 1;
                }
                for (int i = 0; i < j; i++)
                {
                    SaveGame.Instance.DunLevel++;
                    if (SaveGame.Instance.Quests.IsQuest(SaveGame.Instance.DunLevel))
                    {
                        break;
                    }
                }
                if (SaveGame.Instance.DunLevel > SaveGame.Instance.CurDungeon.MaxLevel)
                {
                    SaveGame.Instance.DunLevel = SaveGame.Instance.CurDungeon.MaxLevel;
                }
                if (SaveGame.Instance.DunLevel == 0)
                {
                    SaveGame.Instance.DunLevel++;
                }
            }
            SaveGame.Instance.NewLevelFlag = true;
            if (!fallTrap)
            {
                SaveGame.Instance.CreateUpStair = true;
            }
        }

        private void DoCmdGoUp()
        {
            GridTile cPtr = Level.Grid[Player.MapY][Player.MapX];
            if (cPtr.FeatureType.Name != "UpStair")
            {
                Profile.Instance.MsgPrint("I see no up staircase here.");
                SaveGame.Instance.EnergyUse = 0;
                return;
            }
            SaveGame.Instance.EnergyUse = 0;
            if (SaveGame.Instance.DunLevel == 0)
            {
                SaveGame.Instance.CurDungeon = SaveGame.Instance.Wilderness[Player.WildernessY][Player.WildernessX].Dungeon;
                Profile.Instance.MsgPrint($"You enter {SaveGame.Instance.CurDungeon.Name}");
            }
            else
            {
                Profile.Instance.MsgPrint("You enter a maze of up staircases.");
            }
            SaveGame.Instance.IsAutosave = true;
            SaveGame.Instance.DoCmdSaveGame();
            SaveGame.Instance.IsAutosave = false;
            if (SaveGame.Instance.CurDungeon.Tower)
            {
                int j = Program.Rng.DieRoll(5);
                if (j > SaveGame.Instance.DunLevel)
                {
                    j = 1;
                }
                for (int i = 0; i < j; i++)
                {
                    SaveGame.Instance.DunLevel++;
                    if (SaveGame.Instance.Quests.IsQuest(SaveGame.Instance.DunLevel))
                    {
                        break;
                    }
                }
                if (SaveGame.Instance.DunLevel > SaveGame.Instance.CurDungeon.MaxLevel)
                {
                    SaveGame.Instance.DunLevel = SaveGame.Instance.CurDungeon.MaxLevel;
                }
            }
            else
            {
                int j = Program.Rng.DieRoll(5);
                if (j > SaveGame.Instance.DunLevel)
                {
                    j = 1;
                }
                SaveGame.Instance.DunLevel -= j;
                if (SaveGame.Instance.DunLevel < 0)
                {
                    SaveGame.Instance.DunLevel = 0;
                }
                if (SaveGame.Instance.DunLevel == 0)
                {
                    Player.WildernessX = SaveGame.Instance.CurDungeon.X;
                    Player.WildernessY = SaveGame.Instance.CurDungeon.Y;
                    SaveGame.Instance.CameFrom = LevelStart.StartStairs;
                }
            }
            SaveGame.Instance.NewLevelFlag = true;
            SaveGame.Instance.CreateDownStair = true;
        }

        private void DoCmdLocate()
        {
            int y1, x1;
            int y2 = y1 = Level.PanelRow;
            int x2 = x1 = Level.PanelCol;
            TargetEngine targetEngine = new TargetEngine(Player, Level);
            while (true)
            {
                string tmpVal;
                if (y2 == y1 && x2 == x1)
                {
                    tmpVal = "";
                }
                else
                {
                    string d1 = y2 < y1 ? " North" : y2 > y1 ? " South" : "";
                    string d2 = x2 < x1 ? " West" : x2 > x1 ? " East" : "";
                    tmpVal = $"{d1}{d2} of";
                }
                string outVal = $"Map sector [{y2},{x2}], which is{tmpVal} your sector. Direction?";
                int dir = 0;
                while (dir == 0)
                {
                    if (!Gui.GetCom(outVal, out char command))
                    {
                        break;
                    }
                    dir = Gui.GetKeymapDir(command);
                }
                if (dir == 0)
                {
                    break;
                }
                y2 += Level.KeypadDirectionYOffset[dir];
                x2 += Level.KeypadDirectionXOffset[dir];
                if (y2 > Level.MaxPanelRows)
                {
                    y2 = Level.MaxPanelRows;
                }
                else if (y2 < 0)
                {
                    y2 = 0;
                }
                if (x2 > Level.MaxPanelCols)
                {
                    x2 = Level.MaxPanelCols;
                }
                else if (x2 < 0)
                {
                    x2 = 0;
                }
                if (y2 != Level.PanelRow || x2 != Level.PanelCol)
                {
                    Level.PanelRow = y2;
                    Level.PanelCol = x2;
                    targetEngine.PanelBounds();
                    Player.UpdatesNeeded.Set(UpdateFlags.UpdateMonsters);
                    Player.RedrawNeeded.Set(RedrawFlag.PrMap);
                    SaveGame.Instance.HandleStuff();
                }
            }
            targetEngine.RecenterScreenAroundPlayer();
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateMonsters);
            Player.RedrawNeeded.Set(RedrawFlag.PrMap);
            SaveGame.Instance.HandleStuff();
        }

        private void DoCmdLook()
        {
            TargetEngine targetEngine = new TargetEngine(Player, Level);
            if (targetEngine.TargetSet(Constants.TargetLook))
            {
                Profile.Instance.MsgPrint(SaveGame.Instance.TargetWho > 0 ? "Target Selected." : "Location Targeted.");
            }
        }

        private void DoCmdOpen()
        {
            bool more = false;
            MapCoordinate coord = new MapCoordinate();
            int numDoors =
                SaveGame.Instance.CommandEngine.CountClosedDoors(coord);
            int numChests = SaveGame.Instance.CommandEngine.CountChests(coord, false);
            if (numDoors != 0 || numChests != 0)
            {
                bool tooMany = (numDoors != 0 && numChests != 0) || numDoors > 1 || numChests > 1;
                if (!tooMany)
                {
                    Gui.CommandDirection = Level.CoordsToDir(coord.Y, coord.X);
                }
            }
            if (Gui.CommandArgument != 0)
            {
                CommandRepeat = Gui.CommandArgument - 1;
                Player.RedrawNeeded.Set(RedrawFlag.PrState);
                Gui.CommandArgument = 0;
            }
            TargetEngine targetEngine = new TargetEngine(Player, Level);
            if (targetEngine.GetRepDir(out int dir))
            {
                int y = Player.MapY + Level.KeypadDirectionYOffset[dir];
                int x = Player.MapX + Level.KeypadDirectionXOffset[dir];
                GridTile cPtr = Level.Grid[y][x];
                int oIdx = Level.ChestCheck(y, x);
                if (!cPtr.FeatureType.IsClosedDoor &&
                    oIdx == 0)
                {
                    Profile.Instance.MsgPrint("You see nothing there to open.");
                }
                else if (cPtr.MonsterIndex != 0)
                {
                    SaveGame.Instance.EnergyUse = 100;
                    Profile.Instance.MsgPrint("There is a monster in the way!");
                    SaveGame.Instance.CommandEngine.PlayerAttackMonster(y, x);
                }
                else if (oIdx != 0)
                {
                    more = SaveGame.Instance.CommandEngine.OpenChest(y, x, oIdx);
                }
                else
                {
                    more = SaveGame.Instance.CommandEngine.OpenDoor(y, x);
                }
            }
            if (!more)
            {
                SaveGame.Instance.Disturb(false);
            }
        }

        private void DoCmdQuit()
        {
            if (!Gui.GetCheck("Are you sure you want to save and quit? "))
            {
                return;
            }
            SaveGame.Instance.Playing = false;
        }

        private void DoCmdRacialPower()
        {
            int i = 0;
            int num;
            int[] powers = new int[36];
            string[] powerDesc = new string[36];
            int lvl = Player.Level;
            bool warrior = Player.ProfessionIndex == CharacterClass.Warrior;
            int pets = 0, petCtr;
            bool allPets = false;
            Monster mPtr;
            bool hasRacial = false;
            string racialPower = "(none)";
            for (num = 0; num < 36; num++)
            {
                powers[num] = 0;
                powerDesc[num] = "";
            }
            num = 0;
            if (Player.TimedConfusion != 0)
            {
                Profile.Instance.MsgPrint("You are too confused to use any powers!");
                SaveGame.Instance.EnergyUse = 0;
                return;
            }
            switch (Player.RaceIndex)
            {
                case RaceId.Dwarf:
                    racialPower = lvl < 5
                        ? "detect doors+traps (racial, unusable until level 5)"
                        : "detect doors+traps (racial, cost 5, WIS based)";
                    hasRacial = true;
                    break;

                case RaceId.Nibelung:
                    racialPower = lvl < 5
                        ? "detect doors+traps (racial, WIS based, unusable until level 5)"
                        : "detect doors+traps (racial, cost 5, WIS based)";
                    hasRacial = true;
                    break;

                case RaceId.Hobbit:
                    racialPower = lvl < 15
                        ? "create food        (racial, unusable until level 15)"
                        : "create food        (racial, cost 10, INT based)";
                    hasRacial = true;
                    break;

                case RaceId.Gnome:
                    racialPower = lvl < 5
                        ? "teleport           (racial, unusable until level 5)"
                        : "teleport           (racial, cost 5 + lvl/5, INT based)";
                    hasRacial = true;
                    break;

                case RaceId.HalfOrc:
                    if (lvl < 3)
                    {
                        racialPower = "remove fear        (racial, unusable until level 3)";
                    }
                    else if (warrior)
                    {
                        racialPower = "remove fear        (racial, cost 5, WIS based)";
                    }
                    else
                    {
                        racialPower = "remove fear        (racial, cost 5, WIS based)";
                    }
                    hasRacial = true;
                    break;

                case RaceId.HalfTroll:
                    if (lvl < 10)
                    {
                        racialPower = "berserk            (racial, unusable until level 10)";
                    }
                    else if (warrior)
                    {
                        racialPower = "berserk            (racial, cost 12, WIS based)";
                    }
                    else
                    {
                        racialPower = "berserk            (racial, cost 12, WIS based)";
                    }
                    hasRacial = true;
                    break;

                case RaceId.TchoTcho:
                    if (lvl < 8)
                    {
                        racialPower = "berserk            (racial, unusable until level 8)";
                    }
                    else if (warrior)
                    {
                        racialPower = "berserk            (racial, cost 10, WIS based)";
                    }
                    else
                    {
                        racialPower = "berserk            (racial, cost 10, WIS based)";
                    }
                    hasRacial = true;
                    break;

                case RaceId.Great:
                    racialPower = "dream powers    (unusable until level 30/40)";
                    hasRacial = true;
                    break;

                case RaceId.HalfOgre:
                    racialPower = lvl < 25
                        ? "Yellow Sign     (racial, unusable until level 25)"
                        : "Yellow Sign     (racial, cost 35, INT based)";
                    hasRacial = true;
                    break;

                case RaceId.HalfGiant:
                    racialPower = lvl < 20
                        ? "stone to mud       (racial, unusable until level 20)"
                        : "stone to mud       (racial, cost 10, STR based)";
                    hasRacial = true;
                    break;

                case RaceId.HalfTitan:
                    racialPower = lvl < 35
                        ? "probing            (racial, unusable until level 35)"
                        : "probing            (racial, cost 20, INT based)";
                    hasRacial = true;
                    break;

                case RaceId.Cyclops:
                    racialPower = lvl < 20
                        ? "throw boulder      (racial, unusable until level 20)"
                        : "throw boulder      (racial, cost 15, dam 3*lvl, STR based)";
                    hasRacial = true;
                    break;

                case RaceId.Yeek:
                    racialPower = lvl < 15
                        ? "scare monster      (racial, unusable until level 15)"
                        : "scare monster      (racial, cost 15, WIS based)";
                    hasRacial = true;
                    break;

                case RaceId.Spectre:
                    racialPower = lvl < 4
                        ? "scare monster      (racial, unusable until level 4)"
                        : "scare monster      (racial, cost 3, INT based)";
                    hasRacial = true;
                    break;

                case RaceId.Klackon:
                    racialPower = lvl < 9
                        ? "spit acid          (racial, unusable until level 9)"
                        : "spit acid          (racial, cost 9, dam lvl, DEX based)";
                    hasRacial = true;
                    break;

                case RaceId.Kobold:
                    racialPower = lvl < 12
                        ? "poison dart        (racial, unusable until level 12)"
                        : "poison dart        (racial, cost 8, dam lvl, DEX based)";
                    hasRacial = true;
                    break;

                case RaceId.DarkElf:
                    racialPower = lvl < 2
                        ? "magic missile      (racial, unusable until level 2)"
                        : "magic missile      (racial, cost 2, INT based)";
                    hasRacial = true;
                    break;

                case RaceId.Draconian:
                    racialPower = "breath weapon      (racial, cost lvl, dam 2*lvl, CON based)";
                    hasRacial = true;
                    break;

                case RaceId.MindFlayer:
                    racialPower = lvl < 15
                        ? "mind blast         (racial, unusable until level 15)"
                        : "mind blast         (racial, cost 12, dam lvl, INT based)";
                    hasRacial = true;
                    break;

                case RaceId.Imp:
                    racialPower = lvl < 9
                        ? "fire bolt/ball     (racial, unusable until level 9/30)"
                        : "fire bolt/ball(30) (racial, cost 15, dam lvl, WIS based)";
                    hasRacial = true;
                    break;

                case RaceId.Golem:
                    racialPower = lvl < 20
                        ? "stone skin         (racial, unusable until level 20)"
                        : "stone skin         (racial, cost 15, dur 30+d20, CON based)";
                    hasRacial = true;
                    break;

                case RaceId.Skeleton:
                case RaceId.Zombie:
                    racialPower = lvl < 30
                        ? "restore life       (racial, unusable until level 30)"
                        : "restore life       (racial, cost 30, WIS based)";
                    hasRacial = true;
                    break;

                case RaceId.Vampire:
                    racialPower = lvl < 2
                        ? "drain life         (racial, unusable until level 2)"
                        : "drain life         (racial, cost 1 + lvl/3, based)";
                    hasRacial = true;
                    break;

                case RaceId.Sprite:
                    racialPower = lvl < 12
                        ? "sleeping dust      (racial, unusable until level 12)"
                        : "sleeping dust      (racial, cost 12, INT based)";
                    hasRacial = true;
                    break;
            }
            for (petCtr = Level.MMax - 1; petCtr >= 1; petCtr--)
            {
                mPtr = Level.Monsters[petCtr];
                if ((mPtr.Mind & Constants.SmFriendly) != 0)
                {
                    pets++;
                }
            }
            System.Collections.Generic.List<Mutations.Mutation> activeMutations = Player.Dna.ActivatableMutations(Player);
            if (!hasRacial && activeMutations.Count == 0 && pets == 0)
            {
                Profile.Instance.MsgPrint("You have no powers to activate.");
                SaveGame.Instance.EnergyUse = 0;
                return;
            }
            if (hasRacial)
            {
                powers[0] = int.MaxValue;
                powerDesc[0] = racialPower;
                num++;
            }
            for (int j = 0; j < activeMutations.Count; j++)
            {
                powers[num] = j + 100;
                powerDesc[num] = activeMutations[j].ActivationSummary(Player.Level);
                num++;
            }
            if (pets > 0)
            {
                powerDesc[num] = "dismiss pets";
                powers[num++] = 3;
            }
            bool flag = false;
            bool redraw = false;
            string outVal = $"(Powers {0.I2A()}-{(num - 1).I2A()}, *=List, ESC=exit) Use which power? ";
            while (!flag && Gui.GetCom(outVal, out char choice))
            {
                if (choice == ' ' || choice == '*' || choice == '?')
                {
                    if (!redraw)
                    {
                        int y = 1, x = 13;
                        int ctr = 0;
                        redraw = true;
                        Gui.Save();
                        Gui.PrintLine("", y++, x);
                        while (ctr < num)
                        {
                            string dummy = $"{ctr.I2A()}) {powerDesc[ctr]}";
                            Gui.PrintLine(dummy, y + ctr, x);
                            ctr++;
                        }
                        Gui.PrintLine("", y + ctr, x);
                    }
                    else
                    {
                        redraw = false;
                        Gui.Load();
                    }
                    continue;
                }
                if (choice == '\r' && num == 1)
                {
                    choice = 'a';
                }
                bool ask = char.IsUpper(choice);
                if (ask)
                {
                    choice = char.ToLower(choice);
                }
                i = char.IsLower(choice) ? choice.A2I() : -1;
                if (i < 0 || i >= num)
                {
                    continue;
                }
                if (ask)
                {
                    string tmpVal = $"Use {powerDesc[i]}? ";
                    if (!Gui.GetCheck(tmpVal))
                    {
                        continue;
                    }
                }
                flag = true;
            }
            if (redraw)
            {
                Gui.Load();
            }
            if (!flag)
            {
                SaveGame.Instance.EnergyUse = 0;
                return;
            }
            if (powers[i] == int.MaxValue)
            {
                SaveGame.Instance.CommandEngine.UseRacialPower();
            }
            else if (powers[i] == 3)
            {
                int dismissed = 0;
                if (Gui.GetCheck("Dismiss all pets? "))
                {
                    allPets = true;
                }
                for (petCtr = Level.MMax - 1; petCtr >= 1; petCtr--)
                {
                    mPtr = Level.Monsters[petCtr];
                    if ((mPtr.Mind & Constants.SmFriendly) != 0)
                    {
                        bool deleteThis = false;
                        if (allPets)
                        {
                            deleteThis = true;
                        }
                        else
                        {
                            string friendName = mPtr.MonsterDesc(0x80);
                            string checkFriend = $"Dismiss {friendName}? ";
                            if (Gui.GetCheck(checkFriend))
                            {
                                deleteThis = true;
                            }
                        }
                        if (deleteThis)
                        {
                            Level.Monsters.DeleteMonsterByIndex(petCtr, true);
                            dismissed++;
                        }
                    }
                }
                string s = dismissed == 1 ? "" : "s";
                Profile.Instance.MsgPrint($"You have dismissed {dismissed} pet{s}.");
            }
            else
            {
                SaveGame.Instance.EnergyUse = 100;
                activeMutations[powers[i] - 100].Activate(SaveGame.Instance, Player, Level);
            }
        }

        private void DoCmdRest()
        {
            if (Gui.CommandArgument <= 0)
            {
                const string p = "Rest (0-9999, '*' for HP/SP, '&' as needed): ";
                if (!Gui.GetString(p, out string outVal, "&", 4))
                {
                    return;
                }
                if (string.IsNullOrEmpty(outVal))
                {
                    outVal = "&";
                }
                if (outVal[0] == '&')
                {
                    Gui.CommandArgument = -2;
                }
                else if (outVal[0] == '*')
                {
                    Gui.CommandArgument = -1;
                }
                else
                {
                    if (int.TryParse(outVal, out int i))
                    {
                        Gui.CommandArgument = i;
                    }
                    if (Gui.CommandArgument <= 0)
                    {
                        return;
                    }
                }
            }
            if (Gui.CommandArgument > 9999)
            {
                Gui.CommandArgument = 9999;
            }
            SaveGame.Instance.EnergyUse = 100;
            SaveGame.Instance.Resting = Gui.CommandArgument;
            Player.IsSearching = false;
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            Player.RedrawNeeded.Set(RedrawFlag.PrState);
            SaveGame.Instance.HandleStuff();
            Gui.Refresh();
        }

        private void DoCmdRetire()
        {
            if (Player.IsWinner)
            {
                if (!Gui.GetCheck("Do you want to retire? "))
                {
                    return;
                }
            }
            else
            {
                if (!Player.IsWizard)
                {
                    if (!Gui.GetCheck("Do you really want to give up? "))
                    {
                        return;
                    }
                    Gui.PrintLine("Type the '@' sign to give up (this character will no longer be playable): ", 0, 0);
                    int i = Gui.Inkey();
                    Gui.PrintLine("", 0, 0);
                    if (i != '@')
                    {
                        return;
                    }
                }
            }
            SaveGame.Instance.Playing = false;
            Player.IsDead = true;
            SaveGame.Instance.DiedFrom = "quitting";
        }

        private void DoCmdRun()
        {
            TargetEngine targetEngine = new TargetEngine(Player, Level);
            if (Player.TimedConfusion != 0)
            {
                Profile.Instance.MsgPrint("You are too confused!");
                return;
            }
            if (targetEngine.GetRepDir(out int dir))
            {
                SaveGame.Instance.Running = Gui.CommandArgument != 0 ? Gui.CommandArgument : 1000;
                SaveGame.Instance.CommandEngine.RunOneStep(dir);
            }
        }

        private void DoCmdSearch()
        {
            if (Gui.CommandArgument != 0)
            {
                CommandRepeat = Gui.CommandArgument - 1;
                Player.RedrawNeeded.Set(RedrawFlag.PrState);
                Gui.CommandArgument = 0;
            }
            SaveGame.Instance.EnergyUse = 100;
            SaveGame.Instance.CommandEngine.Search();
        }

        private void DoCmdStay(bool pickup)
        {
            GridTile cPtr = Level.Grid[Player.MapY][Player.MapX];
            if (Gui.CommandArgument != 0)
            {
                CommandRepeat = Gui.CommandArgument - 1;
                Player.RedrawNeeded.Set(RedrawFlag.PrState);
                Gui.CommandArgument = 0;
            }
            SaveGame.Instance.EnergyUse = 100;
            if (Player.SkillSearchFrequency >= 50 || 0 == Program.Rng.RandomLessThan(50 - Player.SkillSearchFrequency))
            {
                SaveGame.Instance.CommandEngine.Search();
            }
            if (Player.IsSearching)
            {
                SaveGame.Instance.CommandEngine.Search();
            }
            SaveGame.Instance.CommandEngine.PickUpItems(pickup);
            if (cPtr.FeatureType.IsShop)
            {
                SaveGame.Instance.Disturb(false);
                Gui.QueuedCommand = '_';
            }
        }

        private void DoCmdTarget()
        {
            TargetEngine targetEngine = new TargetEngine(Player, Level);
            if (targetEngine.TargetSet(Constants.TargetKill))
            {
                Profile.Instance.MsgPrint(SaveGame.Instance.TargetWho > 0 ? "Target Selected." : "Location Targeted.");
            }
            else
            {
                Profile.Instance.MsgPrint("Target Aborted.");
            }
        }

        private void DoCmdToggleSearch()
        {
            if (Player.IsSearching)
            {
                Player.IsSearching = false;
                Player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
                Player.RedrawNeeded.Set(RedrawFlag.PrState);
            }
            else
            {
                Player.IsSearching = true;
                Player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
                Player.RedrawNeeded.Set(RedrawFlag.PrState | RedrawFlag.PrSpeed);
            }
        }

        private void DoCmdTunnel()
        {
            TargetEngine targetEngine = new TargetEngine(Player, Level);
            bool more = false;
            if (Gui.CommandArgument != 0)
            {
                CommandRepeat = Gui.CommandArgument - 1;
                Player.RedrawNeeded.Set(RedrawFlag.PrState);
                Gui.CommandArgument = 0;
            }
            if (targetEngine.GetRepDir(out int dir))
            {
                int y = Player.MapY + Level.KeypadDirectionYOffset[dir];
                int x = Player.MapX + Level.KeypadDirectionXOffset[dir];
                GridTile cPtr = Level.Grid[y][x];
                if (cPtr.FeatureType.IsPassable || cPtr.FeatureType.Name == "YellowSign")
                {
                    Profile.Instance.MsgPrint("You cannot tunnel through air.");
                }
                else if (cPtr.FeatureType.IsClosedDoor)
                {
                    Profile.Instance.MsgPrint("You cannot tunnel through doors.");
                }
                else if (cPtr.MonsterIndex != 0)
                {
                    SaveGame.Instance.EnergyUse = 100;
                    Profile.Instance.MsgPrint("There is a monster in the way!");
                    SaveGame.Instance.CommandEngine.PlayerAttackMonster(y, x);
                }
                else
                {
                    more = SaveGame.Instance.CommandEngine.TunnelThroughTile(y, x);
                }
            }
            if (!more)
            {
                SaveGame.Instance.Disturb(false);
            }
        }

        private void DoCmdVersion()
        {
            AssemblyName assembly = Assembly.GetExecutingAssembly().GetName();
            Version version = assembly.Version;
            Profile.Instance.MsgPrint($"You are playing {Constants.VersionName} {version}.");
            Profile.Instance.MsgPrint($"(Build time: {Constants.CompileTime})");
        }

        private void DoCmdViewMap()
        {
            int cy = -1, cx = -1;
            Gui.FullScreenOverlay = true;
            Gui.Save();
            Gui.Clear();
            if (SaveGame.Instance.DunLevel == 0)
            {
                Gui.SetBackground(Terminal.BackgroundImage.WildMap);
                SaveGame.Instance.DisplayWildMap();
            }
            else
            {
                Gui.SetBackground(Terminal.BackgroundImage.Map);
                Level.DisplayMap(out cy, out cx);
            }
            Gui.Print(Colour.Orange, "[Press any key to continue]", 43, 26);
            if (SaveGame.Instance.DunLevel == 0)
            {
                Gui.Goto(Player.WildernessY + 2, Player.WildernessX + 2);
            }
            else
            {
                Gui.Goto(cy, cx);
            }
            Gui.Inkey();
            Gui.Load();
            Gui.FullScreenOverlay = false;
            Gui.SetBackground(Terminal.BackgroundImage.Overhead);
        }

        private void DoCmdWalk(bool pickup)
        {
            TargetEngine targetEngine = new TargetEngine(Player, Level);
            bool more = false;
            if (Gui.CommandArgument > 0)
            {
                CommandRepeat = Gui.CommandArgument - 1;
                Player.RedrawNeeded.Set(RedrawFlag.PrState);
                Gui.CommandArgument = 0;
            }
            if (targetEngine.GetRepDir(out int dir))
            {
                SaveGame.Instance.EnergyUse = 100;
                SaveGame.Instance.CommandEngine.MovePlayer(dir, pickup);
                more = true;
            }
            if (!more)
            {
                SaveGame.Instance.Disturb(false);
            }
        }
    }
}