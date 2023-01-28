// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Debug;

namespace Cthangband.StaticData
{
    internal class BaseVaultType : EntityType
    {
        public int Category
        {
            get;
            set;
        }

        public int Height
        {
            get;
            set;
        }

        public int Rating
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public int Width
        {
            get;
            set;
        }
    }
}