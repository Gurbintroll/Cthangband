using System;
using System.Reflection;

namespace Cthangband.StaticData
{
    internal static class Constants
    {
        public const int ActivationChance = 3;
        public const int ArifactCurseChance = 13;
        public const int BreakElderSign = 550;
        public const int BreakYellowSign = 99;
        public const int BthPlusAdj = 3;
        public const int ConsoleHeight = 45;
        public const int ConsoleWidth = 80;
        public const int DrsAcid = 1;
        public const int DrsBlind = 13;
        public const int DrsChaos = 11;
        public const int DrsCold = 4;
        public const int DrsConf = 10;
        public const int DrsDark = 8;
        public const int DrsDisen = 12;
        public const int DrsElec = 2;
        public const int DrsFear = 9;
        public const int DrsFire = 3;
        public const int DrsFree = 30;
        public const int DrsLight = 7;
        public const int DrsMana = 31;
        public const int DrsNeth = 6;
        public const int DrsNexus = 14;
        public const int DrsPois = 5;
        public const int DrsReflect = 32;
        public const int DrsShard = 16;
        public const int DrsSound = 15;
        public const int EnchToac = 0x04;
        public const int EnchTodam = 0x02;
        public const int EnchTohit = 0x01;
        public const int FollowDistance = 4;
        public const int FuelLamp = 15000;
        public const int FuelTorch = 5000;
        public const int GenerateChoose = 0;
        public const int GenerateRandom = 1;
        public const int GenerateReplay = 2;
        public const int GreatObj = 20;
        public const int GroupMax = 32;
        public const int IdentBroken = 0x80;
        public const int IdentCursed = 0x40;
        public const int IdentEmpty = 0x04;
        public const int IdentFixed = 0x02;
        public const int IdentKnown = 0x08;
        public const int IdentMental = 0x20;
        public const int IdentSense = 0x01;
        public const int IdentStoreb = 0x10;
        public const int KeymapModeOrig = 0;
        public const int KeymapModeRogue = 1;
        public const int KeymapModes = 2;
        public const int LightMax = 128;
        public const int MaKnee = 1;
        public const int MaSlow = 2;
        public const int MaxAmulets = 17;
        public const int MaxCaves = 20;
        public const int MaxClass = 16;
        public const int MaxColors = 66;
        public const int MaxComment = 5;
        public const int MaxDepth = 128;
        public const int MaxFunny = 22;
        public const int MaxGenders = 3;
        public const int MaxGold = 18;
        public const int MaxHorror = 20;
        public const int MaxMa = 17;
        public const int MaxMAllocChance = 160;
        public const int MaxMetals = 39;
        public const int MaxMIdx = 512;
        public const int MaxMindcraftPowers = 12;
        public const int MaxOIdx = 256;
        public const int MaxOwners = 4;
        public const int MaxPatron = 16;
        public const int MaxRaces = 30;
        public const int MaxRange = 18;
        public const int MaxRealm = 8;
        public const int MaxRepro = 100;
        public const int MaxRocks = 56;
        public const int MaxShort = 32767;
        public const int MaxShroom = 20;
        public const int MaxSight = 20;
        public const int MaxStackSize = 100;
        public const int MaxStoresTotal = 96;
        public const int MaxTitles = 54;
        public const int MaxUchar = 255;
        public const int MaxWoods = 32;
        public const int MflagBorn = 0x10;
        public const int MflagMark = 0x80;
        public const int MflagNice = 0x20;
        public const int MflagShow = 0x40;
        public const int MflagView = 0x01;
        public const int MinMAllocLevel = 14;
        public const int MinMAllocTd = 4;
        public const int MinMAllocTn = 8;
        public const int MonDrainLife = 2;
        public const int MonMultAdj = 8;
        public const int MonsterFlowDepth = 32;
        public const int MonSummonAdj = 2;
        public const int NastyMon = 50;
        public const int ObjGoldList = 480;
        public const int PenetrateInvulnerability = 13;
        public const uint PnCombine = 0x00000001;
        public const uint PnReorder = 0x00000002;
        public const uint PuBonus = 0x00000001;
        public const uint PuDistance = 0x02000000;
        public const uint PuFlow = 0x10000000;
        public const uint PuHp = 0x00000010;
        public const uint PuLight = 0x00200000;
        public const uint PuMana = 0x00000020;
        public const uint PuMonsters = 0x01000000;
        public const uint PuSpells = 0x00000040;
        public const uint PuTorch = 0x00000002;
        public const uint PuUnLight = 0x00020000;
        public const uint PuUnView = 0x00010000;
        public const uint PuView = 0x00100000;
        public const int PyFoodAlert = 2000;
        public const int PyFoodFaint = 500;
        public const int PyFoodFull = 10000;
        public const int PyFoodMax = 15000;
        public const int PyFoodStarve = 100;
        public const int PyFoodWeak = 1000;
        public const int PyMaxExp = 99999999;
        public const int PyMaxGold = 999999999;
        public const int PyMaxLevel = 50;
        public const int PyRegenFaint = 33;
        public const int PyRegenHpbase = 1442;
        public const int PyRegenMnbase = 524;
        public const int PyRegenNormal = 197;
        public const int PyRegenWeak = 98;
        public const int RoadDown = 0x04;
        public const int RoadLeft = 0x01;
        public const int RoadRight = 0x08;
        public const int RoadUp = 0x02;
        public const int ScreenHgt = 42;
        public const int ScreenWid = 66;
        public const int SexFemale = 0;
        public const int SexMale = 1;
        public const uint SmCloned = 0x00400000;
        public const uint SmFriendly = 0x00800000;
        public const uint SmImmAcid = 0x01000000;
        public const uint SmImmCold = 0x08000000;
        public const uint SmImmElec = 0x02000000;
        public const uint SmImmFire = 0x04000000;
        public const uint SmImmFree = 0x40000000;
        public const uint SmImmMana = 0x80000000;
        public const uint SmImmReflect = 0x20000000;
        public const uint SmImmXxx5 = 0x10000000;
        public const uint SmOppAcid = 0x00010000;
        public const uint SmOppCold = 0x00080000;
        public const uint SmOppElec = 0x00020000;
        public const uint SmOppFire = 0x00040000;
        public const uint SmOppPois = 0x00100000;
        public const uint SmOppXXx1 = 0x00200000;
        public const uint SmResAcid = 0x00000001;
        public const uint SmResBlind = 0x00001000;
        public const uint SmResChaos = 0x00000400;
        public const uint SmResCold = 0x00000008;
        public const uint SmResConf = 0x00000200;
        public const uint SmResDark = 0x00000080;
        public const uint SmResDisen = 0x00000800;
        public const uint SmResElec = 0x00000002;
        public const uint SmResFear = 0x00000100;
        public const uint SmResFire = 0x00000004;
        public const uint SmResLight = 0x00000040;
        public const uint SmResNeth = 0x00000020;
        public const uint SmResNexus = 0x00002000;
        public const uint SmResPois = 0x00000010;
        public const uint SmResShard = 0x00008000;
        public const uint SmResSound = 0x00004000;
        public const int StoreChoices = 48;
        public const int StoreInvenMax = 26;
        public const int StoreMaxKeep = 18;
        public const int StoreMinKeep = 6;
        public const int StoreObjLevel = 5;
        public const int StoreShuffle = 21;
        public const int StoreTurnover = 9;
        public const int StoreTurns = 1000;
        public const int SummonAnimal = 42;
        public const int SummonAnimalRanger = 43;
        public const int SummonAnt = 11;
        public const int SummonAvatar = 41;
        public const int SummonBizarre1 = 33;
        public const int SummonBizarre2 = 34;
        public const int SummonBizarre3 = 35;
        public const int SummonBizarre4 = 36;
        public const int SummonBizarre5 = 37;
        public const int SummonBizarre6 = 38;
        public const int SummonCthuloid = 15;
        public const int SummonDemon = 16;
        public const int SummonDragon = 18;
        public const int SummonElemental = 48;
        public const int SummonGoo = 31;
        public const int SummonHiDragon = 22;
        public const int SummonHiDragonNoUniques = 45;
        public const int SummonHiUndead = 21;
        public const int SummonHiUndeadNoUniques = 44;
        public const int SummonHound = 13;
        public const int SummonHuman = 51;
        public const int SummonHydra = 14;
        public const int SummonKin = 40;
        public const int SummonKobold = 52;
        public const int SummonNoUniques = 46;
        public const int SummonOrc = 49;
        public const int SummonPhantom = 47;
        public const int SummonReaver = 39;
        public const int SummonSpider = 12;
        public const int SummonUndead = 17;
        public const int SummonUnique = 32;
        public const int SummonYeek = 50;
        public const int TableName = 45;
        public const int TargetGrid = 0x08;
        public const int TargetKill = 0x01;
        public const int TargetLook = 0x02;
        public const int TargetXtra = 0x04;
        public const int TempMax = 1536;
        public const int TextACursedSize = 50;
        public const int TextAHighSize = 66;
        public const int TextALowSize = 85;
        public const int TextAMedSize = 84;
        public const int TextElvishSize = 216;
        public const int TextWCursedSize = 39;
        public const int TextWHighSize = 87;
        public const int TextWLowSize = 98;
        public const int TextWMedSize = 106;
        public const int TurnsInADay = 108000;
        public const int TurnsInAHalfDay = (TurnsInADay / 2);
        public const int UseDevice = 3;
        public const int ViewMax = 1536;
        public const int WeirdLuck = 12;
        public const int WildernessHeight = 44;
        public const int WildernessWidth = 66;

        public static readonly DateTime CompileTime = new DateTime(2000, 1, 1)
            .AddDays(Assembly.GetExecutingAssembly().GetName().Version.Build)
            .AddSeconds(Assembly.GetExecutingAssembly().GetName().Version.Revision * 2);

        public static readonly int VersionMajor = Assembly.GetExecutingAssembly().GetName().Version.Major;
        public static readonly int VersionMinor = Assembly.GetExecutingAssembly().GetName().Version.Minor;
        public static readonly string VersionName = Assembly.GetExecutingAssembly().GetName().Name;
        public static readonly string VersionStamp = $"Version {VersionMajor}.{VersionMinor}";
    }
}