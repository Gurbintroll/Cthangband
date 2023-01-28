// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
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
            foreach (var baseType in StaticResources.Instance.BaseItemTypes)
            {
                Add(new ItemType(baseType.Value));
            }
        }

        public ItemType LookupKind(ItemCategory tval, int sval)
        {
            for (var k = 1; k < Count; k++)
            {
                var kPtr = this[k];
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
            foreach (var itemType in this)
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