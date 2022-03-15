// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
namespace Cthangband.Enumerations
{
    internal static class ItemFlag1
    {
        public const uint Blows = 0x00002000;
        public const uint BrandAcid = 0x10000000;
        public const uint BrandCold = 0x80000000;
        public const uint BrandElec = 0x20000000;
        public const uint BrandFire = 0x40000000;
        public const uint BrandPois = 0x08000000;
        public const uint Cha = 0x00000020;
        public const uint Chaotic = 0x00004000;
        public const uint Con = 0x00000010;
        public const uint Dex = 0x00000008;
        public const uint Impact = 0x04000000;
        public const uint Infra = 0x00000400;
        public const uint Int = 0x00000002;
        public const uint KillDragon = 0x01000000;
        public const uint PvalMask = Str | Int | Wis | Dex | Con | Cha | Stealth | Search | Infra | Tunnel | Speed | Blows;
        public const uint Search = 0x00000200;
        public const uint SlayAnimal = 0x00010000;
        public const uint SlayDemon = 0x00080000;
        public const uint SlayDragon = 0x00800000;
        public const uint SlayEvil = 0x00020000;
        public const uint SlayGiant = 0x00400000;
        public const uint SlayOrc = 0x00100000;
        public const uint SlayTroll = 0x00200000;
        public const uint SlayUndead = 0x00040000;
        public const uint Speed = 0x00001000;
        public const uint Stealth = 0x00000100;
        public const uint Str = 0x00000001;
        public const uint Tunnel = 0x00000800;
        public const uint Vampiric = 0x00008000;
        public const uint Vorpal = 0x02000000;
        public const uint Wis = 0x00000004;
        public const uint Xxx1 = 0x00000080;
        public const uint Xxx2 = 0x00000040;
    }
}