// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Commands;
using Cthangband.Enumerations;
using Cthangband.Pantheon;
using Cthangband.Spells;
using Cthangband.StaticData;
using Cthangband.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cthangband
{
    [Serializable]
    internal class Store
    {
        public readonly StoreType StoreType;

        public int X;

        public int Y;

        private static readonly string[] _comment_7A =
                                    {"Arrgghh!", "You bastard!", "You hear someone sobbing...", "The shopkeeper howls in agony!"};

        private static readonly string[] _comment_7B =
            {"Damn!", "You bastard!", "The shopkeeper curses at you.", "The shopkeeper glares at you."};

        private static readonly string[] _comment_7C =
            {"Cool!", "You've made my day!", "The shopkeeper giggles.", "The shopkeeper laughs loudly."};

        private static readonly string[] _comment_7D =
            {"Yipee!", "I think I'll retire!", "The shopkeeper jumps for joy.", "The shopkeeper smiles gleefully."};

        private static readonly string[] _comment1 = { "Okay.", "Fine.", "Accepted!", "Agreed!", "Done!", "Taken!" };
        private readonly Item[] _stock;
        private readonly int _stockSize;
        private readonly int[] _table;
        private readonly int _tableNum;
        private CommandHandler _command;
        private bool _leaveStore;
        private StoreOwner _owner;
        private Player _player;
        private int _stockNum;
        private int _storeTop;

        public Store(StoreType storeType)
        {
            StoreType = storeType;
            _stockSize = Constants.StoreInvenMax;
            _stock = new Item[_stockSize];
            for (int k = 0; k < _stockSize; k++)
            {
                _stock[k] = new Item();
            }
            if (StoreType == StoreType.StoreBlack || StoreType == StoreType.StoreHome ||
                StoreType == StoreType.StorePawn || StoreType == StoreType.StoreHall ||
                StoreType == StoreType.StoreEmptyLot)
            {
                return;
            }
            const int tableSize = Constants.StoreChoices;
            _table = new int[tableSize];
            ItemIdentifier[] master = StoreFactory.GetStoreTable(StoreType);
            for (int k = 0; k < Constants.StoreChoices; k++)
            {
                int kIdx;
                ItemCategory tv = master[k].Category;
                int sv = master[k].SubCategory;
                for (kIdx = 1; kIdx < Profile.Instance.ItemTypes.Count; kIdx++)
                {
                    ItemType kPtr = Profile.Instance.ItemTypes[kIdx];
                    if (kPtr.Category == tv && kPtr.SubCategory == sv)
                    {
                        break;
                    }
                }
                if (kIdx == Profile.Instance.ItemTypes.Count)
                {
                    continue;
                }
                _table[_tableNum++] = kIdx;
            }
        }

        public string FeatureType
        {
            get
            {
                switch (StoreType)
                {
                    case StoreType.StoreGeneral:
                        return "GeneralStore";

                    case StoreType.StoreArmoury:
                        return "Armoury";

                    case StoreType.StoreWeapon:
                        return "Weaponsmiths";

                    case StoreType.StoreTemple:
                        return "Temple";

                    case StoreType.StoreAlchemist:
                        return "Alchemist";

                    case StoreType.StoreMagic:
                        return "MagicShop";

                    case StoreType.StoreBlack:
                        return "BlackMarket";

                    case StoreType.StoreHome:
                        return "Home";

                    case StoreType.StoreLibrary:
                        return "Bookstore";

                    case StoreType.StoreInn:
                        return "Inn";

                    case StoreType.StoreHall:
                        return "HallOfRecords";

                    case StoreType.StorePawn:
                        return "Pawnbrokers";

                    default:
                        return "GeneralStore";
                }
            }
        }

        public void EnterStore(Player player)
        {
            _player = player;
            _storeTop = 0;
            DisplayStore();
            _leaveStore = false;
            while (!_leaveStore)
            {
                Gui.PrintLine("", 1, 0);
                int tmpCha = _player.AbilityScores[Ability.Charisma].Adjusted;
                Gui.Clear(41);
                Gui.PrintLine(" ESC) Exit from Building.", 42, 0);
                if (StoreType == StoreType.StoreHome)
                {
                    Gui.PrintLine(" g) Get an item.", 42, 31);
                    Gui.PrintLine(" d) Drop an item.", 43, 31);
                }
                if (StoreType != StoreType.StoreHome && StoreType != StoreType.StoreHall)
                {
                    Gui.PrintLine(" p) Purchase an item.", 42, 31);
                    Gui.PrintLine(" s) Sell an item.", 43, 31);
                }
                if (StoreType == StoreType.StoreHall)
                {
                    Gui.PrintLine(" v) view racial Heroes.", 42, 31);
                    Gui.PrintLine(" c) view Class heroes.", 43, 31);
                }
                else
                {
                    Gui.PrintLine(" x) eXamine an item.", 42, 56);
                }
                switch (StoreType)
                {
                    case StoreType.StoreGeneral:
                        Gui.PrintLine(" r) Hire an escort.", 43, 56);
                        break;

                    case StoreType.StoreArmoury:
                        Gui.PrintLine(" r) Enchant your armour.", 43, 56);
                        break;

                    case StoreType.StoreWeapon:
                        Gui.PrintLine(" r) Enchant your weapon.", 43, 56);
                        break;

                    case StoreType.StoreTemple:
                        Gui.PrintLine(" v) Sacrifice Item.", 43, 0);
                        Gui.PrintLine(" r) buy Remove Curse.", 43, 56);
                        break;

                    case StoreType.StoreAlchemist:
                        Gui.PrintLine(" r) buy Restoration.", 43, 56);
                        break;

                    case StoreType.StoreMagic:
                        Gui.PrintLine(" r) Research an item.", 43, 56);
                        break;

                    case StoreType.StoreHome:
                        Gui.PrintLine(" r) Rest a while.", 43, 56);
                        break;

                    case StoreType.StoreLibrary:
                        Gui.PrintLine(" r) Research a spell.", 43, 56);
                        break;

                    case StoreType.StoreInn:
                        Gui.PrintLine(" r) hire a Room.", 43, 56);
                        break;

                    case StoreType.StoreHall:
                        Gui.PrintLine(" r) Buy a house.", 43, 56);
                        break;

                    case StoreType.StorePawn:
                        Gui.PrintLine(" r) Identify all.", 43, 56);
                        break;
                }
                Gui.Print("You may: ", 41, 0);
                Gui.RequestCommand(true);
                StoreProcessCommand();
                Gui.FullScreenOverlay = true;
                SaveGame.Instance.NoticeStuff();
                SaveGame.Instance.HandleStuff();
                if (_player.Inventory[InventorySlot.Pack].ItemType != null)
                {
                    const int item = InventorySlot.Pack;
                    Item oPtr = _player.Inventory[item];
                    if (StoreType != StoreType.StoreHome)
                    {
                        Profile.Instance.MsgPrint("Your pack is so full that you flee the Stores...");
                        _leaveStore = true;
                    }
                    else if (!StoreCheckNum(oPtr))
                    {
                        Profile.Instance.MsgPrint("Your pack is so full that you flee your home...");
                        _leaveStore = true;
                    }
                    else
                    {
                        Profile.Instance.MsgPrint("Your pack overflows!");
                        Item qPtr = new Item(oPtr);
                        string oName = qPtr.Description(true, 3);
                        Profile.Instance.MsgPrint($"You drop {oName} ({item.IndexToLabel()}).");
                        _player.Inventory.InvenItemIncrease(item, -255);
                        _player.Inventory.InvenItemDescribe(item);
                        _player.Inventory.InvenItemOptimize(item);
                        SaveGame.Instance.HandleStuff();
                        int itemPos = HomeCarry(qPtr);
                        if (itemPos >= 0)
                        {
                            _storeTop = itemPos / 26 * 26;
                            DisplayInventory();
                        }
                    }
                }
                if (tmpCha != _player.AbilityScores[Ability.Charisma].Adjusted)
                {
                    DisplayInventory();
                }
            }
            SaveGame.Instance.EnergyUse = 0;
            Gui.FullScreenOverlay = false;
            Gui.QueuedCommand = '\0';
            SaveGame.Instance.ViewingItemList = false;
            Profile.Instance.MsgPrint(null);
            Gui.Clear();
            Gui.SetBackground(Terminal.BackgroundImage.Overhead);
            _player.UpdatesNeeded.Set(UpdateFlags.UpdateView | UpdateFlags.UpdateLight);
            _player.UpdatesNeeded.Set(UpdateFlags.UpdateMonsters);
            _player.RedrawNeeded.Set(RedrawFlag.PrBasic | RedrawFlag.PrExtra | RedrawFlag.PrEquippy);
            _player.RedrawNeeded.Set(RedrawFlag.PrMap);
        }

        public void StoreInit()
        {
            _owner = StoreFactory.GetRandomOwner(StoreType);
            _stockNum = 0;
            for (int k = 0; k < _stockSize; k++)
            {
                _stock[k] = new Item();
            }
        }

        public void StoreMaint()
        {
            int oldRating = 0;
            if (SaveGame.Instance.Level != null)
            {
                oldRating = SaveGame.Instance.Level.TreasureRating;
            }
            if (StoreType == StoreType.StoreHome || StoreType == StoreType.StoreHall ||
                StoreType == StoreType.StorePawn || StoreType == StoreType.StoreEmptyLot)
            {
                return;
            }
            int j = _stockNum;
            j -= Program.Rng.DieRoll(Constants.StoreTurnover);
            if (j > Constants.StoreMaxKeep)
            {
                j = Constants.StoreMaxKeep;
            }
            if (j < Constants.StoreMinKeep)
            {
                j = Constants.StoreMinKeep;
            }
            if (j < 0)
            {
                j = 0;
            }
            while (_stockNum > j)
            {
                StoreDelete();
            }
            j = _stockNum;
            j += Program.Rng.DieRoll(Constants.StoreTurnover);
            if (j > Constants.StoreMaxKeep)
            {
                j = Constants.StoreMaxKeep;
            }
            if (j < Constants.StoreMinKeep)
            {
                j = Constants.StoreMinKeep;
            }
            if (j >= _stockSize)
            {
                j = _stockSize - 1;
            }
            if (j > 4 && StoreType == StoreType.StoreInn)
            {
                j = 4;
            }
            while (_stockNum < j)
            {
                StoreCreate();
            }
            if (SaveGame.Instance.Level != null)
            {
                SaveGame.Instance.Level.TreasureRating = oldRating;
            }
        }

        public void StoreShuffle()
        {
            if (StoreType == StoreType.StoreHome || StoreType == StoreType.StoreHall ||
                StoreType == StoreType.StorePawn || StoreType == StoreType.StoreEmptyLot)
            {
                return;
            }
            _owner = StoreFactory.GetRandomOwner(StoreType);
            for (int i = 0; i < _stockNum; i++)
            {
                Item oPtr = _stock[i];
                if (string.IsNullOrEmpty(oPtr.RandartName))
                {
                    oPtr.Discount = 50;
                }
                oPtr.IdentifyFlags.Clear(Constants.IdentFixed);
                oPtr.Inscription = "on sale";
            }
        }

        private void DisplayEntry(int pos)
        {
            string oName;
            int maxwid;
            Item oPtr = _stock[pos];
            int i = pos % 26;
            string outVal = $"{i.IndexToLetter()}) ";
            Gui.PrintLine(outVal, i + 6, 0);
            Colour a = oPtr.ItemType.Colour;
            char c = oPtr.ItemType.Character;
            Gui.Place(a, c, i + 6, 3);
            if (StoreType == StoreType.StoreHome)
            {
                maxwid = 75;
                maxwid -= 10;
                oName = oPtr.Description(true, 3);
                if (maxwid < oName.Length)
                {
                    oName = oName.Substring(0, maxwid);
                }
                Gui.Print(oPtr.Category.ToAttr(), oName, i + 6, 5);
                int wgt = oPtr.Weight;
                outVal = $"{wgt / 10,3}.{wgt % 10} lb";
                Gui.Print(outVal, i + 6, 68);
            }
            else
            {
                maxwid = 65;
                maxwid -= 7;
                oName = StoreType == StoreType.StorePawn ? oPtr.Description(true, 3) : oPtr.StoreDescription(true, 3);
                if (maxwid < oName.Length)
                {
                    oName = oName.Substring(0, maxwid);
                }
                Gui.Print(oPtr.Category.ToAttr(), oName, i + 6, 5);
                int wgt = oPtr.Weight;
                outVal = $"{wgt / 10,3}.{wgt % 10}";
                Gui.Print(outVal, i + 6, 61);
                int x;
                if (oPtr.IdentifyFlags.IsSet(Constants.IdentFixed))
                {
                    x = PriceItem(oPtr, _owner.MinInflate, false);
                    outVal = $"{x,9} F";
                    Gui.Print(outVal, i + 6, 68);
                }
                else
                {
                    x = PriceItem(oPtr, _owner.MinInflate, false);
                    x += x / 10;
                    outVal = $"{x,9}  ";
                    Gui.Print(outVal, i + 6, 68);
                }
            }
        }

        private void DisplayInventory()
        {
            int k;
            for (k = 0; k < 26; k++)
            {
                if (_storeTop + k >= _stockNum)
                {
                    break;
                }
                DisplayEntry(_storeTop + k);
            }
            for (int i = k; i < 27; i++)
            {
                Gui.PrintLine("", i + 6, 0);
            }
            Gui.Print("        ", 5, 20);
            if (_stockNum > 26)
            {
                Gui.PrintLine("-more-", k + 6, 3);
                Gui.Print($"(Page {(_storeTop / 26) + 1})", 5, 20);
            }
        }

        private void DisplayStore()
        {
            Gui.Clear();
            Gui.SetBackground(Terminal.BackgroundImage.Normal);
            if (StoreType == StoreType.StoreHome)
            {
                Gui.Print("Your Home", 3, 30);
                Gui.Print("Item Description", 5, 3);
                Gui.Print("Weight", 5, 70);
            }
            if (StoreType == StoreType.StoreHall)
            {
                Gui.Print("Hall Of Records", 3, 30);
            }
            if (StoreType != StoreType.StoreHome && StoreType != StoreType.StoreHall)
            {
                string storeName = StaticResources.Instance.FloorTileTypes[FeatureType].Description;
                string ownerName = _owner.OwnerName;
                string raceName = Race.RaceInfo[_owner.OwnerRace].Title;
                string buf = $"{ownerName} ({raceName})";
                Gui.Print(buf, 3, 10);
                buf = $"{storeName} ({_owner.MaxCost})";
                Gui.PrintLine(buf, 3, 50);
                Gui.Print("Item Description", 5, 3);
                Gui.Print("Weight", 5, 60);
                Gui.Print("Price", 5, 72);
            }
            StorePrtGold();
            DisplayInventory();
        }

        private void DoStoreBrowse(Item oPtr)
        {
            int num = 0;
            int[] spells = new int[64];
            int sval = oPtr.ItemSubCategory;
            for (int spell = 0; spell < 32; spell++)
            {
                if ((GlobalData.BookSpellFlags[sval] & (1u << spell)) != 0)
                {
                    spells[num++] = spell;
                }
            }
            Gui.Save();
            SaveGame.Instance.Player.PrintSpells(spells, num, 1, 20, oPtr.Category.SpellBookToToRealm());
            Gui.PrintLine("", 0, 0);
            Gui.Print("[Press any key to continue]", 0, 23);
            Gui.Inkey();
            Gui.Load();
        }

        private Town GetEscortDestination(Dictionary<char, Town> towns)
        {
            Gui.Save();
            var keys = towns.Keys.ToList();
            keys.Sort();
            string outVal = $"Destination town ({keys[0].ToString().ToLower()} to {keys[keys.Count - 1].ToString().ToLower()})? ";
            for (int i = 0; i < keys.Count; i++)
            {
                Gui.Print(Colour.White, $" {keys[i].ToString().ToLower()}) {towns[keys[i]].Name}".PadRight(60), i + 1, 20);
            }
            Gui.Print(Colour.White, "".PadRight(60), keys.Count + 1, 20);
            while (Gui.GetCom(outVal, out char choice))
            {
                choice = choice.ToString().ToUpper()[0];
                foreach (var c in keys)
                {
                    if (choice == c)
                    {
                        Gui.Load();
                        return towns[c];
                    }
                }
            }
            Gui.Load();
            return null;
        }

        private GodName GetSacrificeTarget()
        {
            Gui.Save();
            var deities = _player.Religion.GetAllDeities();
            var names = new List<string>();
            var keys = new List<char>();
            foreach (var deity in deities)
            {
                names.Add(deity.LongName);
                keys.Add(deity.LongName[0]);
            }
            names.Sort();
            keys.Sort();
            string outVal = $"Destination town ({keys[0].ToString().ToLower()} to {keys[keys.Count - 1].ToString().ToLower()})? ";
            for (int i = 0; i < keys.Count; i++)
            {
                Gui.Print(Colour.White, $" {keys[i].ToString().ToLower()}) {names[i]}".PadRight(60), i + 1, 20);
            }
            Gui.Print(Colour.White, "".PadRight(60), keys.Count + 1, 20);
            while (Gui.GetCom(outVal, out char choice))
            {
                choice = choice.ToString().ToUpper()[0];
                foreach (var c in keys)
                {
                    if (choice == c)
                    {
                        foreach (var deity in deities)
                        {
                            if (deity.ShortName.StartsWith(choice.ToString()))
                            {
                                Gui.Load();
                                return deity.Name;
                            }
                        }
                        return GodName.None;
                    }
                }
            }
            Gui.Load();
            return GodName.None;
        }

        private bool GetStock(out int comVal, string pmt, int i, int j)
        {
            char command;
            Profile.Instance.MsgPrint(null);
            comVal = -1;
            string outVal = $"(Items {i.IndexToLetter()}-{j.IndexToLetter()}, ESC to exit) {pmt}";
            while (Gui.GetCom(outVal, out command))
            {
                int k = char.IsLower(command) ? command.LetterToNumber() : -1;
                if (k >= i && k <= j)
                {
                    comVal = k;
                    break;
                }
            }
            Gui.PrintLine("", 0, 0);
            return command != '\x1b';
        }

        private int HomeCarry(Item oPtr)
        {
            int slot;
            Item jPtr;
            for (slot = 0; slot < _stockNum; slot++)
            {
                jPtr = _stock[slot];
                if (jPtr.CanAbsorb(oPtr))
                {
                    jPtr.Absorb(oPtr);
                    return slot;
                }
            }
            if (_stockNum >= _stockSize)
            {
                return -1;
            }
            int value = oPtr.Value();
            for (slot = 0; slot < _stockNum; slot++)
            {
                jPtr = _stock[slot];
                if (oPtr.Category > jPtr.Category)
                {
                    break;
                }
                if (oPtr.Category < jPtr.Category)
                {
                    continue;
                }
                if (!oPtr.IsFlavourAware())
                {
                    continue;
                }
                if (!jPtr.IsFlavourAware())
                {
                    break;
                }
                if (oPtr.ItemSubCategory < jPtr.ItemSubCategory)
                {
                    break;
                }
                if (oPtr.ItemSubCategory > jPtr.ItemSubCategory)
                {
                    continue;
                }
                if (!oPtr.IsKnown())
                {
                    continue;
                }
                if (!jPtr.IsKnown())
                {
                    break;
                }
                if (oPtr.Category == ItemCategory.Rod)
                {
                    if (oPtr.TypeSpecificValue < jPtr.TypeSpecificValue)
                    {
                        break;
                    }
                    if (oPtr.TypeSpecificValue > jPtr.TypeSpecificValue)
                    {
                        continue;
                    }
                }
                int jValue = jPtr.Value();
                if (value > jValue)
                {
                    break;
                }
                if (value < jValue)
                {
                }
            }
            for (int i = _stockNum; i > slot; i--)
            {
                _stock[i] = new Item(_stock[i - 1]);
            }
            _stockNum++;
            _stock[slot] = new Item(oPtr);
            return slot;
        }

        private bool IsBlessed(Item oPtr)
        {
            FlagSet f1 = new FlagSet();
            FlagSet f2 = new FlagSet();
            FlagSet f3 = new FlagSet();
            oPtr.GetMergedFlags(f1, f2, f3);
            return f3.IsSet(ItemFlag3.Blessed);
        }

        private void MassProduce(Item oPtr)
        {
            int size = 1;
            int discount = 0;
            int cost = oPtr.Value();
            switch (oPtr.Category)
            {
                case ItemCategory.Food:
                case ItemCategory.Flask:
                case ItemCategory.Light:
                    {
                        if (cost <= 5)
                        {
                            size += MassRoll(3, 5);
                        }
                        if (cost <= 20)
                        {
                            size += MassRoll(3, 5);
                        }
                        break;
                    }
                case ItemCategory.Potion:
                case ItemCategory.Scroll:
                    {
                        if (cost <= 60)
                        {
                            size += MassRoll(3, 5);
                        }
                        if (cost <= 240)
                        {
                            size += MassRoll(1, 5);
                        }
                        break;
                    }
                case ItemCategory.LifeBook:
                case ItemCategory.SorceryBook:
                case ItemCategory.NatureBook:
                case ItemCategory.ChaosBook:
                case ItemCategory.DeathBook:
                case ItemCategory.TarotBook:
                case ItemCategory.FolkBook:
                case ItemCategory.CorporealBook:
                    {
                        if (cost <= 50)
                        {
                            size += MassRoll(2, 3);
                        }
                        if (cost <= 500)
                        {
                            size += MassRoll(1, 3);
                        }
                        break;
                    }
                case ItemCategory.SoftArmor:
                case ItemCategory.HardArmor:
                case ItemCategory.Shield:
                case ItemCategory.Gloves:
                case ItemCategory.Boots:
                case ItemCategory.Cloak:
                case ItemCategory.Helm:
                case ItemCategory.Crown:
                case ItemCategory.Sword:
                case ItemCategory.Polearm:
                case ItemCategory.Hafted:
                case ItemCategory.Digging:
                case ItemCategory.Bow:
                    {
                        if (oPtr.RareItemTypeIndex != 0)
                        {
                            break;
                        }
                        if (cost <= 10)
                        {
                            size += MassRoll(3, 5);
                        }
                        if (cost <= 100)
                        {
                            size += MassRoll(3, 5);
                        }
                        break;
                    }
                case ItemCategory.Spike:
                case ItemCategory.Shot:
                case ItemCategory.Arrow:
                case ItemCategory.Bolt:
                    {
                        if (cost <= 5)
                        {
                            size += MassRoll(5, 5);
                        }
                        if (cost <= 50)
                        {
                            size += MassRoll(5, 5);
                        }
                        if (cost <= 500)
                        {
                            size += MassRoll(5, 5);
                        }
                        break;
                    }
            }
            if (cost < 5)
            {
                discount = 0;
            }
            else if (Program.Rng.RandomLessThan(25) == 0)
            {
                discount = 25;
            }
            else if (Program.Rng.RandomLessThan(150) == 0)
            {
                discount = 50;
            }
            else if (Program.Rng.RandomLessThan(300) == 0)
            {
                discount = 75;
            }
            else if (Program.Rng.RandomLessThan(500) == 0)
            {
                discount = 90;
            }
            if (!string.IsNullOrEmpty(oPtr.RandartName))
            {
                discount = 0;
            }
            oPtr.Discount = discount;
            oPtr.Count = size - (size * discount / 100);
        }

        private int MassRoll(int num, int max)
        {
            int t = 0;
            for (int i = 0; i < num; i++)
            {
                t += Program.Rng.RandomLessThan(max);
            }
            return t;
        }

        private void MoveHouse(int oldTown, int newTown)
        {
            Store newStore = Array.Find(SaveGame.Instance.Towns[newTown].Stores, store => store.StoreType == StoreType.StoreHome);
            Store oldStore = Array.Find(SaveGame.Instance.Towns[oldTown].Stores, store => store.StoreType == StoreType.StoreHome);
            if (oldStore == null)
            {
                return;
            }
            if (newStore == null)
            {
                return;
            }
            newStore._stockNum = oldStore._stockNum;
            for (int i = 0; i < oldStore._stock.Length; i++)
            {
                newStore._stock[i] = new Item(oldStore._stock[i]);
                oldStore._stock[i] = new Item();
            }
            oldStore._stockNum = 0;
        }

        private int PriceItem(Item oPtr, int greed, bool flip)
        {
            int adjust;
            int price = oPtr.Value();
            if (price <= 0)
            {
                return 0;
            }
            int factor = 100;
            factor += _player.AbilityScores[Ability.Charisma].ChaPriceAdjustment;
            if (flip)
            {
                adjust = 100 + (300 - (greed + factor));
                if (adjust > 100)
                {
                    adjust = 100;
                }
                if (StoreType == StoreType.StoreBlack)
                {
                    price /= 2;
                }
                if (StoreType == StoreType.StorePawn)
                {
                    price /= 3;
                }
            }
            else
            {
                adjust = 100 + (greed + factor - 300);
                if (adjust < 100)
                {
                    adjust = 100;
                }
                if (StoreType == StoreType.StoreBlack)
                {
                    price *= 2;
                }
                if (StoreType == StoreType.StorePawn)
                {
                    price /= 3;
                }
            }
            price = ((price * adjust) + 50) / 100;
            if (price <= 0)
            {
                return 1;
            }
            return price;
        }

        private void PurchaseAnalyze(int price, int value, int guess)
        {
            if (value <= 0 && price > value)
            {
                Profile.Instance.MsgPrint(_comment_7A[Program.Rng.RandomLessThan(_comment_7A.Length)]);
                Gui.PlaySound(SoundEffect.StoreSoldWorthless);
            }
            else if (value < guess && price > value)
            {
                Profile.Instance.MsgPrint(_comment_7B[Program.Rng.RandomLessThan(_comment_7B.Length)]);
                Gui.PlaySound(SoundEffect.StoreSoldBargain);
            }
            else if (value > guess && value < 4 * guess && price < value)
            {
                Profile.Instance.MsgPrint(_comment_7C[Program.Rng.RandomLessThan(_comment_7C.Length)]);
                Gui.PlaySound(SoundEffect.StoreSoldCheaply);
            }
            else if (value > guess && price < value)
            {
                Profile.Instance.MsgPrint(_comment_7D[Program.Rng.RandomLessThan(_comment_7D.Length)]);
                Gui.PlaySound(SoundEffect.StoreSoldExtraCheaply);
            }
        }

        private bool PurchaseHaggle(Item oPtr, out int price)
        {
            int finalAsk = PriceItem(oPtr, _owner.MinInflate, false);
            Profile.Instance.MsgPrint("You quickly agree upon the price.");
            Profile.Instance.MsgPrint(null);
            finalAsk += finalAsk / 10;
            const string pmt = "Final Offer";
            finalAsk *= oPtr.Count;
            price = finalAsk;
            string outVal = $"{pmt} :  {finalAsk}";
            Gui.Print(outVal, 1, 0);
            return !Gui.GetCheck("Accept deal? ");
        }

        private void RoomRest(bool toDusk)
        {
            if (toDusk)
            {
                _player.GameTime.ToNextDusk();
                Profile.Instance.MsgPrint("You awake, ready for the night.");
                Profile.Instance.MsgPrint("You eat a tasty supper.");
            }
            else
            {
                _player.GameTime.ToNextDawn();
                Profile.Instance.MsgPrint("You awake refreshed for the new day.");
                Profile.Instance.MsgPrint("You eat a hearty breakfast.");
            }
            _player.Religion.DecayFavour();
            _player.UpdatesNeeded.Set(UpdateFlags.UpdateHealth | UpdateFlags.UpdateMana);
            _player.SetFood(Constants.PyFoodMax - 1);
            foreach (Town town in SaveGame.Instance.Towns)
            {
                foreach (Store store in town.Stores)
                {
                    switch (store.StoreType)
                    {
                        case StoreType.StoreGeneral:
                        case StoreType.StoreArmoury:
                        case StoreType.StoreWeapon:
                        case StoreType.StoreTemple:
                        case StoreType.StoreAlchemist:
                        case StoreType.StoreMagic:
                        case StoreType.StoreBlack:
                        case StoreType.StoreLibrary:
                            store.StoreMaint();
                            break;

                        case StoreType.StoreHome:
                        case StoreType.StoreInn:
                        case StoreType.StoreHall:
                        case StoreType.StorePawn:
                        case StoreType.StoreEmptyLot:
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            _player.TimedHaste = 0;
            _player.TimedSlow = 0;
            _player.TimedBlindness = 0;
            _player.TimedParalysis = 0;
            _player.TimedConfusion = 0;
            _player.TimedFear = 0;
            _player.TimedHallucinations = 0;
            _player.TimedPoison = 0;
            _player.TimedBleeding = 0;
            _player.TimedStun = 0;
            _player.TimedProtectionFromEvil = 0;
            _player.TimedInvulnerability = 0;
            _player.TimedHeroism = 0;
            _player.TimedSuperheroism = 0;
            _player.TimedStoneskin = 0;
            _player.TimedBlessing = 0;
            _player.TimedSeeInvisibility = 0;
            _player.TimedEtherealness = 0;
            _player.TimedInfravision = 0;
            _player.TimedAcidResistance = 0;
            _player.TimedLightningResistance = 0;
            _player.TimedFireResistance = 0;
            _player.TimedColdResistance = 0;
            _player.TimedPoisonResistance = 0;
            _player.Health = _player.MaxHealth;
            _player.Mana = _player.MaxMana;
            _player.SetTimedBlindness(0);
            _player.SetTimedConfusion(0);
            _player.TimedStun = 0;
            SaveGame.Instance.NewLevelFlag = true;
            SaveGame.Instance.CameFrom = LevelStart.StartWalk;
        }

        private void SacrificeItem()
        {
            var godName = GetSacrificeTarget();
            if (godName == GodName.None)
            {
                return;
            }
            var deity = _player.Religion.GetNamedDeity(godName);
            string pmt = "Sacrifice which item? ";
            SaveGame.Instance.ItemFilter = null;
            if (!SaveGame.Instance.GetItem(out int item, pmt, true, true, false))
            {
                if (item == -2)
                {
                    Profile.Instance.MsgPrint("You have nothing to sacrifice.");
                    return;
                }
            }
            Item oPtr = item >= 0 ? _player.Inventory[item] : SaveGame.Instance.Level.Items[0 - item];
            if (item >= InventorySlot.MeleeWeapon && oPtr.IsCursed())
            {
                Profile.Instance.MsgPrint("Hmmm, it seems to be cursed.");
                return;
            }
            int amt = 1;
            if (oPtr.Count > 1)
            {
                amt = Gui.GetQuantity(null, oPtr.Count, true);
                if (amt <= 0)
                {
                    return;
                }
            }
            Item qPtr = new Item(oPtr) { Count = amt };
            string oName = qPtr.Description(true, 3);
            qPtr.Inscription = "";
            int finalAsk = PriceItem(qPtr, _owner.MinInflate, true) * qPtr.Count;
            _player.Inventory.InvenItemIncrease(item, -amt);
            _player.Inventory.InvenItemDescribe(item);
            _player.Inventory.InvenItemOptimize(item);
            SaveGame.Instance.HandleStuff();
            var deityName = deity.ShortName;
            if (finalAsk <= 0)
            {
                finalAsk = -100;
            }
            var favour = finalAsk / 10;
            var oldFavour = deity.AdjustedFavour;
            _player.Religion.AddFavour(godName, favour);
            var newFavour = deity.AdjustedFavour;
            var change = newFavour - oldFavour;
            if (change < 0)
            {
                Profile.Instance.MsgPrint($"{deityName} is displeased with your sacrifice!");
            }
            else if (change == 0)
            {
                Profile.Instance.MsgPrint($"{deityName} is indifferent to your sacrifice!");
            }
            else if (change == 1)
            {
                Profile.Instance.MsgPrint($"{deityName} approves of your sacrifice!");
            }
            else if (change == 2)
            {
                Profile.Instance.MsgPrint($"{deityName} likes your sacrifice!");
            }
            else if (change == 3)
            {
                Profile.Instance.MsgPrint($"{deityName} loves your sacrifice!");
            }
            else
            {
                Profile.Instance.MsgPrint($"{deityName} is delighted by your sacrifice!");
            }
            _player.UpdatesNeeded.Set(UpdateFlags.UpdateHealth | UpdateFlags.UpdateMana);
        }

        private void SayComment_1()
        {
            Profile.Instance.MsgPrint(_comment1[Program.Rng.RandomLessThan(_comment1.Length)]);
        }

        private bool SellHaggle(Item oPtr, out int price)
        {
            int finalAsk = PriceItem(oPtr, _owner.MinInflate, true);
            int purse = _owner.MaxCost;
            if (finalAsk >= purse)
            {
                Profile.Instance.MsgPrint("You instantly agree upon the price.");
                Profile.Instance.MsgPrint(null);
                finalAsk = purse;
            }
            else
            {
                Profile.Instance.MsgPrint("You quickly agree upon the price.");
                Profile.Instance.MsgPrint(null);
                finalAsk -= finalAsk / 10;
            }
            const string pmt = "Final Offer";
            finalAsk *= oPtr.Count;
            price = finalAsk;
            string outVal = $"{pmt} :  {finalAsk}";
            Gui.Print(outVal, 1, 0);
            return !Gui.GetCheck("Accept deal? ");
        }

        private bool ServiceHaggle(int serviceCost, out int price)
        {
            int finalAsk = serviceCost;
            Profile.Instance.MsgPrint("You quickly agree upon the price.");
            Profile.Instance.MsgPrint(null);
            finalAsk += finalAsk / 10;
            price = finalAsk;
            const string pmt = "Final Offer";
            string outVal = $"{pmt} :  {finalAsk}";
            Gui.Print(outVal, 1, 0);
            return !Gui.GetCheck("Accept deal? ");
        }

        private int StoreCarry(Item oPtr)
        {
            int slot;
            Item jPtr;
            int value = oPtr.Value();
            if (value <= 0)
            {
                return -1;
            }
            if (StoreType != StoreType.StorePawn)
            {
                oPtr.IdentifyFlags.Set(Constants.IdentMental);
                oPtr.Inscription = "";
            }
            for (slot = 0; slot < _stockNum; slot++)
            {
                jPtr = _stock[slot];
                if (StoreObjectSimilar(jPtr, oPtr))
                {
                    StoreObjectAbsorb(jPtr, oPtr);
                    return slot;
                }
            }
            if (_stockNum >= _stockSize)
            {
                return -1;
            }
            for (slot = 0; slot < _stockNum; slot++)
            {
                jPtr = _stock[slot];
                if (oPtr.Category > jPtr.Category)
                {
                    break;
                }
                if (oPtr.Category < jPtr.Category)
                {
                    continue;
                }
                if (oPtr.ItemSubCategory < jPtr.ItemSubCategory)
                {
                    break;
                }
                if (oPtr.ItemSubCategory > jPtr.ItemSubCategory)
                {
                    continue;
                }
                if (oPtr.Category == ItemCategory.Rod)
                {
                    if (oPtr.TypeSpecificValue < jPtr.TypeSpecificValue)
                    {
                        break;
                    }
                    if (oPtr.TypeSpecificValue > jPtr.TypeSpecificValue)
                    {
                        continue;
                    }
                }
                int jValue = jPtr.Value();
                if (value > jValue)
                {
                    break;
                }
                if (value < jValue)
                {
                }
            }
            for (int i = _stockNum; i > slot; i--)
            {
                _stock[i] = _stock[i - 1];
            }
            _stockNum++;
            _stock[slot] = oPtr;
            return slot;
        }

        private bool StoreCheckNum(Item oPtr)
        {
            int i;
            Item jPtr;
            if (_stockNum < _stockSize)
            {
                return true;
            }
            if (StoreType == StoreType.StoreHome)
            {
                for (i = 0; i < _stockNum; i++)
                {
                    jPtr = _stock[i];
                    if (jPtr.CanAbsorb(oPtr))
                    {
                        return true;
                    }
                }
            }
            else
            {
                for (i = 0; i < _stockNum; i++)
                {
                    jPtr = _stock[i];
                    if (StoreObjectSimilar(jPtr, oPtr))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void StoreCreate()
        {
            if (_stockNum >= _stockSize)
            {
                return;
            }
            for (int tries = 0; tries < 4; tries++)
            {
                int i;
                int level;
                ItemType itemType;
                if (StoreType == StoreType.StoreBlack)
                {
                    level = 35 + Program.Rng.RandomLessThan(35);
                    itemType = ItemType.RandomItemType(level);
                    if (itemType == null)
                    {
                        continue;
                    }
                }
                else
                {
                    i = _table[Program.Rng.RandomLessThan(_tableNum)];
                    level = Program.Rng.RandomBetween(1, Constants.StoreObjLevel);
                    itemType = Profile.Instance.ItemTypes[i];
                }
                Item qPtr = new Item();
                qPtr.AssignItemType(itemType);
                qPtr.ApplyMagic(level, false, false, false);
                if (qPtr.Category == ItemCategory.Light)
                {
                    if (qPtr.ItemSubCategory == LightType.Torch)
                    {
                        qPtr.TypeSpecificValue = Constants.FuelTorch / 2;
                    }
                    if (qPtr.ItemSubCategory == LightType.Lantern)
                    {
                        qPtr.TypeSpecificValue = Constants.FuelLamp / 2;
                    }
                }
                qPtr.BecomeKnown();
                qPtr.IdentifyFlags.Set(Constants.IdentStoreb);
                if (qPtr.Category == ItemCategory.Chest)
                {
                    continue;
                }
                if (StoreType == StoreType.StoreBlack)
                {
                    if (qPtr.Value() < 10)
                    {
                        continue;
                    }
                }
                else
                {
                    if (qPtr.Value() <= 0)
                    {
                        continue;
                    }
                }
                MassProduce(qPtr);
                StoreCarry(qPtr);
                break;
            }
        }

        private void StoreDelete()
        {
            int what = Program.Rng.RandomLessThan(_stockNum);
            int num = _stock[what].Count;
            if (Program.Rng.RandomLessThan(100) < 50)
            {
                num = (num + 1) / 2;
            }
            if (Program.Rng.RandomLessThan(100) < 50)
            {
                num = 1;
            }
            StoreItemIncrease(what, -num);
            StoreItemOptimize(what);
        }

        private void StoreExamine()
        {
            if (_stockNum <= 0)
            {
                Profile.Instance.MsgPrint(StoreType == StoreType.StoreHome
                    ? "Your home is empty."
                    : "I am currently out of stock.");
                return;
            }
            int i = _stockNum - _storeTop;
            if (i > 26)
            {
                i = 26;
            }
            const string outVal = "Which item do you want to examine? ";
            if (!GetStock(out int item, outVal, 0, i - 1))
            {
                return;
            }
            item += _storeTop;
            Item oPtr = _stock[item];
            if (oPtr.Category == ItemCategory.LifeBook || oPtr.Category == ItemCategory.SorceryBook ||
                oPtr.Category == ItemCategory.NatureBook || oPtr.Category == ItemCategory.ChaosBook ||
                oPtr.Category == ItemCategory.DeathBook ||
                oPtr.Category == ItemCategory.CorporealBook ||
                oPtr.Category == ItemCategory.TarotBook || oPtr.Category == ItemCategory.FolkBook)
            {
                switch (oPtr.Category)
                {
                    case ItemCategory.LifeBook:
                        if (_player.Realm1 == Realm.Life || _player.Realm2 == Realm.Life)
                        {
                            DoStoreBrowse(oPtr);
                            return;
                        }
                        break;

                    case ItemCategory.SorceryBook:
                        if (_player.Realm1 == Realm.Sorcery || _player.Realm2 == Realm.Sorcery)
                        {
                            DoStoreBrowse(oPtr);
                            return;
                        }
                        break;

                    case ItemCategory.NatureBook:
                        if (_player.Realm1 == Realm.Nature || _player.Realm2 == Realm.Nature)
                        {
                            DoStoreBrowse(oPtr);
                            return;
                        }
                        break;

                    case ItemCategory.ChaosBook:
                        if (_player.Realm1 == Realm.Chaos || _player.Realm2 == Realm.Chaos)
                        {
                            DoStoreBrowse(oPtr);
                            return;
                        }
                        break;

                    case ItemCategory.DeathBook:
                        if (_player.Realm1 == Realm.Death || _player.Realm2 == Realm.Death)
                        {
                            DoStoreBrowse(oPtr);
                            return;
                        }
                        break;

                    case ItemCategory.CorporealBook:
                        if (_player.Realm1 == Realm.Corporeal || _player.Realm2 == Realm.Corporeal)
                        {
                            DoStoreBrowse(oPtr);
                            return;
                        }
                        break;

                    case ItemCategory.TarotBook:
                        if (_player.Realm1 == Realm.Tarot || _player.Realm2 == Realm.Tarot)
                        {
                            DoStoreBrowse(oPtr);
                            return;
                        }
                        break;

                    case ItemCategory.FolkBook:
                        if (_player.Realm1 == Realm.Folk || _player.Realm2 == Realm.Folk)
                        {
                            DoStoreBrowse(oPtr);
                            return;
                        }
                        break;
                }
                Profile.Instance.MsgPrint("The spells in the book are unintelligible to you.");
                return;
            }
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

        private void StoreItemIncrease(int item, int num)
        {
            Item oPtr = _stock[item];
            int cnt = oPtr.Count + num;
            if (cnt > 255)
            {
                cnt = 255;
            }
            else if (cnt < 0)
            {
                cnt = 0;
            }
            num = cnt - oPtr.Count;
            oPtr.Count += num;
        }

        private void StoreItemOptimize(int item)
        {
            int j;
            Item oPtr = _stock[item];
            if (oPtr.ItemType == null)
            {
                return;
            }
            if (oPtr.Count > 0)
            {
                return;
            }
            _stockNum--;
            for (j = item; j < _stockNum; j++)
            {
                _stock[j] = _stock[j + 1];
            }
            _stock[j] = new Item();
        }

        private void StoreObjectAbsorb(Item oPtr, Item jPtr)
        {
            int total = oPtr.Count + jPtr.Count;
            oPtr.Count = total > 99 ? 99 : total;
        }

        private bool StoreObjectSimilar(Item oPtr, Item jPtr)
        {
            if (oPtr == jPtr)
            {
                return false;
            }
            if (oPtr.ItemType != jPtr.ItemType)
            {
                return false;
            }
            if (oPtr.TypeSpecificValue != jPtr.TypeSpecificValue)
            {
                return false;
            }
            if (oPtr.BonusToHit != jPtr.BonusToHit)
            {
                return false;
            }
            if (oPtr.BonusDamage != jPtr.BonusDamage)
            {
                return false;
            }
            if (oPtr.BonusArmourClass != jPtr.BonusArmourClass)
            {
                return false;
            }
            if (oPtr.FixedArtifactIndex != jPtr.FixedArtifactIndex)
            {
                return false;
            }
            if (oPtr.RareItemTypeIndex != jPtr.RareItemTypeIndex)
            {
                return false;
            }
            if (!string.IsNullOrEmpty(oPtr.RandartName) || !string.IsNullOrEmpty(jPtr.RandartName))
            {
                return false;
            }
            if (oPtr.RandartFlags1.Value != jPtr.RandartFlags1.Value ||
                oPtr.RandartFlags2.Value != jPtr.RandartFlags2.Value ||
                oPtr.RandartFlags3.Value != jPtr.RandartFlags3.Value)
            {
                return false;
            }
            if (oPtr.BonusPowerType != 0 || jPtr.BonusPowerType != 0)
            {
                return false;
            }
            if (oPtr.RechargeTimeLeft != 0 || jPtr.RechargeTimeLeft != 0)
            {
                return false;
            }
            if (oPtr.BaseArmourClass != jPtr.BaseArmourClass)
            {
                return false;
            }
            if (oPtr.DamageDice != jPtr.DamageDice)
            {
                return false;
            }
            if (oPtr.DamageDiceSides != jPtr.DamageDiceSides)
            {
                return false;
            }
            if (oPtr.Category == ItemCategory.Chest)
            {
                return false;
            }
            if (oPtr.Discount != jPtr.Discount)
            {
                return false;
            }
            return true;
        }

        private void DoCmdStudy()
        {
            string spellType = _player.Spellcasting.Type == CastingType.Arcane ? "spell" : "prayer";
            // If we don't have a realm then we can't do anything
            if (_player.Realm1 == 0)
            {
                Profile.Instance.MsgPrint("You cannot read books!");
                return;
            }
            // We can't learn spells if we're blind or confused
            if (_player.TimedBlindness != 0)
            {
                Profile.Instance.MsgPrint("You cannot see!");
                return;
            }
            if (_player.TimedConfusion != 0)
            {
                Profile.Instance.MsgPrint("You are too confused!");
                return;
            }
            // We can only learn new spells if we have spare slots
            if (_player.SpareSpellSlots == 0)
            {
                Profile.Instance.MsgPrint($"You cannot learn any new {spellType}s!");
                return;
            }
            string plural = _player.SpareSpellSlots == 1 ? "" : "s";
            Profile.Instance.MsgPrint($"You can learn {_player.SpareSpellSlots} new {spellType}{plural}.");
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
            Item item = itemIndex >= 0 ? _player.Inventory[itemIndex] : SaveGame.Instance.Level.Items[0 - itemIndex];
            int itemSubCategory = item.ItemSubCategory;
            bool useSetTwo = item.Category == _player.Realm2.ToSpellBookItemCategory();
            SaveGame.Instance.HandleStuff();
            int spellIndex;
            // Arcane casters can choose their spell
            if (_player.Spellcasting.Type != CastingType.Divine)
            {
                if (!CastCommand.GetSpell(out spellIndex, "study", itemSubCategory, false, useSetTwo, _player) && spellIndex == -1)
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
                        if (!_player.SpellOkay(spellIndex, false, useSetTwo))
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
            Spell spell = useSetTwo ? _player.Spellcasting.Spells[1][spellIndex] : _player.Spellcasting.Spells[0][spellIndex];
            spell.Learned = true;
            int i;
            // Mark the spell as the last spell learned, in case we need to start forgetting them
            for (i = 0; i < 64; i++)
            {
                if (_player.Spellcasting.SpellOrder[i] == 99)
                {
                    break;
                }
            }
            _player.Spellcasting.SpellOrder[i] = spellIndex;
            // Let the player know they've learned a spell
            Profile.Instance.MsgPrint($"You have learned the {spellType} of {spell.Name}.");
            Gui.PlaySound(SoundEffect.Study);
            _player.SpareSpellSlots--;
            if (_player.SpareSpellSlots != 0)
            {
                plural = _player.SpareSpellSlots != 1 ? "s" : "";
                Profile.Instance.MsgPrint($"You can learn {_player.SpareSpellSlots} more {spellType}{plural}.");
            }
            _player.OldSpareSpellSlots = _player.SpareSpellSlots;
            _player.RedrawNeeded.Set(RedrawFlag.PrStudy);
        }

        private void StoreProcessCommand()
        {
            char c = Gui.CurrentCommand;

            // Process commands
            foreach (IStoreCommand command in CommandManager.StoreCommands)
            {
                // TODO: The IF statement below can be converted into a dictionary with the applicable object 
                // attached for improved performance.
                if (command.IsEnabled && command.Key == c)
                {
                    command.Execute(_player);

                    if (command.RequiresRerendering)
                        DisplayStore();

                    // The command was processed.  Skip the SWITCH statement.
                    return;
                }
            }

            switch (c)
            {
                case '\x1b':
                    _leaveStore = true;
                    break;

                case 'g':
                    StorePurchase();
                    break;

                case 'd':
                    StoreSell();
                    break;

                case 'x':
                    StoreExamine();
                    break;

                case 'c':
                    if (StoreType == StoreType.StoreHall)
                    {
                        Gui.Save();
                        Program.HiScores.ClassFilter = _player.ProfessionIndex;
                        Program.HiScores.DisplayScores(new HighScore(_player));
                        Program.HiScores.ClassFilter = -1;
                        Gui.Load();
                    }
                    else
                    {
                        Profile.Instance.MsgPrint("That command does not work in this Stores.");
                    }
                    break;

                case 'v':
                    if (StoreType == StoreType.StoreHall)
                    {
                        Gui.Save();
                        Program.HiScores.RaceFilter = _player.RaceIndex;
                        Program.HiScores.DisplayScores(new HighScore(_player));
                        Program.HiScores.RaceFilter = -1;
                        Gui.Load();
                    }
                    else if (StoreType == StoreType.StoreTemple)
                    {
                        SacrificeItem();
                    }
                    else
                    {
                        Profile.Instance.MsgPrint("That command does not work in this Stores.");
                    }
                    break;

                case 'r':
                    int price;
                    switch (StoreType)
                    {
                        case StoreType.StoreGeneral:
                            var escortable = new Dictionary<char, Town>();
                            foreach (var town in SaveGame.Instance.Towns)
                            {
                                if (town.Visited && town.Name != SaveGame.Instance.CurTown.Name && town.Char != 'K')
                                {
                                    escortable.Add(town.Char, town);
                                }
                            }
                            if (escortable.Count == 0)
                            {
                                Profile.Instance.MsgPrint("There are no valid destinations to be escorted to.");
                                Profile.Instance.MsgPrint("You must have visited a town before you can be escorted there.");
                            }
                            else
                            {
                                var destination = GetEscortDestination(escortable);
                                if (destination != null)
                                {
                                    if (!ServiceHaggle(200, out price))
                                    {
                                        if (price > _player.Gold)
                                        {
                                            Profile.Instance.MsgPrint("You do not have the gold!");
                                        }
                                        else
                                        {
                                            _player.Gold -= price;
                                            SayComment_1();
                                            Gui.PlaySound(SoundEffect.StoreTransaction);
                                            StorePrtGold();
                                            _player.WildernessX = destination.X;
                                            _player.WildernessY = destination.Y;
                                            SaveGame.Instance.CurTown = destination;
                                            SaveGame.Instance.NewLevelFlag = true;
                                            SaveGame.Instance.CameFrom = LevelStart.StartRandom;
                                            Profile.Instance.MsgPrint("The journey takes all day.");
                                            _player.GameTime.ToNextDusk();
                                            _leaveStore = true;
                                        }
                                    }
                                }
                            }
                            SaveGame.Instance.HandleStuff();
                            break;

                        case StoreType.StoreArmoury:
                            if (!ServiceHaggle(400, out price))
                            {
                                if (price > _player.Gold)
                                {
                                    Profile.Instance.MsgPrint("You do not have the gold!");
                                }
                                else
                                {
                                    _player.Gold -= price;
                                    SayComment_1();
                                    Gui.PlaySound(SoundEffect.StoreTransaction);
                                    StorePrtGold();
                                    SaveGame.Instance.SpellEffects.EnchantSpell(0, 0, 4);
                                }
                                SaveGame.Instance.HandleStuff();
                            }
                            break;

                        case StoreType.StoreWeapon:
                            if (!ServiceHaggle(800, out price))
                            {
                                if (price > _player.Gold)
                                {
                                    Profile.Instance.MsgPrint("You do not have the gold!");
                                }
                                else
                                {
                                    _player.Gold -= price;
                                    SayComment_1();
                                    Gui.PlaySound(SoundEffect.StoreTransaction);
                                    StorePrtGold();
                                    SaveGame.Instance.SpellEffects.EnchantSpell(4, 4, 0);
                                }
                                SaveGame.Instance.HandleStuff();
                            }
                            break;

                        case StoreType.StoreTemple:
                            if (!ServiceHaggle(500, out price))
                            {
                                if (price > _player.Gold)
                                {
                                    Profile.Instance.MsgPrint("You do not have the gold!");
                                }
                                else
                                {
                                    _player.Gold -= price;
                                    SayComment_1();
                                    Gui.PlaySound(SoundEffect.StoreTransaction);
                                    StorePrtGold();
                                    SaveGame.Instance.SpellEffects.RemoveCurse();
                                }
                                SaveGame.Instance.HandleStuff();
                            }
                            break;

                        case StoreType.StoreAlchemist:
                            if (!ServiceHaggle(750, out price))
                            {
                                if (price > _player.Gold)
                                {
                                    Profile.Instance.MsgPrint("You do not have the gold!");
                                }
                                else
                                {
                                    _player.Gold -= price;
                                    SayComment_1();
                                    Gui.PlaySound(SoundEffect.StoreTransaction);
                                    StorePrtGold();
                                    _player.TryRestoringAbilityScore(Ability.Strength);
                                    _player.TryRestoringAbilityScore(Ability.Intelligence);
                                    _player.TryRestoringAbilityScore(Ability.Wisdom);
                                    _player.TryRestoringAbilityScore(Ability.Dexterity);
                                    _player.TryRestoringAbilityScore(Ability.Constitution);
                                    _player.TryRestoringAbilityScore(Ability.Charisma);
                                    _player.RestoreLevel();
                                }
                                SaveGame.Instance.HandleStuff();
                            }
                            break;

                        case StoreType.StoreMagic:
                            if (!ServiceHaggle(2000, out price))
                            {
                                if (price > _player.Gold)
                                {
                                    Profile.Instance.MsgPrint("You do not have the gold!");
                                }
                                else
                                {
                                    _player.Gold -= price;
                                    SayComment_1();
                                    Gui.PlaySound(SoundEffect.StoreTransaction);
                                    StorePrtGold();
                                    SaveGame.Instance.SpellEffects.IdentifyFully();
                                }
                                SaveGame.Instance.HandleStuff();
                            }
                            break;

                        case StoreType.StoreBlack:
                            Profile.Instance.MsgPrint("That command does not work in this Stores.");
                            break;

                        case StoreType.StoreHome:
                            if (_player.TimedPoison > 0 || _player.TimedBleeding > 0)
                            {
                                Profile.Instance.MsgPrint("Your wounds prevent you from sleeping.");
                            }
                            else
                            {
                                if (_player.RaceIndex == RaceId.Spectre || _player.RaceIndex == RaceId.Zombie ||
                                    _player.RaceIndex == RaceId.Skeleton || _player.RaceIndex == RaceId.Vampire)
                                {
                                    RoomRest(true);
                                }
                                else
                                {
                                    RoomRest(false);
                                }
                            }
                            break;

                        case StoreType.StoreLibrary:
                            DoCmdStudy();
                            break;

                        case StoreType.StoreInn:
                            if (_player.TimedPoison > 0 || _player.TimedBleeding > 0)
                            {
                                Profile.Instance.MsgPrint("You need a healer, not a room!");
                                Profile.Instance.MsgPrint("I'm sorry, but  I don't want anyone dying in here.");
                            }
                            else
                            {
                                if (!ServiceHaggle(10, out price))
                                {
                                    if (price >= _player.Gold)
                                    {
                                        Profile.Instance.MsgPrint("You do not have the gold!");
                                    }
                                    else
                                    {
                                        _player.Gold -= price;
                                        SayComment_1();
                                        Gui.PlaySound(SoundEffect.StoreTransaction);
                                        StorePrtGold();
                                        if (_player.RaceIndex == RaceId.Spectre || _player.RaceIndex == RaceId.Zombie ||
                                            _player.RaceIndex == RaceId.Skeleton || _player.RaceIndex == RaceId.Vampire)
                                        {
                                            RoomRest(true);
                                        }
                                        else
                                        {
                                            RoomRest(false);
                                        }
                                    }
                                }
                            }
                            break;

                        case StoreType.StoreHall:
                            if (_player.TownWithHouse == SaveGame.Instance.CurTown.Index)
                            {
                                Profile.Instance.MsgPrint("You already have the deeds!");
                            }
                            else
                            {
                                if (!ServiceHaggle(SaveGame.Instance.CurTown.HousePrice, out price))
                                {
                                    if (price >= _player.Gold)
                                    {
                                        Profile.Instance.MsgPrint("You do not have the gold!");
                                    }
                                    else
                                    {
                                        _player.Gold -= price;
                                        SayComment_1();
                                        Gui.PlaySound(SoundEffect.StoreTransaction);
                                        StorePrtGold();
                                        int oldHouse = _player.TownWithHouse;
                                        _player.TownWithHouse = SaveGame.Instance.CurTown.Index;
                                        if (oldHouse == -1)
                                        {
                                            Profile.Instance.MsgPrint("You may move in at once.");
                                        }
                                        else
                                        {
                                            Profile.Instance.MsgPrint(
                                                "I've sold your old house to pay for the removal service.");
                                            MoveHouse(oldHouse, _player.TownWithHouse);
                                        }
                                    }
                                    SaveGame.Instance.HandleStuff();
                                }
                            }
                            break;

                        case StoreType.StorePawn:
                            if (!ServiceHaggle(500, out price))
                            {
                                if (price >= _player.Gold)
                                {
                                    Profile.Instance.MsgPrint("You do not have the gold!");
                                }
                                else
                                {
                                    _player.Gold -= price;
                                    SayComment_1();
                                    Gui.PlaySound(SoundEffect.StoreTransaction);
                                    StorePrtGold();
                                    SaveGame.Instance.SpellEffects.IdentifyPack();
                                    Profile.Instance.MsgPrint("All your goods have been identified.");
                                }
                                SaveGame.Instance.HandleStuff();
                            }
                            break;
                    }
                    break;

                case '\r':
                    break;

                default:
                    Profile.Instance.MsgPrint("That command does not work in stores.");
                    break;
            }
        }

        private void StorePrtGold()
        {
            Gui.PrintLine("Gold Remaining: ", 39, 53);
            string outVal = $"{_player.Gold,9}";
            Gui.PrintLine(outVal, 39, 68);
        }

        private void StorePurchase()
        {
            int itemNew;
            string oName;
            if (_stockNum <= 0)
            {
                Profile.Instance.MsgPrint(StoreType == StoreType.StoreHome
                    ? "Your home is empty."
                    : "I am currently out of stock.");
                return;
            }
            int i = _stockNum - _storeTop;
            if (i > 26)
            {
                i = 26;
            }
            string outVal = StoreType == StoreType.StoreHome
                ? "Which item do you want to take? "
                : "Which item are you interested in? ";
            if (!GetStock(out int item, outVal, 0, i - 1))
            {
                return;
            }
            item += _storeTop;
            Item oPtr = _stock[item];
            int amt = 1;
            Item jPtr = new Item(oPtr) { Count = amt };
            if (!_player.Inventory.InvenCarryOkay(jPtr))
            {
                Profile.Instance.MsgPrint("You cannot carry that many different items.");
                return;
            }
            int best = PriceItem(jPtr, _owner.MinInflate, false);
            if (oPtr.Count > 1)
            {
                if (StoreType != StoreType.StoreHome && oPtr.IdentifyFlags.IsSet(Constants.IdentFixed))
                {
                    Profile.Instance.MsgPrint($"That costs {best} gold per item.");
                }
                int maxBuy = Math.Min(_player.Gold / best, oPtr.Count);
                if (maxBuy < 2)
                {
                    amt = 1;
                }
                else
                {
                    amt = Gui.GetQuantity(null, maxBuy, false);
                    if (amt <= 0)
                    {
                        return;
                    }
                }
            }
            jPtr = new Item(oPtr) { Count = amt };
            if (!_player.Inventory.InvenCarryOkay(jPtr))
            {
                Profile.Instance.MsgPrint("You cannot carry that many items.");
                return;
            }
            if (StoreType != StoreType.StoreHome)
            {
                bool choice;
                int price;
                if (oPtr.IdentifyFlags.IsSet(Constants.IdentFixed))
                {
                    choice = false;
                    price = best * jPtr.Count;
                }
                else
                {
                    oName = StoreType == StoreType.StorePawn
                        ? jPtr.Description(true, 3)
                        : jPtr.StoreDescription(true, 3);
                    Profile.Instance.MsgPrint($"Buying {oName} ({item.IndexToLetter()}).");
                    Profile.Instance.MsgPrint(null);
                    choice = PurchaseHaggle(jPtr, out price);
                }
                if (!choice)
                {
                    if (_player.Gold >= price)
                    {
                        SayComment_1();
                        Gui.PlaySound(SoundEffect.StoreTransaction);
                        _player.Gold -= price;
                        StorePrtGold();
                        if (StoreType != StoreType.StorePawn)
                        {
                            jPtr.BecomeFlavourAware();
                        }
                        jPtr.IdentifyFlags.Clear(Constants.IdentFixed);
                        oName = jPtr.Description(true, 3);
                        Profile.Instance.MsgPrint(StoreType == StoreType.StorePawn
                            ? $"You bought back {oName} for {price} gold."
                            : $"You bought {oName} for {price} gold.");
                        jPtr.Inscription = "";
                        itemNew = _player.Inventory.InvenCarry(jPtr, false);
                        oName = _player.Inventory[itemNew].Description(true, 3);
                        Profile.Instance.MsgPrint($"You have {oName} ({itemNew.IndexToLabel()}).");
                        SaveGame.Instance.HandleStuff();
                        i = _stockNum;
                        StoreItemIncrease(item, -amt);
                        StoreItemOptimize(item);
                        if (_stockNum == 0)
                        {
                            if (StoreType == StoreType.StorePawn)
                            {
                                _storeTop = 0;
                                DisplayInventory();
                            }
                            else
                            {
                                if (Program.Rng.RandomLessThan(Constants.StoreShuffle) == 0)
                                {
                                    Profile.Instance.MsgPrint("The shopkeeper retires.");
                                    StoreShuffle();
                                }
                                else
                                {
                                    Profile.Instance.MsgPrint("The shopkeeper brings out some new stock.");
                                }
                                for (i = 0; i < 10; i++)
                                {
                                    StoreMaint();
                                }
                                _storeTop = 0;
                                DisplayInventory();
                            }
                        }
                        else if (_stockNum != i)
                        {
                            if (_storeTop >= _stockNum)
                            {
                                _storeTop -= 26;
                            }
                            DisplayInventory();
                        }
                        else
                        {
                            DisplayEntry(item);
                        }
                    }
                    else
                    {
                        Profile.Instance.MsgPrint("You do not have enough gold.");
                    }
                }
            }
            else
            {
                itemNew = _player.Inventory.InvenCarry(jPtr, false);
                oName = _player.Inventory[itemNew].Description(true, 3);
                Profile.Instance.MsgPrint($"You have {oName} ({itemNew.IndexToLabel()}).");
                SaveGame.Instance.HandleStuff();
                i = _stockNum;
                StoreItemIncrease(item, -amt);
                StoreItemOptimize(item);
                if (i == _stockNum)
                {
                    DisplayEntry(item);
                }
                else
                {
                    if (_stockNum == 0)
                    {
                        _storeTop = 0;
                    }
                    else if (_storeTop >= _stockNum)
                    {
                        _storeTop -= 26;
                    }
                    DisplayInventory();
                }
            }
        }

        private void StoreSell()
        {
            int itemPos;
            string pmt = "Sell which item? ";
            if (StoreType == StoreType.StoreHome)
            {
                pmt = "Drop which item? ";
            }
            SaveGame.Instance.ItemFilter = StoreWillBuy;
            if (!SaveGame.Instance.GetItem(out int item, pmt, true, true, false))
            {
                if (item == -2)
                {
                    Profile.Instance.MsgPrint("You have nothing that I want.");
                }
                return;
            }
            Item oPtr = item >= 0 ? _player.Inventory[item] : SaveGame.Instance.Level.Items[0 - item];
            if (item >= InventorySlot.MeleeWeapon && oPtr.IsCursed())
            {
                Profile.Instance.MsgPrint("Hmmm, it seems to be cursed.");
                return;
            }
            int amt = 1;
            if (oPtr.Count > 1)
            {
                amt = Gui.GetQuantity(null, oPtr.Count, true);
                if (amt <= 0)
                {
                    return;
                }
            }
            Item qPtr = new Item(oPtr) { Count = amt };
            string oName = qPtr.Description(true, 3);
            if (StoreType != StoreType.StoreHome)
            {
                qPtr.Inscription = "";
            }
            if (!StoreCheckNum(qPtr))
            {
                Profile.Instance.MsgPrint(StoreType == StoreType.StoreHome
                    ? "Your home is full."
                    : "I have not the room in my Stores to keep it.");
                return;
            }
            if (StoreType != StoreType.StoreHome)
            {
                Profile.Instance.MsgPrint($"Selling {oName} ({item.IndexToLabel()}).");
                Profile.Instance.MsgPrint(null);
                bool choice = SellHaggle(qPtr, out int price);
                if (!choice)
                {
                    SayComment_1();
                    Gui.PlaySound(SoundEffect.StoreTransaction);
                    _player.Gold += price;
                    StorePrtGold();
                    int dummy = qPtr.Value() * qPtr.Count;
                    if (StoreType != StoreType.StorePawn)
                    {
                        oPtr.BecomeFlavourAware();
                        oPtr.BecomeKnown();
                    }
                    _player.NoticeFlags |= Constants.PnCombine | Constants.PnReorder;
                    qPtr = new Item(oPtr) { Count = amt };
                    int value;
                    if (StoreType == StoreType.StorePawn)
                    {
                        value = dummy;
                    }
                    else
                    {
                        value = qPtr.Value() * qPtr.Count;
                        oName = qPtr.Description(true, 3);
                    }
                    Profile.Instance.MsgPrint(StoreType != StoreType.StorePawn
                        ? $"You sold {oName} for {price} gold."
                        : $"You pawn {oName} for {price} gold.");
                    PurchaseAnalyze(price, value, dummy);
                    _player.Inventory.InvenItemIncrease(item, -amt);
                    _player.Inventory.InvenItemDescribe(item);
                    _player.Inventory.InvenItemOptimize(item);
                    SaveGame.Instance.HandleStuff();
                    itemPos = StoreType != StoreType.StorePawn ? StoreCarry(qPtr) : HomeCarry(qPtr);
                    if (itemPos >= 0)
                    {
                        _storeTop = itemPos / 26 * 26;
                        DisplayInventory();
                    }
                }
            }
            else
            {
                Profile.Instance.MsgPrint($"You drop {oName} ({item.IndexToLabel()}).");
                _player.Inventory.InvenItemIncrease(item, -amt);
                _player.Inventory.InvenItemDescribe(item);
                _player.Inventory.InvenItemOptimize(item);
                SaveGame.Instance.HandleStuff();
                itemPos = HomeCarry(qPtr);
                if (itemPos >= 0)
                {
                    _storeTop = itemPos / 26 * 26;
                    DisplayInventory();
                }
            }
        }

        private bool StoreWillBuy(Item oPtr)
        {
            switch (StoreType)
            {
                case StoreType.StoreGeneral:
                    {
                        switch (oPtr.Category)
                        {
                            case ItemCategory.Food:
                            case ItemCategory.Light:
                            case ItemCategory.Flask:
                            case ItemCategory.Spike:
                            case ItemCategory.Shot:
                            case ItemCategory.Arrow:
                            case ItemCategory.Bolt:
                            case ItemCategory.Digging:
                            case ItemCategory.Cloak:
                            case ItemCategory.Bottle:
                                break;

                            default:
                                return false;
                        }
                        break;
                    }
                case StoreType.StoreArmoury:
                    {
                        switch (oPtr.Category)
                        {
                            case ItemCategory.Boots:
                            case ItemCategory.Gloves:
                            case ItemCategory.Crown:
                            case ItemCategory.Helm:
                            case ItemCategory.Shield:
                            case ItemCategory.Cloak:
                            case ItemCategory.SoftArmor:
                            case ItemCategory.HardArmor:
                            case ItemCategory.DragArmor:
                                break;

                            default:
                                return false;
                        }
                        break;
                    }
                case StoreType.StoreWeapon:
                    {
                        switch (oPtr.Category)
                        {
                            case ItemCategory.Shot:
                            case ItemCategory.Bolt:
                            case ItemCategory.Arrow:
                            case ItemCategory.Bow:
                            case ItemCategory.Digging:
                            case ItemCategory.Hafted:
                            case ItemCategory.Polearm:
                            case ItemCategory.Sword:
                                break;

                            default:
                                return false;
                        }
                        break;
                    }
                case StoreType.StoreTemple:
                    {
                        switch (oPtr.Category)
                        {
                            case ItemCategory.LifeBook:
                            case ItemCategory.Scroll:
                            case ItemCategory.Potion:
                            case ItemCategory.Hafted:
                                break;

                            case ItemCategory.Polearm:
                            case ItemCategory.Sword:
                                if (IsBlessed(oPtr))
                                {
                                    break;
                                }
                                return false;

                            default:
                                return false;
                        }
                        break;
                    }
                case StoreType.StoreAlchemist:
                    {
                        switch (oPtr.Category)
                        {
                            case ItemCategory.Scroll:
                            case ItemCategory.Potion:
                                break;

                            default:
                                return false;
                        }
                        break;
                    }
                case StoreType.StoreMagic:
                    {
                        switch (oPtr.Category)
                        {
                            case ItemCategory.SorceryBook:
                            case ItemCategory.NatureBook:
                            case ItemCategory.ChaosBook:
                            case ItemCategory.DeathBook:
                            case ItemCategory.TarotBook:
                            case ItemCategory.FolkBook:
                            case ItemCategory.CorporealBook:
                            case ItemCategory.Amulet:
                            case ItemCategory.Ring:
                            case ItemCategory.Staff:
                            case ItemCategory.Wand:
                            case ItemCategory.Rod:
                            case ItemCategory.Scroll:
                            case ItemCategory.Potion:
                                break;

                            default:
                                return false;
                        }
                        break;
                    }
                case StoreType.StoreHome:
                    {
                        return true;
                    }
                case StoreType.StoreLibrary:
                    {
                        switch (oPtr.Category)
                        {
                            case ItemCategory.SorceryBook:
                            case ItemCategory.NatureBook:
                            case ItemCategory.ChaosBook:
                            case ItemCategory.DeathBook:
                            case ItemCategory.LifeBook:
                            case ItemCategory.TarotBook:
                            case ItemCategory.FolkBook:
                            case ItemCategory.CorporealBook:
                                break;

                            default:
                                return false;
                        }
                        break;
                    }
                case StoreType.StoreHall:
                    {
                        return false;
                    }
                case StoreType.StoreInn:
                    {
                        return false;
                    }
                case StoreType.StorePawn:
                    {
                        break;
                    }
                case StoreType.StoreEmptyLot:
                    {
                        return false;
                    }
            }
            return oPtr.Value() > 0;
        }
    }
}