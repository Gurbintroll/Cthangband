﻿// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
namespace Cthangband.Enumerations
{
    internal static class ItemFlag2
    {
        public const uint FreeAct = 0x00004000;
        public const uint HoldLife = 0x00008000;
        public const uint ImAcid = 0x00000100;
        public const uint ImCold = 0x00000800;
        public const uint ImElec = 0x00000200;
        public const uint ImFire = 0x00000400;
        public const uint Reflect = 0x00002000;
        public const uint ResAcid = 0x00010000;
        public const uint ResBlind = 0x01000000;
        public const uint ResChaos = 0x40000000;
        public const uint ResCold = 0x00080000;
        public const uint ResConf = 0x02000000;
        public const uint ResDark = 0x00800000;
        public const uint ResDisen = 0x80000000;
        public const uint ResElec = 0x00020000;
        public const uint ResFear = 0x00200000;
        public const uint ResFire = 0x00040000;
        public const uint ResLight = 0x00400000;
        public const uint ResNether = 0x10000000;
        public const uint ResNexus = 0x20000000;
        public const uint ResPois = 0x00100000;
        public const uint ResShards = 0x08000000;
        public const uint ResSound = 0x04000000;
        public const uint SustCha = 0x00000020;
        public const uint SustCon = 0x00000010;
        public const uint SustDex = 0x00000008;
        public const uint SustInt = 0x00000002;
        public const uint SustStr = 0x00000001;
        public const uint SustWis = 0x00000004;
        public const uint Xxx1 = 0x00000040;
        public const uint Xxx2 = 0x00000080;
        public const uint Xxx3 = 0x00001000;
    }
}