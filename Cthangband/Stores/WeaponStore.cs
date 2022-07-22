using Cthangband.Enumerations;
using System;

namespace Cthangband.Stores
{
    [Serializable]
    internal class WeaponStore : Store
    {
        public WeaponStore() : base(StoreType.StoreWeapon)
        {
        }

        public override string FeatureType => "Weaponsmiths";

        protected override ItemIdentifier[] GetStoreTable()
        {
            return new[]
            {
                new ItemIdentifier(ItemCategory.Sword, SwordType.SvDagger),
                new ItemIdentifier(ItemCategory.Sword, SwordType.SvMainGauche),
                new ItemIdentifier(ItemCategory.Sword, SwordType.SvRapier),
                new ItemIdentifier(ItemCategory.Sword, SwordType.SvSmallSword),
                new ItemIdentifier(ItemCategory.Sword, SwordType.SvShortSword),
                new ItemIdentifier(ItemCategory.Sword, SwordType.SvSabre),
                new ItemIdentifier(ItemCategory.Sword, SwordType.SvCutlass),
                new ItemIdentifier(ItemCategory.Sword, SwordType.SvTulwar),
                new ItemIdentifier(ItemCategory.Sword, SwordType.SvBroadSword),
                new ItemIdentifier(ItemCategory.Sword, SwordType.SvLongSword),
                new ItemIdentifier(ItemCategory.Sword, SwordType.SvScimitar),
                new ItemIdentifier(ItemCategory.Sword, SwordType.SvKatana),
                new ItemIdentifier(ItemCategory.Sword, SwordType.SvBastardSword),
                new ItemIdentifier(ItemCategory.Polearm, PolearmType.SvSpear),
                new ItemIdentifier(ItemCategory.Polearm, PolearmType.SvAwlPike),
                new ItemIdentifier(ItemCategory.Polearm, PolearmType.SvTrident),
                new ItemIdentifier(ItemCategory.Polearm, PolearmType.SvPike),
                new ItemIdentifier(ItemCategory.Polearm, PolearmType.SvBeakedAxe),
                new ItemIdentifier(ItemCategory.Polearm, PolearmType.SvBroadAxe),
                new ItemIdentifier(ItemCategory.Polearm, PolearmType.SvLance),
                new ItemIdentifier(ItemCategory.Polearm, PolearmType.SvBattleAxe),
                new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvWhip),
                new ItemIdentifier(ItemCategory.Bow, BowType.SvSling),
                new ItemIdentifier(ItemCategory.Bow, BowType.SvShortBow),
                new ItemIdentifier(ItemCategory.Bow, BowType.SvLongBow),
                new ItemIdentifier(ItemCategory.Bow, BowType.SvLightXbow),
                new ItemIdentifier(ItemCategory.Shot, AmmoType.SvAmmoNormal),
                new ItemIdentifier(ItemCategory.Shot, AmmoType.SvAmmoNormal),
                new ItemIdentifier(ItemCategory.Arrow, AmmoType.SvAmmoNormal),
                new ItemIdentifier(ItemCategory.Arrow, AmmoType.SvAmmoNormal),
                new ItemIdentifier(ItemCategory.Bolt, AmmoType.SvAmmoNormal),
                new ItemIdentifier(ItemCategory.Bolt, AmmoType.SvAmmoNormal),
                new ItemIdentifier(ItemCategory.Bow, BowType.SvLongBow),
                new ItemIdentifier(ItemCategory.Bow, BowType.SvLightXbow),
                new ItemIdentifier(ItemCategory.Arrow, AmmoType.SvAmmoNormal),
                new ItemIdentifier(ItemCategory.Arrow, AmmoType.SvAmmoNormal),
                new ItemIdentifier(ItemCategory.Bolt, AmmoType.SvAmmoNormal),
                new ItemIdentifier(ItemCategory.Bolt, AmmoType.SvAmmoNormal),
                new ItemIdentifier(ItemCategory.Bow, BowType.SvShortBow),
                new ItemIdentifier(ItemCategory.Sword, SwordType.SvDagger),
                new ItemIdentifier(ItemCategory.Sword, SwordType.SvMainGauche),
                new ItemIdentifier(ItemCategory.Sword, SwordType.SvRapier),
                new ItemIdentifier(ItemCategory.Sword, SwordType.SvSmallSword),
                new ItemIdentifier(ItemCategory.Sword, SwordType.SvShortSword),
                new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvWhip),
                new ItemIdentifier(ItemCategory.Sword, SwordType.SvBroadSword),
                new ItemIdentifier(ItemCategory.Sword, SwordType.SvLongSword),
                new ItemIdentifier(ItemCategory.Sword, SwordType.SvScimitar)
            };
        }

        protected override bool StoreWillBuy(Item item)
        {
            switch (item.Category)
            {
                case ItemCategory.Shot:
                case ItemCategory.Bolt:
                case ItemCategory.Arrow:
                case ItemCategory.Bow:
                case ItemCategory.Digging:
                case ItemCategory.Hafted:
                case ItemCategory.Polearm:
                case ItemCategory.Sword:
                    return item.Value() > 0;
                default:
                    return false;
            }
        }
    }
}
