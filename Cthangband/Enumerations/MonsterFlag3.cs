// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
namespace Cthangband.Enumerations
{
    internal static class MonsterFlag3
    {
        public const uint Animal = 0x00000080;
        public const uint Cthuloid = 0x08000000;
        public const uint Demon = 0x00000010;
        public const uint Dragon = 0x00000008;
        public const uint Evil = 0x00000040;
        public const uint Giant = 0x00000004;
        public const uint Good = 0x00000200;
        public const uint GreatOldOne = 0x00000100;
        public const uint HurtByCold = 0x00008000;
        public const uint HurtByFire = 0x00004000;
        public const uint HurtByLight = 0x00001000;
        public const uint HurtByRock = 0x00002000;
        public const uint ImmuneAcid = 0x00010000;
        public const uint ImmuneCold = 0x00080000;
        public const uint ImmuneConfusion = 0x40000000;
        public const uint ImmuneFear = 0x10000000;
        public const uint ImmuneFire = 0x00040000;
        public const uint ImmuneLightning = 0x00020000;
        public const uint ImmunePoison = 0x00100000;
        public const uint ImmuneSleep = 0x80000000;
        public const uint ImmuneStun = 0x20000000;
        public const uint Nonliving = 0x00000800;
        public const uint Orc = 0x00000001;
        public const uint ResistDisenchant = 0x04000000;
        public const uint ResistNether = 0x00400000;
        public const uint ResistNexus = 0x02000000;
        public const uint ResistPlasma = 0x01000000;
        public const uint ResistTeleport = 0x00200000;
        public const uint ResistWater = 0x00800000;
        public const uint Troll = 0x00000002;
        public const uint Undead = 0x00000020;
        public const uint Xxx3 = 0x00000400;
    }
}