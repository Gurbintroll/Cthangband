using Cthangband.Enumerations;
using Cthangband.Pantheon;
using Cthangband.StaticData;
using Cthangband.UI;
using System.Collections.Generic;

namespace Cthangband
{
    internal class Journal
    {
        private readonly Colour[] _menuColours = new Colour[128];
        private readonly int[] _menuIndices = new int[128];
        private readonly string[] _menuItem = new string[128];
        private readonly Player _player;
        private int _menuLength;

        public Journal(Player player)
        {
            _player = player;
        }

        public void ShowMenu()
        {
            Gui.FullScreenOverlay = true;
            Gui.Save();
            Gui.SetBackground(Terminal.BackgroundImage.Paper);
            while (true)
            {
                Gui.Refresh();
                Gui.Clear();
                Gui.Print(Colour.Blue, "Journal", 0, 1);
                Gui.Print(Colour.Blue, "=======", 1, 1);
                Gui.Print(Colour.Blue, "(a) Abilities", 3, 0);
                Gui.Print(Colour.Blue, "(d) Deities", 4, 0);
                Gui.Print(Colour.Blue, "(k) Kill Count", 5, 0);
                Gui.Print(Colour.Blue, "(m) Mutations", 6, 0);
                Gui.Print(Colour.Blue, "(p) Pets", 7, 0);
                Gui.Print(Colour.Blue, "(q) Quests", 8, 0);
                Gui.Print(Colour.Blue, "(r) Word of Recall", 9, 0);
                Gui.Print(Colour.Blue, "(s) Monsters Seen", 10, 0);
                Gui.Print(Colour.Blue, "(u) Uniques", 11, 0);
                Gui.Print(Colour.Blue, "(w) Worthless Items", 12, 0);
                Gui.Print(Colour.Orange, "[Select a journal section, or Escape to finish.]", 43, 1);
                char k = Gui.Inkey();
                if (k == '\x1b')
                {
                    break;
                }
                switch (k)
                {
                    case 'a':
                    case 'A':
                        JournalAbilities();
                        break;

                    case 'd':
                    case 'D':
                        JournalDeities();
                        break;

                    case 'p':
                    case 'P':
                        JournalPets();
                        break;

                    case 'q':
                    case 'Q':
                        JournalQuests();
                        break;

                    case 's':
                    case 'S':
                        JournalMonsters();
                        break;

                    case 'u':
                    case 'U':
                        JournalUniques();
                        break;

                    case 'k':
                    case 'K':
                        JournalKills();
                        break;

                    case 'm':
                    case 'M':
                        JournalMutations();
                        break;

                    case 'r':
                    case 'R':
                        JournalRecall();
                        break;

                    case 'w':
                    case 'W':
                        JournalWorthlessItems();
                        break;
                }
            }
            Gui.SetBackground(Terminal.BackgroundImage.Overhead);
            Gui.Load();
            Gui.FullScreenOverlay = false;
        }

        private void DisplayFlags(int mode)
        {
            int n;
            int top = 20 * mode;
            FlagSet f1 = new FlagSet();
            FlagSet f2 = new FlagSet();
            FlagSet f3 = new FlagSet();
            FlagSet[][] b = new FlagSet[14][];
            for (int i = 0; i < 14; i++)
            {
                b[i] = new FlagSet[6];
                for (int j = 0; j < 6; j++)
                {
                    b[i][j] = new FlagSet();
                }
            }
            for (int i = InventorySlot.MeleeWeapon; i < InventorySlot.Total; i++)
            {
                n = i - InventorySlot.MeleeWeapon;
                Item oPtr = _player.Inventory[i];
                oPtr.ObjectFlagsKnown(f1, f2, f3);
                b[n][0].Set(f1.LowOrder);
                b[n][1].Set(f1.HighOrder);
                b[n][2].Set(f2.LowOrder);
                b[n][3].Set(f2.HighOrder);
                b[n][4].Set(f3.LowOrder);
                b[n][5].Set(f3.HighOrder);
            }
            n = 13;
            _player.GetAbilitiesAsItemFlags(f1, f2, f3);
            b[n][0].Set(f1.LowOrder);
            b[n][1].Set(f1.HighOrder);
            b[n][2].Set(f2.LowOrder);
            b[n][3].Set(f2.HighOrder);
            b[n][4].Set(f3.LowOrder);
            b[n][5].Set(f3.HighOrder);
            for (int x = 0; x < 3; x++)
            {
                CharacterViewer.DisplayPlayerEquippy(_player, top + 2, (x * 26) + 12);
                Gui.Print(Colour.Blue, "abcdefghijklm@", top + 3, (x * 26) + 12);
                for (int y = 0; y < 16; y++)
                {
                    string name = GlobalData.ObjectFlagNames[(48 * mode) + (16 * x) + y];
                    if (string.IsNullOrEmpty(name))
                    {
                        continue;
                    }
                    var baseColour = Colour.Blue;
                    for (n = 0; n < 14; n++)
                    {
                        if (b[n][(3 * mode) + x].IsSet(1u << y))
                        {
                            baseColour = Colour.Green;
                        }
                    }
                    Gui.Print(baseColour, name, top + y + 4, (x * 26) + 1);
                    Gui.Print(baseColour, ':', top + y + 4, (x * 26) + 11);
                    for (n = 0; n < 14; n++)
                    {
                        Colour a = Colour.Grey;
                        char c = '.';
                        if (b[n][(3 * mode) + x].IsSet(1u << y))
                        {
                            a = baseColour;
                            c = '+';
                        }
                        Gui.Print(a, c, top + y + 4, (x * 26) + 12 + n);
                    }
                }
            }
        }

        private void DisplayMonster(int rIdx, int num, int of)
        {
            for (int i = 0; GlobalData.IdentInfo[i] != null; i++)
            {
                if (GlobalData.IdentInfo[i][0] == Profile.Instance.MonsterRaces[rIdx].Character)
                {
                    string name = GlobalData.IdentInfo[i].Substring(2);
                    string buf = $"Monster Type: {name} ({num + 1} of {of})";
                    Gui.Print(Colour.Blue, buf, 3, 0);
                    break;
                }
            }
            Gui.Goto(5, 0);
            DisplayMonsterHeader(rIdx);
            Gui.Goto(6, 0);
            Profile.Instance.MonsterRaces[rIdx].Knowledge.DisplayBody(Colour.Brown);
        }

        private void DisplayMonsterHeader(int rIdx)
        {
            MonsterRace rPtr = Profile.Instance.MonsterRaces[rIdx];
            char c1 = rPtr.Character;
            Colour a1 = rPtr.Colour;
            if ((rPtr.Flags1 & MonsterFlag1.Unique) == 0)
            {
                Gui.Print(Colour.Brown, "The ", -1);
            }
            Gui.Print(Colour.Brown, rPtr.Name, -1);
            Gui.Print(Colour.Brown, " ('", -1);
            Gui.Print(a1, c1);
            Gui.Print(Colour.Brown, "')", -1);
        }

        private void JournalAbilities()
        {
            Gui.Clear();
            DisplayFlags(0);
            DisplayFlags(1);
            Gui.Print(Colour.Orange, "[Press any key to finish.]", 43, 1);
            Gui.Inkey();
        }

        private void JournalDeities()
        {
            Gui.Clear();
            Gui.Print(Colour.Blue, "Standings with Deities", 0, 1);
            Gui.Print(Colour.Blue, "======================", 1, 1);
            int row = 3;
            God patron = null;
            foreach (var deity in _player.Religion.GetAllDeities())
            {
                var text = deity.ShortName;
                if (deity.IsPatron)
                {
                    patron = deity;
                }
                var adjusted = deity.AdjustedFavour;
                switch (adjusted)
                {
                    case 0:
                        if (deity.Favour < -1000)
                        {
                            text += " hates you";
                        }
                        else if (deity.Favour < -100)
                        {
                            text += " dislikes you";
                        }
                        else if (deity.Favour < -20)
                        {
                            text += " is annoyed by you";
                        }
                        else
                        {
                            text += " is indifferent to you";
                        }
                        break;

                    case 1:
                        text += " has noticed you";
                        break;

                    case 2:
                        text += " is watching you";
                        break;

                    case 3:
                        text += " approves of you";
                        break;

                    case 4:
                        text += " likes you";
                        break;

                    case 5:
                        text += " holds you dear";
                        break;

                    case 6:
                        text += " loves you";
                        break;

                    case 7:
                        text += " adores you";
                        break;

                    case 8:
                        text += " prizes you";
                        break;

                    case 9:
                        text += " dotes on you";
                        break;

                    case 10:
                        text += " cherishes you";
                        break;
                }
                if (adjusted > 0)
                {
                    switch (deity.Name)
                    {
                        case GodName.Lobon:
                            text += $" ({adjusted * 10}% chance to avoid ability drain)";
                            break;

                        case GodName.Nath_Horthah:
                            text += $" (+{adjusted * 10}% max health)";
                            break;

                        case GodName.Hagarg_Ryonis:
                            text += $" ({adjusted * 10}% chance to avoid poison/life drain)";
                            break;

                        case GodName.Tamash:
                            text += $" (+{adjusted * 10}% max mana)";
                            break;

                        case GodName.Zo_Kalar:
                            text += $" ({adjusted * 10}% chance to avoid death)";
                            break;
                    }
                }
                text += ".";
                Gui.Print(Colour.Blue, text, row, 1);
                row++;
            }
            if (patron != null)
            {
                Gui.Print(Colour.Blue, $"You are a follower of {patron.LongName}.", 12, 1);
                Gui.Print(Colour.Blue, $"Over time, your standing with {patron.ShortName} will revert to approval.", 13, 1);
                Gui.Print(Colour.Blue, $"Your standing with other deities will revert to annoyance.", 14, 1);
            }
            else
            {
                Gui.Print(Colour.Blue, "Over time, your standing with all deities will revert back to indifference.", 12, 1);
            }
            Gui.Print(Colour.Orange, "[Press any key to finish.]", 43, 1);
            Gui.Inkey();
        }

        private void JournalKills()
        {
            string[] names = new string[Profile.Instance.MonsterRaces.Count];
            int[] counts = new int[Profile.Instance.MonsterRaces.Count];
            bool[] unique = new bool[Profile.Instance.MonsterRaces.Count];
            int maxCount = 0;
            int total = 0;
            for (int i = 0; i < Profile.Instance.MonsterRaces.Count - 1; i++)
            {
                MonsterRace monster = Profile.Instance.MonsterRaces[i];
                if ((monster.Flags1 & MonsterFlag1.Unique) != 0)
                {
                    bool dead = monster.MaxNum == 0;
                    if (dead)
                    {
                        total++;
                        names[maxCount] = monster.Name;
                        counts[maxCount] = 1;
                        unique[maxCount] = true;
                        maxCount++;
                    }
                }
                else
                {
                    if (monster.Knowledge.RPkills > 0)
                    {
                        total += monster.Knowledge.RPkills;
                        names[maxCount] = monster.Name;
                        counts[maxCount] = monster.Knowledge.RPkills;
                        unique[maxCount] = false;
                        maxCount++;
                    }
                }
            }
            for (int i = 0; i < maxCount - 1; i++)
            {
                for (int j = maxCount - 1; j > i; j--)
                {
                    if (counts[j] <= counts[j - 1])
                    {
                        continue;
                    }
                    string tempName = names[j];
                    names[j] = names[j - 1];
                    names[j - 1] = tempName;
                    int tempCount = counts[j];
                    counts[j] = counts[j - 1];
                    counts[j - 1] = tempCount;
                    bool tempUnique = unique[j];
                    unique[j] = unique[j - 1];
                    unique[j - 1] = tempUnique;
                }
            }
            int first = 0;
            while (true)
            {
                string buf;
                Gui.Clear();
                Gui.Print(Colour.Blue, "Kill Count", 0, 1);
                Gui.Print(Colour.Blue, "==========", 1, 1);
                if (maxCount == 0)
                {
                    Gui.Print(Colour.Blue, "You haven't killed anything yet!", 3, 0);
                }
                for (int i = first; i < first + 38; i++)
                {
                    if (i < maxCount)
                    {
                        if (unique[i])
                        {
                            buf = $"You have killed {names[i]}";
                        }
                        else
                        {
                            if (counts[i] > 1)
                            {
                                string plural = names[i].PluraliseMonsterName();
                                buf = $"You have killed {counts[i]} {plural}";
                            }
                            else
                            {
                                buf = $"You have killed {counts[i]} {names[i]}";
                            }
                        }
                        Gui.Print(Colour.Blue, buf, i - first + 3, 0);
                    }
                }
                buf = $"Total Kills: {total}";
                Gui.Print(Colour.Blue, buf, 41, 0);
                Gui.Print(Colour.Orange, "[Use up and down to navigate list, and Escape to finish.]", 43, 1);
                int c = Gui.Inkey();
                if (c == '\x1b')
                {
                    break;
                }
                if (c == '8' || c == '4')
                {
                    first--;
                    if (first < 0)
                    {
                        first = 0;
                    }
                }
                if (c == '2' || c == '6')
                {
                    first++;
                    if (first > maxCount - 38)
                    {
                        first = maxCount - 38;
                    }
                    if (first < 0)
                    {
                        first = 0;
                    }
                }
            }
        }

        private void JournalMonsters()
        {
            int[] seen = new int[Profile.Instance.MonsterRaces.Count];
            int[] filtered = new int[Profile.Instance.MonsterRaces.Count];
            int maxSeen = 0;
            bool[] filterMask = new bool[256];
            int[] filterLookup = new int[256];
            char[] usedFilters = new char[256];
            int maxUsedFilter = 0;
            for (int i = 0; i < 256; i++)
            {
                filterMask[i] = false;
            }
            for (int i = 1; i < Profile.Instance.MonsterRaces.Count; i++)
            {
                if (Profile.Instance.MonsterRaces[i].Knowledge.RSights != 0 || _player.IsWizard)
                {
                    seen[maxSeen] = i;
                    maxSeen++;
                    char symbol = Profile.Instance.MonsterRaces[i].Character;
                    if (!filterMask[symbol])
                    {
                        filterMask[symbol] = true;
                    }
                }
            }
            for (char i = (char)0; i < 256; i++)
            {
                usedFilters[i] = (char)0;
                if (!filterMask[i])
                {
                    continue;
                }
                usedFilters[maxUsedFilter] = i;
                filterLookup[i] = maxUsedFilter;
                maxUsedFilter++;
            }
            if (maxSeen == 0)
            {
                Gui.Clear();
                Gui.Print(Colour.Blue, "Monsters Seen", 0, 1);
                Gui.Print(Colour.Blue, "=============", 1, 1);
                Gui.Print(Colour.Blue, "You haven't seen any monsters yet!", 3, 0);
                Gui.Print(Colour.Orange, "[Press any key to finish]", 43, 1);
                Gui.Inkey();
                return;
            }
            int currentFilterIndex = 0;
            char currentFilter = usedFilters[0];
            bool useMax = false;
            while (true)
            {
                int maxFiltered = 0;
                for (int i = 0; i < maxSeen; i++)
                {
                    if (Profile.Instance.MonsterRaces[seen[i]].Character == currentFilter)
                    {
                        filtered[maxFiltered] = seen[i];
                        maxFiltered++;
                    }
                }
                int currentIndex = 0;
                if (useMax)
                {
                    currentIndex = maxFiltered - 1;
                }
                char c;
                while (true)
                {
                    Gui.Clear();
                    Gui.Print(Colour.Blue, "Monsters Seen", 0, 1);
                    Gui.Print(Colour.Blue, "=============", 1, 1);
                    DisplayMonster(filtered[currentIndex], currentIndex, maxFiltered);
                    Gui.Print(Colour.Orange,
                        "[Up and down to change type, left and right to change monster, Esc to finish]", 43, 1);
                    c = Gui.Inkey();
                    if (c == '4')
                    {
                        if (currentIndex > 0)
                        {
                            currentIndex--;
                        }
                        else
                        {
                            c = '8';
                            break;
                        }
                    }
                    else if (c == '6')
                    {
                        if (currentIndex < maxFiltered - 1)
                        {
                            currentIndex++;
                        }
                        else
                        {
                            c = '2';
                            break;
                        }
                    }
                    else if (c == '\x1b' || c == '2' || c == '8')
                    {
                        break;
                    }
                    else
                    {
                        if (filterMask[c])
                        {
                            currentFilterIndex = filterLookup[c];
                            currentFilter = usedFilters[currentFilterIndex];
                            break;
                        }
                    }
                }
                if (c == '8')
                {
                    if (currentFilterIndex > 0)
                    {
                        currentFilterIndex--;
                        currentFilter = usedFilters[currentFilterIndex];
                        useMax = true;
                    }
                    else
                    {
                        useMax = false;
                    }
                }
                if (c == '2')
                {
                    if (currentFilterIndex < maxUsedFilter - 1)
                    {
                        currentFilterIndex++;
                        currentFilter = usedFilters[currentFilterIndex];
                        useMax = false;
                    }
                    else
                    {
                        useMax = true;
                    }
                }
                if (c == '\x1b')
                {
                    break;
                }
            }
        }

        private void JournalMutations()
        {
            string[] features = _player.Dna.GetMutationList();
            int maxFeature = features.Length;
            int first = 0;
            while (true)
            {
                Gui.Clear();
                Gui.Print(Colour.Blue, "Mutations", 0, 1);
                Gui.Print(Colour.Blue, "=========", 1, 1);
                if (maxFeature == 0)
                {
                    Gui.Print(Colour.Blue, "You have no mutations.", 3, 0);
                }
                else
                {
                    for (int i = first; i < first + 38; i++)
                    {
                        if (i < maxFeature)
                        {
                            Gui.Print(Colour.Blue, features[i], i - first + 3, 0);
                        }
                    }
                }
                Gui.Print(Colour.Orange, "[Use up and down to navigate list, and Escape to finish.]", 43, 1);
                int c = Gui.Inkey();
                if (c == '\x1b')
                {
                    break;
                }
                if (c == '8' || c == '4')
                {
                    first--;
                    if (first < 0)
                    {
                        first = 0;
                    }
                }
                if (c == '2' || c == '6')
                {
                    first++;
                    if (first > maxFeature - 38)
                    {
                        first = maxFeature - 38;
                    }
                    if (first < 0)
                    {
                        first = 0;
                    }
                }
            }
        }

        private void JournalPets()
        {
            List<string> petNames = new List<string>();
            int pets = 0;
            Level level = SaveGame.Instance.Level;
            for (int petCtr = level.MMax - 1; petCtr >= 1; petCtr--)
            {
                Monster mPtr = level.Monsters[petCtr];
                if ((mPtr.Mind & Constants.SmFriendly) == 0)
                {
                    continue;
                }
                petNames.Add(mPtr.Race.Name);
                pets++;
            }
            int first = 0;
            while (true)
            {
                Gui.Clear();
                Gui.Print(Colour.Blue, "Pets", 0, 1);
                Gui.Print(Colour.Blue, "====", 1, 1);
                if (pets == 0)
                {
                    Gui.Print(Colour.Blue, "You have no pets.", 3, 0);
                }
                else
                {
                    for (int i = first; i < first + 38; i++)
                    {
                        if (i < pets)
                        {
                            Gui.Print(Colour.Blue, petNames[i], i - first + 3, 0);
                        }
                    }
                }
                Gui.Print(Colour.Orange, "[Use up and down to navigate list, and Escape to finish.]", 43, 1);
                int c = Gui.Inkey();
                if (c == '\x1b')
                {
                    break;
                }
                if (c == '8' || c == '4')
                {
                    first--;
                    if (first < 0)
                    {
                        first = 0;
                    }
                }
                if (c == '2' || c == '6')
                {
                    first++;
                    if (first > pets - 38)
                    {
                        first = pets - 38;
                    }
                    if (first < 0)
                    {
                        first = 0;
                    }
                }
            }
        }

        private void JournalQuests()
        {
            Gui.Clear();
            Gui.Print(Colour.Blue, "Outstanding Quests", 0, 1);
            Gui.Print(Colour.Blue, "==================", 1, 1);
            int[] lev = new int[Constants.MaxCaves];
            int[] first = new int[Constants.MaxCaves];
            for (int i = 0; i < Constants.MaxCaves; i++)
            {
                first[i] = -1;
                lev[i] = -1;
            }
            for (int i = 0; i < SaveGame.Instance.Quests.Count; i++)
            {
                Quest q = SaveGame.Instance.Quests[i];
                if (q.Level > 0)
                {
                    int dungeon = q.Dungeon;
                    if (first[dungeon] == -1 || q.Level < lev[dungeon])
                    {
                        first[dungeon] = i;
                        lev[dungeon] = q.Level;
                    }
                }
            }
            int row = 3;
            for (int i = 0; i < Constants.MaxCaves; i++)
            {
                if (first[i] != -1)
                {
                    string line = SaveGame.Instance.Quests.DescribeQuest(first[i]);
                    Gui.Print(Colour.Blue, line, row, 0);
                    row++;
                }
            }
            if (row == 3)
            {
                Gui.Print(Colour.Blue, "Congratulations! You have completed all the quests.", row, 0);
            }
            Gui.Print(Colour.Orange, "[Press any key to finish.]", 43, 1);
            Gui.Inkey();
        }

        private void JournalRecall()
        {
            Gui.Clear();
            Gui.Print(Colour.Blue, "Word of Recall", 0, 1);
            Gui.Print(Colour.Blue, "==============", 1, 1);
            string recallTown = _player.TownWithHouse > -1
                ? SaveGame.Instance.Towns[_player.TownWithHouse].Name
                : SaveGame.Instance.CurTown.Name;
            string recallDungeon = SaveGame.Instance.Dungeons[SaveGame.Instance.RecallDungeon].Name;
            int recallLev = _player.MaxDlv[SaveGame.Instance.RecallDungeon];
            Gui.Print(Colour.Blue, $"Your Word of Recall position is level {recallLev} of {recallDungeon}.", 3, 0);
            Gui.Print(Colour.Blue, $"Your home town is {recallTown}.", 4, 0);
            if (_player.TownWithHouse > -1)
            {
                recallTown = "your house in " + SaveGame.Instance.Dungeons[_player.TownWithHouse].Shortname;
            }
            Gui.Print(Colour.Brown,
                SaveGame.Instance.DunLevel == 0
                    ? $"If you recall now, you will return to level {recallLev} of {recallDungeon}."
                    : $"If you recall now, you will return to {recallTown}.", 6, 0);
            string description =
                "(If you own a house, your home town is always considered to be the town containing that house ";
            description += "and you will be transported directly to your house. ";
            description += "If not, your home town is updated each time you visit a new town, and you will be transported to a random location in that town. ";
            description += "Your Word of Recall position has its dungeon location updated ";
            description += "only when you recall from a new dungeon or tower; but has its level updated ";
            description += "each time you reach a new level within that dungeon or tower. In either case, you will be transported to a random location on the dungeon or tower level.)";
            Gui.Goto(8, 0);
            Gui.PrintWrap(Colour.Blue, description);
            Gui.Print(Colour.Orange, "[Press any key to finish.]", 43, 1);
            Gui.Inkey();
        }

        private void JournalUniques()
        {
            string[] names = new string[Profile.Instance.MonsterRaces.Count];
            bool[] alive = new bool[Profile.Instance.MonsterRaces.Count];
            int maxCount = 0;
            for (int i = 0; i < Profile.Instance.MonsterRaces.Count - 1; i++)
            {
                MonsterRace monster = Profile.Instance.MonsterRaces[i];
                if ((monster.Flags1 & MonsterFlag1.Unique) != 0 &&
                    (monster.Knowledge.RSights > 0 || _player.IsWizard))
                {
                    names[maxCount] = monster.Name;
                    bool dead = monster.MaxNum == 0;
                    alive[maxCount] = !dead;
                    maxCount++;
                }
            }
            int first = 0;
            while (true)
            {
                Gui.Clear();
                Gui.Print(Colour.Blue, "Unique Foes", 0, 1);
                Gui.Print(Colour.Blue, "===========", 1, 1);
                if (maxCount == 0)
                {
                    Gui.Print(Colour.Blue, "You know of no unique foes!", 3, 0);
                }
                for (int i = first; i < first + 38; i++)
                {
                    if (i < maxCount)
                    {
                        string buf = alive[i] ? $"{names[i]} is alive." : $"{names[i]} is dead.";
                        Gui.Print(Colour.Blue, buf, i - first + 3, 0);
                    }
                }
                Gui.Print(Colour.Orange, "[Use up and down to navigate list, and Escape to finish.]", 43, 1);
                int c = Gui.Inkey();
                if (c == '\x1b')
                {
                    break;
                }
                if (c == '8' || c == '4')
                {
                    first--;
                    if (first < 0)
                    {
                        first = 0;
                    }
                }
                if (c == '2' || c == '6')
                {
                    first++;
                    if (first > maxCount - 38)
                    {
                        first = maxCount - 38;
                    }
                    if (first < 0)
                    {
                        first = 0;
                    }
                }
            }
        }

        private void JournalWorthlessItems()
        {
            Gui.Clear();
            Gui.Print(Colour.Blue, "Worthless Items", 0, 1);
            Gui.Print(Colour.Blue, "===============", 1, 1);
            Gui.Goto(3, 0);
            string text = "Items marked in red ";
            text += "will be considered 'worthless' and you will stomp on them (destroying them) rather than ";
            text += "picking them up. Destroying (using 'k' or 'K') a worthless object will be done automatically ";
            text += "without you being prompted. Items will only be destroyed if they are on the floor or in your ";
            text += "inventory. Items you are wielding will never be destroyed (giving you chance to improve their ";
            text += "quality to a non-worthless level).";
            Gui.PrintWrap(Colour.Blue, text);
            for (int i = 0; i < TvalDescriptionPair.Tvals.Length - 1; i++)
            {
                _menuItem[i] = TvalDescriptionPair.Tvals[i].Desc;
                _menuColours[i] = Colour.Blue;
            }
            _menuLength = TvalDescriptionPair.Tvals.Length - 1;
            int menu = _menuLength / 2;
            while (true)
            {
                MenuDisplay(menu);
                Gui.Print(Colour.Orange, "[Up/Down = select item type, Left/Right = forward/back.]", 43, 1);
                while (true)
                {
                    char c = Gui.Inkey();
                    if (c == '8' && menu > 0)
                    {
                        menu--;
                        break;
                    }
                    if (c == '2' && menu < _menuLength - 1)
                    {
                        menu++;
                        break;
                    }
                    if (c == '6')
                    {
                        WorthlessItemTypeSelection(TvalDescriptionPair.Tvals[menu].Tval);
                        for (int i = 0; i < TvalDescriptionPair.Tvals.Length - 1; i++)
                        {
                            _menuItem[i] = TvalDescriptionPair.Tvals[i].Desc;
                            _menuColours[i] = Colour.Blue;
                        }
                        _menuLength = TvalDescriptionPair.Tvals.Length - 1;
                        break;
                    }
                    if (c == '4')
                    {
                        return;
                    }
                }
            }
        }

        private void MenuDisplay(int current)
        {
            Gui.Clear(9);
            Gui.Print(Colour.Orange, "=>", 25, 0);
            string desc = string.Empty;
            Colour descColour = Colour.Brown;
            for (int i = 0; i < _menuLength; i++)
            {
                int row = 25 + i - current;
                if (row < 10 || row > 40)
                {
                    continue;
                }
                Colour a = _menuColours[i];
                if (i == current)
                {
                    switch (a)
                    {
                        case Colour.Blue:
                            a = Colour.BrightBlue;
                            desc = "(This type of item has further sub-types.)";
                            break;

                        case Colour.Green:
                            a = Colour.BrightGreen;
                            desc = "(This type of item has value to you.)";
                            break;

                        default:
                            a = Colour.BrightRed;
                            desc = "(This type of item is worthless to you.)";
                            break;
                    }
                    descColour = a;
                }
                Gui.Print(a, _menuItem[i], row, 2);
            }
            Gui.Print(descColour, desc, 25, 33);
        }

        private string StripDownName(string name)
        {
            string val = name.Replace("~", "");
            val = val.Replace("%", "");
            val = val.Replace("&", "");
            return val.Trim();
        }

        private void WorthlessItemChestSelection(ItemType kPtr)
        {
            string[] qualityText = new[] { "Empty", "Unlocked", "Locked", "Trapped" };
            _menuLength = 0;
            for (int i = 0; i < 4; i++)
            {
                _menuItem[i] = qualityText[i];
                _menuColours[i] = kPtr.Stompable[i] ? Colour.Red : Colour.Green;
            }
            _menuLength = 4;
            int menu = 1;
            while (true)
            {
                MenuDisplay(menu);
                Gui.Print(Colour.Orange, "[Up/Down = select item type, Left/Right = forward/back.]", 43, 1);
                while (true)
                {
                    char c = Gui.Inkey();
                    if (c == '8' && menu > 0)
                    {
                        menu--;
                        break;
                    }
                    if (c == '2' && menu < _menuLength - 1)
                    {
                        menu++;
                        break;
                    }
                    if (c == '6')
                    {
                        kPtr.Stompable[menu] = !kPtr.Stompable[menu];
                        _menuColours[menu] = kPtr.Stompable[menu] ? Colour.Red : Colour.Green;
                        break;
                    }
                    if (c == '4')
                    {
                        return;
                    }
                }
            }
        }

        private void WorthlessItemQualitySelection(ItemType kPtr)
        {
            string[] qualityText = new[] { "Bad", "Average", "Good", "Excellent" };
            _menuLength = 0;
            for (int i = 0; i < 4; i++)
            {
                _menuItem[i] = qualityText[i];
                _menuColours[i] = kPtr.Stompable[i] ? Colour.Red : Colour.Green;
            }
            _menuLength = 4;
            int menu = 1;
            while (true)
            {
                MenuDisplay(menu);
                Gui.Print(Colour.Orange, "[Up/Down = select item type, Left/Right = forward/back.]", 43, 1);
                while (true)
                {
                    char c = Gui.Inkey();
                    if (c == '8' && menu > 0)
                    {
                        menu--;
                        break;
                    }
                    if (c == '2' && menu < _menuLength - 1)
                    {
                        menu++;
                        break;
                    }
                    if (c == '6')
                    {
                        kPtr.Stompable[menu] = !kPtr.Stompable[menu];
                        _menuColours[menu] = kPtr.Stompable[menu] ? Colour.Red : Colour.Green;
                        break;
                    }
                    if (c == '4')
                    {
                        return;
                    }
                }
            }
        }

        private void WorthlessItemTypeSelection(ItemCategory tval)
        {
            _menuLength = 0;
            for (int i = 1; i < Profile.Instance.ItemTypes.Count; i++)
            {
                ItemType kPtr = Profile.Instance.ItemTypes[i];
                if (kPtr.Category == tval)
                {
                    if (kPtr.Flags3.IsSet(ItemFlag3.InstaArt))
                    {
                        continue;
                    }
                    _menuItem[_menuLength] = StripDownName(kPtr.Name);
                    if (kPtr.HasQuality() || kPtr.Category == ItemCategory.Chest)
                    {
                        _menuColours[_menuLength] = Colour.Blue;
                    }
                    else
                    {
                        _menuColours[_menuLength] = kPtr.Stompable[0] ? Colour.Red : Colour.Green;
                    }
                    _menuIndices[_menuLength] = i;
                    _menuLength++;
                }
            }
            int menu = _menuLength / 2;
            while (true)
            {
                MenuDisplay(menu);
                Gui.Print(Colour.Orange, "[Up/Down = select item type, Left/Right = forward/back.]", 43, 1);
                while (true)
                {
                    char c = Gui.Inkey();
                    if (c == '8' && menu > 0)
                    {
                        menu--;
                        break;
                    }
                    if (c == '2' && menu < _menuLength - 1)
                    {
                        menu++;
                        break;
                    }
                    if (c == '6')
                    {
                        ItemType kPtr = Profile.Instance.ItemTypes[_menuIndices[menu]];
                        if (kPtr.HasQuality())
                        {
                            WorthlessItemQualitySelection(kPtr);
                            _menuLength = 0;
                            for (int i = 1; i < Profile.Instance.ItemTypes.Count; i++)
                            {
                                kPtr = Profile.Instance.ItemTypes[i];
                                if (kPtr.Category == tval)
                                {
                                    if (kPtr.Flags3.IsSet(ItemFlag3.InstaArt))
                                    {
                                        continue;
                                    }
                                    _menuItem[_menuLength] = StripDownName(kPtr.Name);
                                    if (kPtr.HasQuality())
                                    {
                                        _menuColours[_menuLength] = Colour.Blue;
                                    }
                                    else
                                    {
                                        _menuColours[_menuLength] = kPtr.Stompable[0] ? Colour.Red : Colour.Green;
                                    }
                                    _menuIndices[_menuLength] = i;
                                    _menuLength++;
                                }
                            }
                        }
                        else if (kPtr.Category == ItemCategory.Chest)
                        {
                            WorthlessItemChestSelection(kPtr);
                            _menuLength = 0;
                            for (int i = 1; i < Profile.Instance.ItemTypes.Count; i++)
                            {
                                kPtr = Profile.Instance.ItemTypes[i];
                                if (kPtr.Category == tval)
                                {
                                    if (kPtr.Flags3.IsSet(ItemFlag3.InstaArt))
                                    {
                                        continue;
                                    }
                                    _menuItem[_menuLength] = StripDownName(kPtr.Name);
                                    if (kPtr.Category == ItemCategory.Chest)
                                    {
                                        _menuColours[_menuLength] = Colour.Blue;
                                    }
                                    else
                                    {
                                        _menuColours[_menuLength] = kPtr.Stompable[0] ? Colour.Red : Colour.Green;
                                    }
                                    _menuIndices[_menuLength] = i;
                                    _menuLength++;
                                }
                            }
                        }
                        else
                        {
                            kPtr.Stompable[0] = !kPtr.Stompable[0];
                            _menuColours[menu] = kPtr.Stompable[0] ? Colour.Red : Colour.Green;
                        }
                        break;
                    }
                    if (c == '4')
                    {
                        return;
                    }
                }
            }
        }
    }
}