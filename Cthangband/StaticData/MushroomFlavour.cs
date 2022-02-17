using Cthangband.Debug;
using System;

namespace Cthangband.StaticData
{
    [Serializable]
    internal class MushroomFlavour : EntityType
    {
        public override string ToString()
        {
            return Name;
        }
    }
}