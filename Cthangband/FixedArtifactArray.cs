// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
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