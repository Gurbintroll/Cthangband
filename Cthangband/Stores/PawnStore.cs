using Cthangband.Enumerations;
using System;

namespace Cthangband.Stores
{
    [Serializable]
    internal class PawnStore : Store
    {
        public PawnStore() : base(StoreType.StorePawn)
        {
        }

        public override string FeatureType => "Pawnbrokers";

        protected override bool StoreWillBuy(Item item)
        {
            return item.Value() > 0;
        }
        protected override bool HasStoreTable => false;
        
        protected override bool MaintainsStockLevels => false;
        public override bool ShufflesOwnersAndPricing => false;
        protected override string BoughtVerb => "pawn";
        protected override int PriceMultiplier => 3;
    }
}
