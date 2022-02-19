using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cthangband.Enumerations
{
    internal class UpdateFlags
    {
        public const uint PuBonus = 0x00000001;
        public const uint PuDistance = 0x02000000;
        public const uint PuFlow = 0x10000000;
        public const uint PuHp = 0x00000010;
        public const uint PuLight = 0x00200000;
        public const uint PuMana = 0x00000020;
        public const uint PuMonsters = 0x01000000;
        public const uint PuSpells = 0x00000040;
        public const uint PuTorch = 0x00000002;
        public const uint PuUnLight = 0x00020000;
        public const uint PuUnView = 0x00010000;
        public const uint PuView = 0x00100000;
    }
}
