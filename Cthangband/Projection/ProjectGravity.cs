using Cthangband.Enumerations;
using Cthangband.Spells;
using Cthangband.StaticData;
using Cthangband.UI;

namespace Cthangband.Projection
{
    internal class ProjectGravity : Projectile
    {
        public ProjectGravity(SpellEffectsHandler spellEffectsHandler) : base(spellEffectsHandler)
        {
            BoltGraphic = "TurquoiseBolt";
            ImpactGraphic = "TurquoiseSplat";
            EffectAnimation = "";
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
            GridTile cPtr = Level.Grid[y][x];
            Monster mPtr = Level.Monsters[cPtr.Monster];
            MonsterRace rPtr = mPtr.Race;
            bool seen = mPtr.IsVisible;
            bool obvious = false;
            int doStun = 0;
            string note = null;
            string noteDies = " dies.";
            if (cPtr.Monster == 0)
            {
                return false;
            }
            if (who != 0 && cPtr.Monster == who)
            {
                return false;
            }
            dam = (dam + r) / (r + 1);
            string mName = mPtr.MonsterDesc(0);
            if ((rPtr.Flags3 & MonsterFlag3.Demon) != 0 || (rPtr.Flags3 & MonsterFlag3.Undead) != 0 ||
                (rPtr.Flags3 & MonsterFlag3.Cthuloid) != 0 || (rPtr.Flags2 & MonsterFlag2.Stupid) != 0 ||
                (rPtr.Flags3 & MonsterFlag3.Nonliving) != 0 || "Evg".Contains(rPtr.Character.ToString()))
            {
                noteDies = " is destroyed.";
            }
            if (who == 0 && (mPtr.Mind & Constants.SmFriendly) != 0)
            {
                if (who == 0)
                {
                    Profile.Instance.MsgPrint($"{mName} gets angry!");
                    mPtr.Mind &= ~Constants.SmFriendly;
                }
            }
            bool resistTele = false;
            if (seen)
            {
                obvious = true;
            }
            if ((rPtr.Flags3 & MonsterFlag3.ResistTeleport) != 0)
            {
                if ((rPtr.Flags1 & MonsterFlag1.Unique) != 0)
                {
                    if (seen)
                    {
                        rPtr.Knowledge.RFlags3 |= MonsterFlag3.ResistTeleport;
                    }
                    note = " is unaffected!";
                    resistTele = true;
                }
                else if (rPtr.Level > Program.Rng.DieRoll(100))
                {
                    if (seen)
                    {
                        rPtr.Knowledge.RFlags3 |= MonsterFlag3.ResistTeleport;
                    }
                    note = " resists!";
                    resistTele = true;
                }
            }
            int doDist = resistTele ? 0 : 10;
            if ((rPtr.Flags4 & MonsterFlag4.BreatheGravity) != 0)
            {
                note = " resists.";
                dam *= 3;
                dam /= Program.Rng.DieRoll(6) + 6;
                doDist = 0;
            }
            else
            {
                if ((rPtr.Flags1 & MonsterFlag1.Unique) != 0 ||
                    rPtr.Level > Program.Rng.DieRoll(dam - 10 < 1 ? 1 : dam - 10) + 10)
                {
                    obvious = false;
                }
                else
                {
                    if (mPtr.Speed > 60)
                    {
                        mPtr.Speed -= 10;
                    }
                    note = " starts moving slower.";
                }
                doStun = Program.Rng.DiceRoll((Player.Level / 10) + 3, dam) + 1;
                if ((rPtr.Flags1 & MonsterFlag1.Unique) != 0 ||
                    rPtr.Level > Program.Rng.DieRoll(dam - 10 < 1 ? 1 : dam - 10) + 10)
                {
                    doStun = 0;
                    note = " is unaffected!";
                    obvious = false;
                }
            }
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
            else if (doDist != 0)
            {
                if (seen)
                {
                    obvious = true;
                }
                note = " disappears!";
                SpellEffects.TeleportAway(cPtr.Monster, doDist);
                y = mPtr.MapY;
                x = mPtr.MapX;
                cPtr = Level.Grid[y][x];
            }
            else if (doStun != 0 && (rPtr.Flags4 & MonsterFlag4.BreatheSound) == 0 &&
                     (rPtr.Flags4 & MonsterFlag4.BreatheForce) == 0)
            {
                if (seen)
                {
                    obvious = true;
                }
                int tmp;
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
            if (who != 0)
            {
                if (SaveGame.TrackedMonsterIndex == cPtr.Monster)
                {
                    Player.RedrawNeeded.Set(RedrawFlag.PrHealth);
                }
                mPtr.SleepLevel = 0;
                mPtr.Health -= dam;
                if (mPtr.Health < 0)
                {
                    bool sad = (mPtr.Mind & Constants.SmFriendly) != 0 && !mPtr.IsVisible;
                    SaveGame.MonsterDeath(cPtr.Monster);
                    Level.Monsters.DeleteMonsterByIndex(cPtr.Monster, true);
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
                        Level.Monsters.MessagePain(cPtr.Monster, dam);
                    }
                }
            }
            else
            {
                if (Level.Monsters.DamageMonster(cPtr.Monster, dam, out bool fear, noteDies))
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
                        Level.Monsters.MessagePain(cPtr.Monster, dam);
                    }
                    if (fear && mPtr.IsVisible)
                    {
                        Gui.PlaySound(SoundEffect.MonsterFlees);
                        Profile.Instance.MsgPrint($"{mName} flees in terror!");
                    }
                }
            }
            Level.Monsters.UpdateMonsterVisibility(cPtr.Monster, false);
            Level.RedrawSingleLocation(y, x);
            ProjectMn++;
            ProjectMx = x;
            ProjectMy = y;
            return obvious;
        }

        protected override bool AffectPlayer(int who, int r, int y, int x, int dam, int aRad)
        {
            bool blind = Player.TimedBlindness != 0;
            bool fuzzy = false;
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
            if (dam > 1600)
            {
                dam = 1600;
            }
            dam = (dam + r) / (r + 1);
            if (blind)
            {
                fuzzy = true;
            }
            Monster mPtr = Level.Monsters[who];
            string killer = mPtr.MonsterDesc(0x88);
            if (fuzzy)
            {
                Profile.Instance.MsgPrint("You are hit by something heavy!");
            }
            Profile.Instance.MsgPrint("Gravity warps around you.");
            SpellEffects.TeleportPlayer(5);
            if (!Player.HasFeatherFall)
            {
                Player.SetTimedSlow(Player.TimedSlow + Program.Rng.RandomLessThan(4) + 4);
            }
            if (!(Player.HasSoundResistance || Player.HasFeatherFall))
            {
                int kk = Program.Rng.DieRoll(dam > 90 ? 35 : (dam / 3) + 5);
                Player.SetTimedStun(Player.TimedStun + kk);
            }
            if (Player.HasFeatherFall)
            {
                dam = dam * 2 / 3;
            }
            if (!Player.HasFeatherFall || Program.Rng.DieRoll(13) == 1)
            {
                Player.Inventory.InvenDamage(SpellEffects.SetColdDestroy, 2);
            }
            Player.TakeHit(dam, killer);
            SaveGame.Disturb(true);
            return true;
        }
    }
}