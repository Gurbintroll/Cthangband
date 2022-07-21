// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Commands;
using Cthangband.Debug;
using Cthangband.Enumerations;
using Cthangband.Patrons;
using Cthangband.Spells;
using Cthangband.StaticData;
using Cthangband.UI;
using System;
using System.Collections.Generic;

namespace Cthangband
{
    [Serializable]
    internal class SaveGame
    {
        public readonly CombatEngine CombatEngine;
        public readonly CommandHandler Command = new CommandHandler();
        public readonly CommandEngine CommandEngine;
        public readonly Dungeon[] Dungeons;
        public readonly Patron[] PatronList;
        public readonly QuestArray Quests = new QuestArray();
        public readonly SpellEffectsHandler SpellEffects;
        public readonly Town[] Towns;
        public readonly Island Wilderness = new Island();
        public int AllocKindSize;
        public AllocationEntry[] AllocKindTable;
        public int AllocRaceSize;
        public AllocationEntry[] AllocRaceTable;
        public List<AmuletFlavour> AmuletFlavours;
        public LevelStart CameFrom;
        public bool CharacterXtra;
        public bool CreateDownStair;
        public bool CreateUpStair;
        public Dungeon CurDungeon;
        public int CurrentDepth;
        public Town CurTown;
        public string DiedFrom;
        public int DungeonDifficulty;
        public int EnergyUse;
        public bool HackMind;
        public bool IsAutosave;
        public int ItemDisplayColumn = 50;
        public ItemFilterDelegate ItemFilter;
        public bool ItemFilterAll;
        public Level Level;
        public bool MartialArtistArmourAux;
        public bool MartialArtistNotifyAux;
        public List<MushroomFlavour> MushroomFlavours;
        public bool NewLevelFlag;
        public Player Player;
        public bool Playing;
        public List<PotionFlavour> PotionFlavours;
        public int RecallDungeon;
        public int Resting;
        public List<RingFlavour> RingFlavours;
        public List<RodFlavour> RodFlavours;
        public int Running;
        public List<ScrollFlavour> ScrollFlavours;
        public List<StaffFlavour> StaffFlavours;
        public int TargetCol;
        public int TargetRow;
        public int TargetWho;
        public int TotalFriendLevels;
        public int TotalFriends;
        public int TrackedMonsterIndex;
        public bool ViewingEquipment;
        public bool ViewingItemList;
        public List<WandFlavour> WandFlavours;

        private List<Monster> _petList = new List<Monster>();
        private int _seedFlavor;

        public SaveGame()
        {
            CombatEngine = new CombatEngine(this);
            CommandEngine = new CommandEngine(this);
            SpellEffects = new SpellEffectsHandler(this);
            Towns = Town.NewTownList();
            Dungeons = Dungeon.NewDungeonList();
            PatronList = Patron.NewPatronList();
        }

        internal delegate bool ItemFilterDelegate(Item item);

        public static SaveGame Instance
        {
            get; set;
        }

        public int Difficulty => CurrentDepth + DungeonDifficulty;

        public void ActivateDreadCurse()
        {
            int i = 0;
            do
            {
                switch (Program.Rng.DieRoll(27))
                {
                    case 1:
                    case 2:
                    case 3:
                    case 16:
                    case 17:
                        SpellEffects.AggravateMonsters(1);
                        break;

                    case 4:
                    case 5:
                    case 6:
                        SpellEffects.ActivateHiSummon();
                        break;

                    case 7:
                    case 8:
                    case 9:
                    case 18:
                        Level.Monsters.SummonSpecific(Player.MapY, Player.MapX, Difficulty, 0);
                        break;

                    case 10:
                    case 11:
                    case 12:
                        Profile.Instance.MsgPrint("You feel your life draining away...");
                        Player.LoseExperience(Player.ExperiencePoints / 16);
                        break;

                    case 13:
                    case 14:
                    case 15:
                    case 19:
                    case 20:
                        if (!Player.HasFreeAction || Program.Rng.DieRoll(100) >= Player.SkillSavingThrow)
                        {
                            Profile.Instance.MsgPrint("You feel like a statue!");
                            if (Player.HasFreeAction)
                            {
                                Player.SetTimedParalysis(Player.TimedParalysis + Program.Rng.DieRoll(3));
                            }
                            else
                            {
                                Player.SetTimedParalysis(Player.TimedParalysis + Program.Rng.DieRoll(13));
                            }
                        }
                        break;

                    case 21:
                    case 22:
                    case 23:
                        Player.TryDecreasingAbilityScore(Program.Rng.DieRoll(6) - 1);
                        break;

                    case 24:
                        Profile.Instance.MsgPrint("Huh? Who am I? What am I doing here?");
                        SpellEffects.LoseAllInfo();
                        break;

                    case 25:
                        SpellEffects.SummonReaver();
                        break;

                    default:
                        while (i < 6)
                        {
                            do
                            {
                                Player.TryDecreasingAbilityScore(i);
                            } while (Program.Rng.DieRoll(2) == 1);
                            i++;
                        }
                        break;
                }
            } while (Program.Rng.DieRoll(3) == 1);
        }

        public void ChestTrap(int y, int x, int oIdx)
        {
            Item oPtr = Level.Items[oIdx];
            if (oPtr.TypeSpecificValue <= 0)
            {
                return;
            }
            int trap = GlobalData.ChestTraps[oPtr.TypeSpecificValue];
            if ((trap & Enumerations.ChestTrap.ChestLoseStr) != 0)
            {
                Profile.Instance.MsgPrint("A small needle has pricked you!");
                Player.TakeHit(Program.Rng.DiceRoll(1, 4), "a poison needle");
                Player.TryDecreasingAbilityScore(Ability.Strength);
            }
            if ((trap & Enumerations.ChestTrap.ChestLoseCon) != 0)
            {
                Profile.Instance.MsgPrint("A small needle has pricked you!");
                Player.TakeHit(Program.Rng.DiceRoll(1, 4), "a poison needle");
                Player.TryDecreasingAbilityScore(Ability.Constitution);
            }
            if ((trap & Enumerations.ChestTrap.ChestPoison) != 0)
            {
                Profile.Instance.MsgPrint("A puff of green gas surrounds you!");
                if (!(Player.HasPoisonResistance || Player.TimedPoisonResistance != 0))
                {
                    if (Program.Rng.DieRoll(10) <= Player.Religion.GetNamedDeity(Pantheon.GodName.Hagarg_Ryonis).AdjustedFavour)
                    {
                        Profile.Instance.MsgPrint("Hagarg Ryonis's favour protects you!");
                    }
                    else
                    {
                        Player.SetTimedPoison(Player.TimedPoison + 10 + Program.Rng.DieRoll(20));
                    }
                }
            }
            if ((trap & Enumerations.ChestTrap.ChestParalyze) != 0)
            {
                Profile.Instance.MsgPrint("A puff of yellow gas surrounds you!");
                if (!Player.HasFreeAction)
                {
                    Player.SetTimedParalysis(Player.TimedParalysis + 10 + Program.Rng.DieRoll(20));
                }
            }
            if ((trap & Enumerations.ChestTrap.ChestSummon) != 0)
            {
                int num = 2 + Program.Rng.DieRoll(3);
                Profile.Instance.MsgPrint("You are enveloped in a cloud of smoke!");
                for (int i = 0; i < num; i++)
                {
                    if (Program.Rng.DieRoll(100) < Difficulty)
                    {
                        SpellEffects.ActivateHiSummon();
                    }
                    else
                    {
                        Level.Monsters.SummonSpecific(y, x, Difficulty, 0);
                    }
                }
            }
            if ((trap & Enumerations.ChestTrap.ChestExplode) != 0)
            {
                Profile.Instance.MsgPrint("There is a sudden explosion!");
                Profile.Instance.MsgPrint("Everything inside the chest is destroyed!");
                oPtr.TypeSpecificValue = 0;
                Player.TakeHit(Program.Rng.DiceRoll(5, 8), "an exploding chest");
            }
        }

        public void DisplayWildMap()
        {
            int[] dungeonGuardians = new int[Constants.MaxCaves];
            int y, i;
            for (i = 0; i < Constants.MaxCaves; i++)
            {
                dungeonGuardians[i] = 0;
            }
            for (i = 0; i < Quests.Count; i++)
            {
                if (Quests[i].IsActive)
                {
                    dungeonGuardians[Quests[i].Dungeon]++;
                }
            }
            for (y = 0; y < 12; y++)
            {
                for (int x = 0; x < 12; x++)
                {
                    string wildMapSymbol = "^";
                    Colour wildMapAttr = Colour.Green;
                    if (Wilderness[y][x].Dungeon != null)
                    {
                        Dungeon dungeon = Wilderness[y][x].Dungeon;
                        wildMapSymbol = dungeon.Visited ? dungeon.MapSymbol : "?";
                        wildMapAttr = Wilderness[y][x].Town != null ? Colour.Grey : Colour.Brown;
                        if (dungeonGuardians[Wilderness[y][x].Dungeon.Index] != 0)
                        {
                            wildMapAttr = Colour.BrightRed;
                        }
                    }
                    if (x == 0 || y == 0 || x == 11 || y == 11)
                    {
                        wildMapSymbol = "~";
                        wildMapAttr = Colour.Blue;
                    }
                    Gui.Print(wildMapAttr, wildMapSymbol, y + 2, x + 2);
                }
            }
            Gui.Print(Colour.Purple, "+------------+", 1, 1);
            for (y = 0; y < 12; y++)
            {
                Gui.Print(Colour.Purple, "|", y + 2, 1);
                Gui.Print(Colour.Purple, "|", y + 2, 14);
            }
            Gui.Print(Colour.Purple, "+------------+", 14, 1);
            for (y = 0; y < Constants.MaxCaves; y++)
            {
                string depth = Dungeons[y].KnownDepth ? $"{Dungeons[y].MaxLevel}" : "?";
                string difficulty = Dungeons[y].KnownOffset ? $"{Dungeons[y].Offset}" : "?";
                string buffer;
                if (Dungeons[y].Visited)
                {
                    buffer = y < Instance.Towns.Length
                        ? $"{Dungeons[y].MapSymbol} = {Towns[y].Name} (L:{depth}, D:{difficulty}, Q:{dungeonGuardians[y]})"
                        : $"{Dungeons[y].MapSymbol} = {Dungeons[y].Name} (L:{depth}, D:{difficulty}, Q:{dungeonGuardians[y]})";
                }
                else
                {
                    buffer = $"? = {Dungeons[y].Name} (L:{depth}, D:{difficulty}, Q:{dungeonGuardians[y]})";
                }
                Colour keyAttr = Colour.Brown;
                if (y < Instance.Towns.Length)
                {
                    keyAttr = Colour.Grey;
                }
                if (dungeonGuardians[y] != 0)
                {
                    keyAttr = Colour.BrightRed;
                }
                Gui.Print(keyAttr, buffer, y + 1, 19);
            }
            Gui.Print(Colour.Purple, "L:levels", 16, 1);
            Gui.Print(Colour.Purple, "D:difficulty", 17, 1);
            Gui.Print(Colour.Purple, "Q:quests", 18, 1);
            Gui.Print(Colour.Purple, "(Your position is marked with the cursor)", Constants.MaxCaves + 2, 19);
        }

        public void Disturb(bool stopSearch)
        {
            if (Command.CommandRepeat != 0)
            {
                Command.CommandRepeat = 0;
                Player.RedrawNeeded.Set(RedrawFlag.PrState);
            }
            if (Resting != 0)
            {
                Resting = 0;
                Player.RedrawNeeded.Set(RedrawFlag.PrState);
            }
            if (Running != 0)
            {
                Running = 0;
                Player.UpdatesNeeded.Set(UpdateFlags.UpdateTorchRadius);
            }
            if (stopSearch && Player.IsSearching)
            {
                Player.IsSearching = false;
                Player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
                Player.RedrawNeeded.Set(RedrawFlag.PrState);
            }
        }

        public void DoCmdSaveGame()
        {
            if (!IsAutosave)
            {
                Disturb(true);
            }
            Profile.Instance.MsgPrint(null);
            HandleStuff();
            Gui.Refresh();
            DiedFrom = "(saved)";
            SavePlayer();
            Gui.Refresh();
            DiedFrom = "(alive and well)";
        }

        public bool GetItem(out int itemIndex, string prompt, bool canChooseFromEquipment, bool canChooseFromInventory, bool canChooseFromFloor)
        {
            GridTile tile = Level.Grid[Player.MapY][Player.MapX];
            Inventory inventory = Player.Inventory;
            int currentItemIndex;
            int nextItemIndex;
            bool allowFloor = false;
            Profile.Instance.MsgPrint(null);
            bool done = false;
            bool item = false;
            itemIndex = -1;
            int i1 = 0;
            int i2 = InventorySlot.Pack - 1;
            if (!canChooseFromInventory)
            {
                i2 = -1;
            }
            while (i1 <= i2 && !inventory.GetItemOkay(i1))
            {
                i1++;
            }
            while (i1 <= i2 && !inventory.GetItemOkay(i2))
            {
                i2--;
            }
            int e1 = InventorySlot.MeleeWeapon;
            int e2 = InventorySlot.Total - 1;
            if (!canChooseFromEquipment)
            {
                e2 = -1;
            }
            while (e1 <= e2 && !inventory.GetItemOkay(e1))
            {
                e1++;
            }
            while (e1 <= e2 && !inventory.GetItemOkay(e2))
            {
                e2--;
            }
            if (canChooseFromFloor)
            {
                for (currentItemIndex = tile.ItemIndex; currentItemIndex != 0; currentItemIndex = nextItemIndex)
                {
                    Item oPtr = Level.Items[currentItemIndex];
                    nextItemIndex = oPtr.NextInStack;
                    if (inventory.ItemMatchesFilter(oPtr))
                    {
                        allowFloor = true;
                    }
                }
            }
            if (!allowFloor && i1 > i2 && e1 > e2)
            {
                ViewingItemList = false;
                itemIndex = -2;
                done = true;
            }
            else
            {
                if (!ViewingItemList)
                {
                    ItemDisplayColumn = 50;
                }
                if (ViewingItemList && ViewingEquipment && canChooseFromEquipment)
                {
                    ViewingEquipment = true;
                }
                else if (canChooseFromInventory)
                {
                    ViewingEquipment = false;
                }
                else if (canChooseFromEquipment)
                {
                    ViewingEquipment = true;
                }
                else
                {
                    ViewingEquipment = false;
                }
            }
            if (ViewingItemList)
            {
                Gui.Save();
            }
            while (!done)
            {
                if (!ViewingEquipment)
                {
                    i1.IndexToLetter();
                    i2.IndexToLetter();
                    if (ViewingItemList)
                    {
                        Player.Inventory.ShowInven();
                    }
                }
                else
                {
                    (e1 - InventorySlot.MeleeWeapon).IndexToLetter();
                    (e2 - InventorySlot.MeleeWeapon).IndexToLetter();
                    if (ViewingItemList)
                    {
                        Player.Inventory.ShowEquip();
                    }
                }
                string tmpVal;
                string outVal;
                if (!ViewingEquipment)
                {
                    outVal = "Inven:";
                    if (i1 <= i2)
                    {
                        tmpVal = $" {i1.IndexToLabel()}-{i2.IndexToLabel()},";
                        outVal += tmpVal;
                    }
                    if (!ViewingItemList)
                    {
                        outVal += " * to see,";
                    }
                    if (canChooseFromEquipment)
                    {
                        outVal += " / for Equip,";
                    }
                }
                else
                {
                    outVal = "Equip:";
                    if (e1 <= e2)
                    {
                        tmpVal = $" {e1.IndexToLabel()}-{e2.IndexToLabel()}";
                        outVal += tmpVal;
                    }
                    if (!ViewingItemList)
                    {
                        outVal += " * to see,";
                    }
                    if (canChooseFromInventory)
                    {
                        outVal += " / for Inven,";
                    }
                }
                if (allowFloor)
                {
                    outVal += " - for floor,";
                }
                outVal += " ESC";
                tmpVal = $"({outVal}) {prompt}";
                Gui.PrintLine(tmpVal, 0, 0);
                char which = Gui.Inkey();
                int k;
                switch (which)
                {
                    case '\x1b':
                        {
                            ItemDisplayColumn = 50;
                            done = true;
                            break;
                        }
                    case '*':
                    case '?':
                    case ' ':
                        {
                            if (!ViewingItemList)
                            {
                                Gui.Save();
                                ViewingItemList = true;
                            }
                            else
                            {
                                Gui.Load();
                                ViewingItemList = false;
                            }
                            break;
                        }
                    case '/':
                        {
                            if (!canChooseFromInventory || !canChooseFromEquipment)
                            {
                                break;
                            }
                            if (ViewingItemList)
                            {
                                Gui.Load();
                                Gui.Save();
                            }
                            ViewingEquipment = !ViewingEquipment;
                            break;
                        }
                    case '-':
                        {
                            if (allowFloor)
                            {
                                for (currentItemIndex = tile.ItemIndex; currentItemIndex != 0; currentItemIndex = nextItemIndex)
                                {
                                    Item oPtr = Level.Items[currentItemIndex];
                                    nextItemIndex = oPtr.NextInStack;
                                    if (!inventory.ItemMatchesFilter(oPtr))
                                    {
                                        continue;
                                    }
                                    itemIndex = 0 - currentItemIndex;
                                    item = true;
                                    done = true;
                                    break;
                                }
                                if (done)
                                {
                                }
                            }
                            break;
                        }
                    case '\n':
                    case '\r':
                        {
                            if (!ViewingEquipment)
                            {
                                k = i1 == i2 ? i1 : -1;
                            }
                            else
                            {
                                k = e1 == e2 ? e1 : -1;
                            }
                            if (!inventory.GetItemOkay(k))
                            {
                                break;
                            }
                            itemIndex = k;
                            item = true;
                            done = true;
                            break;
                        }
                    default:
                        {
                            bool ver = char.IsUpper(which);
                            if (ver)
                            {
                                which = char.ToLower(which);
                            }
                            k = !ViewingEquipment ? Player.Inventory.LabelToInven(which) : Player.Inventory.LabelToEquip(which);
                            if (!inventory.GetItemOkay(k))
                            {
                                break;
                            }
                            if (ver && !Verify("Try", k))
                            {
                                done = true;
                                break;
                            }
                            itemIndex = k;
                            item = true;
                            done = true;
                            break;
                        }
                }
            }
            if (ViewingItemList)
            {
                Gui.Load();
            }
            ViewingItemList = false;
            Inventory.ItemFilterCategory = 0;
            ItemFilter = null;
            Gui.PrintLine("", 0, 0);
            return item;
        }

        public Store GetWhichStore()
        {
            foreach (Store store in CurTown.Stores)
            {
                if (Player.MapX == store.X && Player.MapY == store.Y)
                {
                    return store;
                }
            }
            return null;
        }

        public void HandleStuff()
        {
            // Oops - we might have just died...
            if (Player == null)
            {
                return;
            }
            if (Player.UpdatesNeeded.IsSet())
            {
                UpdateStuff();
            }
            if (Player.RedrawNeeded.IsSet())
            {
                RedrawStuff();
            }
        }

        public void HealthTrack(int mIdx)
        {
            TrackedMonsterIndex = mIdx;
            Player.RedrawNeeded.Set(RedrawFlag.PrHealth);
        }

        public void Initialise()
        {
            InitialiseAllocationTables();
        }

        public void MonsterDeath(int mIdx)
        {
            int dumpItem = 0;
            int dumpGold = 0;
            int number = 0;
            int qIdx = 0;
            bool quest = false;
            int nextOIdx;
            Monster mPtr = Level.Monsters[mIdx];
            MonsterRace rPtr = mPtr.Race;
            if (rPtr == null)
            {
                return;
            }
            bool visible = mPtr.IsVisible || (rPtr.Flags1 & MonsterFlag1.Unique) != 0;
            bool good = (rPtr.Flags1 & MonsterFlag1.DropGood) != 0;
            bool great = (rPtr.Flags1 & MonsterFlag1.DropGreat) != 0;
            bool doGold = (rPtr.Flags1 & MonsterFlag1.OnlyDropItem) == 0;
            bool doItem = (rPtr.Flags1 & MonsterFlag1.OnlyDropGold) == 0;
            bool cloned = false;
            int forceCoin = rPtr.GetCoinType();
            Item qPtr;
            int y = mPtr.MapY;
            int x = mPtr.MapX;
            if ((mPtr.Mind & Constants.SmCloned) != 0)
            {
                cloned = true;
            }
            for (int thisOIdx = mPtr.FirstHeldItemIndex; thisOIdx != 0; thisOIdx = nextOIdx)
            {
                Item oPtr = Level.Items[thisOIdx];
                nextOIdx = oPtr.NextInStack;
                oPtr.HoldingMonsterIndex = 0;
                qPtr = new Item(Level.Items[thisOIdx]);
                Level.DeleteObjectIdx(thisOIdx);
                Level.DropNear(qPtr, -1, y, x);
            }
            if (mPtr.StolenGold > 0)
            {
                Item oPtr = new Item();
                Item.CoinType = 10;
                oPtr.MakeGold();
                Item.CoinType = 0;
                oPtr.TypeSpecificValue = mPtr.StolenGold;
                Level.DropNear(oPtr, -1, y, x);
            }
            mPtr.FirstHeldItemIndex = 0;
            if ((rPtr.Flags1 & MonsterFlag1.Drop60) != 0 && Program.Rng.RandomLessThan(100) < 60)
            {
                number++;
            }
            if ((rPtr.Flags1 & MonsterFlag1.Drop90) != 0 && Program.Rng.RandomLessThan(100) < 90)
            {
                number++;
            }
            if ((rPtr.Flags1 & MonsterFlag1.Drop_1D2) != 0)
            {
                number += Program.Rng.DiceRoll(1, 2);
            }
            if ((rPtr.Flags1 & MonsterFlag1.Drop_2D2) != 0)
            {
                number += Program.Rng.DiceRoll(2, 2);
            }
            if ((rPtr.Flags1 & MonsterFlag1.Drop_3D2) != 0)
            {
                number += Program.Rng.DiceRoll(3, 2);
            }
            if ((rPtr.Flags1 & MonsterFlag1.Drop_4D2) != 0)
            {
                number += Program.Rng.DiceRoll(4, 2);
            }
            if (cloned)
            {
                number = 0;
            }
            if (Quests.IsQuest(CurrentDepth) && (rPtr.Flags1 & MonsterFlag1.Guardian) != 0)
            {
                qIdx = Quests.GetQuestNumber();
                Quests[qIdx].Killed++;
                if (Quests[qIdx].Killed == Quests[qIdx].ToKill)
                {
                    great = true;
                    good = true;
                    doGold = false;
                    number += 2;
                    quest = true;
                    Quests[qIdx].Level = 0;
                }
            }
            Item.CoinType = forceCoin;
            Level.ObjectLevel = (Difficulty + rPtr.Level) / 2;
            for (int j = 0; j < number; j++)
            {
                qPtr = new Item();
                if (doGold && (!doItem || Program.Rng.RandomLessThan(100) < 50))
                {
                    if (!qPtr.MakeGold())
                    {
                        continue;
                    }
                    dumpGold++;
                }
                else
                {
                    if (!quest || j > 1)
                    {
                        if (!qPtr.MakeObject(good, great))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (!qPtr.MakeObject(true, true))
                        {
                            continue;
                        }
                    }
                    dumpItem++;
                }
                Level.DropNear(qPtr, -1, y, x);
            }
            Level.ObjectLevel = Difficulty;
            Item.CoinType = 0;
            if (visible && (dumpItem != 0 || dumpGold != 0))
            {
                Level.Monsters.LoreTreasure(mIdx, dumpItem, dumpGold);
            }
            if ((rPtr.Flags1 & MonsterFlag1.Guardian) == 0)
            {
                return;
            }
            if (Quests[qIdx].Killed != Quests[qIdx].ToKill)
            {
                return;
            }
            rPtr.Flags1 ^= MonsterFlag1.Guardian;
            if (Quests.ActiveQuests == 0)
            {
                Player.IsWinner = true;
                Player.RedrawNeeded.Set(RedrawFlag.PrTitle);
                Profile.Instance.MsgPrint("*** CONGRATULATIONS ***");
                Profile.Instance.MsgPrint("You have won the game!");
                Profile.Instance.MsgPrint("You may retire ('Q') when you are ready.");
            }
            else
            {
                if (CurrentDepth < CurDungeon.MaxLevel)
                {
                    while (!Level.CaveValidBold(y, x))
                    {
                        const int d = 1;
                        Level.Scatter(out int ny, out int nx, y, x, d);
                        y = ny;
                        x = nx;
                    }
                    Level.DeleteObject(y, x);
                    Profile.Instance.MsgPrint("A magical stairway appears...");
                    Level.CaveSetFeat(y, x, CurDungeon.Tower ? "UpStair" : "DownStair");
                    Player.UpdatesNeeded.Set(UpdateFlags.UpdateView | UpdateFlags.UpdateLight | UpdateFlags.UpdateScent | UpdateFlags.UpdateMonsters);
                }
            }
        }

        public void NoticeStuff()
        {
            if (Player.NoticeFlags == 0)
            {
                return;
            }
            if ((Player.NoticeFlags & Constants.PnCombine) != 0)
            {
                Player.NoticeFlags &= ~Constants.PnCombine;
                Player.Inventory.CombinePack();
            }
            if ((Player.NoticeFlags & Constants.PnReorder) != 0)
            {
                Player.NoticeFlags &= ~Constants.PnReorder;
                Player.Inventory.ReorderPack();
            }
        }

        public void OpenChest(int y, int x, int oIdx)
        {
            Item oPtr = Level.Items[oIdx];
            bool small = oPtr.ItemSubCategory < ItemSubCategory.SvChestMinLarge;
            int number = oPtr.ItemSubCategory % ItemSubCategory.SvChestMinLarge * 2;
            if (oPtr.TypeSpecificValue == 0)
            {
                number = 0;
            }
            Level.OpeningChest = true;
            Level.ObjectLevel = Math.Abs(oPtr.TypeSpecificValue) + 10;
            for (; number > 0; --number)
            {
                Item qPtr = new Item();
                if (small && Program.Rng.RandomLessThan(100) < 75)
                {
                    if (!qPtr.MakeGold())
                    {
                        continue;
                    }
                }
                else
                {
                    if (!qPtr.MakeObject(false, false))
                    {
                        continue;
                    }
                }
                Level.DropNear(qPtr, -1, y, x);
            }
            Level.ObjectLevel = Difficulty;
            Level.OpeningChest = false;
            oPtr.TypeSpecificValue = 0;
            oPtr.BecomeKnown();
        }

        public void Play()
        {
            Gui.FullScreenOverlay = true;
            Gui.SetBackground(Terminal.BackgroundImage.Normal);
            Gui.CursorVisible = false;
            if (Program.Rng.UseFixed)
            {
                Program.Rng.UseFixed = false;
            }
            if (Player == null)
            {
                PlayerFactory factory = new PlayerFactory();
                Player newPlayer = factory.CharacterGeneration(Profile.Instance.ExPlayer);
                if (newPlayer == null)
                {
                    return;
                }
                Player = newPlayer;
                foreach (Town town in Towns)
                {
                    foreach (Store store in town.Stores)
                    {
                        store.StoreInit();
                        store.StoreMaint();
                    }
                }
                Level = null;
                _seedFlavor = Program.Rng.RandomLessThan(int.MaxValue);
                CreateWorld();
                foreach (var dungeon in Dungeons)
                {
                    dungeon.RandomiseOffset();
                }
                Profile.Instance.ItemTypes.ResetStompability();
                CurrentDepth = 0;
                CurTown = Towns[Program.Rng.RandomLessThan(Towns.Length)];
                while (CurTown.Char == 'K' || CurTown.Char == 'N')
                {
                    CurTown = Towns[Program.Rng.RandomLessThan(Towns.Length)];
                }
                CurDungeon = Dungeons[CurTown.Index];
                RecallDungeon = CurDungeon.Index;
                Player.MaxDlv[RecallDungeon] = 1;
                DungeonDifficulty = 0;
                Player.WildernessX = CurTown.X;
                Player.WildernessY = CurTown.Y;
                CameFrom = LevelStart.StartRandom;
            }
            Profile.Instance.MsgFlag = false;
            Profile.Instance.MsgPrint(null);
            Gui.Refresh();
            FlavorInit();
            ApplyFlavourVisuals();
            if (Level == null)
            {
                Level = new Level();
                LevelFactory factory = new LevelFactory(Level);
                factory.GenerateNewLevel();
            }
            Gui.FullScreenOverlay = false;
            Gui.SetBackground(Terminal.BackgroundImage.Overhead);
            Playing = true;
            if (Player.Health < 0)
            {
                Player.IsDead = true;
            }
            while (true)
            {
                DungeonLoop();
                if (Player.NoticeFlags != 0)
                {
                    NoticeStuff();
                }
                if (Player.UpdatesNeeded.IsSet())
                {
                    UpdateStuff();
                }
                if (Player.RedrawNeeded.IsSet())
                {
                    RedrawStuff();
                }
                TargetWho = 0;
                HealthTrack(0);
                Level.ForgetLight();
                Level.ForgetView();
                if (!Playing && !Player.IsDead)
                {
                    break;
                }
                Level.WipeOList();
                _petList = Level.Monsters.GetPets();
                Level.Monsters.WipeMList();
                Profile.Instance.MsgPrint(null);
                if (Player.IsDead)
                {
                    // Store the player info
                    Profile.Instance.ExPlayer = new ExPlayer(Player);
                    break;
                }
                Level = new Level();
                LevelFactory factory = new LevelFactory(Level);
                factory.GenerateNewLevel();
                Level.ReplacePets(Player.MapY, Player.MapX, _petList);
            }
            CloseGame();
        }

        public void UpdateStuff()
        {
            if (Player.UpdatesNeeded.IsClear())
            {
                return;
            }
            PlayerStatus playerStatus = new PlayerStatus(Player, Level);
            if (Player.UpdatesNeeded.IsSet(UpdateFlags.UpdateBonuses))
            {
                Player.UpdatesNeeded.Clear(UpdateFlags.UpdateBonuses);
                playerStatus.CalcBonuses();
            }
            if (Player.UpdatesNeeded.IsSet(UpdateFlags.UpdateTorchRadius))
            {
                Player.UpdatesNeeded.Clear(UpdateFlags.UpdateTorchRadius);
                playerStatus.CalcTorch();
            }
            if (Player.UpdatesNeeded.IsSet(UpdateFlags.UpdateHealth))
            {
                Player.UpdatesNeeded.Clear(UpdateFlags.UpdateHealth);
                playerStatus.CalcHitpoints();
            }
            if (Player.UpdatesNeeded.IsSet(UpdateFlags.UpdateMana))
            {
                Player.UpdatesNeeded.Clear(UpdateFlags.UpdateMana);
                playerStatus.CalcMana();
            }
            if (Player.UpdatesNeeded.IsSet(UpdateFlags.UpdateSpells))
            {
                Player.UpdatesNeeded.Clear(UpdateFlags.UpdateSpells);
                playerStatus.CalcSpells();
            }
            if (Player == null)
            {
                return;
            }
            if (Gui.FullScreenOverlay)
            {
                return;
            }
            if (Player.UpdatesNeeded.IsSet(UpdateFlags.UpdateRemoveLight))
            {
                Player.UpdatesNeeded.Clear(UpdateFlags.UpdateRemoveLight);
                Level.ForgetLight();
            }
            if (Player.UpdatesNeeded.IsSet(UpdateFlags.UpdateRemoveView))
            {
                Player.UpdatesNeeded.Clear(UpdateFlags.UpdateRemoveView);
                Level.ForgetView();
            }
            if (Player.UpdatesNeeded.IsSet(UpdateFlags.UpdateView))
            {
                Player.UpdatesNeeded.Clear(UpdateFlags.UpdateView);
                Level.UpdateView();
            }
            if (Player.UpdatesNeeded.IsSet(UpdateFlags.UpdateLight))
            {
                Player.UpdatesNeeded.Clear(UpdateFlags.UpdateLight);
                Level.UpdateLight();
            }
            if (Player.UpdatesNeeded.IsSet(UpdateFlags.UpdateScent))
            {
                Player.UpdatesNeeded.Clear(UpdateFlags.UpdateScent);
                Level.UpdateFlow();
            }
            if (Player.UpdatesNeeded.IsSet(UpdateFlags.UpdateDistances))
            {
                Player.UpdatesNeeded.Clear(UpdateFlags.UpdateDistances);
                Player.UpdatesNeeded.Clear(UpdateFlags.UpdateMonsters);
                Level.UpdateMonsters(true);
            }
            if (Player.UpdatesNeeded.IsSet(UpdateFlags.UpdateMonsters))
            {
                Player.UpdatesNeeded.Clear(UpdateFlags.UpdateMonsters);
                Level.UpdateMonsters(false);
            }
        }

        private void ApplyFlavourVisuals()
        {
            int i;
            for (i = 0; i < Profile.Instance.ItemTypes.Count; i++)
            {
                ItemType kPtr = Profile.Instance.ItemTypes[i];
                EntityType visual = ObjectFlavourEntity(i);
                if (visual != null)
                {
                    kPtr.Character = visual.Character;
                    kPtr.Colour = visual.Colour;
                }
            }
        }

        private void CloseGame()
        {
            HandleStuff();
            Profile.Instance.MsgPrint(null);
            Gui.FullScreenOverlay = true;
            if (Player.IsDead)
            {
                if (Player.IsWinner)
                {
                    Kingly();
                }
                Player corpse = Player;
                HighScore score = new HighScore(Player);
                Player = null;
                SavePlayer();
                PrintTomb(corpse);
                if (corpse.IsWizard)
                {
                    return;
                }
                Program.HiScores.InsertNewScore(score);
                Program.HiScores.DisplayScores(score.Pts);
            }
            else
            {
                IsAutosave = false;
                DoCmdSaveGame();
                if (!Program.ExitToDesktop)
                {
                    Gui.Mixer.Play(MusicTrack.Menu);
                    Program.HiScores.DisplayScores(new HighScore(Player));
                }
            }
        }

        private void CreateWorld()
        {
            int i;
            int j;
            int x = 0;
            int y = 0;
            Wilderness.MakeIslandContours();
            for (i = 0; i < 12; i++)
            {
                for (j = 0; j < 12; j++)
                {
                    Wilderness[i][j].Seed = Program.Rng.RandomLessThan(int.MaxValue);
                    Wilderness[i][j].Dungeon = null;
                    Wilderness[i][j].Town = null;
                    Wilderness[i][j].RoadMap = 0;
                }
            }
            for (i = 0; i < Towns.Length; i++)
            {
                Towns[i].Seed = Program.Rng.RandomLessThan(int.MaxValue);
                Towns[i].Visited = false;
                Towns[i].X = 0;
                Towns[i].Y = 0;
            }
            for (i = 0; i < Constants.MaxCaves; i++)
            {
                Dungeons[i].X = 0;
                Dungeons[i].Y = 0;
            }
            for (i = 0; i < Constants.MaxCaves; i++)
            {
                Dungeons[i].Visited = false;
                Dungeons[i].KnownDepth = false;
                Dungeons[i].KnownOffset = false;
                if (i < Towns.Length)
                {
                    Dungeons[i].Visited = true;
                    j = 0;
                    while (j == 0)
                    {
                        x = Program.Rng.RandomBetween(2, 9);
                        y = Program.Rng.RandomBetween(2, 9);
                        j = 1;
                        if (Wilderness[y][x].Dungeon != null || Wilderness[y - 1][x].Dungeon != null ||
                            Wilderness[y + 1][x].Dungeon != null || Wilderness[y][x - 1].Dungeon != null ||
                            Wilderness[y][x + 1].Dungeon != null || Wilderness[y - 1][x + 1].Dungeon != null ||
                            Wilderness[y + 1][x + 1].Dungeon != null || Wilderness[y - 1][x - 1].Dungeon != null ||
                            Wilderness[y + 1][x - 1].Dungeon != null)
                        {
                            j = 0;
                        }
                    }
                }
                else
                {
                    j = 0;
                    while (j == 0)
                    {
                        x = Program.Rng.RandomBetween(2, 9);
                        y = Program.Rng.RandomBetween(2, 9);
                        j = 1;
                        if (Wilderness[y][x].Dungeon != null)
                        {
                            j = 0;
                        }
                    }
                }
                Wilderness[y][x].Dungeon = Dungeons[i];
                if (i < Towns.Length)
                {
                    Wilderness[y][x].Town = Towns[i];
                    Towns[i].X = x;
                    Towns[i].Y = y;
                }
                Dungeons[i].X = x;
                Dungeons[i].Y = y;
            }
            for (i = 0; i < Towns.Length - 1; i++)
            {
                int curX = Towns[i].X;
                int curY = Towns[i].Y;
                int destX = Towns[i + 1].X;
                int destY = Towns[i + 1].Y;
                bool fin = false;
                while (!fin)
                {
                    int xDisp = destX - curX;
                    int xSgn = 0;
                    if (xDisp > 0)
                    {
                        xSgn = 1;
                    }
                    if (xDisp < 0)
                    {
                        xSgn = -1;
                        xDisp = -xDisp;
                    }
                    int yDisp = destY - curY;
                    int ySgn = 0;
                    if (yDisp > 0)
                    {
                        ySgn = 1;
                    }
                    if (yDisp < 0)
                    {
                        ySgn = -1;
                        yDisp = -yDisp;
                    }
                    if (xDisp == 0 && yDisp == 0)
                    {
                        fin = true;
                    }
                    else
                    {
                        int curdir;
                        int nextdir;
                        if (xDisp == yDisp && xSgn == 1 && ySgn == -1)
                        {
                            curdir = Constants.RoadUp;
                            nextdir = Constants.RoadDown;
                        }
                        else if (xSgn == 1 && xDisp >= yDisp)
                        {
                            curdir = Constants.RoadRight;
                            nextdir = Constants.RoadLeft;
                        }
                        else if (ySgn == 1 && yDisp >= xDisp)
                        {
                            curdir = Constants.RoadDown;
                            nextdir = Constants.RoadUp;
                        }
                        else if (xSgn == -1 && xDisp >= yDisp)
                        {
                            curdir = Constants.RoadLeft;
                            nextdir = Constants.RoadRight;
                        }
                        else
                        {
                            curdir = Constants.RoadUp;
                            nextdir = Constants.RoadDown;
                        }
                        Wilderness[curY][curX].RoadMap |= curdir;
                        if (curdir == Constants.RoadRight)
                        {
                            curX++;
                        }
                        else if (curdir == Constants.RoadLeft)
                        {
                            curX--;
                        }
                        else if (curdir == Constants.RoadDown)
                        {
                            curY++;
                        }
                        else
                        {
                            curY--;
                        }
                        Wilderness[curY][curX].RoadMap |= nextdir;
                    }
                }
            }
        }

        private void DungeonLoop()
        {
            TargetEngine targetEngine = new TargetEngine(Player, Level);
            NewLevelFlag = false;
            HackMind = false;
            Gui.CurrentCommand = (char)0;
            Gui.QueuedCommand = (char)0;
            Command.CommandRepeat = 0;
            Gui.CommandArgument = 0;
            Gui.CommandDirection = 0;
            TargetWho = 0;
            HealthTrack(0);
            Level.Monsters.ShimmerMonsters = true;
            Level.Monsters.RepairMonsters = true;
            Disturb(true);
            if (Player.MaxLevelGained < Player.Level)
            {
                Player.MaxLevelGained = Player.Level;
            }
            if (Player.MaxDlv[CurDungeon.Index] < CurrentDepth)
            {
                Player.MaxDlv[CurDungeon.Index] = CurrentDepth;
            }
            if (Quests.IsQuest(CurrentDepth))
            {
                if (CurDungeon.Tower)
                {
                    CreateUpStair = false;
                }
                else
                {
                    CreateDownStair = false;
                }
            }
            if (CurrentDepth <= 0)
            {
                CreateDownStair = false;
                CreateUpStair = false;
            }
            if (CreateUpStair || CreateDownStair)
            {
                if (Level.CaveValidBold(Player.MapY, Player.MapX))
                {
                    Level.DeleteObject(Player.MapY, Player.MapX);
                    Level.CaveSetFeat(Player.MapY, Player.MapX,
                        CreateDownStair ? "DownStair" : "UpStair");
                }
                CreateDownStair = false;
                CreateUpStair = false;
            }
            targetEngine.RecenterScreenAroundPlayer();
            targetEngine.PanelBoundsCenter();
            Profile.Instance.MsgPrint(null);
            CharacterXtra = true;
            Player.RedrawNeeded.Set(RedrawFlag.PrWipe | RedrawFlag.PrBasic | RedrawFlag.PrExtra | RedrawFlag.PrEquippy);
            Player.RedrawNeeded.Set(RedrawFlag.PrMap);
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses | UpdateFlags.UpdateHealth | UpdateFlags.UpdateMana | UpdateFlags.UpdateSpells);
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateTorchRadius);
            UpdateStuff();
            RedrawStuff();
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateView | UpdateFlags.UpdateLight | UpdateFlags.UpdateScent | UpdateFlags.UpdateDistances);
            UpdateStuff();
            RedrawStuff();
            CharacterXtra = false;
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses | UpdateFlags.UpdateHealth | UpdateFlags.UpdateMana | UpdateFlags.UpdateSpells);
            Player.NoticeFlags |= Constants.PnCombine | Constants.PnReorder;
            NoticeStuff();
            UpdateStuff();
            RedrawStuff();
            Gui.Refresh();
            if (!Playing || Player.IsDead || NewLevelFlag)
            {
                return;
            }
            if (Quests.IsQuest(CurrentDepth))
            {
                Quests.QuestDiscovery();
            }
            Level.MonsterLevel = Difficulty;
            Level.ObjectLevel = Difficulty;
            HackMind = true;
            if (CameFrom == LevelStart.StartHouse)
            {
                UpdateCommand();
                StoreCommand.DoCmdStore(Player, SaveGame.Instance.Level);
                CameFrom = LevelStart.StartRandom;
            }
            if (CurrentDepth == 0)
            {
                if (Difficulty == 0)
                {
                    Gui.Mixer.Play(MusicTrack.Town);
                }
                else
                {
                    Gui.Mixer.Play(MusicTrack.Wilderness);
                }
            }
            else
            {
                if (Quests.IsQuest(CurrentDepth))
                {
                    Gui.Mixer.Play(MusicTrack.QuestLevel);
                }
                else
                {
                    Gui.Mixer.Play(MusicTrack.Dungeon);
                }
            }
            while (true)
            {
                if (Level.MCnt + 32 > Constants.MaxMIdx)
                {
                    Level.Monsters.CompactMonsters(64);
                }
                if (Level.MCnt + 32 < Level.MMax)
                {
                    Level.Monsters.CompactMonsters(0);
                }
                if (Level.OCnt + 32 > Constants.MaxOIdx)
                {
                    Level.CompactObjects(64);
                }
                if (Level.OCnt + 32 < Level.OMax)
                {
                    Level.CompactObjects(0);
                }
                ProcessPlayer();
                if (Player.NoticeFlags != 0)
                {
                    NoticeStuff();
                }
                if (Player.UpdatesNeeded.IsSet())
                {
                    UpdateStuff();
                }
                if (Player.RedrawNeeded.IsSet())
                {
                    RedrawStuff();
                }
                Level.MoveCursorRelative(Player.MapY, Player.MapX);
                if (!Playing || Player.IsDead || NewLevelFlag)
                {
                    break;
                }
                TotalFriends = 0;
                TotalFriendLevels = 0;
                ArtificialIntelligence ai = new ArtificialIntelligence(Player, Level);
                ai.ProcessAllMonsters();
                if (Player.NoticeFlags != 0)
                {
                    NoticeStuff();
                }
                if (Player.UpdatesNeeded.IsSet())
                {
                    UpdateStuff();
                }
                if (Player.RedrawNeeded.IsSet())
                {
                    RedrawStuff();
                }
                Level.MoveCursorRelative(Player.MapY, Player.MapX);
                if (!Playing || Player.IsDead || NewLevelFlag)
                {
                    break;
                }
                ProcessWorld();
                if (Player.NoticeFlags != 0)
                {
                    NoticeStuff();
                }
                if (Player.UpdatesNeeded.IsSet())
                {
                    UpdateStuff();
                }
                if (Player.RedrawNeeded.IsSet())
                {
                    RedrawStuff();
                }
                Level.MoveCursorRelative(Player.MapY, Player.MapX);
                if (!Playing || Player.IsDead || NewLevelFlag)
                {
                    break;
                }
                Player.GameTime.Tick();
                if (!Player.GameTime.IsFeelTime)
                {
                    continue;
                }
                if (CurrentDepth > 0)
                {
                    Commands.FeelingAndLocationCommand.DoCmdFeeling(Player, Level, true);
                }
            }
        }

        private void FlavorInit()
        {
            int i, j;
            Program.Rng.UseFixed = true;
            Program.Rng.FixedSeed = _seedFlavor;
            PotionFlavours = new List<PotionFlavour>();
            List<PotionFlavour> tempPotions = new List<PotionFlavour>();
            PotionFlavours.Add(StaticResources.Instance.PotionFlavours["Clear"]);
            PotionFlavours.Add(StaticResources.Instance.PotionFlavours["Light Brown"]);
            PotionFlavours.Add(StaticResources.Instance.PotionFlavours["Icky Green"]);
            foreach (KeyValuePair<string, PotionFlavour> pair in StaticResources.Instance.PotionFlavours)
            {
                if (pair.Key == "Clear")
                {
                    continue;
                }
                if (pair.Key == "Light Brown")
                {
                    continue;
                }
                if (pair.Key == "Icky Green")
                {
                    continue;
                }
                tempPotions.Add(pair.Value);
            }
            do
            {
                int index = Program.Rng.RandomLessThan(tempPotions.Count);
                PotionFlavours.Add(tempPotions[index]);
                tempPotions.RemoveAt(index);
            } while (tempPotions.Count > 0);
            MushroomFlavours = new List<MushroomFlavour>();
            List<MushroomFlavour> tempMushrooms = new List<MushroomFlavour>();
            foreach (KeyValuePair<string, MushroomFlavour> pair in StaticResources.Instance.MushroomFlavours)
            {
                tempMushrooms.Add(pair.Value);
            }
            do
            {
                int index = Program.Rng.RandomLessThan(tempMushrooms.Count);
                MushroomFlavours.Add(tempMushrooms[index]);
                tempMushrooms.RemoveAt(index);
            } while (tempMushrooms.Count > 0);
            AmuletFlavours = new List<AmuletFlavour>();
            List<AmuletFlavour> tempAmulets = new List<AmuletFlavour>();
            foreach (KeyValuePair<string, AmuletFlavour> pair in StaticResources.Instance.AmuletFlavours)
            {
                tempAmulets.Add(pair.Value);
            }
            do
            {
                int index = Program.Rng.RandomLessThan(tempAmulets.Count);
                AmuletFlavours.Add(tempAmulets[index]);
                tempAmulets.RemoveAt(index);
            } while (tempAmulets.Count > 0);
            WandFlavours = new List<WandFlavour>();
            List<WandFlavour> tempWands = new List<WandFlavour>();
            foreach (KeyValuePair<string, WandFlavour> pair in StaticResources.Instance.WandFlavours)
            {
                tempWands.Add(pair.Value);
            }
            do
            {
                int index = Program.Rng.RandomLessThan(tempWands.Count);
                WandFlavours.Add(tempWands[index]);
                tempWands.RemoveAt(index);
            } while (tempWands.Count > 0);
            RingFlavours = new List<RingFlavour>();
            List<RingFlavour> tempRings = new List<RingFlavour>();
            foreach (KeyValuePair<string, RingFlavour> pair in StaticResources.Instance.RingFlavours)
            {
                tempRings.Add(pair.Value);
            }
            do
            {
                int index = Program.Rng.RandomLessThan(tempRings.Count);
                RingFlavours.Add(tempRings[index]);
                tempRings.RemoveAt(index);
            } while (tempRings.Count > 0);
            RodFlavours = new List<RodFlavour>();
            List<RodFlavour> tempRods = new List<RodFlavour>();
            foreach (KeyValuePair<string, RodFlavour> pair in StaticResources.Instance.RodFlavours)
            {
                tempRods.Add(pair.Value);
            }
            do
            {
                int index = Program.Rng.RandomLessThan(tempRods.Count);
                RodFlavours.Add(tempRods[index]);
                tempRods.RemoveAt(index);
            } while (tempRods.Count > 0);
            StaffFlavours = new List<StaffFlavour>();
            List<StaffFlavour> tempStaffs = new List<StaffFlavour>();
            foreach (KeyValuePair<string, StaffFlavour> pair in StaticResources.Instance.StaffFlavours)
            {
                tempStaffs.Add(pair.Value);
            }
            do
            {
                int index = Program.Rng.RandomLessThan(tempStaffs.Count);
                StaffFlavours.Add(tempStaffs[index]);
                tempStaffs.RemoveAt(index);
            } while (tempStaffs.Count > 0);
            ScrollFlavours = new List<ScrollFlavour>();
            List<ScrollFlavour> tempScrolls = new List<ScrollFlavour>();
            foreach (KeyValuePair<string, ScrollFlavour> pair in StaticResources.Instance.ScrollFlavours)
            {
                tempScrolls.Add(pair.Value);
            }
            for (i = 0; i < Constants.MaxTitles; i++)
            {
                ScrollFlavour flavour = new ScrollFlavour();
                ScrollFlavours.Add(flavour);
                int index = Program.Rng.RandomLessThan(tempScrolls.Count);
                flavour.Character = tempScrolls[index].Character;
                flavour.Colour = tempScrolls[index].Colour;
                while (true)
                {
                    string buf = "";
                    while (true)
                    {
                        string tmp = "";
                        int s = Program.Rng.RandomLessThan(100) < 30 ? 1 : 2;
                        for (int q = 0; q < s; q++)
                        {
                            tmp += ScrollFlavour.Syllables[Program.Rng.RandomLessThan(ScrollFlavour.Syllables.Length)];
                        }
                        if (buf.Length + tmp.Length > 14)
                        {
                            break;
                        }
                        buf += " ";
                        buf += tmp;
                    }
                    flavour.Name = buf.Substring(1);
                    bool okay = true;
                    for (j = 0; j < i; j++)
                    {
                        string hack1 = ScrollFlavours[j].Name;
                        string hack2 = ScrollFlavours[i].Name;
                        if (hack1.Substring(0, 4) != hack2.Substring(0, 4))
                        {
                            continue;
                        }
                        okay = false;
                        break;
                    }
                    if (okay)
                    {
                        break;
                    }
                }
            }
            Program.Rng.UseFixed = false;
            for (i = 1; i < Profile.Instance.ItemTypes.Count; i++)
            {
                ItemType kPtr = Profile.Instance.ItemTypes[i];
                if (string.IsNullOrEmpty(kPtr.Name))
                {
                    continue;
                }
                kPtr.HasFlavor = Inventory.ObjectHasFlavor(kPtr);
                if (!kPtr.HasFlavor)
                {
                    kPtr.FlavourAware = true;
                }
                kPtr.EasyKnow = Inventory.ObjectEasyKnow(i);
            }
        }

        private void InitialiseAllocationTables()
        {
            int i, j;
            ItemType kPtr;
            MonsterRace rPtr;
            int[] num = new int[Constants.MaxDepth];
            int[] aux = new int[Constants.MaxDepth];
            AllocKindSize = 0;
            for (i = 1; i < Profile.Instance.ItemTypes.Count; i++)
            {
                kPtr = Profile.Instance.ItemTypes[i];
                for (j = 0; j < 4; j++)
                {
                    if (kPtr.Chance[j] != 0)
                    {
                        AllocKindSize++;
                        num[kPtr.Locale[j]]++;
                    }
                }
            }
            for (i = 1; i < Constants.MaxDepth; i++)
            {
                num[i] += num[i - 1];
            }
            if (num[0] == 0)
            {
                Program.Quit("No town objects!");
            }
            AllocKindTable = new AllocationEntry[AllocKindSize];
            for (int k = 0; k < AllocKindSize; k++)
            {
                AllocKindTable[k] = new AllocationEntry();
            }
            AllocationEntry[] table = AllocKindTable;
            for (i = 1; i < Profile.Instance.ItemTypes.Count; i++)
            {
                kPtr = Profile.Instance.ItemTypes[i];
                for (j = 0; j < 4; j++)
                {
                    if (kPtr.Chance[j] != 0)
                    {
                        int x = kPtr.Locale[j];
                        int p = 100 / kPtr.Chance[j];
                        int y = x > 0 ? num[x - 1] : 0;
                        int z = y + aux[x];
                        table[z].Index = i;
                        table[z].Level = x;
                        table[z].BaseProbability = p;
                        table[z].FilteredProbabiity = p;
                        table[z].FinalProbability = p;
                        aux[x]++;
                    }
                }
            }
            aux = new int[Constants.MaxDepth];
            num = new int[Constants.MaxDepth];
            AllocRaceSize = 0;
            for (i = 1; i < Profile.Instance.MonsterRaces.Count - 1; i++)
            {
                rPtr = Profile.Instance.MonsterRaces[i];
                if (rPtr.Rarity != 0)
                {
                    AllocRaceSize++;
                    num[rPtr.Level]++;
                }
            }
            for (i = 1; i < Constants.MaxDepth; i++)
            {
                num[i] += num[i - 1];
            }
            if (num[0] == 0)
            {
                Program.Quit("No town monsters!");
            }
            AllocRaceTable = new AllocationEntry[AllocRaceSize];
            for (int k = 0; k < AllocRaceSize; k++)
            {
                AllocRaceTable[k] = new AllocationEntry();
            }
            table = AllocRaceTable;
            for (i = 1; i < Profile.Instance.MonsterRaces.Count - 1; i++)
            {
                rPtr = Profile.Instance.MonsterRaces[i];
                if (rPtr.Rarity != 0)
                {
                    int x = rPtr.Level;
                    int p = 100 / rPtr.Rarity;
                    int y = x > 0 ? num[x - 1] : 0;
                    int z = y + aux[x];
                    table[z].Index = i;
                    table[z].Level = x;
                    table[z].BaseProbability = p;
                    table[z].FilteredProbabiity = p;
                    table[z].FinalProbability = p;
                    aux[x]++;
                }
            }
        }

        private void Kingly()
        {
            CurrentDepth = 0;
            DiedFrom = "Ripe Old Age";
            Player.ExperiencePoints = Player.MaxExperienceGained;
            Player.Level = Player.MaxLevelGained;
            Player.Gold += 10000000;
            Gui.SetBackground(Terminal.BackgroundImage.Crown);
            Gui.Clear();
            Gui.AnyKey(44);
        }

        private EntityType ObjectFlavourEntity(int i)
        {
            ItemType kPtr = Profile.Instance.ItemTypes[i];
            if (kPtr.HasFlavor)
            {
                int indexx = kPtr.SubCategory;
                switch (kPtr.Category)
                {
                    case ItemCategory.Food:
                        return MushroomFlavours[indexx];

                    case ItemCategory.Potion:
                        return PotionFlavours[indexx];

                    case ItemCategory.Scroll:
                        return ScrollFlavours[indexx];

                    case ItemCategory.Amulet:
                        return AmuletFlavours[indexx];

                    case ItemCategory.Ring:
                        return RingFlavours[indexx];

                    case ItemCategory.Staff:
                        return StaffFlavours[indexx];

                    case ItemCategory.Wand:
                        return WandFlavours[indexx];

                    case ItemCategory.Rod:
                        return RodFlavours[indexx];
                }
            }
            return null;
        }

        private void PrintTomb(Player corpse)
        {
            {
                DateTime ct = DateTime.Now;
                if (corpse.IsWinner)
                {
                    Gui.SetBackground(Terminal.BackgroundImage.Sunset);
                    Gui.Mixer.Play(MusicTrack.Victory);
                }
                else
                {
                    Gui.SetBackground(Terminal.BackgroundImage.Tomb);
                    Gui.Mixer.Play(MusicTrack.Death);
                }
                Gui.Clear();
                string buf = corpse.Name.Trim() + corpse.Generation.ToRoman(true);
                if (corpse.IsWinner || corpse.Level > Constants.PyMaxLevel)
                {
                    buf += " the Magnificent";
                }
                Gui.Print(buf, 39, 1);
                buf = $"Level {corpse.Level} {Profession.ClassSubName(corpse.ProfessionIndex, corpse.Realm1)}";
                Gui.Print(buf, 40, 1);
                string tmp = $"Killed on Level {CurrentDepth}".PadLeft(45);
                Gui.Print(tmp, 39, 34);
                tmp = $"by {DiedFrom}".PadLeft(45);
                Gui.Print(tmp, 40, 34);
                tmp = $"on {ct:dd MMM yyyy h.mm tt}".PadLeft(45);
                Gui.Print(tmp, 41, 34);
                Gui.AnyKey(44);
            }
        }

        private void ProcessPlayer()
        {
            if (Player.GetFirstLevelMutation)
            {
                Profile.Instance.MsgPrint("You feel different!");
                Player.Dna.GainMutation();
                Player.GetFirstLevelMutation = false;
            }
            Player.Energy += GlobalData.ExtractEnergy[Player.Speed];
            if (Player.Energy < 100)
            {
                return;
            }
            if (Resting < 0)
            {
                if (Resting == -1)
                {
                    if (Player.Health == Player.MaxHealth && Player.Mana >= Player.MaxMana)
                    {
                        Disturb(false);
                    }
                }
                else if (Resting == -2)
                {
                    if (Player.Health == Player.MaxHealth && Player.Mana == Player.MaxMana && Player.TimedBlindness == 0 &&
                        Player.TimedConfusion == 0 && Player.TimedPoison == 0 && Player.TimedFear == 0 && Player.TimedStun == 0 &&
                        Player.TimedBleeding == 0 && Player.TimedSlow == 0 && Player.TimedParalysis == 0 && Player.TimedHallucinations == 0 &&
                        Player.WordOfRecallDelay == 0)
                    {
                        Disturb(false);
                    }
                }
            }
            if (Running != 0 || Command.CommandRepeat != 0 || (Resting != 0 && (Resting & 0x0F) == 0))
            {
                Gui.DoNotWaitOnInkey = true;
                if (Gui.Inkey() != 0)
                {
                    Disturb(false);
                    Profile.Instance.MsgPrint("Cancelled.");
                }
            }
            while (Player.Energy >= 100)
            {
                Player.RedrawNeeded.Set(RedrawFlag.PrDtrap);
                if (Player.NoticeFlags != 0)
                {
                    NoticeStuff();
                }
                if (Player.UpdatesNeeded.IsSet())
                {
                    UpdateStuff();
                }
                if (Player.RedrawNeeded.IsSet())
                {
                    RedrawStuff();
                }
                Level.MoveCursorRelative(Player.MapY, Player.MapX);
                Gui.Refresh();
                if (Player.Inventory[InventorySlot.Pack].ItemType != null)
                {
                    const int item = InventorySlot.Pack;
                    Item oPtr = Player.Inventory[item];
                    Disturb(false);
                    Profile.Instance.MsgPrint("Your pack overflows!");
                    string oName = oPtr.Description(true, 3);
                    Profile.Instance.MsgPrint($"You drop {oName} ({item.IndexToLabel()}).");
                    Level.DropNear(oPtr, 0, Player.MapY, Player.MapX);
                    Player.Inventory.InvenItemIncrease(item, -255);
                    Player.Inventory.InvenItemDescribe(item);
                    Player.Inventory.InvenItemOptimize(item);
                    if (Player.NoticeFlags != 0)
                    {
                        NoticeStuff();
                    }
                    if (Player.UpdatesNeeded.IsSet())
                    {
                        UpdateStuff();
                    }
                    if (Player.RedrawNeeded.IsSet())
                    {
                        RedrawStuff();
                    }
                }
                if (Gui.QueuedCommand == 0)
                {
                    ViewingItemList = false;
                }
                EnergyUse = 0;
                if (Player.TimedParalysis != 0 || Player.TimedStun >= 100)
                {
                    EnergyUse = 100;
                }
                else if (Resting != 0)
                {
                    if (Resting > 0)
                    {
                        Resting--;
                        Player.RedrawNeeded.Set(RedrawFlag.PrState);
                    }
                    EnergyUse = 100;
                }
                else if (Running != 0)
                {
                    CommandEngine.RunOneStep(0);
                }
                else if (Command.CommandRepeat != 0)
                {
                    Command.CommandRepeat--;
                    Player.RedrawNeeded.Set(RedrawFlag.PrState);
                    RedrawStuff();
                    Profile.Instance.MsgFlag = false;
                    Gui.PrintLine("", 0, 0);
                    UpdateCommand();
                    Command.ProcessCommand(true);
                }
                else
                {
                    Level.MoveCursorRelative(Player.MapY, Player.MapX);
                    Gui.RequestCommand(false);
                    UpdateCommand();
                    Command.ProcessCommand(false);
                }
                if (EnergyUse != 0)
                {
                    Player.Energy -= EnergyUse;
                    int i;
                    if (Level.Monsters.ShimmerMonsters)
                    {
                        Level.Monsters.ShimmerMonsters = false;
                        for (i = 1; i < Level.MMax; i++)
                        {
                            Monster mPtr = Level.Monsters[i];
                            if (mPtr.Race == null)
                            {
                                continue;
                            }
                            MonsterRace rPtr = mPtr.Race;
                            if ((rPtr.Flags1 & MonsterFlag1.AttrMulti) == 0)
                            {
                                continue;
                            }
                            Level.Monsters.ShimmerMonsters = true;
                            Level.RedrawSingleLocation(mPtr.MapY, mPtr.MapX);
                        }
                    }
                    if (Level.Monsters.RepairMonsters)
                    {
                        Level.Monsters.RepairMonsters = false;
                        for (i = 1; i < Level.MMax; i++)
                        {
                            Monster mPtr = Level.Monsters[i];
                            if (mPtr.Race == null)
                            {
                                continue;
                            }
                            if ((mPtr.IndividualMonsterFlags & Constants.MflagNice) != 0)
                            {
                                mPtr.IndividualMonsterFlags &= ~Constants.MflagNice;
                            }
                            if ((mPtr.IndividualMonsterFlags & Constants.MflagMark) != 0)
                            {
                                if ((mPtr.IndividualMonsterFlags & Constants.MflagShow) != 0)
                                {
                                    mPtr.IndividualMonsterFlags &= ~Constants.MflagShow;
                                    Level.Monsters.RepairMonsters = true;
                                }
                                else
                                {
                                    mPtr.IndividualMonsterFlags &= ~Constants.MflagMark;
                                    mPtr.IsVisible = false;
                                    Level.Monsters.UpdateMonsterVisibility(i, false);
                                    Level.RedrawSingleLocation(mPtr.MapY, mPtr.MapX);
                                }
                            }
                        }
                    }
                }
                if (!Playing || Player.IsDead || NewLevelFlag)
                {
                    break;
                }
            }
        }

        private void ProcessWorld()
        {
            FlagSet f1 = new FlagSet();
            FlagSet f2 = new FlagSet();
            FlagSet f3 = new FlagSet();
            if (Player.GameTime.IsBirthday)
            {
                Profile.Instance.MsgPrint("Happy Birthday!");
                Level.Acquirement(Player.MapY, Player.MapX, Program.Rng.DieRoll(2) + 1, true);
                Player.Age++;
            }
            if (Player.GameTime.IsNewYear)
            {
                Profile.Instance.MsgPrint("Happy New Year!");
                Level.Acquirement(Player.MapY, Player.MapX, Program.Rng.DieRoll(2) + 1, true);
            }
            if (Player.GameTime.IsHalloween)
            {
                Profile.Instance.MsgPrint("All Hallows Eve and the ghouls come out to play...");
                Level.Monsters.SummonSpecific(Player.MapY, Player.MapX, Difficulty, Constants.SummonUndead);
            }
            if (CurrentDepth <= 0)
            {
                if (Player.GameTime.IsDawn)
                {
                    GridTile cPtr;
                    int x;
                    int y;
                    Profile.Instance.MsgPrint("The sun has risen.");
                    for (y = 0; y < Level.CurHgt; y++)
                    {
                        for (x = 0; x < Level.CurWid; x++)
                        {
                            cPtr = Level.Grid[y][x];
                            cPtr.TileFlags.Set(GridTile.SelfLit);
                            cPtr.TileFlags.Set(GridTile.PlayerMemorised);
                            Level.NoteSpot(y, x);
                        }
                    }
                }
                else if (Player.GameTime.IsDusk)
                {
                    GridTile cPtr;
                    int x;
                    int y;
                    Profile.Instance.MsgPrint("The sun has fallen.");
                    for (y = 0; y < Level.CurHgt; y++)
                    {
                        for (x = 0; x < Level.CurWid; x++)
                        {
                            cPtr = Level.Grid[y][x];
                            if (cPtr.FeatureType.IsOpenFloor)
                            {
                                cPtr.TileFlags.Clear(GridTile.SelfLit);
                                Level.NoteSpot(y, x);
                            }
                        }
                    }
                }
                Player.UpdatesNeeded.Set(UpdateFlags.UpdateMonsters);
                Player.RedrawNeeded.Set(RedrawFlag.PrMap);
            }
            if (Player.GameTime.IsMidnight)
            {
                Player.Religion.DecayFavour();
                Player.UpdatesNeeded.Set(UpdateFlags.UpdateHealth | UpdateFlags.UpdateMana);
                foreach (Town town in Towns)
                {
                    foreach (Store store in town.Stores)
                    {
                        store.StoreMaint();
                    }
                }
                if (Program.Rng.RandomLessThan(Constants.StoreShuffle) == 0)
                {
                    int town = Program.Rng.RandomLessThan(Towns.Length);
                    int store = Program.Rng.RandomLessThan(12);
                    Towns[town].Stores[store].StoreShuffle();
                }
            }
            if (!Player.GameTime.IsTurnTen)
            {
                return;
            }
            if (Program.Rng.RandomLessThan(Constants.MaxMAllocChance) == 0)
            {
                Level.Monsters.AllocMonster(Constants.MaxSight + 5, false);
            }
            if (Player.GameTime.IsTurnHundred)
            {
                RegenMonsters();
            }
            if (Player.TimedPoison != 0 && Player.TimedInvulnerability == 0)
            {
                Player.TakeHit(1, "poison");
            }
            Item oPtr;
            bool caveNoRegen = false;
            if (Player.RaceIndex == RaceId.Vampire)
            {
                if (CurrentDepth <= 0 && !Player.HasLightResistance && Player.TimedInvulnerability == 0 &&
                    Player.GameTime.IsLight)
                {
                    if (Level.Grid[Player.MapY][Player.MapX].TileFlags.IsSet(GridTile.SelfLit))
                    {
                        Profile.Instance.MsgPrint("The sun's rays scorch your undead flesh!");
                        Player.TakeHit(1, "sunlight");
                        caveNoRegen = true;
                    }
                }
                if (Player.Inventory[InventorySlot.Lightsource].Category != 0 &&
                    Player.Inventory[InventorySlot.Lightsource].ItemSubCategory >= LightType.Galadriel &&
                    Player.Inventory[InventorySlot.Lightsource].ItemSubCategory < LightType.Thrain &&
                    !Player.HasLightResistance)
                {
                    oPtr = Player.Inventory[InventorySlot.Lightsource];
                    string oName = oPtr.Description(false, 0);
                    Profile.Instance.MsgPrint($"The {oName} scorches your undead flesh!");
                    caveNoRegen = true;
                    oName = oPtr.Description(true, 0);
                    string ouch = $"wielding {oName}";
                    if (Player.TimedInvulnerability == 0)
                    {
                        Player.TakeHit(1, ouch);
                    }
                }
            }
            if (!Level.GridPassable(Player.MapY, Player.MapX))
            {
                caveNoRegen = true;
                if (Player.TimedInvulnerability == 0 && Player.TimedEtherealness == 0 &&
                    (Player.Health > Player.Level / 5 || Player.RaceIndex != RaceId.Spectre))
                {
                    string damDesc;
                    if (Player.RaceIndex == RaceId.Spectre)
                    {
                        Profile.Instance.MsgPrint("Your body feels disrupted!");
                        damDesc = "density";
                    }
                    else
                    {
                        Profile.Instance.MsgPrint("You are being crushed!");
                        damDesc = "solid rock";
                    }
                    Player.TakeHit(1 + (Player.Level / 5), damDesc);
                }
            }
            int i;
            if (Player.TimedBleeding != 0 && Player.TimedInvulnerability == 0)
            {
                if (Player.TimedBleeding > 200)
                {
                    i = 3;
                }
                else if (Player.TimedBleeding > 100)
                {
                    i = 2;
                }
                else
                {
                    i = 1;
                }
                Player.TakeHit(i, "a fatal wound");
            }
            if (Player.Food < Constants.PyFoodMax)
            {
                if (Player.GameTime.IsTurnHundred)
                {
                    i = GlobalData.ExtractEnergy[Player.Speed] * 2;
                    if (Player.HasRegeneration)
                    {
                        i += 30;
                    }
                    if (Player.HasSlowDigestion)
                    {
                        i -= 10;
                    }
                    if (i < 1)
                    {
                        i = 1;
                    }
                    Player.SetFood(Player.Food - i);
                }
            }
            else
            {
                Player.SetFood(Player.Food - 100);
            }
            if (Player.Food < Constants.PyFoodStarve)
            {
                i = (Constants.PyFoodStarve - Player.Food) / 10;
                if (Player.TimedInvulnerability == 0)
                {
                    Player.TakeHit(i, "starvation");
                }
            }
            int regenAmount = Constants.PyRegenNormal;
            if (Player.Food < Constants.PyFoodWeak)
            {
                if (Player.Food < Constants.PyFoodStarve)
                {
                    regenAmount = 0;
                }
                else if (Player.Food < Constants.PyFoodFaint)
                {
                    regenAmount = Constants.PyRegenFaint;
                }
                else
                {
                    regenAmount = Constants.PyRegenWeak;
                }
                if (Player.Food < Constants.PyFoodFaint)
                {
                    if (Player.TimedParalysis == 0 && Program.Rng.RandomLessThan(100) < 10)
                    {
                        Profile.Instance.MsgPrint("You faint from the lack of food.");
                        Disturb(true);
                        Player.SetTimedParalysis(Player.TimedParalysis + 1 + Program.Rng.RandomLessThan(5));
                    }
                }
            }
            if (Player.HasRegeneration)
            {
                regenAmount *= 2;
            }
            if (Player.IsSearching || Resting != 0)
            {
                regenAmount *= 2;
            }
            int upkeepFactor = 0;
            if (TotalFriends != 0)
            {
                int upkeepDivider = 20;
                if (Player.ProfessionIndex == CharacterClass.Mage)
                {
                    upkeepDivider = 15;
                }
                else if (Player.ProfessionIndex == CharacterClass.HighMage)
                {
                    upkeepDivider = 12;
                }
                if (TotalFriends > 1 + (Player.Level / upkeepDivider))
                {
                    upkeepFactor = TotalFriendLevels;
                    if (upkeepFactor > 100)
                    {
                        upkeepFactor = 100;
                    }
                    else if (upkeepFactor < 10)
                    {
                        upkeepFactor = 10;
                    }
                }
            }
            if (Player.Mana < Player.MaxMana)
            {
                if (upkeepFactor != 0)
                {
                    int upkeepRegen = (100 - upkeepFactor) * regenAmount / 100;
                    Player.RegenerateMana(upkeepRegen);
                }
                else
                {
                    Player.RegenerateMana(regenAmount);
                }
            }
            if (Player.TimedPoison != 0)
            {
                regenAmount = 0;
            }
            if (Player.TimedBleeding != 0)
            {
                regenAmount = 0;
            }
            if (caveNoRegen)
            {
                regenAmount = 0;
            }
            if (Player.Health < Player.MaxHealth && !caveNoRegen)
            {
                Player.RegenerateHealth(regenAmount);
            }
            if (Player.TimedHallucinations != 0)
            {
                Player.SetTimedHallucinations(Player.TimedHallucinations - 1);
            }
            if (Player.TimedBlindness != 0)
            {
                Player.SetTimedBlindness(Player.TimedBlindness - 1);
            }
            if (Player.TimedSeeInvisibility != 0)
            {
                Player.SetTimedSeeInvisibility(Player.TimedSeeInvisibility - 1);
            }
            if (Player.GooPatron.MultiRew)
            {
                Player.GooPatron.MultiRew = false;
            }
            if (Player.TimedTelepathy != 0)
            {
                Player.SetTimedTelepathy(Player.TimedTelepathy - 1);
            }
            if (Player.TimedInfravision != 0)
            {
                Player.SetTimedInfravision(Player.TimedInfravision - 1);
            }
            if (Player.TimedParalysis != 0)
            {
                Player.SetTimedParalysis(Player.TimedParalysis - 1);
            }
            if (Player.TimedConfusion != 0)
            {
                Player.SetTimedConfusion(Player.TimedConfusion - 1);
            }
            if (Player.TimedFear != 0)
            {
                Player.SetTimedFear(Player.TimedFear - 1);
            }
            if (Player.TimedHaste != 0)
            {
                Player.SetTimedHaste(Player.TimedHaste - 1);
            }
            if (Player.TimedSlow != 0)
            {
                Player.SetTimedSlow(Player.TimedSlow - 1);
            }
            if (Player.TimedProtectionFromEvil != 0)
            {
                Player.SetTimedProtectionFromEvil(Player.TimedProtectionFromEvil - 1);
            }
            if (Player.TimedInvulnerability != 0)
            {
                Player.SetTimedInvulnerability(Player.TimedInvulnerability - 1);
            }
            if (Player.TimedEtherealness != 0)
            {
                Player.SetTimedEtherealness(Player.TimedEtherealness - 1);
            }
            if (Player.TimedHeroism != 0)
            {
                Player.SetTimedHeroism(Player.TimedHeroism - 1);
            }
            if (Player.TimedSuperheroism != 0)
            {
                Player.SetTimedSuperheroism(Player.TimedSuperheroism - 1);
            }
            if (Player.TimedBlessing != 0)
            {
                Player.SetTimedBlessing(Player.TimedBlessing - 1);
            }
            if (Player.TimedStoneskin != 0)
            {
                Player.SetTimedStoneskin(Player.TimedStoneskin - 1);
            }
            if (Player.TimedAcidResistance != 0)
            {
                Player.SetTimedAcidResistance(Player.TimedAcidResistance - 1);
            }
            if (Player.TimedLightningResistance != 0)
            {
                Player.SetTimedLightningResistance(Player.TimedLightningResistance - 1);
            }
            if (Player.TimedFireResistance != 0)
            {
                Player.SetTimedFireResistance(Player.TimedFireResistance - 1);
            }
            if (Player.TimedColdResistance != 0)
            {
                Player.SetTimedColdResistance(Player.TimedColdResistance - 1);
            }
            if (Player.TimedPoisonResistance != 0)
            {
                Player.SetTimedPoisonResistance(Player.TimedPoisonResistance - 1);
            }
            if (Player.TimedPoison != 0)
            {
                int adjust = Player.AbilityScores[Ability.Constitution].ConRecoverySpeed + 1;
                Player.SetTimedPoison(Player.TimedPoison - adjust);
            }
            if (Player.TimedStun != 0)
            {
                int adjust = Player.AbilityScores[Ability.Constitution].ConRecoverySpeed + 1;
                Player.SetTimedStun(Player.TimedStun - adjust);
            }
            if (Player.TimedBleeding != 0)
            {
                int adjust = Player.AbilityScores[Ability.Constitution].ConRecoverySpeed + 1;
                if (Player.TimedBleeding > 1000)
                {
                    adjust = 0;
                }
                Player.SetTimedBleeding(Player.TimedBleeding - adjust);
            }
            oPtr = Player.Inventory[InventorySlot.Lightsource];
            if (oPtr.Category == ItemCategory.Light)
            {
                if ((oPtr.ItemSubCategory == LightType.Torch || oPtr.ItemSubCategory == LightType.Lantern) &&
                    oPtr.TypeSpecificValue > 0)
                {
                    oPtr.TypeSpecificValue--;
                    if (Player.TimedBlindness != 0)
                    {
                        if (oPtr.TypeSpecificValue == 0)
                        {
                            oPtr.TypeSpecificValue++;
                        }
                    }
                    else if (oPtr.TypeSpecificValue == 0)
                    {
                        Disturb(true);
                        Profile.Instance.MsgPrint("Your light has gone out!");
                    }
                    else if (oPtr.TypeSpecificValue < 100 && oPtr.TypeSpecificValue % 10 == 0)
                    {
                        Profile.Instance.MsgPrint("Your light is growing faint.");
                    }
                }
            }
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateTorchRadius);
            if (Player.HasExperienceDrain)
            {
                if (Program.Rng.RandomLessThan(100) < 10 && Player.ExperiencePoints > 0)
                {
                    Player.ExperiencePoints--;
                    Player.MaxExperienceGained--;
                    Player.CheckExperience();
                }
            }
            if (Program.Rng.DieRoll(999) == 1 && !Player.HasAntiMagic)
            {
                if (Player.Inventory[InventorySlot.Lightsource].Category != 0 && Player.TimedInvulnerability == 0 &&
                    Player.Inventory[InventorySlot.Lightsource].ItemSubCategory == LightType.Thrain)
                {
                    Profile.Instance.MsgPrint("The Jewel of Judgement drains life from you!");
                    Player.TakeHit(Math.Min(Player.Level, 50), "the Jewel of Judgement");
                }
            }
            int j;
            for (j = 0, i = InventorySlot.MeleeWeapon; i < InventorySlot.Total; i++)
            {
                oPtr = Player.Inventory[i];
                oPtr.GetMergedFlags(f1, f2, f3);
                if (f3.IsSet(ItemFlag3.DreadCurse) && Program.Rng.DieRoll(100) == 1)
                {
                    ActivateDreadCurse();
                }
                if (f3.IsSet(ItemFlag3.Teleport) && Program.Rng.RandomLessThan(100) < 1)
                {
                    if (oPtr.IdentifyFlags.IsSet(Constants.IdentCursed) && !Player.HasAntiTeleport)
                    {
                        Disturb(true);
                        SpellEffects.TeleportPlayer(40);
                    }
                    else
                    {
                        if (Gui.GetCheck("Teleport? "))
                        {
                            Disturb(false);
                            SpellEffects.TeleportPlayer(50);
                        }
                    }
                }
                Player.Dna.OnProcessWorld(this, Player, Level);
                if (oPtr.ItemType == null)
                {
                    continue;
                }
                if (oPtr.RechargeTimeLeft > 0)
                {
                    oPtr.RechargeTimeLeft--;
                    if (oPtr.RechargeTimeLeft == 0)
                    {
                        j++;
                    }
                }
            }
            for (j = 0, i = 0; i < InventorySlot.Pack; i++)
            {
                oPtr = Player.Inventory[i];
                if (oPtr.ItemType == null)
                {
                    continue;
                }
                if (oPtr.Category == ItemCategory.Rod && oPtr.TypeSpecificValue != 0)
                {
                    oPtr.TypeSpecificValue--;
                    if (oPtr.TypeSpecificValue == 0)
                    {
                        j++;
                    }
                }
            }
            if (j != 0)
            {
                Player.NoticeFlags |= Constants.PnCombine;
            }
            Player.SenseInventory();
            for (i = 1; i < Level.OMax; i++)
            {
                oPtr = Level.Items[i];
                if (oPtr.ItemType == null)
                {
                    continue;
                }
                if (oPtr.Category == ItemCategory.Rod && oPtr.TypeSpecificValue != 0)
                {
                    oPtr.TypeSpecificValue--;
                }
            }
            if (Player.WordOfRecallDelay != 0)
            {
                Player.WordOfRecallDelay--;
                if (Player.WordOfRecallDelay == 0)
                {
                    Disturb(false);
                    if (CurrentDepth != 0)
                    {
                        Profile.Instance.MsgPrint(CurDungeon.Tower
                            ? "You feel yourself yanked downwards!"
                            : "You feel yourself yanked upwards!");
                        IsAutosave = true;
                        DoCmdSaveGame();
                        IsAutosave = false;
                        RecallDungeon = CurDungeon.Index;
                        CurrentDepth = 0;
                        if (Player.TownWithHouse > -1)
                        {
                            CurTown = Towns[Player.TownWithHouse];
                            Player.WildernessX = CurTown.X;
                            Player.WildernessY = CurTown.Y;
                            NewLevelFlag = true;
                            CameFrom = LevelStart.StartHouse;
                        }
                        else
                        {
                            Player.WildernessX = CurTown.X;
                            Player.WildernessY = CurTown.Y;
                            NewLevelFlag = true;
                            CameFrom = LevelStart.StartRandom;
                        }
                    }
                    else
                    {
                        Profile.Instance.MsgPrint(Dungeons[RecallDungeon].Tower
                            ? "You feel yourself yanked upwards!"
                            : "You feel yourself yanked downwards!");
                        IsAutosave = true;
                        DoCmdSaveGame();
                        IsAutosave = false;
                        CurDungeon = Dungeons[RecallDungeon];
                        Player.WildernessX = CurDungeon.X;
                        Player.WildernessY = CurDungeon.Y;
                        CurrentDepth = Player.MaxDlv[CurDungeon.Index];
                        if (CurrentDepth < 1)
                        {
                            CurrentDepth = 1;
                        }
                        NewLevelFlag = true;
                    }
                    Gui.PlaySound(SoundEffect.TeleportLevel);
                }
            }
        }

        private void RedrawStuff()
        {
            if (Player.RedrawNeeded.IsClear())
            {
                return;
            }
            if (Player == null)
            {
                return;
            }
            if (Gui.FullScreenOverlay)
            {
                return;
            }
            PlayerStatus playerStatus = new PlayerStatus(Player, Level);
            if (Player.RedrawNeeded.IsSet(RedrawFlag.PrWipe))
            {
                Player.RedrawNeeded.Clear(RedrawFlag.PrWipe);
                Profile.Instance.MsgPrint(null);
                Gui.Clear();
            }
            if (Player.RedrawNeeded.IsSet(RedrawFlag.PrMap))
            {
                Player.RedrawNeeded.Clear(RedrawFlag.PrMap);
                Level.PrtMap();
            }
            if (Player.RedrawNeeded.IsSet(RedrawFlag.PrBasic))
            {
                Player.RedrawNeeded.Clear(RedrawFlag.PrBasic);
                Player.RedrawNeeded.Clear(RedrawFlag.PrMisc | RedrawFlag.PrTitle | RedrawFlag.PrStats);
                Player.RedrawNeeded.Clear(RedrawFlag.PrLev | RedrawFlag.PrExp | RedrawFlag.PrGold);
                Player.RedrawNeeded.Clear(RedrawFlag.PrArmor | RedrawFlag.PrHp | RedrawFlag.PrMana);
                Player.RedrawNeeded.Clear(RedrawFlag.PrDepth | RedrawFlag.PrHealth);
                playerStatus.PrtFrameBasic();
            }
            if (Player.RedrawNeeded.IsSet(RedrawFlag.PrEquippy))
            {
                Player.RedrawNeeded.Clear(RedrawFlag.PrEquippy);
                CharacterViewer.PrintEquippy(Player);
            }
            if (Player.RedrawNeeded.IsSet(RedrawFlag.PrMisc))
            {
                Player.RedrawNeeded.Clear(RedrawFlag.PrMisc);
                playerStatus.PrtField(Player.Race.Title, ScreenLocation.RowRace, ScreenLocation.ColRace);
                playerStatus.PrtField(Profession.ClassSubName(Player.ProfessionIndex, Player.Realm1), ScreenLocation.RowClass,
                    ScreenLocation.ColClass);
            }
            if (Player.RedrawNeeded.IsSet(RedrawFlag.PrTitle))
            {
                Player.RedrawNeeded.Clear(RedrawFlag.PrTitle);
                playerStatus.PrtTitle();
            }
            if (Player.RedrawNeeded.IsSet(RedrawFlag.PrLev))
            {
                Player.RedrawNeeded.Clear(RedrawFlag.PrLev);
                playerStatus.PrtLevel();
            }
            if (Player.RedrawNeeded.IsSet(RedrawFlag.PrExp))
            {
                Player.RedrawNeeded.Clear(RedrawFlag.PrExp);
                playerStatus.PrtExp();
            }
            if (Player.RedrawNeeded.IsSet(RedrawFlag.PrStats))
            {
                Player.RedrawNeeded.Clear(RedrawFlag.PrStats);
                playerStatus.PrtStat(Ability.Strength);
                playerStatus.PrtStat(Ability.Intelligence);
                playerStatus.PrtStat(Ability.Wisdom);
                playerStatus.PrtStat(Ability.Dexterity);
                playerStatus.PrtStat(Ability.Constitution);
                playerStatus.PrtStat(Ability.Charisma);
            }
            if (Player.RedrawNeeded.IsSet(RedrawFlag.PrArmor))
            {
                Player.RedrawNeeded.Clear(RedrawFlag.PrArmor);
                playerStatus.PrtAc();
            }
            if (Player.RedrawNeeded.IsSet(RedrawFlag.PrHp))
            {
                Player.RedrawNeeded.Clear(RedrawFlag.PrHp);
                playerStatus.PrtHp();
            }
            if (Player.RedrawNeeded.IsSet(RedrawFlag.PrMana))
            {
                Player.RedrawNeeded.Clear(RedrawFlag.PrMana);
                playerStatus.PrtSp();
            }
            if (Player.RedrawNeeded.IsSet(RedrawFlag.PrGold))
            {
                Player.RedrawNeeded.Clear(RedrawFlag.PrGold);
                playerStatus.PrtGold();
            }
            if (Player.RedrawNeeded.IsSet(RedrawFlag.PrDepth))
            {
                Player.RedrawNeeded.Clear(RedrawFlag.PrDepth);
                playerStatus.PrtDepth();
            }
            if (Player.RedrawNeeded.IsSet(RedrawFlag.PrHealth))
            {
                Player.RedrawNeeded.Clear(RedrawFlag.PrHealth);
                playerStatus.HealthRedraw();
            }
            if (Player.RedrawNeeded.IsSet(RedrawFlag.PrExtra))
            {
                Player.RedrawNeeded.Clear(RedrawFlag.PrExtra);
                Player.RedrawNeeded.Clear(RedrawFlag.PrCut | RedrawFlag.PrStun);
                Player.RedrawNeeded.Clear(RedrawFlag.PrHunger | RedrawFlag.PrDtrap);
                Player.RedrawNeeded.Clear(RedrawFlag.PrBlind | RedrawFlag.PrConfused);
                Player.RedrawNeeded.Clear(RedrawFlag.PrAfraid | RedrawFlag.PrPoisoned);
                Player.RedrawNeeded.Clear(RedrawFlag.PrState | RedrawFlag.PrSpeed | RedrawFlag.PrStudy);
                playerStatus.PrtFrameExtra();
            }
            if (Player.RedrawNeeded.IsSet(RedrawFlag.PrCut))
            {
                Player.RedrawNeeded.Clear(RedrawFlag.PrCut);
                playerStatus.PrtCut();
            }
            if (Player.RedrawNeeded.IsSet(RedrawFlag.PrStun))
            {
                Player.RedrawNeeded.Clear(RedrawFlag.PrStun);
                playerStatus.PrtStun();
            }
            if (Player.RedrawNeeded.IsSet(RedrawFlag.PrHunger))
            {
                Player.RedrawNeeded.Clear(RedrawFlag.PrHunger);
                playerStatus.PrtHunger();
            }
            if (Player.RedrawNeeded.IsSet(RedrawFlag.PrDtrap))
            {
                Player.RedrawNeeded.Clear(RedrawFlag.PrDtrap);
                playerStatus.PrtDtrap();
            }
            if (Player.RedrawNeeded.IsSet(RedrawFlag.PrBlind))
            {
                Player.RedrawNeeded.Clear(RedrawFlag.PrBlind);
                playerStatus.PrtBlind();
            }
            if (Player.RedrawNeeded.IsSet(RedrawFlag.PrConfused))
            {
                Player.RedrawNeeded.Clear(RedrawFlag.PrConfused);
                playerStatus.PrtConfused();
            }
            if (Player.RedrawNeeded.IsSet(RedrawFlag.PrAfraid))
            {
                Player.RedrawNeeded.Clear(RedrawFlag.PrAfraid);
                playerStatus.PrtAfraid();
            }
            if (Player.RedrawNeeded.IsSet(RedrawFlag.PrPoisoned))
            {
                Player.RedrawNeeded.Clear(RedrawFlag.PrPoisoned);
                playerStatus.PrtPoisoned();
            }
            if (Player.RedrawNeeded.IsSet(RedrawFlag.PrState))
            {
                Player.RedrawNeeded.Clear(RedrawFlag.PrState);
                playerStatus.PrtState();
            }
            if (Player.RedrawNeeded.IsSet(RedrawFlag.PrSpeed))
            {
                Player.RedrawNeeded.Clear(RedrawFlag.PrSpeed);
                playerStatus.PrtSpeed();
            }
            if (Player.RedrawNeeded.IsSet(RedrawFlag.PrStudy))
            {
                Player.RedrawNeeded.Clear(RedrawFlag.PrStudy);
                playerStatus.PrtStudy();
            }
            playerStatus.PrtTime();
        }

        private void RegenMonsters()
        {
            for (int i = 1; i < Level.MMax; i++)
            {
                Monster mPtr = Level.Monsters[i];
                MonsterRace rPtr = mPtr.Race;
                if (mPtr.Race == null)
                {
                    continue;
                }
                if (mPtr.Health < mPtr.MaxHealth)
                {
                    int frac = mPtr.MaxHealth / 100;
                    if (frac == 0)
                    {
                        frac = 1;
                    }
                    if ((rPtr.Flags2 & MonsterFlag2.Regenerate) != 0)
                    {
                        frac *= 2;
                    }
                    mPtr.Health += frac;
                    if (mPtr.Health > mPtr.MaxHealth)
                    {
                        mPtr.Health = mPtr.MaxHealth;
                    }
                    if (TrackedMonsterIndex == i)
                    {
                        Player.RedrawNeeded.Set(RedrawFlag.PrHealth);
                    }
                }
            }
        }

        private void SavePlayer()
        {
            Program.SerializeToSaveFolder(Profile.Instance, Program.ActiveSaveSlot);
        }

        private void UpdateCommand()
        {
            Command.Player = Player;
            Command.Level = Level;
        }

        private bool Verify(string prompt, int item)
        {
            Item oPtr = item >= 0 ? Player.Inventory[item] : Level.Items[0 - item];
            string oName = oPtr.Description(true, 3);
            string outVal = $"{prompt} {oName}? ";
            return Gui.GetCheck(outVal);
        }
    }
}