using Cthangband.Enumerations;
using Cthangband.Spells;
using Cthangband.StaticData;
using Cthangband.UI;

namespace Cthangband.Projection
{
    internal class ProjectKillWall : Projectile
    {
        public ProjectKillWall(SpellEffectsHandler spellEffectsHandler) : base(spellEffectsHandler)
        {
            BoltGraphic = "";
            ImpactGraphic = "";
            EffectAnimation = "BrownSwirl";
        }

        protected override bool AffectFloor(int y, int x)
        {
            GridTile cPtr = Level.Grid[y][x];
            bool obvious = false;
            if (Level.GridPassable(y, x))
            {
                return false;
            }
            if (cPtr.FeatureType.IsPermanent)
            {
                return false;
            }
            if (cPtr.FeatureType.Name.Contains("Treas"))
            {
                if (cPtr.TileFlags.IsSet(GridTile.PlayerMemorised))
                {
                    Profile.Instance.MsgPrint("The vein turns into mud!");
                    Profile.Instance.MsgPrint("You have found something!");
                    obvious = true;
                }
                cPtr.TileFlags.Clear(GridTile.PlayerMemorised);
                Level.CaveRemoveFeat(y, x);
                Level.PlaceGold(y, x);
            }
            else if (cPtr.FeatureType.Name.Contains("Magma") || cPtr.FeatureType.Name.Contains("Quartz"))
            {
                if (cPtr.TileFlags.IsSet(GridTile.PlayerMemorised))
                {
                    Profile.Instance.MsgPrint("The vein turns into mud!");
                    obvious = true;
                }
                cPtr.TileFlags.Clear(GridTile.PlayerMemorised);
                Level.CaveRemoveFeat(y, x);
            }
            else if (cPtr.FeatureType.IsWall)
            {
                if (cPtr.TileFlags.IsSet(GridTile.PlayerMemorised))
                {
                    Profile.Instance.MsgPrint("The wall turns into mud!");
                    obvious = true;
                }
                cPtr.TileFlags.Clear(GridTile.PlayerMemorised);
                Level.CaveRemoveFeat(y, x);
            }
            else if (cPtr.FeatureType.Name == "Rubble")
            {
                if (cPtr.TileFlags.IsSet(GridTile.PlayerMemorised))
                {
                    Profile.Instance.MsgPrint("The rubble turns into mud!");
                    obvious = true;
                }
                cPtr.TileFlags.Clear(GridTile.PlayerMemorised);
                Level.CaveRemoveFeat(y, x);
                if (Program.Rng.RandomLessThan(100) < 10)
                {
                    if (Level.PlayerCanSeeBold(y, x))
                    {
                        Profile.Instance.MsgPrint("There was something buried in the rubble!");
                        obvious = true;
                    }
                    Level.PlaceObject(y, x, false, false);
                }
            }
            else
            {
                if (cPtr.TileFlags.IsSet(GridTile.PlayerMemorised))
                {
                    Profile.Instance.MsgPrint("The door turns into mud!");
                    obvious = true;
                }
                cPtr.TileFlags.Clear(GridTile.PlayerMemorised);
                Level.CaveRemoveFeat(y, x);
            }
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateView | UpdateFlags.UpdateLight | UpdateFlags.UpdateScent | UpdateFlags.UpdateMonsters);
            return obvious;
        }

        protected override bool AffectItem(int who, int y, int x)
        {
            return false;
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
                bool getAngry = (rPtr.Flags3 & MonsterFlag3.HurtByRock) != 0;
                if (getAngry && who == 0)
                {
                    Profile.Instance.MsgPrint($"{mName} gets angry!");
                    mPtr.Mind &= ~Constants.SmFriendly;
                }
            }
            if ((rPtr.Flags3 & MonsterFlag3.HurtByRock) != 0)
            {
                if (seen)
                {
                    obvious = true;
                }
                if (seen)
                {
                    rPtr.Knowledge.RFlags3 |= MonsterFlag3.HurtByRock;
                }
                note = " loses some skin!";
                noteDies = " dissolves!";
            }
            else
            {
                dam = 0;
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