// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
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
            var array = new[]
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
            for (var i = 0; i < array.Length; i++)
            {
                array[i].Index = i;
            }
            return array;
        }
    }
}