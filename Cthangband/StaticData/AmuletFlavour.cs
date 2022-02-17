using Cthangband.Debug;
using System;

namespace Cthangband.StaticData
{
    [Serializable]
    internal class AmuletFlavour : EntityType
    {
        public override string ToString()
        {
            return Name;
        }
    }
}