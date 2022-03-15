// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
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