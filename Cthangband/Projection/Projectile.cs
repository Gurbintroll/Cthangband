// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Spells;
using Cthangband.StaticData;
using Cthangband.UI;

namespace Cthangband.Projection
{
    internal abstract class Projectile
    {
        protected readonly Level Level;
        protected readonly Player Player;
        protected readonly SaveGame SaveGame;
        protected readonly SpellEffectsHandler SpellEffects;
        protected string BoltGraphic = "WhiteBolt";
        protected string EffectAnimation = string.Empty;
        protected string ImpactGraphic = "WhiteSplat";
        protected int ProjectMn;
        protected int ProjectMx;
        protected int ProjectMy;

        public Projectile(SpellEffectsHandler spellEffectsHandler)
        {
            SpellEffects = spellEffectsHandler;
            SaveGame = SaveGame.Instance;
            Player = SaveGame.Player;
            Level = SaveGame.Level;
        }

        public bool Fire(int who, int rad, int y, int x, int dam, ProjectionFlag flg)
        {
            int i, dist;
            int y1, x1;
            int msec = GlobalData.DelayFactor * GlobalData.DelayFactor * GlobalData.DelayFactor;
            GridTile cPtr;
            bool notice = false;
            bool visual = false;
            bool drawn = false;
            bool breath = false;
            bool blind = Player.TimedBlindness != 0;
            int grids = 0;
            int[] gx = new int[256];
            int[] gy = new int[256];
            int[] gm = new int[32];
            int gmRad = rad;
            ProjectileGraphic projectileEntity = string.IsNullOrEmpty(BoltGraphic) ? null : StaticResources.Instance.ProjectileGraphics[BoltGraphic];
            ProjectileGraphic impactEntity = string.IsNullOrEmpty(ImpactGraphic) ? null : StaticResources.Instance.ProjectileGraphics[ImpactGraphic];
            Animation animationEntity = string.IsNullOrEmpty(EffectAnimation) ? null : StaticResources.Instance.Animations[EffectAnimation];
            if ((flg & ProjectionFlag.ProjectJump) != 0)
            {
                x1 = x;
                y1 = y;
            }
            else if (who == 0)
            {
                x1 = Player.MapX;
                y1 = Player.MapY;
            }
            else
            {
                x1 = Level.Monsters[who].MapX;
                y1 = Level.Monsters[who].MapY;
            }
            int ySaver = y1;
            int xSaver = x1;
            int y2 = y;
            int x2 = x;
            if ((flg & ProjectionFlag.ProjectThru) != 0)
            {
                if (x1 == x2 && y1 == y2)
                {
                    flg &= ~ProjectionFlag.ProjectThru;
                }
            }
            if (rad < 0)
            {
                rad = 0 - rad;
                breath = true;
                flg |= ProjectionFlag.ProjectHide;
            }
            for (dist = 0; dist < 32; dist++)
            {
                gm[dist] = 0;
            }
            SaveGame.HandleStuff();
            x = x1;
            y = y1;
            dist = 0;
            while (true)
            {
                if ((flg & ProjectionFlag.ProjectBeam) != 0)
                {
                    gy[grids] = y;
                    gx[grids] = x;
                    grids++;
                }
                if (!blind && (flg & ProjectionFlag.ProjectHide) == 0 && dist != 0 &&
                    (flg & ProjectionFlag.ProjectBeam) != 0 && Level.PanelContains(y, x) &&
                    Level.PlayerHasLosBold(y, x))
                {
                    if (impactEntity != null)
                    {
                        Level.PrintCharacterAtMapLocation(impactEntity.Character, impactEntity.Colour, y, x);
                    }
                }
                cPtr = Level.Grid[y][x];
                if (dist != 0 && !Level.GridPassable(y, x))
                {
                    break;
                }
                if ((flg & ProjectionFlag.ProjectThru) == 0 && x == x2 && y == y2)
                {
                    break;
                }
                if (cPtr.MonsterIndex != 0 && dist != 0 && (flg & ProjectionFlag.ProjectStop) != 0)
                {
                    break;
                }
                Level.MoveOneStepTowards(out int y9, out int x9, y, x, y1, x1, y2, x2);
                if (!Level.GridPassable(y9, x9) && rad > 0)
                {
                    break;
                }
                dist++;
                if (dist > Constants.MaxRange)
                {
                    break;
                }
                if (!blind && (flg & ProjectionFlag.ProjectHide) == 0)
                {
                    if (Level.PlayerHasLosBold(y9, x9) && Level.PanelContains(y9, x9))
                    {
                        if (projectileEntity != null)
                        {
                            char directionalCharacter = projectileEntity.Character;
                            if (directionalCharacter == '|')
                            {
                                directionalCharacter = BoltChar(y, x, y9, x9);
                            }
                            Level.PrintCharacterAtMapLocation(directionalCharacter, projectileEntity.Colour, y9, x9);
                            Level.MoveCursorRelative(y9, x9);
                            Gui.Refresh();
                            visual = true;
                            Gui.Pause(msec);
                            Level.RedrawSingleLocation(y9, x9);
                            Gui.Refresh();
                        }
                    }
                    else if (visual)
                    {
                        Gui.Pause(msec);
                    }
                }
                y = y9;
                x = x9;
            }
            y2 = y;
            x2 = x;
            gm[0] = 0;
            gm[1] = grids;
            int distHack = dist;
            if (dist <= Constants.MaxRange)
            {
                if ((flg & ProjectionFlag.ProjectBeam) != 0 && grids > 0)
                {
                    grids--;
                }
                if (breath)
                {
                    int brad = 0;
                    int bdis = 0;
                    bool done = false;
                    flg &= ~ProjectionFlag.ProjectHide;
                    int by = y1;
                    int bx = x1;
                    while (bdis <= dist + rad)
                    {
                        for (int cdis = 0; cdis <= brad; cdis++)
                        {
                            for (y = by - cdis; y <= by + cdis; y++)
                            {
                                for (x = bx - cdis; x <= bx + cdis; x++)
                                {
                                    if (!Level.InBounds(y, x))
                                    {
                                        continue;
                                    }
                                    if (Level.Distance(y1, x1, y, x) != bdis)
                                    {
                                        continue;
                                    }
                                    if (Level.Distance(by, bx, y, x) != cdis)
                                    {
                                        continue;
                                    }
                                    if (!Level.Los(by, bx, y, x))
                                    {
                                        continue;
                                    }
                                    gy[grids] = y;
                                    gx[grids] = x;
                                    grids++;
                                }
                            }
                        }
                        gm[bdis + 1] = grids;
                        if (by == y2 && bx == x2)
                        {
                            done = true;
                        }
                        if (done)
                        {
                            bdis++;
                            continue;
                        }
                        Level.MoveOneStepTowards(out by, out bx, by, bx, y1, x1, y2, x2);
                        bdis++;
                        brad = rad * bdis / dist;
                    }
                    gmRad = bdis;
                }
                else
                {
                    for (dist = 0; dist <= rad; dist++)
                    {
                        for (y = y2 - dist; y <= y2 + dist; y++)
                        {
                            for (x = x2 - dist; x <= x2 + dist; x++)
                            {
                                if (!Level.InBounds2(y, x))
                                {
                                    continue;
                                }
                                if (Level.Distance(y2, x2, y, x) != dist)
                                {
                                    continue;
                                }
                                if (GetType().Name == "ProjectDisintegrate")
                                {
                                    if (Level.CaveValidBold(y, x))
                                    {
                                        Level.RevertTileToBackground(y, x);
                                    }
                                    Player.UpdatesNeeded.Set(UpdateFlags.UpdateView | UpdateFlags.UpdateLight | UpdateFlags.UpdateScent |
                                                      UpdateFlags.UpdateMonsters);
                                }
                                else
                                {
                                    if (!Level.Los(y2, x2, y, x))
                                    {
                                        continue;
                                    }
                                }
                                gy[grids] = y;
                                gx[grids] = x;
                                grids++;
                            }
                        }
                        gm[dist + 1] = grids;
                    }
                }
            }
            if (grids == 0)
            {
                return false;
            }
            if (!blind && (flg & ProjectionFlag.ProjectHide) == 0)
            {
                for (int t = 0; t <= gmRad; t++)
                {
                    for (i = gm[t]; i < gm[t + 1]; i++)
                    {
                        y = gy[i];
                        x = gx[i];
                        if (Level.PlayerHasLosBold(y, x) && Level.PanelContains(y, x))
                        {
                            if (impactEntity != null)
                            {
                                drawn = true;
                                Level.PrintCharacterAtMapLocation(impactEntity.Character, impactEntity.Colour, y, x);
                            }
                        }
                    }
                    if (impactEntity != null)
                    {
                        Level.MoveCursorRelative(y2, x2);
                        Gui.Refresh();
                        if (visual || drawn)
                        {
                            Gui.Pause(msec);
                        }
                    }
                }
                if (drawn)
                {
                    for (i = 0; i < grids; i++)
                    {
                        y = gy[i];
                        x = gx[i];
                        if (Level.PlayerHasLosBold(y, x) && Level.PanelContains(y, x))
                        {
                            Level.RedrawSingleLocation(y, x);
                        }
                    }
                    Level.MoveCursorRelative(y2, x2);
                    Gui.Refresh();
                }
            }

            if (animationEntity != null)
            {
                animationEntity.Animate(Level, gy, gx);
            }

            if ((flg & ProjectionFlag.ProjectGrid) != 0)
            {
                dist = 0;
                for (i = 0; i < grids; i++)
                {
                    if (gm[dist + 1] == i)
                    {
                        dist++;
                    }
                    y = gy[i];
                    x = gx[i];
                    if (AffectFloor(y, x))
                    {
                        notice = true;
                    }
                }
            }
            if ((flg & ProjectionFlag.ProjectItem) != 0)
            {
                dist = 0;
                for (i = 0; i < grids; i++)
                {
                    if (gm[dist + 1] == i)
                    {
                        dist++;
                    }
                    y = gy[i];
                    x = gx[i];
                    if (AffectItem(who, y, x))
                    {
                        notice = true;
                    }
                }
            }
            if ((flg & ProjectionFlag.ProjectKill) != 0)
            {
                ProjectMn = 0;
                ProjectMx = 0;
                ProjectMy = 0;
                dist = 0;
                for (i = 0; i < grids; i++)
                {
                    if (gm[dist + 1] == i)
                    {
                        dist++;
                    }
                    y = gy[i];
                    x = gx[i];
                    if (grids > 1)
                    {
                        if (AffectMonster(who, dist, y, x, dam))
                        {
                            notice = true;
                        }
                    }
                    else
                    {
                        if (cPtr.MonsterIndex == 0)
                        {
                            continue;
                        }
                        MonsterRace refPtr = Level.Monsters[cPtr.MonsterIndex].Race;
                        if ((refPtr.Flags2 & MonsterFlag2.Reflecting) != 0 && Program.Rng.DieRoll(10) != 1 &&
                            distHack > 1 && GetType().Name != "ProjectWizardBolt")
                        {
                            int tY, tX;
                            int maxAttempts = 10;
                            do
                            {
                                tY = ySaver - 1 + Program.Rng.DieRoll(3);
                                tX = xSaver - 1 + Program.Rng.DieRoll(3);
                                maxAttempts--;
                            } while (maxAttempts > 0 && Level.InBounds2(tY, tX) && !Level.Los(y, x, tY, tX));
                            if (maxAttempts < 1)
                            {
                                tY = ySaver;
                                tX = xSaver;
                            }
                            if (Level.Monsters[cPtr.MonsterIndex].IsVisible)
                            {
                                Profile.Instance.MsgPrint("The attack bounces!");
                                refPtr.Knowledge.RFlags2 |= MonsterFlag2.Reflecting;
                            }
                            Fire(cPtr.MonsterIndex, 0, tY, tX, dam, flg);
                        }
                        else
                        {
                            if (AffectMonster(who, dist, y, x, dam))
                            {
                                notice = true;
                            }
                        }
                    }
                }
                if (who == 0 && ProjectMn == 1 && (flg & ProjectionFlag.ProjectJump) == 0)
                {
                    x = ProjectMx;
                    y = ProjectMy;
                    cPtr = Level.Grid[y][x];
                    if (cPtr.MonsterIndex != 0)
                    {
                        Monster mPtr = Level.Monsters[cPtr.MonsterIndex];
                        if (mPtr.IsVisible)
                        {
                            SaveGame.HealthTrack(cPtr.MonsterIndex);
                        }
                    }
                }
            }
            if ((flg & ProjectionFlag.ProjectKill) != 0)
            {
                dist = 0;
                for (i = 0; i < grids; i++)
                {
                    if (gm[dist + 1] == i)
                    {
                        dist++;
                    }
                    y = gy[i];
                    x = gx[i];
                    if (AffectPlayer(who, dist, y, x, dam, rad))
                    {
                        notice = true;
                    }
                }
            }
            return notice;
        }

        public void OverrideGraphics(string boltGraphic, string impactGraphic, string effectAnimation)
        {
            BoltGraphic = boltGraphic;
            ImpactGraphic = impactGraphic;
            EffectAnimation = effectAnimation;
        }

        protected abstract bool AffectFloor(int y, int x);

        protected abstract bool AffectItem(int who, int y, int x);

        protected abstract bool AffectMonster(int who, int r, int y, int x, int dam);

        protected abstract bool AffectPlayer(int who, int r, int y, int x, int dam, int aRad);

        private char BoltChar(int y, int x, int ny, int nx)
        {
            if (ny == y && nx == x)
            {
                return '*';
            }
            if (ny == y)
            {
                return '-';
            }
            if (nx == x)
            {
                return '|';
            }
            if (ny - y == x - nx)
            {
                return '/';
            }
            if (ny - y == nx - x)
            {
                return '\\';
            }
            return '*';
        }
    }
}