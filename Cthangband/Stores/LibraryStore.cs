using Cthangband.Enumerations;
using System;

namespace Cthangband.Stores
{
    [Serializable]
    internal class LibraryStore : Store
    {
        public LibraryStore() : base(StoreType.StoreLibrary)
        {
        }

        public override string FeatureType => "Bookstore";

        protected override ItemIdentifier[] GetStoreTable()
        {
            return new[]
            {
                new ItemIdentifier(ItemCategory.SorceryBook, 0),
                new ItemIdentifier(ItemCategory.SorceryBook, 0),
                new ItemIdentifier(ItemCategory.SorceryBook, 1),
                new ItemIdentifier(ItemCategory.SorceryBook, 1), new ItemIdentifier(ItemCategory.NatureBook, 0),
                new ItemIdentifier(ItemCategory.NatureBook, 0), new ItemIdentifier(ItemCategory.NatureBook, 1),
                new ItemIdentifier(ItemCategory.NatureBook, 1), new ItemIdentifier(ItemCategory.ChaosBook, 0),
                new ItemIdentifier(ItemCategory.ChaosBook, 0), new ItemIdentifier(ItemCategory.ChaosBook, 1),
                new ItemIdentifier(ItemCategory.ChaosBook, 1), new ItemIdentifier(ItemCategory.DeathBook, 0),
                new ItemIdentifier(ItemCategory.DeathBook, 0), new ItemIdentifier(ItemCategory.DeathBook, 1),
                new ItemIdentifier(ItemCategory.DeathBook, 1), new ItemIdentifier(ItemCategory.TarotBook, 0),
                new ItemIdentifier(ItemCategory.TarotBook, 0), new ItemIdentifier(ItemCategory.TarotBook, 1),
                new ItemIdentifier(ItemCategory.TarotBook, 1), new ItemIdentifier(ItemCategory.FolkBook, 0),
                new ItemIdentifier(ItemCategory.FolkBook, 0), new ItemIdentifier(ItemCategory.FolkBook, 1),
                new ItemIdentifier(ItemCategory.FolkBook, 1), new ItemIdentifier(ItemCategory.FolkBook, 2),
                new ItemIdentifier(ItemCategory.FolkBook, 2), new ItemIdentifier(ItemCategory.FolkBook, 3),
                new ItemIdentifier(ItemCategory.FolkBook, 3), new ItemIdentifier(ItemCategory.LifeBook, 0),
                new ItemIdentifier(ItemCategory.LifeBook, 0), new ItemIdentifier(ItemCategory.LifeBook, 0),
                new ItemIdentifier(ItemCategory.LifeBook, 0), new ItemIdentifier(ItemCategory.LifeBook, 1),
                new ItemIdentifier(ItemCategory.LifeBook, 1), new ItemIdentifier(ItemCategory.LifeBook, 1),
                new ItemIdentifier(ItemCategory.LifeBook, 1), new ItemIdentifier(ItemCategory.DeathBook, 0),
                new ItemIdentifier(ItemCategory.DeathBook, 0), new ItemIdentifier(ItemCategory.DeathBook, 1),
                new ItemIdentifier(ItemCategory.DeathBook, 1),
                new ItemIdentifier(ItemCategory.CorporealBook, 0),
                new ItemIdentifier(ItemCategory.CorporealBook, 0),
                new ItemIdentifier(ItemCategory.CorporealBook, 1),
                new ItemIdentifier(ItemCategory.CorporealBook, 1),
                new ItemIdentifier(ItemCategory.NatureBook, 0), new ItemIdentifier(ItemCategory.NatureBook, 0),
                new ItemIdentifier(ItemCategory.NatureBook, 1), new ItemIdentifier(ItemCategory.NatureBook, 1)
            };
        }

        protected override bool StoreWillBuy(Item item)
        {
            switch (item.Category)
            {
                case ItemCategory.SorceryBook:
                case ItemCategory.NatureBook:
                case ItemCategory.ChaosBook:
                case ItemCategory.DeathBook:
                case ItemCategory.LifeBook:
                case ItemCategory.TarotBook:
                case ItemCategory.FolkBook:
                case ItemCategory.CorporealBook:
                    return item.Value() > 0;
                default:
                    return false;
            }
        }
    }
}
