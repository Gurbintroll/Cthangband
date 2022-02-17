using Cthangband.Enumerations;
using System;

namespace Cthangband
{
    internal static class StoreFactory
    {
        private static readonly StoreOwner[] _alchemistOwners =
        {
            new StoreOwner("Mauser the Chemist", 10000, 111, RaceId.HalfElf),
            new StoreOwner("Wizzle the Chaotic", 10000, 110, RaceId.Hobbit),
            new StoreOwner("Kakalrakakal", 15000, 116, RaceId.Klackon),
            new StoreOwner("Jal-Eth the Alchemist", 15000, 111, RaceId.Elf),
            new StoreOwner("Fanelath the Cautious", 10000, 111, RaceId.Dwarf),
            new StoreOwner("Runcie the Insane", 10000, 110, RaceId.Human),
            new StoreOwner("Grumbleworth", 15000, 116, RaceId.Gnome),
            new StoreOwner("Flitter", 15000, 111, RaceId.Sprite), new StoreOwner("Xarillus", 10000, 111, RaceId.Human),
            new StoreOwner("Egbert the Old", 10000, 110, RaceId.Dwarf),
            new StoreOwner("Valindra the Proud", 15000, 116, RaceId.HighElf),
            new StoreOwner("Taen the Alchemist", 15000, 111, RaceId.Human),
            new StoreOwner("Cayd the Sweet", 10000, 111, RaceId.Vampire),
            new StoreOwner("Fulir the Dark", 10000, 110, RaceId.Nibelung),
            new StoreOwner("Domli the Humble", 15000, 116, RaceId.Dwarf),
            new StoreOwner("Yaarjukka Demonspawn", 15000, 111, RaceId.Imp),
            new StoreOwner("Gelaraldor the Herbmaster", 10000, 111, RaceId.HighElf),
            new StoreOwner("Olelaldan the Wise", 10000, 110, RaceId.TchoTcho),
            new StoreOwner("Fthoglo the Demonicist", 15000, 116, RaceId.Imp),
            new StoreOwner("Dridash the Alchemist", 15000, 111, RaceId.HalfOrc)
        };

        private static readonly StoreOwner[] _armouryOwners =
        {
            new StoreOwner("Kon-Dar the Ugly", 10000, 115, RaceId.HalfOrc),
            new StoreOwner("Darg-Low the Grim", 15000, 111, RaceId.Human),
            new StoreOwner("Decado the Handsome", 25000, 112, RaceId.Great),
            new StoreOwner("Elo Dragonscale", 30000, 112, RaceId.Elf),
            new StoreOwner("Delicatus", 10000, 115, RaceId.Sprite),
            new StoreOwner("Gruce the Huge", 15000, 111, RaceId.HalfGiant),
            new StoreOwner("Animus", 25000, 112, RaceId.Golem), new StoreOwner("Malvus", 30000, 112, RaceId.HalfTitan),
            new StoreOwner("Selaxis", 10000, 115, RaceId.Zombie),
            new StoreOwner("Deathchill", 15000, 111, RaceId.Spectre),
            new StoreOwner("Drios the Faint", 25000, 112, RaceId.Spectre),
            new StoreOwner("Bathric the Cold", 30000, 112, RaceId.Vampire),
            new StoreOwner("Vengella the Cruel", 10000, 115, RaceId.HalfTroll),
            new StoreOwner("Wyrana the Mighty", 15000, 111, RaceId.Human),
            new StoreOwner("Yojo II", 25000, 112, RaceId.Dwarf),
            new StoreOwner("Ranalar the Sweet", 30000, 112, RaceId.Great),
            new StoreOwner("Horbag the Unclean", 10000, 115, RaceId.HalfOrc),
            new StoreOwner("Elelen the Telepath", 15000, 111, RaceId.DarkElf),
            new StoreOwner("Isedrelias", 25000, 112, RaceId.Sprite),
            new StoreOwner("Vegnar One-eye", 30000, 112, RaceId.Cyclops),
            new StoreOwner("Rodish the Chaotic", 10000, 115, RaceId.MiriNigri),
            new StoreOwner("Hesin Swordmaster", 15000, 111, RaceId.Nibelung),
            new StoreOwner("Elvererith the Cheat", 25000, 112, RaceId.DarkElf),
            new StoreOwner("Zzathath the Imp", 30000, 112, RaceId.Imp)
        };

        private static readonly StoreOwner[] _blackMarketOwners =
        {
            new StoreOwner("Vhassa the Dead", 20000, 150, RaceId.Zombie),
            new StoreOwner("Kyn the Treacherous", 20000, 150, RaceId.Vampire),
            new StoreOwner("Batrachian Belle", 30000, 150, RaceId.MiriNigri),
            new StoreOwner("Corpselight", 30000, 150, RaceId.Spectre),
            new StoreOwner("Parrish the Bloodthirsty", 20000, 150, RaceId.Vampire),
            new StoreOwner("Vile", 20000, 150, RaceId.Skeleton),
            new StoreOwner("Prentice the Trusted", 30000, 150, RaceId.Skeleton),
            new StoreOwner("Griella Humanslayer", 30000, 150, RaceId.Imp),
            new StoreOwner("Charity the Necromancer", 20000, 150, RaceId.DarkElf),
            new StoreOwner("Pugnacious the Pugilist", 20000, 150, RaceId.HalfOrc),
            new StoreOwner("Footsore the Lucky", 30000, 150, RaceId.MiriNigri),
            new StoreOwner("Sidria Lighfingered", 30000, 150, RaceId.Human),
            new StoreOwner("Riatho the Juggler", 20000, 150, RaceId.Hobbit),
            new StoreOwner("Janaaka the Shifty", 20000, 150, RaceId.Gnome),
            new StoreOwner("Cina the Rogue", 30000, 150, RaceId.Gnome),
            new StoreOwner("Arunikki Greatclaw", 30000, 150, RaceId.Draconian),
            new StoreOwner("Chaeand the Poor", 20000, 150, RaceId.Human),
            new StoreOwner("Afardorf the Brigand", 20000, 150, RaceId.TchoTcho),
            new StoreOwner("Lathaxl the Greedy", 30000, 150, RaceId.MindFlayer),
            new StoreOwner("Falarewyn", 30000, 150, RaceId.Sprite)
        };

        private static readonly StoreOwner[] _emptyStoreOwners = { new StoreOwner("Empty lot", 0, 100, 99) };

        private static readonly StoreOwner[] _generalStoreOwners =
        {
            new StoreOwner("Falilmawen the Friendly", 250, 108, RaceId.Hobbit),
            new StoreOwner("Voirin the Cowardly", 500, 108, RaceId.Human),
            new StoreOwner("Erashnak the Midget", 750, 107, RaceId.MiriNigri),
            new StoreOwner("Grug the Comely", 1000, 107, RaceId.HalfTitan),
            new StoreOwner("Forovir the Cheap", 250, 108, RaceId.Human),
            new StoreOwner("Ellis the Fool", 500, 108, RaceId.Human),
            new StoreOwner("Filbert the Hungry", 750, 107, RaceId.Vampire),
            new StoreOwner("Fthnargl Psathiggua", 1000, 107, RaceId.MindFlayer),
            new StoreOwner("Eloise Long-Dead", 250, 108, RaceId.Spectre),
            new StoreOwner("Fundi the Slow", 500, 108, RaceId.Zombie),
            new StoreOwner("Granthus", 750, 107, RaceId.Skeleton),
            new StoreOwner("Lorax the Suave", 1000, 107, RaceId.Vampire),
            new StoreOwner("Butch", 250, 108, RaceId.HalfOrc),
            new StoreOwner("Elbereth the Beautiful", 500, 108, RaceId.HighElf),
            new StoreOwner("Sarleth the Sneaky", 750, 107, RaceId.Gnome),
            new StoreOwner("Narlock", 1000, 107, RaceId.Dwarf),
            new StoreOwner("Haneka the Small", 250, 108, RaceId.Gnome),
            new StoreOwner("Loirin the Mad", 500, 108, RaceId.HalfGiant),
            new StoreOwner("Wuto Poisonbreath", 750, 107, RaceId.Draconian),
            new StoreOwner("Araaka the Rotund", 1000, 107, RaceId.Draconian),
            new StoreOwner("Poogor the Dumb", 250, 108, RaceId.MiriNigri),
            new StoreOwner("Felorfiliand", 500, 108, RaceId.Elf),
            new StoreOwner("Maroka the Aged", 750, 107, RaceId.Gnome),
            new StoreOwner("Sasin the Bold", 1000, 107, RaceId.HalfGiant),
            new StoreOwner("Abiemar the Peasant", 250, 108, RaceId.Human),
            new StoreOwner("Hurk the Poor", 500, 108, RaceId.HalfOrc),
            new StoreOwner("Soalin the Wretched", 750, 107, RaceId.Zombie),
            new StoreOwner("Merulla the Humble", 1000, 107, RaceId.Elf)
        };

        private static readonly StoreOwner[] _hallOwners = { new StoreOwner("Hall of Records", 0, 100, 99) };
        private static readonly StoreOwner[] _homeOwners = { new StoreOwner("Your home", 0, 100, 99) };

        private static readonly StoreOwner[] _innOwners =
        {
            new StoreOwner("Mordsan", 15000, 108, RaceId.Human),
            new StoreOwner("Furfoot Pobber", 20000, 105, RaceId.Hobbit),
            new StoreOwner("Oddo Yeekson", 25000, 110, RaceId.Yeek),
            new StoreOwner("Dry-Bones", 30000, 105, RaceId.Skeleton),
            new StoreOwner("Kleibons", 15000, 108, RaceId.Klackon),
            new StoreOwner("Prendegast", 20000, 105, RaceId.Hobbit),
            new StoreOwner("Straasha", 25000, 110, RaceId.Draconian),
            new StoreOwner("Allia the Servile", 30000, 105, RaceId.Human),
            new StoreOwner("Lumin the Blue", 15000, 108, RaceId.Spectre),
            new StoreOwner("Short Al", 20000, 105, RaceId.Zombie),
            new StoreOwner("Silent Faldus", 25000, 110, RaceId.Zombie),
            new StoreOwner("Quirmby the Strange", 30000, 105, RaceId.Vampire),
            new StoreOwner("Aldous the Sleepy", 15000, 108, RaceId.Human),
            new StoreOwner("Grundy the Tall", 20000, 105, RaceId.Hobbit),
            new StoreOwner("Gobbleguts Thunderbreath", 25000, 110, RaceId.HalfTroll),
            new StoreOwner("Silverscale", 30000, 105, RaceId.Draconian),
            new StoreOwner("Etheraa the Furious", 15000, 108, RaceId.TchoTcho),
            new StoreOwner("Paetlan the Alcoholic", 20000, 105, RaceId.Human),
            new StoreOwner("Drang", 25000, 110, RaceId.HalfOgre),
            new StoreOwner("Barbag the Sly", 30000, 105, RaceId.Kobold),
            new StoreOwner("Kirakak", 15000, 108, RaceId.Klackon),
            new StoreOwner("Nafur the Wooden", 20000, 105, RaceId.Golem),
            new StoreOwner("Grarak the Hospitable", 25000, 110, RaceId.HalfGiant),
            new StoreOwner("Lona the Charismatic", 30000, 105, RaceId.Gnome),
            new StoreOwner("Crediric the Brewer", 15000, 108, RaceId.Human),
            new StoreOwner("Nydudus the Slow", 20000, 105, RaceId.Zombie),
            new StoreOwner("Baurk the Busy", 25000, 110, RaceId.Yeek),
            new StoreOwner("Seviras the Mindcrafter", 30000, 105, RaceId.Human)
        };

        private static readonly StoreOwner[] _libraryOwners =
        {
            new StoreOwner("Randolph Carter", 15000, 108, RaceId.Human),
            new StoreOwner("Odnar the Sage", 20000, 105, RaceId.HighElf),
            new StoreOwner("Gandar the Neutral", 25000, 110, RaceId.Vampire),
            new StoreOwner("Ro-sha the Patient", 30000, 105, RaceId.Golem),
            new StoreOwner("Sarai the Swift", 15000, 108, RaceId.Human),
            new StoreOwner("Bodril the Seer", 20000, 105, RaceId.HighElf),
            new StoreOwner("Veloin the Quiet", 25000, 110, RaceId.Zombie),
            new StoreOwner("Vanthylas the Learned", 30000, 105, RaceId.MindFlayer),
            new StoreOwner("Ossein the Literate", 15000, 108, RaceId.Skeleton),
            new StoreOwner("Olvar Bookworm", 20000, 105, RaceId.Vampire),
            new StoreOwner("Shallowgrave", 25000, 110, RaceId.Zombie),
            new StoreOwner("Death Mask", 30000, 105, RaceId.Zombie),
            new StoreOwner("Porcina the Obese", 15000, 108, RaceId.HalfOrc),
            new StoreOwner("Glaruna Brandybreath", 20000, 105, RaceId.Dwarf),
            new StoreOwner("Furface Yeek", 25000, 110, RaceId.Yeek),
            new StoreOwner("Bald Oggin", 30000, 105, RaceId.Gnome),
            new StoreOwner("Asuunu the Learned", 15000, 108, RaceId.MindFlayer),
            new StoreOwner("Prirand the Dead", 20000, 105, RaceId.Zombie),
            new StoreOwner("Ronar the Iron", 25000, 110, RaceId.Golem),
            new StoreOwner("Galil-Gamir", 30000, 105, RaceId.Elf),
            new StoreOwner("Rorbag Book-Eater", 15000, 108, RaceId.Kobold),
            new StoreOwner("Kiriarikirk", 20000, 105, RaceId.Klackon),
            new StoreOwner("Rilin the Quiet", 25000, 110, RaceId.Dwarf),
            new StoreOwner("Isung the Lord", 30000, 105, RaceId.HighElf)
        };

        private static readonly StoreOwner[] _magicShopOwners =
        {
            new StoreOwner("Skidney the Sorcerer", 15000, 110, RaceId.HalfElf),
            new StoreOwner("Buggerby the Great", 20000, 113, RaceId.Gnome),
            new StoreOwner("Kyria the Illusionist", 30000, 110, RaceId.Human),
            new StoreOwner("Nikki the Necromancer", 30000, 110, RaceId.DarkElf),
            new StoreOwner("Solostoran", 15000, 110, RaceId.Sprite),
            new StoreOwner("Achshe the Tentacled", 20000, 113, RaceId.MindFlayer),
            new StoreOwner("Kaza the Noble", 30000, 110, RaceId.HighElf),
            new StoreOwner("Fazzil the Dark", 30000, 110, RaceId.DarkElf),
            new StoreOwner("Angel", 20000, 150, RaceId.Vampire),
            new StoreOwner("Flotsam the Bloated", 20000, 150, RaceId.Zombie),
            new StoreOwner("Nieval", 30000, 150, RaceId.Vampire),
            new StoreOwner("Anastasia the Luminous", 30000, 150, RaceId.Spectre),
            new StoreOwner("Keldorn the Grand", 15000, 110, RaceId.Dwarf),
            new StoreOwner("Philanthropus", 20000, 113, RaceId.Hobbit),
            new StoreOwner("Agnar the Enchantress", 30000, 110, RaceId.Human),
            new StoreOwner("Buliance the Necromancer", 30000, 110, RaceId.MiriNigri),
            new StoreOwner("Vuirak the High-Mage", 15000, 110, RaceId.MiriNigri),
            new StoreOwner("Madish the Smart", 20000, 113, RaceId.MiriNigri),
            new StoreOwner("Falebrimbor", 30000, 110, RaceId.HighElf),
            new StoreOwner("Felil-Gand the Subtle", 30000, 110, RaceId.DarkElf),
            new StoreOwner("Thalegord the Shaman", 15000, 110, RaceId.TchoTcho),
            new StoreOwner("Cthoaloth the Mystic", 20000, 113, RaceId.MindFlayer),
            new StoreOwner("Ibeli the Illusionist", 30000, 110, RaceId.Skeleton),
            new StoreOwner("Heto the Necromancer", 30000, 110, RaceId.Yeek)
        };

        private static readonly StoreOwner[] _pawnbrokerOwners =
        {
            new StoreOwner("Magd the Ruthless", 2000, 100, RaceId.Human),
            new StoreOwner("Drako Fairdeal", 4000, 100, RaceId.Draconian),
            new StoreOwner("Featherwing", 5000, 100, RaceId.Sprite),
            new StoreOwner("Xochinaggua", 10000, 100, RaceId.MindFlayer),
            new StoreOwner("Od the Penniless", 2000, 100, RaceId.Elf), new StoreOwner("Xax", 4000, 100, RaceId.Golem),
            new StoreOwner("Jake Small", 5000, 100, RaceId.HalfGiant),
            new StoreOwner("Helga the Lost", 10000, 100, RaceId.Human),
            new StoreOwner("Gloom the Phlegmatic", 2000, 100, RaceId.Zombie),
            new StoreOwner("Quick-Arm Vollaire", 4000, 100, RaceId.Vampire),
            new StoreOwner("Asenath", 5000, 100, RaceId.Zombie),
            new StoreOwner("Lord Filbert", 10000, 100, RaceId.Vampire),
            new StoreOwner("Herranyth the Ruthless", 2000, 100, RaceId.Human),
            new StoreOwner("Gagrin Moneylender", 4000, 100, RaceId.Yeek),
            new StoreOwner("Thrambor the Grubby", 5000, 100, RaceId.HalfElf),
            new StoreOwner("Derigrin the Honest", 10000, 100, RaceId.Hobbit),
            new StoreOwner("Munk the Barterer", 2000, 100, RaceId.HalfOgre),
            new StoreOwner("Gadrialdur the Fair", 4000, 100, RaceId.HalfElf),
            new StoreOwner("Ninar the Stooped", 5000, 100, RaceId.Dwarf),
            new StoreOwner("Adirath the Unmagical", 10000, 100, RaceId.TchoTcho)
        };

        private static readonly StoreOwner[] _templeOwners =
        {
            new StoreOwner("Ludwig the Humble", 10000, 109, RaceId.Dwarf),
            new StoreOwner("Gunnar the Paladin", 15000, 110, RaceId.HalfTroll),
            new StoreOwner("Sir Parsival the Pure", 25000, 107, RaceId.HighElf),
            new StoreOwner("Asenath the Holy", 30000, 109, RaceId.Human),
            new StoreOwner("McKinnon", 10000, 109, RaceId.Human),
            new StoreOwner("Mistress Chastity", 15000, 110, RaceId.HighElf),
            new StoreOwner("Hashnik the Druid", 25000, 107, RaceId.Hobbit),
            new StoreOwner("Finak", 30000, 109, RaceId.Yeek), new StoreOwner("Krikkik", 10000, 109, RaceId.Klackon),
            new StoreOwner("Morival the Wild", 15000, 110, RaceId.Elf),
            new StoreOwner("Hoshak the Dark", 25000, 107, RaceId.Imp),
            new StoreOwner("Atal the Wise", 30000, 109, RaceId.Human),
            new StoreOwner("Ibenidd the Chaste", 10000, 109, RaceId.Human),
            new StoreOwner("Eridish", 15000, 110, RaceId.HalfTroll),
            new StoreOwner("Vrudush the Shaman", 25000, 107, RaceId.HalfOgre),
            new StoreOwner("Haob the Berserker", 30000, 109, RaceId.TchoTcho),
            new StoreOwner("Proogdish the Youthfull", 10000, 109, RaceId.HalfOgre),
            new StoreOwner("Lumwise the Mad", 15000, 110, RaceId.Yeek),
            new StoreOwner("Muirt the Virtuous", 25000, 107, RaceId.Kobold),
            new StoreOwner("Dardobard the Weak", 30000, 109, RaceId.Spectre)
        };

        private static readonly StoreOwner[] _weaponsmithOwners =
        {
            new StoreOwner("Arnold the Beastly", 10000, 115, RaceId.TchoTcho),
            new StoreOwner("Arndal Beast-Slayer", 15000, 110, RaceId.HalfElf),
            new StoreOwner("Edor the Short", 25000, 115, RaceId.Hobbit),
            new StoreOwner("Oglign Dragon-Slayer", 30000, 112, RaceId.Dwarf),
            new StoreOwner("Drew the Skilled", 10000, 115, RaceId.Human),
            new StoreOwner("Orrax Dragonson", 15000, 110, RaceId.Draconian),
            new StoreOwner("Bob", 25000, 115, RaceId.MiriNigri),
            new StoreOwner("Arkhoth the Stout", 30000, 112, RaceId.Dwarf),
            new StoreOwner("Sarlyas the Rotten", 10000, 115, RaceId.Zombie),
            new StoreOwner("Tuethic Bare-Bones", 15000, 110, RaceId.Skeleton),
            new StoreOwner("Bilious the Toad", 25000, 115, RaceId.MiriNigri),
            new StoreOwner("Fasgul", 30000, 112, RaceId.Zombie),
            new StoreOwner("Ellefris the Paladin", 10000, 115, RaceId.TchoTcho),
            new StoreOwner("K'trrik'k", 15000, 110, RaceId.Klackon),
            new StoreOwner("Drocus Spiderfriend", 25000, 115, RaceId.DarkElf),
            new StoreOwner("Fungus Giant-Slayer", 30000, 112, RaceId.Dwarf),
            new StoreOwner("Nadoc the Strong", 10000, 115, RaceId.Hobbit),
            new StoreOwner("Eramog the Weak", 15000, 110, RaceId.Kobold),
            new StoreOwner("Eowilith the Fair", 25000, 115, RaceId.Vampire),
            new StoreOwner("Huimog Balrog-Slayer", 30000, 112, RaceId.HalfOrc),
            new StoreOwner("Peadus the Cruel", 10000, 115, RaceId.Human),
            new StoreOwner("Vamog Slayer", 15000, 110, RaceId.HalfOgre),
            new StoreOwner("Hooshnak the Vicious", 25000, 115, RaceId.MiriNigri),
            new StoreOwner("Balenn War-Dancer", 30000, 112, RaceId.TchoTcho)
        };

        public static StoreOwner GetRandomOwner(StoreType storeType)
        {
            switch (storeType)
            {
                case StoreType.StoreGeneral:
                    return _generalStoreOwners[Program.Rng.RandomLessThan(_generalStoreOwners.Length)];

                case StoreType.StoreArmoury:
                    return _armouryOwners[Program.Rng.RandomLessThan(_armouryOwners.Length)];

                case StoreType.StoreWeapon:
                    return _weaponsmithOwners[Program.Rng.RandomLessThan(_weaponsmithOwners.Length)];

                case StoreType.StoreTemple:
                    return _templeOwners[Program.Rng.RandomLessThan(_templeOwners.Length)];

                case StoreType.StoreAlchemist:
                    return _alchemistOwners[Program.Rng.RandomLessThan(_alchemistOwners.Length)];

                case StoreType.StoreMagic:
                    return _magicShopOwners[Program.Rng.RandomLessThan(_magicShopOwners.Length)];

                case StoreType.StoreBlack:
                    return _blackMarketOwners[Program.Rng.RandomLessThan(_blackMarketOwners.Length)];

                case StoreType.StoreHome:
                    return _homeOwners[Program.Rng.RandomLessThan(_homeOwners.Length)];

                case StoreType.StoreLibrary:
                    return _libraryOwners[Program.Rng.RandomLessThan(_libraryOwners.Length)];

                case StoreType.StoreInn:
                    return _innOwners[Program.Rng.RandomLessThan(_innOwners.Length)];

                case StoreType.StoreHall:
                    return _hallOwners[Program.Rng.RandomLessThan(_hallOwners.Length)];

                case StoreType.StorePawn:
                    return _pawnbrokerOwners[Program.Rng.RandomLessThan(_pawnbrokerOwners.Length)];

                case StoreType.StoreEmptyLot:
                    return _emptyStoreOwners[Program.Rng.RandomLessThan(_emptyStoreOwners.Length)];

                default:
                    throw new ArgumentOutOfRangeException(nameof(storeType), storeType, null);
            }
        }

        public static ItemIdentifier[] GetStoreTable(StoreType storeType)
        {
            switch (storeType)
            {
                case StoreType.StoreGeneral:
                    return new[]
                    {
                        new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Biscuit),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Jerky),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Jerky),
                        new ItemIdentifier(ItemCategory.Food, FoodType.PintOfWine),
                        new ItemIdentifier(ItemCategory.Food, FoodType.PintOfAle),
                        new ItemIdentifier(ItemCategory.Light, LightType.Torch),
                        new ItemIdentifier(ItemCategory.Light, LightType.Torch),
                        new ItemIdentifier(ItemCategory.Light, LightType.Torch),
                        new ItemIdentifier(ItemCategory.Light, LightType.Lantern),
                        new ItemIdentifier(ItemCategory.Light, LightType.Lantern),
                        new ItemIdentifier(ItemCategory.Light, LightType.Orb),
                        new ItemIdentifier(ItemCategory.Flask, 0), new ItemIdentifier(ItemCategory.Flask, 0),
                        new ItemIdentifier(ItemCategory.Flask, 0), new ItemIdentifier(ItemCategory.Flask, 0),
                        new ItemIdentifier(ItemCategory.Flask, 0), new ItemIdentifier(ItemCategory.Flask, 0),
                        new ItemIdentifier(ItemCategory.Spike, 0), new ItemIdentifier(ItemCategory.Spike, 0),
                        new ItemIdentifier(ItemCategory.Shot, AmmoType.SvAmmoNormal),
                        new ItemIdentifier(ItemCategory.Arrow, AmmoType.SvAmmoNormal),
                        new ItemIdentifier(ItemCategory.Bolt, AmmoType.SvAmmoNormal),
                        new ItemIdentifier(ItemCategory.Digging, DiggerType.SvShovel),
                        new ItemIdentifier(ItemCategory.Digging, DiggerType.SvPick),
                        new ItemIdentifier(ItemCategory.Cloak, CloakType.SvCloak),
                        new ItemIdentifier(ItemCategory.Cloak, CloakType.SvCloak),
                        new ItemIdentifier(ItemCategory.Cloak, CloakType.SvCloak),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                        new ItemIdentifier(ItemCategory.Light, LightType.Torch),
                        new ItemIdentifier(ItemCategory.Light, LightType.Torch),
                        new ItemIdentifier(ItemCategory.Light, LightType.Lantern),
                        new ItemIdentifier(ItemCategory.Light, LightType.Lantern),
                        new ItemIdentifier(ItemCategory.Flask, 0), new ItemIdentifier(ItemCategory.Flask, 0),
                        new ItemIdentifier(ItemCategory.Flask, 0), new ItemIdentifier(ItemCategory.Flask, 0),
                        new ItemIdentifier(ItemCategory.Shot, AmmoType.SvAmmoNormal),
                        new ItemIdentifier(ItemCategory.Arrow, AmmoType.SvAmmoNormal),
                        new ItemIdentifier(ItemCategory.Bolt, AmmoType.SvAmmoNormal),
                        new ItemIdentifier(ItemCategory.Digging, DiggerType.SvShovel)
                    };

                case StoreType.StoreArmoury:
                    return new[]
                    {
                        new ItemIdentifier(ItemCategory.Boots, BootsType.SvPairOfSoftLeatherBoots),
                        new ItemIdentifier(ItemCategory.Boots, BootsType.SvPairOfSoftLeatherBoots),
                        new ItemIdentifier(ItemCategory.Boots, BootsType.SvPairOfHardLeatherBoots),
                        new ItemIdentifier(ItemCategory.Boots, BootsType.SvPairOfHardLeatherBoots),
                        new ItemIdentifier(ItemCategory.Helm, HelmType.SvHardLeatherCap),
                        new ItemIdentifier(ItemCategory.Helm, HelmType.SvHardLeatherCap),
                        new ItemIdentifier(ItemCategory.Helm, HelmType.SvMetalCap),
                        new ItemIdentifier(ItemCategory.Helm, HelmType.SvIronHelm),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvRobe),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvRobe),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvSoftLeatherArmor),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvSoftLeatherArmor),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvHardLeatherArmor),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvHardLeatherArmor),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvHardStuddedLeather),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvHardStuddedLeather),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvLeatherScaleMail),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvLeatherScaleMail),
                        new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.SvMetalScaleMail),
                        new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.SvChainMail),
                        new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.SvChainMail),
                        new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.SvAugmentedChainMail),
                        new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.SvBarChainMail),
                        new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.SvDoubleChainMail),
                        new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.SvMetalBrigandineArmor),
                        new ItemIdentifier(ItemCategory.Gloves, GlovesType.SvSetOfLeatherGloves),
                        new ItemIdentifier(ItemCategory.Gloves, GlovesType.SvSetOfLeatherGloves),
                        new ItemIdentifier(ItemCategory.Gloves, GlovesType.SvSetOfGauntlets),
                        new ItemIdentifier(ItemCategory.Shield, ShieldType.SvSmallLeatherShield),
                        new ItemIdentifier(ItemCategory.Shield, ShieldType.SvSmallLeatherShield),
                        new ItemIdentifier(ItemCategory.Shield, ShieldType.SvLargeLeatherShield),
                        new ItemIdentifier(ItemCategory.Shield, ShieldType.SvSmallMetalShield),
                        new ItemIdentifier(ItemCategory.Boots, BootsType.SvPairOfHardLeatherBoots),
                        new ItemIdentifier(ItemCategory.Boots, BootsType.SvPairOfHardLeatherBoots),
                        new ItemIdentifier(ItemCategory.Helm, HelmType.SvHardLeatherCap),
                        new ItemIdentifier(ItemCategory.Helm, HelmType.SvHardLeatherCap),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvRobe),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvSoftLeatherArmor),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvSoftLeatherArmor),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvHardLeatherArmor),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SvLeatherScaleMail),
                        new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.SvMetalScaleMail),
                        new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.SvChainMail),
                        new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.SvChainMail),
                        new ItemIdentifier(ItemCategory.Gloves, GlovesType.SvSetOfLeatherGloves),
                        new ItemIdentifier(ItemCategory.Gloves, GlovesType.SvSetOfGauntlets),
                        new ItemIdentifier(ItemCategory.Shield, ShieldType.SvSmallLeatherShield),
                        new ItemIdentifier(ItemCategory.Shield, ShieldType.SvSmallLeatherShield)
                    };

                case StoreType.StoreWeapon:
                    return new[]
                    {
                        new ItemIdentifier(ItemCategory.Sword, SwordType.SvDagger),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.SvMainGauche),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.SvRapier),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.SvSmallSword),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.SvShortSword),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.SvSabre),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.SvCutlass),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.SvTulwar),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.SvBroadSword),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.SvLongSword),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.SvScimitar),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.SvKatana),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.SvBastardSword),
                        new ItemIdentifier(ItemCategory.Polearm, PolearmType.SvSpear),
                        new ItemIdentifier(ItemCategory.Polearm, PolearmType.SvAwlPike),
                        new ItemIdentifier(ItemCategory.Polearm, PolearmType.SvTrident),
                        new ItemIdentifier(ItemCategory.Polearm, PolearmType.SvPike),
                        new ItemIdentifier(ItemCategory.Polearm, PolearmType.SvBeakedAxe),
                        new ItemIdentifier(ItemCategory.Polearm, PolearmType.SvBroadAxe),
                        new ItemIdentifier(ItemCategory.Polearm, PolearmType.SvLance),
                        new ItemIdentifier(ItemCategory.Polearm, PolearmType.SvBattleAxe),
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvWhip),
                        new ItemIdentifier(ItemCategory.Bow, BowType.SvSling),
                        new ItemIdentifier(ItemCategory.Bow, BowType.SvShortBow),
                        new ItemIdentifier(ItemCategory.Bow, BowType.SvLongBow),
                        new ItemIdentifier(ItemCategory.Bow, BowType.SvLightXbow),
                        new ItemIdentifier(ItemCategory.Shot, AmmoType.SvAmmoNormal),
                        new ItemIdentifier(ItemCategory.Shot, AmmoType.SvAmmoNormal),
                        new ItemIdentifier(ItemCategory.Arrow, AmmoType.SvAmmoNormal),
                        new ItemIdentifier(ItemCategory.Arrow, AmmoType.SvAmmoNormal),
                        new ItemIdentifier(ItemCategory.Bolt, AmmoType.SvAmmoNormal),
                        new ItemIdentifier(ItemCategory.Bolt, AmmoType.SvAmmoNormal),
                        new ItemIdentifier(ItemCategory.Bow, BowType.SvLongBow),
                        new ItemIdentifier(ItemCategory.Bow, BowType.SvLightXbow),
                        new ItemIdentifier(ItemCategory.Arrow, AmmoType.SvAmmoNormal),
                        new ItemIdentifier(ItemCategory.Arrow, AmmoType.SvAmmoNormal),
                        new ItemIdentifier(ItemCategory.Bolt, AmmoType.SvAmmoNormal),
                        new ItemIdentifier(ItemCategory.Bolt, AmmoType.SvAmmoNormal),
                        new ItemIdentifier(ItemCategory.Bow, BowType.SvShortBow),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.SvDagger),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.SvMainGauche),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.SvRapier),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.SvSmallSword),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.SvShortSword),
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvWhip),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.SvBroadSword),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.SvLongSword),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.SvScimitar)
                    };

                case StoreType.StoreTemple:
                    return new[]
                    {
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvWhip),
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvQuarterstaff),
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvMace),
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvBallAndChain),
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvWarHammer),
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvLucernHammer),
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvMorningStar),
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvFlail),
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvLeadFilledMace),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.RemoveCurse),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.Blessing),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.HolyChant),
                        new ItemIdentifier(ItemCategory.Potion, PotionType.Heroism),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.WordOfRecall),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.WordOfRecall),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.WordOfRecall),
                        new ItemIdentifier(ItemCategory.Potion, PotionType.CureLight),
                        new ItemIdentifier(ItemCategory.Potion, PotionType.CureSerious),
                        new ItemIdentifier(ItemCategory.Potion, PotionType.CureSerious),
                        new ItemIdentifier(ItemCategory.Potion, PotionType.CureCritical),
                        new ItemIdentifier(ItemCategory.Potion, PotionType.CureCritical),
                        new ItemIdentifier(ItemCategory.Potion, PotionType.RestoreExp),
                        new ItemIdentifier(ItemCategory.Potion, PotionType.RestoreExp),
                        new ItemIdentifier(ItemCategory.Potion, PotionType.RestoreExp),
                        new ItemIdentifier(ItemCategory.LifeBook, 0), new ItemIdentifier(ItemCategory.LifeBook, 0),
                        new ItemIdentifier(ItemCategory.LifeBook, 0), new ItemIdentifier(ItemCategory.LifeBook, 0),
                        new ItemIdentifier(ItemCategory.LifeBook, 1), new ItemIdentifier(ItemCategory.LifeBook, 1),
                        new ItemIdentifier(ItemCategory.LifeBook, 1), new ItemIdentifier(ItemCategory.LifeBook, 1),
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvWhip),
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvMace),
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvBallAndChain),
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.SvWarHammer),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.WordOfRecall),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.WordOfRecall),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.WordOfRecall),
                        new ItemIdentifier(ItemCategory.Potion, PotionType.CureCritical),
                        new ItemIdentifier(ItemCategory.Potion, PotionType.CureCritical),
                        new ItemIdentifier(ItemCategory.Potion, PotionType.RestoreExp),
                        new ItemIdentifier(ItemCategory.Potion, PotionType.RestoreExp),
                        new ItemIdentifier(ItemCategory.Potion, PotionType.RestoreExp),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.RemoveCurse),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.RemoveCurse),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.StarRemoveCurse),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.StarRemoveCurse)
                    };

                case StoreType.StoreAlchemist:
                    return new[]
                    {
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.EnchantWeaponToHit),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.EnchantWeaponToDam),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.EnchantArmor),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.Identify),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.Identify),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.Identify),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.Identify),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.Light),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.PhaseDoor),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.PhaseDoor),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.Teleport),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.MonsterConfusion),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.Mapping),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.DetectGold),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.DetectItem),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.DetectTrap),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.DetectInvis),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.Recharging),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.SatisfyHunger),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.WordOfRecall),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.WordOfRecall),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.WordOfRecall),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.WordOfRecall),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.Teleport),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.Teleport),
                        new ItemIdentifier(ItemCategory.Potion, PotionType.ResStr),
                        new ItemIdentifier(ItemCategory.Potion, PotionType.ResInt),
                        new ItemIdentifier(ItemCategory.Potion, PotionType.ResWis),
                        new ItemIdentifier(ItemCategory.Potion, PotionType.ResDex),
                        new ItemIdentifier(ItemCategory.Potion, PotionType.ResCon),
                        new ItemIdentifier(ItemCategory.Potion, PotionType.ResCha),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.Identify),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.Identify),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.StarIdentify),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.StarIdentify),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.Light),
                        new ItemIdentifier(ItemCategory.Potion, PotionType.ResStr),
                        new ItemIdentifier(ItemCategory.Potion, PotionType.ResInt),
                        new ItemIdentifier(ItemCategory.Potion, PotionType.ResWis),
                        new ItemIdentifier(ItemCategory.Potion, PotionType.ResDex),
                        new ItemIdentifier(ItemCategory.Potion, PotionType.ResCon),
                        new ItemIdentifier(ItemCategory.Potion, PotionType.ResCha),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.EnchantArmor),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.EnchantArmor),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.Recharging),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.SatisfyHunger),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.SatisfyHunger),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.SatisfyHunger)
                    };

                case StoreType.StoreMagic:
                    return new[]
                    {
                        new ItemIdentifier(ItemCategory.Ring, RingType.Protection),
                        new ItemIdentifier(ItemCategory.Ring, RingType.FeatherFall),
                        new ItemIdentifier(ItemCategory.Ring, RingType.Protection),
                        new ItemIdentifier(ItemCategory.Ring, RingType.ResistFire),
                        new ItemIdentifier(ItemCategory.Ring, RingType.ResistCold),
                        new ItemIdentifier(ItemCategory.Amulet, AmuletType.Charisma),
                        new ItemIdentifier(ItemCategory.Amulet, AmuletType.SlowDigest),
                        new ItemIdentifier(ItemCategory.Amulet, AmuletType.ResistAcid),
                        new ItemIdentifier(ItemCategory.Amulet, AmuletType.Searching),
                        new ItemIdentifier(ItemCategory.Wand, WandType.SlowMonster),
                        new ItemIdentifier(ItemCategory.Wand, WandType.ConfuseMonster),
                        new ItemIdentifier(ItemCategory.Wand, WandType.SleepMonster),
                        new ItemIdentifier(ItemCategory.Wand, WandType.MagicMissile),
                        new ItemIdentifier(ItemCategory.Wand, WandType.StinkingCloud),
                        new ItemIdentifier(ItemCategory.Wand, WandType.Wonder),
                        new ItemIdentifier(ItemCategory.Wand, WandType.Disarming),
                        new ItemIdentifier(ItemCategory.Staff, StaffType.Light),
                        new ItemIdentifier(ItemCategory.Staff, StaffType.Mapping),
                        new ItemIdentifier(ItemCategory.Staff, StaffType.DetectTrap),
                        new ItemIdentifier(ItemCategory.Staff, StaffType.DetectDoor),
                        new ItemIdentifier(ItemCategory.Staff, StaffType.DetectGold),
                        new ItemIdentifier(ItemCategory.Staff, StaffType.DetectItem),
                        new ItemIdentifier(ItemCategory.Staff, StaffType.DetectInvis),
                        new ItemIdentifier(ItemCategory.Staff, StaffType.DetectEvil),
                        new ItemIdentifier(ItemCategory.Light, LightType.Orb),
                        new ItemIdentifier(ItemCategory.Staff, StaffType.Teleportation),
                        new ItemIdentifier(ItemCategory.Staff, StaffType.Teleportation),
                        new ItemIdentifier(ItemCategory.Staff, StaffType.Teleportation),
                        new ItemIdentifier(ItemCategory.Staff, StaffType.Identify),
                        new ItemIdentifier(ItemCategory.Staff, StaffType.Identify),
                        new ItemIdentifier(ItemCategory.Staff, StaffType.Identify),
                        new ItemIdentifier(ItemCategory.Staff, StaffType.Identify),
                        new ItemIdentifier(ItemCategory.Staff, StaffType.Identify),
                        new ItemIdentifier(ItemCategory.Staff, StaffType.RemoveCurse),
                        new ItemIdentifier(ItemCategory.Staff, StaffType.CureLight),
                        new ItemIdentifier(ItemCategory.Staff, StaffType.Probing),
                        new ItemIdentifier(ItemCategory.SorceryBook, 0),
                        new ItemIdentifier(ItemCategory.SorceryBook, 0),
                        new ItemIdentifier(ItemCategory.SorceryBook, 1),
                        new ItemIdentifier(ItemCategory.SorceryBook, 1), new ItemIdentifier(ItemCategory.FolkBook, 0),
                        new ItemIdentifier(ItemCategory.FolkBook, 0), new ItemIdentifier(ItemCategory.FolkBook, 1),
                        new ItemIdentifier(ItemCategory.FolkBook, 1), new ItemIdentifier(ItemCategory.FolkBook, 2),
                        new ItemIdentifier(ItemCategory.FolkBook, 2), new ItemIdentifier(ItemCategory.FolkBook, 3),
                        new ItemIdentifier(ItemCategory.FolkBook, 3)
                    };

                case StoreType.StoreLibrary:
                    return new[]
                    {
                        new ItemIdentifier(ItemCategory.SorceryBook, 0),
                        new ItemIdentifier(ItemCategory.SorceryBook, 0),
                        new ItemIdentifier(ItemCategory.SorceryBook, 1),
                        new ItemIdentifier(ItemCategory.SorceryBook, 1), new ItemIdentifier(ItemCategory.NatureBook, 0),
                        new ItemIdentifier(ItemCategory.NatureBook, 0), new ItemIdentifier(ItemCategory.NatureBook, 1),
                        new ItemIdentifier(ItemCategory.NatureBook, 1), new ItemIdentifier(ItemCategory.ChaosBook, 0),
                        new ItemIdentifier(ItemCategory.ChaosBook, 0), new ItemIdentifier(ItemCategory.ChaosBook, 1),
                        new ItemIdentifier(ItemCategory.ChaosBook, 1), new ItemIdentifier(ItemCategory.DeathBook, 0),
                        new ItemIdentifier(ItemCategory.DeathBook, 0), new ItemIdentifier(ItemCategory.DeathBook, 1),
                        new ItemIdentifier(ItemCategory.DeathBook, 1), new ItemIdentifier(ItemCategory.TarotBook, 0),
                        new ItemIdentifier(ItemCategory.TarotBook, 0), new ItemIdentifier(ItemCategory.TarotBook, 1),
                        new ItemIdentifier(ItemCategory.TarotBook, 1), new ItemIdentifier(ItemCategory.FolkBook, 0),
                        new ItemIdentifier(ItemCategory.FolkBook, 0), new ItemIdentifier(ItemCategory.FolkBook, 1),
                        new ItemIdentifier(ItemCategory.FolkBook, 1), new ItemIdentifier(ItemCategory.FolkBook, 2),
                        new ItemIdentifier(ItemCategory.FolkBook, 2), new ItemIdentifier(ItemCategory.FolkBook, 3),
                        new ItemIdentifier(ItemCategory.FolkBook, 3), new ItemIdentifier(ItemCategory.LifeBook, 0),
                        new ItemIdentifier(ItemCategory.LifeBook, 0), new ItemIdentifier(ItemCategory.LifeBook, 0),
                        new ItemIdentifier(ItemCategory.LifeBook, 0), new ItemIdentifier(ItemCategory.LifeBook, 1),
                        new ItemIdentifier(ItemCategory.LifeBook, 1), new ItemIdentifier(ItemCategory.LifeBook, 1),
                        new ItemIdentifier(ItemCategory.LifeBook, 1), new ItemIdentifier(ItemCategory.DeathBook, 0),
                        new ItemIdentifier(ItemCategory.DeathBook, 0), new ItemIdentifier(ItemCategory.DeathBook, 1),
                        new ItemIdentifier(ItemCategory.DeathBook, 1),
                        new ItemIdentifier(ItemCategory.CorporealBook, 0),
                        new ItemIdentifier(ItemCategory.CorporealBook, 0),
                        new ItemIdentifier(ItemCategory.CorporealBook, 1),
                        new ItemIdentifier(ItemCategory.CorporealBook, 1),
                        new ItemIdentifier(ItemCategory.NatureBook, 0), new ItemIdentifier(ItemCategory.NatureBook, 0),
                        new ItemIdentifier(ItemCategory.NatureBook, 1), new ItemIdentifier(ItemCategory.NatureBook, 1)
                    };

                case StoreType.StoreInn:
                    return new[]
                    {
                        new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Biscuit),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Jerky),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Jerky),
                        new ItemIdentifier(ItemCategory.Food, FoodType.PintOfWine),
                        new ItemIdentifier(ItemCategory.Food, FoodType.PintOfAle),
                        new ItemIdentifier(ItemCategory.Food, FoodType.PintOfWine),
                        new ItemIdentifier(ItemCategory.Food, FoodType.PintOfAle),
                        new ItemIdentifier(ItemCategory.Food, FoodType.PintOfWine),
                        new ItemIdentifier(ItemCategory.Food, FoodType.PintOfAle),
                        new ItemIdentifier(ItemCategory.Food, FoodType.PintOfWine),
                        new ItemIdentifier(ItemCategory.Food, FoodType.PintOfAle),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Biscuit),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Jerky),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Jerky),
                        new ItemIdentifier(ItemCategory.Food, FoodType.PintOfWine),
                        new ItemIdentifier(ItemCategory.Food, FoodType.PintOfAle),
                        new ItemIdentifier(ItemCategory.Food, FoodType.PintOfWine),
                        new ItemIdentifier(ItemCategory.Food, FoodType.PintOfAle),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Ration),
                        new ItemIdentifier(ItemCategory.Food, FoodType.PintOfWine),
                        new ItemIdentifier(ItemCategory.Food, FoodType.PintOfAle),
                        new ItemIdentifier(ItemCategory.Food, FoodType.PintOfWine),
                        new ItemIdentifier(ItemCategory.Food, FoodType.PintOfAle),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.SatisfyHunger),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.SatisfyHunger),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.SatisfyHunger),
                        new ItemIdentifier(ItemCategory.Scroll, ScrollType.SatisfyHunger),
                        new ItemIdentifier(ItemCategory.Food, FoodType.PintOfWine),
                        new ItemIdentifier(ItemCategory.Food, FoodType.PintOfAle),
                        new ItemIdentifier(ItemCategory.Food, FoodType.PintOfWine),
                        new ItemIdentifier(ItemCategory.Food, FoodType.PintOfAle)
                    };

                default:
                    return new[]
                    {
                        new ItemIdentifier(ItemCategory.None, 0), new ItemIdentifier(ItemCategory.None, 0),
                        new ItemIdentifier(ItemCategory.None, 0), new ItemIdentifier(ItemCategory.None, 0),
                        new ItemIdentifier(ItemCategory.None, 0), new ItemIdentifier(ItemCategory.None, 0),
                        new ItemIdentifier(ItemCategory.None, 0), new ItemIdentifier(ItemCategory.None, 0),
                        new ItemIdentifier(ItemCategory.None, 0), new ItemIdentifier(ItemCategory.None, 0),
                        new ItemIdentifier(ItemCategory.None, 0), new ItemIdentifier(ItemCategory.None, 0),
                        new ItemIdentifier(ItemCategory.None, 0), new ItemIdentifier(ItemCategory.None, 0),
                        new ItemIdentifier(ItemCategory.None, 0), new ItemIdentifier(ItemCategory.None, 0),
                        new ItemIdentifier(ItemCategory.None, 0), new ItemIdentifier(ItemCategory.None, 0),
                        new ItemIdentifier(ItemCategory.None, 0), new ItemIdentifier(ItemCategory.None, 0),
                        new ItemIdentifier(ItemCategory.None, 0), new ItemIdentifier(ItemCategory.None, 0),
                        new ItemIdentifier(ItemCategory.None, 0), new ItemIdentifier(ItemCategory.None, 0),
                        new ItemIdentifier(ItemCategory.None, 0), new ItemIdentifier(ItemCategory.None, 0),
                        new ItemIdentifier(ItemCategory.None, 0), new ItemIdentifier(ItemCategory.None, 0),
                        new ItemIdentifier(ItemCategory.None, 0), new ItemIdentifier(ItemCategory.None, 0),
                        new ItemIdentifier(ItemCategory.None, 0), new ItemIdentifier(ItemCategory.None, 0),
                        new ItemIdentifier(ItemCategory.None, 0), new ItemIdentifier(ItemCategory.None, 0),
                        new ItemIdentifier(ItemCategory.None, 0), new ItemIdentifier(ItemCategory.None, 0),
                        new ItemIdentifier(ItemCategory.None, 0), new ItemIdentifier(ItemCategory.None, 0),
                        new ItemIdentifier(ItemCategory.None, 0), new ItemIdentifier(ItemCategory.None, 0),
                        new ItemIdentifier(ItemCategory.None, 0), new ItemIdentifier(ItemCategory.None, 0),
                        new ItemIdentifier(ItemCategory.None, 0), new ItemIdentifier(ItemCategory.None, 0),
                        new ItemIdentifier(ItemCategory.None, 0), new ItemIdentifier(ItemCategory.None, 0),
                        new ItemIdentifier(ItemCategory.None, 0), new ItemIdentifier(ItemCategory.None, 0)
                    };
            }
        }
    }
}