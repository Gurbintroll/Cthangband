using Cthangband.StaticData;
using System;
using System.Collections.Generic;

namespace Cthangband
{
    [Serializable]
    internal class VaultTypeArray : List<VaultType>
    {
        public VaultTypeArray()
        {
            foreach (KeyValuePair<string, BaseVaultType> baseType in StaticResources.Instance.BaseVaultTypes)
            {
                Add(new VaultType(baseType.Value));
            }
        }
    }
}