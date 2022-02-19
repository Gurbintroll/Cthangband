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