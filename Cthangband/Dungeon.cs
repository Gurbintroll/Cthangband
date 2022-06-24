// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.StaticData;
using System;

namespace Cthangband
{
    /// <summary>
    /// A dungeon that the player can explore
    /// </summary>
    [Serializable]
    internal class Dungeon
    {
        /// <summary>
        /// The base offset (difficulty) for the dungeon
        /// </summary>
        public readonly int BaseOffset;

        /// <summary>
        /// The bias for monster generation in the dungeon
        /// </summary>
        public readonly int Bias;

        /// <summary>
        /// The race of the first fixed quest monster
        /// </summary>
        public readonly string FirstGuardian;

        /// <summary>
        /// The level of the first fixed quest
        /// </summary>
        public readonly int FirstLevel;

        /// <summary>
        /// The symbol used for the dungeon on the wilderness map
        /// </summary>
        public readonly string MapSymbol;

        /// <summary>
        /// The number of levels the dungeon has
        /// </summary>
        public readonly int MaxLevel;

        /// <summary>
        /// The full name of the dungeon
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The race of the second fixed quest monster
        /// </summary>
        public readonly string SecondGuardian;

        /// <summary>
        /// The level of the second fixed quest
        /// </summary>
        public readonly int SecondLevel;

        /// <summary>
        /// The shortened name of the dungeon for display purposes
        /// </summary>
        public readonly string Shortname;

        /// <summary>
        /// Whether or not the dungeon is actually a tower
        /// </summary>
        public readonly bool Tower;

        /// <summary>
        /// The index of the dungeon in the dungeon list
        /// </summary>
        public int Index;

        /// <summary>
        /// Whether or not the player knows the depth of the dungeon
        /// </summary>
        public bool KnownDepth;

        /// <summary>
        /// Whether or not the player knows the offset (difficulty) of the dungeon
        /// </summary>
        public bool KnownOffset;

        /// <summary>
        /// The actual offset (difficulty) of the dungeon
        /// </summary>
        public int Offset;

        /// <summary>
        /// Whether or not the player has visited the dungeon
        /// </summary>
        public bool Visited;

        /// <summary>
        /// The X coordinate of the dungeon on the island
        /// </summary>
        public int X;

        /// <summary>
        /// The Y coordinate of the dungeon on the island
        /// </summary>
        public int Y;

        /// <summary>
        /// Create the dungeon
        /// </summary>
        /// <param name="tower"> Whether or not the dungeon is a tower </param>
        /// <param name="offset"> The base offset of the dungeon </param>
        /// <param name="maxLevel"> The number of levels the dungeon has </param>
        /// <param name="bias"> The bias for monster generation in the dungeon </param>
        /// <param name="firstGuardian"> The race of the dungeon's first quest monster </param>
        /// <param name="secondGuardian"> The race of the dungeon's second quest monster </param>
        /// <param name="firstLevel"> The level of the dungeon's first quest </param>
        /// <param name="secondLevel"> The level of the dungeon's second quest </param>
        /// <param name="name"> The full name of the dungeon </param>
        /// <param name="shortname"> The shortened display name of the dungeon </param>
        /// <param name="mapSymbol"> The symbol for the dungeon on the wilderness map </param>
        private Dungeon(bool tower, int offset, int maxLevel, int bias, string firstGuardian, string secondGuardian,
            int firstLevel, int secondLevel, string name, string shortname, string mapSymbol)
        {
            X = 0;
            Y = 0;
            Tower = tower;
            Offset = offset;
            BaseOffset = offset;
            MaxLevel = maxLevel;
            Bias = bias;
            FirstGuardian = firstGuardian;
            SecondGuardian = secondGuardian;
            FirstLevel = firstLevel;
            SecondLevel = secondLevel;
            Name = name;
            Shortname = shortname;
            MapSymbol = mapSymbol;
            Visited = false;
            KnownDepth = false;
            KnownOffset = false;
        }

        /// <summary>
        /// Create the list of dungeons from a hard-coded set
        /// </summary>
        /// <returns> </returns>
        public static Dungeon[] NewDungeonList()
        {
            Dungeon[] array = new[]
            {
                // Note that the indices of the first eight dungeons match the towns that they are under
                new Dungeon(false, 0, 3, 0, "", "", 0, 0, "the Sewers under Celephais", "Celephais", "C"),
                new Dungeon(false, 1, 9, 0, "", "", 0, 0, "the Sewers under Dylath Leen", "Dylath Leen", "D"),
                new Dungeon(false, 0, 5, 0, "", "", 0, 0, "the Sewers under Hlanith", "Hlanith", "H"),
                new Dungeon(false, 0, 5, 0, "", "", 0, 0, "the Sewers under Inganok", "Inganok", "I"),
                new Dungeon(false, 50, 75, Constants.SummonCthuloid, "Nyarlathotep", "Azathoth, The Daemon Sultan", 49, 50, "the Catacombs under Kadath",
                    "Kadath", "K"),
                new Dungeon(false, 0, 7, Constants.SummonHuman, "Robin Hood, the Outlaw", "", 7, 0, "the Sewers under Nir", "Nir", "N"),
                new Dungeon(false, 0, 7, Constants.SummonAnimal, "Hobbes the Tiger", "", 7, 0, "the Sewers under Ulthar", "Ulthar",
                    "U"),
                new Dungeon(false, 0, 5, 0, "", "", 0, 0, "the Sewers under Ilek-Vad", "Ilek-Vad", "V"),
                // The remaining dungeons are in the wilderness
                new Dungeon(false, 30, 20, 0, "The Collector", "", 20, 0, "the Collector's Cave", "Cave", "c"),
                new Dungeon(true, 15, 20, Constants.SummonDemon, "The Emissary", "Glaryssa, Succubus Queen", 1, 20, "the Demon Spire", "Spire", "d"),
                new Dungeon(true, 20, 20, Constants.SummonElemental, "Lasha, Mistress of Water", "Grom, Master of Earth", 15, 20, "the Conflux of the Elements",
                    "Conflux", "e"),
                new Dungeon(true, 1, 5, Constants.SummonKobold, "Vort the Kobold Queen", "", 5, 0, "the Kobold Fort", "Fort", "f"),
                new Dungeon(true, 40, 20, Constants.SummonCthuloid, "Father Dagon", "Tulzscha", 1, 20, "the Tower of Koth", "Koth", "k"),
                new Dungeon(false, 15, 35, Constants.SummonDragon, "Glaurung, Father of the Dragons", "Ancalagon the Black", 34, 35, "the Dragon's Lair", "Dragon Lair",
                    "l"),
                new Dungeon(true, 30, 40, Constants.SummonHiUndead, "Fire Phantom", "Vecna, the Emperor Lich", 1, 40, "the Necropolis", "Necropolis",
                    "n"),
                new Dungeon(true, 3, 17, Constants.SummonOrc, "Bolg, Son of Azog", "Azog, King of the Uruk-Hai", 16, 17, "the Orc Tower", "Orc Tower", "o"),
                new Dungeon(true, 13, 17, Constants.SummonSpider, "Shelob, Spider of Darkness", "", 17, 0, "Shelob's Tower", "Tower", "s"),
                new Dungeon(false, 4, 21, Constants.SummonUndead, "The disembodied hand", "Khufu the mummified King", 1, 21, "Khufu's Tomb", "Tomb", "t"),
                new Dungeon(false, 10, 30, 0, "The Stormbringer", "", 30, 0, "the Vault of the Sword", "Vault", "v"),
                new Dungeon(false, 2, 8, Constants.SummonYeek, "Orfax, Son of Boldor", "Boldor, King of the Yeeks", 7, 8, "the Yeek King's Lair", "Yeek Lair", "y")
            };
            // Make sure each dungeon knows its index
            for (int i = 0; i < array.Length; i++)
            {
                array[i].Index = i;
            }
            return array;
        }

        /// <summary>
        /// Randomise the actual offset (difficulty) of each dungeon around its base offset for some variability
        /// </summary>
        public void RandomiseOffset()
        {
            // Maximum offset is the dungeon's base offset or 10, whichever is less (but since we're
            // using 'less than', we add one to both values here)
            int range = Math.Min((BaseOffset + 1), 11);
            int offsetChange = Program.Rng.RandomLessThan(range);
            // Dungeons are equally likely to be more or less difficult
            if (Program.Rng.DieRoll(6) >= 4)
            {
                Offset = BaseOffset + offsetChange;
            }
            else
            {
                Offset = BaseOffset - offsetChange;
            }
        }
    }
}