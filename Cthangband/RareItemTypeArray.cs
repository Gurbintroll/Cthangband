using Cthangband.StaticData;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cthangband
{
    [Serializable]
    internal class RareItemTypeArray : Dictionary<Enumerations.RareItemType, RareItemType>
    {
        public RareItemTypeArray()
        {
            foreach (KeyValuePair<string, BaseRareItemType> pair in StaticResources.Instance.BaseRareItemTypes)
            {
                Add(pair.Value.RareItemType, new RareItemType(pair.Value));
            }
        }

        public RareItemTypeArray(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            // Needed for serialising a dictionary
        }
    }
}