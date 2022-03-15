// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cthangband.Enumerations
{
    internal class UpdateFlags
    {
        public const uint UpdateBonuses = 0x00000001;
        public const uint UpdateDistances = 0x02000000;
        public const uint UpdateHealth = 0x00000010;
        public const uint UpdateLight = 0x00200000;
        public const uint UpdateMana = 0x00000020;
        public const uint UpdateMonsters = 0x01000000;
        public const uint UpdateRemoveLight = 0x00020000;
        public const uint UpdateRemoveView = 0x00010000;
        public const uint UpdateScent = 0x10000000;
        public const uint UpdateSpells = 0x00000040;
        public const uint UpdateTorchRadius = 0x00000002;
        public const uint UpdateView = 0x00100000;
    }
}