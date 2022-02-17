using Cthangband.Enumerations;
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

        private Town(Store[] store, int housePrice, string name, char character)
        {
            X = 0;
            Y = 0;
            Seed = 0;
            Visited = false;
            Stores = store;
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
                    new[]
                    {
                        new Store(StoreType.StoreGeneral), new Store(StoreType.StoreArmoury),
                        new Store(StoreType.StoreWeapon), new Store(StoreType.StoreTemple),
                        new Store(StoreType.StoreTemple), new Store(StoreType.StoreAlchemist),
                        new Store(StoreType.StoreMagic), new Store(StoreType.StoreHome),
                        new Store(StoreType.StoreLibrary), new Store(StoreType.StoreInn),
                        new Store(StoreType.StoreHall), new Store(StoreType.StorePawn)
                    }, 50000, "the beautiful city of Celephais", 'C'),
                // Dylath-Leen
                new Town(
                    new[]
                    {
                        new Store(StoreType.StoreGeneral), new Store(StoreType.StoreArmoury),
                        new Store(StoreType.StoreWeapon), new Store(StoreType.StoreBlack),
                        new Store(StoreType.StoreBlack), new Store(StoreType.StoreBlack),
                        new Store(StoreType.StoreHome), new Store(StoreType.StoreLibrary),
                        new Store(StoreType.StoreEmptyLot), new Store(StoreType.StoreInn),
                        new Store(StoreType.StoreHall), new Store(StoreType.StorePawn)
                    }, 25000, "the unwholesome city of Dylath-Leen", 'D'),
                // Hlanith
                new Town(
                    new[]
                    {
                        new Store(StoreType.StoreGeneral), new Store(StoreType.StoreArmoury),
                        new Store(StoreType.StoreEmptyLot), new Store(StoreType.StoreWeapon),
                        new Store(StoreType.StoreEmptyLot), new Store(StoreType.StoreAlchemist),
                        new Store(StoreType.StoreMagic), new Store(StoreType.StoreBlack),
                        new Store(StoreType.StoreHome), new Store(StoreType.StoreLibrary),
                        new Store(StoreType.StoreInn), new Store(StoreType.StoreHall)
                    }, 45000, "the market town of Hlanith", 'H'),
                // Inganok
                new Town(
                    new[]
                    {
                        new Store(StoreType.StoreGeneral), new Store(StoreType.StoreArmoury),
                        new Store(StoreType.StoreWeapon), new Store(StoreType.StoreTemple),
                        new Store(StoreType.StoreAlchemist), new Store(StoreType.StoreEmptyLot),
                        new Store(StoreType.StoreMagic), new Store(StoreType.StoreBlack),
                        new Store(StoreType.StoreEmptyLot), new Store(StoreType.StoreLibrary),
                        new Store(StoreType.StoreInn), new Store(StoreType.StorePawn)
                    }, 0, "the industrious town of Inganok", 'I'),
                // Kadath
                new Town(
                    new[]
                    {
                        new Store(StoreType.StoreEmptyLot), new Store(StoreType.StoreEmptyLot),
                        new Store(StoreType.StoreEmptyLot), new Store(StoreType.StoreEmptyLot),
                        new Store(StoreType.StoreEmptyLot), new Store(StoreType.StoreEmptyLot),
                        new Store(StoreType.StoreEmptyLot), new Store(StoreType.StoreEmptyLot),
                        new Store(StoreType.StoreEmptyLot), new Store(StoreType.StoreEmptyLot),
                        new Store(StoreType.StoreEmptyLot), new Store(StoreType.StoreEmptyLot)
                    }, 0, "Kadath, home of the Gods", 'K'),
                // Nir
                new Town(
                    new[]
                    {
                        new Store(StoreType.StoreGeneral), new Store(StoreType.StoreEmptyLot),
                        new Store(StoreType.StoreEmptyLot), new Store(StoreType.StoreEmptyLot),
                        new Store(StoreType.StoreEmptyLot), new Store(StoreType.StoreEmptyLot),
                        new Store(StoreType.StoreEmptyLot), new Store(StoreType.StoreEmptyLot),
                        new Store(StoreType.StoreEmptyLot), new Store(StoreType.StoreEmptyLot),
                        new Store(StoreType.StoreInn), new Store(StoreType.StorePawn)
                    }, 0, "the hamlet of Nir", 'N'),
                // Ulthar
                new Town(
                    new[]
                    {
                        new Store(StoreType.StoreGeneral), new Store(StoreType.StoreArmoury),
                        new Store(StoreType.StoreWeapon), new Store(StoreType.StoreTemple),
                        new Store(StoreType.StoreAlchemist), new Store(StoreType.StoreMagic),
                        new Store(StoreType.StoreEmptyLot), new Store(StoreType.StoreHome),
                        new Store(StoreType.StoreLibrary), new Store(StoreType.StoreInn),
                        new Store(StoreType.StoreHall), new Store(StoreType.StorePawn)
                    }, 45000, "the picturesque town of Ulthar", 'U'),
                // Ilek-Vad
                new Town(
                    new[]
                    {
                        new Store(StoreType.StoreGeneral), new Store(StoreType.StoreArmoury),
                        new Store(StoreType.StoreWeapon), new Store(StoreType.StoreTemple),
                        new Store(StoreType.StoreAlchemist), new Store(StoreType.StoreMagic),
                        new Store(StoreType.StoreBlack), new Store(StoreType.StoreHome),
                        new Store(StoreType.StoreLibrary), new Store(StoreType.StoreEmptyLot),
                        new Store(StoreType.StoreInn), new Store(StoreType.StoreHall)
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