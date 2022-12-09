// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.StaticData;
using Cthangband.UI;
using System;

namespace Cthangband
{
    [Serializable]
    internal class Inventory
    {
        public static ItemCategory ItemFilterCategory;
        public static bool ItemFilterUseableSpellBook;

        private readonly Item[] _items;
        private readonly Player _player;
        private int _invenCnt;

        public Inventory(Player player)
        {
            _items = new Item[InventorySlot.Total];
            _player = player;
            for (int i = 0; i < InventorySlot.Total; i++)
            {
                _items[i] = new Item();
            }
            _invenCnt = 0;
        }

        public Item this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }

        public static bool ObjectEasyKnow(int i)
        {
            ItemType kPtr = Profile.Instance.ItemTypes[i];
            switch (kPtr.Category)
            {
                case ItemCategory.LifeBook:
                case ItemCategory.SorceryBook:
                case ItemCategory.NatureBook:
                case ItemCategory.ChaosBook:
                case ItemCategory.DeathBook:
                case ItemCategory.TarotBook:
                case ItemCategory.FolkBook:
                case ItemCategory.CorporealBook:
                case ItemCategory.Flask:
                case ItemCategory.Junk:
                case ItemCategory.Bottle:
                case ItemCategory.Skeleton:
                case ItemCategory.Spike:
                case ItemCategory.Food:
                case ItemCategory.Potion:
                case ItemCategory.Scroll:
                case ItemCategory.Rod:
                    return true;

                case ItemCategory.Ring:
                case ItemCategory.Amulet:
                case ItemCategory.Light:
                    return kPtr.Flags3.IsSet(ItemFlag3.EasyKnow);
            }
            return false;
        }

        public static bool ObjectHasFlavor(ItemType i)
        {
            ItemType kPtr = i;
            switch (kPtr.Category)
            {
                case ItemCategory.Amulet:
                case ItemCategory.Ring:
                case ItemCategory.Staff:
                case ItemCategory.Wand:
                case ItemCategory.Scroll:
                case ItemCategory.Potion:
                case ItemCategory.Rod:
                    return true;

                case ItemCategory.Food:
                    return kPtr.SubCategory < FoodType.MinFood;
            }
            return false;
        }

        public void CombinePack()
        {
            bool flag = false;
            for (int i = InventorySlot.Pack; i > 0; i--)
            {
                Item oPtr = _items[i];
                if (oPtr.ItemType == null)
                {
                    continue;
                }
                for (int j = 0; j < i; j++)
                {
                    Item jPtr = _items[j];
                    if (jPtr.ItemType == null)
                    {
                        continue;
                    }
                    if (jPtr.CanAbsorb(oPtr))
                    {
                        flag = true;
                        jPtr.Absorb(oPtr);
                        _invenCnt--;
                        int k;
                        for (k = i; k < InventorySlot.Pack; k++)
                        {
                            _items[k] = new Item(_items[k + 1]);
                        }
                        _items[k] = new Item();
                        break;
                    }
                }
            }
            if (flag)
            {
                Profile.Instance.MsgPrint("You combine some items in your pack.");
            }
        }

        public bool GetItemOkay(int i)
        {
            if (i < 0 || i >= InventorySlot.Total)
            {
                return false;
            }
            return ItemMatchesFilter(_items[i]);
        }

        public int InvenCarry(Item oPtr, bool final)
        {
            int j;
            int n = -1;
            Item jPtr;
            if (!final)
            {
                for (j = 0; j < InventorySlot.Pack; j++)
                {
                    jPtr = _items[j];
                    if (jPtr.ItemType == null)
                    {
                        continue;
                    }
                    n = j;
                    if (oPtr.CanAbsorb(jPtr))
                    {
                        jPtr.Absorb(oPtr);
                        _player.WeightCarried += oPtr.Count * oPtr.Weight;
                        _player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
                        return j;
                    }
                }
            }
            if (_invenCnt > InventorySlot.Pack)
            {
                return -1;
            }
            for (j = 0; j <= InventorySlot.Pack; j++)
            {
                jPtr = _items[j];
                if (jPtr.ItemType == null)
                {
                    break;
                }
            }
            int i = j;
            if (!final && i < InventorySlot.Pack)
            {
                int oValue = oPtr.Value();
                for (j = 0; j < InventorySlot.Pack; j++)
                {
                    jPtr = _items[j];
                    if (jPtr.ItemType == null)
                    {
                        break;
                    }
                    if (oPtr.Category.SpellBookToToRealm() == _player.Realm1 && jPtr.Category.SpellBookToToRealm() != _player.Realm1)
                    {
                        break;
                    }
                    if (jPtr.Category.SpellBookToToRealm() == _player.Realm1 && oPtr.Category.SpellBookToToRealm() != _player.Realm1)
                    {
                        continue;
                    }
                    if (oPtr.Category.SpellBookToToRealm() == _player.Realm2 && jPtr.Category.SpellBookToToRealm() != _player.Realm2)
                    {
                        break;
                    }
                    if (jPtr.Category.SpellBookToToRealm() == _player.Realm2 && oPtr.Category.SpellBookToToRealm() != _player.Realm2)
                    {
                        continue;
                    }
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
                    if (oValue > jValue)
                    {
                        break;
                    }
                    if (oValue < jValue)
                    {
                    }
                }
                i = j;
                for (int k = n; k >= i; k--)
                {
                    _items[k + 1] = new Item(_items[k]);
                }
                _items[i] = new Item();
            }
            _items[i] = new Item(oPtr);
            oPtr = _items[i];
            oPtr.Y = 0;
            oPtr.X = 0;
            oPtr.NextInStack = 0;
            oPtr.HoldingMonsterIndex = 0;
            _player.WeightCarried += oPtr.Count * oPtr.Weight;
            _invenCnt++;
            _player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            _player.NoticeFlags |= Constants.PnCombine | Constants.PnReorder;
            return i;
        }

        public bool InvenCarryOkay(Item oPtr)
        {
            if (_invenCnt < InventorySlot.Pack)
            {
                return true;
            }
            for (int j = 0; j < InventorySlot.Pack; j++)
            {
                Item jPtr = _items[j];
                if (jPtr.ItemType == null)
                {
                    continue;
                }
                if (jPtr.CanAbsorb(oPtr))
                {
                    return true;
                }
            }
            return false;
        }

        public int InvenDamage(Func<Item, bool> testerFunc, int perc)
        {
            int k = 0;
            for (int i = 0; i < InventorySlot.Pack; i++)
            {
                Item oPtr = _items[i];
                if (oPtr.ItemType == null)
                {
                    continue;
                }
                if (oPtr.IsArtifact() || oPtr.IsLegendary())
                {
                    continue;
                }
                if (testerFunc(oPtr))
                {
                    int j;
                    int amt;
                    for (amt = j = 0; j < oPtr.Count; ++j)
                    {
                        if (Program.Rng.RandomLessThan(100) < perc)
                        {
                            amt++;
                        }
                    }
                    if (amt != 0)
                    {
                        string oName = oPtr.Description(false, 3);
                        string y = oPtr.Count > 1
                            ? (amt == oPtr.Count ? "All of y" : (amt > 1 ? "Some of y" : "One of y"))
                            : "Y";
                        string w = amt > 1 ? "were" : "was";
                        Profile.Instance.MsgPrint($"{y}our {oName} ({i.IndexToLabel()}) {w} destroyed!");
                        if (oPtr.ItemType.Category == ItemCategory.Potion)
                        {
                            SaveGame.Instance.SpellEffects.PotionSmashEffect(0, _player.MapY, _player.MapX,
                                oPtr.ItemSubCategory);
                        }
                        InvenItemIncrease(i, -amt);
                        InvenItemOptimize(i);
                        k += amt;
                    }
                }
            }
            return k;
        }

        public void InvenDrop(int item, int amt)
        {
            Item oPtr = _items[item];
            if (amt <= 0)
            {
                return;
            }
            if (amt > oPtr.Count)
            {
                amt = oPtr.Count;
            }
            if (item >= InventorySlot.MeleeWeapon)
            {
                item = InvenTakeoff(item, amt);
                oPtr = _items[item];
            }
            Item qPtr = new Item(oPtr) { Count = amt };
            string oName = qPtr.Description(true, 3);
            Profile.Instance.MsgPrint($"You drop {oName} ({item.IndexToLabel()}).");
            SaveGame.Instance.Level.DropNear(qPtr, 0, _player.MapY, _player.MapX);
            InvenItemIncrease(item, -amt);
            InvenItemDescribe(item);
            InvenItemOptimize(item);
        }

        public void InvenItemDescribe(int item)
        {
            Item oPtr = _items[item];
            string oName = oPtr.Description(true, 3);
            Profile.Instance.MsgPrint($"You have {oName}.");
        }

        public void InvenItemIncrease(int item, int num)
        {
            Item oPtr = _items[item];
            num += oPtr.Count;
            if (num > 255)
            {
                num = 255;
            }
            else if (num < 0)
            {
                num = 0;
            }
            num -= oPtr.Count;
            if (num != 0)
            {
                oPtr.Count += num;
                _player.WeightCarried += num * oPtr.Weight;
                _player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
                _player.UpdatesNeeded.Set(UpdateFlags.UpdateVril);
                _player.NoticeFlags |= Constants.PnCombine;
            }
        }

        public void InvenItemOptimize(int item)
        {
            Item oPtr = _items[item];
            if (oPtr.ItemType == null)
            {
                return;
            }
            if (oPtr.Count > 0)
            {
                return;
            }
            if (item < InventorySlot.MeleeWeapon)
            {
                int i;
                _invenCnt--;
                for (i = item; i < InventorySlot.Pack; i++)
                {
                    _items[i] = _items[i + 1];
                }
                _items[i] = new Item();
            }
            else
            {
                _items[item] = new Item();
                _player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
                _player.UpdatesNeeded.Set(UpdateFlags.UpdateTorchRadius);
                _player.UpdatesNeeded.Set(UpdateFlags.UpdateVril);
            }
        }

        public int InvenTakeoff(int item, int amt)
        {
            string act;
            Item oPtr = _items[item];
            if (amt <= 0)
            {
                return -1;
            }
            if (amt > oPtr.Count)
            {
                amt = oPtr.Count;
            }
            Item qPtr = new Item(oPtr) { Count = amt };
            string oName = qPtr.Description(true, 3);
            if (item == InventorySlot.MeleeWeapon)
            {
                act = "You were wielding";
            }
            else if (item == InventorySlot.RangedWeapon)
            {
                act = "You were holding";
            }
            else if (item == InventorySlot.Lightsource)
            {
                act = "You were holding";
            }
            else
            {
                act = "You were wearing";
            }
            InvenItemIncrease(item, -amt);
            InvenItemOptimize(item);
            int slot = InvenCarry(qPtr, false);
            Profile.Instance.MsgPrint($"{act} {oName} ({slot.IndexToLabel()}).");
            return slot;
        }

        public bool ItemMatchesFilter(Item item)
        {
            if (SaveGame.Instance.ItemFilterAll)
            {
                return true;
            }
            if (item.ItemType == null)
            {
                return false;
            }
            if (item.Category == ItemCategory.Gold)
            {
                return false;
            }
            if (ItemFilterUseableSpellBook)
            {
                return CheckBookRealm(item.Category);
            }
            if (ItemFilterCategory != 0)
            {
                if (ItemFilterCategory != item.Category)
                {
                    return false;
                }
            }
            if (SaveGame.Instance.ItemFilter != null)
            {
                if (!SaveGame.Instance.ItemFilter(item))
                {
                    return false;
                }
            }
            return true;
        }

        public int LabelToEquip(char c)
        {
            int i = (char.IsLower(c) ? c.LetterToNumber() : -1) + InventorySlot.MeleeWeapon;
            if (i < InventorySlot.MeleeWeapon || i >= InventorySlot.Total)
            {
                return -1;
            }
            if (_items[i].ItemType == null)
            {
                return -1;
            }
            return i;
        }

        public int LabelToInven(char c)
        {
            int i = char.IsLower(c) ? c.LetterToNumber() : -1;
            if (i < 0 || i > InventorySlot.Pack)
            {
                return -1;
            }
            if (_items[i].ItemType == null)
            {
                return -1;
            }
            return i;
        }

        public void ReorderPack()
        {
            bool flag = false;
            for (int i = 0; i < InventorySlot.Pack; i++)
            {
                if (i == InventorySlot.Pack && _invenCnt == InventorySlot.Pack)
                {
                    break;
                }
                Item oPtr = _items[i];
                if (oPtr.ItemType == null)
                {
                    continue;
                }
                int oValue = oPtr.Value();
                int j;
                for (j = 0; j < InventorySlot.Pack; j++)
                {
                    Item jPtr = _items[j];
                    if (jPtr.ItemType == null)
                    {
                        break;
                    }
                    if (oPtr.Category.SpellBookToToRealm() == _player.Realm1 && jPtr.Category.SpellBookToToRealm() != _player.Realm1)
                    {
                        break;
                    }
                    if (jPtr.Category.SpellBookToToRealm() == _player.Realm1 && oPtr.Category.SpellBookToToRealm() != _player.Realm1)
                    {
                        continue;
                    }
                    if (oPtr.Category.SpellBookToToRealm() == _player.Realm2 && jPtr.Category.SpellBookToToRealm() != _player.Realm2)
                    {
                        break;
                    }
                    if (jPtr.Category.SpellBookToToRealm() == _player.Realm2 && oPtr.Category.SpellBookToToRealm() != _player.Realm2)
                    {
                        continue;
                    }
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
                    if (oValue > jValue)
                    {
                        break;
                    }
                    if (oValue < jValue)
                    {
                    }
                }
                if (j >= i)
                {
                    continue;
                }
                flag = true;
                Item qPtr = new Item(_items[i]);
                for (int k = i; k > j; k--)
                {
                    _items[k] = new Item(_items[k - 1]);
                }
                _items[j] = new Item(qPtr);
            }
            if (flag)
            {
                Profile.Instance.MsgPrint("You reorder some items in your pack.");
            }
        }

        public void ReportChargeUsageFromInventory(int item)
        {
            Item oPtr = _items[item];
            if (oPtr.Category != ItemCategory.Staff && oPtr.Category != ItemCategory.Wand)
            {
                return;
            }
            if (!oPtr.IsKnown())
            {
                return;
            }
            Profile.Instance.MsgPrint(oPtr.TypeSpecificValue != 1
                ? $"You have {oPtr.TypeSpecificValue} charges remaining."
                : $"You have {oPtr.TypeSpecificValue} charge remaining.");
        }

        public void ShowEquip()
        {
            int i, j, k;
            Item oPtr;
            int[] outIndex = new int[23];
            Colour[] outColour = new Colour[23];
            string[] outDesc = new string[23];
            int col = SaveGame.Instance.ItemDisplayColumn;
            int len = 79 - col;
            int lim = 79 - 3;
            lim -= 14 + 2;
            lim -= 9;
            lim -= 2;
            for (k = 0, i = InventorySlot.MeleeWeapon; i < InventorySlot.Total; i++)
            {
                oPtr = _items[i];
                if (!ItemMatchesFilter(oPtr))
                {
                    continue;
                }
                string oName = oPtr.Description(true, 3);
                if (oName.Length > lim)
                {
                    oName = oName.Substring(0, lim);
                }
                outIndex[k] = i;
                outColour[k] = oPtr.Category.ToAttr();
                outDesc[k] = oName;
                int l = outDesc[k].Length + 5;
                l += 16;
                l += 9;
                l += 2;
                if (l > len)
                {
                    len = l;
                }
                k++;
            }
            col = len > 76 ? 0 : 79 - len;
            for (j = 0; j < k; j++)
            {
                i = outIndex[j];
                oPtr = _items[i];
                Gui.PrintLine("", j + 1, col != 0 ? col - 2 : col);
                string tmpVal = $"{i.IndexToLabel()})";
                Gui.Print(tmpVal, j + 1, col);
                if (oPtr.ItemType != null)
                {
                    Colour a = oPtr.ItemType.Colour;
                    char c = oPtr.ItemType.Character;
                    Gui.Place(a, c, j + 1, col + 3);
                }
                else
                {
                    Gui.Place(Colour.Background, ' ', j + 1, col + 3);
                }
                tmpVal = $"{MentionUse(i)}: ";
                Gui.Print(tmpVal, j + 1, col + 5);
                Gui.Print(outColour[j], outDesc[j], j + 1, col + 21);
                int wgt = oPtr.Weight * oPtr.Count;
                tmpVal = $"{wgt / 10}.{wgt % 10} lb";
                Gui.Print(tmpVal, j + 1, 71);
            }
            if (j != 0 && j < 23)
            {
                Gui.PrintLine("", j + 1, col != 0 ? col - 2 : col);
            }
            SaveGame.Instance.ItemDisplayColumn = col;
        }

        public void ShowInven()
        {
            int i, j, k, z = 0;
            Item oPtr;
            int[] outIndex = new int[26];
            Colour[] outColour = new Colour[26];
            string[] outDesc = new string[26];
            int col = SaveGame.Instance.ItemDisplayColumn;
            int len = 79 - col;
            int lim = 79 - 3;
            lim -= 9;
            lim -= 2;
            for (i = 0; i < InventorySlot.Pack; i++)
            {
                oPtr = _items[i];
                if (oPtr.ItemType == null)
                {
                    continue;
                }
                z = i + 1;
            }
            for (k = 0, i = 0; i < z; i++)
            {
                oPtr = _items[i];
                if (!ItemMatchesFilter(oPtr))
                {
                    continue;
                }
                string oName = oPtr.Description(true, 3);
                if (oName.Length > lim)
                {
                    oName = oName.Substring(0, lim);
                }
                outIndex[k] = i;
                outColour[k] = oPtr.Category.ToAttr();
                outDesc[k] = oName;
                int l = outDesc[k].Length + 5;
                l += 9;
                l += 2;
                if (l > len)
                {
                    len = l;
                }
                k++;
            }
            col = len > 76 ? 0 : 79 - len;
            for (j = 0; j < k; j++)
            {
                i = outIndex[j];
                oPtr = _items[i];
                Gui.PrintLine("", j + 1, col != 0 ? col - 2 : col);
                string tmpVal = $"{i.IndexToLabel()})";
                Gui.Print(tmpVal, j + 1, col);
                Colour a = oPtr.ItemType.Colour;
                char c = oPtr.ItemType.Character;
                Gui.Place(a, c, j + 1, col + 3);
                Gui.Print(outColour[j], outDesc[j], j + 1, col + 5);
                int wgt = oPtr.Weight * oPtr.Count;
                tmpVal = $"{wgt / 10,2}.{wgt % 10} lb";
                Gui.Print(tmpVal, j + 1, 71);
            }
            if (j != 0 && j < 26)
            {
                Gui.PrintLine("", j + 1, col != 0 ? col - 2 : col);
            }
            SaveGame.Instance.ItemDisplayColumn = col;
        }

        public int WieldSlot(Item oPtr)
        {
            switch (oPtr.Category)
            {
                case ItemCategory.Digging:
                    return InventorySlot.Digger;

                case ItemCategory.Hafted:
                case ItemCategory.Polearm:
                case ItemCategory.Sword:
                    return InventorySlot.MeleeWeapon;

                case ItemCategory.Bow:
                    return InventorySlot.RangedWeapon;

                case ItemCategory.Ring:
                    if (_player.Inventory[InventorySlot.RightHand].ItemType == null)
                    {
                        return InventorySlot.RightHand;
                    }
                    return InventorySlot.LeftHand;

                case ItemCategory.Amulet:
                    return InventorySlot.Neck;

                case ItemCategory.Light:
                    return InventorySlot.Lightsource;

                case ItemCategory.DragArmor:
                case ItemCategory.HardArmor:
                case ItemCategory.SoftArmor:
                    return InventorySlot.Body;

                case ItemCategory.Cloak:
                    return InventorySlot.Cloak;

                case ItemCategory.Shield:
                    return InventorySlot.Arm;

                case ItemCategory.Crown:
                case ItemCategory.Helm:
                    return InventorySlot.Head;

                case ItemCategory.Gloves:
                    return InventorySlot.Hands;

                case ItemCategory.Boots:
                    return InventorySlot.Feet;
            }
            return -1;
        }

        private bool CheckBookRealm(ItemCategory bookTval)
        {
            return _player.Realm1.ToSpellBookItemCategory() == bookTval || _player.Realm2.ToSpellBookItemCategory() == bookTval;
        }

        private string MentionUse(int i)
        {
            string p;
            switch (i)
            {
                case InventorySlot.MeleeWeapon:
                    p = "Wielding";
                    break;

                case InventorySlot.RangedWeapon:
                    p = "Shooting";
                    break;

                case InventorySlot.Digger:
                    p = "Digging with";
                    break;

                case InventorySlot.LeftHand:
                    p = "On left hand";
                    break;

                case InventorySlot.RightHand:
                    p = "On right hand";
                    break;

                case InventorySlot.Neck:
                    p = "Around neck";
                    break;

                case InventorySlot.Lightsource:
                    p = "Light source";
                    break;

                case InventorySlot.Body:
                    p = "On body";
                    break;

                case InventorySlot.Cloak:
                    p = "About body";
                    break;

                case InventorySlot.Arm:
                    p = "On arm";
                    break;

                case InventorySlot.Head:
                    p = "On head";
                    break;

                case InventorySlot.Hands:
                    p = "On hands";
                    break;

                case InventorySlot.Feet:
                    p = "On feet";
                    break;

                default:
                    p = "In pack";
                    break;
            }
            if (i == InventorySlot.MeleeWeapon)
            {
                Item oPtr = _items[i];
                if (_player.AbilityScores[Ability.Strength].StrMaxWeaponWeight < oPtr.Weight / 10)
                {
                    p = "Just lifting";
                }
            }
            if (i == InventorySlot.RangedWeapon)
            {
                Item oPtr = _items[i];
                if (_player.AbilityScores[Ability.Strength].StrMaxWeaponWeight < oPtr.Weight / 10)
                {
                    p = "Just holding";
                }
            }
            return p;
        }
    }
}