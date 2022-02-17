using Cthangband.Debug;
using System;

namespace Cthangband.StaticData
{
    [Serializable]
    internal class WandFlavour : EntityType
    {
        public override string ToString()
        {
            return Name;
        }
    }
}