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
using Cthangband.Terminal;
using Cthangband.UI;
using System;
using System.Collections.Generic;
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
            if (SaveGame.Instance.CurrentDepth <= 0)
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
                if (SaveGame.Instance.Quests.IsQuest(SaveGame.Instance.CurrentDepth))
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
            Gui.ShowManual();
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
                case '\x1b':
                    {
                        DoCmdPopupMenu();
                        break;
                    }
                case ' ':
                case '\r':
                    {
                        break;
                    }
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
                        DoCmdMutantPower();
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
            if (targetEngine.GetDirectionNoAim(out int dir))
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
            if (targetEngine.GetDirectionNoAim(out int dir))
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

        /// <summary>
        /// Close a door
        /// </summary>
        private void DoCmdClose()
        {
            TargetEngine targetEngine = new TargetEngine(Player, Level);
            MapCoordinate coord = new MapCoordinate();
            bool disturb = false;
            // If there's only one door, assume we mean that one and don't ask for a direction
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
            // Tet the location to close
            if (targetEngine.GetDirectionNoAim(out int dir))
            {
                int y = Player.MapY + Level.KeypadDirectionYOffset[dir];
                int x = Player.MapX + Level.KeypadDirectionXOffset[dir];
                GridTile tile = Level.Grid[y][x];
                // Can only close actual open doors
                if (tile.FeatureType.Category != FloorTileTypeCategory.OpenDoorway)
                {
                    Profile.Instance.MsgPrint("You see nothing there to close.");
                }
                // Can't close if there's a monster in the way
                else if (tile.MonsterIndex != 0)
                {
                    SaveGame.Instance.EnergyUse = 100;
                    Profile.Instance.MsgPrint("There is a monster in the way!");
                    SaveGame.Instance.CommandEngine.PlayerAttackMonster(y, x);
                }
                // Actually close the door
                else
                {
                    disturb = SaveGame.Instance.CommandEngine.CloseDoor(y, x);
                }
            }
            if (!disturb)
            {
                SaveGame.Instance.Disturb(false);
            }
        }

        /// <summary>
        /// Display a list of all the keyboard commands
        /// </summary>
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
            Gui.Print("m = Cast spell/Use talent", 15, 25);
            Gui.Print("n =", 16, 25);
            Gui.Print("o = Open a door/chest", 17, 25);
            Gui.Print("p = Mutant/Racial power", 18, 25);
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
            Gui.Print("V = Version info", 24, 52);
            if (Player.IsWizard)
            {
                Gui.Print("W = Wizard command", 25, 52);
            }
            Gui.AnyKey(44);
            Gui.Load();
            Gui.SetBackground(Terminal.BackgroundImage.Overhead);
            Gui.FullScreenOverlay = false;
        }

        /// <summary>
        /// Attempt to disarm a trap on a door or chest
        /// </summary>
        private void DoCmdDisarm()
        {
            TargetEngine targetEngine = new TargetEngine(Player, Level);
            bool disturb = false;
            MapCoordinate coord = new MapCoordinate();
            int numTraps =
                SaveGame.Instance.CommandEngine.CountKnownTraps(coord);
            int numChests = SaveGame.Instance.CommandEngine.CountChests(coord, true);
            // Count the possible traps and chests we might want to disarm
            if (numTraps != 0 || numChests != 0)
            {
                bool tooMany = (numTraps != 0 && numChests != 0) || numTraps > 1 || numChests > 1;
                // If only one then we have our target
                if (!tooMany)
                {
                    Gui.CommandDirection = Level.CoordsToDir(coord.Y, coord.X);
                }
            }
            // We might be repeating the command
            if (Gui.CommandArgument != 0)
            {
                CommandRepeat = Gui.CommandArgument - 1;
                Player.RedrawNeeded.Set(RedrawFlag.PrState);
                Gui.CommandArgument = 0;
            }
            // Get a direction if we don't already have one
            if (targetEngine.GetDirectionNoAim(out int dir))
            {
                int y = Player.MapY + Level.KeypadDirectionYOffset[dir];
                int x = Player.MapX + Level.KeypadDirectionXOffset[dir];
                GridTile tile = Level.Grid[y][x];
                // Check for a chest
                int itemIndex = Level.ChestCheck(y, x);
                if (!tile.FeatureType.IsTrap &&
                    itemIndex == 0)
                {
                    Profile.Instance.MsgPrint("You see nothing there to disarm.");
                }
                // Can't disarm with a monster in the way
                else if (tile.MonsterIndex != 0)
                {
                    Profile.Instance.MsgPrint("There is a monster in the way!");
                    SaveGame.Instance.CommandEngine.PlayerAttackMonster(y, x);
                }
                // Disarm the chest or trap
                else if (itemIndex != 0)
                {
                    disturb = SaveGame.Instance.CommandEngine.DisarmChest(y, x, itemIndex);
                }
                else
                {
                    disturb = SaveGame.Instance.CommandEngine.DisarmTrap(y, x, dir);
                }
            }
            if (!disturb)
            {
                SaveGame.Instance.Disturb(false);
            }
        }

        /// <summary>
        /// Drop an item
        /// </summary>
        private void DoCmdDrop()
        {
            int amount = 1;
            // Get an item from the inventory/equipment
            if (!SaveGame.Instance.GetItem(out int itemIndex, "Drop which item? ", true, true, false))
            {
                if (itemIndex == -2)
                {
                    Profile.Instance.MsgPrint("You have nothing to drop.");
                }
                return;
            }
            Item item = itemIndex >= 0 ? Player.Inventory[itemIndex] : Level.Items[0 - itemIndex];
            // Can't drop a cursed item
            if (itemIndex >= InventorySlot.MeleeWeapon && item.IsCursed())
            {
                Profile.Instance.MsgPrint("Hmmm, it seems to be cursed.");
                return;
            }
            // It's a stack, so find out how many to drop
            if (item.Count > 1)
            {
                amount = Gui.GetQuantity(null, item.Count, true);
                if (amount <= 0)
                {
                    return;
                }
            }
            // Dropping things takes half a turn
            SaveGame.Instance.EnergyUse = 50;
            // Drop it
            Player.Inventory.InvenDrop(itemIndex, amount);
            Player.RedrawNeeded.Set(RedrawFlag.PrEquippy);
        }

        /// <summary>
        /// Use a down staircase or trapdoor
        /// </summary>
        private void DoCmdGoDown()
        {
            bool isTrapDoor = false;
            GridTile tile = Level.Grid[Player.MapY][Player.MapX];
            if (tile.FeatureType.Category == FloorTileTypeCategory.TrapDoor)
            {
                isTrapDoor = true;
            }
            // Need to be on a staircase or trapdoor
            if (tile.FeatureType.Name != "DownStair" && !isTrapDoor)
            {
                Profile.Instance.MsgPrint("I see no down staircase here.");
                SaveGame.Instance.EnergyUse = 0;
                return;
            }
            // Going onto a new level takes no energy, so the monsters on that level don't get to
            // move before us
            SaveGame.Instance.EnergyUse = 0;
            if (isTrapDoor)
            {
                Profile.Instance.MsgPrint("You deliberately jump through the trap door.");
            }
            else
            {
                // If we're on the surface, enter the relevant dungeon
                if (SaveGame.Instance.CurrentDepth == 0)
                {
                    SaveGame.Instance.CurDungeon = SaveGame.Instance.Wilderness[Player.WildernessY][Player.WildernessX].Dungeon;
                    Profile.Instance.MsgPrint($"You enter {SaveGame.Instance.CurDungeon.Name}");
                }
                else
                {
                    Profile.Instance.MsgPrint("You enter a maze of down staircases.");
                }
                // Save the game, just in case
                SaveGame.Instance.IsAutosave = true;
                SaveGame.Instance.DoCmdSaveGame();
                SaveGame.Instance.IsAutosave = false;
            }
            // If we're in a tower, a down staircase reduces our level number
            if (SaveGame.Instance.CurDungeon.Tower)
            {
                int stairLength = Program.Rng.DieRoll(5);
                if (stairLength > SaveGame.Instance.CurrentDepth)
                {
                    stairLength = 1;
                }
                SaveGame.Instance.CurrentDepth -= stairLength;
                if (SaveGame.Instance.CurrentDepth < 0)
                {
                    SaveGame.Instance.CurrentDepth = 0;
                }
                // If we left the dungeon, remember where we are
                if (SaveGame.Instance.CurrentDepth == 0)
                {
                    Player.WildernessX = SaveGame.Instance.CurDungeon.X;
                    Player.WildernessY = SaveGame.Instance.CurDungeon.Y;
                    SaveGame.Instance.CameFrom = LevelStart.StartStairs;
                }
            }
            else
            {
                // We're not in a tower, so a down staircase increases our level number
                int stairLength = Program.Rng.DieRoll(5);
                if (stairLength > SaveGame.Instance.CurrentDepth)
                {
                    stairLength = 1;
                }
                // Check if we're about to go past a quest level
                for (int i = 0; i < stairLength; i++)
                {
                    SaveGame.Instance.CurrentDepth++;
                    if (SaveGame.Instance.Quests.IsQuest(SaveGame.Instance.CurrentDepth))
                    {
                        // Stop on the quest level
                        break;
                    }
                }
                // Don't go past the max dungeon level
                if (SaveGame.Instance.CurrentDepth > SaveGame.Instance.CurDungeon.MaxLevel)
                {
                    SaveGame.Instance.CurrentDepth = SaveGame.Instance.CurDungeon.MaxLevel;
                }
                // From the surface we always go to the first level
                if (SaveGame.Instance.CurrentDepth == 0)
                {
                    SaveGame.Instance.CurrentDepth++;
                }
            }
            // We need a new level
            SaveGame.Instance.NewLevelFlag = true;
            if (!isTrapDoor)
            {
                // Create an up staircase if we went down a staircase
                SaveGame.Instance.CreateUpStair = true;
            }
        }

        // Go up a staircase
        private void DoCmdGoUp()
        {
            // We need to actually be on an up staircase
            GridTile tile = Level.Grid[Player.MapY][Player.MapX];
            if (tile.FeatureType.Name != "UpStair")
            {
                Profile.Instance.MsgPrint("I see no up staircase here.");
                SaveGame.Instance.EnergyUse = 0;
                return;
            }
            // Use no energy, so monsters in the new level don't get to go first
            SaveGame.Instance.EnergyUse = 0;
            // If we're outside then we must be entering a tower
            if (SaveGame.Instance.CurrentDepth == 0)
            {
                SaveGame.Instance.CurDungeon = SaveGame.Instance.Wilderness[Player.WildernessY][Player.WildernessX].Dungeon;
                Profile.Instance.MsgPrint($"You enter {SaveGame.Instance.CurDungeon.Name}");
            }
            else
            {
                Profile.Instance.MsgPrint("You enter a maze of up staircases.");
            }
            // Autosave, just in case
            SaveGame.Instance.IsAutosave = true;
            SaveGame.Instance.DoCmdSaveGame();
            SaveGame.Instance.IsAutosave = false;
            // In a tower, going up increases our level number
            if (SaveGame.Instance.CurDungeon.Tower)
            {
                int stairLength = Program.Rng.DieRoll(5);
                if (stairLength > SaveGame.Instance.CurrentDepth)
                {
                    stairLength = 1;
                }
                // Make sure we don't go past a quest level
                for (int i = 0; i < stairLength; i++)
                {
                    SaveGame.Instance.CurrentDepth++;
                    if (SaveGame.Instance.Quests.IsQuest(SaveGame.Instance.CurrentDepth))
                    {
                        break;
                    }
                }
                // Make sure we don't go deeper than the dungeon depth
                if (SaveGame.Instance.CurrentDepth > SaveGame.Instance.CurDungeon.MaxLevel)
                {
                    SaveGame.Instance.CurrentDepth = SaveGame.Instance.CurDungeon.MaxLevel;
                }
            }
            else
            {
                // We're not in a tower, so going up decreases our level number
                int j = Program.Rng.DieRoll(5);
                if (j > SaveGame.Instance.CurrentDepth)
                {
                    j = 1;
                }
                SaveGame.Instance.CurrentDepth -= j;
                if (SaveGame.Instance.CurrentDepth < 0)
                {
                    SaveGame.Instance.CurrentDepth = 0;
                }
                if (SaveGame.Instance.CurrentDepth == 0)
                {
                    Player.WildernessX = SaveGame.Instance.CurDungeon.X;
                    Player.WildernessY = SaveGame.Instance.CurDungeon.Y;
                    SaveGame.Instance.CameFrom = LevelStart.StartStairs;
                }
            }
            SaveGame.Instance.NewLevelFlag = true;
            SaveGame.Instance.CreateDownStair = true;
        }

        /// <summary>
        /// Locate the player on the level and let them scroll the map around
        /// </summary>
        private void DoCmdLocate()
        {
            int startRow = Level.PanelRow;
            int startCol = Level.PanelCol;
            int currentRow = startRow;
            int currentCol = startCol;
            TargetEngine targetEngine = new TargetEngine(Player, Level);
            // Enter a loop so the player can browse the level
            while (true)
            {
                // Describe the location being viewed
                string offsetText;
                if (currentRow == startRow && currentCol == startCol)
                {
                    offsetText = "";
                }
                else
                {
                    string northSouth = currentRow < startRow ? " North" : currentRow > startRow ? " South" : "";
                    string eastWest = currentCol < startCol ? " West" : currentCol > startCol ? " East" : "";
                    offsetText = $"{northSouth}{eastWest} of";
                }
                string message = $"Map sector [{currentRow},{currentCol}], which is{offsetText} your sector. Direction?";
                // Get a direction command or escape
                int dir = 0;
                while (dir == 0)
                {
                    if (!Gui.GetCom(message, out char command))
                    {
                        break;
                    }
                    dir = Gui.GetKeymapDir(command);
                }
                if (dir == 0)
                {
                    break;
                }
                // Move the view based on the direction
                currentRow += Level.KeypadDirectionYOffset[dir];
                currentCol += Level.KeypadDirectionXOffset[dir];
                if (currentRow > Level.MaxPanelRows)
                {
                    currentRow = Level.MaxPanelRows;
                }
                else if (currentRow < 0)
                {
                    currentRow = 0;
                }
                if (currentCol > Level.MaxPanelCols)
                {
                    currentCol = Level.MaxPanelCols;
                }
                else if (currentCol < 0)
                {
                    currentCol = 0;
                }
                // Update the view if necessary
                if (currentRow != Level.PanelRow || currentCol != Level.PanelCol)
                {
                    Level.PanelRow = currentRow;
                    Level.PanelCol = currentCol;
                    targetEngine.PanelBounds();
                    Player.UpdatesNeeded.Set(UpdateFlags.UpdateMonsters);
                    Player.RedrawNeeded.Set(RedrawFlag.PrMap);
                    SaveGame.Instance.HandleStuff();
                }
            }
            // We've finished, so snap back to the player's location
            targetEngine.RecenterScreenAroundPlayer();
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateMonsters);
            Player.RedrawNeeded.Set(RedrawFlag.PrMap);
            SaveGame.Instance.HandleStuff();
        }

        /// <summary>
        /// Look around (using the target code) stopping on anything interesting rather than just
        /// things that can be targeted
        /// </summary>
        private void DoCmdLook()
        {
            TargetEngine targetEngine = new TargetEngine(Player, Level);
            if (targetEngine.TargetSet(Constants.TargetLook))
            {
                Profile.Instance.MsgPrint(SaveGame.Instance.TargetWho > 0 ? "Target Selected." : "Location Targeted.");
            }
        }

        /// <summary>
        /// Use a mutant or racial power
        /// </summary>
        private void DoCmdMutantPower()
        {
            int i = 0;
            int num;
            int[] powers = new int[36];
            string[] powerDesc = new string[36];
            int lvl = Player.Level;
            int pets = 0;
            int petCtr;
            bool allPets = false;
            Monster monster;
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
                    racialPower = lvl < 3
                        ? "remove fear        (racial, unusable until level 3)"
                        : "remove fear        (racial, cost 5, WIS based)";
                    hasRacial = true;
                    break;

                case RaceId.HalfTroll:
                    racialPower = lvl < 10
                        ? "berserk            (racial, unusable until level 10)"
                        : "berserk            (racial, cost 12, WIS based)";
                    hasRacial = true;
                    break;

                case RaceId.TchoTcho:
                    racialPower = lvl < 8
                        ? "berserk            (racial, unusable until level 8)"
                        : "berserk            (racial, cost 10, WIS based)";
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
                monster = Level.Monsters[petCtr];
                if ((monster.Mind & Constants.SmFriendly) != 0)
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
            string outVal = $"(Powers {0.IndexToLetter()}-{(num - 1).IndexToLetter()}, *=List, ESC=exit) Use which power? ";
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
                            string dummy = $"{ctr.IndexToLetter()}) {powerDesc[ctr]}";
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
                i = char.IsLower(choice) ? choice.LetterToNumber() : -1;
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
                    monster = Level.Monsters[petCtr];
                    if ((monster.Mind & Constants.SmFriendly) != 0)
                    {
                        bool deleteThis = false;
                        if (allPets)
                        {
                            deleteThis = true;
                        }
                        else
                        {
                            string friendName = monster.MonsterDesc(0x80);
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

        /// <summary>
        /// Open a door or chest
        /// </summary>
        private void DoCmdOpen()
        {
            bool disturb = false;
            // Check if there's only one thing we can open
            MapCoordinate coord = new MapCoordinate();
            int numDoors =
                SaveGame.Instance.CommandEngine.CountClosedDoors(coord);
            int numChests = SaveGame.Instance.CommandEngine.CountChests(coord, false);
            if (numDoors != 0 || numChests != 0)
            {
                bool tooMany = (numDoors != 0 && numChests != 0) || numDoors > 1 || numChests > 1;
                if (!tooMany)
                {
                    // There's only one thing we can open, so assume we mean that thing
                    Gui.CommandDirection = Level.CoordsToDir(coord.Y, coord.X);
                }
            }
            // If we're repeatedly trying to open something, mark the repeat flag
            if (Gui.CommandArgument != 0)
            {
                CommandRepeat = Gui.CommandArgument - 1;
                Player.RedrawNeeded.Set(RedrawFlag.PrState);
                Gui.CommandArgument = 0;
            }
            // If we don't already have a direction, prompt for one
            TargetEngine targetEngine = new TargetEngine(Player, Level);
            if (targetEngine.GetDirectionNoAim(out int dir))
            {
                int y = Player.MapY + Level.KeypadDirectionYOffset[dir];
                int x = Player.MapX + Level.KeypadDirectionXOffset[dir];
                GridTile tile = Level.Grid[y][x];
                int itemIndex = Level.ChestCheck(y, x);
                // Make sure there is something to open in the direction we chose
                if (!tile.FeatureType.IsClosedDoor &&
                    itemIndex == 0)
                {
                    Profile.Instance.MsgPrint("You see nothing there to open.");
                }
                // Can't open something if there's a monster in the way
                else if (tile.MonsterIndex != 0)
                {
                    SaveGame.Instance.EnergyUse = 100;
                    Profile.Instance.MsgPrint("There is a monster in the way!");
                    SaveGame.Instance.CommandEngine.PlayerAttackMonster(y, x);
                }
                // Open the chest or door
                else if (itemIndex != 0)
                {
                    disturb = SaveGame.Instance.CommandEngine.OpenChest(y, x, itemIndex);
                }
                else
                {
                    disturb = SaveGame.Instance.CommandEngine.OpenDoor(y, x);
                }
            }
            if (!disturb)
            {
                SaveGame.Instance.Disturb(false);
            }
        }

        /// <summary>
        /// Fire the popup menu for quitting and changing options
        /// </summary>
        private void DoCmdPopupMenu()
        {
            var menuItems = new List<string>() { "Resume Game", "Options", "Quit to Menu", "Quit to Desktop" };
            var menu = new PopupMenu(menuItems);
            var result = menu.Show();
            switch (result)
            {
                // Escape or Resume Game
                case -1:
                case 0:
                    return;
                // Options
                case 1:
                    Program.ChangeOptions();
                    Gui.SetBackground(BackgroundImage.Overhead);
                    break;
                // Quit to Menu
                case 2:
                    SaveGame.Instance.Playing = false;
                    break;
                // Quit to Desktop
                case 3:
                    SaveGame.Instance.Playing = false;
                    Program.ExitToDesktop = true;
                    break;
            }
        }

        /// <summary>
        /// Rest for either a fixed amount of time or until back to max health and mana
        /// </summary>
        private void DoCmdRest()
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
            Player.IsSearching = false;
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            Player.RedrawNeeded.Set(RedrawFlag.PrState);
            SaveGame.Instance.HandleStuff();
            Gui.Refresh();
        }

        /// <summary>
        /// Retire (if a winner) or give up (if not a winner)
        /// </summary>
        private void DoCmdRetire()
        {
            // If we're a winner it's a simple question with a more positive connotation
            if (Player.IsWinner)
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
                if (!Player.IsWizard)
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
            Player.IsDead = true;
            SaveGame.Instance.DiedFrom = "quitting";
        }

        /// <summary>
        /// Start running
        /// </summary>
        private void DoCmdRun()
        {
            TargetEngine targetEngine = new TargetEngine(Player, Level);
            // Can't run if we're confused
            if (Player.TimedConfusion != 0)
            {
                Profile.Instance.MsgPrint("You are too confused!");
                return;
            }
            // Get a direction if we don't already have one
            if (targetEngine.GetDirectionNoAim(out int dir))
            {
                // If we don't have a distance, assume we'll run for 1,000 steps
                SaveGame.Instance.Running = Gui.CommandArgument != 0 ? Gui.CommandArgument : 1000;
                // Run one step in the chosen direction
                SaveGame.Instance.CommandEngine.RunOneStep(dir);
            }
        }

        /// <summary>
        /// Spend a turn searching for traps and secret doors
        /// </summary>
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

        /// <summary>
        /// Stand still for a turn, possibly picking up and item
        /// </summary>
        /// <param name="pickup"> Whether or not we should pick up an object in our location </param>
        private void DoCmdStay(bool pickup)
        {
            // We might be staying still for multiple turns
            if (Gui.CommandArgument != 0)
            {
                CommandRepeat = Gui.CommandArgument - 1;
                Player.RedrawNeeded.Set(RedrawFlag.PrState);
                Gui.CommandArgument = 0;
            }
            // Standing still takes a turn
            SaveGame.Instance.EnergyUse = 100;
            // Periodically search if we're not actively in search mode
            if (Player.SkillSearchFrequency >= 50 || 0 == Program.Rng.RandomLessThan(50 - Player.SkillSearchFrequency))
            {
                SaveGame.Instance.CommandEngine.Search();
            }
            // Always search if we are actively in search mode
            if (Player.IsSearching)
            {
                SaveGame.Instance.CommandEngine.Search();
            }
            // Pick up items if we should
            SaveGame.Instance.CommandEngine.PickUpItems(pickup);
            // If we're in a shop doorway, enter the shop
            GridTile tile = Level.Grid[Player.MapY][Player.MapX];
            if (tile.FeatureType.IsShop)
            {
                SaveGame.Instance.Disturb(false);
                Gui.QueuedCommand = '_';
            }
        }

        /// <summary>
        /// Select a target in advance for attacks. Note that this does not cost any in-game time
        /// </summary>
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

        /// <summary>
        /// Toggle whether we're automatically searching while moving
        /// </summary>
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

        /// <summary>
        /// Tunnel into the wall (or whatever is in front of us
        /// </summary>
        private void DoCmdTunnel()
        {
            TargetEngine targetEngine = new TargetEngine(Player, Level);
            bool disturb = false;
            // We might be tunnelling for multiple turns
            if (Gui.CommandArgument != 0)
            {
                CommandRepeat = Gui.CommandArgument - 1;
                Player.RedrawNeeded.Set(RedrawFlag.PrState);
                Gui.CommandArgument = 0;
            }
            // Get the direction in which we wish to tunnel
            if (targetEngine.GetDirectionNoAim(out int dir))
            {
                // Pick up the tile that the player wishes to tunnel through
                int tileY = Player.MapY + Level.KeypadDirectionYOffset[dir];
                int tileX = Player.MapX + Level.KeypadDirectionXOffset[dir];
                GridTile tile = Level.Grid[tileY][tileX];
                // Check if it can be tunneled through
                if (tile.FeatureType.IsPassable || tile.FeatureType.Name == "YellowSign")
                {
                    Profile.Instance.MsgPrint("You cannot tunnel through air.");
                }
                else if (tile.FeatureType.IsClosedDoor)
                {
                    Profile.Instance.MsgPrint("You cannot tunnel through doors.");
                }
                // Can't tunnel if there's a monster there - so attack the monster instead
                else if (tile.MonsterIndex != 0)
                {
                    SaveGame.Instance.EnergyUse = 100;
                    Profile.Instance.MsgPrint("There is a monster in the way!");
                    SaveGame.Instance.CommandEngine.PlayerAttackMonster(tileY, tileX);
                }
                else
                {
                    // Tunnel through the tile
                    disturb = SaveGame.Instance.CommandEngine.TunnelThroughTile(tileY, tileX);
                }
            }
            // Something might have disturbed us
            if (!disturb)
            {
                SaveGame.Instance.Disturb(false);
            }
        }

        /// <summary>
        /// Print the version number and build info of the game
        /// </summary>
        private void DoCmdVersion()
        {
            AssemblyName assembly = Assembly.GetExecutingAssembly().GetName();
            Version version = assembly.Version;
            Profile.Instance.MsgPrint($"You are playing {Constants.VersionName} {version}.");
            Profile.Instance.MsgPrint($"(Build time: {Constants.CompileTime})");
        }

        /// <summary>
        /// Display a map of the area on screen
        /// </summary>
        private void DoCmdViewMap()
        {
            int cy = -1;
            int cx = -1;
            Gui.FullScreenOverlay = true;
            Gui.Save();
            Gui.Clear();
            // If we're on the surface, display the island map
            if (SaveGame.Instance.CurrentDepth == 0)
            {
                Gui.SetBackground(BackgroundImage.WildMap);
                SaveGame.Instance.DisplayWildMap();
            }
            else
            {
                // We're not on the surface, so draw the level map
                Gui.SetBackground(BackgroundImage.Map);
                Level.DisplayMap(out cy, out cx);
            }
            // Give us a prompt, and display the cursor in the player's location
            Gui.Print(Colour.Orange, "[Press any key to continue]", 43, 26);
            if (SaveGame.Instance.CurrentDepth == 0)
            {
                Gui.Goto(Player.WildernessY + 2, Player.WildernessX + 2);
            }
            else
            {
                Gui.Goto(cy, cx);
            }
            // Wait for a keypress, and restore the screen (looking at the map takes no time)
            Gui.Inkey();
            Gui.Load();
            Gui.FullScreenOverlay = false;
            Gui.SetBackground(BackgroundImage.Overhead);
        }

        /// <summary>
        /// Take a single step in a direction
        /// </summary>
        /// <param name="pickup"> Whether or not we should pick up an object that we step on </param>
        private void DoCmdWalk(bool pickup)
        {
            TargetEngine targetEngine = new TargetEngine(Player, Level);
            bool disturb = false;
            // We might be walking (or running!) more than one step
            if (Gui.CommandArgument > 0)
            {
                CommandRepeat = Gui.CommandArgument - 1;
                Player.RedrawNeeded.Set(RedrawFlag.PrState);
                Gui.CommandArgument = 0;
            }
            // If we don't already have a direction, get one
            if (targetEngine.GetDirectionNoAim(out int dir))
            {
                // Walking takes a full turn
                SaveGame.Instance.EnergyUse = 100;
                SaveGame.Instance.CommandEngine.MovePlayer(dir, pickup);
                disturb = true;
            }
            // We will have been disturbed, unless we cancelled the direction prompt
            if (!disturb)
            {
                SaveGame.Instance.Disturb(false);
            }
        }
    }
}