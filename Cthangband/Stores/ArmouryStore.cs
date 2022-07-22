using Cthangband.Enumerations;
using System;

namespace Cthangband.Stores
{
    [Serializable]
    internal class ArmouryStore : Store
    {
        public ArmouryStore() : base(StoreType.StoreArmoury)
        {
        }

        public override string FeatureType => "Armoury";

        protected override ItemIdentifier[] GetStoreTable()
        {
            return new[]
            {
                new ItemIdentifier(ItemCategory.Boots, BootsType.SvPairOfSoftLeatherBoots),
                new ItemIdentifier(ItemCategory.Boots, BootsType.SvPairOfSoftLeatherBoots),
                new ItemIdentifier(ItemCategory.Boots, BootsType.SvPairOfHardLeatherBoots),
                new ItemIdentifier(ItemCategory.Boots, BootsType.SvPairOfHardLeatherBoots),
                new ItemIdentifier(ItemCategory.Helm, HelmType.SvHardLeatherCap),
                new ItemIdentifier(ItemCategory.Helm, HelmType.SvHardLeatherCap),
                new ItemIdentifier(ItemCategory.Helm, HelmType.SvMetalCap),
                new ItemIdentifier(ItemCategory.Helm, HelmType.SvIronHelm),
                new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvRobe),
                new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvRobe),
                new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvSoftLeatherArmor),
                new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvSoftLeatherArmor),
                new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvHardLeatherArmor),
                new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvHardLeatherArmor),
                new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvHardStuddedLeather),
                new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvHardStuddedLeather),
                new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvLeatherScaleMail),
                new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvLeatherScaleMail),
                new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.SvMetalScaleMail),
                new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.SvChainMail),
                new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.SvChainMail),
                new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.SvAugmentedChainMail),
                new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.SvBarChainMail),
                new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.SvDoubleChainMail),
                new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.SvMetalBrigandineArmor),
                new ItemIdentifier(ItemCategory.Gloves, GlovesType.SvSetOfLeatherGloves),
                new ItemIdentifier(ItemCategory.Gloves, GlovesType.SvSetOfLeatherGloves),
                new ItemIdentifier(ItemCategory.Gloves, GlovesType.SvSetOfGauntlets),
                new ItemIdentifier(ItemCategory.Shield, ShieldType.SvSmallLeatherShield),
                new ItemIdentifier(ItemCategory.Shield, ShieldType.SvSmallLeatherShield),
                new ItemIdentifier(ItemCategory.Shield, ShieldType.SvLargeLeatherShield),
                new ItemIdentifier(ItemCategory.Shield, ShieldType.SvSmallMetalShield),
                new ItemIdentifier(ItemCategory.Boots, BootsType.SvPairOfHardLeatherBoots),
                new ItemIdentifier(ItemCategory.Boots, BootsType.SvPairOfHardLeatherBoots),
                new ItemIdentifier(ItemCategory.Helm, HelmType.SvHardLeatherCap),
                new ItemIdentifier(ItemCategory.Helm, HelmType.SvHardLeatherCap),
                new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvRobe),
                new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvSoftLeatherArmor),
                new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvSoftLeatherArmor),
                new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvHardLeatherArmor),
                new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvLeatherScaleMail),
                new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.SvMetalScaleMail),
                new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.SvChainMail),
                new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.SvChainMail),
                new ItemIdentifier(ItemCategory.Gloves, GlovesType.SvSetOfLeatherGloves),
                new ItemIdentifier(ItemCategory.Gloves, GlovesType.SvSetOfGauntlets),
                new ItemIdentifier(ItemCategory.Shield, ShieldType.SvSmallLeatherShield),
                new ItemIdentifier(ItemCategory.Shield, ShieldType.SvSmallLeatherShield)
            };
        }

        protected override bool StoreWillBuy(Item item)
        {
            switch (item.Category)
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
                    return item.Value() > 0;
                default:
                    return false;
            }
        }
    }
}
