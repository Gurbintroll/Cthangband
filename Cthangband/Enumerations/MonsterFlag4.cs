// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
namespace Cthangband.Enumerations
{
    internal static class MonsterFlag4
    {
        public const uint Arrow1D6 = 0x00000010;
        public const uint Arrow3D6 = 0x00000020;
        public const uint Arrow5D6 = 0x00000040;
        public const uint Arrow7D6 = 0x00000080;
        public const uint BoltMask = Arrow1D6 | Arrow3D6 | Arrow5D6 | Arrow7D6;
        public const uint BreatheAcid = 0x00000100;
        public const uint BreatheChaos = 0x00040000;
        public const uint BreatheCold = 0x00000800;
        public const uint BreatheConfusion = 0x00010000;
        public const uint BreatheDark = 0x00008000;
        public const uint BreatheDisenchant = 0x00080000;
        public const uint BreatheDisintegration = 0x80000000;
        public const uint BreatheFire = 0x00000400;
        public const uint BreatheForce = 0x04000000;
        public const uint BreatheGravity = 0x00800000;
        public const uint BreatheInertia = 0x00400000;
        public const uint BreatheLight = 0x00004000;
        public const uint BreatheLightning = 0x00000200;
        public const uint BreatheMana = 0x08000000;
        public const uint BreatheNether = 0x00002000;
        public const uint BreatheNexus = 0x00100000;
        public const uint BreathePlasma = 0x02000000;
        public const uint BreathePoison = 0x00001000;
        public const uint BreatheRadiation = 0x20000000;
        public const uint BreatheShards = 0x01000000;
        public const uint BreatheSound = 0x00020000;
        public const uint BreatheTime = 0x00200000;
        public const uint ChaosBall = 0x40000000;
        public const uint IntMask = 0;
        public const uint RadiationBall = 0x10000000;
        public const uint ShardBall = 0x00000008;
        public const uint Shriek = 0x00000001;
        public const uint SummonMask = 0;
        public const uint Xxx2 = 0x00000002;
        public const uint Xxx3 = 0x00000004;
    }
}