using Cthangband.Enumerations;
using Cthangband.StaticData;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cthangband
{
    [Serializable]
    internal class FixedArtifactArray : Dictionary<FixedArtifactId, FixedArtifact>
    {
        public FixedArtifactArray()
        {
            foreach (KeyValuePair<string, BaseFixedartifact> pair in StaticResources.Instance.BaseFixedartifacts)
            {
                Add(pair.Value.FixedArtifactID, new FixedArtifact(pair.Value));
            }
        }

        public FixedArtifactArray(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            // Needed for serialising a dictionary
        }
    }
}