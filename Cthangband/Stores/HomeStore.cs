using Cthangband.Enumerations;
using System;

namespace Cthangband.Stores
{
    [Serializable]
    internal class HomeStore : Store
    {
        public HomeStore() : base(StoreType.StoreHome)
        {
        }

        public override string FeatureType => "Home";

        protected override bool StoreWillBuy(Item item)
        {
            return true;
        }

        protected override bool MaintainsStockLevels => false;

        protected override bool HasStoreTable => false;
        public override bool ShufflesOwnersAndPricing => false;

        protected override string OwnerName => "";

        protected override string Title => "Your Home";
        protected override StoreInventoryDisplayTypeEnum ShowInventoryDisplayType => StoreInventoryDisplayTypeEnum.InventoryWithoutPrice;
        protected override string SellPrompt => "Drop which item? ";
        protected override bool ItemsInstantlyIdentified => false;
        protected override string StoreFullMessage => "Your home is full.";
        protected override bool BuysItems => false;
        protected override string NoStockMessage => "Your home is empty.";
        protected override string PurchaseMessage => "Which item do you want to take? ";


    }
}