using Cthangband.Enumerations;

namespace Cthangband
{
    internal class ItemIdentifier
    {
        public ItemCategory Category;
        public int SubCategory;

        public ItemIdentifier(ItemCategory category, int subCategory)
        {
            Category = category;
            SubCategory = subCategory;
        }
    }
}