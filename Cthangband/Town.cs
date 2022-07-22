// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Stores;
using System;

namespace Cthangband
{
    [Serializable]
    internal class Town
    {
        public readonly char Char;
        public readonly int HousePrice;
        public readonly string Name;
        public readonly Store[] Stores;
        public int Index;
        public int Seed;
        public bool Visited;
        public int X;
        public int Y;

        private Town(Store[] stores, int housePrice, string name, char character)
        {
            X = 0;
            Y = 0;
            Seed = 0;
            Visited = false;
            Stores = stores;
            HousePrice = housePrice;
            Name = name;
            Char = character;
        }

        public static Town[] NewTownList()
        {
            Town[] array = new[]
            {
                // Celephais
                new Town(
                    new Store[]
                    {
                        new GeneralStore(), new ArmouryStore(),
                        new WeaponStore(), new TempleStore(),
                        new TempleStore(), new AlchemistStore(),
                        new MagicStore(), new HomeStore(),
                        new LibraryStore(), new InnStore(),
                        new HallStore(), new PawnStore()
                    }, 50000, "the beautiful city of Celephais", 'C'),
                // Dylath-Leen
                new Town(
                    new Store[]
                    {
                        new GeneralStore(), new ArmouryStore(),
                        new WeaponStore(), new BlackStore(),
                        new BlackStore(), new BlackStore(),
                        new HomeStore(), new LibraryStore(),
                        new EmptyLotStore(), new InnStore(),
                        new HallStore(), new PawnStore()
                    }, 25000, "the unwholesome city of Dylath-Leen", 'D'),
                // Hlanith
                new Town(
                    new Store[]
                    {
                        new GeneralStore(), new ArmouryStore(),
                        new EmptyLotStore(), new WeaponStore(),
                        new EmptyLotStore(), new AlchemistStore(),
                        new MagicStore(), new BlackStore(),
                        new HomeStore(), new LibraryStore(),
                        new InnStore(), new HallStore()
                    }, 45000, "the market town of Hlanith", 'H'),
                // Inganok
                new Town(
                    new Store[]
                    {
                        new GeneralStore(), new ArmouryStore(),
                        new WeaponStore(), new TempleStore(),
                        new AlchemistStore(), new EmptyLotStore(),
                        new MagicStore(), new BlackStore(),
                        new EmptyLotStore(), new LibraryStore(),
                        new InnStore(), new PawnStore()
                    }, 0, "the industrious town of Inganok", 'I'),
                // Kadath
                new Town(
                    new[]
                    {
                        new EmptyLotStore(), new EmptyLotStore(),
                        new EmptyLotStore(), new EmptyLotStore(),
                        new EmptyLotStore(), new EmptyLotStore(),
                        new EmptyLotStore(), new EmptyLotStore(),
                        new EmptyLotStore(), new EmptyLotStore(),
                        new EmptyLotStore(), new EmptyLotStore()
                    }, 0, "Kadath, home of the Gods", 'K'),
                // Nir
                new Town(
                    new Store[]
                    {
                        new GeneralStore(), new EmptyLotStore(),
                        new EmptyLotStore(), new EmptyLotStore(),
                        new EmptyLotStore(), new EmptyLotStore(),
                        new EmptyLotStore(), new EmptyLotStore(),
                        new EmptyLotStore(), new EmptyLotStore(),
                        new InnStore(), new PawnStore()
                    }, 0, "the hamlet of Nir", 'N'),
                // Ulthar
                new Town(
                    new Store[]
                    {
                        new GeneralStore(), new ArmouryStore(),
                        new WeaponStore(), new TempleStore(),
                        new AlchemistStore(), new MagicStore(),
                        new EmptyLotStore(), new HomeStore(),
                        new LibraryStore(), new InnStore(),
                        new HallStore(), new PawnStore()
                    }, 45000, "the picturesque town of Ulthar", 'U'),
                // Ilek-Vad
                new Town(
                    new Store[]
                    {
                        new GeneralStore(), new ArmouryStore(),
                        new WeaponStore(), new TempleStore(),
                        new AlchemistStore(), new MagicStore(),
                        new BlackStore(), new HomeStore(),
                        new LibraryStore(), new EmptyLotStore(),
                        new InnStore(), new HallStore()
                    }, 60000, "the city of Ilek-Vad", 'V')
            };
            for (int i = 0; i < array.Length; i++)
            {
                array[i].Index = i;
            }
            return array;
        }
    }
}