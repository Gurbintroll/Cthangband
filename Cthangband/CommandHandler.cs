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
        public int CommandRep;
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
            bool force = Gui.CommandArg > 0;
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
            SaveGame.Instance.CommandWrk = true;
            Gui.Save();
            SaveGame.Instance.ItemFilterAll = true;
            Player.Inventory.ShowEquip();
            SaveGame.Instance.ItemFilterAll = false;
            string outVal =
                $"Equipment: carrying {Player.WeightCarried / 10}.{Player.WeightCarried % 10} pounds ({Player.WeightCarried * 100 / (Player.AbilityScores[Ability.Strength].StrCarryingCapacity * 100 / 2)}% of capacity). Command: ";
            Gui.PrintLine(outVal, 0, 0);
            Gui.CommandNew = Gui.Inkey();
            Gui.Load();
            if (Gui.CommandNew == '\x1b')
            {
                Gui.CommandNew = (char)0;
                SaveGame.Instance.CommandGap = 50;
            }
            else
            {
                SaveGame.Instance.CommandSee = true;
            }
        }

        public void DoCmdFeeling(bool feelingOnly)
        {
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
                return;
            }
            if (!feelingOnly)
            {
                Profile.Instance.MsgPrint($"You are in {SaveGame.Instance.CurDungeon.Name}.");
                if (SaveGame.Instance.Quests.IsQuest(SaveGame.Instance.DunLevel))
                {
                    SaveGame.Instance.Quests.PrintQuestMessage();
                }
            }
            if (Level.DangerFeeling == 1 || Level.TreasureFeeling == 1)
            {
                string message = GlobalData.DangerFeelingText[1];
                Profile.Instance.MsgPrint(Player.GameTime.LevelFeel
                    ? message : GlobalData.DangerFeelingText[0]);
            }
            else
            {
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

        public void DoCmdInven()
        {
            SaveGame.Instance.CommandWrk = false;
            Gui.Save();
            SaveGame.Instance.ItemFilterAll = true;
            Player.Inventory.ShowInven();
            SaveGame.Instance.ItemFilterAll = false;
            string outVal =
                $"Inventory: carrying {Player.WeightCarried / 10}.{Player.WeightCarried % 10} pounds ({Player.WeightCarried * 100 / (Player.AbilityScores[Ability.Strength].StrCarryingCapacity * 100 / 2)}% of capacity). Command: ";
            Gui.PrintLine(outVal, 0, 0);
            Gui.CommandNew = Gui.Inkey();
            Gui.Load();
            if (Gui.CommandNew == '\x1b')
            {
                Gui.CommandNew = (char)0;
                SaveGame.Instance.CommandGap = 50;
            }
            else
            {
                SaveGame.Instance.CommandSee = true;
            }
        }

        public void DoCmdJournal()
        {
            Journal journal = new Journal(Player);
            journal.ShowMenu();
        }

        public void DoCmdManual()
        {
            using (Manual.ManualViewer manual = new Manual.ManualViewer())
            {
                manual.ShowDialog();
            }
        }

        public void DoCmdMessageOne()
        {
            Gui.PrintLine($"> {Profile.Instance.MessageStr(0)}", 0, 0);
        }

        public void DoCmdMessages()
        {
            int n = Profile.Instance.MessageNum();
            int i = 0;
            int q = 0;
            Gui.FullScreenOverlay = true;
            Gui.Save();
            Gui.SetBackground(Terminal.BackgroundImage.Normal);
            while (true)
            {
                Gui.Clear();
                int j;
                for (j = 0; j < 40 && i + j < n; j++)
                {
                    string msg = Profile.Instance.MessageStr((short)(i + j));
                    msg = msg.Length >= q ? msg.Substring(q) : "";
                    Gui.Print(Colour.White, msg, 41 - j, 0);
                }
                Gui.PrintLine($"Message Recall ({i}-{i + j - 1} of {n}), Offset {q}", 0, 0);
                Gui.PrintLine("[Press 'p' for older, 'n' for newer, <dir> to scroll, or ESCAPE]", 43, 0);
                int k = Gui.Inkey();
                if (k == '\x1b')
                {
                    break;
                }
                if (k == '4')
                {
                    q = q >= 40 ? q - 40 : 0;
                    continue;
                }
                if (k == '6')
                {
                    q += 40;
                    continue;
                }
                if (k == '8' || k == '\n' || k == '\r')
                {
                    if (i + 1 < n)
                    {
                        i++;
                    }
                }
                if (k == 'p' || k == ' ')
                {
                    if (i + 40 < n)
                    {
                        i += 40;
                    }
                }
                if (k == 'n')
                {
                    i = i >= 40 ? i - 40 : 0;
                }
                if (k == '2')
                {
                    i = i >= 1 ? i - 1 : 0;
                }
            }
            Gui.Load();
            Gui.FullScreenOverlay = false;
        }

        public void DoCmdObserve()
        {
            if (!SaveGame.Instance.GetItem(out int item, "Examine which item? ", true, true, true))
            {
                if (item == -2)
                {
                    Profile.Instance.MsgPrint("You have nothing to examine.");
                }
                return;
            }
            Item oPtr = item >= 0 ? Player.Inventory[item] : Level.Items[0 - item];
            if (oPtr.IdentifyFlags.IsClear(Constants.IdentMental))
            {
                Profile.Instance.MsgPrint("You have no special knowledge about that item.");
                return;
            }
            string oName = oPtr.Description(true, 3);
            Profile.Instance.MsgPrint($"Examining {oName}...");
            if (!oPtr.IdentifyFully())
            {
                Profile.Instance.MsgPrint("You see nothing special.");
            }
        }

        public void DoCmdQuerySymbol()
        {
            int i;
            if (!Gui.GetCom("Enter character to be identified: ", out char sym))
            {
                return;
            }
            for (i = 0; GlobalData.IdentInfo[i] != null; ++i)
            {
                if (sym == GlobalData.IdentInfo[i][0])
                {
                    break;
                }
            }
            string buf = GlobalData.IdentInfo[i] != null
                ? $"{sym} - {GlobalData.IdentInfo[i].Substring(2)}."
                : $"{sym} - Unknown Symbol";
            Profile.Instance.MsgPrint(buf);
        }

        public void DoCmdStore()
        {
            GridTile cPtr = Level.Grid[Player.MapY][Player.MapX];
            if (!cPtr.FeatureType.IsShop)
            {
                Profile.Instance.MsgPrint("You see no Stores here.");
                return;
            }
            Store which = SaveGame.Instance.GetWhichStore();
            if (which.StoreType == StoreType.StoreHome && Player.TownWithHouse != SaveGame.Instance.CurTown.Index)
            {
                Profile.Instance.MsgPrint("The door is locked.");
                return;
            }
            Level.ForgetLight();
            Level.ForgetView();
            Gui.FullScreenOverlay = true;
            Gui.CommandArg = 0;
            CommandRep = 0;
            Gui.CommandNew = '\0';
            which.EnterStore(Player, this);
        }

        public void DoCmdStudy()
        {
            string p = Player.Spellcasting.Type == CastingType.Arcane ? "spell" : "prayer";
            if (Player.Realm1 == 0)
            {
                Profile.Instance.MsgPrint("You cannot read books!");
                return;
            }
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
            if (Player.NewSpells == 0)
            {
                Profile.Instance.MsgPrint($"You cannot learn any new {p}s!");
                return;
            }
            string s = Player.NewSpells == 1 ? "" : "s";
            Profile.Instance.MsgPrint($"You can learn {Player.NewSpells} new {p}{s}.");
            Profile.Instance.MsgPrint(null);
            Inventory.ItemFilterUseableSpellBook = true;
            if (!SaveGame.Instance.GetItem(out int item, "Study which book? ", false, true, true))
            {
                if (item == -2)
                {
                    Profile.Instance.MsgPrint("You have no books that you can read.");
                }
                Inventory.ItemFilterUseableSpellBook = false;
                return;
            }
            Inventory.ItemFilterUseableSpellBook = false;
            Item oPtr = item >= 0 ? Player.Inventory[item] : Level.Items[0 - item];
            int sval = oPtr.ItemSubCategory;
            bool useSetTwo = oPtr.Category == Player.Realm2.ToSpellBookItemCategory();
            SaveGame.Instance.HandleStuff();
            int spell;
            if (Player.Spellcasting.Type != CastingType.Divine)
            {
                CastingHandler castingHandler = new CastingHandler(SaveGame.Instance.Player, SaveGame.Instance.Level);
                if (!castingHandler.GetSpell(out spell, "study", sval, false, useSetTwo) && spell == -1)
                {
                    return;
                }
            }
            else
            {
                int k = 0;
                int gift = -1;
                for (spell = 0; spell < 32; spell++)
                {
                    if ((GlobalData.BookSpellFlags[sval] & (1u << spell)) != 0)
                    {
                        if (!Player.SpellOkay(spell, false, useSetTwo))
                        {
                            continue;
                        }
                        k++;
                        if (Program.Rng.RandomLessThan(k) == 0)
                        {
                            gift = spell;
                        }
                    }
                }
                spell = gift;
            }
            if (spell < 0)
            {
                Profile.Instance.MsgPrint($"You cannot learn any {p}s from that book.");
                return;
            }
            SaveGame.Instance.EnergyUse = 100;
            Spell sPtr = useSetTwo ? Player.Spellcasting.Spells[1][spell] : Player.Spellcasting.Spells[0][spell];
            sPtr.Learned = true;
            int i;
            for (i = 0; i < 64; i++)
            {
                if (Player.Spellcasting.SpellOrder[i] == 99)
                {
                    break;
                }
            }
            Player.Spellcasting.SpellOrder[i] = spell;
            Profile.Instance.MsgPrint($"You have learned the {p} of {sPtr.Name}.");
            Gui.PlaySound(SoundEffect.Study);
            Player.NewSpells--;
            if (Player.NewSpells != 0)
            {
                s = Player.NewSpells != 1 ? "s" : "";
                Profile.Instance.MsgPrint($"You can learn {Player.NewSpells} more {p}{s}.");
            }
            Player.OldSpells = Player.NewSpells;
            Player.RedrawNeeded.Set(RedrawFlag.PrStudy);
        }

        public void DoCmdTakeoff()
        {
            if (!SaveGame.Instance.GetItem(out int item, "Take off which item? ", true, false, false))
            {
                if (item == -2)
                {
                    Profile.Instance.MsgPrint("You are not wearing anything to take off.");
                }
                return;
            }
            Item oPtr = item >= 0 ? Player.Inventory[item] : Level.Items[0 - item];
            if (oPtr.IsCursed())
            {
                Profile.Instance.MsgPrint("Hmmm, it seems to be cursed.");
                return;
            }
            SaveGame.Instance.EnergyUse = 50;
            Player.Inventory.InvenTakeoff(item, 255);
            Player.RedrawNeeded.Set(RedrawFlag.PrEquippy);
        }

        public void DoCmdViewCharacter()
        {
            Gui.FullScreenOverlay = true;
            Gui.Save();
            Gui.SetBackground(Terminal.BackgroundImage.Paper);
            CharacterViewer characterViewer = new CharacterViewer(Player);
            while (true)
            {
                characterViewer.DisplayPlayer();
                Gui.Print(Colour.Orange, "[Press 'c' to change name, or ESC]", 43, 23);
                char c = Gui.Inkey();
                if (c == '\x1b')
                {
                    break;
                }
                if (c == 'c')
                {
                    Player.InputPlayerName();
                }
                Profile.Instance.MsgPrint(null);
            }
            Gui.SetBackground(Terminal.BackgroundImage.Overhead);
            Gui.Load();
            Gui.FullScreenOverlay = false;
            Player.RedrawNeeded.Set(RedrawFlag.PrWipe | RedrawFlag.PrBasic | RedrawFlag.PrExtra | RedrawFlag.PrMap |
                             RedrawFlag.PrEquippy);
            SaveGame.Instance.HandleStuff();
        }

        public void DoCmdWield()
        {
            string act;
            string oName;
            SaveGame.Instance.ItemFilter = SaveGame.Instance.CommandEngine.ItemFilterWearable;
            if (!SaveGame.Instance.GetItem(out int item, "Wear/Wield which item? ", false, true, true))
            {
                if (item == -2)
                {
                    Profile.Instance.MsgPrint("You have nothing you can wear or wield.");
                }
                return;
            }
            Item oPtr = item >= 0 ? Player.Inventory[item] : Level.Items[0 - item];
            int slot = Player.Inventory.WieldSlot(oPtr);
            if (Player.Inventory[slot].IsCursed())
            {
                oName = Player.Inventory[slot].Description(false, 0);
                Profile.Instance.MsgPrint($"The {oName} you are {Player.DescribeWieldLocation(slot)} appears to be cursed.");
                return;
            }
            if (oPtr.IsCursed() && (oPtr.IsKnown() || oPtr.IdentifyFlags.IsSet(Constants.IdentSense)))
            {
                oName = oPtr.Description(false, 0);
                string dummy = $"Really use the {oName} {{cursed}}? ";
                if (!Gui.GetCheck(dummy))
                {
                    return;
                }
            }
            SaveGame.Instance.EnergyUse = 100;
            Item qPtr = new Item(oPtr) { Count = 1 };
            if (item >= 0)
            {
                Player.Inventory.InvenItemIncrease(item, -1);
                Player.Inventory.InvenItemOptimize(item);
            }
            else
            {
                SaveGame.Instance.Level.FloorItemIncrease(0 - item, -1);
                SaveGame.Instance.Level.FloorItemOptimize(0 - item);
            }
            oPtr = Player.Inventory[slot];
            if (oPtr.ItemType != null)
            {
                Player.Inventory.InvenTakeoff(slot, 255);
            }
            Player.Inventory[slot] = new Item(qPtr);
            oPtr = Player.Inventory[slot];
            Player.WeightCarried += qPtr.Weight;
            if (slot == InventorySlot.MeleeWeapon)
            {
                act = "You are wielding";
            }
            else if (slot == InventorySlot.RangedWeapon)
            {
                act = "You are shooting with";
            }
            else if (slot == InventorySlot.Lightsource)
            {
                act = "Your light source is";
            }
            else if (slot == InventorySlot.Digger)
            {
                act = "You are digging with";
            }
            else
            {
                act = "You are wearing";
            }
            oName = oPtr.Description(true, 3);
            Profile.Instance.MsgPrint($"{act} {oName} ({slot.IndexToLabel()}).");
            if (oPtr.IsCursed())
            {
                Profile.Instance.MsgPrint("Oops! It feels deathly cold!");
                oPtr.IdentifyFlags.Set(Constants.IdentSense);
            }
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateTorchRadius);
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateMana);
            Player.RedrawNeeded.Set(RedrawFlag.PrEquippy);
        }

        public void ProcessCommand()
        {
            char c = Gui.CommandCmd;
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
                        DoCmdObserve();
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
                        DoCmdSuicide();
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

        private void DoCmdAlter()
        {
            bool more = false;
            TargetEngine targetEngine = new TargetEngine(Player, Level);
            if (Gui.CommandArg != 0)
            {
                CommandRep = Gui.CommandArg - 1;
                Player.RedrawNeeded.Set(RedrawFlag.PrState);
                Gui.CommandArg = 0;
            }
            if (targetEngine.GetRepDir(out int dir))
            {
                int y = Player.MapY + Level.KeypadDirectionYOffset[dir];
                int x = Player.MapX + Level.KeypadDirectionXOffset[dir];
                GridTile cPtr = Level.Grid[y][x];
                SaveGame.Instance.EnergyUse = 100;
                if (cPtr.MonsterIndex != 0)
                {
                    SaveGame.Instance.CommandEngine.PlayerAttackMonster(y, x);
                }
                else
                {
                    switch (cPtr.FeatureType.AlterAction)
                    {
                        case FloorTileAlterAction.Nothing:
                            Profile.Instance.MsgPrint("You're not sure what you can do with that...");
                            break;

                        case FloorTileAlterAction.Tunnel:
                            more = SaveGame.Instance.CommandEngine.TunnelThroughTile(y, x);
                            break;

                        case FloorTileAlterAction.Disarm:
                            more = SaveGame.Instance.CommandEngine.DisarmTrap(y, x, dir);
                            break;

                        case FloorTileAlterAction.Open:
                            more = SaveGame.Instance.CommandEngine.OpenDoor(y, x);
                            break;

                        case FloorTileAlterAction.Close:
                            more = SaveGame.Instance.CommandEngine.CloseDoor(y, x);
                            break;

                        case FloorTileAlterAction.Bash:
                            more = SaveGame.Instance.CommandEngine.BashClosedDoor(y, x, dir);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            if (!more)
            {
                SaveGame.Instance.Disturb(false);
            }
        }

        private void DoCmdBash()
        {
            bool more = false;
            TargetEngine targetEngine = new TargetEngine(Player, Level);
            if (Gui.CommandArg != 0)
            {
                CommandRep = Gui.CommandArg - 1;
                Player.RedrawNeeded.Set(RedrawFlag.PrState);
                Gui.CommandArg = 0;
            }
            if (targetEngine.GetRepDir(out int dir))
            {
                int y = Player.MapY + Level.KeypadDirectionYOffset[dir];
                int x = Player.MapX + Level.KeypadDirectionXOffset[dir];
                GridTile cPtr = Level.Grid[y][x];
                if (!cPtr.FeatureType.IsClosedDoor)
                {
                    Profile.Instance.MsgPrint("You see nothing there to bash.");
                }
                else if (cPtr.MonsterIndex != 0)
                {
                    SaveGame.Instance.EnergyUse = 100;
                    Profile.Instance.MsgPrint("There is a monster in the way!");
                    SaveGame.Instance.CommandEngine.PlayerAttackMonster(y, x);
                }
                else
                {
                    more = SaveGame.Instance.CommandEngine.BashClosedDoor(y, x, dir);
                }
            }
            if (!more)
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
                Gui.CommandDir = Level.CoordsToDir(coord.Y, coord.X);
            }
            if (Gui.CommandArg != 0)
            {
                CommandRep = Gui.CommandArg - 1;
                Player.RedrawNeeded.Set(RedrawFlag.PrState);
                Gui.CommandArg = 0;
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
            Gui.Print(Colour.Yellow, "Movement Commands", 1, 1);
            Gui.Print("7 8 9", 3, 1);
            Gui.Print(" \\|/", 4, 1);
            Gui.Print("4- -6 = Move", 5, 1);
            Gui.Print(" /|\\    (+Shift = run)", 6, 1);
            Gui.Print("1 2 3", 7, 1);
            Gui.Print("5 = Stand still", 8, 1);
            Gui.Print(". = Run", 9, 1);
            Gui.Print("< = Go up stairs", 10, 1);
            Gui.Print("> = Go down stairs", 11, 1);
            Gui.Print("T = Tunnel", 12, 1);
            Gui.Print("o = Open a door/chest", 13, 1);
            Gui.Print("c = Close a door", 14, 1);
            Gui.Print("D = Disarm a trap", 15, 1);
            Gui.Print("s = Search for traps", 16, 1);
            Gui.Print("B = Bash a stuck door", 17, 1);
            Gui.Print("S = Auto-search on/off", 18, 1);
            Gui.Print("+ = Auto-alter a space", 19, 1);
            Gui.Print("L = Locate player", 20, 1);
            Gui.Print("M = View the map", 21, 1);
            Gui.Print(Colour.Yellow, "Object Commands", 1, 25);
            Gui.Print("i = Show Inventory", 3, 25);
            Gui.Print("e = Show equipment", 4, 25);
            Gui.Print("w = Wield/wear an item", 5, 25);
            Gui.Print("t = Take off an item", 6, 25);
            Gui.Print("d = Drop object", 7, 25);
            Gui.Print("k = Destroy an item", 8, 25);
            Gui.Print("a = Aim a wand", 9, 25);
            Gui.Print("b = Browse a book", 10, 25);
            Gui.Print("r = Read a scroll", 11, 25);
            Gui.Print("z = Zap a rod", 12, 25);
            Gui.Print("A = Activate an artifact", 13, 25);
            Gui.Print("q = Quaff a potion", 14, 25);
            Gui.Print("r = Read a scroll", 15, 25);
            Gui.Print("u = Use a staff", 16, 25);
            Gui.Print("E = Eat some food", 17, 25);
            Gui.Print("f = Fire a missile weapon", 18, 25);
            Gui.Print("j = Jam spike in a door", 19, 25);
            Gui.Print("F = Fuel a light source", 20, 25);
            Gui.Print("K = Destroy trash objects", 21, 25);
            Gui.Print(Colour.Yellow, "Other Commands", 1, 52);
            Gui.Print("C = View your character", 3, 52);
            Gui.Print("J = View your journal", 4, 52);
            Gui.Print("R = Rest", 5, 52);
            Gui.Print("m = Spell/Mentalism power", 6, 52);
            Gui.Print("p = Racial power", 7, 52);
            Gui.Print("l = Look around", 8, 52);
            Gui.Print("H = How you feel here", 10, 52);
            Gui.Print("O = Show last message", 11, 52);
            Gui.Print("P = Show previous messages", 12, 52);
            Gui.Print("* = Target a creature", 13, 52);
            Gui.Print("/ = Identify a symbol", 14, 52);
            Gui.Print("x = Examine an object", 15, 52);
            Gui.Print("Q = Commit suicide", 16, 52);
            Gui.Print("h = View game help", 9, 52);
            if (Player.IsWizard)
            {
                Gui.Print("W = Wizard command", 17, 52);
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
                    Gui.CommandDir = Level.CoordsToDir(coord.Y, coord.X);
                }
            }
            if (Gui.CommandArg != 0)
            {
                CommandRep = Gui.CommandArg - 1;
                Player.RedrawNeeded.Set(RedrawFlag.PrState);
                Gui.CommandArg = 0;
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
                    Gui.CommandDir = Level.CoordsToDir(coord.Y, coord.X);
                }
            }
            if (Gui.CommandArg != 0)
            {
                CommandRep = Gui.CommandArg - 1;
                Player.RedrawNeeded.Set(RedrawFlag.PrState);
                Gui.CommandArg = 0;
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
            if (Gui.CommandArg <= 0)
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
                    Gui.CommandArg = -2;
                }
                else if (outVal[0] == '*')
                {
                    Gui.CommandArg = -1;
                }
                else
                {
                    if (int.TryParse(outVal, out int i))
                    {
                        Gui.CommandArg = i;
                    }
                    if (Gui.CommandArg <= 0)
                    {
                        return;
                    }
                }
            }
            if (Gui.CommandArg > 9999)
            {
                Gui.CommandArg = 9999;
            }
            SaveGame.Instance.EnergyUse = 100;
            SaveGame.Instance.Resting = Gui.CommandArg;
            Player.IsSearching = false;
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            Player.RedrawNeeded.Set(RedrawFlag.PrState);
            SaveGame.Instance.HandleStuff();
            Gui.Refresh();
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
                SaveGame.Instance.Running = Gui.CommandArg != 0 ? Gui.CommandArg : 1000;
                SaveGame.Instance.CommandEngine.RunOneStep(dir);
            }
        }

        private void DoCmdSearch()
        {
            if (Gui.CommandArg != 0)
            {
                CommandRep = Gui.CommandArg - 1;
                Player.RedrawNeeded.Set(RedrawFlag.PrState);
                Gui.CommandArg = 0;
            }
            SaveGame.Instance.EnergyUse = 100;
            SaveGame.Instance.CommandEngine.Search();
        }

        private void DoCmdStay(bool pickup)
        {
            GridTile cPtr = Level.Grid[Player.MapY][Player.MapX];
            if (Gui.CommandArg != 0)
            {
                CommandRep = Gui.CommandArg - 1;
                Player.RedrawNeeded.Set(RedrawFlag.PrState);
                Gui.CommandArg = 0;
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
                Gui.CommandNew = '_';
            }
        }

        private void DoCmdSuicide()
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
                    if (!Gui.GetCheck("Do you really want to end it all? "))
                    {
                        return;
                    }
                    Gui.PrintLine("Please verify SUICIDE by typing the '@' sign: ", 0, 0);
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
            SaveGame.Instance.DiedFrom = "Suicide";
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
            if (Gui.CommandArg != 0)
            {
                CommandRep = Gui.CommandArg - 1;
                Player.RedrawNeeded.Set(RedrawFlag.PrState);
                Gui.CommandArg = 0;
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
            if (Gui.CommandArg > 0)
            {
                CommandRep = Gui.CommandArg - 1;
                Player.RedrawNeeded.Set(RedrawFlag.PrState);
                Gui.CommandArg = 0;
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