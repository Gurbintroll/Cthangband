using Cthangband.Enumerations;
using System;

namespace Cthangband.Stores
{
    [Serializable]
    internal class GeneralStore : Store
    {
        public GeneralStore() : base(StoreType.StoreGeneral)
        {
        }

        public override string FeatureType => "GeneralStore";

        protected override ItemIdentifier[] GetStoreTable()
        {
            return new[]
            {
                new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                new ItemIdentifier(ItemCategory.Food, FoodType.Biscuit),
                new ItemIdentifier(ItemCategory.Food, FoodType.Jerky),
                new ItemIdentifier(ItemCategory.Food, FoodType.Jerky),
                new ItemIdentifier(ItemCategory.Food, FoodType.PintOfWine),
                new ItemIdentifier(ItemCategory.Food, FoodType.PintOfAle),
                new ItemIdentifier(ItemCategory.Light, LightType.Torch),
                new ItemIdentifier(ItemCategory.Light, LightType.Torch),
                new ItemIdentifier(ItemCategory.Light, LightType.Torch),
                new ItemIdentifier(ItemCategory.Light, LightType.Lantern),
                new ItemIdentifier(ItemCategory.Light, LightType.Lantern),
                new ItemIdentifier(ItemCategory.Light, LightType.Orb),
                new ItemIdentifier(ItemCategory.Flask, 0), new ItemIdentifier(ItemCategory.Flask, 0),
                new ItemIdentifier(ItemCategory.Flask, 0), new ItemIdentifier(ItemCategory.Flask, 0),
                new ItemIdentifier(ItemCategory.Flask, 0), new ItemIdentifier(ItemCategory.Flask, 0),
                new ItemIdentifier(ItemCategory.Spike, 0), new ItemIdentifier(ItemCategory.Spike, 0),
                new ItemIdentifier(ItemCategory.Shot, AmmoType.SvAmmoNormal),
                new ItemIdentifier(ItemCategory.Arrow, AmmoType.SvAmmoNormal),
                new ItemIdentifier(ItemCategory.Bolt, AmmoType.SvAmmoNormal),
                new ItemIdentifier(ItemCategory.Digging, DiggerType.SvShovel),
                new ItemIdentifier(ItemCategory.Digging, DiggerType.SvPick),
                new ItemIdentifier(ItemCategory.Cloak, CloakType.SvCloak),
                new ItemIdentifier(ItemCategory.Cloak, CloakType.SvCloak),
                new ItemIdentifier(ItemCategory.Cloak, CloakType.SvCloak),
                new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                new ItemIdentifier(ItemCategory.Light, LightType.Torch),
                new ItemIdentifier(ItemCategory.Light, LightType.Torch),
                new ItemIdentifier(ItemCategory.Light, LightType.Lantern),
                new ItemIdentifier(ItemCategory.Light, LightType.Lantern),
                new ItemIdentifier(ItemCategory.Flask, 0), new ItemIdentifier(ItemCategory.Flask, 0),
                new ItemIdentifier(ItemCategory.Flask, 0), new ItemIdentifier(ItemCategory.Flask, 0),
                new ItemIdentifier(ItemCategory.Shot, AmmoType.SvAmmoNormal),
                new ItemIdentifier(ItemCategory.Arrow, AmmoType.SvAmmoNormal),
                new ItemIdentifier(ItemCategory.Bolt, AmmoType.SvAmmoNormal),
                new ItemIdentifier(ItemCategory.Digging, DiggerType.SvShovel)
            };
        }

        protected override bool StoreWillBuy(Item item)
        {
            switch (item.Category)
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
                    return item.Value() > 0;
                default:
                    return false;
            }
        }
    }
}
