using Cthangband.Enumerations;
using Cthangband.StaticData;
using System;
using System.Collections.Generic;

namespace Cthangband
{
    [Serializable]
    internal class ItemTypeArray : List<ItemType>
    {
        public ItemTypeArray()
        {
            foreach (KeyValuePair<string, BaseItemType> baseType in StaticResources.Instance.BaseItemTypes)
            {
                Add(new ItemType(baseType.Value));
            }
        }

        public ItemType LookupKind(ItemCategory tval, int sval)
        {
            for (int k = 1; k < Count; k++)
            {
                ItemType kPtr = this[k];
                if (kPtr.Category == tval && kPtr.SubCategory == sval)
                {
                    return kPtr;
                }
            }
            Profile.Instance.MsgPrint($"No object ({tval},{sval})");
            return null;
        }

        public void ResetStompability()
        {
            foreach (ItemType itemType in this)
            {
                if (itemType.HasQuality())
                {
                    itemType.Stompable[0] = true;
                    itemType.Stompable[1] = false;
                    itemType.Stompable[2] = false;
                    itemType.Stompable[3] = false;
                }
                else
                {
                    itemType.Stompable[0] = itemType.Cost <= 0;
                    itemType.Stompable[1] = false;
                    itemType.Stompable[2] = false;
                    itemType.Stompable[3] = false;
                }
            }
        }
    }
}