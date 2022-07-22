namespace Cthangband.Stores
{
    internal enum StoreInventoryDisplayTypeEnum
    {
        DoNotShowInventory,
        InventoryWithPrice,
        InventoryWithoutPrice
    }

    internal interface IStore
    {
        string FeatureType { get; }

        void EnterStore(Player player);

        void StoreInit();

        void StoreMaint();

        void StoreShuffle();
    }

}
