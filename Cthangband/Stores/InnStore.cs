using Cthangband.Enumerations;
using System;

namespace Cthangband.Stores
{
    [Serializable]
    internal class InnStore : Store
    {
        public InnStore() : base(StoreType.StoreInn)
        {
        }

        public override string FeatureType => "Inn";

        protected override bool StoreWillBuy(Item item)
        {
            return false;
        }

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
                new ItemIdentifier(ItemCategory.Food, FoodType.PintOfWine),
                new ItemIdentifier(ItemCategory.Food, FoodType.PintOfAle),
                new ItemIdentifier(ItemCategory.Food, FoodType.PintOfWine),
                new ItemIdentifier(ItemCategory.Food, FoodType.PintOfAle),
                new ItemIdentifier(ItemCategory.Food, FoodType.PintOfWine),
                new ItemIdentifier(ItemCategory.Food, FoodType.PintOfAle),
                new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
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
                new ItemIdentifier(ItemCategory.Food, FoodType.PintOfWine),
                new ItemIdentifier(ItemCategory.Food, FoodType.PintOfAle),
                new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                new ItemIdentifier(ItemCategory.Food, FoodType.PintOfWine),
                new ItemIdentifier(ItemCategory.Food, FoodType.PintOfAle),
                new ItemIdentifier(ItemCategory.Food, FoodType.PintOfWine),
                new ItemIdentifier(ItemCategory.Food, FoodType.PintOfAle),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.SatisfyHunger),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.SatisfyHunger),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.SatisfyHunger),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.SatisfyHunger),
                new ItemIdentifier(ItemCategory.Food, FoodType.PintOfWine),
                new ItemIdentifier(ItemCategory.Food, FoodType.PintOfAle),
                new ItemIdentifier(ItemCategory.Food, FoodType.PintOfWine),
                new ItemIdentifier(ItemCategory.Food, FoodType.PintOfAle)
            };
        }

        public override int StoreMaxKeep => 4;
    }
}
