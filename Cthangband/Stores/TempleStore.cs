using Cthangband.Enumerations;
using System;

namespace Cthangband.Stores
{
    [Serializable]
    internal class TempleStore : Store
    {
        public TempleStore() : base(StoreType.StoreTemple)
        {
        }

        public override string FeatureType => "Temple";

        protected override ItemIdentifier[] GetStoreTable()
        {
            return new[]
            {
                new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvWhip),
                new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvQuarterstaff),
                new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvMace),
                new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvBallAndChain),
                new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvWarHammer),
                new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvLucernHammer),
                new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvMorningStar),
                new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvFlail),
                new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvLeadFilledMace),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.RemoveCurse),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.Blessing),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.HolyChant),
                new ItemIdentifier(ItemCategory.Potion, PotionType.Heroism),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.WordOfRecall),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.WordOfRecall),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.WordOfRecall),
                new ItemIdentifier(ItemCategory.Potion, PotionType.CureLight),
                new ItemIdentifier(ItemCategory.Potion, PotionType.CureSerious),
                new ItemIdentifier(ItemCategory.Potion, PotionType.CureSerious),
                new ItemIdentifier(ItemCategory.Potion, PotionType.CureCritical),
                new ItemIdentifier(ItemCategory.Potion, PotionType.CureCritical),
                new ItemIdentifier(ItemCategory.Potion, PotionType.RestoreExp),
                new ItemIdentifier(ItemCategory.Potion, PotionType.RestoreExp),
                new ItemIdentifier(ItemCategory.Potion, PotionType.RestoreExp),
                new ItemIdentifier(ItemCategory.LifeBook, 0), new ItemIdentifier(ItemCategory.LifeBook, 0),
                new ItemIdentifier(ItemCategory.LifeBook, 0), new ItemIdentifier(ItemCategory.LifeBook, 0),
                new ItemIdentifier(ItemCategory.LifeBook, 1), new ItemIdentifier(ItemCategory.LifeBook, 1),
                new ItemIdentifier(ItemCategory.LifeBook, 1), new ItemIdentifier(ItemCategory.LifeBook, 1),
                new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvWhip),
                new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvMace),
                new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvBallAndChain),
                new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvWarHammer),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.WordOfRecall),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.WordOfRecall),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.WordOfRecall),
                new ItemIdentifier(ItemCategory.Potion, PotionType.CureCritical),
                new ItemIdentifier(ItemCategory.Potion, PotionType.CureCritical),
                new ItemIdentifier(ItemCategory.Potion, PotionType.RestoreExp),
                new ItemIdentifier(ItemCategory.Potion, PotionType.RestoreExp),
                new ItemIdentifier(ItemCategory.Potion, PotionType.RestoreExp),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.RemoveCurse),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.RemoveCurse),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.StarRemoveCurse),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.StarRemoveCurse)
            };
        }

        protected override bool StoreWillBuy(Item item)
        {
            switch (item.Category)
            {
                case ItemCategory.LifeBook:
                case ItemCategory.Scroll:
                case ItemCategory.Potion:
                case ItemCategory.Hafted:
                    return item.Value() > 0;
                case ItemCategory.Polearm:
                case ItemCategory.Sword:
                    if (item.IsBlessed())
                        return item.Value() > 0;
                    else
                        return false;
                default:
                    return false;
            }
        }
    }
}
