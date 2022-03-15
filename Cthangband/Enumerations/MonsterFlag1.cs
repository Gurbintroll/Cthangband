// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
namespace Cthangband.Enumerations
{
    internal static class MonsterFlag1
    {
        public const uint AttrClear = 0x00000040;
        public const uint AttrMulti = 0x00000080;
        public const uint CharClear = 0x00000010;
        public const uint CharMulti = 0x00000020;
        public const uint Drop_1D2 = 0x01000000;
        public const uint Drop_2D2 = 0x02000000;
        public const uint Drop_3D2 = 0x04000000;
        public const uint Drop_4D2 = 0x08000000;
        public const uint Drop60 = 0x00400000;
        public const uint Drop90 = 0x00800000;
        public const uint DropGood = 0x10000000;
        public const uint DropGreat = 0x20000000;
        public const uint Escorted = 0x00004000;
        public const uint EscortsGroup = 0x00008000;
        public const uint Female = 0x00000008;
        public const uint ForceMaxHp = 0x00000200;
        public const uint ForceSleep = 0x00000400;
        public const uint Friends = 0x00002000;
        public const uint Guardian = 0x00000002;
        public const uint Male = 0x00000004;
        public const uint NeverAttack = 0x00010000;
        public const uint NeverMove = 0x00020000;
        public const uint OnlyDropGold = 0x00100000;
        public const uint OnlyDropItem = 0x00200000;
        public const uint OnlyGuardian = 0x00000100;
        public const uint RandomMove25 = 0x00040000;
        public const uint RandomMove50 = 0x00080000;
        public const uint Unique = 0x00000001;
        public const uint Xxx1 = 0x00000800;
        public const uint Xxx2 = 0x40000000;
        public const uint Xxx3 = 0x80000000;
        public const uint Xxx4 = 0x00001000;
    }
}