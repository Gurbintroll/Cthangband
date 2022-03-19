using Cthangband.Enumerations;
using System.Collections.Generic;

namespace Cthangband.StaticData
{
    internal static class GlobalData
    {
        public const int DelayFactor = 4;
        public const int HitpointWarn = 2;

        /// <summary>
        /// Spell flags for each book
        /// </summary>
        public static readonly uint[] BookSpellFlags = { 0x000000ff, 0x0000ff00, 0x00ff0000, 0xff000000 };

        public static readonly int[] ChestTraps =
                {
            0, ChestTrap.ChestPoison, ChestTrap.ChestLoseStr, ChestTrap.ChestLoseCon, ChestTrap.ChestLoseStr,
            ChestTrap.ChestLoseCon, 0, ChestTrap.ChestPoison, ChestTrap.ChestPoison, ChestTrap.ChestLoseStr,
            ChestTrap.ChestLoseCon, ChestTrap.ChestPoison, ChestTrap.ChestLoseStr | ChestTrap.ChestLoseCon,
            ChestTrap.ChestLoseStr | ChestTrap.ChestLoseCon, ChestTrap.ChestLoseStr | ChestTrap.ChestLoseCon,
            ChestTrap.ChestSummon, 0, ChestTrap.ChestLoseStr, ChestTrap.ChestLoseCon, ChestTrap.ChestParalyze,
            ChestTrap.ChestLoseStr | ChestTrap.ChestLoseCon, ChestTrap.ChestSummon, ChestTrap.ChestParalyze,
            ChestTrap.ChestLoseStr, ChestTrap.ChestLoseCon, ChestTrap.ChestExplode, 0,
            ChestTrap.ChestPoison | ChestTrap.ChestLoseStr, ChestTrap.ChestPoison | ChestTrap.ChestLoseCon,
            ChestTrap.ChestLoseStr | ChestTrap.ChestLoseCon, ChestTrap.ChestExplode | ChestTrap.ChestSummon,
            ChestTrap.ChestParalyze, ChestTrap.ChestPoison | ChestTrap.ChestSummon, ChestTrap.ChestSummon,
            ChestTrap.ChestExplode, ChestTrap.ChestExplode | ChestTrap.ChestSummon, 0, ChestTrap.ChestSummon,
            ChestTrap.ChestExplode, ChestTrap.ChestExplode | ChestTrap.ChestSummon,
            ChestTrap.ChestExplode | ChestTrap.ChestSummon, ChestTrap.ChestPoison | ChestTrap.ChestParalyze,
            ChestTrap.ChestExplode, ChestTrap.ChestExplode | ChestTrap.ChestSummon,
            ChestTrap.ChestExplode | ChestTrap.ChestSummon, ChestTrap.ChestPoison | ChestTrap.ChestParalyze, 0,
            ChestTrap.ChestLoseStr | ChestTrap.ChestLoseCon, ChestTrap.ChestLoseStr | ChestTrap.ChestLoseCon,
            ChestTrap.ChestPoison | ChestTrap.ChestParalyze | ChestTrap.ChestLoseStr,
            ChestTrap.ChestPoison | ChestTrap.ChestParalyze | ChestTrap.ChestLoseCon,
            ChestTrap.ChestPoison | ChestTrap.ChestLoseStr | ChestTrap.ChestLoseCon,
            ChestTrap.ChestPoison | ChestTrap.ChestLoseStr | ChestTrap.ChestLoseCon,
            ChestTrap.ChestPoison | ChestTrap.ChestParalyze | ChestTrap.ChestLoseStr | ChestTrap.ChestLoseCon,
            ChestTrap.ChestPoison | ChestTrap.ChestParalyze, ChestTrap.ChestPoison | ChestTrap.ChestParalyze,
            ChestTrap.ChestExplode | ChestTrap.ChestSummon, ChestTrap.ChestExplode | ChestTrap.ChestSummon,
            ChestTrap.ChestExplode | ChestTrap.ChestSummon, ChestTrap.ChestExplode | ChestTrap.ChestSummon,
            ChestTrap.ChestExplode | ChestTrap.ChestSummon, ChestTrap.ChestExplode | ChestTrap.ChestSummon,
            ChestTrap.ChestExplode | ChestTrap.ChestSummon, ChestTrap.ChestExplode | ChestTrap.ChestSummon
        };

        public static readonly string[] DangerFeelingText =
        {
            "You're not sure about this level yet", "You feel there is something special about this level.",
            "You nearly faint as horrible visions of death fill your mind", "This level looks very dangerous",
            "You have a very bad feeling", "You have a bad feeling", "You feel nervous",
            "You feel unsafe", "You don't like the look of this place",
            "This level looks reasonably safe", "What a boring place"
        };

        public static readonly string[] DescStatNeg = { "weak", "stupid", "naive", "clumsy", "sickly", "ugly" };
        public static readonly string[] DescStatPos = { "strong", "smart", "wise", "dextrous", "healthy", "cute" };

        public static readonly int[] EnchantTable =
            {0, 10, 50, 100, 200, 300, 400, 500, 650, 800, 950, 987, 993, 995, 998, 1000};

        public static readonly int[] ExtractEnergy =
        {
	/* Slow */     1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
	/* Slow */     1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
	/* Slow */     1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
	/* Slow */     1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
	/* Slow */     1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
	/* Slow */     1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
	/* S-50 */     1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
	/* S-40 */     2,  2,  2,  2,  2,  2,  2,  2,  2,  2,
	/* S-30 */     2,  2,  2,  2,  2,  2,  2,  3,  3,  3,
	/* S-20 */     3,  3,  3,  3,  3,  4,  4,  4,  4,  4,
	/* S-10 */     5,  5,  5,  5,  6,  6,  7,  7,  8,  9,
	/* Norm */    10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
	/* F+10 */    20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
	/* F+20 */    30, 31, 32, 33, 34, 35, 36, 36, 37, 37,
	/* F+30 */    38, 38, 39, 39, 40, 40, 40, 41, 41, 41,
	/* F+40 */    42, 42, 42, 43, 43, 43, 44, 44, 44, 44,
	/* F+50 */    45, 45, 45, 45, 45, 46, 46, 46, 46, 46,
	/* F+60 */    47, 47, 47, 47, 47, 48, 48, 48, 48, 48,
	/* F+70 */    49, 49, 49, 49, 49, 49, 49, 49, 49, 49,
	/* Fast */    49, 49, 49, 49, 49, 49, 49, 49, 49, 49
        };

        public static readonly string[] SymbolIdentification =
        {
            " :A dark grid", "!:A potion (or oil)", "\":An amulet (or necklace)", "#:A wall (or secret door)",
            "$:Treasure (gold or gems)", "%:A vein (magma or quartz)", "&:Entrance to Inn", "':An open door",
            "(:Soft armour", "):A shield", "*:A vein with treasure", "+:A closed door", ",:Food (or mushroom patch)",
            "-:A wand (or rod)", ".:Floor", "/:A polearm (Axe/Pike/etc)", "0:Entrance to Pawnbrokers",
            "1:Entrance to General Store", "2:Entrance to Armoury", "3:Entrance to Weaponsmith", "4:Entrance to Temple",
            "5:Entrance to Alchemy shop", "6:Entrance to Magic Stores", "7:Entrance to Black Market",
            "8:Entrance to Hall of Records", "9:Entrance to Bookstore", "::Rubble",
            ";:An Elder Sign / Yellow Sign", "<:An up staircase", "=:A ring", ">:A down staircase",
            "?:A scroll", "@:You (or the entrance to your home)", "A:Abomination", "B:Bird", "C:Canine",
            "D:Ancient Dragon/Wyrm", "E:Elemental", "F:Dragon Fly", "G:Ghost", "H:Hybrid", "I:Insect", "J:Snake",
            "K:Killer Beetle", "L:Lich", "M:Multi-Headed Reptile", "O:Ogre", "P:Giant Humanoid",
            "Q:Quylthulg (Pulsing Flesh Mound)", "R:Reptile/Amphibian", "S:Spider/Scorpion/Tick", "T:Troll",
            "U:Major Demon", "V:Vampire", "W:Wight/Wraith/etc", "X:Extradimensional Entity", "Y:Yeti", "Z:Zephyr Hound",
            "[:Hard armour", "\\:A hafted weapon (mace/whip/etc)", "]:Misc. armour", "^:A trap", "_:A staff", "a:Ant",
            "b:Bat", "c:Centipede", "d:Dragon", "e:Floating Eye", "f:Feline", "g:Golem", "h:Hobbit/Elf/Dwarf",
            "i:Icky Thing", "j:Jelly", "k:Kobold", "l:Louse", "m:Mold", "n:Naga", "o:Orc", "p:Person/Human",
            "q:Quadruped", "r:Rodent", "s:Skeleton", "t:Townsperson", "u:Minor Demon", "v:Vortex", "w:Worm/Worm-Mass",
            "x:Xorn/Xaren/etc", "y:Yeek", "z:Zombie/Mummy", "{:A missile (arrow/bolt/shot)", "|:An edged weapon (sword/dagger/etc)",
            "}:A launcher (bow/crossbow/sling)", "~:A tool (or miscellaneous item)", null
        };

        public static readonly MartialArtsAttack[] MaBlows =
        {
            new MartialArtsAttack("You punch {0}.", 1, 0, 1, 4, 0),
            new MartialArtsAttack("You kick {0}.", 2, 0, 1, 6, 0),
            new MartialArtsAttack("You strike {0}.", 3, 0, 1, 7, 0),
            new MartialArtsAttack("You hit {0} with your knee.", 5, 5, 2, 3, Constants.MaKnee),
            new MartialArtsAttack("You hit {0} with your elbow.", 7, 5, 1, 8, 0),
            new MartialArtsAttack("You butt {0}.", 9, 10, 2, 5, 0),
            new MartialArtsAttack("You kick {0}.", 11, 10, 3, 4, Constants.MaSlow),
            new MartialArtsAttack("You uppercut {0}.", 13, 12, 4, 4, 6),
            new MartialArtsAttack("You double-kick {0}.", 16, 15, 5, 4, 8),
            new MartialArtsAttack("You hit {0} with a Cat's Claw.", 20, 20, 5, 5, 0),
            new MartialArtsAttack("You hit {0} with a jump kick.", 25, 25, 5, 6, 10),
            new MartialArtsAttack("You hit {0} with an Eagle's Claw.", 29, 25, 6, 6, 0),
            new MartialArtsAttack("You hit {0} with a circle kick.", 33, 30, 6, 8, 10),
            new MartialArtsAttack("You hit {0} with an Iron Fist.", 37, 35, 8, 8, 10),
            new MartialArtsAttack("You hit {0} with a flying kick.", 41, 35, 8, 10, 12),
            new MartialArtsAttack("You hit {0} with a Dragon Fist.", 45, 35, 10, 10, 16),
            new MartialArtsAttack("You hit {0} with a Crushing Blow.", 48, 35, 10, 12, 18)
        };

        public static readonly Dictionary<int, string> NumberRomanDictionary = new Dictionary<int, string>
        {
            { 1000, "M" },
            { 900, "CM" },
            { 500, "D" },
            { 400, "CD" },
            { 100, "C" },
            { 90, "XC" },
            { 50, "L" },
            { 40, "XL" },
            { 10, "X" },
            { 9, "IX" },
            { 5, "V" },
            { 4, "IV" },
            { 1, "I" },
        };

        public static readonly string[] ObjectFlagNames =
        {
            "Add Str", "Add Int", "Add Wis", "Add Dex", "Add Con", "Add Cha", null, null, "Add Stea.", "Add Sear.",
            "Add Infra", "Add Tun..", "Add Speed", "Add Blows", "Chaotic", "Vampiric", "Slay Anim.", "Slay Evil",
            "Slay Und.", "Slay Demon", "Slay Orc", "Slay Troll", "Slay Giant", "Slay Drag.", "Kill Drag.", "Sharpness",
            "Impact", "Poison Brd", "Acid Brand", "Elec Brand", "Fire Brand", "Cold Brand", "Sust Str", "Sust Int",
            "Sust Wis", "Sust Dex", "Sust Con", "Sust Cha", null, null, "Imm Acid", "Imm Elec", "Imm Fire", "Imm Cold",
            null, "Reflect", "Free Act", "Hold Life", "Res Acid", "Res Elec", "Res Fire", "Res Cold", "Res Pois",
            "Res Fear", "Res Light", "Res Dark", "Res Blind", "Res Conf", "Res Sound", "Res Shard", "Res Neth",
            "Res Nexus", "Res Chaos", "Res Disen", "Aura Fire", "Aura Elec", null, "Anti-Theft", "Anti-Tele", "Anti-Magic",
            "WraithForm", "EvilCurse", "Easy Know", "Hide Type", "Show Mods", "Insta Art", "Levitate", "Light",
            "See Invis", "Telepathy", "Digestion", "Regen", "Xtra Might", "Xtra Shots", "Ign Acid", "Ign Elec",
            "Ign Fire", "Ign Cold", "Activate", "Drain Exp", "Teleport", "Aggravate", "Blessed", "Cursed", "Hvy Curse",
            "Prm Curse"
        };

        public static readonly int[] PlayerExp =
        {
            10, 25, 45, 70, 100, 140, 200, 280, 380, 500, 650, 850, 1100, 1400, 1800, 2300, 2900, 3600, 4400, 5400,
            6800, 8400, 10200, 12500, 17500, 25000, 35000, 50000, 75000, 100000, 150000, 200000, 275000, 350000, 450000,
            550000, 700000, 850000, 1000000, 1250000, 1500000, 1800000, 2100000, 2400000, 2700000, 3000000, 3500000,
            4000000, 4500000, 5000000
        };

        public static readonly string[][] PlayerTitle =
        {
            new[]
            {
                "Rookie", "Soldier", "Mercenary", "Veteran", "Swordsman", "Champion", "Hero", "Baron", "Duke", "Lord"
            },
            new[]
            {
                "Apprentice", "Trickster", "Illusionist", "Spellbinder", "Evoker", "Conjurer", "Warlock", "Sorcerer",
                "Ipsissimus", "Archimage"
            },
            new[]
            {
                "Believer", "Acolyte", "Adept", "Curate", "Canon", "Priest", "High Priest", "Cardinal", "Inquisitor",
                "Pope"
            },
            new[]
            {
                "Sneak", "Cutpurse", "Robber", "Burglar", "Filcher", "Mountebank", "Low Thief", "High Thief",
                "Master Thief", "Guildmaster"
            },
            new[]
            {
                "Runner", "Strider", "Scout", "Courser", "Tracker", "Guide", "Pathfinder", "Low Ranger", "High Ranger",
                "Ranger Lord"
            },
            new[]
            {
                "Gallant", "Keeper", "Protector", "Defender", "Warder", "Knight", "Guardian", "Low Paladin",
                "High Paladin", "Paladin Lord"
            },
            new[]
            {
                "Novice", "Apprentice", "Journeyman", "Veteran", "Enchanter", "Champion", "Mage-Hero", "Baron Mage",
                "Battlemage", "Wizard Lord"
            },
            new[]
            {
                "Rookie", "Soldier", "Mercenary", "Veteran", "Swordsman", "Champion", "Chaos Hero", "Chaos Baron",
                "Chaos Duke", "Chaos Lord"
            },
            new[]
            {
                "Initiate", "Brother", "Disciple", "Immaculate", "Master", "Soft Master", "Hard Master",
                "Flower Master", "Dragon Master", "Grand Master"
            },
            new[]
            {
                "Trainee", "Acolyte", "Adept", "Immaculate", "Contemplator", "Mentalist", "Psychic", "Psionicist",
                "Esper", "Mindmaster"
            },
            new[]
            {
                "Apprentice", "Trickster", "Illusionist", "Spellbinder", "Evoker", "Conjurer", "Warlock", "Sorcerer",
                "Ipsissimus", "Archimage"
            },
            new[]
            {
                "Neophyte", "Initiate", "Adept", "Lesser Druid", "Druid", "Silver Druid", "Golden Druid", "Great Druid",
                "Arch-Druid", "Grand Druid"
            },
            new[]
            {
                "Apprentice", "Trickster", "Illusionist", "Spellbinder", "Entropist", "Chaotic", "Randomiser",
                "Chaos-Mage", "Chaos-Archmage", "Chaos Master"
            },
            new[]
            {
                "Apprentice", "Journeyman", "Guilder", "Alchemist", "Wandsmith", "Rodsmith", "Stavemaster", "Artificer",
                "Master Channeler", "Grand Channeler"
            },
            new[]
            {
                "Favoured", "Martyr", "Trusted One", "Celebrity", "Diva", "Champion", "Holy One", "Paragon", "Idol",
                "Saint"
            },
            new[]
            {
                "Initiate", "Brother", "Disciple", "Immaculate", "Master", "Soft Master", "Hard Master",
                "Flower Master", "Dragon Master", "Grand Master"
            }
        };

        public static readonly string[] ReportMagicDurations =
        {
            "for a short time", "for a little while", "for a while", "for a long while", "for a long time",
            "for a very long time", "for an incredibly long time", "until you hit a monster"
        };

        public static readonly string[] StatNames = { "STR: ", "INT: ", "WIS: ", "DEX: ", "CON: ", "CHA: " };

        public static readonly string[] StatNamesReduced = { "str: ", "int: ", "wis: ", "dex: ", "con: ", "cha: " };

        public static readonly string[] TextElvish =
{
            "adan", "ael", "in", "agl", "ar", "aina", "alda", "al", "qua", "am", "arth", "amon", "anca", "an", "dune",
            "anga", "anna", "ann", "on", "ar", "ien", "atar", "band", "bar", "ad", "bel", "eg", "brag", "ol", "breth",
            "il", "brith", "cal", "en", "gal", "en", "cam", "car", "ak", "cel", "eb", "cor", "on", "cu", "cui", "vie",
            "cul", "curu", "dae", "dag", "or", "del", "din", "dol", "dor", "draug", "du", "duin", "dur", "ear", "ech",
            "or", "edh", "el", "eith", "elen", "er", "ereg", "es", "gal", "fal", "as", "far", "oth", "faug", "fea",
            "fin", "for", "men", "fuin", "gaer", "gaur", "gil", "gir", "ith", "glin", "gol", "odh", "gond", "gor",
            "groth", "grod", "gul", "gurth", "gwaith", "gwath", "wath", "had", "hod", "haudh", "heru", "him", "hini",
            "hith", "hoth", "hyar", "men", "ia", "iant", "iath", "iaur", "ilm", "iluve", "kal", "gal", "kano", "kel",
            "kemen", "khel", "ek", "khil", "kir", "lad", "laure", "lhach", "lin", "lith", "lok", "lom", "lome", "londe",
            "los", "loth", "luin", "maeg", "mal", "man", "mel", "men", "menel", "mer", "eth", "min", "as", "mir",
            "mith", "mor", "moth", "nan", "nar", "naug", "dil", "dur", "nel", "dor", "nen", "nim", "orn", "orod", "os",
            "pal", "an", "pel", "quen", "quet", "ram", "ran", "rant", "ras", "rauko", "ril", "rim", "ring", "ris",
            "roch", "rom", "rond", "ros", "ruin", "ruth", "sarn", "ser", "eg", "sil", "sir", "sul", "tal", "dal", "tal",
            "ath", "tar", "tath", "ar", "taur", "tel", "thal", "thang", "thar", "thaur", "thin", "thol", "thon", "thor",
            "on", "til", "tin", "tir", "tol", "tum", "tur", "uial", "ur", "val", "wen", "wing", "yave"
        };

        public static readonly string[] TreasureFeelingText =
                                                                                                                {
            "You're not sure about this level yet.", "you feel it contains something special",
            "treasure galore!", "with a veritable hoard.",
            "powerful magic can be found here.", "there's magic in the air.", "there's wealth to be found.",
            "with significant treasure.", "there's not much of value here.",
            "with nothing of worth.", "what meagre pickings..."
        };

        public static void PopulateNewProfile(Profile profile)
        {
            profile.FixedArtifacts = new FixedArtifactArray();
            profile.MonsterRaces = new MonsterRaceArray();
            profile.MonsterRaces.AddKnowledge();
            profile.RareItemTypes = new RareItemTypeArray();
            profile.ItemTypes = new ItemTypeArray();
            profile.VaultTypes = new VaultTypeArray();
        }
    }
}