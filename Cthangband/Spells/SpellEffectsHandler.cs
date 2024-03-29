﻿// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Projection;
using Cthangband.Projection.Base;
using Cthangband.StaticData;
using Cthangband.UI;
using System;

namespace Cthangband.Spells
{
    [Serializable]
    internal class SpellEffectsHandler
    {
        public const int HurtChance = 16;

        private readonly SaveGame _saveGame;

        public SpellEffectsHandler(SaveGame saveGame)
        {
            _saveGame = saveGame;
        }

        public Level Level => _saveGame.Level;

        public Player Player => _saveGame.Player;

        public void AcidDam(int dam, string kbStr)
        {
            var inv = dam < 30 ? 1 : dam < 60 ? 2 : 3;
            if (Player.HasAcidImmunity || dam <= 0)
            {
                return;
            }
            if (Player.HasElementalVulnerability)
            {
                dam *= 2;
            }
            if (Player.HasAcidResistance)
            {
                dam = (dam + 2) / 3;
            }
            if (Player.TimedAcidResistance != 0)
            {
                dam = (dam + 2) / 3;
            }
            if (!(Player.TimedAcidResistance != 0 || Player.HasAcidResistance) && Program.Rng.DieRoll(HurtChance) == 1)
            {
                Player.TryDecreasingAbilityScore(Ability.Charisma);
            }
            if (MinusAc())
            {
                dam = (dam + 1) / 2;
            }
            Player.TakeHit(dam, kbStr);
            if (!(Player.TimedAcidResistance != 0 && Player.HasAcidResistance))
            {
                Player.Inventory.InvenDamage(SetAcidDestroy, inv);
            }
        }

        public void ActivateHiSummon()
        {
            int i;
            for (i = 0; i < Program.Rng.DieRoll(9) + (_saveGame.Difficulty / 40); i++)
            {
                switch (Program.Rng.DieRoll(26) + (_saveGame.Difficulty / 20))
                {
                    case 1:
                    case 2:
                        Level.Monsters.SummonSpecific(Player.MapY, Player.MapX, _saveGame.Difficulty, Constants.SummonAnt);
                        break;

                    case 3:
                    case 4:
                        Level.Monsters.SummonSpecific(Player.MapY, Player.MapX, _saveGame.Difficulty,
                            Constants.SummonSpider);
                        break;

                    case 5:
                    case 6:
                        Level.Monsters.SummonSpecific(Player.MapY, Player.MapX, _saveGame.Difficulty,
                            Constants.SummonHound);
                        break;

                    case 7:
                    case 8:
                        Level.Monsters.SummonSpecific(Player.MapY, Player.MapX, _saveGame.Difficulty,
                            Constants.SummonHydra);
                        break;

                    case 9:
                    case 10:
                        Level.Monsters.SummonSpecific(Player.MapY, Player.MapX, _saveGame.Difficulty,
                            Constants.SummonCthuloid);
                        break;

                    case 11:
                    case 12:
                        Level.Monsters.SummonSpecific(Player.MapY, Player.MapX, _saveGame.Difficulty,
                            Constants.SummonUndead);
                        break;

                    case 13:
                    case 14:
                        Level.Monsters.SummonSpecific(Player.MapY, Player.MapX, _saveGame.Difficulty,
                            Constants.SummonDragon);
                        break;

                    case 15:
                    case 16:
                        Level.Monsters.SummonSpecific(Player.MapY, Player.MapX, _saveGame.Difficulty,
                            Constants.SummonDemon);
                        break;

                    case 17:
                        Level.Monsters.SummonSpecific(Player.MapY, Player.MapX, _saveGame.Difficulty, Constants.SummonGoo);
                        break;

                    case 18:
                    case 19:
                        Level.Monsters.SummonSpecific(Player.MapY, Player.MapX, _saveGame.Difficulty,
                            Constants.SummonUnique);
                        break;

                    case 20:
                    case 21:
                        Level.Monsters.SummonSpecific(Player.MapY, Player.MapX, _saveGame.Difficulty,
                            Constants.SummonHiUndead);
                        break;

                    case 22:
                    case 23:
                        Level.Monsters.SummonSpecific(Player.MapY, Player.MapX, _saveGame.Difficulty,
                            Constants.SummonHiDragon);
                        break;

                    case 24:
                    case 25:
                        Level.Monsters.SummonSpecific(Player.MapY, Player.MapX, 100, Constants.SummonReaver);
                        break;

                    default:
                        Level.Monsters.SummonSpecific(Player.MapY, Player.MapX, (_saveGame.Difficulty * 3 / 2) + 5, 0);
                        break;
                }
            }
        }

        public void AggravateMonsters(int who)
        {
            var sleep = false;
            var speed = false;
            for (var i = 1; i < Level.MMax; i++)
            {
                var mPtr = Level.Monsters[i];
                var rPtr = mPtr.Race;
                if (mPtr.Race == null)
                {
                    continue;
                }
                if (i == who)
                {
                    continue;
                }
                if (mPtr.DistanceFromPlayer < Constants.MaxSight * 2)
                {
                    if (mPtr.SleepLevel != 0)
                    {
                        mPtr.SleepLevel = 0;
                        sleep = true;
                    }
                }
                if (Level.PlayerHasLosBold(mPtr.MapY, mPtr.MapX))
                {
                    if (mPtr.Speed < rPtr.Speed + 10)
                    {
                        mPtr.Speed = rPtr.Speed + 10;
                        speed = true;
                    }
                    if ((mPtr.Mind & Constants.SmFriendly) != 0)
                    {
                        if (Program.Rng.DieRoll(2) == 1)
                        {
                            mPtr.Mind &= ~Constants.SmFriendly;
                        }
                    }
                }
            }
            if (speed)
            {
                Profile.Instance.MsgPrint("You feel a sudden stirring nearby!");
            }
            else if (sleep)
            {
                Profile.Instance.MsgPrint("You hear a sudden stirring in the distance!");
            }
        }

        public void Alchemy()
        {
            var amt = 1;
            var force = Gui.CommandArgument > 0;
            if (!_saveGame.GetItem(out var item, "Turn which item to gold? ", false, true, true))
            {
                if (item == -2)
                {
                    Profile.Instance.MsgPrint("You have nothing to turn to gold.");
                }
                return;
            }
            var oPtr = item >= 0 ? Player.Inventory[item] : Level.Items[0 - item];
            if (oPtr.Count > 1)
            {
                amt = Gui.GetQuantity(null, oPtr.Count, true);
                if (amt <= 0)
                {
                    return;
                }
            }
            var oldNumber = oPtr.Count;
            oPtr.Count = amt;
            var oName = oPtr.Description(true, 3);
            oPtr.Count = oldNumber;
            if (!force)
            {
                if (!(oPtr.Value() < 1))
                {
                    var outVal = $"Really turn {oName} to gold? ";
                    if (!Gui.GetCheck(outVal))
                    {
                        return;
                    }
                }
            }
            if (oPtr.IsArtifact() || oPtr.IsLegendary())
            {
                var feel = "special";
                Profile.Instance.MsgPrint($"You fail to turn {oName} to gold!");
                if (oPtr.IsCursed() || oPtr.IsBroken())
                {
                    feel = "terrible";
                }
                oPtr.Inscription = feel;
                oPtr.IdentifyFlags.Set(Constants.IdentSense);
                Player.NoticeFlags |= Constants.PnCombine;
                return;
            }
            var price = oPtr.RealValue();
            if (price <= 0)
            {
                Profile.Instance.MsgPrint($"You turn {oName} to fool's gold.");
            }
            else
            {
                price /= 3;
                if (amt > 1)
                {
                    price *= amt;
                }
                if (price > 30000)
                {
                    price = 30000;
                }
                Profile.Instance.MsgPrint($"You turn {oName} to {price} coins worth of gold.");
                Player.Gold += price;
                Player.RedrawNeeded.Set(RedrawFlag.PrGold);
            }
            if (item >= 0)
            {
                Player.Inventory.InvenItemIncrease(item, -amt);
                Player.Inventory.InvenItemDescribe(item);
                Player.Inventory.InvenItemOptimize(item);
            }
            else
            {
                Level.FloorItemIncrease(0 - item, -amt);
                Level.FloorItemDescribe(0 - item);
                Level.FloorItemOptimize(0 - item);
            }
        }

        public void AlterReality()
        {
            Profile.Instance.MsgPrint("The world changes!");
            _saveGame.IsAutosave = true;
            _saveGame.DoCmdSaveGame();
            _saveGame.IsAutosave = false;
            _saveGame.NewLevelFlag = true;
            _saveGame.CameFrom = LevelStart.StartRandom;
        }

        public bool ApplyDisenchant()
        {
            var t = 0;
            switch (Program.Rng.DieRoll(8))
            {
                case 1:
                    t = InventorySlot.MeleeWeapon;
                    break;

                case 2:
                    t = InventorySlot.RangedWeapon;
                    break;

                case 3:
                    t = InventorySlot.Body;
                    break;

                case 4:
                    t = InventorySlot.Cloak;
                    break;

                case 5:
                    t = InventorySlot.Arm;
                    break;

                case 6:
                    t = InventorySlot.Head;
                    break;

                case 7:
                    t = InventorySlot.Hands;
                    break;

                case 8:
                    t = InventorySlot.Feet;
                    break;
            }
            var oPtr = Player.Inventory[t];
            if (oPtr.ItemType == null)
            {
                return false;
            }
            if (oPtr.BonusToHit <= 0 && oPtr.BonusDamage <= 0 && oPtr.BonusArmourClass <= 0)
            {
                return false;
            }
            var oName = oPtr.Description(false, 0);
            string s;
            if ((oPtr.IsArtifact() || oPtr.IsLegendary()) &&
                Program.Rng.RandomLessThan(100) < 71)
            {
                s = oPtr.Count != 1 ? "" : "s";
                Profile.Instance.MsgPrint($"Your {oName} ({t.IndexToLabel()}) resist{s} disenchantment!");
                return true;
            }
            if (oPtr.BonusToHit > 0)
            {
                oPtr.BonusToHit--;
            }
            if (oPtr.BonusToHit > 5 && Program.Rng.RandomLessThan(100) < 20)
            {
                oPtr.BonusToHit--;
            }
            if (oPtr.BonusDamage > 0)
            {
                oPtr.BonusDamage--;
            }
            if (oPtr.BonusDamage > 5 && Program.Rng.RandomLessThan(100) < 20)
            {
                oPtr.BonusDamage--;
            }
            if (oPtr.BonusArmourClass > 0)
            {
                oPtr.BonusArmourClass--;
            }
            if (oPtr.BonusArmourClass > 5 && Program.Rng.RandomLessThan(100) < 20)
            {
                oPtr.BonusArmourClass--;
            }
            s = oPtr.Count != 1 ? "were" : "was";
            Profile.Instance.MsgPrint($"Your {oName} ({t.IndexToLabel()}) {s} disenchanted!");
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            return true;
        }

        public void ApplyNexus(Monster mPtr)
        {
            switch (Program.Rng.DieRoll(7))
            {
                case 1:
                case 2:
                case 3:
                    {
                        TeleportPlayer(200);
                        break;
                    }
                case 4:
                case 5:
                    {
                        TeleportPlayerTo(mPtr.MapY, mPtr.MapX);
                        break;
                    }
                case 6:
                    {
                        if (Program.Rng.RandomLessThan(100) < Player.SkillSavingThrow)
                        {
                            Profile.Instance.MsgPrint("You resist the effects!");
                            break;
                        }
                        TeleportPlayerLevel();
                        break;
                    }
                case 7:
                    {
                        if (Program.Rng.RandomLessThan(100) < Player.SkillSavingThrow)
                        {
                            Profile.Instance.MsgPrint("You resist the effects!");
                            break;
                        }
                        Profile.Instance.MsgPrint("Your body starts to scramble...");
                        Player.ShuffleAbilityScores();
                        break;
                    }
            }
        }

        public void ArtifactScroll()
        {
            bool okay;
            _saveGame.ItemFilter = ItemTesterHookWeapon;
            if (!_saveGame.GetItem(out var item, "Enchant which item? ", true, true, true))
            {
                if (item == -2)
                {
                    Profile.Instance.MsgPrint("You have nothing to enchant.");
                }
                return;
            }
            var oPtr = item >= 0 ? Player.Inventory[item] : Level.Items[0 - item];
            var oName = oPtr.Description(false, 0);
            var your = item >= 0 ? "Your" : "The";
            var s = oPtr.Count > 1 ? "" : "s";
            Profile.Instance.MsgPrint($"{your} {oName} radiate{s} a blinding light!");
            if (oPtr.ArtifactIndex != 0 || oPtr.IsLegendary())
            {
                var are = oPtr.Count > 1 ? "are" : "is";
                s = oPtr.Count > 1 ? "artifacts" : "an artifact";
                Profile.Instance.MsgPrint($"The {oName} {are} already {s}!");
                okay = false;
            }
            else if (oPtr.RareItemTypeIndex != 0)
            {
                var are = oPtr.Count > 1 ? "are" : "is";
                s = oPtr.Count > 1 ? "rare items" : "a rare item";
                Profile.Instance.MsgPrint($"The {oName} {are} already {s}!");
                okay = false;
            }
            else
            {
                if (oPtr.Count > 1)
                {
                    Profile.Instance.MsgPrint("Not enough enough energy to enchant more than one object!");
                    s = oPtr.Count > 2 ? "were" : "was";
                    Profile.Instance.MsgPrint($"{oPtr.Count - 1} of your oName {s} destroyed!");
                    oPtr.Count = 1;
                }
                okay = oPtr.CreateLegendary(true);
            }
            if (!okay)
            {
                Profile.Instance.MsgPrint("The enchantment failed.");
            }
        }

        public bool BanishEvil(int dist)
        {
            return ProjectAtAllInLos(new ProjectAwayEvil(this), dist);
        }

        public void BanishMonsters(int dist)
        {
            ProjectAtAllInLos(new ProjectAwayAll(this), dist);
        }

        public void BlessWeapon()
        {
            var f1 = new FlagSet();
            var f2 = new FlagSet();
            var f3 = new FlagSet();
            _saveGame.ItemFilter = ItemTesterHookWeapon;
            if (!_saveGame.GetItem(out var item, "Bless which weapon? ", true, true, true))
            {
                if (item == -2)
                {
                    Profile.Instance.MsgPrint("You have weapon to bless.");
                }
                return;
            }
            var oPtr = item >= 0 ? Player.Inventory[item] : Level.Items[0 - item];
            var oName = oPtr.Description(false, 0);
            oPtr.GetMergedFlags(f1, f2, f3);
            if (oPtr.IdentifyFlags.IsSet(Constants.IdentCursed))
            {
                string your;
                if ((f3.IsSet(ItemFlag3.HeavyCurse) && Program.Rng.DieRoll(100) < 33) ||
                    f3.IsSet(ItemFlag3.PermaCurse))
                {
                    your = item >= 0 ? "your" : "the";
                    Profile.Instance.MsgPrint($"The black aura on {your} {oName} disrupts the blessing!");
                    return;
                }
                your = item >= 0 ? "your" : "the";
                Profile.Instance.MsgPrint($"A malignant aura leaves {your} {oName}.");
                oPtr.IdentifyFlags.Clear(Constants.IdentCursed);
                oPtr.IdentifyFlags.Set(Constants.IdentSense);
                oPtr.Inscription = "uncursed";
                Player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            }
            if (f3.IsSet(ItemFlag3.Blessed))
            {
                var your = item >= 0 ? "your" : "the";
                var s = oPtr.Count > 1 ? "were" : "was";
                Profile.Instance.MsgPrint($"{your} {oName} {s} blessed already.");
                return;
            }
            if (!(oPtr.IsLegendary() || oPtr.ArtifactIndex != 0) ||
                Program.Rng.DieRoll(3) == 1)
            {
                var your = item >= 0 ? "your" : "the";
                var s = oPtr.Count > 1 ? "" : "s";
                Profile.Instance.MsgPrint($"{your} {oName} shine{s}!");
                oPtr.LegendaryFlags3.Set(ItemFlag3.Blessed);
            }
            else
            {
                var disHappened = false;
                var rarity = oPtr.IsLegendary() ? "legendary item" : "artifact";
                Profile.Instance.MsgPrint($"The {rarity} resists your blessing!");
                if (oPtr.BonusToHit > 0)
                {
                    oPtr.BonusToHit--;
                    disHappened = true;
                }
                if (oPtr.BonusToHit > 5 && Program.Rng.RandomLessThan(100) < 33)
                {
                    oPtr.BonusToHit--;
                }
                if (oPtr.BonusDamage > 0)
                {
                    oPtr.BonusDamage--;
                    disHappened = true;
                }
                if (oPtr.BonusDamage > 5 && Program.Rng.RandomLessThan(100) < 33)
                {
                    oPtr.BonusDamage--;
                }
                if (oPtr.BonusArmourClass > 0)
                {
                    oPtr.BonusArmourClass--;
                    disHappened = true;
                }
                if (oPtr.BonusArmourClass > 5 && Program.Rng.RandomLessThan(100) < 33)
                {
                    oPtr.BonusArmourClass--;
                }
                if (disHappened)
                {
                    Profile.Instance.MsgPrint("There is a  feeling in the air...");
                    var your = item >= 0 ? "your" : "the";
                    var s = oPtr.Count > 1 ? "were" : "was";
                    Profile.Instance.MsgPrint($"{your} {oName} {s} disenchanted!");
                }
            }
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
        }

        public void CallChaos()
        {
            var targetEngine = new TargetEngine(Player, Level);
            var plev = Player.Level;
            var lineChaos = false;
            IProjection[] hurtTypes =
            {
                new ProjectElectricity(this), new ProjectPoison(this), new ProjectAcid(this), new ProjectCold(this),
                new ProjectFire(this), new ProjectMissile(this), new ProjectArrow(this), new ProjectPlasma(this),
                new ProjectHolyFire(this), new ProjectWater(this), new ProjectLight(this), new ProjectDark(this),
                new ProjectForce(this), new ProjectInertia(this), new ProjectVis(this), new ProjectMeteor(this),
                new ProjectIce(this), new ProjectChaos(this), new ProjectNether(this), new ProjectDisenchant(this),
                new ProjectExplode(this), new ProjectSound(this), new ProjectNexus(this), new ProjectConfusion(this),
                new ProjectTime(this), new ProjectGravity(this), new ProjectShard(this), new ProjectNuke(this),
                new ProjectHellFire(this), new ProjectDisintegrate(this)
            };
            var chaosType = hurtTypes[Program.Rng.DieRoll(30) - 1];
            if (Program.Rng.DieRoll(4) == 1)
            {
                lineChaos = true;
            }
            if (Program.Rng.DieRoll(6) == 1)
            {
                for (var dummy = 1; dummy < 10; dummy++)
                {
                    if (dummy - 5 != 0)
                    {
                        if (lineChaos)
                        {
                            FireBeam(chaosType, dummy, 75);
                        }
                        else
                        {
                            FireBall(chaosType, dummy, 75, 2);
                        }
                    }
                }
            }
            else if (Program.Rng.DieRoll(3) == 1)
            {
                FireBall(chaosType, 0, 300, 8);
            }
            else
            {
                if (!targetEngine.GetDirectionWithAim(out var dir))
                {
                    return;
                }
                if (lineChaos)
                {
                    FireBeam(chaosType, dir, 150);
                }
                else
                {
                    FireBall(chaosType, dir, 150, 3 + (plev / 35));
                }
            }
        }

        public void Carnage(bool playerCast)
        {
            var msec = GlobalData.DelayFactor * GlobalData.DelayFactor * GlobalData.DelayFactor;
            Gui.GetCom("Choose a monster race (by symbol) to carnage: ", out var typ);
            for (var i = 1; i < Level.MMax; i++)
            {
                var mPtr = Level.Monsters[i];
                var rPtr = mPtr.Race;
                if (mPtr.Race == null)
                {
                    continue;
                }
                if ((rPtr.Flags1 & MonsterFlag1.Unique) != 0)
                {
                    continue;
                }
                if (rPtr.Character != typ)
                {
                    continue;
                }
                if ((rPtr.Flags1 & MonsterFlag1.Guardian) != 0)
                {
                    continue;
                }
                Level.Monsters.DeleteMonsterByIndex(i, true);
                if (playerCast)
                {
                    Player.TakeHit(Program.Rng.DieRoll(4), "the strain of casting Carnage");
                }
                Level.MoveCursorRelative(Player.MapY, Player.MapX);
                Player.RedrawNeeded.Set(RedrawFlag.PrHp);
                _saveGame.HandleStuff();
                Gui.Refresh();
                Gui.Pause(msec);
            }
        }

        public void CharmAnimal(int dir, int plev)
        {
            var flg = ProjectionFlag.ProjectStop | ProjectionFlag.ProjectKill;
            TargetedProject(new ProjectControlAnimal(this), dir, plev, flg);
        }

        public void CharmAnimals(int dam)
        {
            ProjectAtAllInLos(new ProjectControlAnimal(this), dam);
        }

        public bool CharmMonster(int dir, int plev)
        {
            var flg = ProjectionFlag.ProjectStop | ProjectionFlag.ProjectKill;
            return TargetedProject(new ProjectCharm(this), dir, plev, flg);
        }

        public void CharmMonsters(int dam)
        {
            ProjectAtAllInLos(new ProjectCharm(this), dam);
        }

        public bool CloneMonster(int dir)
        {
            var flg = ProjectionFlag.ProjectStop | ProjectionFlag.ProjectKill;
            return TargetedProject(new ProjectOldClone(this), dir, 0, flg);
        }

        public void ColdDam(int dam, string kbStr)
        {
            var inv = dam < 30 ? 1 : dam < 60 ? 2 : 3;
            if (Player.HasColdImmunity || dam <= 0)
            {
                return;
            }
            if (Player.HasElementalVulnerability)
            {
                dam *= 2;
            }
            if (Player.HasColdResistance)
            {
                dam = (dam + 2) / 3;
            }
            if (Player.TimedColdResistance != 0)
            {
                dam = (dam + 2) / 3;
            }
            if (!(Player.TimedColdResistance != 0 || Player.HasColdResistance) && Program.Rng.DieRoll(HurtChance) == 1)
            {
                Player.TryDecreasingAbilityScore(Ability.Strength);
            }
            Player.TakeHit(dam, kbStr);
            if (!(Player.HasColdResistance && Player.TimedColdResistance != 0))
            {
                Player.Inventory.InvenDamage(SetColdDestroy, inv);
            }
        }

        public bool ConfuseMonster(int dir, int plev)
        {
            var flg = ProjectionFlag.ProjectStop | ProjectionFlag.ProjectKill;
            return TargetedProject(new ProjectOldConf(this), dir, plev, flg);
        }

        public void ConfuseMonsters(int dam)
        {
            ProjectAtAllInLos(new ProjectOldConf(this), dam);
        }

        public void ControlOneUndead(int dir, int plev)
        {
            var flg = ProjectionFlag.ProjectStop | ProjectionFlag.ProjectKill;
            TargetedProject(new ProjectControlUndead(this), dir, plev, flg);
        }

        public void DeathRay(int dir, int plev)
        {
            var flg = ProjectionFlag.ProjectStop | ProjectionFlag.ProjectKill;
            TargetedProject(new ProjectDeathRay(this), dir, plev, flg);
        }

        public void DestroyArea(int y1, int x1, int r)
        {
            int y, x;
            var flag = false;
            for (y = y1 - r; y <= y1 + r; y++)
            {
                for (x = x1 - r; x <= x1 + r; x++)
                {
                    if (!Level.InBounds(y, x))
                    {
                        continue;
                    }
                    var k = Level.Distance(y1, x1, y, x);
                    if (k > r)
                    {
                        continue;
                    }
                    var cPtr = Level.Grid[y][x];
                    cPtr.TileFlags.Clear(GridTile.InRoom | GridTile.InVault);
                    cPtr.TileFlags.Clear(GridTile.PlayerMemorised | GridTile.SelfLit);
                    if (x == Player.MapX && y == Player.MapY)
                    {
                        flag = true;
                        continue;
                    }
                    if (y == y1 && x == x1)
                    {
                        continue;
                    }
                    Level.DeleteMonster(y, x);
                    if (Level.CaveValidBold(y, x))
                    {
                        Level.DeleteObject(y, x);
                        var t = Program.Rng.RandomLessThan(200);
                        if (t < 20)
                        {
                            cPtr.SetFeature("WallBasic");
                        }
                        else if (t < 70)
                        {
                            cPtr.SetFeature("Quartz");
                        }
                        else if (t < 100)
                        {
                            cPtr.SetFeature("Magma");
                        }
                        else
                        {
                            cPtr.RevertToBackground();
                        }
                    }
                }
            }
            if (flag)
            {
                Profile.Instance.MsgPrint("There is a searing blast of light!");
                if (!Player.HasBlindnessResistance && !Player.HasLightResistance)
                {
                    Player.SetTimedBlindness(Player.TimedBlindness + 10 + Program.Rng.DieRoll(10));
                }
            }
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateRemoveView | UpdateFlags.UpdateRemoveLight);
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateView | UpdateFlags.UpdateLight | UpdateFlags.UpdateScent);
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateMonsters);
            Player.RedrawNeeded.Set(RedrawFlag.PrMap);
        }

        public bool DestroyDoor(int dir)
        {
            var flg = ProjectionFlag.ProjectBeam | ProjectionFlag.ProjectGrid | ProjectionFlag.ProjectItem;
            return TargetedProject(new ProjectKillDoor(this), dir, 0, flg);
        }

        public bool DestroyDoorsTouch()
        {
            var flg = ProjectionFlag.ProjectGrid | ProjectionFlag.ProjectItem | ProjectionFlag.ProjectHide;
            return Project(0, 1, Player.MapY, Player.MapX, 0, new ProjectKillDoor(this), flg);
        }

        public bool DetectAll()
        {
            var detect = DetectTraps();
            detect |= DetectDoors();
            detect |= DetectStairs();
            detect |= DetectTreasure();
            detect |= DetectObjectsGold();
            detect |= DetectObjectsNormal();
            detect |= DetectMonstersInvis();
            detect |= DetectMonstersNormal();
            return detect;
        }

        public bool DetectDoors()
        {
            var detect = false;
            for (var y = Level.PanelRowMin; y <= Level.PanelRowMax; y++)
            {
                for (var x = Level.PanelColMin; x <= Level.PanelColMax; x++)
                {
                    var cPtr = Level.Grid[y][x];
                    if (cPtr.FeatureType.Category == FloorTileTypeCategory.SecretDoor)
                    {
                        Level.ReplaceSecretDoor(y, x);
                    }
                    if (cPtr.FeatureType.IsClosedDoor ||
                        cPtr.FeatureType.Category == FloorTileTypeCategory.OpenDoorway)
                    {
                        cPtr.TileFlags.Set(GridTile.PlayerMemorised);
                        Level.RedrawSingleLocation(y, x);
                        detect = true;
                    }
                }
            }
            if (detect)
            {
                Profile.Instance.MsgPrint("You sense the presence of doors!");
            }
            return detect;
        }

        public bool DetectMonstersEvil()
        {
            var flag = false;
            for (var i = 1; i < Level.MMax; i++)
            {
                var mPtr = Level.Monsters[i];
                var rPtr = mPtr.Race;
                if (mPtr.Race == null)
                {
                    continue;
                }
                var y = mPtr.MapY;
                var x = mPtr.MapX;
                if (!Level.PanelContains(y, x))
                {
                    continue;
                }
                if ((rPtr.Flags3 & MonsterFlag3.Evil) != 0)
                {
                    rPtr.Knowledge.RFlags3 |= MonsterFlag3.Evil;
                    Level.Monsters.RepairMonsters = true;
                    mPtr.IndividualMonsterFlags |= Constants.MflagMark | Constants.MflagShow;
                    mPtr.IsVisible = true;
                    Level.RedrawSingleLocation(y, x);
                    flag = true;
                }
            }
            if (flag)
            {
                Profile.Instance.MsgPrint("You sense the presence of evil creatures!");
            }
            return flag;
        }

        public bool DetectMonstersInvis()
        {
            var flag = false;
            for (var i = 1; i < Level.MMax; i++)
            {
                var mPtr = Level.Monsters[i];
                var rPtr = mPtr.Race;
                if (mPtr.Race == null)
                {
                    continue;
                }
                var y = mPtr.MapY;
                var x = mPtr.MapX;
                if (!Level.PanelContains(y, x))
                {
                    continue;
                }
                if ((rPtr.Flags2 & MonsterFlag2.Invisible) != 0)
                {
                    rPtr.Knowledge.RFlags2 |= MonsterFlag2.Invisible;
                    Level.Monsters.RepairMonsters = true;
                    mPtr.IndividualMonsterFlags |= Constants.MflagMark | Constants.MflagShow;
                    mPtr.IsVisible = true;
                    Level.RedrawSingleLocation(y, x);
                    flag = true;
                }
            }
            if (flag)
            {
                Profile.Instance.MsgPrint("You sense the presence of invisible creatures!");
            }
            return flag;
        }

        public void DetectMonstersNonliving()
        {
            var flag = false;
            for (var i = 1; i < Level.MMax; i++)
            {
                var mPtr = Level.Monsters[i];
                var rPtr = mPtr.Race;
                if (mPtr.Race == null)
                {
                    continue;
                }
                var y = mPtr.MapY;
                var x = mPtr.MapX;
                if (!Level.PanelContains(y, x))
                {
                    continue;
                }
                if ((rPtr.Flags3 & MonsterFlag3.Nonliving) != 0 || (rPtr.Flags3 & MonsterFlag3.Undead) != 0 ||
                    (rPtr.Flags3 & MonsterFlag3.Cthuloid) != 0 || (rPtr.Flags3 & MonsterFlag3.Demon) != 0)
                {
                    Level.Monsters.RepairMonsters = true;
                    mPtr.IndividualMonsterFlags |= Constants.MflagMark | Constants.MflagShow;
                    mPtr.IsVisible = true;
                    Level.RedrawSingleLocation(y, x);
                    flag = true;
                }
            }
            if (flag)
            {
                Profile.Instance.MsgPrint("You sense the presence of unnatural beings!");
            }
        }

        public bool DetectMonstersNormal()
        {
            var flag = false;
            for (var i = 1; i < Level.MMax; i++)
            {
                var mPtr = Level.Monsters[i];
                var rPtr = mPtr.Race;
                if (mPtr.Race == null)
                {
                    continue;
                }
                var y = mPtr.MapY;
                var x = mPtr.MapX;
                if (!Level.PanelContains(y, x))
                {
                    continue;
                }
                if ((rPtr.Flags2 & MonsterFlag2.Invisible) == 0 || Player.HasSeeInvisibility || Player.TimedSeeInvisibility != 0)
                {
                    Level.Monsters.RepairMonsters = true;
                    mPtr.IndividualMonsterFlags |= Constants.MflagMark | Constants.MflagShow;
                    mPtr.IsVisible = true;
                    Level.RedrawSingleLocation(y, x);
                    flag = true;
                }
            }
            if (flag)
            {
                Profile.Instance.MsgPrint("You sense the presence of monsters!");
            }
            return flag;
        }

        public bool DetectObjectsGold()
        {
            var detect = false;
            for (var i = 1; i < Level.OMax; i++)
            {
                var oPtr = Level.Items[i];
                if (oPtr.ItemType == null)
                {
                    continue;
                }
                if (oPtr.HoldingMonsterIndex != 0)
                {
                    continue;
                }
                var y = oPtr.Y;
                var x = oPtr.X;
                if (!Level.PanelContains(y, x))
                {
                    continue;
                }
                if (oPtr.Category == ItemCategory.Gold)
                {
                    oPtr.Marked = true;
                    Level.RedrawSingleLocation(y, x);
                    detect = true;
                }
            }
            if (detect)
            {
                Profile.Instance.MsgPrint("You sense the presence of treasure!");
            }
            if (DetectMonstersString("$*"))
            {
                detect = true;
            }
            return detect;
        }

        public void DetectObjectsMagic()
        {
            var detect = false;
            for (var i = 1; i < Level.OMax; i++)
            {
                var oPtr = Level.Items[i];
                if (oPtr.ItemType == null)
                {
                    continue;
                }
                if (oPtr.HoldingMonsterIndex != 0)
                {
                    continue;
                }
                var y = oPtr.Y;
                var x = oPtr.X;
                if (!Level.PanelContains(y, x))
                {
                    continue;
                }
                var tv = oPtr.Category;
                if (oPtr.IsArtifact() || oPtr.IsRare() || oPtr.IsLegendary() ||
                    tv == ItemCategory.Amulet || tv == ItemCategory.Ring || tv == ItemCategory.Staff ||
                    tv == ItemCategory.Wand || tv == ItemCategory.Rod || tv == ItemCategory.Scroll ||
                    tv == ItemCategory.Potion || tv == ItemCategory.LifeBook || tv == ItemCategory.SorceryBook ||
                    tv == ItemCategory.NatureBook || tv == ItemCategory.ChaosBook || tv == ItemCategory.DeathBook ||
                    tv == ItemCategory.CorporealBook || tv == ItemCategory.TarotBook || tv == ItemCategory.FolkBook ||
                    oPtr.BonusArmourClass > 0 || oPtr.BonusToHit + oPtr.BonusDamage > 0)
                {
                    oPtr.Marked = true;
                    Level.RedrawSingleLocation(y, x);
                    detect = true;
                }
            }
            if (detect)
            {
                Profile.Instance.MsgPrint("You sense the presence of magic objects!");
            }
        }

        public bool DetectObjectsNormal()
        {
            var detect = false;
            for (var i = 1; i < Level.OMax; i++)
            {
                var oPtr = Level.Items[i];
                if (oPtr.ItemType == null)
                {
                    continue;
                }
                if (oPtr.HoldingMonsterIndex != 0)
                {
                    continue;
                }
                var y = oPtr.Y;
                var x = oPtr.X;
                if (!Level.PanelContains(y, x))
                {
                    continue;
                }
                if (oPtr.Category != ItemCategory.Gold)
                {
                    oPtr.Marked = true;
                    Level.RedrawSingleLocation(y, x);
                    detect = true;
                }
            }
            if (detect)
            {
                Profile.Instance.MsgPrint("You sense the presence of objects!");
            }
            if (DetectMonstersString("!=?|"))
            {
                detect = true;
            }
            return detect;
        }

        public bool DetectStairs()
        {
            var detect = false;
            for (var y = Level.PanelRowMin; y <= Level.PanelRowMax; y++)
            {
                for (var x = Level.PanelColMin; x <= Level.PanelColMax; x++)
                {
                    var cPtr = Level.Grid[y][x];
                    if (cPtr.FeatureType.Category == FloorTileTypeCategory.UpStair || cPtr.FeatureType.Category == FloorTileTypeCategory.DownStair)
                    {
                        cPtr.TileFlags.Set(GridTile.PlayerMemorised);
                        Level.RedrawSingleLocation(y, x);
                        detect = true;
                    }
                }
            }
            if (detect)
            {
                Profile.Instance.MsgPrint("You sense the presence of stairs!");
            }
            return detect;
        }

        public bool DetectTraps()
        {
            var detect = false;
            for (var y = Level.PanelRowMin; y <= Level.PanelRowMax; y++)
            {
                for (var x = Level.PanelColMin; x <= Level.PanelColMax; x++)
                {
                    var cPtr = Level.Grid[y][x];
                    cPtr.TileFlags.Set(GridTile.TrapsDetected);
                    Level.RedrawSingleLocation(y, x);
                    if (cPtr.FeatureType.Category == FloorTileTypeCategory.UnidentifiedTrap)
                    {
                        Level.PickTrap(y, x);
                    }
                    if (cPtr.FeatureType.IsTrap)
                    {
                        cPtr.TileFlags.Set(GridTile.PlayerMemorised);
                        detect = true;
                    }
                }
            }
            Player.RedrawNeeded.Set(RedrawFlag.PrDtrap);
            Player.RedrawNeeded.Set(RedrawFlag.PrMap);
            if (detect)
            {
                Profile.Instance.MsgPrint("You sense the presence of traps!");
            }
            return detect;
        }

        public bool DetectTreasure()
        {
            var detect = false;
            for (var y = Level.PanelRowMin; y <= Level.PanelRowMax; y++)
            {
                for (var x = Level.PanelColMin; x <= Level.PanelColMax; x++)
                {
                    var cPtr = Level.Grid[y][x];
                    if (cPtr.FeatureType.Name.Contains("HidTreas"))
                    {
                        cPtr.SetFeature(cPtr.FeatureType.Name.Replace("Hid", "Vis"));
                    }
                    if (cPtr.FeatureType.Name.Contains("VisTreas"))
                    {
                        cPtr.TileFlags.Set(GridTile.PlayerMemorised);
                        Level.RedrawSingleLocation(y, x);
                        detect = true;
                    }
                }
            }
            if (detect)
            {
                Profile.Instance.MsgPrint("You sense the presence of buried treasure!");
            }
            return detect;
        }

        public bool DisarmTrap(int dir)
        {
            var flg = ProjectionFlag.ProjectBeam | ProjectionFlag.ProjectGrid | ProjectionFlag.ProjectItem;
            return TargetedProject(new ProjectKillTrap(this), dir, 0, flg);
        }

        public void DispelDemons(int dam)
        {
            ProjectAtAllInLos(new ProjectDispDemon(this), dam);
        }

        public bool DispelEvil(int dam)
        {
            return ProjectAtAllInLos(new ProjectDispEvil(this), dam);
        }

        public void DispelGood(int dam)
        {
            ProjectAtAllInLos(new ProjectDispGood(this), dam);
        }

        public void DispelLiving(int dam)
        {
            ProjectAtAllInLos(new ProjectDispLiving(this), dam);
        }

        public bool DispelMonsters(int dam)
        {
            return ProjectAtAllInLos(new ProjectDispAll(this), dam);
        }

        public bool DispelUndead(int dam)
        {
            return ProjectAtAllInLos(new ProjectDispUndead(this), dam);
        }

        public void DoorCreation()
        {
            var flg = ProjectionFlag.ProjectGrid | ProjectionFlag.ProjectItem | ProjectionFlag.ProjectHide;
            Project(0, 1, Player.MapY, Player.MapX, 0, new ProjectMakeDoor(this), flg);
        }

        public bool DrainLife(int dir, int dam)
        {
            var flg = ProjectionFlag.ProjectStop | ProjectionFlag.ProjectKill;
            return TargetedProject(new ProjectOldDrain(this), dir, dam, flg);
        }

        public void Earthquake(int cy, int cx, int r)
        {
            var targetEngine = new TargetEngine(Player, Level);
            int i, y, x, yy, xx, dy, dx;
            var damage = 0;
            int sn = 0, sy = 0, sx = 0;
            var hurt = false;
            GridTile cPtr;
            var map = new bool[32][];
            for (var j = 0; j < 32; j++)
            {
                map[j] = new bool[32];
            }
            if (r > 12)
            {
                r = 12;
            }
            for (y = 0; y < 32; y++)
            {
                for (x = 0; x < 32; x++)
                {
                    map[y][x] = false;
                }
            }
            for (dy = -r; dy <= r; dy++)
            {
                for (dx = -r; dx <= r; dx++)
                {
                    yy = cy + dy;
                    xx = cx + dx;
                    if (!Level.InBounds(yy, xx))
                    {
                        continue;
                    }
                    if (Level.Distance(cy, cx, yy, xx) > r)
                    {
                        continue;
                    }
                    cPtr = Level.Grid[yy][xx];
                    cPtr.TileFlags.Clear(GridTile.InRoom | GridTile.InVault);
                    cPtr.TileFlags.Clear(GridTile.SelfLit | GridTile.PlayerMemorised);
                    if (dx == 0 && dy == 0)
                    {
                        continue;
                    }
                    if (Program.Rng.RandomLessThan(100) < 85)
                    {
                        continue;
                    }
                    map[16 + yy - cy][16 + xx - cx] = true;
                    if (yy == Player.MapY && xx == Player.MapX)
                    {
                        hurt = true;
                    }
                }
            }
            if (hurt)
            {
                for (i = 0; i < 8; i++)
                {
                    y = Player.MapY + Level.KeypadDirectionYOffset[i];
                    x = Player.MapX + Level.KeypadDirectionXOffset[i];
                    if (!Level.GridPassableNoCreature(y, x))
                    {
                        continue;
                    }
                    if (map[16 + y - cy][16 + x - cx])
                    {
                        continue;
                    }
                    sn++;
                    if (Program.Rng.RandomLessThan(sn) > 0)
                    {
                        continue;
                    }
                    sy = y;
                    sx = x;
                }
                switch (Program.Rng.DieRoll(3))
                {
                    case 1:
                        {
                            Profile.Instance.MsgPrint("The Level.Grid ceiling collapses!");
                            break;
                        }
                    case 2:
                        {
                            Profile.Instance.MsgPrint("The Level.Grid floor twists in an unnatural way!");
                            break;
                        }
                    default:
                        {
                            Profile.Instance.MsgPrint("The Level.Grid quakes!  You are pummeled with debris!");
                            break;
                        }
                }
                if (sn == 0)
                {
                    Profile.Instance.MsgPrint("You are severely crushed!");
                    damage = 300;
                }
                else
                {
                    switch (Program.Rng.DieRoll(3))
                    {
                        case 1:
                            {
                                Profile.Instance.MsgPrint("You nimbly dodge the blast!");
                                damage = 0;
                                break;
                            }
                        case 2:
                            {
                                Profile.Instance.MsgPrint("You are bashed by rubble!");
                                damage = Program.Rng.DiceRoll(10, 4);
                                Player.SetTimedStun(Player.TimedStun + Program.Rng.DieRoll(50));
                                break;
                            }
                        case 3:
                            {
                                Profile.Instance.MsgPrint("You are crushed between the floor and ceiling!");
                                damage = Program.Rng.DiceRoll(10, 4);
                                Player.SetTimedStun(Player.TimedStun + Program.Rng.DieRoll(50));
                                break;
                            }
                    }
                    var oy = Player.MapY;
                    var ox = Player.MapX;
                    Player.MapY = sy;
                    Player.MapX = sx;
                    Level.RedrawSingleLocation(oy, ox);
                    Level.RedrawSingleLocation(Player.MapY, Player.MapX);
                    targetEngine.RecenterScreenAroundPlayer();
                }
                map[16 + Player.MapY - cy][16 + Player.MapX - cx] = false;
                if (damage != 0)
                {
                    Player.TakeHit(damage, "an earthquake");
                }
            }
            for (dy = -r; dy <= r; dy++)
            {
                for (dx = -r; dx <= r; dx++)
                {
                    yy = cy + dy;
                    xx = cx + dx;
                    if (!map[16 + yy - cy][16 + xx - cx])
                    {
                        continue;
                    }
                    cPtr = Level.Grid[yy][xx];
                    if (cPtr.MonsterIndex != 0)
                    {
                        var mPtr = Level.Monsters[cPtr.MonsterIndex];
                        var rPtr = mPtr.Race;
                        if ((rPtr.Flags2 & MonsterFlag2.KillWall) == 0 && (rPtr.Flags2 & MonsterFlag2.PassWall) == 0)
                        {
                            sn = 0;
                            if ((rPtr.Flags1 & MonsterFlag1.NeverMove) == 0)
                            {
                                for (i = 0; i < 8; i++)
                                {
                                    y = yy + Level.KeypadDirectionYOffset[i];
                                    x = xx + Level.KeypadDirectionXOffset[i];
                                    if (!Level.GridPassableNoCreature(y, x))
                                    {
                                        continue;
                                    }
                                    if (Level.Grid[y][x].FeatureType.Name == "ElderSign")
                                    {
                                        continue;
                                    }
                                    if (Level.Grid[y][x].FeatureType.Name == "YellowSign")
                                    {
                                        continue;
                                    }
                                    if (map[16 + y - cy][16 + x - cx])
                                    {
                                        continue;
                                    }
                                    sn++;
                                    if (Program.Rng.RandomLessThan(sn) > 0)
                                    {
                                        continue;
                                    }
                                    sy = y;
                                    sx = x;
                                }
                            }
                            var mName = mPtr.MonsterDesc(0);
                            Profile.Instance.MsgPrint($"{mName} wails out in pain!");
                            damage = sn != 0 ? Program.Rng.DiceRoll(4, 8) : 200;
                            mPtr.SleepLevel = 0;
                            mPtr.Health -= damage;
                            if (mPtr.Health < 0)
                            {
                                Profile.Instance.MsgPrint($"{mName} is embedded in the rock!");
                                Level.DeleteMonster(yy, xx);
                                sn = 0;
                            }
                            if (sn != 0)
                            {
                                var mIdx = Level.Grid[yy][xx].MonsterIndex;
                                Level.Grid[sy][sx].MonsterIndex = mIdx;
                                Level.Grid[yy][xx].MonsterIndex = 0;
                                mPtr.MapY = sy;
                                mPtr.MapX = sx;
                                Level.Monsters.UpdateMonsterVisibility(mIdx, true);
                                Level.RedrawSingleLocation(yy, xx);
                                Level.RedrawSingleLocation(sy, sx);
                            }
                        }
                    }
                }
            }
            for (dy = -r; dy <= r; dy++)
            {
                for (dx = -r; dx <= r; dx++)
                {
                    yy = cy + dy;
                    xx = cx + dx;
                    if (!map[16 + yy - cy][16 + xx - cx])
                    {
                        continue;
                    }
                    cPtr = Level.Grid[yy][xx];
                    if (yy == Player.MapY && xx == Player.MapX)
                    {
                        continue;
                    }
                    if (Level.CaveValidBold(yy, xx))
                    {
                        var floor = Level.GridPassable(yy, xx);
                        Level.DeleteObject(yy, xx);
                        var t = floor ? Program.Rng.RandomLessThan(100) : 200;
                        if (t < 20)
                        {
                            cPtr.SetFeature("WallBasic");
                        }
                        else if (t < 70)
                        {
                            cPtr.SetFeature("Quartz");
                        }
                        else if (t < 100)
                        {
                            cPtr.SetFeature("Magma");
                        }
                        else
                        {
                            cPtr.RevertToBackground();
                        }
                    }
                }
            }
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateRemoveView | UpdateFlags.UpdateRemoveLight);
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateView | UpdateFlags.UpdateLight | UpdateFlags.UpdateScent);
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateDistances);
            Player.RedrawNeeded.Set(RedrawFlag.PrHealth);
            Player.RedrawNeeded.Set(RedrawFlag.PrMap);
        }

        public void ElderSign()
        {
            if (!Level.GridOpenNoItem(Player.MapY, Player.MapX))
            {
                Profile.Instance.MsgPrint("The object resists the spell.");
                return;
            }
            Level.CaveSetFeat(Player.MapY, Player.MapX, "ElderSign");
        }

        public void ElderSignCreation()
        {
            var flg = ProjectionFlag.ProjectGrid | ProjectionFlag.ProjectItem;
            Project(0, 1, Player.MapY, Player.MapX, 0, new ProjectMakeElderSign(this), flg);
        }

        public void ElecDam(int dam, string kbStr)
        {
            var inv = dam < 30 ? 1 : dam < 60 ? 2 : 3;
            if (Player.HasLightningImmunity || dam <= 0)
            {
                return;
            }
            if (Player.HasElementalVulnerability)
            {
                dam *= 2;
            }
            if (Player.TimedLightningResistance != 0)
            {
                dam = (dam + 2) / 3;
            }
            if (Player.HasLightningResistance)
            {
                dam = (dam + 2) / 3;
            }
            if (!(Player.TimedLightningResistance != 0 || Player.HasLightningResistance) && Program.Rng.DieRoll(HurtChance) == 1)
            {
                Player.TryDecreasingAbilityScore(Ability.Dexterity);
            }
            Player.TakeHit(dam, kbStr);
            if (!(Player.TimedLightningResistance != 0 && Player.HasLightningResistance))
            {
                Player.Inventory.InvenDamage(SetElecDestroy, inv);
            }
        }

        public bool Enchant(Item oPtr, int n, int eflag)
        {
            var res = false;
            var a = oPtr.IsArtifact() || oPtr.IsLegendary();
            var f1 = new FlagSet();
            var f2 = new FlagSet();
            var f3 = new FlagSet();
            oPtr.GetMergedFlags(f1, f2, f3);
            var prob = oPtr.Count * 100;
            if (oPtr.Category == ItemCategory.Bolt || oPtr.Category == ItemCategory.Arrow ||
                oPtr.Category == ItemCategory.Shot)
            {
                prob /= 20;
            }
            for (var i = 0; i < n; i++)
            {
                if (Program.Rng.RandomLessThan(prob) >= 100)
                {
                    continue;
                }
                int chance;
                if ((eflag & Constants.EnchTohit) != 0)
                {
                    if (oPtr.BonusToHit < 0)
                    {
                        chance = 0;
                    }
                    else if (oPtr.BonusToHit > 15)
                    {
                        chance = 1000;
                    }
                    else
                    {
                        chance = GlobalData.EnchantTable[oPtr.BonusToHit];
                    }
                    if (Program.Rng.DieRoll(1000) > chance && (!a || Program.Rng.RandomLessThan(100) < 50))
                    {
                        oPtr.BonusToHit++;
                        res = true;
                        if (oPtr.IsCursed() && f3.IsClear(ItemFlag3.PermaCurse) && oPtr.BonusToHit >= 0 &&
                            Program.Rng.RandomLessThan(100) < 25)
                        {
                            Profile.Instance.MsgPrint("The curse is broken!");
                            oPtr.IdentifyFlags.Clear(Constants.IdentCursed);
                            oPtr.IdentifyFlags.Set(Constants.IdentSense);
                            if (oPtr.LegendaryFlags3.IsSet(ItemFlag3.Cursed))
                            {
                                oPtr.LegendaryFlags3.Clear(ItemFlag3.Cursed);
                            }
                            if (oPtr.LegendaryFlags3.IsSet(ItemFlag3.HeavyCurse))
                            {
                                oPtr.LegendaryFlags3.Clear(ItemFlag3.HeavyCurse);
                            }
                            oPtr.Inscription = "uncursed";
                        }
                    }
                }
                if ((eflag & Constants.EnchTodam) != 0)
                {
                    if (oPtr.BonusDamage < 0)
                    {
                        chance = 0;
                    }
                    else if (oPtr.BonusDamage > 15)
                    {
                        chance = 1000;
                    }
                    else
                    {
                        chance = GlobalData.EnchantTable[oPtr.BonusDamage];
                    }
                    if (Program.Rng.DieRoll(1000) > chance && (!a || Program.Rng.RandomLessThan(100) < 50))
                    {
                        oPtr.BonusDamage++;
                        res = true;
                        if (oPtr.IsCursed() && f3.IsClear(ItemFlag3.PermaCurse) && oPtr.BonusDamage >= 0 &&
                            Program.Rng.RandomLessThan(100) < 25)
                        {
                            Profile.Instance.MsgPrint("The curse is broken!");
                            oPtr.IdentifyFlags.Clear(Constants.IdentCursed);
                            oPtr.IdentifyFlags.Set(Constants.IdentSense);
                            if (oPtr.LegendaryFlags3.IsSet(ItemFlag3.Cursed))
                            {
                                oPtr.LegendaryFlags3.Clear(ItemFlag3.Cursed);
                            }
                            if (oPtr.LegendaryFlags3.IsSet(ItemFlag3.HeavyCurse))
                            {
                                oPtr.LegendaryFlags3.Clear(ItemFlag3.HeavyCurse);
                            }
                            oPtr.Inscription = "uncursed";
                        }
                    }
                }
                if ((eflag & Constants.EnchToac) != 0)
                {
                    if (oPtr.BonusArmourClass < 0)
                    {
                        chance = 0;
                    }
                    else if (oPtr.BonusArmourClass > 15)
                    {
                        chance = 1000;
                    }
                    else
                    {
                        chance = GlobalData.EnchantTable[oPtr.BonusArmourClass];
                    }
                    if (Program.Rng.DieRoll(1000) > chance && (!a || Program.Rng.RandomLessThan(100) < 50))
                    {
                        oPtr.BonusArmourClass++;
                        res = true;
                        if (oPtr.IsCursed() && f3.IsClear(ItemFlag3.PermaCurse) && oPtr.BonusArmourClass >= 0 &&
                            Program.Rng.RandomLessThan(100) < 25)
                        {
                            Profile.Instance.MsgPrint("The curse is broken!");
                            oPtr.IdentifyFlags.Clear(Constants.IdentCursed);
                            oPtr.IdentifyFlags.Set(Constants.IdentSense);
                            if (oPtr.LegendaryFlags3.IsSet(ItemFlag3.Cursed))
                            {
                                oPtr.LegendaryFlags3.Clear(ItemFlag3.Cursed);
                            }
                            if (oPtr.LegendaryFlags3.IsSet(ItemFlag3.HeavyCurse))
                            {
                                oPtr.LegendaryFlags3.Clear(ItemFlag3.HeavyCurse);
                            }
                            oPtr.Inscription = "uncursed";
                        }
                    }
                }
            }
            if (!res)
            {
                return false;
            }
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            Player.NoticeFlags |= Constants.PnCombine | Constants.PnReorder;
            return true;
        }

        public bool EnchantSpell(int numHit, int numDam, int numAc)
        {
            var okay = false;
            _saveGame.ItemFilter = ItemTesterHookWeapon;
            if (numAc != 0)
            {
                _saveGame.ItemFilter = ItemTesterHookArmour;
            }
            if (!_saveGame.GetItem(out var item, "Enchant which item? ", true, true, true))
            {
                if (item == -2)
                {
                    Profile.Instance.MsgPrint("You have nothing to enchant.");
                }
                return false;
            }
            var oPtr = item >= 0 ? Player.Inventory[item] : Level.Items[0 - item];
            var oName = oPtr.Description(false, 0);
            var your = item >= 0 ? "Your" : "The";
            var s = oPtr.Count > 1 ? "" : "s";
            Profile.Instance.MsgPrint($"{your} {oName} glow{s} brightly!");
            if (Enchant(oPtr, numHit, Constants.EnchTohit))
            {
                okay = true;
            }
            if (Enchant(oPtr, numDam, Constants.EnchTodam))
            {
                okay = true;
            }
            if (Enchant(oPtr, numAc, Constants.EnchToac))
            {
                okay = true;
            }
            if (!okay)
            {
                Profile.Instance.MsgPrint("The enchantment failed.");
            }
            return true;
        }

        public bool FearMonster(int dir, int plev)
        {
            var flg = ProjectionFlag.ProjectStop | ProjectionFlag.ProjectKill;
            return TargetedProject(new ProjectTurnAll(this), dir, plev, flg);
        }

        public bool FireBall(IProjection projectile, int dir, int dam, int rad)
        {
            var targetEngine = new TargetEngine(Player, Level);
            var flg = ProjectionFlag.ProjectStop | ProjectionFlag.ProjectGrid | ProjectionFlag.ProjectItem |
                      ProjectionFlag.ProjectKill;
            var tx = Player.MapX + (99 * Level.KeypadDirectionXOffset[dir]);
            var ty = Player.MapY + (99 * Level.KeypadDirectionYOffset[dir]);
            if (dir == 5 && targetEngine.TargetOkay())
            {
                flg &= ~ProjectionFlag.ProjectStop;
                tx = _saveGame.TargetCol;
                ty = _saveGame.TargetRow;
            }
            return Project(0, rad, ty, tx, dam, projectile, flg);
        }

        public void FireBeam(IProjection projectile, int dir, int dam)
        {
            var flg = ProjectionFlag.ProjectBeam | ProjectionFlag.ProjectKill;
            TargetedProject(projectile, dir, dam, flg);
        }

        public void FireBolt(IProjection projectile, int dir, int dam)
        {
            var flg = ProjectionFlag.ProjectStop | ProjectionFlag.ProjectKill;
            TargetedProject(projectile, dir, dam, flg);
        }

        public void FireBoltOrBeam(int prob, IProjection projectile, int dir, int dam)
        {
            if (Program.Rng.RandomLessThan(100) < prob)
            {
                FireBeam(projectile, dir, dam);
            }
            else
            {
                FireBolt(projectile, dir, dam);
            }
        }

        public void FireDam(int dam, string kbStr)
        {
            var inv = dam < 30 ? 1 : dam < 60 ? 2 : 3;
            if (Player.HasFireImmunity || dam <= 0)
            {
                return;
            }
            if (Player.HasElementalVulnerability)
            {
                dam *= 2;
            }
            if (Player.HasFireResistance)
            {
                dam = (dam + 2) / 3;
            }
            if (Player.TimedFireResistance != 0)
            {
                dam = (dam + 2) / 3;
            }
            if (!(Player.TimedFireResistance != 0 || Player.HasFireResistance) && Program.Rng.DieRoll(HurtChance) == 1)
            {
                Player.TryDecreasingAbilityScore(Ability.Strength);
            }
            Player.TakeHit(dam, kbStr);
            if (!(Player.HasFireResistance && Player.TimedFireResistance != 0))
            {
                Player.Inventory.InvenDamage(SetFireDestroy, inv);
            }
        }

        public bool HasteMonsters()
        {
            return ProjectAtAllInLos(new ProjectOldSpeed(this), Player.Level);
        }

        public bool HealMonster(int dir)
        {
            var flg = ProjectionFlag.ProjectStop | ProjectionFlag.ProjectKill;
            return TargetedProject(new ProjectOldHeal(this), dir, Program.Rng.DiceRoll(4, 6), flg);
        }

        public bool IdentifyFully()
        {
            if (!_saveGame.GetItem(out var item, "Identify which item? ", true, true, true))
            {
                if (item == -2)
                {
                    Profile.Instance.MsgPrint("You have nothing to identify.");
                }
                return false;
            }
            var oPtr = item >= 0 ? Player.Inventory[item] : Level.Items[0 - item];
            oPtr.BecomeFlavourAware();
            oPtr.BecomeKnown();
            oPtr.IdentifyFlags.Set(Constants.IdentMental);
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            Player.NoticeFlags |= Constants.PnCombine | Constants.PnReorder;
            _saveGame.HandleStuff();
            var oName = oPtr.Description(true, 3);
            if (item >= InventorySlot.MeleeWeapon)
            {
                Profile.Instance.MsgPrint($"{Player.DescribeWieldLocation(item)}: {oName} ({item.IndexToLabel()}).");
                if (oPtr.Stompable())
                {
                    var itemName = oPtr.Description(true, 3);
                    Profile.Instance.MsgPrint($"You destroy {oName}.");
                    var amount = oPtr.Count;
                    Player.Inventory.InvenItemIncrease(item, -amount);
                    Player.Inventory.InvenItemOptimize(item);
                }
            }
            else if (item >= 0)
            {
                Profile.Instance.MsgPrint($"In your pack: {oName} ({item.IndexToLabel()}).");
                if (oPtr.Stompable())
                {
                    Profile.Instance.MsgPrint($"You destroy {oName}.");
                    var amount = oPtr.Count;
                    Player.Inventory.InvenItemIncrease(item, -amount);
                    Player.Inventory.InvenItemOptimize(item);
                }
            }
            else
            {
                Profile.Instance.MsgPrint($"On the ground: {oName}.");
            }
            oPtr.IdentifyFully();
            return true;
        }

        public bool IdentifyItem()
        {
            if (!_saveGame.GetItem(out var item, "Identify which item? ", true, true, true))
            {
                if (item == -2)
                {
                    Profile.Instance.MsgPrint("You have nothing to identify.");
                }
                return false;
            }
            var oPtr = item >= 0 ? Player.Inventory[item] : Level.Items[0 - item];
            oPtr.BecomeFlavourAware();
            oPtr.BecomeKnown();
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            Player.NoticeFlags |= Constants.PnCombine | Constants.PnReorder;
            var oName = oPtr.Description(true, 3);
            if (item >= InventorySlot.MeleeWeapon)
            {
                Profile.Instance.MsgPrint($"{Player.DescribeWieldLocation(item)}: {oName} ({item.IndexToLabel()}).");
                if (oPtr.Stompable())
                {
                    Profile.Instance.MsgPrint($"You destroy {oName}.");
                    var amount = oPtr.Count;
                    Player.Inventory.InvenItemIncrease(item, -amount);
                    Player.Inventory.InvenItemOptimize(item);
                }
            }
            else if (item >= 0)
            {
                Profile.Instance.MsgPrint($"In your pack: {oName} ({item.IndexToLabel()}).");
                if (oPtr.Stompable())
                {
                    Profile.Instance.MsgPrint($"You destroy {oName}.");
                    var amount = oPtr.Count;
                    Player.Inventory.InvenItemIncrease(item, -amount);
                    Player.Inventory.InvenItemOptimize(item);
                }
            }
            else
            {
                Profile.Instance.MsgPrint($"On the ground: {oName}.");
            }
            return true;
        }

        public void IdentifyPack()
        {
            for (var i = 0; i < InventorySlot.Total; i++)
            {
                var oPtr = Player.Inventory[i];
                if (oPtr.ItemType == null)
                {
                    continue;
                }
                oPtr.BecomeFlavourAware();
                oPtr.BecomeKnown();
                if (oPtr.Stompable())
                {
                    var itemName = oPtr.Description(true, 3);
                    Profile.Instance.MsgPrint($"You destroy {itemName}.");
                    var amount = oPtr.Count;
                    Player.Inventory.InvenItemIncrease(i, -amount);
                    Player.Inventory.InvenItemOptimize(i);
                    i--;
                }
            }
        }

        public bool ItemTesterHookArmour(Item oPtr)
        {
            switch (oPtr.Category)
            {
                case ItemCategory.DragArmor:
                case ItemCategory.HardArmor:
                case ItemCategory.SoftArmor:
                case ItemCategory.Shield:
                case ItemCategory.Cloak:
                case ItemCategory.Crown:
                case ItemCategory.Helm:
                case ItemCategory.Boots:
                case ItemCategory.Gloves:
                    {
                        return true;
                    }
            }
            return false;
        }

        public bool ItemTesterHookRecharge(Item oPtr)
        {
            if (oPtr.Category == ItemCategory.Staff)
            {
                return true;
            }
            if (oPtr.Category == ItemCategory.Wand)
            {
                return true;
            }
            if (oPtr.Category == ItemCategory.Rod)
            {
                return true;
            }
            return false;
        }

        public bool LightArea(int dam, int rad)
        {
            var flg = ProjectionFlag.ProjectGrid | ProjectionFlag.ProjectKill;
            if (Player.TimedBlindness == 0)
            {
                Profile.Instance.MsgPrint("You are surrounded by a white light.");
            }
            Project(0, rad, Player.MapY, Player.MapX, dam, new ProjectLightWeak(this), flg);
            LightRoom(Player.MapY, Player.MapX);
            return true;
        }

        public void LightLine(int dir)
        {
            var flg = ProjectionFlag.ProjectBeam | ProjectionFlag.ProjectGrid | ProjectionFlag.ProjectKill;
            TargetedProject(new ProjectLightWeak(this), dir, Program.Rng.DiceRoll(6, 8), flg);
        }

        public bool LoseAllInfo()
        {
            for (var i = 0; i < InventorySlot.Total; i++)
            {
                var oPtr = Player.Inventory[i];
                if (oPtr.ItemType == null)
                {
                    continue;
                }
                if (oPtr.IdentifyFlags.IsSet(Constants.IdentMental))
                {
                    continue;
                }
                if (string.IsNullOrEmpty(oPtr.Inscription) == false && oPtr.IdentifyFlags.IsSet(Constants.IdentSense))
                {
                    var q = oPtr.Inscription;
                    if (q == "cursed" || q == "broken" || q == "good" || q == "average" || q == "excellent" ||
                        q == "worthless" || q == "special" || q == "terrible")
                    {
                        oPtr.Inscription = string.Empty;
                    }
                }
                oPtr.IdentifyFlags.Clear(Constants.IdentEmpty);
                oPtr.IdentifyFlags.Clear(Constants.IdentKnown);
                oPtr.IdentifyFlags.Clear(Constants.IdentSense);
            }
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            Player.NoticeFlags |= Constants.PnCombine | Constants.PnReorder;
            Level.WizDark();
            return true;
        }

        public void MassCarnage(bool playerCast)
        {
            var msec = GlobalData.DelayFactor * GlobalData.DelayFactor * GlobalData.DelayFactor;
            for (var i = 1; i < Level.MMax; i++)
            {
                var mPtr = Level.Monsters[i];
                var rPtr = mPtr.Race;
                if (mPtr.Race == null)
                {
                    continue;
                }
                if ((rPtr.Flags1 & MonsterFlag1.Unique) != 0)
                {
                    continue;
                }
                if ((rPtr.Flags1 & MonsterFlag1.Guardian) != 0)
                {
                    continue;
                }
                if (mPtr.DistanceFromPlayer > Constants.MaxSight)
                {
                    continue;
                }
                Level.Monsters.DeleteMonsterByIndex(i, true);
                if (playerCast)
                {
                    Player.TakeHit(Program.Rng.DieRoll(3), "the strain of casting Mass Carnage");
                }
                Level.MoveCursorRelative(Player.MapY, Player.MapX);
                Player.RedrawNeeded.Set(RedrawFlag.PrHp);
                _saveGame.HandleStuff();
                Gui.Refresh();
                Gui.Pause(msec);
            }
        }

        public void MindblastMonsters(int dam)
        {
            ProjectAtAllInLos(new ProjectPsi(this), dam);
        }

        public bool PolyMonster(int dir)
        {
            var flg = ProjectionFlag.ProjectStop | ProjectionFlag.ProjectKill;
            return TargetedProject(new ProjectOldPoly(this), dir, Player.Level, flg);
        }

        public int PolymorphMonster(MonsterRace rPtr)
        {
            var index = rPtr.Index;
            if ((rPtr.Flags1 & MonsterFlag1.Unique) != 0 || (rPtr.Flags1 & MonsterFlag1.Guardian) != 0)
            {
                return rPtr.Index;
            }
            var lev1 = rPtr.Level - ((Program.Rng.DieRoll(20) / Program.Rng.DieRoll(9)) + 1);
            var lev2 = rPtr.Level + (Program.Rng.DieRoll(20) / Program.Rng.DieRoll(9)) + 1;
            for (var i = 0; i < 1000; i++)
            {
                var r = Level.Monsters.GetMonNum(((_saveGame.Difficulty + rPtr.Level) / 2) + 5);
                if (r == 0)
                {
                    break;
                }
                rPtr = Profile.Instance.MonsterRaces[r];
                if ((rPtr.Flags1 & MonsterFlag1.Unique) != 0)
                {
                    continue;
                }
                if (rPtr.Level < lev1 || rPtr.Level > lev2)
                {
                    continue;
                }
                index = r;
                break;
            }
            return index;
        }

        public bool PotionSmashEffect(int who, int y, int x, int oSval)
        {
            var radius = 2;
            IProjection dt = null;
            var dam = 0;
            var angry = false;
            switch (oSval)
            {
                case PotionType.SaltWater:
                case PotionType.SlimeMold:
                case PotionType.LoseMemories:
                case PotionType.DecStr:
                case PotionType.DecInt:
                case PotionType.DecWis:
                case PotionType.DecDex:
                case PotionType.DecCon:
                case PotionType.DecCha:
                case PotionType.Water:
                case PotionType.AppleJuice:
                    return true;

                case PotionType.Infravision:
                case PotionType.DetectInvis:
                case PotionType.SlowPoison:
                case PotionType.CurePoison:
                case PotionType.Boldness:
                case PotionType.ResistHeat:
                case PotionType.ResistCold:
                case PotionType.Heroism:
                case PotionType.BeserkStrength:
                case PotionType.RestoreExp:
                case PotionType.ResStr:
                case PotionType.ResInt:
                case PotionType.ResWis:
                case PotionType.ResDex:
                case PotionType.ResCon:
                case PotionType.ResCha:
                case PotionType.IncStr:
                case PotionType.IncInt:
                case PotionType.IncWis:
                case PotionType.IncDex:
                case PotionType.IncCon:
                case PotionType.IncCha:
                case PotionType.Augmentation:
                case PotionType.Enlightenment:
                case PotionType.StarEnlightenment:
                case PotionType.SelfKnowledge:
                case PotionType.Experience:
                case PotionType.Resistance:
                case PotionType.Invulnerability:
                case PotionType.NewLife:
                    return false;

                case PotionType.Slowness:
                    dt = new ProjectOldSlow(this);
                    dam = 5;
                    angry = true;
                    break;

                case PotionType.Poison:
                    dt = new ProjectPoison(this);
                    dam = 3;
                    angry = true;
                    break;

                case PotionType.Blindness:
                    dt = new ProjectDark(this);
                    angry = true;
                    break;

                case PotionType.Confusion:
                    dt = new ProjectOldConf(this);
                    angry = true;
                    break;

                case PotionType.Sleep:
                    dt = new ProjectOldSleep(this);
                    angry = true;
                    break;

                case PotionType.Ruination:
                case PotionType.Detonations:
                    dt = new ProjectExplode(this);
                    dam = Program.Rng.DiceRoll(25, 25);
                    angry = true;
                    break;

                case PotionType.Death:
                    dt = new ProjectDeathRay(this);
                    angry = true;
                    radius = 1;
                    break;

                case PotionType.Speed:
                    dt = new ProjectOldSpeed(this);
                    break;

                case PotionType.CureLight:
                    dt = new ProjectOldHeal(this);
                    dam = Program.Rng.DiceRoll(2, 3);
                    break;

                case PotionType.CureSerious:
                    dt = new ProjectOldHeal(this);
                    dam = Program.Rng.DiceRoll(4, 3);
                    break;

                case PotionType.CureCritical:
                case PotionType.Curing:
                    dt = new ProjectOldHeal(this);
                    dam = Program.Rng.DiceRoll(6, 3);
                    break;

                case PotionType.Healing:
                    dt = new ProjectOldHeal(this);
                    dam = Program.Rng.DiceRoll(10, 10);
                    break;

                case PotionType.StarHealing:
                case PotionType.Life:
                    dt = new ProjectOldHeal(this);
                    dam = Program.Rng.DiceRoll(50, 50);
                    radius = 1;
                    break;

                case PotionType.RestoreVis:
                    dt = new ProjectVis(this);
                    dam = Program.Rng.DiceRoll(10, 10);
                    radius = 1;
                    break;
            }
            Project(who, radius, y, x, dam, dt,
                ProjectionFlag.ProjectJump | ProjectionFlag.ProjectItem | ProjectionFlag.ProjectKill);
            return angry;
        }

        public void Probing()
        {
            var probe = false;
            for (var i = 1; i < Level.MMax; i++)
            {
                var mPtr = Level.Monsters[i];
                if (mPtr.Race == null)
                {
                    continue;
                }
                if (!Level.PlayerHasLosBold(mPtr.MapY, mPtr.MapX))
                {
                    continue;
                }
                if (mPtr.IsVisible)
                {
                    if (!probe)
                    {
                        Profile.Instance.MsgPrint("Probing...");
                    }
                    var mName = mPtr.MonsterDesc(0x04);
                    Profile.Instance.MsgPrint($"{mName} has {mPtr.Health} hit points.");
                    Level.Monsters.LoreDoProbe(i);
                    probe = true;
                }
            }
            if (probe)
            {
                Profile.Instance.MsgPrint("That's all.");
            }
        }

        public bool Project(int who, int rad, int y, int x, int dam, IProjection projectile, ProjectionFlag flg)
        {
            return projectile.Fire(who, rad, y, x, dam, flg);
        }

        public bool Recharge(int num)
        {
            int i, t;
            _saveGame.ItemFilter = ItemTesterHookRecharge;
            if (!_saveGame.GetItem(out var item, "Recharge which item? ", false, true, true))
            {
                if (item == -2)
                {
                    Profile.Instance.MsgPrint("You have nothing to recharge.");
                }
                return false;
            }
            var oPtr = item >= 0 ? Player.Inventory[item] : Level.Items[0 - item];
            var lev = oPtr.ItemType.Level;
            if (oPtr.Category == ItemCategory.Rod)
            {
                i = (100 - lev + num) / 5;
                if (i < 1)
                {
                    i = 1;
                }
                if (Program.Rng.RandomLessThan(i) == 0)
                {
                    Profile.Instance.MsgPrint("The recharge backfires, draining the rod further!");
                    if (oPtr.TypeSpecificValue < 10000)
                    {
                        oPtr.TypeSpecificValue = (oPtr.TypeSpecificValue + 100) * 2;
                    }
                }
                else
                {
                    t = num * Program.Rng.DiceRoll(2, 4);
                    if (oPtr.TypeSpecificValue > t)
                    {
                        oPtr.TypeSpecificValue -= t;
                    }
                    else
                    {
                        oPtr.TypeSpecificValue = 0;
                    }
                }
            }
            else
            {
                i = (num + 100 - lev - (10 * oPtr.TypeSpecificValue)) / 15;
                if (i < 1)
                {
                    i = 1;
                }
                if (Program.Rng.RandomLessThan(i) == 0)
                {
                    Profile.Instance.MsgPrint("There is a bright flash of light.");
                    if (item >= 0)
                    {
                        Player.Inventory.InvenItemIncrease(item, -999);
                        Player.Inventory.InvenItemDescribe(item);
                        Player.Inventory.InvenItemOptimize(item);
                    }
                    else
                    {
                        Level.FloorItemIncrease(0 - item, -999);
                        Level.FloorItemDescribe(0 - item);
                        Level.FloorItemOptimize(0 - item);
                    }
                }
                else
                {
                    t = (num / (lev + 2)) + 1;
                    if (t > 0)
                    {
                        oPtr.TypeSpecificValue += 2 + Program.Rng.DieRoll(t);
                    }
                    oPtr.IdentifyFlags.Clear(Constants.IdentKnown);
                    oPtr.IdentifyFlags.Clear(Constants.IdentEmpty);
                }
            }
            Player.NoticeFlags |= Constants.PnCombine | Constants.PnReorder;
            return true;
        }

        public void RemoveAllCurse()
        {
            RemoveCurseAux(true);
        }

        public bool RemoveCurse()
        {
            return RemoveCurseAux(false);
        }

        public void ReportMagics()
        {
            int i = 0, j, k;
            var info = new string[128];
            var info2 = new int[128];
            if (Player.TimedBlindness != 0)
            {
                info2[i] = ReportMagicsAux(Player.TimedBlindness);
                info[i++] = "You cannot see";
            }
            if (Player.TimedConfusion != 0)
            {
                info2[i] = ReportMagicsAux(Player.TimedConfusion);
                info[i++] = "You are confused";
            }
            if (Player.TimedFear != 0)
            {
                info2[i] = ReportMagicsAux(Player.TimedFear);
                info[i++] = "You are terrified";
            }
            if (Player.TimedPoison != 0)
            {
                info2[i] = ReportMagicsAux(Player.TimedPoison);
                info[i++] = "You are poisoned";
            }
            if (Player.TimedHallucinations != 0)
            {
                info2[i] = ReportMagicsAux(Player.TimedHallucinations);
                info[i++] = "You are hallucinating";
            }
            if (Player.TimedBlessing != 0)
            {
                info2[i] = ReportMagicsAux(Player.TimedBlessing);
                info[i++] = "You feel rightous";
            }
            if (Player.TimedHeroism != 0)
            {
                info2[i] = ReportMagicsAux(Player.TimedHeroism);
                info[i++] = "You feel heroic";
            }
            if (Player.TimedSuperheroism != 0)
            {
                info2[i] = ReportMagicsAux(Player.TimedSuperheroism);
                info[i++] = "You are in a battle rage";
            }
            if (Player.TimedProtectionFromEvil != 0)
            {
                info2[i] = ReportMagicsAux(Player.TimedProtectionFromEvil);
                info[i++] = "You are protected from evil";
            }
            if (Player.TimedStoneskin != 0)
            {
                info2[i] = ReportMagicsAux(Player.TimedStoneskin);
                info[i++] = "You are protected by a mystic shield";
            }
            if (Player.TimedInvulnerability != 0)
            {
                info2[i] = ReportMagicsAux(Player.TimedInvulnerability);
                info[i++] = "You are invulnerable";
            }
            if (Player.TimedEtherealness != 0)
            {
                info2[i] = ReportMagicsAux(Player.TimedEtherealness);
                info[i++] = "You are incorporeal";
            }
            if (Player.HasConfusingTouch)
            {
                info2[i] = 7;
                info[i++] = "Your hands are glowing dull red.";
            }
            if (Player.WordOfRecallDelay != 0)
            {
                info2[i] = ReportMagicsAux(Player.WordOfRecallDelay);
                info[i++] = "You waiting to be recalled";
            }
            if (Player.TimedAcidResistance != 0)
            {
                info2[i] = ReportMagicsAux(Player.TimedAcidResistance);
                info[i++] = "You are resistant to acid";
            }
            if (Player.TimedLightningResistance != 0)
            {
                info2[i] = ReportMagicsAux(Player.TimedLightningResistance);
                info[i++] = "You are resistant to lightning";
            }
            if (Player.TimedFireResistance != 0)
            {
                info2[i] = ReportMagicsAux(Player.TimedFireResistance);
                info[i++] = "You are resistant to fire";
            }
            if (Player.TimedColdResistance != 0)
            {
                info2[i] = ReportMagicsAux(Player.TimedColdResistance);
                info[i++] = "You are resistant to cold";
            }
            if (Player.TimedPoisonResistance != 0)
            {
                info2[i] = ReportMagicsAux(Player.TimedPoisonResistance);
                info[i++] = "You are resistant to poison";
            }
            Gui.Save();
            for (k = 1; k < 24; k++)
            {
                Gui.PrintLine("", k, 13);
            }
            Gui.PrintLine("     Your Current Magic:", 1, 15);
            for (k = 2, j = 0; j < i; j++)
            {
                var dummy = $"{info[j]} {GlobalData.ReportMagicDurations[info2[j]]}.";
                Gui.PrintLine(dummy, k++, 15);
                if (k == 22 && j + 1 < i)
                {
                    Gui.PrintLine("-- more --", k, 15);
                    Gui.Inkey();
                    for (; k > 2; k--)
                    {
                        Gui.PrintLine("", k, 15);
                    }
                }
            }
            Gui.PrintLine("[Press any key to continue]", k, 13);
            Gui.Inkey();
            Gui.Load();
        }

        public void SelfKnowledge()
        {
            int i = 0, j, k;
            var f1 = new FlagSet();
            var f2 = new FlagSet();
            var f3 = new FlagSet();
            Item oPtr;
            var info = new string[128];
            var plev = Player.Level;
            for (k = InventorySlot.MeleeWeapon; k < InventorySlot.Total; k++)
            {
                var t1 = new FlagSet();
                var t2 = new FlagSet();
                var t3 = new FlagSet();
                oPtr = Player.Inventory[k];
                if (oPtr.ItemType != null)
                {
                    continue;
                }
                oPtr.GetMergedFlags(t1, t2, t3);
                f1.Set(t1);
                f2.Set(t2);
                f3.Set(t3);
            }

            var racial = Player.Race.SelfKnowledge(Player);
            foreach (var item in racial)
            {
                info[i++] = item;
            }

            var mutations = Player.Dna.GetMutationList();
            if (mutations.Length > 0)
            {
                foreach (var m in mutations)
                {
                    info[i++] = m;
                }
            }
            if (Player.TimedBlindness != 0)
            {
                info[i++] = "You cannot see.";
            }
            if (Player.TimedConfusion != 0)
            {
                info[i++] = "You are confused.";
            }
            if (Player.TimedFear != 0)
            {
                info[i++] = "You are terrified.";
            }
            if (Player.TimedBleeding != 0)
            {
                info[i++] = "You are bleeding.";
            }
            if (Player.TimedStun != 0)
            {
                info[i++] = "You are stunned.";
            }
            if (Player.TimedPoison != 0)
            {
                info[i++] = "You are poisoned.";
            }
            if (Player.TimedHallucinations != 0)
            {
                info[i++] = "You are hallucinating.";
            }
            if (Player.HasAggravation)
            {
                info[i++] = "You aggravate monsters.";
            }
            if (Player.HasRandomTeleport)
            {
                info[i++] = "Your position is very uncertain.";
            }
            if (Player.TimedBlessing != 0)
            {
                info[i++] = "You feel rightous.";
            }
            if (Player.TimedHeroism != 0)
            {
                info[i++] = "You feel heroic.";
            }
            if (Player.TimedSuperheroism != 0)
            {
                info[i++] = "You are in a battle rage.";
            }
            if (Player.TimedProtectionFromEvil != 0)
            {
                info[i++] = "You are protected from evil.";
            }
            if (Player.TimedStoneskin != 0)
            {
                info[i++] = "You are protected by a mystic shield.";
            }
            if (Player.TimedInvulnerability != 0)
            {
                info[i++] = "You are temporarily invulnerable.";
            }
            if (Player.TimedEtherealness != 0)
            {
                info[i++] = "You are temporarily incorporeal.";
            }
            if (Player.HasConfusingTouch)
            {
                info[i++] = "Your hands are glowing dull red.";
            }
            if (Player.IsSearching)
            {
                info[i++] = "You are looking around very carefully.";
            }
            if (Player.SpareSpellSlots != 0)
            {
                info[i++] = "You can learn some spells/prayers.";
            }
            if (Player.WordOfRecallDelay != 0)
            {
                info[i++] = "You will soon be recalled.";
            }
            if (Player.InfravisionRange != 0)
            {
                info[i++] = "Your eyes are sensitive to infrared light.";
            }
            if (Player.HasSeeInvisibility)
            {
                info[i++] = "You can see invisible creatures.";
            }
            if (Player.HasFeatherFall)
            {
                info[i++] = "You can fly.";
            }
            if (Player.HasFreeAction)
            {
                info[i++] = "You have free action.";
            }
            if (Player.HasRegeneration)
            {
                info[i++] = "You regenerate quickly.";
            }
            if (Player.HasSlowDigestion)
            {
                info[i++] = "Your appetite is small.";
            }
            if (Player.HasTelepathy)
            {
                info[i++] = "You have ESP.";
            }
            if (Player.HasHoldLife)
            {
                info[i++] = "You have a firm hold on your life force.";
            }
            if (Player.HasReflection)
            {
                info[i++] = "You reflect arrows and bolts.";
            }
            if (Player.HasFireShield)
            {
                info[i++] = "You are surrounded with a fiery aura.";
            }
            if (Player.HasLightningShield)
            {
                info[i++] = "You are surrounded with electricity.";
            }
            if (Player.HasAntiMagic)
            {
                info[i++] = "You are surrounded by an anti-magic shell.";
            }
            if (Player.HasAntiTeleport)
            {
                info[i++] = "You cannot teleport.";
            }
            if (Player.HasGlow)
            {
                info[i++] = "You are carrying a permanent light.";
            }
            if (Player.HasAcidImmunity)
            {
                info[i++] = "You are completely immune to acid.";
            }
            else if (Player.HasAcidResistance && Player.TimedAcidResistance != 0)
            {
                info[i++] = "You resist acid exceptionally well.";
            }
            else if (Player.HasAcidResistance || Player.TimedAcidResistance != 0)
            {
                info[i++] = "You are resistant to acid.";
            }
            if (Player.HasLightningImmunity)
            {
                info[i++] = "You are completely immune to lightning.";
            }
            else if (Player.HasLightningResistance && Player.TimedLightningResistance != 0)
            {
                info[i++] = "You resist lightning exceptionally well.";
            }
            else if (Player.HasLightningResistance || Player.TimedLightningResistance != 0)
            {
                info[i++] = "You are resistant to lightning.";
            }
            if (Player.HasFireImmunity)
            {
                info[i++] = "You are completely immune to fire.";
            }
            else if (Player.HasFireResistance && Player.TimedFireResistance != 0)
            {
                info[i++] = "You resist fire exceptionally well.";
            }
            else if (Player.HasFireResistance || Player.TimedFireResistance != 0)
            {
                info[i++] = "You are resistant to fire.";
            }
            if (Player.HasColdImmunity)
            {
                info[i++] = "You are completely immune to cold.";
            }
            else if (Player.HasColdResistance && Player.TimedColdResistance != 0)
            {
                info[i++] = "You resist cold exceptionally well.";
            }
            else if (Player.HasColdResistance || Player.TimedColdResistance != 0)
            {
                info[i++] = "You are resistant to cold.";
            }
            if (Player.HasPoisonResistance && Player.TimedPoisonResistance != 0)
            {
                info[i++] = "You resist poison exceptionally well.";
            }
            else if (Player.HasPoisonResistance || Player.TimedPoisonResistance != 0)
            {
                info[i++] = "You are resistant to poison.";
            }
            if (Player.HasLightResistance)
            {
                info[i++] = "You are resistant to bright light.";
            }
            if (Player.HasDarkResistance)
            {
                info[i++] = "You are resistant to darkness.";
            }
            if (Player.HasConfusionResistance)
            {
                info[i++] = "You are resistant to confusion.";
            }
            if (Player.HasSoundResistance)
            {
                info[i++] = "You are resistant to sonic attacks.";
            }
            if (Player.HasDisenchantResistance)
            {
                info[i++] = "You are resistant to disenchantment.";
            }
            if (Player.HasChaosResistance)
            {
                info[i++] = "You are resistant to chaos.";
            }
            if (Player.HasShardResistance)
            {
                info[i++] = "You are resistant to blasts of shards.";
            }
            if (Player.HasNexusResistance)
            {
                info[i++] = "You are resistant to nexus attacks.";
            }
            if (Player.HasNetherResistance)
            {
                info[i++] = "You are resistant to nether forces.";
            }
            if (Player.HasFearResistance)
            {
                info[i++] = "You are completely fearless.";
            }
            if (Player.HasBlindnessResistance)
            {
                info[i++] = "Your eyes are resistant to blindness.";
            }
            if (Player.HasSustainStrength)
            {
                info[i++] = "Your strength is sustained.";
            }
            if (Player.HasSustainIntelligence)
            {
                info[i++] = "Your intelligence is sustained.";
            }
            if (Player.HasSustainWisdom)
            {
                info[i++] = "Your wisdom is sustained.";
            }
            if (Player.HasSustainConstitution)
            {
                info[i++] = "Your constitution is sustained.";
            }
            if (Player.HasSustainDexterity)
            {
                info[i++] = "Your dexterity is sustained.";
            }
            if (Player.HasSustainCharisma)
            {
                info[i++] = "Your charisma is sustained.";
            }
            if (f1.IsSet(ItemFlag1.Str))
            {
                info[i++] = "Your strength is affected by your equipment.";
            }
            if (f1.IsSet(ItemFlag1.Int))
            {
                info[i++] = "Your intelligence is affected by your equipment.";
            }
            if (f1.IsSet(ItemFlag1.Wis))
            {
                info[i++] = "Your wisdom is affected by your equipment.";
            }
            if (f1.IsSet(ItemFlag1.Dex))
            {
                info[i++] = "Your dexterity is affected by your equipment.";
            }
            if (f1.IsSet(ItemFlag1.Con))
            {
                info[i++] = "Your constitution is affected by your equipment.";
            }
            if (f1.IsSet(ItemFlag1.Cha))
            {
                info[i++] = "Your charisma is affected by your equipment.";
            }
            if (f1.IsSet(ItemFlag1.Stealth))
            {
                info[i++] = "Your stealth is affected by your equipment.";
            }
            if (f1.IsSet(ItemFlag1.Search))
            {
                info[i++] = "Your searching ability is affected by your equipment.";
            }
            if (f1.IsSet(ItemFlag1.Infra))
            {
                info[i++] = "Your infravision is affected by your equipment.";
            }
            if (f1.IsSet(ItemFlag1.Tunnel))
            {
                info[i++] = "Your digging ability is affected by your equipment.";
            }
            if (f1.IsSet(ItemFlag1.Speed))
            {
                info[i++] = "Your speed is affected by your equipment.";
            }
            if (f1.IsSet(ItemFlag1.Blows))
            {
                info[i++] = "Your attack speed is affected by your equipment.";
            }
            oPtr = Player.Inventory[InventorySlot.MeleeWeapon];
            if (oPtr.ItemType != null)
            {
                if (f3.IsSet(ItemFlag3.Blessed))
                {
                    info[i++] = "Your weapon has been blessed by the gods.";
                }
                if (f1.IsSet(ItemFlag1.Chaotic))
                {
                    info[i++] = "Your weapon is branded with the Yellow Sign.";
                }
                if (f1.IsSet(ItemFlag1.Impact))
                {
                    info[i++] = "The impact of your weapon can cause earthquakes.";
                }
                if (f1.IsSet(ItemFlag1.Vorpal))
                {
                    info[i++] = "Your weapon is very sharp.";
                }
                if (f1.IsSet(ItemFlag1.Vampiric))
                {
                    info[i++] = "Your weapon drains life from your foes.";
                }
                if (f1.IsSet(ItemFlag1.BrandAcid))
                {
                    info[i++] = "Your weapon melts your foes.";
                }
                if (f1.IsSet(ItemFlag1.BrandElec))
                {
                    info[i++] = "Your weapon shocks your foes.";
                }
                if (f1.IsSet(ItemFlag1.BrandFire))
                {
                    info[i++] = "Your weapon burns your foes.";
                }
                if (f1.IsSet(ItemFlag1.BrandCold))
                {
                    info[i++] = "Your weapon freezes your foes.";
                }
                if (f1.IsSet(ItemFlag1.BrandPois))
                {
                    info[i++] = "Your weapon poisons your foes.";
                }
                if (f1.IsSet(ItemFlag1.SlayAnimal))
                {
                    info[i++] = "Your weapon strikes at animals with extra force.";
                }
                if (f1.IsSet(ItemFlag1.SlayEvil))
                {
                    info[i++] = "Your weapon strikes at evil with extra force.";
                }
                if (f1.IsSet(ItemFlag1.SlayUndead))
                {
                    info[i++] = "Your weapon strikes at undead with holy wrath.";
                }
                if (f1.IsSet(ItemFlag1.SlayDemon))
                {
                    info[i++] = "Your weapon strikes at demons with holy wrath.";
                }
                if (f1.IsSet(ItemFlag1.SlayOrc))
                {
                    info[i++] = "Your weapon is especially deadly against orcs.";
                }
                if (f1.IsSet(ItemFlag1.SlayTroll))
                {
                    info[i++] = "Your weapon is especially deadly against trolls.";
                }
                if (f1.IsSet(ItemFlag1.SlayGiant))
                {
                    info[i++] = "Your weapon is especially deadly against giants.";
                }
                if (f1.IsSet(ItemFlag1.SlayDragon))
                {
                    info[i++] = "Your weapon is especially deadly against dragons.";
                }
                if (f1.IsSet(ItemFlag1.KillDragon))
                {
                    info[i++] = "Your weapon is a great bane of dragons.";
                }
            }
            Gui.Save();
            for (k = 1; k < 24; k++)
            {
                Gui.PrintLine("", k, 13);
            }
            Gui.PrintLine("     Your Attributes:", 1, 15);
            for (k = 2, j = 0; j < i; j++)
            {
                Gui.PrintLine(info[j], k++, 15);
                if (k == 22 && j + 1 < i)
                {
                    Gui.PrintLine("-- more --", k, 15);
                    Gui.Inkey();
                    for (; k > 2; k--)
                    {
                        Gui.PrintLine("", k, 15);
                    }
                }
            }
            Gui.PrintLine("[Press any key to continue]", k, 13);
            Gui.Inkey();
            Gui.Load();
        }

        public bool SetAcidDestroy(Item oPtr)
        {
            var f1 = new FlagSet();
            var f2 = new FlagSet();
            var f3 = new FlagSet();
            if (!oPtr.HatesAcid())
            {
                return false;
            }
            oPtr.GetMergedFlags(f1, f2, f3);
            if (f3.IsSet(ItemFlag3.IgnoreAcid))
            {
                return false;
            }
            return true;
        }

        public bool SetColdDestroy(Item oPtr)
        {
            var f1 = new FlagSet();
            var f2 = new FlagSet();
            var f3 = new FlagSet();
            if (!oPtr.HatesCold())
            {
                return false;
            }
            oPtr.GetMergedFlags(f1, f2, f3);
            if (f3.IsSet(ItemFlag3.IgnoreCold))
            {
                return false;
            }
            return true;
        }

        public bool SetElecDestroy(Item oPtr)
        {
            var f1 = new FlagSet();
            var f2 = new FlagSet();
            var f3 = new FlagSet();
            if (!oPtr.HatesElec())
            {
                return false;
            }
            oPtr.GetMergedFlags(f1, f2, f3);
            if (f3.IsSet(ItemFlag3.IgnoreElec))
            {
                return false;
            }
            return true;
        }

        public bool SetFireDestroy(Item oPtr)
        {
            var f1 = new FlagSet();
            var f2 = new FlagSet();
            var f3 = new FlagSet();
            if (!oPtr.HatesFire())
            {
                return false;
            }
            oPtr.GetMergedFlags(f1, f2, f3);
            if (f3.IsSet(ItemFlag3.IgnoreFire))
            {
                return false;
            }
            return true;
        }

        public bool SleepMonster(int dir)
        {
            var flg = ProjectionFlag.ProjectStop | ProjectionFlag.ProjectKill;
            return TargetedProject(new ProjectOldSleep(this), dir, Player.Level, flg);
        }

        public bool SleepMonsters()
        {
            return ProjectAtAllInLos(new ProjectOldSleep(this), Player.Level);
        }

        public void SleepMonstersTouch()
        {
            var flg = ProjectionFlag.ProjectKill | ProjectionFlag.ProjectHide;
            Project(0, 1, Player.MapY, Player.MapX, Player.Level, new ProjectOldSleep(this), flg);
        }

        public bool SlowMonster(int dir)
        {
            var flg = ProjectionFlag.ProjectStop | ProjectionFlag.ProjectKill;
            return TargetedProject(new ProjectOldSlow(this), dir, Player.Level, flg);
        }

        public bool SlowMonsters()
        {
            return ProjectAtAllInLos(new ProjectOldSlow(this), Player.Level);
        }

        public bool SpeedMonster(int dir)
        {
            var flg = ProjectionFlag.ProjectStop | ProjectionFlag.ProjectKill;
            return TargetedProject(new ProjectOldSpeed(this), dir, Player.Level, flg);
        }

        public void StairCreation()
        {
            if (!Level.CaveValidBold(Player.MapY, Player.MapX))
            {
                Profile.Instance.MsgPrint("The object resists the spell.");
                return;
            }
            Level.DeleteObject(Player.MapY, Player.MapX);
            if (_saveGame.CurrentDepth <= 0)
            {
                Level.CaveSetFeat(Player.MapY, Player.MapX, "DownStair");
            }
            else if (_saveGame.Quests.IsQuest(_saveGame.CurrentDepth) ||
                     _saveGame.CurrentDepth >= _saveGame.CurDungeon.MaxLevel)
            {
                Level.CaveSetFeat(Player.MapY, Player.MapX,
                    _saveGame.CurDungeon.Tower ? "DownStair" : "UpStair");
            }
            else if (Program.Rng.RandomLessThan(100) < 50)
            {
                Level.CaveSetFeat(Player.MapY, Player.MapX, "DownStair");
            }
            else
            {
                Level.CaveSetFeat(Player.MapY, Player.MapX, "UpStair");
            }
        }

        public void StasisMonster(int dir)
        {
            var flg = ProjectionFlag.ProjectStop | ProjectionFlag.ProjectKill;
            TargetedProject(new ProjectStasis(this), dir, Player.Level, flg);
        }

        public void StasisMonsters(int dam)
        {
            ProjectAtAllInLos(new ProjectStasis(this), dam);
        }

        public void StunMonster(int dir, int plev)
        {
            var flg = ProjectionFlag.ProjectStop | ProjectionFlag.ProjectKill;
            TargetedProject(new ProjectStun(this), dir, plev, flg);
        }

        public void StunMonsters(int dam)
        {
            ProjectAtAllInLos(new ProjectStun(this), dam);
        }

        public void SummonReaver()
        {
            int i;
            var maxReaver = (_saveGame.Difficulty / 50) + Program.Rng.DieRoll(6);
            for (i = 0; i < maxReaver; i++)
            {
                Level.Monsters.SummonSpecific(Player.MapY, Player.MapX, 100, Constants.SummonReaver);
            }
        }

        public void TeleportAway(int mIdx, int dis)
        {
            var ny = 0;
            var nx = 0;
            var look = true;
            var mPtr = Level.Monsters[mIdx];
            if (mPtr.Race == null)
            {
                return;
            }
            var oy = mPtr.MapY;
            var ox = mPtr.MapX;
            var min = dis / 2;
            while (look)
            {
                if (dis > 200)
                {
                    dis = 200;
                }
                for (var i = 0; i < 500; i++)
                {
                    while (true)
                    {
                        ny = Program.Rng.RandomSpread(oy, dis);
                        nx = Program.Rng.RandomSpread(ox, dis);
                        var d = Level.Distance(oy, ox, ny, nx);
                        if (d >= min && d <= dis)
                        {
                            break;
                        }
                    }
                    if (!Level.InBounds(ny, nx))
                    {
                        continue;
                    }
                    if (!Level.GridPassableNoCreature(ny, nx))
                    {
                        continue;
                    }
                    if (Level.Grid[ny][nx].FeatureType.Name == "ElderSign")
                    {
                        continue;
                    }
                    if (Level.Grid[ny][nx].FeatureType.Name == "YellowSign")
                    {
                        continue;
                    }
                    look = false;
                    break;
                }
                dis *= 2;
                min /= 2;
            }
            Gui.PlaySound(SoundEffect.Teleport);
            Level.Grid[ny][nx].MonsterIndex = mIdx;
            Level.Grid[oy][ox].MonsterIndex = 0;
            mPtr.MapY = ny;
            mPtr.MapX = nx;
            Level.Monsters.UpdateMonsterVisibility(mIdx, true);
            Level.RedrawSingleLocation(oy, ox);
            Level.RedrawSingleLocation(ny, nx);
        }

        public bool TeleportMonster(int dir)
        {
            var flg = ProjectionFlag.ProjectBeam | ProjectionFlag.ProjectKill;
            return TargetedProject(new ProjectAwayAll(this), dir, Constants.MaxSight * 5, flg);
        }

        public void TeleportPlayer(int dis)
        {
            int x = Player.MapY, y = Player.MapX;
            var xx = -1;
            var look = true;
            if (Player.HasAntiTeleport)
            {
                Profile.Instance.MsgPrint("A mysterious force prevents you from teleporting!");
                return;
            }
            if (dis > 200)
            {
                dis = 200;
            }
            var min = dis / 2;
            while (look)
            {
                if (dis > 200)
                {
                    dis = 200;
                }
                for (var i = 0; i < 500; i++)
                {
                    while (true)
                    {
                        y = Program.Rng.RandomSpread(Player.MapY, dis);
                        x = Program.Rng.RandomSpread(Player.MapX, dis);
                        var d = Level.Distance(Player.MapY, Player.MapX, y, x);
                        if (d >= min && d <= dis)
                        {
                            break;
                        }
                    }
                    if (!Level.InBounds(y, x))
                    {
                        continue;
                    }
                    if (!Level.GridOpenNoItemOrCreature(y, x))
                    {
                        continue;
                    }
                    if (Level.Grid[y][x].TileFlags.IsSet(GridTile.InVault))
                    {
                        continue;
                    }
                    look = false;
                    break;
                }
                dis *= 2;
                min /= 2;
            }
            Gui.PlaySound(SoundEffect.Teleport);
            var oy = Player.MapY;
            var ox = Player.MapX;
            Player.MapY = y;
            Player.MapX = x;
            Level.RedrawSingleLocation(oy, ox);
            while (xx < 2)
            {
                var yy = -1;
                while (yy < 2)
                {
                    if (xx == 0 && yy == 0)
                    {
                    }
                    else
                    {
                        if (Level.Grid[oy + yy][ox + xx].MonsterIndex != 0)
                        {
                            if ((Level.Monsters[Level.Grid[oy + yy][ox + xx].MonsterIndex].Race.Flags6 &
                                 MonsterFlag6.TeleportSelf) != 0 &&
                                (Level.Monsters[Level.Grid[oy + yy][ox + xx].MonsterIndex].Race.Flags3 &
                                 MonsterFlag3.ResistTeleport) == 0)
                            {
                                if (Level.Monsters[Level.Grid[oy + yy][ox + xx].MonsterIndex].SleepLevel == 0)
                                {
                                    TeleportToPlayer(Level.Grid[oy + yy][ox + xx].MonsterIndex);
                                }
                            }
                        }
                    }
                    yy++;
                }
                xx++;
            }
            Level.RedrawSingleLocation(Player.MapY, Player.MapX);
            var targetEngine = new TargetEngine(Player, Level);
            targetEngine.RecenterScreenAroundPlayer();
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateView | UpdateFlags.UpdateLight | UpdateFlags.UpdateScent);
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateDistances);
            _saveGame.HandleStuff();
        }

        public void TeleportPlayerLevel()
        {
            if (Player.HasAntiTeleport)
            {
                Profile.Instance.MsgPrint("A mysterious force prevents you from teleporting!");
                return;
            }
            var downDesc = _saveGame.CurDungeon.Tower ? "You rise up through the ceiling." : "You sink through the floor.";
            var upDesc = _saveGame.CurDungeon.Tower ? "You sink through the floor." : "You rise up through the ceiling.";
            if (_saveGame.CurrentDepth <= 0)
            {
                Profile.Instance.MsgPrint(downDesc);
                _saveGame.IsAutosave = true;
                _saveGame.DoCmdSaveGame();
                _saveGame.IsAutosave = false;
                _saveGame.CurrentDepth++;
                _saveGame.NewLevelFlag = true;
            }
            else if (_saveGame.Quests.IsQuest(_saveGame.CurrentDepth) ||
                     _saveGame.CurrentDepth >= _saveGame.CurDungeon.MaxLevel)
            {
                Profile.Instance.MsgPrint(upDesc);
                _saveGame.IsAutosave = true;
                _saveGame.DoCmdSaveGame();
                _saveGame.IsAutosave = false;
                _saveGame.CurrentDepth--;
                _saveGame.NewLevelFlag = true;
            }
            else if (Program.Rng.RandomLessThan(100) < 50)
            {
                Profile.Instance.MsgPrint(upDesc);
                _saveGame.IsAutosave = true;
                _saveGame.DoCmdSaveGame();
                _saveGame.IsAutosave = false;
                _saveGame.CurrentDepth--;
                _saveGame.NewLevelFlag = true;
                _saveGame.CameFrom = LevelStart.StartRandom;
            }
            else
            {
                Profile.Instance.MsgPrint(downDesc);
                _saveGame.IsAutosave = true;
                _saveGame.DoCmdSaveGame();
                _saveGame.IsAutosave = false;
                _saveGame.CurrentDepth++;
                _saveGame.NewLevelFlag = true;
            }
            _saveGame.IsAutosave = true;
            _saveGame.DoCmdSaveGame();
            _saveGame.IsAutosave = false;
            _saveGame.CurrentDepth++;
            _saveGame.NewLevelFlag = true;
            Gui.PlaySound(SoundEffect.TeleportLevel);
        }

        public void TeleportPlayerTo(int ny, int nx)
        {
            int y, x, dis = 0, ctr = 0;
            if (Player.HasAntiTeleport)
            {
                Profile.Instance.MsgPrint("A mysterious force prevents you from teleporting!");
                return;
            }
            while (true)
            {
                while (true)
                {
                    y = Program.Rng.RandomSpread(ny, dis);
                    x = Program.Rng.RandomSpread(nx, dis);
                    if (Level.InBounds(y, x))
                    {
                        break;
                    }
                }
                if (Level.GridOpenNoItemOrCreature(y, x))
                {
                    break;
                }
                if (++ctr > (4 * dis * dis) + (4 * dis) + 1)
                {
                    ctr = 0;
                    dis++;
                }
            }
            Gui.PlaySound(SoundEffect.Teleport);
            var oy = Player.MapY;
            var ox = Player.MapX;
            Player.MapY = y;
            Player.MapX = x;
            Level.RedrawSingleLocation(oy, ox);
            Level.RedrawSingleLocation(Player.MapY, Player.MapX);
            var targetEngine = new TargetEngine(Player, Level);
            targetEngine.RecenterScreenAroundPlayer();
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateView | UpdateFlags.UpdateLight | UpdateFlags.UpdateScent);
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateDistances);
            _saveGame.HandleStuff();
        }

        public void TeleportSwap(int dir)
        {
            var targetEngine = new TargetEngine(Player, Level);
            int tx, ty;
            if (dir == 5 && targetEngine.TargetOkay())
            {
                tx = _saveGame.TargetCol;
                ty = _saveGame.TargetRow;
            }
            else
            {
                tx = Player.MapX + Level.KeypadDirectionXOffset[dir];
                ty = Player.MapY + Level.KeypadDirectionYOffset[dir];
            }
            var cPtr = Level.Grid[ty][tx];
            if (cPtr.MonsterIndex == 0)
            {
                Profile.Instance.MsgPrint("You can't trade places with that!");
            }
            else
            {
                var mPtr = Level.Monsters[cPtr.MonsterIndex];
                var rPtr = mPtr.Race;
                if ((rPtr.Flags3 & MonsterFlag3.ResistTeleport) != 0)
                {
                    Profile.Instance.MsgPrint("Your teleportation is blocked!");
                }
                else
                {
                    Gui.PlaySound(SoundEffect.Teleport);
                    Level.Grid[Player.MapY][Player.MapX].MonsterIndex = cPtr.MonsterIndex;
                    cPtr.MonsterIndex = 0;
                    mPtr.MapY = Player.MapY;
                    mPtr.MapX = Player.MapX;
                    Player.MapX = tx;
                    Player.MapY = ty;
                    tx = mPtr.MapX;
                    ty = mPtr.MapY;
                    Level.Monsters.UpdateMonsterVisibility(Level.Grid[ty][tx].MonsterIndex, true);
                    Level.RedrawSingleLocation(ty, tx);
                    Level.RedrawSingleLocation(Player.MapY, Player.MapX);
                    targetEngine.RecenterScreenAroundPlayer();
                    Player.UpdatesNeeded.Set(UpdateFlags.UpdateView | UpdateFlags.UpdateLight | UpdateFlags.UpdateScent);
                    Player.UpdatesNeeded.Set(UpdateFlags.UpdateDistances);
                    _saveGame.HandleStuff();
                }
            }
        }

        public bool TrapCreation()
        {
            var flg = ProjectionFlag.ProjectGrid | ProjectionFlag.ProjectItem | ProjectionFlag.ProjectHide;
            return Project(0, 1, Player.MapY, Player.MapX, 0, new ProjectMakeTrap(this), flg);
        }

        public void TurnEvil(int dam)
        {
            ProjectAtAllInLos(new ProjectTurnEvil(this), dam);
        }

        public void TurnMonsters(int dam)
        {
            ProjectAtAllInLos(new ProjectTurnAll(this), dam);
        }

        public bool UnlightArea(int dam, int rad)
        {
            var flg = ProjectionFlag.ProjectGrid | ProjectionFlag.ProjectKill;
            if (Player.TimedBlindness == 0)
            {
                Profile.Instance.MsgPrint("Darkness surrounds you.");
            }
            Project(0, rad, Player.MapY, Player.MapX, dam, new ProjectDarkWeak(this), flg);
            UnlightRoom(Player.MapY, Player.MapX);
            return true;
        }

        public void UnlightRoom(int y1, int x1)
        {
            CaveTempRoomAux(y1, x1);
            for (var i = 0; i < Level.TempN; i++)
            {
                var x = Level.TempX[i];
                var y = Level.TempY[i];
                if (!Level.GridPassable(y, x))
                {
                    continue;
                }
                CaveTempRoomAux(y + 1, x);
                CaveTempRoomAux(y - 1, x);
                CaveTempRoomAux(y, x + 1);
                CaveTempRoomAux(y, x - 1);
                CaveTempRoomAux(y + 1, x + 1);
                CaveTempRoomAux(y - 1, x - 1);
                CaveTempRoomAux(y - 1, x + 1);
                CaveTempRoomAux(y + 1, x - 1);
            }
            CaveTempRoomUnlight();
        }

        public void WallBreaker()
        {
            int dummy;
            if (Program.Rng.DieRoll(80 + Player.Level) < 70)
            {
                do
                {
                    dummy = Program.Rng.DieRoll(9);
                } while (dummy == 5 || dummy == 0);
                WallToMud(dummy);
            }
            else if (Program.Rng.DieRoll(100) > 30)
            {
                Earthquake(Player.MapY, Player.MapX, 1);
            }
            else
            {
                for (dummy = 1; dummy < 10; dummy++)
                {
                    if (dummy != 5)
                    {
                        WallToMud(dummy);
                    }
                }
            }
        }

        public void WallStone()
        {
            var flg = ProjectionFlag.ProjectGrid | ProjectionFlag.ProjectItem;
            _ = Project(0, 1, Player.MapY, Player.MapX, 0, new ProjectStoneWall(this), flg);
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateView | UpdateFlags.UpdateLight | UpdateFlags.UpdateScent);
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateMonsters);
            Player.RedrawNeeded.Set(RedrawFlag.PrMap);
        }

        public bool WallToMud(int dir)
        {
            var flg = ProjectionFlag.ProjectBeam | ProjectionFlag.ProjectGrid | ProjectionFlag.ProjectItem |
                      ProjectionFlag.ProjectKill;
            return TargetedProject(new ProjectKillWall(this), dir, 20 + Program.Rng.DieRoll(30), flg);
        }

        public void WizardLock(int dir)
        {
            var flg = ProjectionFlag.ProjectBeam | ProjectionFlag.ProjectGrid | ProjectionFlag.ProjectItem |
                      ProjectionFlag.ProjectKill;
            TargetedProject(new ProjectJamDoor(this), dir, 20 + Program.Rng.DieRoll(30), flg);
        }

        public void YellowSign()
        {
            if (!Level.GridOpenNoItem(Player.MapY, Player.MapX))
            {
                Profile.Instance.MsgPrint("The object resists the spell.");
                return;
            }
            Level.CaveSetFeat(Player.MapY, Player.MapX, "YellowSign");
        }

        private void CaveTempRoomAux(int y, int x)
        {
            var cPtr = Level.Grid[y][x];
            if (cPtr.TileFlags.IsSet(GridTile.TempFlag))
            {
                return;
            }
            if (cPtr.TileFlags.IsClear(GridTile.InRoom))
            {
                return;
            }
            if (Level.TempN == Constants.TempMax)
            {
                return;
            }
            cPtr.TileFlags.Set(GridTile.TempFlag);
            Level.TempY[Level.TempN] = y;
            Level.TempX[Level.TempN] = x;
            Level.TempN++;
        }

        private void CaveTempRoomLight()
        {
            for (var i = 0; i < Level.TempN; i++)
            {
                var y = Level.TempY[i];
                var x = Level.TempX[i];
                var cPtr = Level.Grid[y][x];
                cPtr.TileFlags.Clear(GridTile.TempFlag);
                cPtr.TileFlags.Set(GridTile.SelfLit);
                if (cPtr.MonsterIndex != 0)
                {
                    var chance = 25;
                    var mPtr = Level.Monsters[cPtr.MonsterIndex];
                    var rPtr = mPtr.Race;
                    Level.Monsters.UpdateMonsterVisibility(cPtr.MonsterIndex, false);
                    if ((rPtr.Flags2 & MonsterFlag2.Stupid) != 0)
                    {
                        chance = 10;
                    }
                    if ((rPtr.Flags2 & MonsterFlag2.Smart) != 0)
                    {
                        chance = 100;
                    }
                    if (mPtr.SleepLevel != 0 && Program.Rng.RandomLessThan(100) < chance)
                    {
                        mPtr.SleepLevel = 0;
                        if (mPtr.IsVisible)
                        {
                            var mName = mPtr.MonsterDesc(0);
                            Profile.Instance.MsgPrint($"{mName} wakes up.");
                        }
                    }
                }
                Level.NoteSpot(y, x);
                Level.RedrawSingleLocation(y, x);
            }
            Level.TempN = 0;
        }

        private void CaveTempRoomUnlight()
        {
            for (var i = 0; i < Level.TempN; i++)
            {
                var y = Level.TempY[i];
                var x = Level.TempX[i];
                var cPtr = Level.Grid[y][x];
                cPtr.TileFlags.Clear(GridTile.TempFlag);
                cPtr.TileFlags.Clear(GridTile.SelfLit);
                if (cPtr.FeatureType.IsOpenFloor)
                {
                    cPtr.TileFlags.Clear(GridTile.PlayerMemorised);
                    Level.NoteSpot(y, x);
                }
                if (cPtr.MonsterIndex != 0)
                {
                    Level.Monsters.UpdateMonsterVisibility(cPtr.MonsterIndex, false);
                }
                Level.RedrawSingleLocation(y, x);
            }
            Level.TempN = 0;
        }

        private bool DetectMonstersString(string match)
        {
            var flag = false;
            for (var i = 1; i < Level.MMax; i++)
            {
                var mPtr = Level.Monsters[i];
                var rPtr = mPtr.Race;
                if (mPtr.Race == null)
                {
                    continue;
                }
                var y = mPtr.MapY;
                var x = mPtr.MapX;
                if (!Level.PanelContains(y, x))
                {
                    continue;
                }
                if (match.Contains(rPtr.Character.ToString()))
                {
                    Level.Monsters.RepairMonsters = true;
                    mPtr.IndividualMonsterFlags |= Constants.MflagMark | Constants.MflagShow;
                    mPtr.IsVisible = true;
                    Level.RedrawSingleLocation(y, x);
                    flag = true;
                }
            }
            if (flag)
            {
                Profile.Instance.MsgPrint("You sense the presence of monsters!");
            }
            return flag;
        }

        private bool ItemTesterHookWeapon(Item oPtr)
        {
            switch (oPtr.Category)
            {
                case ItemCategory.Sword:
                case ItemCategory.Hafted:
                case ItemCategory.Polearm:
                case ItemCategory.Digging:
                case ItemCategory.Bow:
                case ItemCategory.Bolt:
                case ItemCategory.Arrow:
                case ItemCategory.Shot:
                    {
                        return true;
                    }
            }
            return false;
        }

        private void LightRoom(int y1, int x1)
        {
            CaveTempRoomAux(y1, x1);
            for (var i = 0; i < Level.TempN; i++)
            {
                var x = Level.TempX[i];
                var y = Level.TempY[i];
                if (!Level.GridPassable(y, x))
                {
                    continue;
                }
                CaveTempRoomAux(y + 1, x);
                CaveTempRoomAux(y - 1, x);
                CaveTempRoomAux(y, x + 1);
                CaveTempRoomAux(y, x - 1);
                CaveTempRoomAux(y + 1, x + 1);
                CaveTempRoomAux(y - 1, x - 1);
                CaveTempRoomAux(y - 1, x + 1);
                CaveTempRoomAux(y + 1, x - 1);
            }
            CaveTempRoomLight();
        }

        private bool MinusAc()
        {
            Item oPtr = null;
            var f1 = new FlagSet();
            var f2 = new FlagSet();
            var f3 = new FlagSet();
            switch (Program.Rng.DieRoll(6))
            {
                case 1:
                    oPtr = Player.Inventory[InventorySlot.Body];
                    break;

                case 2:
                    oPtr = Player.Inventory[InventorySlot.Arm];
                    break;

                case 3:
                    oPtr = Player.Inventory[InventorySlot.Cloak];
                    break;

                case 4:
                    oPtr = Player.Inventory[InventorySlot.Hands];
                    break;

                case 5:
                    oPtr = Player.Inventory[InventorySlot.Head];
                    break;

                case 6:
                    oPtr = Player.Inventory[InventorySlot.Feet];
                    break;
            }
            if (oPtr == null)
            {
                return false;
            }
            if (oPtr.ItemType == null)
            {
                return false;
            }
            if (oPtr.BaseArmourClass + oPtr.BonusArmourClass <= 0)
            {
                return false;
            }
            var oName = oPtr.Description(false, 0);
            oPtr.GetMergedFlags(f1, f2, f3);
            if (f3.IsSet(ItemFlag3.IgnoreAcid))
            {
                Profile.Instance.MsgPrint($"Your {oName} is unaffected!");
                return true;
            }
            Profile.Instance.MsgPrint($"Your {oName} is damaged!");
            oPtr.BonusArmourClass--;
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            return true;
        }

        private bool ProjectAtAllInLos(IProjection projectile, int dam)
        {
            var flg = ProjectionFlag.ProjectJump | ProjectionFlag.ProjectKill | ProjectionFlag.ProjectHide;
            var obvious = false;
            for (var i = 1; i < Level.MMax; i++)
            {
                var mPtr = Level.Monsters[i];
                if (mPtr.Race == null)
                {
                    continue;
                }
                var y = mPtr.MapY;
                var x = mPtr.MapX;
                if (!Level.PlayerHasLosBold(y, x))
                {
                    continue;
                }
                if (Project(0, 0, y, x, dam, projectile, flg))
                {
                    obvious = true;
                }
            }
            return obvious;
        }

        private bool RemoveCurseAux(bool all)
        {
            var cnt = 0;
            for (var i = InventorySlot.MeleeWeapon; i < InventorySlot.Total; i++)
            {
                var f1 = new FlagSet();
                var f2 = new FlagSet();
                var f3 = new FlagSet();
                var oPtr = Player.Inventory[i];
                if (oPtr.ItemType == null)
                {
                    continue;
                }
                if (!oPtr.IsCursed())
                {
                    continue;
                }
                oPtr.GetMergedFlags(f1, f2, f3);
                if (!all && f3.IsSet(ItemFlag3.HeavyCurse))
                {
                    continue;
                }
                if (f3.IsSet(ItemFlag3.PermaCurse))
                {
                    continue;
                }
                oPtr.IdentifyFlags.Clear(Constants.IdentCursed);
                oPtr.IdentifyFlags.Set(Constants.IdentSense);
                if (oPtr.LegendaryFlags3.IsSet(ItemFlag3.Cursed))
                {
                    oPtr.LegendaryFlags3.Clear(ItemFlag3.Cursed);
                }
                if (oPtr.LegendaryFlags3.IsSet(ItemFlag3.HeavyCurse))
                {
                    oPtr.LegendaryFlags3.Clear(ItemFlag3.HeavyCurse);
                }
                oPtr.Inscription = "uncursed";
                Player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
                cnt++;
            }
            return cnt > 0;
        }

        private int ReportMagicsAux(int dur)
        {
            if (dur <= 5)
            {
                return 0;
            }
            if (dur <= 10)
            {
                return 1;
            }
            if (dur <= 20)
            {
                return 2;
            }
            if (dur <= 50)
            {
                return 3;
            }
            if (dur <= 100)
            {
                return 4;
            }
            if (dur <= 200)
            {
                return 5;
            }
            return 6;
        }

        private bool TargetedProject(IProjection projectile, int dir, int dam, ProjectionFlag flg)
        {
            var targetEngine = new TargetEngine(Player, Level);
            flg |= ProjectionFlag.ProjectThru;
            var tx = Player.MapX + Level.KeypadDirectionXOffset[dir];
            var ty = Player.MapY + Level.KeypadDirectionYOffset[dir];
            if (dir == 5 && targetEngine.TargetOkay())
            {
                tx = _saveGame.TargetCol;
                ty = _saveGame.TargetRow;
            }
            return Project(0, 0, ty, tx, dam, projectile, flg);
        }

        private void TeleportToPlayer(int mIdx)
        {
            var ny = Player.MapY;
            var nx = Player.MapX;
            var dis = 2;
            var look = true;
            var mPtr = Level.Monsters[mIdx];
            var attempts = 500;
            if (mPtr.Race == null)
            {
                return;
            }
            if (Program.Rng.DieRoll(100) > mPtr.Race.Level)
            {
                return;
            }
            var oy = mPtr.MapY;
            var ox = mPtr.MapX;
            var min = dis / 2;
            while (look && --attempts != 0)
            {
                if (dis > 200)
                {
                    dis = 200;
                }
                for (var i = 0; i < 500; i++)
                {
                    while (true)
                    {
                        ny = Program.Rng.RandomSpread(Player.MapY, dis);
                        nx = Program.Rng.RandomSpread(Player.MapX, dis);
                        var d = Level.Distance(Player.MapY, Player.MapX, ny, nx);
                        if (d >= min && d <= dis)
                        {
                            break;
                        }
                    }
                    if (!Level.InBounds(ny, nx))
                    {
                        continue;
                    }
                    if (!Level.GridPassableNoCreature(ny, nx))
                    {
                        continue;
                    }
                    if (Level.Grid[ny][nx].FeatureType.Name == "ElderSign")
                    {
                        continue;
                    }
                    if (Level.Grid[ny][nx].FeatureType.Name == "YellowSign")
                    {
                        continue;
                    }
                    look = false;
                    break;
                }
                dis *= 2;
                min /= 2;
            }
            if (attempts < 1)
            {
                return;
            }
            Gui.PlaySound(SoundEffect.Teleport);
            Level.Grid[ny][nx].MonsterIndex = mIdx;
            Level.Grid[oy][ox].MonsterIndex = 0;
            mPtr.MapY = ny;
            mPtr.MapX = nx;
            Level.Monsters.UpdateMonsterVisibility(mIdx, true);
            Level.RedrawSingleLocation(oy, ox);
            Level.RedrawSingleLocation(ny, nx);
        }
    }
}