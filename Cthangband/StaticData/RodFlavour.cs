using Cthangband.Debug;
using System;

namespace Cthangband.StaticData
{
    [Serializable]
    internal class RodFlavour : EntityType
    {
        public override string ToString()
        {
            return Name;
        }
    }
}