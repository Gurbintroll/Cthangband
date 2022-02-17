using Cthangband.Debug;
using System;

namespace Cthangband.StaticData
{
    [Serializable]
    internal class StaffFlavour : EntityType
    {
        public override string ToString()
        {
            return Name;
        }
    }
}