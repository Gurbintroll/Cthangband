using Cthangband.Debug;
using System;

namespace Cthangband.StaticData
{
    [Serializable]
    internal class RingFlavour : EntityType
    {
        public override string ToString()
        {
            return Name;
        }
    }
}