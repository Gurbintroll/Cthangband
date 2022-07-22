using Cthangband.Enumerations;
using System;

namespace Cthangband.Stores
{
    [Serializable]
    internal class BlackStore : Store
    {
        public BlackStore() : base(StoreType.StoreBlack)
        {
        }

        public override string FeatureType => "BlackMarket";

        protected override bool HasStoreTable => false;

        protected override bool StoreWillBuy(Item item)
        {
            return item.Value() > 0;
        }

        protected override int PriceMultiplier => 2;
    }
}
