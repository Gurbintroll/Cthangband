using Cthangband.Enumerations;
using System;

namespace Cthangband.Stores
{
    [Serializable]
    internal class AlchemistStore : Store
    {
        public AlchemistStore() : base(StoreType.StoreAlchemist)
        {
        }

        public override string FeatureType => "Alchemist";

        protected override ItemIdentifier[] GetStoreTable()
        {
            return new[]
            {
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.EnchantWeaponToHit),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.EnchantWeaponToDam),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.EnchantArmor),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.Identify),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.Identify),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.Identify),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.Identify),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.Light),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.PhaseDoor),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.PhaseDoor),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.Teleport),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.MonsterConfusion),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.Mapping),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.DetectGold),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.DetectItem),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.DetectTrap),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.DetectInvis),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.Recharging),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.SatisfyHunger),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.WordOfRecall),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.WordOfRecall),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.WordOfRecall),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.WordOfRecall),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.Teleport),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.Teleport),
                new ItemIdentifier(ItemCategory.Potion, PotionType.ResStr),
                new ItemIdentifier(ItemCategory.Potion, PotionType.ResInt),
                new ItemIdentifier(ItemCategory.Potion, PotionType.ResWis),
                new ItemIdentifier(ItemCategory.Potion, PotionType.ResDex),
                new ItemIdentifier(ItemCategory.Potion, PotionType.ResCon),
                new ItemIdentifier(ItemCategory.Potion, PotionType.ResCha),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.Identify),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.Identify),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.StarIdentify),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.StarIdentify),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.Light),
                new ItemIdentifier(ItemCategory.Potion, PotionType.ResStr),
                new ItemIdentifier(ItemCategory.Potion, PotionType.ResInt),
                new ItemIdentifier(ItemCategory.Potion, PotionType.ResWis),
                new ItemIdentifier(ItemCategory.Potion, PotionType.ResDex),
                new ItemIdentifier(ItemCategory.Potion, PotionType.ResCon),
                new ItemIdentifier(ItemCategory.Potion, PotionType.ResCha),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.EnchantArmor),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.EnchantArmor),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.Recharging),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.SatisfyHunger),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.SatisfyHunger),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.SatisfyHunger)
            };
        }

        protected override bool StoreWillBuy(Item item)
        {
            switch (item.Category)
            {
                case ItemCategory.Scroll:
                case ItemCategory.Potion:
                    return item.Value() > 0;
                default:
                    return false;
            }
        }
    }
}
