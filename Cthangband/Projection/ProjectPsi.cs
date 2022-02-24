using Cthangband.Enumerations;
using Cthangband.Spells;
using Cthangband.StaticData;
using Cthangband.UI;

namespace Cthangband.Projection
{
    internal class ProjectPsi : Projectile
    {
        public ProjectPsi(SpellEffectsHandler spellEffectsHandler) : base(spellEffectsHandler)
        {
            BoltGraphic = "";
            ImpactGraphic = "";
            EffectAnimation = "DiamondSparkle";
        }

        protected override bool AffectFloor(int y, int x)
        {
            return false;
        }

        protected override bool AffectItem(int who, int y, int x)
        {
            return false;
        }

        protected override bool AffectMonster(int who, int r, int y, int x, int dam)
        {
            int tmp;
            GridTile cPtr = Level.Grid[y][x];
            Monster mPtr = Level.Monsters[cPtr.MonsterIndex];
            MonsterRace rPtr = mPtr.Race;
            bool seen = mPtr.IsVisible;
            bool obvious = false;
            int doConf = 0;
            int doStun = 0;
            int doSleep = 0;
            int doFear = 0;
            string note = null;
            if (cPtr.MonsterIndex == 0)
            {
                return false;
            }
            if (who != 0 && cPtr.MonsterIndex == who)
            {
                return false;
            }
            dam = (dam + r) / (r + 1);
            string mName = mPtr.MonsterDesc(0);
            if (who == 0 && (mPtr.Mind & Constants.SmFriendly) != 0)
            {
                bool getAngry = (rPtr.Flags2 & MonsterFlag2.EmptyMind) == 0;
                if (getAngry && who == 0)
                {
                    Profile.Instance.MsgPrint($"{mName} gets angry!");
                    mPtr.Mind &= ~Constants.SmFriendly;
                }
            }
            if (seen)
            {
                obvious = true;
            }
            if ((rPtr.Flags2 & MonsterFlag2.EmptyMind) != 0)
            {
                dam = 0;
                note = " is immune!";
            }
            else if ((rPtr.Flags2 & MonsterFlag2.Stupid) != 0 || (rPtr.Flags2 & MonsterFlag2.WeirdMind) != 0 ||
                     (rPtr.Flags3 & MonsterFlag3.Animal) != 0 || rPtr.Level > Program.Rng.DieRoll(3 * dam))
            {
                dam /= 3;
                note = " resists.";
                if (((rPtr.Flags3 & MonsterFlag3.Undead) != 0 || (rPtr.Flags3 & MonsterFlag3.Demon) != 0) &&
                    rPtr.Level > Player.Level / 2 && Program.Rng.DieRoll(2) == 1)
                {
                    note = null;
                    string s = seen ? "'s" : "s";
                    Profile.Instance.MsgPrint($"{mName}{s} corrupted mind backlashes your attack!");
                    if (Program.Rng.RandomLessThan(100) < Player.SkillSavingThrow)
                    {
                        Profile.Instance.MsgPrint("You resist the effects!");
                    }
                    else
                    {
                        string killer = mPtr.MonsterDesc(0x88);
                        Player.TakeHit(dam, killer);
                        if (Program.Rng.DieRoll(4) == 1)
                        {
                            switch (Program.Rng.DieRoll(4))
                            {
                                case 1:
                                    Player.SetTimedConfusion(Player.TimedConfusion + 3 + Program.Rng.DieRoll(dam));
                                    break;

                                case 2:
                                    Player.SetTimedStun(Player.TimedStun + Program.Rng.DieRoll(dam));
                                    break;

                                case 3:
                                    {
                                        if ((rPtr.Flags3 & MonsterFlag3.ImmuneFear) != 0)
                                        {
                                            note = " is unaffected.";
                                        }
                                        else
                                        {
                                            Player.SetTimedFear(Player.TimedFear + 3 + Program.Rng.DieRoll(dam));
                                        }
                                    }
                                    break;

                                default:
                                    if (!Player.HasFreeAction)
                                    {
                                        Player.SetTimedParalysis(Player.TimedParalysis + Program.Rng.DieRoll(dam));
                                    }
                                    break;
                            }
                        }
                    }
                    dam = 0;
                }
            }
            if (dam > 0 && Program.Rng.DieRoll(4) == 1)
            {
                switch (Program.Rng.DieRoll(4))
                {
                    case 1:
                        doConf = 3 + Program.Rng.DieRoll(dam);
                        break;

                    case 2:
                        doStun = 3 + Program.Rng.DieRoll(dam);
                        break;

                    case 3:
                        doFear = 3 + Program.Rng.DieRoll(dam);
                        break;

                    default:
                        doSleep = 3 + Program.Rng.DieRoll(dam);
                        break;
                }
            }
            string noteDies = " collapses, a mindless husk.";
            if ((rPtr.Flags1 & MonsterFlag1.Guardian) != 0)
            {
                if (who != 0 && dam > mPtr.Health)
                {
                    dam = mPtr.Health;
                }
            }
            if ((rPtr.Flags1 & MonsterFlag1.Guardian) != 0)
            {
                if (who > 0 && dam > mPtr.Health)
                {
                    dam = mPtr.Health;
                }
            }
            if (dam > mPtr.Health)
            {
                note = noteDies;
            }
            else if (doStun != 0 && (rPtr.Flags4 & MonsterFlag4.BreatheSound) == 0 &&
                     (rPtr.Flags4 & MonsterFlag4.BreatheForce) == 0)
            {
                if (mPtr.StunLevel != 0)
                {
                    note = " is more dazed.";
                    tmp = mPtr.StunLevel + (doStun / 2);
                }
                else
                {
                    note = " is dazed.";
                    tmp = doStun;
                }
                mPtr.StunLevel = tmp < 200 ? tmp : 200;
            }
            else if (doConf != 0 && (rPtr.Flags3 & MonsterFlag3.ImmuneConfusion) == 0 &&
                     (rPtr.Flags4 & MonsterFlag4.BreatheConfusion) == 0 && (rPtr.Flags4 & MonsterFlag4.BreatheChaos) == 0)
            {
                if (mPtr.ConfusionLevel != 0)
                {
                    note = " looks more confused.";
                    tmp = mPtr.ConfusionLevel + (doConf / 2);
                }
                else
                {
                    note = " looks confused.";
                    tmp = doConf;
                }
                mPtr.ConfusionLevel = tmp < 200 ? tmp : 200;
            }
            if (doFear != 0)
            {
                tmp = mPtr.FearLevel + doFear;
                mPtr.FearLevel = tmp < 200 ? tmp : 200;
            }
            if (who != 0)
            {
                if (SaveGame.TrackedMonsterIndex == cPtr.MonsterIndex)
                {
                    Player.RedrawNeeded.Set(RedrawFlag.PrHealth);
                }
                mPtr.SleepLevel = 0;
                mPtr.Health -= dam;
                if (mPtr.Health < 0)
                {
                    bool sad = (mPtr.Mind & Constants.SmFriendly) != 0 && !mPtr.IsVisible;
                    SaveGame.MonsterDeath(cPtr.MonsterIndex);
                    Level.Monsters.DeleteMonsterByIndex(cPtr.MonsterIndex, true);
                    if (string.IsNullOrEmpty(note) == false)
                    {
                        Profile.Instance.MsgPrint($"{mName}{note}");
                    }
                    if (sad)
                    {
                        Profile.Instance.MsgPrint("You feel sad for a moment.");
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(note) == false && seen)
                    {
                        Profile.Instance.MsgPrint($"{mName}{note}");
                    }
                    else if (dam > 0)
                    {
                        Level.Monsters.MessagePain(cPtr.MonsterIndex, dam);
                    }
                    if (doSleep != 0)
                    {
                        mPtr.SleepLevel = doSleep;
                    }
                }
            }
            else
            {
                if (Level.Monsters.DamageMonster(cPtr.MonsterIndex, dam, out bool fear, noteDies))
                {
                }
                else
                {
                    if (string.IsNullOrEmpty(note) == false && seen)
                    {
                        Profile.Instance.MsgPrint($"{mName}{note}");
                    }
                    else if (dam > 0)
                    {
                        Level.Monsters.MessagePain(cPtr.MonsterIndex, dam);
                    }
                    if ((fear || doFear != 0) && mPtr.IsVisible)
                    {
                        Gui.PlaySound(SoundEffect.MonsterFlees);
                        Profile.Instance.MsgPrint($"{mName} flees in terror!");
                    }
                    if (doSleep != 0)
                    {
                        mPtr.SleepLevel = doSleep;
                    }
                }
            }
            Level.Monsters.UpdateMonsterVisibility(cPtr.MonsterIndex, false);
            Level.RedrawSingleLocation(y, x);
            ProjectMn++;
            ProjectMx = x;
            ProjectMy = y;
            return obvious;
        }

        protected override bool AffectPlayer(int who, int r, int y, int x, int dam, int aRad)
        {
            bool blind = Player.TimedBlindness != 0;
            if (x != Player.MapX || y != Player.MapY)
            {
                return false;
            }
            if (who == 0)
            {
                return false;
            }
            if (Player.HasReflection && aRad == 0 && Program.Rng.DieRoll(10) != 1)
            {
                int tY;
                int tX;
                int maxAttempts = 10;
                Profile.Instance.MsgPrint(blind ? "Something bounces!" : "The attack bounces!");
                do
                {
                    tY = Level.Monsters[who].MapY - 1 + Program.Rng.DieRoll(3);
                    tX = Level.Monsters[who].MapX - 1 + Program.Rng.DieRoll(3);
                    maxAttempts--;
                } while (maxAttempts > 0 && Level.InBounds2(tY, tX) && !Level.PlayerHasLosBold(tY, tX));
                if (maxAttempts < 1)
                {
                    tY = Level.Monsters[who].MapY;
                    tX = Level.Monsters[who].MapX;
                }
                Fire(0, 0, tY, tX, dam, ProjectionFlag.ProjectStop | ProjectionFlag.ProjectKill);
                SaveGame.Disturb(true);
                return true;
            }
            SaveGame.Disturb(true);
            return true;
        }
    }
}