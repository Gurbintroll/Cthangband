using Cthangband.Enumerations;
using Cthangband.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cthangband
{
    [Serializable]
    internal class QuestArray : List<Quest>
    {
        private const int _maxQuests = 50;

        private readonly string[] _findQuest =
        {
            "You find the following inscription in the floor",
            "You see a message inscribed in the wall",
            "There is a sign saying",
            "Something is writen on the staircase",
            "You find a scroll with the following message"
        };

        public QuestArray()
        {
            for (int i = 0; i < _maxQuests; i++)
            {
                Add(new Quest());
            }
        }

        public int ActiveQuests => this.Where(q => q.IsActive).Count();

        public string DescribeQuest(int qIdx)
        {
            string buf;
            MonsterRace rPtr = Profile.Instance.MonsterRaces[this[qIdx].RIdx];
            string name = rPtr.Name;
            int qNum = this[qIdx].ToKill;
            string dunName = SaveGame.Instance.Dungeons[this[qIdx].Dungeon].Name;
            int lev = this[qIdx].Level;
            if (this[qIdx].Level == 0)
            {
                if (qNum == 1)
                {
                    buf = $"You have defeated {name} in {dunName}";
                }
                else
                {
                    string plural = name.PluraliseMonsterName();
                    buf = $"You have defeated {qNum} {plural} in {dunName}";
                }
            }
            else
            {
                if (this[qIdx].Discovered)
                {
                    if (qNum == 1)
                    {
                        buf = $"You must defeat {name} at lvl {lev} of {dunName}";
                    }
                    else
                    {
                        if (this[qIdx].ToKill - this[qIdx].Killed > 1)
                        {
                            string plural = name.PluraliseMonsterName();
                            buf = $"You must defeat {qNum} {plural} at lvl {lev} of {dunName}";
                        }
                        else
                        {
                            buf = $"You must defeat 1 {name} at lvl {lev} of {dunName}";
                        }
                    }
                }
                else
                {
                    buf = $"You must defeat something at lvl {lev} of {dunName}";
                }
            }
            return buf;
        }

        public int GetQuestMonster()
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Level == SaveGame.Instance.DunLevel &&
                    this[i].Dungeon == SaveGame.Instance.CurDungeon.Index)
                {
                    return this[i].RIdx;
                }
            }
            return 0;
        }

        public int GetQuestNumber()
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Level == SaveGame.Instance.DunLevel &&
                    this[i].Dungeon == SaveGame.Instance.CurDungeon.Index)
                {
                    return i;
                }
            }
            return -1;
        }

        public bool IsQuest(int level)
        {
            if (level == 0)
            {
                return false;
            }
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Level == level && this[i].Dungeon == SaveGame.Instance.CurDungeon.Index)
                {
                    return true;
                }
            }
            return false;
        }

        public void PlayerBirthQuests()
        {
            Profile.Instance.MonsterRaces.ResetUniqueOnlyGuardianStatus();
            int index = 0;
            Clear();
            for (int i = 0; i < _maxQuests; i++)
            {
                Add(new Quest());
            }
            for (int i = 0; i < Constants.MaxCaves; i++)
            {
                if (SaveGame.Instance.Dungeons[i].FirstGuardian != "")
                {
                    this[index].Level = SaveGame.Instance.Dungeons[i].FirstLevel;
                    this[index].RIdx =
                        Profile.Instance.MonsterRaces.IndexFromName(SaveGame.Instance.Dungeons[i].FirstGuardian);
                    Profile.Instance.MonsterRaces[this[index].RIdx].Flags1 |= MonsterFlag1.OnlyGuardian;
                    this[index].Dungeon = i;
                    this[index].ToKill = 1;
                    this[index].Killed = 0;
                    index++;
                }
                if (SaveGame.Instance.Dungeons[i].SecondGuardian != "")
                {
                    this[index].Level = SaveGame.Instance.Dungeons[i].SecondLevel;
                    this[index].RIdx = Profile.Instance.MonsterRaces.IndexFromName(SaveGame.Instance.Dungeons[i].SecondGuardian);
                    Profile.Instance.MonsterRaces[this[index].RIdx].Flags1 |= MonsterFlag1.OnlyGuardian;
                    this[index].Dungeon = i;
                    this[index].ToKill = 1;
                    this[index].Killed = 0;
                    index++;
                }
            }
            for (int i = 0; i < 26; i++)
            {
                int j;
                bool sameLevel;
                do
                {
                    sameLevel = false;
                    do
                    {
                        this[index].RIdx = GetRndQMonster(index);
                    } while (this[index].RIdx == 0);
                    this[index].Level = Profile.Instance.MonsterRaces[this[index].RIdx].Level;
                    this[index].Level -= Program.Rng.RandomBetween(2, 3 + (this[index].Level / 6));
                    for (j = 0; j < index; j++)
                    {
                        if (this[index].Level == this[j].Level)
                        {
                            sameLevel = true;
                            break;
                        }
                    }
                } while (sameLevel);
                if ((Profile.Instance.MonsterRaces[this[index].RIdx].Flags1 & MonsterFlag1.Unique) != 0)
                {
                    Profile.Instance.MonsterRaces[this[index].RIdx].Flags1 |= MonsterFlag1.OnlyGuardian;
                }
                j = Program.Rng.RandomBetween(1, Constants.MaxCaves) - 1;
                while (this[index].Level <= SaveGame.Instance.Dungeons[j].Offset ||
                       this[index].Level >
                       SaveGame.Instance.Dungeons[j].MaxLevel + SaveGame.Instance.Dungeons[j].Offset ||
                       this[index].Level == SaveGame.Instance.Dungeons[j].FirstLevel +
                       SaveGame.Instance.Dungeons[j].Offset || this[index].Level ==
                       SaveGame.Instance.Dungeons[j].SecondLevel + SaveGame.Instance.Dungeons[j].Offset)
                {
                    j = Program.Rng.RandomBetween(1, Constants.MaxCaves) - 1;
                }
                this[index].Dungeon = j;
                this[index].Level -= SaveGame.Instance.Dungeons[j].Offset;
                this[index].ToKill = GetNumberMonster(index);
                this[index].Killed = 0;
                index++;
            }
        }

        public void PrintQuestMessage()
        {
            int qIdx = GetQuestNumber();
            MonsterRace rPtr = Profile.Instance.MonsterRaces[this[qIdx].RIdx];
            string name = rPtr.Name;
            int qNum = this[qIdx].ToKill - this[qIdx].Killed;
            if (this[qIdx].ToKill == 1)
            {
                Profile.Instance.MsgPrint($"You still have to kill {name}.");
            }
            else if (qNum > 1)
            {
                string plural = name.PluraliseMonsterName();
                Profile.Instance.MsgPrint($"You still have to kill {qNum} {plural}.");
            }
            else
            {
                Profile.Instance.MsgPrint($"You still have to kill 1 {name}.");
            }
        }

        public void QuestDiscovery()
        {
            int qIdx = GetQuestNumber();
            MonsterRace rPtr = Profile.Instance.MonsterRaces[this[qIdx].RIdx];
            string name = rPtr.Name;
            int qNum = this[qIdx].ToKill;
            Profile.Instance.MsgPrint(_findQuest[Program.Rng.RandomBetween(0, 4)]);
            Profile.Instance.MsgPrint(null);
            if (qNum == 1)
            {
                Profile.Instance.MsgPrint($"Beware, this level is protected by {name}!");
            }
            else
            {
                string plural = name.PluraliseMonsterName();
                Profile.Instance.MsgPrint($"Be warned, this level is guarded by {qNum} {plural}!");
            }
            this[qIdx].Discovered = true;
        }

        private int GetNumberMonster(int i)
        {
            if ((Profile.Instance.MonsterRaces[this[i].RIdx].Flags1 & MonsterFlag1.Unique) != 0 ||
                (Profile.Instance.MonsterRaces[this[i].RIdx].Flags2 & MonsterFlag2.Multiply) != 0)
            {
                return 1;
            }
            int num = (Profile.Instance.MonsterRaces[this[i].RIdx].Flags1 & MonsterFlag1.Friends) != 0 ? 10 : 5;
            num += Program.Rng.RandomBetween(1, (this[i].Level / 3) + 5);
            return num;
        }

        private int GetRndQMonster(int qIdx)
        {
            int rIdx;
            int tmp = Program.Rng.RandomBetween(1, 10);
            switch (tmp)
            {
                case 1:
                    rIdx = Program.Rng.RandomBetween(181, 220);
                    break;

                case 2:
                    rIdx = Program.Rng.RandomBetween(221, 260);
                    break;

                case 3:
                    rIdx = Program.Rng.RandomBetween(261, 300);
                    break;

                case 4:
                    rIdx = Program.Rng.RandomBetween(301, 340);
                    break;

                case 5:
                    rIdx = Program.Rng.RandomBetween(341, 380);
                    break;

                case 6:
                    rIdx = Program.Rng.RandomBetween(381, 420);
                    break;

                case 7:
                    rIdx = Program.Rng.RandomBetween(421, 460);
                    break;

                case 8:
                    rIdx = Program.Rng.RandomBetween(461, 500);
                    break;

                case 9:
                    rIdx = Program.Rng.RandomBetween(501, 530);
                    break;

                case 10:
                    rIdx = Program.Rng.RandomBetween(531, 560);
                    break;

                default:
                    rIdx = Program.Rng.RandomBetween(87, 573);
                    break;
            }
            if ((Profile.Instance.MonsterRaces[rIdx].Flags2 & MonsterFlag2.Multiply) != 0)
            {
                return 0;
            }
            for (int j = 2; j < qIdx; j++)
            {
                if (this[j].RIdx == rIdx)
                {
                    return 0;
                }
            }
            return rIdx;
        }
    }
}