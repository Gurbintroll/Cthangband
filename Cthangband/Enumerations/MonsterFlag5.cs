// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
namespace Cthangband.Enumerations
{
    internal static class MonsterFlag5
    {
        public const uint AcidBall = 0x00000001;
        public const uint AcidBolt = 0x00010000;
        public const uint Blindness = 0x10000000;

        public const uint BoltMask = AcidBolt | LightningBolt | FireBolt | ColdBolt | PoisonBolt | NetherBolt |
                                        WaterBolt | ManaBolt | PlasmaBolt | IceBolt | MagicMissile;

        public const uint BrainSmash = 0x00000800;
        public const uint CauseCriticalWounds = 0x00004000;
        public const uint CauseLightWounds = 0x00001000;
        public const uint CauseMortalWounds = 0x00008000;
        public const uint CauseSeriousWounds = 0x00002000;
        public const uint ColdBall = 0x00000008;
        public const uint ColdBolt = 0x00080000;
        public const uint Confuse = 0x20000000;
        public const uint DarkBall = 0x00000100;
        public const uint DrainMana = 0x00000200;
        public const uint FireBall = 0x00000004;
        public const uint FireBolt = 0x00040000;
        public const uint Hold = 0x80000000;
        public const uint IceBolt = 0x02000000;
        public const uint IntMask = Hold | Slow | Confuse | Blindness | Scare;
        public const uint LightningBall = 0x00000002;
        public const uint LightningBolt = 0x00020000;
        public const uint MagicMissile = 0x04000000;
        public const uint ManaBall = 0x00000080;
        public const uint ManaBolt = 0x00800000;
        public const uint MindBlast = 0x00000400;
        public const uint NetherBall = 0x00000020;
        public const uint NetherBolt = 0x00200000;
        public const uint PlasmaBolt = 0x01000000;
        public const uint PoisonBall = 0x00000010;
        public const uint PoisonBolt = 0x00100000;
        public const uint Scare = 0x08000000;
        public const uint Slow = 0x40000000;
        public const uint SummonMask = 0;
        public const uint WaterBall = 0x00000040;
        public const uint WaterBolt = 0x00400000;
    }
}