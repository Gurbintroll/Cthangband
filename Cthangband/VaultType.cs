using Cthangband.StaticData;
using System;

namespace Cthangband
{
    [Serializable]
    internal class VaultType
    {
        public int Category;
        public int Height;
        public string Name;
        public int Rating;
        public string Text;
        public int Width;

        public VaultType()
        {
        }

        public VaultType(VaultType original)
        {
            Height = original.Height;
            Name = original.Name;
            Rating = original.Rating;
            Text = original.Text;
            Category = original.Category;
            Width = original.Width;
        }

        public VaultType(BaseVaultType original)
        {
            Height = original.Height;
            Name = original.Name;
            Rating = original.Rating;
            Text = original.Text;
            Category = original.Category;
            Width = original.Width;
        }
    }
}