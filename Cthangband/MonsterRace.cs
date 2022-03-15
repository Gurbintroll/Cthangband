// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.StaticData;
using System;

namespace Cthangband
{
    [Serializable]
    internal class MonsterRace
    {
        public readonly int ArmourClass;
        public readonly MonsterAttack[] Attack = new MonsterAttack[4];
        public char Character;
        public Colour Colour;
        public int CurNum;
        public string Description;
        public uint Flags1;
        public uint Flags2;
        public uint Flags3;
        public uint Flags4;
        public uint Flags5;
        public uint Flags6;
        public int FreqInate;
        public int FreqSpell;
        public int Hdice;
        public int Hside;
        public int Index;
        public MonsterKnowledge Knowledge;
        public int Level;
        public int MaxNum;
        public int Mexp;
        public string Name;
        public int NoticeRange;
        public int Rarity;
        public int Sleep;
        public int Speed;
        public string SplitName1;
        public string SplitName2;
        public string SplitName3;

        public MonsterRace()
        {
        }

        public MonsterRace(BaseMonsterRace original, int index)
        {
            Name = original.FriendlyName;
            SplitName1 = original.SplitName1;
            SplitName2 = original.SplitName2;
            SplitName3 = original.SplitName3;
            Index = index;
            Character = original.Character;
            Colour = original.Colour;
            Description = original.Description;
            Hdice = original.Hdice;
            Hside = original.Hside;
            ArmourClass = original.ArmourClass;
            Sleep = original.Sleep;
            NoticeRange = original.NoticeRange;
            Speed = original.Speed;
            Mexp = original.Mexp;
            FreqInate = original.FreqInate == 0 ? 0 : 100 / original.FreqInate;
            FreqSpell = original.FreqSpell == 0 ? 0 : 100 / original.FreqSpell;
            Attack = new MonsterAttack[4];
            Attack[0] = original.Attack1;
            Attack[1] = original.Attack2;
            Attack[2] = original.Attack3;
            Attack[3] = original.Attack4;
            Level = original.Level;
            if (Level < 0)
            {
                Level = 0;
            }
            if (Level > 100)
            {
                Level = 0;
            }
            Rarity = original.Rarity;
            Flags1 |= original.AttrClear ? MonsterFlag1.AttrClear : 0;
            Flags1 |= original.AttrMulti ? MonsterFlag1.AttrMulti : 0;
            Flags1 |= original.CharClear ? MonsterFlag1.CharClear : 0;
            Flags1 |= original.CharMulti ? MonsterFlag1.CharMulti : 0;
            Flags1 |= original.Drop_1D2 ? MonsterFlag1.Drop_1D2 : 0;
            Flags1 |= original.Drop_2D2 ? MonsterFlag1.Drop_2D2 : 0;
            Flags1 |= original.Drop_3D2 ? MonsterFlag1.Drop_3D2 : 0;
            Flags1 |= original.Drop_4D2 ? MonsterFlag1.Drop_4D2 : 0;
            Flags1 |= original.Drop60 ? MonsterFlag1.Drop60 : 0;
            Flags1 |= original.Drop90 ? MonsterFlag1.Drop90 : 0;
            Flags1 |= original.DropGood ? MonsterFlag1.DropGood : 0;
            Flags1 |= original.DropGreat ? MonsterFlag1.DropGreat : 0;
            Flags1 |= original.Escorted ? MonsterFlag1.Escorted : 0;
            Flags1 |= original.EscortsGroup ? MonsterFlag1.EscortsGroup : 0;
            Flags1 |= original.Female ? MonsterFlag1.Female : 0;
            Flags1 |= original.ForceMaxHp ? MonsterFlag1.ForceMaxHp : 0;
            Flags1 |= original.ForceSleep ? MonsterFlag1.ForceSleep : 0;
            Flags1 |= original.Friends ? MonsterFlag1.Friends : 0;
            Flags1 |= original.Male ? MonsterFlag1.Male : 0;
            Flags1 |= original.NeverAttack ? MonsterFlag1.NeverAttack : 0;
            Flags1 |= original.NeverMove ? MonsterFlag1.NeverMove : 0;
            Flags1 |= original.OnlyDropGold ? MonsterFlag1.OnlyDropGold : 0;
            Flags1 |= original.OnlyDropItem ? MonsterFlag1.OnlyDropItem : 0;
            Flags1 |= original.RandomMove25 ? MonsterFlag1.RandomMove25 : 0;
            Flags1 |= original.RandomMove50 ? MonsterFlag1.RandomMove50 : 0;
            Flags1 |= original.Unique ? MonsterFlag1.Unique : 0;
            Flags2 |= original.AttrAny ? MonsterFlag2.AttrAny : 0;
            Flags2 |= original.LightningAura ? MonsterFlag2.LightningAura : 0;
            Flags2 |= original.FireAura ? MonsterFlag2.FireAura : 0;
            Flags2 |= original.BashDoor ? MonsterFlag2.BashDoor : 0;
            Flags2 |= original.ColdBlood ? MonsterFlag2.ColdBlood : 0;
            Flags2 |= original.ColdBlood ? MonsterFlag2.ColdBlood : 0;
            Flags2 |= original.EmptyMind ? MonsterFlag2.EmptyMind : 0;
            Flags2 |= original.Invisible ? MonsterFlag2.Invisible : 0;
            Flags2 |= original.KillBody ? MonsterFlag2.KillBody : 0;
            Flags2 |= original.KillItem ? MonsterFlag2.KillItem : 0;
            Flags2 |= original.KillWall ? MonsterFlag2.KillWall : 0;
            Flags2 |= original.MoveBody ? MonsterFlag2.MoveBody : 0;
            Flags2 |= original.Multiply ? MonsterFlag2.Multiply : 0;
            Flags2 |= original.OpenDoor ? MonsterFlag2.OpenDoor : 0;
            Flags2 |= original.PassWall ? MonsterFlag2.PassWall : 0;
            Flags2 |= original.Powerful ? MonsterFlag2.Powerful : 0;
            Flags2 |= original.Reflecting ? MonsterFlag2.Reflecting : 0;
            Flags2 |= original.Regenerate ? MonsterFlag2.Regenerate : 0;
            Flags2 |= original.Shapechanger ? MonsterFlag2.Shapechanger : 0;
            Flags2 |= original.Smart ? MonsterFlag2.Smart : 0;
            Flags2 |= original.Stupid ? MonsterFlag2.Stupid : 0;
            Flags2 |= original.TakeItem ? MonsterFlag2.TakeItem : 0;
            Flags2 |= original.WeirdMind ? MonsterFlag2.WeirdMind : 0;
            Flags3 |= original.Animal ? MonsterFlag3.Animal : 0;
            Flags3 |= original.Cthuloid ? MonsterFlag3.Cthuloid : 0;
            Flags3 |= original.Demon ? MonsterFlag3.Demon : 0;
            Flags3 |= original.Dragon ? MonsterFlag3.Dragon : 0;
            Flags3 |= original.Evil ? MonsterFlag3.Evil : 0;
            Flags3 |= original.Giant ? MonsterFlag3.Giant : 0;
            Flags3 |= original.Good ? MonsterFlag3.Good : 0;
            Flags3 |= original.GreatOldOne ? MonsterFlag3.GreatOldOne : 0;
            Flags3 |= original.HurtByCold ? MonsterFlag3.HurtByCold : 0;
            Flags3 |= original.HurtByFire ? MonsterFlag3.HurtByFire : 0;
            Flags3 |= original.HurtByLight ? MonsterFlag3.HurtByLight : 0;
            Flags3 |= original.HurtByRock ? MonsterFlag3.HurtByRock : 0;
            Flags3 |= original.ImmuneAcid ? MonsterFlag3.ImmuneAcid : 0;
            Flags3 |= original.ImmuneCold ? MonsterFlag3.ImmuneCold : 0;
            Flags3 |= original.ImmuneLightning ? MonsterFlag3.ImmuneLightning : 0;
            Flags3 |= original.ImmuneFire ? MonsterFlag3.ImmuneFire : 0;
            Flags3 |= original.ImmunePoison ? MonsterFlag3.ImmunePoison : 0;
            Flags3 |= original.ImmuneConfusion ? MonsterFlag3.ImmuneConfusion : 0;
            Flags3 |= original.ImmuneFear ? MonsterFlag3.ImmuneFear : 0;
            Flags3 |= original.Nonliving ? MonsterFlag3.Nonliving : 0;
            Flags3 |= original.ImmuneSleep ? MonsterFlag3.ImmuneSleep : 0;
            Flags3 |= original.ImmuneStun ? MonsterFlag3.ImmuneStun : 0;
            Flags3 |= original.Orc ? MonsterFlag3.Orc : 0;
            Flags3 |= original.ResistDisenchant ? MonsterFlag3.ResistDisenchant : 0;
            Flags3 |= original.ResistNether ? MonsterFlag3.ResistNether : 0;
            Flags3 |= original.ResistNexus ? MonsterFlag3.ResistNexus : 0;
            Flags3 |= original.ResistPlasma ? MonsterFlag3.ResistPlasma : 0;
            Flags3 |= original.ResistTeleport ? MonsterFlag3.ResistTeleport : 0;
            Flags3 |= original.ResistWater ? MonsterFlag3.ResistWater : 0;
            Flags3 |= original.Troll ? MonsterFlag3.Troll : 0;
            Flags3 |= original.Undead ? MonsterFlag3.Undead : 0;
            Flags4 |= original.Arrow1D6 ? MonsterFlag4.Arrow1D6 : 0;
            Flags4 |= original.Arrow3D6 ? MonsterFlag4.Arrow3D6 : 0;
            Flags4 |= original.Arrow5D6 ? MonsterFlag4.Arrow5D6 : 0;
            Flags4 |= original.Arrow7D6 ? MonsterFlag4.Arrow7D6 : 0;
            Flags4 |= original.ChaosBall ? MonsterFlag4.ChaosBall : 0;
            Flags4 |= original.RadiationBall ? MonsterFlag4.RadiationBall : 0;
            Flags4 |= original.BreatheAcid ? MonsterFlag4.BreatheAcid : 0;
            Flags4 |= original.BreatheChaos ? MonsterFlag4.BreatheChaos : 0;
            Flags4 |= original.BreatheCold ? MonsterFlag4.BreatheCold : 0;
            Flags4 |= original.BreatheConfusion ? MonsterFlag4.BreatheConfusion : 0;
            Flags4 |= original.BreatheDark ? MonsterFlag4.BreatheDark : 0;
            Flags4 |= original.BreatheDisenchant ? MonsterFlag4.BreatheDisenchant : 0;
            Flags4 |= original.BreatheDisintegration ? MonsterFlag4.BreatheDisintegration : 0;
            Flags4 |= original.BreatheLightning ? MonsterFlag4.BreatheLightning : 0;
            Flags4 |= original.BreatheFire ? MonsterFlag4.BreatheFire : 0;
            Flags4 |= original.BreatheGravity ? MonsterFlag4.BreatheGravity : 0;
            Flags4 |= original.BreatheInertia ? MonsterFlag4.BreatheInertia : 0;
            Flags4 |= original.BreatheLight ? MonsterFlag4.BreatheLight : 0;
            Flags4 |= original.BreatheMana ? MonsterFlag4.BreatheMana : 0;
            Flags4 |= original.BreatheNether ? MonsterFlag4.BreatheNether : 0;
            Flags4 |= original.BreatheNexus ? MonsterFlag4.BreatheNexus : 0;
            Flags4 |= original.BreatheRadiation ? MonsterFlag4.BreatheRadiation : 0;
            Flags4 |= original.BreathePlasma ? MonsterFlag4.BreathePlasma : 0;
            Flags4 |= original.BreathePoison ? MonsterFlag4.BreathePoison : 0;
            Flags4 |= original.BreatheShards ? MonsterFlag4.BreatheShards : 0;
            Flags4 |= original.BreatheSound ? MonsterFlag4.BreatheSound : 0;
            Flags4 |= original.BreatheTime ? MonsterFlag4.BreatheTime : 0;
            Flags4 |= original.BreatheForce ? MonsterFlag4.BreatheForce : 0;
            Flags4 |= original.ShardBall ? MonsterFlag4.ShardBall : 0;
            Flags4 |= original.Shriek ? MonsterFlag4.Shriek : 0;
            Flags5 |= original.AcidBall ? MonsterFlag5.AcidBall : 0;
            Flags5 |= original.ColdBall ? MonsterFlag5.ColdBall : 0;
            Flags5 |= original.DarkBall ? MonsterFlag5.DarkBall : 0;
            Flags5 |= original.LightningBall ? MonsterFlag5.LightningBall : 0;
            Flags5 |= original.FireBall ? MonsterFlag5.FireBall : 0;
            Flags5 |= original.ManaBall ? MonsterFlag5.ManaBall : 0;
            Flags5 |= original.NetherBall ? MonsterFlag5.NetherBall : 0;
            Flags5 |= original.PoisonBall ? MonsterFlag5.PoisonBall : 0;
            Flags5 |= original.WaterBall ? MonsterFlag5.WaterBall : 0;
            Flags5 |= original.Blindness ? MonsterFlag5.Blindness : 0;
            Flags5 |= original.AcidBolt ? MonsterFlag5.AcidBolt : 0;
            Flags5 |= original.ColdBolt ? MonsterFlag5.ColdBolt : 0;
            Flags5 |= original.LightningBolt ? MonsterFlag5.LightningBolt : 0;
            Flags5 |= original.FireBolt ? MonsterFlag5.FireBolt : 0;
            Flags5 |= original.IceBolt ? MonsterFlag5.IceBolt : 0;
            Flags5 |= original.ManaBolt ? MonsterFlag5.ManaBolt : 0;
            Flags5 |= original.NetherBolt ? MonsterFlag5.NetherBolt : 0;
            Flags5 |= original.PlasmaBolt ? MonsterFlag5.PlasmaBolt : 0;
            Flags5 |= original.PoisonBolt ? MonsterFlag5.PoisonBolt : 0;
            Flags5 |= original.WaterBolt ? MonsterFlag5.WaterBolt : 0;
            Flags5 |= original.BrainSmash ? MonsterFlag5.BrainSmash : 0;
            Flags5 |= original.CauseLightWounds ? MonsterFlag5.CauseLightWounds : 0;
            Flags5 |= original.CauseSeriousWounds ? MonsterFlag5.CauseSeriousWounds : 0;
            Flags5 |= original.CauseCriticalWounds ? MonsterFlag5.CauseCriticalWounds : 0;
            Flags5 |= original.CauseMortalWounds ? MonsterFlag5.CauseMortalWounds : 0;
            Flags5 |= original.Confuse ? MonsterFlag5.Confuse : 0;
            Flags5 |= original.DrainMana ? MonsterFlag5.DrainMana : 0;
            Flags5 |= original.Hold ? MonsterFlag5.Hold : 0;
            Flags5 |= original.MindBlast ? MonsterFlag5.MindBlast : 0;
            Flags5 |= original.MagicMissile ? MonsterFlag5.MagicMissile : 0;
            Flags5 |= original.Scare ? MonsterFlag5.Scare : 0;
            Flags5 |= original.Slow ? MonsterFlag5.Slow : 0;
            Flags6 |= original.Blink ? MonsterFlag6.Blink : 0;
            Flags6 |= original.Darkness ? MonsterFlag6.Darkness : 0;
            Flags6 |= original.DreadCurse ? MonsterFlag6.DreadCurse : 0;
            Flags6 |= original.Forget ? MonsterFlag6.Forget : 0;
            Flags6 |= original.Haste ? MonsterFlag6.Haste : 0;
            Flags6 |= original.Heal ? MonsterFlag6.Heal : 0;
            Flags6 |= original.SummonAnt ? MonsterFlag6.SummonAnt : 0;
            Flags6 |= original.SummonCthuloid ? MonsterFlag6.SummonCthuloid : 0;
            Flags6 |= original.SummonDemon ? MonsterFlag6.SummonDemon : 0;
            Flags6 |= original.SummonDragon ? MonsterFlag6.SummonDragon : 0;
            Flags6 |= original.SummonGreatOldOne ? MonsterFlag6.SummonGreatOldOne : 0;
            Flags6 |= original.SummonHiDragon ? MonsterFlag6.SummonHiDragon : 0;
            Flags6 |= original.SummonHiUndead ? MonsterFlag6.SummonHiUndead : 0;
            Flags6 |= original.SummonHound ? MonsterFlag6.SummonHound : 0;
            Flags6 |= original.SummonHydra ? MonsterFlag6.SummonHydra : 0;
            Flags6 |= original.SummonKin ? MonsterFlag6.SummonKin : 0;
            Flags6 |= original.SummonMonster ? MonsterFlag6.SummonMonster : 0;
            Flags6 |= original.SummonMonsters ? MonsterFlag6.SummonMonsters : 0;
            Flags6 |= original.SummonReaver ? MonsterFlag6.SummonReaver : 0;
            Flags6 |= original.SummonSpider ? MonsterFlag6.SummonSpider : 0;
            Flags6 |= original.SummonUndead ? MonsterFlag6.SummonUndead : 0;
            Flags6 |= original.SummonUnique ? MonsterFlag6.SummonUnique : 0;
            Flags6 |= original.TeleportAway ? MonsterFlag6.TeleportAway : 0;
            Flags6 |= original.TeleportLevel ? MonsterFlag6.TeleportLevel : 0;
            Flags6 |= original.TeleportTo ? MonsterFlag6.TeleportTo : 0;
            Flags6 |= original.TeleportSelf ? MonsterFlag6.TeleportSelf : 0;
            Flags6 |= original.CreateTraps ? MonsterFlag6.CreateTraps : 0;
        }

        public int GetCoinType()
        {
            if (Character == '$')
            {
                if (Name.Contains(" copper "))
                {
                    return 2;
                }
                if (Name.Contains(" silver "))
                {
                    return 5;
                }
                if (Name.Contains(" gold "))
                {
                    return 10;
                }
                if (Name.Contains(" mithril "))
                {
                    return 16;
                }
                if (Name.Contains(" adamantite "))
                {
                    return 17;
                }
                if (Name.StartsWith("Copper "))
                {
                    return 2;
                }
                if (Name.StartsWith("Silver "))
                {
                    return 5;
                }
                if (Name.StartsWith("Gold "))
                {
                    return 10;
                }
                if (Name.StartsWith("Mithril "))
                {
                    return 16;
                }
                if (Name.StartsWith("Adamantite "))
                {
                    return 17;
                }
            }
            return 0;
        }

        public override string ToString()
        {
            return $"{Name} (lvl {Level})";
        }
    }
}