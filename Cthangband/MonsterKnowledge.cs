// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.StaticData;
using Cthangband.UI;
using System;
using System.Text;

namespace Cthangband
{
    [Serializable]
    internal class MonsterKnowledge
    {
        public readonly int[] RBlows = new int[4];
        public int RCastInate;
        public int RCastSpell;
        public int RDeaths;
        public int RDropGold;
        public int RDropItem;
        public uint RFlags1;
        public uint RFlags2;
        public uint RFlags3;
        public uint RFlags4;
        public uint RFlags5;
        public uint RFlags6;
        public int RIgnore;
        public int RPkills;
        public bool RProbed;
        public int RSights;
        public int RTkills;
        public int RWake;
        private readonly MonsterRace _monsterType;
        private readonly string[] _wdHe = { "it", "he", "she" };
        private readonly string[] _wdHeCap = { "It", "He", "She" };
        private readonly string[] _wdHis = { "its", "his", "her" };
        private StringBuilder _description;

        public MonsterKnowledge(MonsterRace monsterType)
        {
            _monsterType = monsterType;
        }

        public void Display()
        {
            Profile.Instance.MsgPrint(null);
            Gui.Erase(1, 0, 255);
            DisplayBody(Colour.White);
            DisplayHeader();
        }

        public void DisplayBody(Colour bodyColour)
        {
            int m;
            int msex = 0;
            string[] vp = new string[64];
            MonsterKnowledge knowledge = this;
            _description = new StringBuilder();
            if (SaveGame.Instance.Player.IsWizard)
            {
                knowledge = new MonsterKnowledge(_monsterType);
                for (m = 0; m < 4; m++)
                {
                    if (_monsterType.Attack[m].Effect != 0 || _monsterType.Attack[m].Method != 0)
                    {
                        knowledge.RBlows[m] = Constants.MaxUchar;
                    }
                }
                knowledge.RProbed = true;
                knowledge.RWake = Constants.MaxUchar;
                knowledge.RIgnore = Constants.MaxUchar;
                knowledge.RDropItem = ((_monsterType.Flags1 & MonsterFlag1.Drop_4D2) != 0 ? 8 : 0) +
                                      ((_monsterType.Flags1 & MonsterFlag1.Drop_3D2) != 0 ? 6 : 0) +
                                      ((_monsterType.Flags1 & MonsterFlag1.Drop_2D2) != 0 ? 4 : 0) +
                                      ((_monsterType.Flags1 & MonsterFlag1.Drop_1D2) != 0 ? 2 : 0) +
                                      ((_monsterType.Flags1 & MonsterFlag1.Drop90) != 0 ? 1 : 0) +
                                      ((_monsterType.Flags1 & MonsterFlag1.Drop60) != 0 ? 1 : 0);
                knowledge.RDropGold = knowledge.RDropItem;
                if ((_monsterType.Flags1 & MonsterFlag1.OnlyDropGold) != 0)
                {
                    knowledge.RDropItem = 0;
                }
                if ((_monsterType.Flags1 & MonsterFlag1.OnlyDropItem) != 0)
                {
                    knowledge.RDropGold = 0;
                }
                knowledge.RCastInate = Constants.MaxUchar;
                knowledge.RCastSpell = Constants.MaxUchar;
                knowledge.RFlags1 = _monsterType.Flags1;
                knowledge.RFlags2 = _monsterType.Flags2;
                knowledge.RFlags3 = _monsterType.Flags3;
                knowledge.RFlags4 = _monsterType.Flags4;
                knowledge.RFlags5 = _monsterType.Flags5;
                knowledge.RFlags6 = _monsterType.Flags6;
            }
            if ((_monsterType.Flags1 & MonsterFlag1.Female) != 0)
            {
                msex = 2;
            }
            else if ((_monsterType.Flags1 & MonsterFlag1.Male) != 0)
            {
                msex = 1;
            }
            uint flags1 = _monsterType.Flags1 & knowledge.RFlags1;
            uint flags2 = _monsterType.Flags2 & knowledge.RFlags2;
            uint flags3 = _monsterType.Flags3 & knowledge.RFlags3;
            uint flags4 = _monsterType.Flags4 & knowledge.RFlags4;
            uint flags5 = _monsterType.Flags5 & knowledge.RFlags5;
            uint flags6 = _monsterType.Flags6 & knowledge.RFlags6;
            if ((_monsterType.Flags1 & MonsterFlag1.Unique) != 0)
            {
                flags1 |= MonsterFlag1.Unique;
            }
            if ((_monsterType.Flags1 & MonsterFlag1.Guardian) != 0)
            {
                flags1 |= MonsterFlag1.Guardian;
            }
            if ((_monsterType.Flags1 & MonsterFlag1.Male) != 0)
            {
                flags1 |= MonsterFlag1.Male;
            }
            if ((_monsterType.Flags1 & MonsterFlag1.Female) != 0)
            {
                flags1 |= MonsterFlag1.Female;
            }
            if ((_monsterType.Flags1 & MonsterFlag1.Friends) != 0)
            {
                flags1 |= MonsterFlag1.Friends;
            }
            if ((_monsterType.Flags1 & MonsterFlag1.Escorted) != 0)
            {
                flags1 |= MonsterFlag1.Escorted;
            }
            if ((_monsterType.Flags1 & MonsterFlag1.EscortsGroup) != 0)
            {
                flags1 |= MonsterFlag1.EscortsGroup;
            }
            if (knowledge.RTkills != 0 || knowledge.RProbed)
            {
                if ((_monsterType.Flags3 & MonsterFlag3.Orc) != 0)
                {
                    flags3 |= MonsterFlag3.Orc;
                }
                if ((_monsterType.Flags3 & MonsterFlag3.Troll) != 0)
                {
                    flags3 |= MonsterFlag3.Troll;
                }
                if ((_monsterType.Flags3 & MonsterFlag3.Giant) != 0)
                {
                    flags3 |= MonsterFlag3.Giant;
                }
                if ((_monsterType.Flags3 & MonsterFlag3.Dragon) != 0)
                {
                    flags3 |= MonsterFlag3.Dragon;
                }
                if ((_monsterType.Flags3 & MonsterFlag3.Demon) != 0)
                {
                    flags3 |= MonsterFlag3.Demon;
                }
                if ((_monsterType.Flags3 & MonsterFlag3.Cthuloid) != 0)
                {
                    flags3 |= MonsterFlag3.Cthuloid;
                }
                if ((_monsterType.Flags3 & MonsterFlag3.Undead) != 0)
                {
                    flags3 |= MonsterFlag3.Undead;
                }
                if ((_monsterType.Flags3 & MonsterFlag3.Evil) != 0)
                {
                    flags3 |= MonsterFlag3.Evil;
                }
                if ((_monsterType.Flags3 & MonsterFlag3.Good) != 0)
                {
                    flags3 |= MonsterFlag3.Good;
                }
                if ((_monsterType.Flags3 & MonsterFlag3.Animal) != 0)
                {
                    flags3 |= MonsterFlag3.Animal;
                }
                if ((_monsterType.Flags3 & MonsterFlag3.GreatOldOne) != 0)
                {
                    flags3 |= MonsterFlag3.GreatOldOne;
                }
                if ((_monsterType.Flags1 & MonsterFlag1.ForceMaxHp) != 0)
                {
                    flags1 |= MonsterFlag1.ForceMaxHp;
                }
            }
            string buf = _monsterType.Description;
            _description.Append(buf);
            _description.Append(" ");
            bool old = false;
            if (_monsterType.Level == 0)
            {
                _description.Append(_wdHeCap[msex]).Append(" lives in the town");
                old = true;
            }
            else if (knowledge.RTkills != 0 || knowledge.RProbed)
            {
                _description.Append(_wdHeCap[msex]).Append(" is normally found at level ").Append(_monsterType.Level);
                old = true;
            }
            if (old)
            {
                _description.Append(", and ");
            }
            else
            {
                _description.Append(_wdHeCap[msex]).Append(' ');
                old = true;
            }
            _description.Append("moves");
            if ((flags1 & MonsterFlag1.RandomMove50) != 0 || (flags1 & MonsterFlag1.RandomMove25) != 0)
            {
                if ((flags1 & MonsterFlag1.RandomMove50) != 0 && (flags1 & MonsterFlag1.RandomMove25) != 0)
                {
                    _description.Append(" extremely");
                }
                else if ((flags1 & MonsterFlag1.RandomMove50) != 0)
                {
                    _description.Append(" somewhat");
                }
                else if ((flags1 & MonsterFlag1.RandomMove25) != 0)
                {
                    _description.Append(" a bit");
                }
                _description.Append(" erratically");
                if (_monsterType.Speed != 110)
                {
                    _description.Append(", and");
                }
            }
            if (_monsterType.Speed > 110)
            {
                if (_monsterType.Speed > 130)
                {
                    _description.Append(" incredibly");
                }
                else if (_monsterType.Speed > 120)
                {
                    _description.Append(" very");
                }
                _description.Append(" quickly");
                _description.Append(" (").Append(GlobalData.ExtractEnergy[_monsterType.Speed] / 10.0).Append(" actions per turn)");
            }
            else if (_monsterType.Speed < 110)
            {
                if (_monsterType.Speed < 90)
                {
                    _description.Append(" incredibly");
                }
                else if (_monsterType.Speed < 100)
                {
                    _description.Append(" very");
                }
                _description.Append(" slowly");
                _description.Append(" (").Append(GlobalData.ExtractEnergy[_monsterType.Speed] / 10.0).Append(" actions per turn)");
            }
            else
            {
                _description.Append(" at normal speed");
            }
            if ((flags1 & MonsterFlag1.NeverMove) != 0)
            {
                if (old)
                {
                    _description.Append(", but ");
                }
                else
                {
                    _description.Append(_wdHe[msex]).Append(' ');
                    old = true;
                }
                _description.Append("does not deign to chase intruders");
            }
            if (old)
            {
                _description.Append(". ");
            }
            string q;
            string p;
            if (knowledge.RTkills != 0 || knowledge.RProbed)
            {
                _description.Append((flags1 & MonsterFlag1.Unique) != 0 ? "Killing this" : "A kill of this");
                if ((flags2 & MonsterFlag2.EldritchHorror) != 0)
                {
                    _description.Append(" sanity-blasting");
                }
                if ((flags3 & MonsterFlag3.Animal) != 0)
                {
                    _description.Append(" natural");
                }
                if ((flags3 & MonsterFlag3.Evil) != 0)
                {
                    _description.Append(" evil");
                }
                if ((flags3 & MonsterFlag3.Good) != 0)
                {
                    _description.Append(" good");
                }
                if ((flags3 & MonsterFlag3.Undead) != 0)
                {
                    _description.Append(" undead");
                }
                if ((flags3 & MonsterFlag3.Dragon) != 0)
                {
                    _description.Append(" dragon");
                }
                else if ((flags3 & MonsterFlag3.Demon) != 0)
                {
                    _description.Append(" demon");
                }
                else if ((flags3 & MonsterFlag3.Giant) != 0)
                {
                    _description.Append(" giant");
                }
                else if ((flags3 & MonsterFlag3.Troll) != 0)
                {
                    _description.Append(" troll");
                }
                else if ((flags3 & MonsterFlag3.Orc) != 0)
                {
                    _description.Append(" orc");
                }
                else if ((flags3 & MonsterFlag3.GreatOldOne) != 0)
                {
                    _description.Append(" Great Old One");
                }
                else
                {
                    _description.Append(" creature");
                }
                int i = _monsterType.Mexp * _monsterType.Level / SaveGame.Instance.Player.Level;
                int j = ((_monsterType.Mexp * _monsterType.Level % SaveGame.Instance.Player.Level * 1000 /
                         SaveGame.Instance.Player.Level) + 5) / 10;
                if (i > 0)
                {
                    _description.Append(" is worth ").AppendFormat("{0:n0}", i).Append("xp");
                }
                else if (j > 0)
                {
                    _description.Append(" is worth negligible xp");
                }
                else
                {
                    _description.Append(" is worth no xp");
                }
                p = "th";
                i = SaveGame.Instance.Player.Level % 10;
                if (SaveGame.Instance.Player.Level / 10 == 1)
                {
                }
                else if (i == 1)
                {
                    p = "st";
                }
                else if (i == 2)
                {
                    p = "nd";
                }
                else if (i == 3)
                {
                    p = "rd";
                }
                q = "";
                i = SaveGame.Instance.Player.Level;
                if (i == 8 || i == 11 || i == 18)
                {
                    q = "n";
                }
                _description.Append(" for a").Append(q).Append(' ').Append(i).Append(p).Append(" level character. ");
            }
            if ((flags2 & MonsterFlag2.FireAura) != 0 && (flags2 & MonsterFlag2.LightningAura) != 0)
            {
                _description.Append(_wdHeCap[msex]).Append(" is surrounded by flames and electricity. ");
            }
            else if ((flags2 & MonsterFlag2.FireAura) != 0)
            {
                _description.Append(_wdHeCap[msex]).Append(" is surrounded by flames. ");
            }
            else if ((flags2 & MonsterFlag2.LightningAura) != 0)
            {
                _description.Append(_wdHeCap[msex]).Append(" is surrounded by electricity. ");
            }
            if ((flags2 & MonsterFlag2.Reflecting) != 0)
            {
                _description.Append(_wdHeCap[msex]).Append(" reflects bolt spells. ");
            }
            if ((flags1 & MonsterFlag1.Escorted) != 0 || (flags1 & MonsterFlag1.EscortsGroup) != 0)
            {
                _description.Append(_wdHeCap[msex]).Append(" usually appears with escorts. ");
            }
            else if ((flags1 & MonsterFlag1.Friends) != 0)
            {
                _description.Append(_wdHeCap[msex]).Append(" usually appears in groups. ");
            }
            int vn = 0;
            if ((flags4 & MonsterFlag4.Shriek) != 0)
            {
                vp[vn++] = "shriek for help";
            }
            if ((flags4 & MonsterFlag4.Xxx2) != 0)
            {
                vp[vn++] = "do something";
            }
            if ((flags4 & MonsterFlag4.Xxx3) != 0)
            {
                vp[vn++] = "do something";
            }
            if ((flags4 & MonsterFlag4.ShardBall) != 0)
            {
                vp[vn++] = "produce shard balls";
            }
            if ((flags4 & MonsterFlag4.Arrow1D6) != 0)
            {
                vp[vn++] = "fire an arrow";
            }
            if ((flags4 & MonsterFlag4.Arrow3D6) != 0)
            {
                vp[vn++] = "fire arrows";
            }
            if ((flags4 & MonsterFlag4.Arrow5D6) != 0)
            {
                vp[vn++] = "fire a missile";
            }
            if ((flags4 & MonsterFlag4.Arrow7D6) != 0)
            {
                vp[vn++] = "fire missiles";
            }
            int n;
            if (vn != 0)
            {
                _description.Append(_wdHeCap[msex]);
                for (n = 0; n < vn; n++)
                {
                    if (n == 0)
                    {
                        _description.Append(" may ");
                    }
                    else if (n < vn - 1)
                    {
                        _description.Append(", ");
                    }
                    else
                    {
                        _description.Append(" or ");
                    }
                    _description.Append(vp[n]);
                }
                _description.Append(". ");
            }
            vn = 0;
            if ((flags4 & MonsterFlag4.BreatheAcid) != 0)
            {
                vp[vn++] = "acid";
            }
            if ((flags4 & MonsterFlag4.BreatheLightning) != 0)
            {
                vp[vn++] = "lightning";
            }
            if ((flags4 & MonsterFlag4.BreatheFire) != 0)
            {
                vp[vn++] = "fire";
            }
            if ((flags4 & MonsterFlag4.BreatheCold) != 0)
            {
                vp[vn++] = "frost";
            }
            if ((flags4 & MonsterFlag4.BreathePoison) != 0)
            {
                vp[vn++] = "poison";
            }
            if ((flags4 & MonsterFlag4.BreatheNether) != 0)
            {
                vp[vn++] = "nether";
            }
            if ((flags4 & MonsterFlag4.BreatheLight) != 0)
            {
                vp[vn++] = "light";
            }
            if ((flags4 & MonsterFlag4.BreatheDark) != 0)
            {
                vp[vn++] = "darkness";
            }
            if ((flags4 & MonsterFlag4.BreatheConfusion) != 0)
            {
                vp[vn++] = "confusion";
            }
            if ((flags4 & MonsterFlag4.BreatheSound) != 0)
            {
                vp[vn++] = "sound";
            }
            if ((flags4 & MonsterFlag4.BreatheChaos) != 0)
            {
                vp[vn++] = "chaos";
            }
            if ((flags4 & MonsterFlag4.BreatheDisenchant) != 0)
            {
                vp[vn++] = "disenchantment";
            }
            if ((flags4 & MonsterFlag4.BreatheNexus) != 0)
            {
                vp[vn++] = "nexus";
            }
            if ((flags4 & MonsterFlag4.BreatheTime) != 0)
            {
                vp[vn++] = "time";
            }
            if ((flags4 & MonsterFlag4.BreatheInertia) != 0)
            {
                vp[vn++] = "inertia";
            }
            if ((flags4 & MonsterFlag4.BreatheGravity) != 0)
            {
                vp[vn++] = "gravity";
            }
            if ((flags4 & MonsterFlag4.BreatheShards) != 0)
            {
                vp[vn++] = "shards";
            }
            if ((flags4 & MonsterFlag4.BreathePlasma) != 0)
            {
                vp[vn++] = "plasma";
            }
            if ((flags4 & MonsterFlag4.BreatheForce) != 0)
            {
                vp[vn++] = "force";
            }
            if ((flags4 & MonsterFlag4.BreatheVis) != 0)
            {
                vp[vn++] = "vis";
            }
            if ((flags4 & MonsterFlag4.BreatheRadiation) != 0)
            {
                vp[vn++] = "toxic waste";
            }
            if ((flags4 & MonsterFlag4.BreatheDisintegration) != 0)
            {
                vp[vn++] = "disintegration";
            }
            bool breath = false;
            if (vn != 0)
            {
                breath = true;
                _description.Append(_wdHeCap[msex]);
                for (n = 0; n < vn; n++)
                {
                    if (n == 0)
                    {
                        _description.Append(" may breathe ");
                    }
                    else if (n < vn - 1)
                    {
                        _description.Append(", ");
                    }
                    else
                    {
                        _description.Append(" or ");
                    }
                    _description.Append(vp[n]);
                }
            }
            vn = 0;
            if ((flags5 & MonsterFlag5.AcidBall) != 0)
            {
                vp[vn++] = "produce acid balls";
            }
            if ((flags5 & MonsterFlag5.LightningBall) != 0)
            {
                vp[vn++] = "produce lightning balls";
            }
            if ((flags5 & MonsterFlag5.FireBall) != 0)
            {
                vp[vn++] = "produce fire balls";
            }
            if ((flags5 & MonsterFlag5.ColdBall) != 0)
            {
                vp[vn++] = "produce frost balls";
            }
            if ((flags5 & MonsterFlag5.PoisonBall) != 0)
            {
                vp[vn++] = "produce poison balls";
            }
            if ((flags5 & MonsterFlag5.NetherBall) != 0)
            {
                vp[vn++] = "produce nether balls";
            }
            if ((flags5 & MonsterFlag5.WaterBall) != 0)
            {
                vp[vn++] = "produce water balls";
            }
            if ((flags4 & MonsterFlag4.RadiationBall) != 0)
            {
                vp[vn++] = "produce balls of radiation";
            }
            if ((flags5 & MonsterFlag5.VisBall) != 0)
            {
                vp[vn++] = "invoke vis storms";
            }
            if ((flags5 & MonsterFlag5.DarkBall) != 0)
            {
                vp[vn++] = "invoke darkness storms";
            }
            if ((flags4 & MonsterFlag4.ChaosBall) != 0)
            {
                vp[vn++] = "invoke raw chaos";
            }
            if ((flags6 & MonsterFlag6.DreadCurse) != 0)
            {
                vp[vn++] = "invoke the Dread Curse of Azathoth";
            }
            if ((flags5 & MonsterFlag5.DrainVis) != 0)
            {
                vp[vn++] = "drain vis";
            }
            if ((flags5 & MonsterFlag5.MindBlast) != 0)
            {
                vp[vn++] = "cause mind blasting";
            }
            if ((flags5 & MonsterFlag5.BrainSmash) != 0)
            {
                vp[vn++] = "cause brain smashing";
            }
            if ((flags5 & MonsterFlag5.CauseLightWounds) != 0)
            {
                vp[vn++] = "cause light wounds and cursing";
            }
            if ((flags5 & MonsterFlag5.CauseSeriousWounds) != 0)
            {
                vp[vn++] = "cause serious wounds and cursing";
            }
            if ((flags5 & MonsterFlag5.CauseCriticalWounds) != 0)
            {
                vp[vn++] = "cause critical wounds and cursing";
            }
            if ((flags5 & MonsterFlag5.CauseMortalWounds) != 0)
            {
                vp[vn++] = "cause mortal wounds";
            }
            if ((flags5 & MonsterFlag5.AcidBolt) != 0)
            {
                vp[vn++] = "produce acid bolts";
            }
            if ((flags5 & MonsterFlag5.LightningBolt) != 0)
            {
                vp[vn++] = "produce lightning bolts";
            }
            if ((flags5 & MonsterFlag5.FireBolt) != 0)
            {
                vp[vn++] = "produce fire bolts";
            }
            if ((flags5 & MonsterFlag5.ColdBolt) != 0)
            {
                vp[vn++] = "produce frost bolts";
            }
            if ((flags5 & MonsterFlag5.PoisonBolt) != 0)
            {
                vp[vn++] = "produce poison bolts";
            }
            if ((flags5 & MonsterFlag5.NetherBolt) != 0)
            {
                vp[vn++] = "produce nether bolts";
            }
            if ((flags5 & MonsterFlag5.WaterBolt) != 0)
            {
                vp[vn++] = "produce water bolts";
            }
            if ((flags5 & MonsterFlag5.VisBolt) != 0)
            {
                vp[vn++] = "produce vis bolts";
            }
            if ((flags5 & MonsterFlag5.PlasmaBolt) != 0)
            {
                vp[vn++] = "produce plasma bolts";
            }
            if ((flags5 & MonsterFlag5.IceBolt) != 0)
            {
                vp[vn++] = "produce ice bolts";
            }
            if ((flags5 & MonsterFlag5.MagicMissile) != 0)
            {
                vp[vn++] = "produce magic missiles";
            }
            if ((flags5 & MonsterFlag5.Scare) != 0)
            {
                vp[vn++] = "terrify";
            }
            if ((flags5 & MonsterFlag5.Blindness) != 0)
            {
                vp[vn++] = "blind";
            }
            if ((flags5 & MonsterFlag5.Confuse) != 0)
            {
                vp[vn++] = "confuse";
            }
            if ((flags5 & MonsterFlag5.Slow) != 0)
            {
                vp[vn++] = "slow";
            }
            if ((flags5 & MonsterFlag5.Hold) != 0)
            {
                vp[vn++] = "paralyze";
            }
            if ((flags6 & MonsterFlag6.Haste) != 0)
            {
                vp[vn++] = "haste-self";
            }
            if ((flags6 & MonsterFlag6.Heal) != 0)
            {
                vp[vn++] = "heal-self";
            }
            if ((flags6 & MonsterFlag6.Xxx2) != 0)
            {
                vp[vn++] = "do something";
            }
            if ((flags6 & MonsterFlag6.Blink) != 0)
            {
                vp[vn++] = "blink-self";
            }
            if ((flags6 & MonsterFlag6.TeleportSelf) != 0)
            {
                vp[vn++] = "teleport-self";
            }
            if ((flags6 & MonsterFlag6.Xxx3) != 0)
            {
                vp[vn++] = "do something";
            }
            if ((flags6 & MonsterFlag6.Xxx4) != 0)
            {
                vp[vn++] = "do something";
            }
            if ((flags6 & MonsterFlag6.TeleportTo) != 0)
            {
                vp[vn++] = "teleport to";
            }
            if ((flags6 & MonsterFlag6.TeleportAway) != 0)
            {
                vp[vn++] = "teleport away";
            }
            if ((flags6 & MonsterFlag6.TeleportLevel) != 0)
            {
                vp[vn++] = "teleport level";
            }
            if ((flags6 & MonsterFlag6.Xxx5) != 0)
            {
                vp[vn++] = "do something";
            }
            if ((flags6 & MonsterFlag6.Darkness) != 0)
            {
                vp[vn++] = "create darkness";
            }
            if ((flags6 & MonsterFlag6.CreateTraps) != 0)
            {
                vp[vn++] = "create traps";
            }
            if ((flags6 & MonsterFlag6.Forget) != 0)
            {
                vp[vn++] = "cause amnesia";
            }
            if ((flags6 & MonsterFlag6.Xxx6) != 0)
            {
                vp[vn++] = "do something";
            }
            if ((flags6 & MonsterFlag6.SummonMonster) != 0)
            {
                vp[vn++] = "summon a monster";
            }
            if ((flags6 & MonsterFlag6.SummonMonsters) != 0)
            {
                vp[vn++] = "summon monsters";
            }
            if ((flags6 & MonsterFlag6.SummonKin) != 0)
            {
                vp[vn++] = "summon aid";
            }
            if ((flags6 & MonsterFlag6.SummonAnt) != 0)
            {
                vp[vn++] = "summon ants";
            }
            if ((flags6 & MonsterFlag6.SummonSpider) != 0)
            {
                vp[vn++] = "summon spiders";
            }
            if ((flags6 & MonsterFlag6.SummonHound) != 0)
            {
                vp[vn++] = "summon hounds";
            }
            if ((flags6 & MonsterFlag6.SummonHydra) != 0)
            {
                vp[vn++] = "summon hydras";
            }
            if ((flags6 & MonsterFlag6.SummonCthuloid) != 0)
            {
                vp[vn++] = "summon a Cthuloid entity";
            }
            if ((flags6 & MonsterFlag6.SummonDemon) != 0)
            {
                vp[vn++] = "summon a demon";
            }
            if ((flags6 & MonsterFlag6.SummonUndead) != 0)
            {
                vp[vn++] = "summon an undead";
            }
            if ((flags6 & MonsterFlag6.SummonDragon) != 0)
            {
                vp[vn++] = "summon a dragon";
            }
            if ((flags6 & MonsterFlag6.SummonHiUndead) != 0)
            {
                vp[vn++] = "summon Greater Undead";
            }
            if ((flags6 & MonsterFlag6.SummonHiDragon) != 0)
            {
                vp[vn++] = "summon Ancient Dragons";
            }
            if ((flags6 & MonsterFlag6.SummonReaver) != 0)
            {
                vp[vn++] = "summon Black Reavers";
            }
            if ((flags6 & MonsterFlag6.SummonGreatOldOne) != 0)
            {
                vp[vn++] = "summon Great Old Ones";
            }
            if ((flags6 & MonsterFlag6.SummonUnique) != 0)
            {
                vp[vn++] = "summon Unique Monsters";
            }
            bool magic = false;
            if (vn != 0)
            {
                magic = true;
                _description.Append(breath ? ", and is also" : $"{_wdHeCap[msex]} is");
                _description.Append(" magical, casting spells");
                if ((flags2 & MonsterFlag2.Smart) != 0)
                {
                    _description.Append(" intelligently");
                }
                for (n = 0; n < vn; n++)
                {
                    if (n == 0)
                    {
                        _description.Append(" which ");
                    }
                    else if (n < vn - 1)
                    {
                        _description.Append(", ");
                    }
                    else
                    {
                        _description.Append(" or ");
                    }
                    _description.Append(vp[n]);
                }
            }
            if (breath || magic)
            {
                m = knowledge.RCastInate + knowledge.RCastSpell;
                n = (_monsterType.FreqInate + _monsterType.FreqSpell) / 2;
                if (m > 100)
                {
                    _description.Append("; 1 time in ").Append(100 / n);
                }
                else if (m != 0)
                {
                    n = (n + 9) / 10 * 10;
                    _description.Append("; about 1 time in ").Append(100 / n);
                }
                _description.Append(". ");
            }
            if (KnowArmour(_monsterType, knowledge))
            {
                _description.Append(_wdHeCap[msex]).Append(" is AC ").Append(_monsterType.ArmourClass);
                if (_monsterType.Hdice == 1 && _monsterType.Hside == 1)
                {
                    _description.Append(" and has 1hp. ");
                }
                else
                {
                    _description.Append((flags1 & MonsterFlag1.ForceMaxHp) != 0
                        ? $" and has {_monsterType.Hdice * _monsterType.Hside:n0}hp. "
                        : $" and has {_monsterType.Hdice}d{_monsterType.Hside}hp. ");
                }
            }
            vn = 0;
            if ((flags2 & MonsterFlag2.OpenDoor) != 0)
            {
                vp[vn++] = "open doors";
            }
            if ((flags2 & MonsterFlag2.BashDoor) != 0)
            {
                vp[vn++] = "bash down doors";
            }
            if ((flags2 & MonsterFlag2.PassWall) != 0)
            {
                vp[vn++] = "pass through walls";
            }
            if ((flags2 & MonsterFlag2.KillWall) != 0)
            {
                vp[vn++] = "bore through walls";
            }
            if ((flags2 & MonsterFlag2.MoveBody) != 0)
            {
                vp[vn++] = "push past weaker monsters";
            }
            if ((flags2 & MonsterFlag2.KillBody) != 0)
            {
                vp[vn++] = "destroy weaker monsters";
            }
            if ((flags2 & MonsterFlag2.TakeItem) != 0)
            {
                vp[vn++] = "pick up objects";
            }
            if ((flags2 & MonsterFlag2.KillItem) != 0)
            {
                vp[vn++] = "destroy objects";
            }
            if (vn != 0)
            {
                _description.Append(_wdHeCap[msex]);
                for (n = 0; n < vn; n++)
                {
                    if (n == 0)
                    {
                        _description.Append(" can ");
                    }
                    else if (n < vn - 1)
                    {
                        _description.Append(", ");
                    }
                    else
                    {
                        _description.Append(" and ");
                    }
                    _description.Append(vp[n]);
                }
                _description.Append(". ");
            }
            if ((flags2 & MonsterFlag2.Invisible) != 0)
            {
                _description.Append(_wdHeCap[msex]).Append(" is invisible. ");
            }
            if ((flags2 & MonsterFlag2.ColdBlood) != 0)
            {
                _description.Append(_wdHeCap[msex]).Append(" is cold blooded. ");
            }
            if ((flags2 & MonsterFlag2.EmptyMind) != 0)
            {
                _description.Append(_wdHeCap[msex]).Append(" is not detected by telepathy. ");
            }
            if ((flags2 & MonsterFlag2.WeirdMind) != 0)
            {
                _description.Append(_wdHeCap[msex]).Append(" is rarely detected by telepathy. ");
            }
            if ((flags2 & MonsterFlag2.Multiply) != 0)
            {
                _description.Append(_wdHeCap[msex]).Append(" breeds explosively. ");
            }
            if ((flags2 & MonsterFlag2.Regenerate) != 0)
            {
                _description.Append(_wdHeCap[msex]).Append(" regenerates quickly. ");
            }
            vn = 0;
            if ((flags3 & MonsterFlag3.HurtByRock) != 0)
            {
                vp[vn++] = "rock remover";
            }
            if ((flags3 & MonsterFlag3.HurtByLight) != 0)
            {
                vp[vn++] = "bright light";
            }
            if ((flags3 & MonsterFlag3.HurtByFire) != 0)
            {
                vp[vn++] = "fire";
            }
            if ((flags3 & MonsterFlag3.HurtByCold) != 0)
            {
                vp[vn++] = "cold";
            }
            if (vn != 0)
            {
                _description.Append(_wdHeCap[msex]);
                for (n = 0; n < vn; n++)
                {
                    if (n == 0)
                    {
                        _description.Append(" is hurt by ");
                    }
                    else if (n < vn - 1)
                    {
                        _description.Append(", ");
                    }
                    else
                    {
                        _description.Append(" and ");
                    }
                    _description.Append(vp[n]);
                }
                _description.Append(". ");
            }
            vn = 0;
            if ((flags3 & MonsterFlag3.ImmuneAcid) != 0)
            {
                vp[vn++] = "acid";
            }
            if ((flags3 & MonsterFlag3.ImmuneLightning) != 0)
            {
                vp[vn++] = "lightning";
            }
            if ((flags3 & MonsterFlag3.ImmuneFire) != 0)
            {
                vp[vn++] = "fire";
            }
            if ((flags3 & MonsterFlag3.ImmuneCold) != 0)
            {
                vp[vn++] = "cold";
            }
            if ((flags3 & MonsterFlag3.ImmunePoison) != 0)
            {
                vp[vn++] = "poison";
            }
            if (vn != 0)
            {
                _description.Append(_wdHeCap[msex]);
                for (n = 0; n < vn; n++)
                {
                    if (n == 0)
                    {
                        _description.Append(" resists ");
                    }
                    else if (n < vn - 1)
                    {
                        _description.Append(", ");
                    }
                    else
                    {
                        _description.Append(" and ");
                    }
                    _description.Append(vp[n]);
                }
                _description.Append(". ");
            }
            vn = 0;
            if ((flags3 & MonsterFlag3.ResistNether) != 0)
            {
                vp[vn++] = "nether";
            }
            if ((flags3 & MonsterFlag3.ResistWater) != 0)
            {
                vp[vn++] = "water";
            }
            if ((flags3 & MonsterFlag3.ResistPlasma) != 0)
            {
                vp[vn++] = "plasma";
            }
            if ((flags3 & MonsterFlag3.ResistNexus) != 0)
            {
                vp[vn++] = "nexus";
            }
            if ((flags3 & MonsterFlag3.ResistDisenchant) != 0)
            {
                vp[vn++] = "disenchantment";
            }
            if ((flags3 & MonsterFlag3.ResistTeleport) != 0)
            {
                vp[vn++] = "teleportation";
            }
            if (vn != 0)
            {
                _description.Append(_wdHeCap[msex]);
                for (n = 0; n < vn; n++)
                {
                    if (n == 0)
                    {
                        _description.Append(" resists ");
                    }
                    else if (n < vn - 1)
                    {
                        _description.Append(", ");
                    }
                    else
                    {
                        _description.Append(" and ");
                    }
                    _description.Append(vp[n]);
                }
                _description.Append(". ");
            }
            vn = 0;
            if ((flags3 & MonsterFlag3.ImmuneStun) != 0)
            {
                vp[vn++] = "stunned";
            }
            if ((flags3 & MonsterFlag3.ImmuneFear) != 0)
            {
                vp[vn++] = "frightened";
            }
            if ((flags3 & MonsterFlag3.ImmuneConfusion) != 0)
            {
                vp[vn++] = "confused";
            }
            if ((flags3 & MonsterFlag3.ImmuneSleep) != 0)
            {
                vp[vn++] = "slept";
            }
            if (vn != 0)
            {
                _description.Append(_wdHeCap[msex]);
                for (n = 0; n < vn; n++)
                {
                    if (n == 0)
                    {
                        _description.Append(" cannot be ");
                    }
                    else if (n < vn - 1)
                    {
                        _description.Append(", ");
                    }
                    else
                    {
                        _description.Append(" or ");
                    }
                    _description.Append(vp[n]);
                }
                _description.Append(". ");
            }
            if (knowledge.RWake * knowledge.RWake > _monsterType.Sleep || knowledge.RIgnore == Constants.MaxUchar ||
                (_monsterType.Sleep == 0 && (knowledge.RTkills >= 10 || knowledge.RProbed)))
            {
                string act;
                if (_monsterType.Sleep > 200)
                {
                    act = "prefers to ignore";
                }
                else if (_monsterType.Sleep > 95)
                {
                    act = "pays very little attention to";
                }
                else if (_monsterType.Sleep > 75)
                {
                    act = "pays little attention to";
                }
                else if (_monsterType.Sleep > 45)
                {
                    act = "tends to overlook";
                }
                else if (_monsterType.Sleep > 25)
                {
                    act = "takes quite a while to see";
                }
                else if (_monsterType.Sleep > 10)
                {
                    act = "takes a while to see";
                }
                else if (_monsterType.Sleep > 5)
                {
                    act = "is fairly observant of";
                }
                else if (_monsterType.Sleep > 3)
                {
                    act = "is observant of";
                }
                else if (_monsterType.Sleep > 1)
                {
                    act = "is very observant of";
                }
                else if (_monsterType.Sleep > 0)
                {
                    act = "is vigilant for";
                }
                else
                {
                    act = "is ever vigilant for";
                }
                _description.Append(
                    _wdHeCap[msex]).Append(' ').Append(act).Append(" intruders, which ").Append(_wdHe[msex]).Append(" may notice from ").AppendFormat("{0:n0}", 10 * _monsterType.NoticeRange).Append(" feet. ");
            }
            if (knowledge.RDropGold != 0 || knowledge.RDropItem != 0)
            {
                bool sin = false;
                _description.Append(_wdHeCap[msex]).Append(" may carry");
                n = Math.Max(knowledge.RDropGold, knowledge.RDropItem);
                if (n == 1)
                {
                    _description.Append(" a");
                    sin = true;
                }
                else if (n == 2)
                {
                    _description.Append(" one or two");
                }
                else
                {
                    _description.Append(" up to ").Append(n);
                }
                if ((flags1 & MonsterFlag1.DropGreat) != 0)
                {
                    p = " exceptional";
                }
                else if ((flags1 & MonsterFlag1.DropGood) != 0)
                {
                    p = " good";
                    sin = false;
                }
                else
                {
                    p = null;
                }
                if (knowledge.RDropItem != 0)
                {
                    if (sin)
                    {
                        _description.Append("n");
                    }
                    sin = false;
                    if (!string.IsNullOrEmpty(p))
                    {
                        _description.Append(p);
                    }
                    _description.Append(" object");
                    if (n != 1)
                    {
                        _description.Append("s");
                    }
                    p = " or";
                }
                if (knowledge.RDropGold != 0)
                {
                    if (string.IsNullOrEmpty(p))
                    {
                        sin = false;
                    }
                    if (sin)
                    {
                        _description.Append("n");
                    }
                    if (!string.IsNullOrEmpty(p))
                    {
                        _description.Append(p);
                    }
                    _description.Append(" treasure");
                    if (n != 1)
                    {
                        _description.Append("s");
                    }
                }
                _description.Append(". ");
            }
            for (n = 0, m = 0; m < 4; m++)
            {
                if (_monsterType.Attack[m].Method == 0)
                {
                    continue;
                }
                if (knowledge.RBlows[m] != 0)
                {
                    n++;
                }
            }
            int r;
            for (r = 0, m = 0; m < 4; m++)
            {
                if (_monsterType.Attack[m].Method == 0)
                {
                    continue;
                }
                if (knowledge.RBlows[m] == 0)
                {
                    continue;
                }
                AttackType method = _monsterType.Attack[m].Method;
                AttackEffect effect = _monsterType.Attack[m].Effect;
                int d1 = _monsterType.Attack[m].DDice;
                int d2 = _monsterType.Attack[m].DSide;
                p = null;
                switch (method)
                {
                    case AttackType.Hit:
                        p = "hit";
                        break;

                    case AttackType.Touch:
                        p = "touch";
                        break;

                    case AttackType.Punch:
                        p = "punch";
                        break;

                    case AttackType.Kick:
                        p = "kick";
                        break;

                    case AttackType.Claw:
                        p = "claw";
                        break;

                    case AttackType.Bite:
                        p = "bite";
                        break;

                    case AttackType.Sting:
                        p = "sting";
                        break;

                    case AttackType.Butt:
                        p = "butt";
                        break;

                    case AttackType.Crush:
                        p = "crush";
                        break;

                    case AttackType.Engulf:
                        p = "engulf";
                        break;

                    case AttackType.Charge:
                        p = "charge";
                        break;

                    case AttackType.Crawl:
                        p = "crawl on you";
                        break;

                    case AttackType.Drool:
                        p = "drool on you";
                        break;

                    case AttackType.Spit:
                        p = "spit";
                        break;

                    case AttackType.Gaze:
                        p = "gaze";
                        break;

                    case AttackType.Wail:
                        p = "wail";
                        break;

                    case AttackType.Spore:
                        p = "release spores";
                        break;

                    case AttackType.Worship:
                        p = "hero worship";
                        break;

                    case AttackType.Beg:
                        p = "beg";
                        break;

                    case AttackType.Insult:
                        p = "insult";
                        break;

                    case AttackType.Moan:
                        p = "moan";
                        break;

                    case AttackType.Show:
                        p = "sing";
                        break;
                }
                q = null;
                switch (effect)
                {
                    case AttackEffect.Hurt:
                        q = "attack";
                        break;

                    case AttackEffect.Poison:
                        q = "poison";
                        break;

                    case AttackEffect.UnBonus:
                        q = "disenchant";
                        break;

                    case AttackEffect.UnPower:
                        q = "drain charges";
                        break;

                    case AttackEffect.EatGold:
                        q = "steal gold";
                        break;

                    case AttackEffect.EatItem:
                        q = "steal items";
                        break;

                    case AttackEffect.EatFood:
                        q = "eat your food";
                        break;

                    case AttackEffect.EatLight:
                        q = "absorb light";
                        break;

                    case AttackEffect.Acid:
                        q = "shoot acid";
                        break;

                    case AttackEffect.Electricity:
                        q = "electrocute";
                        break;

                    case AttackEffect.Fire:
                        q = "burn";
                        break;

                    case AttackEffect.Cold:
                        q = "freeze";
                        break;

                    case AttackEffect.Blind:
                        q = "blind";
                        break;

                    case AttackEffect.Confuse:
                        q = "confuse";
                        break;

                    case AttackEffect.Terrify:
                        q = "terrify";
                        break;

                    case AttackEffect.Paralyze:
                        q = "paralyze";
                        break;

                    case AttackEffect.LoseStr:
                        q = "reduce strength";
                        break;

                    case AttackEffect.LoseInt:
                        q = "reduce intelligence";
                        break;

                    case AttackEffect.LoseWis:
                        q = "reduce wisdom";
                        break;

                    case AttackEffect.LoseDex:
                        q = "reduce dexterity";
                        break;

                    case AttackEffect.LoseCon:
                        q = "reduce constitution";
                        break;

                    case AttackEffect.LoseCha:
                        q = "reduce charisma";
                        break;

                    case AttackEffect.LoseAll:
                        q = "reduce all stats";
                        break;

                    case AttackEffect.Shatter:
                        q = "shatter";
                        break;

                    case AttackEffect.Exp10:
                        q = "lower experience (by 10d6+)";
                        break;

                    case AttackEffect.Exp20:
                        q = "lower experience (by 20d6+)";
                        break;

                    case AttackEffect.Exp40:
                        q = "lower experience (by 40d6+)";
                        break;

                    case AttackEffect.Exp80:
                        q = "lower experience (by 80d6+)";
                        break;
                }
                if (r == 0)
                {
                    _description.Append(_wdHeCap[msex]).Append(" can ");
                }
                else if (r < n - 1)
                {
                    _description.Append(", ");
                }
                else
                {
                    _description.Append(", and ");
                }
                if (string.IsNullOrEmpty(p))
                {
                    p = "do something weird";
                }
                _description.Append(p);
                if (!string.IsNullOrEmpty(q))
                {
                    _description.Append(" to ");
                    _description.Append(q);
                    if (d1 != 0 && d2 != 0 && KnowDamage(_monsterType, knowledge, m))
                    {
                        _description.Append(" for ").Append(d1).Append('d').Append(d2).Append(" damage");
                    }
                }
                r++;
            }
            if (r != 0)
            {
                _description.Append(". ");
            }
            else if ((flags1 & MonsterFlag1.NeverAttack) != 0)
            {
                _description.Append(_wdHeCap[msex]).Append(" has no physical attacks. ");
            }
            else
            {
                _description.Append("Nothing is known about ").Append(_wdHis[msex]).Append(" attack. ");
            }
            if ((flags1 & MonsterFlag1.Unique) != 0)
            {
                bool dead = _monsterType.MaxNum == 0;
                if (knowledge.RDeaths != 0)
                {
                    _description.Append(_wdHe[msex]).Append(" has slain ").AppendFormat("{0:n0}", knowledge.RDeaths).Append(" of your ancestors");
                    if (dead)
                    {
                        _description.Append(", but you have avenged them! ");
                    }
                    else
                    {
                        string remain = knowledge.RDeaths == 1 ? "remains" : "remain";
                        _description.Append(", who ").Append(remain).Append(" unavenged. ");
                    }
                }
                else if (dead)
                {
                    _description.Append("You have slain this foe. ");
                }
            }
            else if (knowledge.RDeaths != 0)
            {
                string has = knowledge.RDeaths == 1 ? "has" : "have";
                _description.AppendFormat("{0:n0}", knowledge.RDeaths).Append(" of your ancestors ").Append(has).Append(" been killed by this creature, ");
                if (knowledge.RPkills != 0)
                {
                    _description.Append(
                        "and you have exterminated at least ").AppendFormat("{0:n0}", knowledge.RPkills).Append(" of the creatures. ");
                }
                else if (knowledge.RTkills != 0)
                {
                    _description.Append(
                        "and your ancestors have exterminated at least ").AppendFormat("{0:n0}", knowledge.RTkills).Append(" of the creatures. ");
                }
                else
                {
                    _description.Append("and ").Append(_wdHe[msex]).Append(" is not ever known to have been defeated. ");
                }
            }
            else
            {
                if (knowledge.RPkills != 0)
                {
                    _description.Append("You have killed at least ").AppendFormat("{0:n0}", knowledge.RPkills).Append(" of these creatures. ");
                }
                else if (knowledge.RTkills != 0)
                {
                    _description.Append(
                        "Your ancestors have killed at least ").AppendFormat("{0:n0}", knowledge.RTkills).Append(" of these creatures. ");
                }
                else
                {
                    _description.Append("No battles to the death are recalled. ");
                }
            }
            if ((flags1 & MonsterFlag1.Guardian) != 0)
            {
                _description.Append("You feel an intense desire to kill this monster... ");
            }
            Gui.PrintWrap(bodyColour, _description.ToString());
        }

        private void DisplayHeader()
        {
            char c1 = _monsterType.Character;
            Colour a1 = _monsterType.Colour;
            Gui.Erase(0, 0, 255);
            Gui.Goto(0, 0);
            if ((_monsterType.Flags1 & MonsterFlag1.Unique) == 0)
            {
                Gui.Print(Colour.White, "The ", -1);
            }
            Gui.Print(Colour.White, _monsterType.Name, -1);
            Gui.Print(Colour.White, " ('", -1);
            Gui.Print(a1, c1);
            Gui.Print(Colour.White, "')", -1);
        }

        private bool KnowArmour(MonsterRace monsterType, MonsterKnowledge knowledge)
        {
            int kills = knowledge.RTkills;
            if ((kills > 304 / (4 + monsterType.Level)) || knowledge.RProbed)
            {
                return true;
            }
            if ((monsterType.Flags1 & MonsterFlag1.Unique) == 0)
            {
                return false;
            }
            return kills > 304 / (38 + (5 * monsterType.Level / 4));
        }

        private bool KnowDamage(MonsterRace monsterType, MonsterKnowledge knowledge, int i)
        {
            int a = knowledge.RBlows[i];
            int d1 = monsterType.Attack[i].DDice;
            int d2 = monsterType.Attack[i].DSide;
            int d = d1 * d2;
            if ((4 + monsterType.Level) * a > 80 * d)
            {
                return true;
            }
            if ((monsterType.Flags1 & MonsterFlag1.Unique) == 0)
            {
                return false;
            }
            return (4 + monsterType.Level) * 2 * a > 80 * d;
        }
    }
}