using Cthangband.Enumerations;
using Cthangband.StaticData;
using Cthangband.UI;
using System;
using System.Collections.Generic;

namespace Cthangband
{
    [Serializable]
    internal class MonsterList
    {
        public int CurrentlyActingMonster;
        public int DunBias = 0;
        public GetMonNumHookDelegate GetMonNumHook;
        public int NumRepro;
        public bool RepairMonsters;
        public bool ShimmerMonsters;
        public char SummonKinType;
        private readonly Level _level;
        private readonly Monster[] _monsters;
        private int _hackMIdxIi;
        private int _placeMonsterIdx;
        private int _summonSpecificType;

        public MonsterList(Level level)
        {
            _level = level;
            _monsters = new Monster[Constants.MaxMIdx];
            for (int j = 0; j < Constants.MaxMIdx; j++)
            {
                _monsters[j] = new Monster();
            }
        }

        internal delegate bool GetMonNumHookDelegate(int rIdx);

        public Monster this[int index] => _monsters[index];

        public bool AllocHorde(int y, int x)
        {
            MonsterRace rPtr = null;
            int attempts = 1000;
            while (--attempts != 0)
            {
                int rIdx = GetMonNum(_level.MonsterLevel);
                if (rIdx == 0)
                {
                    return false;
                }
                rPtr = Profile.Instance.MonsterRaces[rIdx];
                if ((rPtr.Flags1 & MonsterFlag1.Unique) == 0 && (rPtr.Flags1 & MonsterFlag1.EscortsGroup) == 0)
                {
                    break;
                }
            }
            if (attempts < 1)
            {
                return false;
            }
            attempts = 1000;
            while (--attempts == 0)
            {
                if (PlaceMonsterAux(y, x, rPtr, false, false, false))
                {
                    break;
                }
            }
            if (attempts < 1)
            {
                return false;
            }
            Monster mPtr = _monsters[_hackMIdxIi];
            SummonKinType = rPtr.Character;
            for (attempts = Program.Rng.DieRoll(10) + 5; attempts != 0; attempts--)
            {
                SummonSpecific(mPtr.MapY, mPtr.MapX, SaveGame.Instance.Difficulty, Constants.SummonKin);
            }
            return true;
        }

        public void AllocMonster(int dis, bool slp)
        {
            int y = 0;
            int x = 0;
            int attemptsLeft = 10000;
            while (attemptsLeft != 0)
            {
                y = Program.Rng.RandomLessThan(_level.CurHgt);
                x = Program.Rng.RandomLessThan(_level.CurWid);
                if (!_level.GridOpenNoItemOrCreature(y, x))
                {
                    continue;
                }
                if (_level.Distance(y, x, SaveGame.Instance.Player.MapY, SaveGame.Instance.Player.MapX) > dis)
                {
                    break;
                }
                attemptsLeft--;
            }
            if (attemptsLeft == 0)
            {
                return;
            }
            if (Program.Rng.DieRoll(5000) <= SaveGame.Instance.Difficulty)
            {
                if (AllocHorde(y, x))
                {
                }
            }
            else
            {
                if (DunBias > 0 && Program.Rng.RandomBetween(1, 10) > 6)
                {
                    if (SummonSpecific(y, x, SaveGame.Instance.Difficulty, DunBias))
                    {
                    }
                }
                else
                {
                    if (PlaceMonster(y, x, slp, true))
                    {
                    }
                }
            }
        }

        public void CompactMonsters(int size)
        {
            int i, num, cnt;
            if (size != 0)
            {
                Profile.Instance.MsgPrint("Compacting monsters...");
            }
            for (num = 0, cnt = 1; num < size; cnt++)
            {
                int curLev = 5 * cnt;
                int curDis = 5 * (20 - cnt);
                for (i = 1; i < _level.MMax; i++)
                {
                    Monster mPtr = _monsters[i];
                    MonsterRace rPtr = mPtr.Race;
                    if (mPtr.Race == null)
                    {
                        continue;
                    }
                    if (rPtr.Level > curLev)
                    {
                        continue;
                    }
                    if (curDis > 0 && mPtr.DistanceFromPlayer < curDis)
                    {
                        continue;
                    }
                    int chance = 90;
                    if ((rPtr.Flags1 & MonsterFlag1.Unique) != 0)
                    {
                        chance = 99;
                    }
                    if ((rPtr.Flags1 & MonsterFlag1.Guardian) != 0 && cnt < 1000)
                    {
                        chance = 100;
                    }
                    if (Program.Rng.RandomLessThan(100) < chance)
                    {
                        continue;
                    }
                    DeleteMonsterByIndex(i, true);
                    num++;
                }
            }
            for (i = _level.MMax - 1; i >= 1; i--)
            {
                Monster mPtr = _monsters[i];
                if (mPtr.Race != null)
                {
                    continue;
                }
                CompactMonstersAux(_level.MMax - 1, i);
                _level.MMax--;
            }
        }

        public bool DamageMonster(int mIdx, int dam, out bool fear, string note)
        {
            fear = false;
            Monster mPtr = _monsters[mIdx];
            MonsterRace rPtr = mPtr.Race;
            if (SaveGame.Instance.TrackedMonsterIndex == mIdx)
            {
                SaveGame.Instance.Player.RedrawFlags |= RedrawFlag.PrHealth;
            }
            mPtr.SleepLevel = 0;
            mPtr.Health -= dam;
            if (mPtr.Health < 0)
            {
                string mName = mPtr.MonsterDesc(0);
                if ((rPtr.Flags3 & MonsterFlag3.GreatOldOne) != 0 && Program.Rng.DieRoll(2) == 1)
                {
                    Profile.Instance.MsgPrint($"{mName} retreats into another dimension!");
                    if (Program.Rng.DieRoll(5) == 1)
                    {
                        int curses = 1 + Program.Rng.DieRoll(3);
                        Profile.Instance.MsgPrint("Nyarlathotep puts a terrible curse on you!");
                        SaveGame.Instance.Player.CurseEquipment(100, 50);
                        do
                        {
                            SaveGame.Instance.ActivateDreadCurse();
                        } while (--curses != 0);
                    }
                }
                Gui.PlaySound(SoundEffect.MonsterDies);
                if (string.IsNullOrEmpty(note) == false)
                {
                    Profile.Instance.MsgPrint($"{mName}{note}");
                }
                else if (!mPtr.IsVisible)
                {
                    Profile.Instance.MsgPrint($"You have killed {mName}.");
                }
                else if ((rPtr.Flags3 & MonsterFlag3.Demon) != 0 || (rPtr.Flags3 & MonsterFlag3.Undead) != 0 ||
                         (rPtr.Flags3 & MonsterFlag3.Cthuloid) != 0 || (rPtr.Flags2 & MonsterFlag2.Stupid) != 0 ||
                         (rPtr.Flags3 & MonsterFlag3.Nonliving) != 0 || "Evg".Contains(rPtr.Character.ToString()))
                {
                    Profile.Instance.MsgPrint($"You have destroyed {mName}.");
                }
                else
                {
                    Profile.Instance.MsgPrint($"You have slain {mName}.");
                }
                int div = 10 * SaveGame.Instance.Player.MaxLevelGained;
                if (rPtr.Knowledge.RPkills >= 19)
                {
                    div *= 2;
                }
                if (rPtr.Knowledge.RPkills == 0)
                {
                    div /= 3;
                }
                if (rPtr.Knowledge.RPkills == 1)
                {
                    div /= 2;
                }
                if (rPtr.Knowledge.RPkills == 2)
                {
                    div /= 2;
                }
                if (div < 1)
                {
                    div = 1;
                }
                int newExp = rPtr.Mexp * rPtr.Level * 10 / div;
                int newExpFrac = (rPtr.Mexp * rPtr.Level % div * 0x10000 / div) + SaveGame.Instance.Player.FractionalExperiencePoints;
                if (newExpFrac >= 0x10000)
                {
                    newExp++;
                    SaveGame.Instance.Player.FractionalExperiencePoints = newExpFrac - 0x10000;
                }
                else
                {
                    SaveGame.Instance.Player.FractionalExperiencePoints = newExpFrac;
                }
                SaveGame.Instance.Player.GainExperience(newExp);
                SaveGame.Instance.MonsterDeath(mIdx);
                if ((rPtr.Flags1 & MonsterFlag1.Unique) != 0)
                {
                    rPtr.MaxNum = 0;
                }
                if (mPtr.IsVisible || (rPtr.Flags1 & MonsterFlag1.Unique) != 0)
                {
                    if (rPtr.Knowledge.RPkills < int.MaxValue)
                    {
                        rPtr.Knowledge.RPkills++;
                    }
                    if (rPtr.Knowledge.RTkills < int.MaxValue)
                    {
                        rPtr.Knowledge.RTkills++;
                    }
                }
                DeleteMonsterByIndex(mIdx, true);
                fear = false;
                return true;
            }
            if (mPtr.FearLevel != 0 && dam > 0)
            {
                int tmp = Program.Rng.DieRoll(dam);
                if (tmp < mPtr.FearLevel)
                {
                    mPtr.FearLevel -= tmp;
                }
                else
                {
                    mPtr.FearLevel = 0;
                    fear = false;
                }
            }
            if (mPtr.FearLevel == 0 && (rPtr.Flags3 & MonsterFlag3.ImmuneFear) == 0)
            {
                int percentage = 100 * mPtr.Health / mPtr.MaxHealth;
                if ((percentage <= 10 && Program.Rng.RandomLessThan(10) < percentage) ||
                    (dam >= mPtr.Health && Program.Rng.RandomLessThan(100) < 80))
                {
                    fear = true;
                    mPtr.FearLevel = Program.Rng.DieRoll(10) +
                                   (dam >= mPtr.Health && percentage > 7 ? 20 : (11 - percentage) * 5);
                }
            }
            return false;
        }

        public void DeleteMonsterByIndex(int i, bool visibly)
        {
            Monster mPtr = _monsters[i];
            MonsterRace rPtr = mPtr.Race;
            if (rPtr == null)
            {
                return;
            }
            int nextOIdx;
            int y = mPtr.MapY;
            int x = mPtr.MapX;
            rPtr.CurNum--;
            if ((rPtr.Flags2 & MonsterFlag2.Multiply) != 0)
            {
                NumRepro--;
            }
            if (i == SaveGame.Instance.TargetWho)
            {
                SaveGame.Instance.TargetWho = 0;
            }
            if (i == SaveGame.Instance.TrackedMonsterIndex)
            {
                SaveGame.Instance.HealthTrack(0);
            }
            _level.Grid[y][x].Monster = 0;
            for (int thisOIdx = mPtr.FirstHeldItemIndex; thisOIdx != 0; thisOIdx = nextOIdx)
            {
                Item oPtr = _level.Items[thisOIdx];
                nextOIdx = oPtr.NextInStack;
                oPtr.HoldingMonsterIndex = 0;
                _level.DeleteObjectIdx(thisOIdx);
            }
            _monsters[i] = new Monster();
            _level.MCnt--;
            if (visibly)
            {
                _level.RedrawSingleLocation(y, x);
            }
        }

        public int GetMonNum(int level)
        {
            int i, j;
            AllocationEntry[] table = SaveGame.Instance.AllocRaceTable;
            if (level > 0)
            {
                if (Program.Rng.RandomLessThan(Constants.NastyMon) == 0)
                {
                    int d = (level / 4) + 2;
                    level += d < 5 ? d : 5;
                }
                if (Program.Rng.RandomLessThan(Constants.NastyMon) == 0)
                {
                    int d = (level / 4) + 2;
                    level += d < 5 ? d : 5;
                }
            }
            int total = 0;
            for (i = 0; i < SaveGame.Instance.AllocRaceSize; i++)
            {
                if (table[i].Level > level)
                {
                    break;
                }
                table[i].FinalProbability = 0;
                if (level > 0 && table[i].Level <= 0)
                {
                    continue;
                }
                int rIdx = table[i].Index;
                MonsterRace rPtr = Profile.Instance.MonsterRaces[rIdx];
                if ((rPtr.Flags1 & MonsterFlag1.Unique) != 0 && rPtr.CurNum >= rPtr.MaxNum)
                {
                    continue;
                }
                table[i].FinalProbability = table[i].FilteredProbabiity;
                total += table[i].FinalProbability;
            }
            if (total <= 0)
            {
                return 0;
            }
            long value = Program.Rng.RandomLessThan(total);
            for (i = 0; i < SaveGame.Instance.AllocRaceSize; i++)
            {
                if (value < table[i].FinalProbability)
                {
                    break;
                }
                value -= table[i].FinalProbability;
            }
            int p = Program.Rng.RandomLessThan(100);
            if (p < 60)
            {
                j = i;
                value = Program.Rng.RandomLessThan(total);
                for (i = 0; i < SaveGame.Instance.AllocRaceSize; i++)
                {
                    if (value < table[i].FinalProbability)
                    {
                        break;
                    }
                    value -= table[i].FinalProbability;
                }
                if (table[i].Level < table[j].Level)
                {
                    i = j;
                }
            }
            if (p < 10)
            {
                j = i;
                value = Program.Rng.RandomLessThan(total);
                for (i = 0; i < SaveGame.Instance.AllocRaceSize; i++)
                {
                    if (value < table[i].FinalProbability)
                    {
                        break;
                    }
                    value -= table[i].FinalProbability;
                }
                if (table[i].Level < table[j].Level)
                {
                    i = j;
                }
            }
            return table[i].Index;
        }

        public void GetMonNumPrep()
        {
            for (int i = 0; i < SaveGame.Instance.AllocRaceSize; i++)
            {
                AllocationEntry entry = SaveGame.Instance.AllocRaceTable[i];
                if (GetMonNumHook == null || GetMonNumHook(entry.Index))
                {
                    entry.FilteredProbabiity = entry.BaseProbability;
                }
                else
                {
                    entry.FilteredProbabiity = 0;
                }
            }
        }

        public List<Monster> GetPets()
        {
            List<Monster> list = new List<Monster>();
            foreach (Monster monster in _monsters)
            {
                if ((monster.Mind & Constants.SmFriendly) != 0)
                {
                    list.Add(monster);
                }
            }
            return list;
        }

        public void LoreDoProbe(int mIdx)
        {
            Monster mPtr = _monsters[mIdx];
            MonsterRace rPtr = mPtr.Race;
            rPtr.Knowledge.RFlags1 = rPtr.Flags1;
            rPtr.Knowledge.RFlags2 = rPtr.Flags2;
            rPtr.Knowledge.RFlags3 = rPtr.Flags3;
        }

        public void LoreTreasure(int mIdx, int numItem, int numGold)
        {
            Monster mPtr = _monsters[mIdx];
            MonsterRace rPtr = mPtr.Race;
            if (numItem > rPtr.Knowledge.RDropItem)
            {
                rPtr.Knowledge.RDropItem = numItem;
            }
            if (numGold > rPtr.Knowledge.RDropGold)
            {
                rPtr.Knowledge.RDropGold = numGold;
            }
            if ((rPtr.Flags1 & MonsterFlag1.DropGood) != 0)
            {
                rPtr.Knowledge.RFlags1 |= MonsterFlag1.DropGood;
            }
            if ((rPtr.Flags1 & MonsterFlag1.DropGreat) != 0)
            {
                rPtr.Knowledge.RFlags1 |= MonsterFlag1.DropGreat;
            }
        }

        public void MessagePain(int mIdx, int dam)
        {
            Monster mPtr = _monsters[mIdx];
            MonsterRace rPtr = mPtr.Race;
            string mName = mPtr.MonsterDesc(0);
            if (dam == 0)
            {
                Profile.Instance.MsgPrint($"{mName} is unharmed.");
                return;
            }
            long newhp = mPtr.Health;
            long oldhp = newhp + dam;
            long tmp = newhp * 100L / oldhp;
            int percentage = (int)tmp;
            if ("jmvQ".Contains(rPtr.Character.ToString()))
            {
                if (percentage > 95)
                {
                    Profile.Instance.MsgPrint($"{mName} barely notices.");
                }
                else if (percentage > 75)
                {
                    Profile.Instance.MsgPrint($"{mName} flinches.");
                }
                else if (percentage > 50)
                {
                    Profile.Instance.MsgPrint($"{mName} squelches.");
                }
                else if (percentage > 35)
                {
                    Profile.Instance.MsgPrint($"{mName} quivers in pain.");
                }
                else if (percentage > 20)
                {
                    Profile.Instance.MsgPrint($"{mName} writhes about.");
                }
                else if (percentage > 10)
                {
                    Profile.Instance.MsgPrint($"{mName} writhes in agony.");
                }
                else
                {
                    Profile.Instance.MsgPrint($"{mName} jerks limply.");
                }
            }
            else if ("CZ".Contains(rPtr.Character.ToString()))
            {
                if (percentage > 95)
                {
                    Profile.Instance.MsgPrint($"{mName} shrugs off the attack.");
                }
                else if (percentage > 75)
                {
                    Profile.Instance.MsgPrint($"{mName} snarls with pain.");
                }
                else if (percentage > 50)
                {
                    Profile.Instance.MsgPrint($"{mName} yelps in pain.");
                }
                else if (percentage > 35)
                {
                    Profile.Instance.MsgPrint($"{mName} howls in pain.");
                }
                else if (percentage > 20)
                {
                    Profile.Instance.MsgPrint($"{mName} howls in agony.");
                }
                else if (percentage > 10)
                {
                    Profile.Instance.MsgPrint($"{mName} writhes in agony.");
                }
                else
                {
                    Profile.Instance.MsgPrint($"{mName} yelps feebly.");
                }
            }
            else if ("FIKMRSXabclqrst".Contains(rPtr.Character.ToString()))
            {
                if (percentage > 95)
                {
                    Profile.Instance.MsgPrint($"{mName} ignores the attack.");
                }
                else if (percentage > 75)
                {
                    Profile.Instance.MsgPrint($"{mName} grunts with pain.");
                }
                else if (percentage > 50)
                {
                    Profile.Instance.MsgPrint($"{mName} squeals in pain.");
                }
                else if (percentage > 35)
                {
                    Profile.Instance.MsgPrint($"{mName} shrieks in pain.");
                }
                else if (percentage > 20)
                {
                    Profile.Instance.MsgPrint($"{mName} shrieks in agony.");
                }
                else if (percentage > 10)
                {
                    Profile.Instance.MsgPrint($"{mName} writhes in agony.");
                }
                else
                {
                    Profile.Instance.MsgPrint($"{mName} cries out feebly.");
                }
            }
            else
            {
                if (percentage > 95)
                {
                    Profile.Instance.MsgPrint($"{mName} shrugs off the attack.");
                }
                else if (percentage > 75)
                {
                    Profile.Instance.MsgPrint($"{mName} grunts with pain.");
                }
                else if (percentage > 50)
                {
                    Profile.Instance.MsgPrint($"{mName} cries out in pain.");
                }
                else if (percentage > 35)
                {
                    Profile.Instance.MsgPrint($"{mName} screams in pain.");
                }
                else if (percentage > 20)
                {
                    Profile.Instance.MsgPrint($"{mName} screams in agony.");
                }
                else if (percentage > 10)
                {
                    Profile.Instance.MsgPrint($"{mName} writhes in agony.");
                }
                else
                {
                    Profile.Instance.MsgPrint($"{mName} cries out feebly.");
                }
            }
        }

        public bool MultiplyMonster(int mIdx, bool charm, bool clone)
        {
            Monster mPtr = _monsters[mIdx];
            bool result = false;
            for (int i = 0; i < 18; i++)
            {
                int d = 1;
                _level.Scatter(out int y, out int x, mPtr.MapY, mPtr.MapX, d);
                if (!_level.GridPassableNoCreature(y, x))
                {
                    continue;
                }
                result = PlaceMonsterAux(y, x, mPtr.Race, false, false, charm);
                break;
            }
            if (clone && result)
            {
                _monsters[_hackMIdxIi].Mind |= Constants.SmCloned;
            }
            mPtr.Generation++;
            _monsters[_hackMIdxIi].Generation = mPtr.Generation;
            return result;
        }

        public bool PlaceMonster(int y, int x, bool slp, bool grp)
        {
            int rIdx = GetMonNum(_level.MonsterLevel);
            if (rIdx == 0)
            {
                return false;
            }
            if (PlaceMonsterByIndex(y, x, rIdx, slp, grp, false))
            {
                return true;
            }
            return false;
        }

        public bool PlaceMonsterAux(int y, int x, MonsterRace rPtr, bool slp, bool grp, bool charm)
        {
            if (!PlaceMonsterOne(y, x, rPtr, slp, charm))
            {
                return false;
            }
            if ((rPtr.Flags1 & MonsterFlag1.Escorted) != 0)
            {
                for (int i = 0; i < 50; i++)
                {
                    int d = 3;
                    _level.Scatter(out int ny, out int nx, y, x, d);
                    if (!_level.GridPassableNoCreature(ny, nx))
                    {
                        continue;
                    }
                    _placeMonsterIdx = rPtr.Index;
                    GetMonNumHook = PlaceMonsterOkay;
                    GetMonNumPrep();
                    int z = GetMonNum(rPtr.Level);
                    GetMonNumHook = null;
                    GetMonNumPrep();
                    if (z == 0)
                    {
                        break;
                    }
                    MonsterRace race = Profile.Instance.MonsterRaces[z];
                    PlaceMonsterOne(ny, nx, race, slp, charm);
                    if ((race.Flags1 & MonsterFlag1.Friends) != 0 ||
                        (rPtr.Flags1 & MonsterFlag1.EscortsGroup) != 0)
                    {
                        PlaceMonsterGroup(ny, nx, z, slp, charm);
                    }
                }
            }
            if (!grp)
            {
                return true;
            }
            if ((rPtr.Flags1 & MonsterFlag1.Friends) != 0)
            {
                PlaceMonsterGroup(y, x, rPtr.Index, slp, charm);
            }
            return true;
        }

        public bool PlaceMonsterByIndex(int y, int x, int index, bool slp, bool grp, bool charm)
        {
            return PlaceMonsterAux(y, x, Profile.Instance.MonsterRaces[index], slp, grp, charm);
        }

        public void ReplacePet(int y1, int x1, Monster monster)
        {
            int i;
            int x = x1;
            int y = y1;
            for (i = 0; i < 20; ++i)
            {
                int d = (i / 15) + 1;
                _level.Scatter(out y, out x, y1, x1, d);
                if (!_level.GridPassableNoCreature(y, x))
                {
                    continue;
                }
                if (_level.Grid[y][x].FeatureType.Category == FloorTileTypeCategory.Sigil)
                {
                    continue;
                }
                break;
            }
            if (i == 20)
            {
                Profile.Instance.MsgPrint($"You lose sight of {monster.MonsterDesc(0)}.");
                return;
            }
            GridTile cPtr = _level.Grid[y][x];
            cPtr.Monster = MPop();
            if (cPtr.Monster == 0)
            {
                Profile.Instance.MsgPrint($"You lose sight of {monster.MonsterDesc(0)}.");
                return;
            }
            _monsters[cPtr.Monster] = monster;
            monster.MapY = y;
            monster.MapX = x;
            MonsterRace rPtr = monster.Race;
            if ((rPtr.Flags2 & MonsterFlag2.Multiply) != 0)
            {
                NumRepro++;
            }
            if ((rPtr.Flags1 & MonsterFlag1.AttrMulti) != 0)
            {
                ShimmerMonsters = true;
            }
        }

        public bool SummonSpecific(int y1, int x1, int lev, int type)
        {
            int i;
            int x = x1;
            int y = y1;
            bool groupOk = true;
            for (i = 0; i < 20; ++i)
            {
                int d = (i / 15) + 1;
                _level.Scatter(out y, out x, y1, x1, d);
                if (!_level.GridPassableNoCreature(y, x))
                {
                    continue;
                }
                if (_level.Grid[y][x].FeatureType.Category == FloorTileTypeCategory.Sigil)
                {
                    continue;
                }
                break;
            }
            if (i == 20)
            {
                return false;
            }
            _summonSpecificType = type;
            GetMonNumHook = SummonSpecificOkay;
            GetMonNumPrep();
            int rIdx = GetMonNum(((SaveGame.Instance.Difficulty + lev) / 2) + 5);
            GetMonNumHook = null;
            GetMonNumPrep();
            if (rIdx == 0)
            {
                return false;
            }
            MonsterRace race = Profile.Instance.MonsterRaces[rIdx];
            if (type == Constants.SummonAvatar)
            {
                groupOk = false;
            }
            if (!PlaceMonsterAux(y, x, race, false, groupOk, false))
            {
                return false;
            }
            return true;
        }

        public bool SummonSpecificFriendly(int y1, int x1, int lev, int type, bool groupOk)
        {
            int i;
            int x = 0;
            int y = 0;
            for (i = 0; i < 20; ++i)
            {
                int d = (i / 15) + 1;
                _level.Scatter(out y, out x, y1, x1, d);
                if (!_level.GridPassableNoCreature(y, x))
                {
                    continue;
                }
                if (_level.Grid[y][x].FeatureType.Name == "ElderSign")
                {
                    continue;
                }
                if (_level.Grid[y][x].FeatureType.Name == "YellowSign")
                {
                    continue;
                }
                break;
            }
            if (i == 20)
            {
                return false;
            }
            _summonSpecificType = type;
            GetMonNumHook = SummonSpecificOkay;
            GetMonNumPrep();
            int rIdx = GetMonNum(((SaveGame.Instance.Difficulty + lev) / 2) + 5);
            GetMonNumHook = null;
            GetMonNumPrep();
            if (rIdx == 0)
            {
                return false;
            }
            MonsterRace race = Profile.Instance.MonsterRaces[rIdx];
            if (!PlaceMonsterAux(y, x, race, false, groupOk, true))
            {
                return false;
            }
            return true;
        }

        public void UpdateMonsterVisibility(int mIdx, bool full)
        {
            Monster mPtr = _monsters[mIdx];
            MonsterRace rPtr = mPtr.Race;
            if (rPtr == null)
            {
                return;
            }
            int fy = mPtr.MapY;
            int fx = mPtr.MapX;
            if (full)
            {
                int dy = SaveGame.Instance.Player.MapY > fy
                    ? SaveGame.Instance.Player.MapY - fy
                    : fy - SaveGame.Instance.Player.MapY;
                int dx = SaveGame.Instance.Player.MapX > fx
                    ? SaveGame.Instance.Player.MapX - fx
                    : fx - SaveGame.Instance.Player.MapX;
                int d = dy > dx ? dy + (dx >> 1) : dx + (dy >> 1);
                mPtr.DistanceFromPlayer = d < 255 ? d : 255;
            }
            bool flag = false;
            bool easy = false;
            bool hard = false;
            bool doEmptyMind = false;
            bool doWeirdMind = false;
            bool doInvisible = false;
            bool doColdBlood = false;
            bool oldMl = mPtr.IsVisible;
            if (mPtr.DistanceFromPlayer > Constants.MaxSight)
            {
                if (!mPtr.IsVisible)
                {
                    return;
                }
                if ((mPtr.IndividualMonsterFlags & Constants.MflagMark) != 0)
                {
                    flag = true;
                }
            }
            else if (_level.PanelContains(fy, fx))
            {
                GridTile cPtr = _level.Grid[fy][fx];
                if (cPtr.TileFlags.IsSet(GridTile.IsVisible) && SaveGame.Instance.Player.TimedBlindness == 0)
                {
                    if (mPtr.DistanceFromPlayer <= SaveGame.Instance.Player.InfravisionRange)
                    {
                        if ((rPtr.Flags2 & MonsterFlag2.ColdBlood) != 0)
                        {
                            doColdBlood = true;
                        }
                        if (!doColdBlood)
                        {
                            easy = true;
                            flag = true;
                        }
                    }
                    if (cPtr.TileFlags.IsSet(GridTile.PlayerLit | GridTile.SelfLit))
                    {
                        if ((rPtr.Flags2 & MonsterFlag2.Invisible) != 0)
                        {
                            doInvisible = true;
                        }
                        if (!doInvisible || SaveGame.Instance.Player.HasSeeInvisibility)
                        {
                            easy = true;
                            flag = true;
                        }
                    }
                }
                if (SaveGame.Instance.Player.HasTelepathy)
                {
                    if ((rPtr.Flags2 & MonsterFlag2.EmptyMind) != 0)
                    {
                        doEmptyMind = true;
                    }
                    else if ((rPtr.Flags2 & MonsterFlag2.WeirdMind) != 0)
                    {
                        doWeirdMind = true;
                        if (Program.Rng.RandomLessThan(100) < 10)
                        {
                            hard = true;
                            flag = true;
                        }
                    }
                    else
                    {
                        hard = true;
                        flag = true;
                    }
                }
                if ((mPtr.IndividualMonsterFlags & Constants.MflagMark) != 0)
                {
                    flag = true;
                }
                if (SaveGame.Instance.Player.IsWizard)
                {
                    flag = true;
                }
            }
            if (flag)
            {
                if (!mPtr.IsVisible)
                {
                    mPtr.IsVisible = true;
                    _level.RedrawSingleLocation(fy, fx);
                    if (SaveGame.Instance.TrackedMonsterIndex == mIdx)
                    {
                        SaveGame.Instance.Player.RedrawFlags |= RedrawFlag.PrHealth;
                    }
                    if (rPtr.Knowledge.RSights < Constants.MaxShort)
                    {
                        rPtr.Knowledge.RSights++;
                    }
                }
                if (hard)
                {
                    if ((rPtr.Flags2 & MonsterFlag2.Smart) != 0)
                    {
                        rPtr.Knowledge.RFlags2 |= MonsterFlag2.Smart;
                    }
                    if ((rPtr.Flags2 & MonsterFlag2.Stupid) != 0)
                    {
                        rPtr.Knowledge.RFlags2 |= MonsterFlag2.Stupid;
                    }
                }
                if (doEmptyMind)
                {
                    rPtr.Knowledge.RFlags2 |= MonsterFlag2.EmptyMind;
                }
                if (doWeirdMind)
                {
                    rPtr.Knowledge.RFlags2 |= MonsterFlag2.WeirdMind;
                }
                if (doColdBlood)
                {
                    rPtr.Knowledge.RFlags2 |= MonsterFlag2.ColdBlood;
                }
                if (doInvisible)
                {
                    rPtr.Knowledge.RFlags2 |= MonsterFlag2.Invisible;
                }
            }
            else
            {
                if (mPtr.IsVisible)
                {
                    mPtr.IsVisible = false;
                    _level.RedrawSingleLocation(fy, fx);
                    if (SaveGame.Instance.TrackedMonsterIndex == mIdx)
                    {
                        SaveGame.Instance.Player.RedrawFlags |= RedrawFlag.PrHealth;
                    }
                }
            }
            if (easy)
            {
                if (mPtr.IsVisible != oldMl)
                {
                    if ((rPtr.Flags2 & MonsterFlag2.EldritchHorror) != 0)
                    {
                        mPtr.SanityBlast(false);
                    }
                }
                if ((mPtr.IndividualMonsterFlags & Constants.MflagView) == 0)
                {
                    mPtr.IndividualMonsterFlags |= Constants.MflagView;
                    if ((mPtr.Mind & Constants.SmFriendly) == 0)
                    {
                        SaveGame.Instance.Disturb(true);
                    }
                }
            }
            else
            {
                if ((mPtr.IndividualMonsterFlags & Constants.MflagView) != 0)
                {
                    mPtr.IndividualMonsterFlags &= ~Constants.MflagView;
                    if ((mPtr.Mind & Constants.SmFriendly) == 0)
                    {
                        SaveGame.Instance.Disturb(true);
                    }
                }
            }
        }

        public void UpdateSmartLearn(int mIdx, int what)
        {
            Player player = SaveGame.Instance.Player;
            Monster mPtr = _monsters[mIdx];
            MonsterRace rPtr = mPtr.Race;
            if ((rPtr.Flags2 & MonsterFlag2.Stupid) != 0)
            {
                return;
            }
            if ((rPtr.Flags2 & MonsterFlag2.Smart) == 0 && Program.Rng.RandomLessThan(100) < 50)
            {
                return;
            }
            switch (what)
            {
                case Constants.DrsAcid:
                    if (player.HasAcidResistance)
                    {
                        mPtr.Mind |= Constants.SmResAcid;
                    }
                    if (player.TimedAcidResistance != 0)
                    {
                        mPtr.Mind |= Constants.SmOppAcid;
                    }
                    if (player.HasAcidImmunity)
                    {
                        mPtr.Mind |= Constants.SmImmAcid;
                    }
                    break;

                case Constants.DrsElec:
                    if (player.HasLightningResistance)
                    {
                        mPtr.Mind |= Constants.SmResElec;
                    }
                    if (player.TimedLightningResistance != 0)
                    {
                        mPtr.Mind |= Constants.SmOppElec;
                    }
                    if (player.HasLightningImmunity)
                    {
                        mPtr.Mind |= Constants.SmImmElec;
                    }
                    break;

                case Constants.DrsFire:
                    if (player.HasFireResistance)
                    {
                        mPtr.Mind |= Constants.SmResFire;
                    }
                    if (player.TimedFireResistance != 0)
                    {
                        mPtr.Mind |= Constants.SmOppFire;
                    }
                    if (player.HasFireImmunity)
                    {
                        mPtr.Mind |= Constants.SmImmFire;
                    }
                    break;

                case Constants.DrsCold:
                    if (player.HasColdResistance)
                    {
                        mPtr.Mind |= Constants.SmResCold;
                    }
                    if (player.TimedColdResistance != 0)
                    {
                        mPtr.Mind |= Constants.SmOppCold;
                    }
                    if (player.HasColdImmunity)
                    {
                        mPtr.Mind |= Constants.SmImmCold;
                    }
                    break;

                case Constants.DrsPois:
                    if (player.HasPoisonResistance)
                    {
                        mPtr.Mind |= Constants.SmResPois;
                    }
                    if (player.TimedPoisonResistance != 0)
                    {
                        mPtr.Mind |= Constants.SmOppPois;
                    }
                    break;

                case Constants.DrsNeth:
                    if (player.HasNetherResistance)
                    {
                        mPtr.Mind |= Constants.SmResNeth;
                    }
                    break;

                case Constants.DrsLight:
                    if (player.HasLightResistance)
                    {
                        mPtr.Mind |= Constants.SmResLight;
                    }
                    break;

                case Constants.DrsDark:
                    if (player.HasDarkResistance)
                    {
                        mPtr.Mind |= Constants.SmResDark;
                    }
                    break;

                case Constants.DrsFear:
                    if (player.HasFearResistance)
                    {
                        mPtr.Mind |= Constants.SmResFear;
                    }
                    break;

                case Constants.DrsConf:
                    if (player.HasConfusionResistance)
                    {
                        mPtr.Mind |= Constants.SmResConf;
                    }
                    break;

                case Constants.DrsChaos:
                    if (player.HasChaosResistance)
                    {
                        mPtr.Mind |= Constants.SmResChaos;
                    }
                    break;

                case Constants.DrsDisen:
                    if (player.HasDisenchantResistance)
                    {
                        mPtr.Mind |= Constants.SmResDisen;
                    }
                    break;

                case Constants.DrsBlind:
                    if (player.HasBlindnessResistance)
                    {
                        mPtr.Mind |= Constants.SmResBlind;
                    }
                    break;

                case Constants.DrsNexus:
                    if (player.HasNexusResistance)
                    {
                        mPtr.Mind |= Constants.SmResNexus;
                    }
                    break;

                case Constants.DrsSound:
                    if (player.HasSoundResistance)
                    {
                        mPtr.Mind |= Constants.SmResSound;
                    }
                    break;

                case Constants.DrsShard:
                    if (player.HasShardResistance)
                    {
                        mPtr.Mind |= Constants.SmResShard;
                    }
                    break;

                case Constants.DrsFree:
                    if (player.HasFreeAction)
                    {
                        mPtr.Mind |= Constants.SmImmFree;
                    }
                    break;

                case Constants.DrsMana:
                    if (player.MaxMana == 0)
                    {
                        mPtr.Mind |= Constants.SmImmMana;
                    }
                    break;

                case Constants.DrsReflect:
                    if (player.HasReflection)
                    {
                        mPtr.Mind |= Constants.SmImmReflect;
                    }
                    break;
            }
        }

        public void WipeMList()
        {
            for (int i = _level.MMax - 1; i >= 1; i--)
            {
                Monster mPtr = _monsters[i];
                MonsterRace rPtr = mPtr.Race;
                if (mPtr.Race == null)
                {
                    continue;
                }
                rPtr.CurNum--;
                _level.Grid[mPtr.MapY][mPtr.MapX].Monster = 0;
                _monsters[i] = new Monster();
            }
            _level.MMax = 1;
            _level.MCnt = 0;
            NumRepro = 0;
            SaveGame.Instance.TargetWho = 0;
            SaveGame.Instance.HealthTrack(0);
        }

        private void CompactMonstersAux(int i1, int i2)
        {
            int nextOIdx;
            if (i1 == i2)
            {
                return;
            }
            Monster mPtr = _monsters[i1];
            int y = mPtr.MapY;
            int x = mPtr.MapX;
            GridTile cPtr = _level.Grid[y][x];
            cPtr.Monster = i2;
            for (int thisOIdx = mPtr.FirstHeldItemIndex; thisOIdx != 0; thisOIdx = nextOIdx)
            {
                Item oPtr = _level.Items[thisOIdx];
                nextOIdx = oPtr.NextInStack;
                oPtr.HoldingMonsterIndex = i2;
            }
            if (SaveGame.Instance.TargetWho == i1)
            {
                SaveGame.Instance.TargetWho = i2;
            }
            if (SaveGame.Instance.TrackedMonsterIndex == i1)
            {
                SaveGame.Instance.HealthTrack(i2);
            }
            _monsters[i2] = _monsters[i1];
            _monsters[i1] = new Monster();
        }

        private int MPop()
        {
            int i;
            if (_level.MMax < Constants.MaxMIdx)
            {
                i = _level.MMax;
                _level.MMax++;
                _level.MCnt++;
                return i;
            }
            for (i = 1; i < _level.MMax; i++)
            {
                Monster mPtr = _monsters[i];
                if (mPtr.Race != null)
                {
                    continue;
                }
                _level.MCnt++;
                return i;
            }
            if (_level != null)
            {
                Profile.Instance.MsgPrint("Too many monsters!");
            }
            return 0;
        }

        private void PlaceMonsterGroup(int y, int x, int rIdx, bool slp, bool charm)
        {
            MonsterRace rPtr = Profile.Instance.MonsterRaces[rIdx];
            int extra = 0;
            int[] hackY = new int[Constants.GroupMax];
            int[] hackX = new int[Constants.GroupMax];
            int total = Program.Rng.DieRoll(13);
            if (rPtr.Level > SaveGame.Instance.Difficulty)
            {
                extra = rPtr.Level - SaveGame.Instance.Difficulty;
                extra = 0 - Program.Rng.DieRoll(extra);
            }
            else if (rPtr.Level < SaveGame.Instance.Difficulty)
            {
                extra = SaveGame.Instance.Difficulty - rPtr.Level;
                extra = Program.Rng.DieRoll(extra);
            }
            if (extra > 12)
            {
                extra = 12;
            }
            total += extra;
            if (total < 1)
            {
                total = 1;
            }
            if (total > Constants.GroupMax)
            {
                total = Constants.GroupMax;
            }
            int old = _level.DangerRating;
            int hackN = 1;
            hackX[0] = x;
            hackY[0] = y;
            for (int n = 0; n < hackN && hackN < total; n++)
            {
                int hx = hackX[n];
                int hy = hackY[n];
                for (int i = 0; i < 8 && hackN < total; i++)
                {
                    int mx = hx + _level.OrderedDirectionXOffset[i];
                    int my = hy + _level.OrderedDirectionYOffset[i];
                    if (!_level.GridPassableNoCreature(my, mx))
                    {
                        continue;
                    }
                    if (PlaceMonsterOne(my, mx, rPtr, slp, charm))
                    {
                        hackY[hackN] = my;
                        hackX[hackN] = mx;
                        hackN++;
                    }
                }
            }
            _level.DangerRating = old;
        }

        private bool PlaceMonsterOkay(int rIdx)
        {
            MonsterRace rPtr = Profile.Instance.MonsterRaces[_placeMonsterIdx];
            MonsterRace zPtr = Profile.Instance.MonsterRaces[rIdx];
            if (zPtr.Character != rPtr.Character)
            {
                return false;
            }
            if (zPtr.Level > rPtr.Level)
            {
                return false;
            }
            if ((zPtr.Flags1 & MonsterFlag1.Unique) != 0)
            {
                return false;
            }
            if (_placeMonsterIdx == rIdx)
            {
                return false;
            }
            return true;
        }

        private bool PlaceMonsterOne(int y, int x, MonsterRace rPtr, bool slp, bool charm)
        {
            if (rPtr == null)
            {
                return false;
            }
            if (rPtr.Name.StartsWith("Player"))
            {
                return false;
            }
            string name = rPtr.Name;
            if (!_level.InBounds(y, x))
            {
                return false;
            }
            if (!_level.GridPassableNoCreature(y, x))
            {
                return false;
            }
            if (_level.Grid[y][x].FeatureType.Category == FloorTileTypeCategory.Sigil)
            {
                return false;
            }
            if (string.IsNullOrEmpty(rPtr.Name))
            {
                return false;
            }
            if ((rPtr.Flags1 & MonsterFlag1.Unique) != 0 && rPtr.CurNum >= rPtr.MaxNum)
            {
                return false;
            }
            if ((rPtr.Flags1 & MonsterFlag1.OnlyGuardian) != 0 || (rPtr.Flags1 & MonsterFlag1.Guardian) != 0)
            {
                int qIdx = SaveGame.Instance.Quests.GetQuestNumber();
                if (qIdx < 0)
                {
                    return false;
                }
                if (rPtr.Index != SaveGame.Instance.Quests[qIdx].RIdx)
                {
                    return false;
                }
                if (rPtr.CurNum >= SaveGame.Instance.Quests[qIdx].ToKill - SaveGame.Instance.Quests[qIdx].Killed)
                {
                    return false;
                }
            }
            if (rPtr.Level > SaveGame.Instance.Difficulty)
            {
                if ((rPtr.Flags1 & MonsterFlag1.Unique) != 0)
                {
                    _level.DangerRating += (rPtr.Level - SaveGame.Instance.Difficulty) * 2;
                }
                else
                {
                    _level.DangerRating += rPtr.Level - SaveGame.Instance.Difficulty;
                }
            }
            GridTile cPtr = _level.Grid[y][x];
            cPtr.Monster = MPop();
            _hackMIdxIi = cPtr.Monster;
            if (cPtr.Monster == 0)
            {
                return false;
            }
            Monster mPtr = _monsters[cPtr.Monster];
            mPtr.Race = rPtr;
            mPtr.MapY = y;
            mPtr.MapX = x;
            mPtr.Generation = 1;
            mPtr.StunLevel = 0;
            mPtr.ConfusionLevel = 0;
            mPtr.FearLevel = 0;
            if (charm)
            {
                mPtr.Mind |= Constants.SmFriendly;
            }
            mPtr.SleepLevel = 0;
            if (slp && rPtr.Sleep != 0)
            {
                int val = rPtr.Sleep;
                mPtr.SleepLevel = (val * 2) + Program.Rng.DieRoll(val * 10);
            }
            mPtr.DistanceFromPlayer = 0;
            mPtr.IndividualMonsterFlags = 0;
            mPtr.IsVisible = false;
            mPtr.MaxHealth = (rPtr.Flags1 & MonsterFlag1.ForceMaxHp) != 0
                ? Program.Rng.DiceRollMax(rPtr.Hdice, rPtr.Hside)
                : Program.Rng.DiceRoll(rPtr.Hdice, rPtr.Hside);
            mPtr.Health = mPtr.MaxHealth;
            mPtr.Speed = rPtr.Speed;
            if ((rPtr.Flags1 & MonsterFlag1.Unique) == 0)
            {
                int i = GlobalData.ExtractEnergy[rPtr.Speed] / 10;
                if (i != 0)
                {
                    mPtr.Speed += Program.Rng.RandomSpread(0, i);
                }
            }
            mPtr.Energy = Program.Rng.RandomLessThan(100);
            if ((rPtr.Flags1 & MonsterFlag1.ForceSleep) != 0)
            {
                mPtr.IndividualMonsterFlags |= Constants.MflagNice;
                RepairMonsters = true;
            }
            if (cPtr.Monster < CurrentlyActingMonster)
            {
                mPtr.IndividualMonsterFlags |= Constants.MflagBorn;
            }
            UpdateMonsterVisibility(cPtr.Monster, true);
            rPtr.CurNum++;
            if ((rPtr.Flags2 & MonsterFlag2.Multiply) != 0)
            {
                NumRepro++;
            }
            if ((rPtr.Flags1 & MonsterFlag1.AttrMulti) != 0)
            {
                ShimmerMonsters = true;
            }
            return true;
        }

        private bool SummonSpecificOkay(int rIdx)
        {
            MonsterRace rPtr = Profile.Instance.MonsterRaces[rIdx];
            bool okay = false;
            if (_summonSpecificType == 0)
            {
                return true;
            }
            switch (_summonSpecificType)
            {
                case Constants.SummonAnt:
                    {
                        okay = rPtr.Character == 'a' && (rPtr.Flags1 & MonsterFlag1.Unique) == 0;
                        break;
                    }
                case Constants.SummonSpider:
                    {
                        okay = rPtr.Character == 'S' && (rPtr.Flags1 & MonsterFlag1.Unique) == 0;
                        break;
                    }
                case Constants.SummonHound:
                    {
                        okay = (rPtr.Character == 'C' || rPtr.Character == 'Z') && (rPtr.Flags1 & MonsterFlag1.Unique) == 0;
                        break;
                    }
                case Constants.SummonHydra:
                    {
                        okay = rPtr.Character == 'M' && (rPtr.Flags1 & MonsterFlag1.Unique) == 0;
                        break;
                    }
                case Constants.SummonCthuloid:
                    {
                        okay = (rPtr.Flags3 & MonsterFlag3.Cthuloid) != 0 && (rPtr.Flags1 & MonsterFlag1.Unique) == 0;
                        break;
                    }
                case Constants.SummonDemon:
                    {
                        okay = (rPtr.Flags3 & MonsterFlag3.Demon) != 0 && (rPtr.Flags1 & MonsterFlag1.Unique) == 0;
                        break;
                    }
                case Constants.SummonUndead:
                    {
                        okay = (rPtr.Flags3 & MonsterFlag3.Undead) != 0 && (rPtr.Flags1 & MonsterFlag1.Unique) == 0;
                        break;
                    }
                case Constants.SummonDragon:
                    {
                        okay = (rPtr.Flags3 & MonsterFlag3.Dragon) != 0 && (rPtr.Flags1 & MonsterFlag1.Unique) == 0;
                        break;
                    }
                case Constants.SummonHiUndead:
                    {
                        okay = rPtr.Character == 'L' || rPtr.Character == 'V' || rPtr.Character == 'W';
                        break;
                    }
                case Constants.SummonHiDragon:
                    {
                        okay = rPtr.Character == 'D';
                        break;
                    }
                case Constants.SummonGoo:
                    {
                        okay = (rPtr.Flags3 & MonsterFlag3.GreatOldOne) != 0;
                        break;
                    }
                case Constants.SummonUnique:
                    {
                        okay = (rPtr.Flags1 & MonsterFlag1.Unique) != 0;
                        break;
                    }
                case Constants.SummonOrc:
                    {
                        okay = rPtr.Character == 'o' && (rPtr.Flags1 & MonsterFlag1.Unique) == 0;
                        break;
                    }
                case Constants.SummonKobold:
                    {
                        okay = rPtr.Character == 'k' && (rPtr.Flags1 & MonsterFlag1.Unique) == 0;
                        break;
                    }
                case Constants.SummonYeek:
                    {
                        okay = rPtr.Character == 'y' && (rPtr.Flags1 & MonsterFlag1.Unique) == 0;
                        break;
                    }
                case Constants.SummonHuman:
                    {
                        okay = rPtr.Character == 'p' && (rPtr.Flags1 & MonsterFlag1.Unique) == 0;
                        break;
                    }
                case Constants.SummonBizarre1:
                    {
                        okay = rPtr.Character == 'm' && (rPtr.Flags1 & MonsterFlag1.Unique) == 0;
                        break;
                    }
                case Constants.SummonBizarre2:
                    {
                        okay = rPtr.Character == 'b' && (rPtr.Flags1 & MonsterFlag1.Unique) == 0;
                        break;
                    }
                case Constants.SummonBizarre3:
                    {
                        okay = rPtr.Character == 'Q' && (rPtr.Flags1 & MonsterFlag1.Unique) == 0;
                        break;
                    }
                case Constants.SummonBizarre4:
                    {
                        okay = rPtr.Character == 'v' && (rPtr.Flags1 & MonsterFlag1.Unique) == 0;
                        break;
                    }
                case Constants.SummonBizarre5:
                    {
                        okay = rPtr.Character == '$' && (rPtr.Flags1 & MonsterFlag1.Unique) == 0;
                        break;
                    }
                case Constants.SummonBizarre6:
                    {
                        okay = (rPtr.Character == '!' || rPtr.Character == '?' || rPtr.Character == '=' || rPtr.Character == '$' ||
                                rPtr.Character == '|') && (rPtr.Flags1 & MonsterFlag1.Unique) == 0;
                        break;
                    }
                case Constants.SummonReaver:
                    {
                        okay = rPtr.Name == "Black reaver" && (rPtr.Flags1 & MonsterFlag1.Unique) == 0;
                        break;
                    }
                case Constants.SummonKin:
                    {
                        okay = rPtr.Character == SummonKinType && (rPtr.Flags1 & MonsterFlag1.Unique) == 0;
                        break;
                    }
                case Constants.SummonAvatar:
                    {
                        okay = rPtr.Name == "Avatar of Nyarlathotep" && (rPtr.Flags1 & MonsterFlag1.Unique) == 0;
                        break;
                    }
                case Constants.SummonAnimal:
                    {
                        okay = (rPtr.Flags3 & MonsterFlag3.Animal) != 0 && (rPtr.Flags1 & MonsterFlag1.Unique) == 0;
                        break;
                    }
                case Constants.SummonAnimalRanger:
                    {
                        okay = (rPtr.Flags3 & MonsterFlag3.Animal) != 0 &&
                               "abcflqrwBCIJKMRS".Contains(rPtr.Character.ToString()) &&
                               (rPtr.Flags3 & MonsterFlag3.Dragon) == 0 && (rPtr.Flags3 & MonsterFlag3.Evil) == 0 &&
                               (rPtr.Flags3 & MonsterFlag3.Undead) == 0 && (rPtr.Flags3 & MonsterFlag3.Demon) == 0 &&
                               (rPtr.Flags3 & MonsterFlag3.Cthuloid) == 0 && rPtr.Flags4 == 0 && rPtr.Flags5 == 0 &&
                               rPtr.Flags6 == 0 && (rPtr.Flags1 & MonsterFlag1.Unique) == 0;
                        break;
                    }
                case Constants.SummonHiUndeadNoUniques:
                    {
                        okay = (rPtr.Character == 'L' || rPtr.Character == 'V' || rPtr.Character == 'W') &&
                               (rPtr.Flags1 & MonsterFlag1.Unique) == 0;
                        break;
                    }
                case Constants.SummonHiDragonNoUniques:
                    {
                        okay = rPtr.Character == 'D' && (rPtr.Flags1 & MonsterFlag1.Unique) == 0;
                        break;
                    }
                case Constants.SummonNoUniques:
                    {
                        okay = (rPtr.Flags1 & MonsterFlag1.Unique) == 0;
                        break;
                    }
                case Constants.SummonPhantom:
                    {
                        okay = rPtr.Name.Contains("Phantom") && (rPtr.Flags1 & MonsterFlag1.Unique) == 0;
                        break;
                    }
                case Constants.SummonElemental:
                    {
                        okay = rPtr.Name.Contains("lemental") && (rPtr.Flags1 & MonsterFlag1.Unique) == 0;
                        break;
                    }
            }
            return okay;
        }
    }
}