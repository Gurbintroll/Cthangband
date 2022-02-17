using Cthangband.Debug;
using System;

namespace Cthangband.StaticData
{
    [Serializable]
    internal class PotionFlavour : EntityType
    {
        public override string ToString()
        {
            return Name;
        }
    }
}