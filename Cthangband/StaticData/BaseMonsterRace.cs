// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Debug;
using Cthangband.Enumerations;
using System.ComponentModel;

namespace Cthangband.StaticData
{
    internal class BaseMonsterRace : EntityType
    {
        public BaseMonsterRace()
        {
            Attack1 = new MonsterAttack();
            Attack2 = new MonsterAttack();
            Attack3 = new MonsterAttack();
            Attack4 = new MonsterAttack();
        }

        [Browsable(true)]
        [Category("Spells")]
        [Description("The monster can cast acid balls")]
        [DefaultValue(false)]
        public bool AcidBall
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Spells")]
        [Description("The monster can cast acid bolts")]
        [DefaultValue(false)]
        public bool AcidBolt
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Type")]
        [Description("The monster is an animal")]
        [DefaultValue(false)]
        public bool Animal
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Defence")]
        [Description("The monster.s armour class")]
        public int ArmourClass
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Spells")]
        [Description("The monster shoots arrows for 1d6 damage")]
        [DefaultValue(false)]
        public bool Arrow1D6
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Spells")]
        [Description("The monster shoots arrows for 3d6 damage")]
        [DefaultValue(false)]
        public bool Arrow3D6
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Spells")]
        [Description("The monster shoots missiles for 5d6 damage")]
        [DefaultValue(false)]
        public bool Arrow5D6
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Spells")]
        [Description("The monster shoots missiles for 7d6 damage")]
        [DefaultValue(false)]
        public bool Arrow7D6
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("MeleeAttacks")]
        [Description("The number of times per round the monster attacks")]
        public MonsterAttack Attack1
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("MeleeAttacks")]
        [Description("The attack's number of damage dice")]
        public int Attack1DDice
        {
            get => Attack1.DDice;
            set => Attack1.DDice = value;
        }

        [Browsable(true)]
        [Category("MeleeAttacks")]
        [Description("The attack's type of damage dice")]
        public int Attack1DSides
        {
            get => Attack1.DSide;
            set => Attack1.DSide = value;
        }

        [Browsable(true)]
        [Category("MeleeAttacks")]
        [Description("The attack's effect")]
        public AttackEffect Attack1Effect
        {
            get => Attack1.Effect;
            set => Attack1.Effect = value;
        }

        [Browsable(true)]
        [Category("MeleeAttacks")]
        [Description("The attack's described method")]
        public AttackType Attack1Type
        {
            get => Attack1.Method;
            set => Attack1.Method = value;
        }

        [Browsable(true)]
        [Category("MeleeAttacks")]
        [Description("The number of times per round the monster attacks")]
        public MonsterAttack Attack2
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("MeleeAttacks")]
        [Description("The attack's number of damage dice")]
        public int Attack2DDice
        {
            get => Attack2.DDice;
            set => Attack2.DDice = value;
        }

        [Browsable(true)]
        [Category("MeleeAttacks")]
        [Description("The attack's type of damage dice")]
        public int Attack2DSides
        {
            get => Attack2.DSide;
            set => Attack2.DSide = value;
        }

        [Browsable(true)]
        [Category("MeleeAttacks")]
        [Description("The attack's effect")]
        public AttackEffect Attack2Effect
        {
            get => Attack2.Effect;
            set => Attack2.Effect = value;
        }

        [Browsable(true)]
        [Category("MeleeAttacks")]
        [Description("The attack's described method")]
        public AttackType Attack2Type
        {
            get => Attack2.Method;
            set => Attack2.Method = value;
        }

        [Browsable(true)]
        [Category("MeleeAttacks")]
        [Description("The number of times per round the monster attacks")]
        public MonsterAttack Attack3
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("MeleeAttacks")]
        [Description("The attack's number of damage dice")]
        public int Attack3DDice
        {
            get => Attack3.DDice;
            set => Attack3.DDice = value;
        }

        [Browsable(true)]
        [Category("MeleeAttacks")]
        [Description("The attack's type of damage dice")]
        public int Attack3DSides
        {
            get => Attack3.DSide;
            set => Attack3.DSide = value;
        }

        [Browsable(true)]
        [Category("MeleeAttacks")]
        [Description("The attack's effect")]
        public AttackEffect Attack3Effect
        {
            get => Attack3.Effect;
            set => Attack3.Effect = value;
        }

        [Browsable(true)]
        [Category("MeleeAttacks")]
        [Description("The attack's described method")]
        public AttackType Attack3Type
        {
            get => Attack3.Method;
            set => Attack3.Method = value;
        }

        [Browsable(true)]
        [Category("MeleeAttacks")]
        [Description("The number of times per round the monster attacks")]
        public MonsterAttack Attack4
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("MeleeAttacks")]
        [Description("The attack's number of damage dice")]
        public int Attack4DDice
        {
            get => Attack4.DDice;
            set => Attack4.DDice = value;
        }

        [Browsable(true)]
        [Category("MeleeAttacks")]
        [Description("The attack's type of damage dice")]
        public int Attack4DSides
        {
            get => Attack4.DSide;
            set => Attack4.DSide = value;
        }

        [Browsable(true)]
        [Category("MeleeAttacks")]
        [Description("The attack's effect")]
        public AttackEffect Attack4Effect
        {
            get => Attack4.Effect;
            set => Attack4.Effect = value;
        }

        [Browsable(true)]
        [Category("MeleeAttacks")]
        [Description("The attack's described method")]
        public AttackType Attack4Type
        {
            get => Attack4.Method;
            set => Attack4.Method = value;
        }

        [Browsable(true)]
        [Category("Visuals")]
        [Description("The monster's colour can be anything (if 'AttrMulti' is set)")]
        [DefaultValue(false)]
        public bool AttrAny
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Visuals")]
        [Description("The monster is transparent")]
        [DefaultValue(false)]
        public bool AttrClear
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Visuals")]
        [Description("The monster changes colour")]
        [DefaultValue(false)]
        public bool AttrMulti
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Movement")]
        [Description("The monster can break open doors")]
        [DefaultValue(false)]
        public bool BashDoor
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Spells")]
        [Description("The monster can cast blindness")]
        [DefaultValue(false)]
        public bool Blindness
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Spells")]
        [Description("The monster can cast blink")]
        [DefaultValue(false)]
        public bool Blink
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool BrainSmash
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool BreatheAcid
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool BreatheChaos
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool BreatheCold
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool BreatheConfusion
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool BreatheDark
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool BreatheDisenchant
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool BreatheDisintegration
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool BreatheFire
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool BreatheForce
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool BreatheGravity
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool BreatheInertia
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool BreatheLight
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool BreatheLightning
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool BreatheMana
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool BreatheNether
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool BreatheNexus
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool BreathePlasma
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool BreathePoison
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool BreatheRadiation
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool BreatheShards
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool BreatheSound
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool BreatheTime
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool CauseCriticalWounds
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool CauseLightWounds
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool CauseMortalWounds
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool CauseSeriousWounds
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Spells")]
        [Description("The monster can cast chaos balls")]
        [DefaultValue(false)]
        public bool ChaosBall
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Visuals")]
        [Description("The monster is never seen, even with see invisible")]
        [DefaultValue(false)]
        public bool CharClear
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Visuals")]
        [Description("The monster is changes shape randomly")]
        [DefaultValue(false)]
        public bool CharMulti
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Spells")]
        [Description("The monster can cast cold balls")]
        [DefaultValue(false)]
        public bool ColdBall
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool ColdBlood
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Spells")]
        [Description("The monster can cast cold bolts")]
        [DefaultValue(false)]
        public bool ColdBolt
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool Confuse
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool CreateTraps
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool Cthuloid
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Spells")]
        [Description("The monster can cast dark balls")]
        [DefaultValue(false)]
        public bool DarkBall
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool Darkness
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool Demon
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Visuals")]
        [Description("The descriptive text")]
        public string Description
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool Dragon
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool DrainMana
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool DreadCurse
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Treasure")]
        [Description("The monster drops 1d2 items")]
        [DefaultValue(false)]
        public bool Drop_1D2
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Treasure")]
        [Description("The monster drops 2d2 items")]
        [DefaultValue(false)]
        public bool Drop_2D2
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Treasure")]
        [Description("The monster drops 3d2 items")]
        [DefaultValue(false)]
        public bool Drop_3D2
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Treasure")]
        [Description("The monster drops 4d2 items")]
        [DefaultValue(false)]
        public bool Drop_4D2
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Treasure")]
        [Description("The monster drops an item 60% of the time")]
        [DefaultValue(false)]
        public bool Drop60
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Treasure")]
        [Description("The monster drops an item 90% of the time")]
        [DefaultValue(false)]
        public bool Drop90
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Treasure")]
        [Description("The monster drops good items")]
        [DefaultValue(false)]
        public bool DropGood
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Treasure")]
        [Description("The monster drops great items")]
        [DefaultValue(false)]
        public bool DropGreat
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool EldritchHorror
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool EmptyMind
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Basics")]
        [Description("The monster comes with minions of the same character")]
        [DefaultValue(false)]
        public bool Escorted
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Basics")]
        [Description("The monster's minions come in groups (this doesn't force minions if 'Escort' isn't set)")]
        [DefaultValue(false)]
        public bool EscortsGroup
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool Evil
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Basics")]
        [Description("The monster should use feminine pronouns")]
        [DefaultValue(false)]
        public bool Female
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Defence")]
        [Description("The monster has an aura of fire around it")]
        [DefaultValue(false)]
        public bool FireAura
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Spells")]
        [Description("The monster can cast fire balls")]
        [DefaultValue(false)]
        public bool FireBall
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Spells")]
        [Description("The monster can cast fire bolts")]
        [DefaultValue(false)]
        public bool FireBolt
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Defence")]
        [Description("The monster has maximum hit points")]
        [DefaultValue(false)]
        public bool ForceMaxHp
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Behaviour")]
        [Description("The monster always starts asleep")]
        [DefaultValue(false)]
        public bool ForceSleep
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool Forget
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Behaviour")]
        [Description("The 1-in-X frequency with which the monster uses special abilities")]
        public int FreqInate
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Behaviour")]
        [Description("The 1-in-X frequency with which the monster uses spells")]
        public int FreqSpell
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Basics")]
        [Description("The name that the game shows the player (may have duplicates)")]
        public string FriendlyName
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Basics")]
        [Description("The monster comes with friends of the same race")]
        [DefaultValue(false)]
        public bool Friends
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool Giant
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool Good
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool GreatOldOne
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool Haste
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Defence")]
        [Description("The number of hit dice the monster has")]
        public int Hdice
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool Heal
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Defence")]
        [Description("The number of hit points the monster has (click to update)")]
        public string HitPoints
        {
            get
            {
                if (ForceMaxHp)
                {
                    return $"{Hdice}d{Hside} (max. {Hdice * Hside})";
                }
                return $"{Hdice}d{Hside} (avg. {(Hdice * Hside / 2) + (Hdice / 2)})";
            }
        }

        [DefaultValue(false)]
        public bool Hold
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Defence")]
        [Description("The number of sides of the monster's hit dice")]
        public int Hside
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool HurtByCold
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool HurtByFire
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool HurtByLight
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool HurtByRock
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Spells")]
        [Description("The monster can cast ice bolts")]
        [DefaultValue(false)]
        public bool IceBolt
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool ImmuneAcid
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool ImmuneCold
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool ImmuneConfusion
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool ImmuneFear
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool ImmuneFire
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool ImmuneLightning
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool ImmunePoison
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool ImmuneSleep
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool ImmuneStun
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool Invisible
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool KillBody
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool KillItem
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool KillWall
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Basics")]
        [Description("The level on which the monster is normally found")]
        public int Level
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Defence")]
        [Description("The monster has an aura of electricity around it")]
        [DefaultValue(false)]
        public bool LightningAura
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Spells")]
        [Description("The monster can cast electricity balls")]
        [DefaultValue(false)]
        public bool LightningBall
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Spells")]
        [Description("The monster can cast lightning bolts")]
        [DefaultValue(false)]
        public bool LightningBolt
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool MagicMissile
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool Male
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Spells")]
        [Description("The monster can cast mana balls")]
        [DefaultValue(false)]
        public bool ManaBall
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Spells")]
        [Description("The monster can cast mana bolts")]
        [DefaultValue(false)]
        public bool ManaBolt
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Basics")]
        [Description("The experience value for killing one of these")]
        public int Mexp
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool MindBlast
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool MoveBody
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool Multiply
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Spells")]
        [Description("The monster can cast nether balls")]
        [DefaultValue(false)]
        public bool NetherBall
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Spells")]
        [Description("The monster can cast nether bolts")]
        [DefaultValue(false)]
        public bool NetherBolt
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool NeverAttack
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool NeverMove
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool Nonliving
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Behaviour")]
        [Description("The distance at which the monster notices the player")]
        public int NoticeRange
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool OnlyDropGold
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool OnlyDropItem
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool OpenDoor
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool Orc
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool PassWall
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Spells")]
        [Description("The monster can cast plasma bolts")]
        [DefaultValue(false)]
        public bool PlasmaBolt
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Spells")]
        [Description("The monster can cast poison balls")]
        [DefaultValue(false)]
        public bool PoisonBall
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Spells")]
        [Description("The monster can cast poison bolts")]
        [DefaultValue(false)]
        public bool PoisonBolt
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool Powerful
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Spells")]
        [Description("The monster can cast nuke balls")]
        [DefaultValue(false)]
        public bool RadiationBall
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool RandomMove25
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool RandomMove50
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Basics")]
        [Description("The rarity with which the monster is usually found")]
        public int Rarity
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool Reflecting
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool Regenerate
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool ResistDisenchant
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool ResistNether
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool ResistNexus
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool ResistPlasma
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool ResistTeleport
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool ResistWater
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool Scare
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool Shapechanger
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool ShardBall
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool Shriek
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Behaviour")]
        [Description("How deeply the monster sleeps")]
        public int Sleep
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool Slow
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool Smart
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Behaviour")]
        [Description("how fast the monster moves (110 = normal speed, higher is better)")]
        public int Speed
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Base")]
        [Description("The shortened name of the monster")]
        [DefaultValue(false)]
        public string SplitName1
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Base")]
        [Description("The shortened name of the monster")]
        [DefaultValue(false)]
        public string SplitName2
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Base")]
        [Description("The shortened name of the monster")]
        [DefaultValue(false)]
        public string SplitName3
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool Stupid
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool SummonAnt
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool SummonCthuloid
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool SummonDemon
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool SummonDragon
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool SummonGreatOldOne
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool SummonHiDragon
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool SummonHiUndead
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool SummonHound
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool SummonHydra
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool SummonKin
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool SummonMonster
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool SummonMonsters
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool SummonReaver
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool SummonSpider
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool SummonUndead
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool SummonUnique
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool TakeItem
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool TeleportAway
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool TeleportLevel
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool TeleportSelf
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool TeleportTo
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool Troll
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool Undead
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool Unique
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Spells")]
        [Description("The monster can cast water balls")]
        [DefaultValue(false)]
        public bool WaterBall
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Spells")]
        [Description("The monster can cast water bolts")]
        [DefaultValue(false)]
        public bool WaterBolt
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool WeirdMind
        {
            get;
            set;
        }
    }
}