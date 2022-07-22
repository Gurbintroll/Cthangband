using Cthangband.Enumerations;
using System;

namespace Cthangband.Stores
{
    [Serializable]
    internal class HallStore : Store
    {
        public HallStore() : base(StoreType.StoreHall)
        {
        }

        public override string FeatureType => "HallOfRecords";

        protected override bool StoreWillBuy(Item item)
        {
            return false;
        }
        protected override bool MaintainsStockLevels => false;
        protected override bool HasStoreTable => false;

        public override bool ShufflesOwnersAndPricing => false;
        protected override string OwnerName => "";
        protected override string Title => "Hall Of Records";
        protected override StoreInventoryDisplayTypeEnum ShowInventoryDisplayType => StoreInventoryDisplayTypeEnum.DoNotShowInventory;
    }
}
