using System;

namespace Cthangband
{
    [Serializable]
    internal class StoreOwner
    {
        public readonly int MaxCost;
        public readonly int MinInflate;
        public readonly string OwnerName;
        public readonly int OwnerRace;

        public StoreOwner(string ownerName, int maxCost, int minInflate, int ownerRace)
        {
            OwnerName = ownerName;
            MaxCost = maxCost;
            MinInflate = minInflate;
            OwnerRace = ownerRace;
        }
    }
}