// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
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
    internal class ArtifactArray : Dictionary<ArtifactId, Artifact>
    {
        public ArtifactArray()
        {
            foreach (var pair in StaticResources.Instance.BaseArtifacts)
            {
                Add(pair.Value.ArtifactID, new Artifact(pair.Value));
            }
        }

        public ArtifactArray(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            // Needed for serialising a dictionary
        }
    }
}