using Cthangband.Enumerations;
using Cthangband.Spells;
using Cthangband.StaticData;
using Cthangband.UI;

namespace Cthangband.Projection
{
    internal class ProjectDisintegrate : Projectile
    {
        public ProjectDisintegrate(SpellEffectsHandler spellEffectsHandler) : base(spellEffectsHandler)
        {
            BoltGraphic = "GreenBolt";
            ImpactGraphic = "";
            EffectAnimation = "GreenContract";
        }

        protected override bool AffectFloor(int y, int x)
        {
            return false;
        }

        protected override bool AffectItem(int who, int y, int x)
        {
            GridTile cPtr = Level.Grid[y][x];
            int nextOIdx;
            bool obvious = false;
            FlagSet f1 = new FlagSet();
            FlagSet f2 = new FlagSet();
            FlagSet f3 = new FlagSet();
            string oName = "";
            for (int thisOIdx = cPtr.ItemIndex; thisOIdx != 0; thisOIdx = nextOIdx)
            {
                bool isArt = false;
                bool plural = false;
                Item oPtr = Level.Items[thisOIdx];
                nextOIdx = oPtr.NextInStack;
                oPtr.GetMergedFlags(f1, f2, f3);
                if (oPtr.Count > 1)
                {
                    plural = true;
                }
                if (oPtr.IsFixedArtifact() || string.IsNullOrEmpty(oPtr.RandartName) == false)
                {
                    isArt = true;
                }
                string noteKill = plural ? " evaporate!" : " evaporates!";
                if (oPtr.Marked)
                {
                    obvious = true;
                    oName = oPtr.Description(false, 0);
                }
                if (isArt)
                {
                    if (oPtr.Marked)
                    {
                        string s = plural ? "are" : "is";
                        Profile.Instance.MsgPrint($"The {oName} {s} unaffected!");
                    }
                }
                else
                {
                    if (oPtr.Marked && string.IsNullOrEmpty(noteKill))
                    {
                        Profile.Instance.MsgPrint($"The {oName}{noteKill}");
                    }
                    int oSval = oPtr.ItemSubCategory;
                    bool isPotion = oPtr.ItemType.Category == ItemCategory.Potion;
                    Level.DeleteObjectIdx(thisOIdx);
                    if (isPotion)
                    {
                        SpellEffects.PotionSmashEffect(who, y, x, oSval);
                    }
                    Level.RedrawSingleLocation(y, x);
                }
            }
            return obvious;
        }

        protected override bool AffectMonster(int who, int r, int y, int x, int dam)
        {
            GridTile cPtr = Level.Grid[y][x];
            Monster mPtr = Level.Monsters[cPtr.MonsterIndex];
            MonsterRace rPtr = mPtr.Race;
            bool seen = mPtr.IsVisible;
            bool obvious = false;
            string note = null;
            string noteDies = " dies.";
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
            if ((rPtr.Flags3 & MonsterFlag3.Demon) != 0 || (rPtr.Flags3 & MonsterFlag3.Undead) != 0 ||
                (rPtr.Flags3 & MonsterFlag3.Cthuloid) != 0 || (rPtr.Flags2 & MonsterFlag2.Stupid) != 0 ||
                (rPtr.Flags3 & MonsterFlag3.Nonliving) != 0 || "Evg".Contains(rPtr.Character.ToString()))
            {
                noteDies = " is destroyed.";
            }
            if (who == 0 && (mPtr.Mind & Constants.SmFriendly) != 0)
            {
                Profile.Instance.MsgPrint($"{mName} gets angry!");
                mPtr.Mind &= ~Constants.SmFriendly;
            }
            if (seen)
            {
                obvious = true;
            }
            if ((rPtr.Flags3 & MonsterFlag3.HurtByRock) != 0)
            {
                if (seen)
                {
                    rPtr.Knowledge.RFlags3 |= MonsterFlag3.HurtByRock;
                }
                note = " loses some skin!";
                noteDies = " evaporates!";
                dam *= 2;
            }
            if ((rPtr.Flags3 & MonsterFlag1.Unique) != 0)
            {
                if (Program.Rng.RandomLessThan(rPtr.Level + 10) > Program.Rng.RandomLessThan(Player.Level))
                {
                    note = " resists.";
                    dam >>= 3;
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
                    if (fear && mPtr.IsVisible)
                    {
                        Gui.PlaySound(SoundEffect.MonsterFlees);
                        Profile.Instance.MsgPrint($"{mName} flees in terror!");
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
                Profile.Instance.MsgPrint("You are hit by pure energy!");
            }
            Player.TakeHit(dam, killer);
            SaveGame.Disturb(true);
            return true;
        }
    }
}