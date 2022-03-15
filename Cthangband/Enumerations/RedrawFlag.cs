// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
namespace Cthangband.Enumerations
{
    internal static class RedrawFlag
    {
        public const uint PrAfraid = 0x00040000;
        public const uint PrArmor = 0x00000020;
        public const uint PrBasic = 0x02000000;
        public const uint PrBlind = 0x00010000;
        public const uint PrConfused = 0x00020000;
        public const uint PrCut = 0x00001000;
        public const uint PrDepth = 0x00000200;
        public const uint PrDtrap = 0x00008000;
        public const uint PrEquippy = 0x00000400;
        public const uint PrExp = 0x00000008;
        public const uint PrExtra = 0x01000000;
        public const uint PrGold = 0x00000100;
        public const uint PrHealth = 0x00000800;
        public const uint PrHp = 0x00000040;
        public const uint PrHunger = 0x00004000;
        public const uint PrLev = 0x00000004;
        public const uint PrMana = 0x00000080;
        public const uint PrMap = 0x04000000;
        public const uint PrMisc = 0x00000001;
        public const uint PrPoisoned = 0x00080000;
        public const uint PrSpeed = 0x00200000;
        public const uint PrState = 0x00100000;
        public const uint PrStats = 0x00000010;
        public const uint PrStudy = 0x00400000;
        public const uint PrStun = 0x00002000;
        public const uint PrTitle = 0x00000002;
        public const uint PrWipe = 0x08000000;
    }
}