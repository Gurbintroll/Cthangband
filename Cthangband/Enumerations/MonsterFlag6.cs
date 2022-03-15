// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
namespace Cthangband.Enumerations
{
    internal static class MonsterFlag6
    {
        public const uint Blink = 0x00000010;
        public const uint BoltMask = 0;
        public const uint CreateTraps = 0x00002000;
        public const uint Darkness = 0x00001000;
        public const uint DreadCurse = 0x00000002;
        public const uint Forget = 0x00004000;
        public const uint Haste = 0x00000001;
        public const uint Heal = 0x00000004;

        public const uint IntMask = Blink | TeleportSelf | TeleportLevel | TeleportAway | Heal | Haste |
                                       CreateTraps | SummonKin | SummonReaver | SummonMonster | SummonMonsters | SummonAnt |
                                       SummonSpider | SummonHound | SummonHydra | SummonCthuloid | SummonDragon | SummonUndead |
                                       SummonDemon | SummonHiDragon | SummonHiUndead | SummonGreatOldOne | SummonUnique;

        public const uint SummonAnt = 0x00100000;
        public const uint SummonCthuloid = 0x01000000;
        public const uint SummonDemon = 0x02000000;
        public const uint SummonDragon = 0x08000000;
        public const uint SummonGreatOldOne = 0x40000000;
        public const uint SummonHiDragon = 0x20000000;
        public const uint SummonHiUndead = 0x10000000;
        public const uint SummonHound = 0x00400000;
        public const uint SummonHydra = 0x00800000;
        public const uint SummonKin = 0x00010000;

        public const uint SummonMask = SummonKin | SummonReaver | SummonMonster | SummonMonsters | SummonAnt | SummonSpider |
                                          SummonHound | SummonHydra | SummonCthuloid | SummonDemon | SummonUndead | SummonDragon |
                                          SummonHiUndead | SummonHiDragon | SummonGreatOldOne | SummonUnique;

        public const uint SummonMonster = 0x00040000;
        public const uint SummonMonsters = 0x00080000;
        public const uint SummonReaver = 0x00020000;
        public const uint SummonSpider = 0x00200000;
        public const uint SummonUndead = 0x04000000;
        public const uint SummonUnique = 0x80000000;
        public const uint TeleportAway = 0x00000200;
        public const uint TeleportLevel = 0x00000400;
        public const uint TeleportSelf = 0x00000020;
        public const uint TeleportTo = 0x00000100;
        public const uint Xxx2 = 0x00000008;
        public const uint Xxx3 = 0x00000040;
        public const uint Xxx4 = 0x00000080;
        public const uint Xxx5 = 0x00000800;
        public const uint Xxx6 = 0x00008000;
    }
}