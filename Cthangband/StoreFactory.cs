// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
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
    internal static class StoreFactory
    {
        private static readonly StoreOwner[] _alchemistOwners =
        {
            new StoreOwner("Mauser the Chemist", 10000, 111, "Half Elf"),
            new StoreOwner("Wizzle the Chaotic", 10000, 110, "Hobbit"),
            new StoreOwner("Kakalrakakal", 15000, 116, "Klackon"),
            new StoreOwner("Jal-Eth the Alchemist", 15000, 111, "Elf"),
            new StoreOwner("Fanelath the Cautious", 10000, 111, "Dwarf"),
            new StoreOwner("Runcie the Insane", 10000, 110, "Human"),
            new StoreOwner("Grumbleworth", 15000, 116, "Gnome"),
            new StoreOwner("Flitter", 15000, 111, "Sprite"), new StoreOwner("Xarillus", 10000, 111, "Human"),
            new StoreOwner("Egbert the Old", 10000, 110, "Dwarf"),
            new StoreOwner("Valindra the Proud", 15000, 116, "High Elf"),
            new StoreOwner("Taen the Alchemist", 15000, 111, "Human"),
            new StoreOwner("Cayd the Sweet", 10000, 111, "Vampire"),
            new StoreOwner("Fulir the Dark", 10000, 110, "Nibelung"),
            new StoreOwner("Domli the Humble", 15000, 116, "Dwarf"),
            new StoreOwner("Yaarjukka Demonspawn", 15000, 111, "Imp"),
            new StoreOwner("Gelaraldor the Herbmaster", 10000, 111, "High Elf"),
            new StoreOwner("Olelaldan the Wise", 10000, 110, "Tcho-Tcho"),
            new StoreOwner("Fthoglo the Demonicist", 15000, 116, "Imp"),
            new StoreOwner("Dridash the Alchemist", 15000, 111, "Half Orc")
        };

        private static readonly StoreOwner[] _armouryOwners =
        {
            new StoreOwner("Kon-Dar the Ugly", 10000, 115, "Half Orc"),
            new StoreOwner("Darg-Low the Grim", 15000, 111, "Human"),
            new StoreOwner("Decado the Handsome", 25000, 112, "Great One"),
            new StoreOwner("Elo Dragonscale", 30000, 112, "Elf"),
            new StoreOwner("Delicatus", 10000, 115, "Sprite"),
            new StoreOwner("Gruce the Huge", 15000, 111, "Half Giant"),
            new StoreOwner("Animus", 25000, 112, "Golem"), new StoreOwner("Malvus", 30000, 112, "Half Titan"),
            new StoreOwner("Selaxis", 10000, 115, "Zombie"),
            new StoreOwner("Deathchill", 15000, 111, "Spectre"),
            new StoreOwner("Drios the Faint", 25000, 112, "Spectre"),
            new StoreOwner("Bathric the Cold", 30000, 112, "Vampire"),
            new StoreOwner("Vengella the Cruel", 10000, 115, "Half Troll"),
            new StoreOwner("Wyrana the Mighty", 15000, 111, "Human"),
            new StoreOwner("Yojo II", 25000, 112, "Dwarf"),
            new StoreOwner("Ranalar the Sweet", 30000, 112, "Great One"),
            new StoreOwner("Horbag the Unclean", 10000, 115, "Half Orc"),
            new StoreOwner("Elelen the Telepath", 15000, 111, "Dark Elf"),
            new StoreOwner("Isedrelias", 25000, 112, "Sprite"),
            new StoreOwner("Vegnar One-eye", 30000, 112, "Cyclops"),
            new StoreOwner("Rodish the Chaotic", 10000, 115, "Miri Nigri"),
            new StoreOwner("Hesin Swordmaster", 15000, 111, "Nibelung"),
            new StoreOwner("Elvererith the Cheat", 25000, 112, "Dark Elf"),
            new StoreOwner("Zzathath the Imp", 30000, 112, "Imp")
        };

        private static readonly StoreOwner[] _blackMarketOwners =
        {
            new StoreOwner("Vhassa the Dead", 20000, 150, "Zombie"),
            new StoreOwner("Kyn the Treacherous", 20000, 150, "Vampire"),
            new StoreOwner("Batrachian Belle", 30000, 150, "Miri Nigri"),
            new StoreOwner("Corpselight", 30000, 150, "Spectre"),
            new StoreOwner("Parrish the Bloodthirsty", 20000, 150, "Vampire"),
            new StoreOwner("Vile", 20000, 150, "Skeleton"),
            new StoreOwner("Prentice the Trusted", 30000, 150, "Skeleton"),
            new StoreOwner("Griella Humanslayer", 30000, 150, "Imp"),
            new StoreOwner("Charity the Necromancer", 20000, 150, "Dark Elf"),
            new StoreOwner("Pugnacious the Pugilist", 20000, 150, "Half Orc"),
            new StoreOwner("Footsore the Lucky", 30000, 150, "Miri Nigri"),
            new StoreOwner("Sidria Lighfingered", 30000, 150, "Human"),
            new StoreOwner("Riatho the Juggler", 20000, 150, "Hobbit"),
            new StoreOwner("Janaaka the Shifty", 20000, 150, "Gnome"),
            new StoreOwner("Cina the Rogue", 30000, 150, "Gnome"),
            new StoreOwner("Arunikki Greatclaw", 30000, 150, "Draconian"),
            new StoreOwner("Chaeand the Poor", 20000, 150, "Human"),
            new StoreOwner("Afardorf the Brigand", 20000, 150, "Tcho-Tcho"),
            new StoreOwner("Lathaxl the Greedy", 30000, 150, "Mind Flayer"),
            new StoreOwner("Falarewyn", 30000, 150, "Sprite")
        };

        private static readonly StoreOwner[] _emptyStoreOwners = { new StoreOwner("Empty lot", 0, 100, string.Empty) };

        private static readonly StoreOwner[] _generalStoreOwners =
        {
            new StoreOwner("Falilmawen the Friendly", 250, 108, "Hobbit"),
            new StoreOwner("Voirin the Cowardly", 500, 108, "Human"),
            new StoreOwner("Erashnak the Midget", 750, 107, "Miri Nigri"),
            new StoreOwner("Grug the Comely", 1000, 107, "Half Titan"),
            new StoreOwner("Forovir the Cheap", 250, 108, "Human"),
            new StoreOwner("Ellis the Fool", 500, 108, "Human"),
            new StoreOwner("Filbert the Hungry", 750, 107, "Vampire"),
            new StoreOwner("Fthnargl Psathiggua", 1000, 107, "Mind Flayer"),
            new StoreOwner("Eloise Long-Dead", 250, 108, "Spectre"),
            new StoreOwner("Fundi the Slow", 500, 108, "Zombie"),
            new StoreOwner("Granthus", 750, 107, "Skeleton"),
            new StoreOwner("Lorax the Suave", 1000, 107, "Vampire"),
            new StoreOwner("Butch", 250, 108, "Half Orc"),
            new StoreOwner("Elbereth the Beautiful", 500, 108, "High Elf"),
            new StoreOwner("Sarleth the Sneaky", 750, 107, "Gnome"),
            new StoreOwner("Narlock", 1000, 107, "Dwarf"),
            new StoreOwner("Haneka the Small", 250, 108, "Gnome"),
            new StoreOwner("Loirin the Mad", 500, 108, "Half Giant"),
            new StoreOwner("Wuto Poisonbreath", 750, 107, "Draconian"),
            new StoreOwner("Araaka the Rotund", 1000, 107, "Draconian"),
            new StoreOwner("Poogor the Dumb", 250, 108, "Miri Nigri"),
            new StoreOwner("Felorfiliand", 500, 108, "Elf"),
            new StoreOwner("Maroka the Aged", 750, 107, "Gnome"),
            new StoreOwner("Sasin the Bold", 1000, 107, "Half Giant"),
            new StoreOwner("Abiemar the Peasant", 250, 108, "Human"),
            new StoreOwner("Hurk the Poor", 500, 108, "Half Orc"),
            new StoreOwner("Soalin the Wretched", 750, 107, "Zombie"),
            new StoreOwner("Merulla the Humble", 1000, 107, "Elf")
        };

        private static readonly StoreOwner[] _hallOwners = { new StoreOwner("Hall of Records", 0, 100, string.Empty) };
        private static readonly StoreOwner[] _homeOwners = { new StoreOwner("Your home", 0, 100, string.Empty) };

        private static readonly StoreOwner[] _innOwners =
        {
            new StoreOwner("Mordsan", 15000, 108, "Human"),
            new StoreOwner("Furfoot Pobber", 20000, 105, "Hobbit"),
            new StoreOwner("Oddo Yeekson", 25000, 110, "Yeek"),
            new StoreOwner("Dry-Bones", 30000, 105, "Skeleton"),
            new StoreOwner("Kleibons", 15000, 108, "Klackon"),
            new StoreOwner("Prendegast", 20000, 105, "Hobbit"),
            new StoreOwner("Straasha", 25000, 110, "Draconian"),
            new StoreOwner("Allia the Servile", 30000, 105, "Human"),
            new StoreOwner("Lumin the Blue", 15000, 108, "Spectre"),
            new StoreOwner("Short Al", 20000, 105, "Zombie"),
            new StoreOwner("Silent Faldus", 25000, 110, "Zombie"),
            new StoreOwner("Quirmby the Strange", 30000, 105, "Vampire"),
            new StoreOwner("Aldous the Sleepy", 15000, 108, "Human"),
            new StoreOwner("Grundy the Tall", 20000, 105, "Hobbit"),
            new StoreOwner("Gobbleguts Thunderbreath", 25000, 110, "Half Troll"),
            new StoreOwner("Silverscale", 30000, 105, "Draconian"),
            new StoreOwner("Etheraa the Furious", 15000, 108, "Tcho-Tcho"),
            new StoreOwner("Paetlan the Alcoholic", 20000, 105, "Human"),
            new StoreOwner("Drang", 25000, 110, "Half Ogre"),
            new StoreOwner("Barbag the Sly", 30000, 105, "Kobold"),
            new StoreOwner("Kirakak", 15000, 108, "Klackon"),
            new StoreOwner("Nafur the Wooden", 20000, 105, "Golem"),
            new StoreOwner("Grarak the Hospitable", 25000, 110, "Half Giant"),
            new StoreOwner("Lona the Charismatic", 30000, 105, "Gnome"),
            new StoreOwner("Crediric the Brewer", 15000, 108, "Human"),
            new StoreOwner("Nydudus the Slow", 20000, 105, "Zombie"),
            new StoreOwner("Baurk the Busy", 25000, 110, "Yeek"),
            new StoreOwner("Seviras the Mindcrafter", 30000, 105, "Human")
        };

        private static readonly StoreOwner[] _libraryOwners =
        {
            new StoreOwner("Randolph Carter", 15000, 108, "Human"),
            new StoreOwner("Odnar the Sage", 20000, 105, "High Elf"),
            new StoreOwner("Gandar the Neutral", 25000, 110, "Vampire"),
            new StoreOwner("Ro-sha the Patient", 30000, 105, "Golem"),
            new StoreOwner("Sarai the Swift", 15000, 108, "Human"),
            new StoreOwner("Bodril the Seer", 20000, 105, "High Elf"),
            new StoreOwner("Veloin the Quiet", 25000, 110, "Zombie"),
            new StoreOwner("Vanthylas the Learned", 30000, 105, "Mind Flayer"),
            new StoreOwner("Ossein the Literate", 15000, 108, "Skeleton"),
            new StoreOwner("Olvar Bookworm", 20000, 105, "Vampire"),
            new StoreOwner("Shallowgrave", 25000, 110, "Zombie"),
            new StoreOwner("Death Mask", 30000, 105, "Zombie"),
            new StoreOwner("Porcina the Obese", 15000, 108, "Half Orc"),
            new StoreOwner("Glaruna Brandybreath", 20000, 105, "Dwarf"),
            new StoreOwner("Furface Yeek", 25000, 110, "Yeek"),
            new StoreOwner("Bald Oggin", 30000, 105, "Gnome"),
            new StoreOwner("Asuunu the Learned", 15000, 108, "Mind Flayer"),
            new StoreOwner("Prirand the Dead", 20000, 105, "Zombie"),
            new StoreOwner("Ronar the Iron", 25000, 110, "Golem"),
            new StoreOwner("Galil-Gamir", 30000, 105, "Elf"),
            new StoreOwner("Rorbag Book-Eater", 15000, 108, "Kobold"),
            new StoreOwner("Kiriarikirk", 20000, 105, "Klackon"),
            new StoreOwner("Rilin the Quiet", 25000, 110, "Dwarf"),
            new StoreOwner("Isung the Lord", 30000, 105, "High Elf")
        };

        private static readonly StoreOwner[] _magicShopOwners =
        {
            new StoreOwner("Skidney the Sorcerer", 15000, 110, "Half Elf"),
            new StoreOwner("Buggerby the Great", 20000, 113, "Gnome"),
            new StoreOwner("Kyria the Illusionist", 30000, 110, "Human"),
            new StoreOwner("Nikki the Necromancer", 30000, 110, "Dark Elf"),
            new StoreOwner("Solostoran", 15000, 110, "Sprite"),
            new StoreOwner("Achshe the Tentacled", 20000, 113, "Mind Flayer"),
            new StoreOwner("Kaza the Noble", 30000, 110, "High Elf"),
            new StoreOwner("Fazzil the Dark", 30000, 110, "Dark Elf"),
            new StoreOwner("Angel", 20000, 150, "Vampire"),
            new StoreOwner("Flotsam the Bloated", 20000, 150, "Zombie"),
            new StoreOwner("Nieval", 30000, 150, "Vampire"),
            new StoreOwner("Anastasia the Luminous", 30000, 150, "Spectre"),
            new StoreOwner("Keldorn the Grand", 15000, 110, "Dwarf"),
            new StoreOwner("Philanthropus", 20000, 113, "Hobbit"),
            new StoreOwner("Agnar the Enchantress", 30000, 110, "Human"),
            new StoreOwner("Buliance the Necromancer", 30000, 110, "Miri Nigri"),
            new StoreOwner("Vuirak the High-Mage", 15000, 110, "Miri Nigri"),
            new StoreOwner("Madish the Smart", 20000, 113, "Miri Nigri"),
            new StoreOwner("Falebrimbor", 30000, 110, "High Elf"),
            new StoreOwner("Felil-Gand the Subtle", 30000, 110, "Dark Elf"),
            new StoreOwner("Thalegord the Shaman", 15000, 110, "Tcho-Tcho"),
            new StoreOwner("Cthoaloth the Mystic", 20000, 113, "Mind Flayer"),
            new StoreOwner("Ibeli the Illusionist", 30000, 110, "Skeleton"),
            new StoreOwner("Heto the Necromancer", 30000, 110, "Yeek")
        };

        private static readonly StoreOwner[] _pawnbrokerOwners =
        {
            new StoreOwner("Magd the Ruthless", 2000, 100, "Human"),
            new StoreOwner("Drako Fairdeal", 4000, 100, "Draconian"),
            new StoreOwner("Featherwing", 5000, 100, "Sprite"),
            new StoreOwner("Xochinaggua", 10000, 100, "Mind Flayer"),
            new StoreOwner("Od the Penniless", 2000, 100, "Elf"), new StoreOwner("Xax", 4000, 100, "Golem"),
            new StoreOwner("Jake Small", 5000, 100, "Half Giant"),
            new StoreOwner("Helga the Lost", 10000, 100, "Human"),
            new StoreOwner("Gloom the Phlegmatic", 2000, 100, "Zombie"),
            new StoreOwner("Quick-Arm Vollaire", 4000, 100, "Vampire"),
            new StoreOwner("Asenath", 5000, 100, "Zombie"),
            new StoreOwner("Lord Filbert", 10000, 100, "Vampire"),
            new StoreOwner("Herranyth the Ruthless", 2000, 100, "Human"),
            new StoreOwner("Gagrin Moneylender", 4000, 100, "Yeek"),
            new StoreOwner("Thrambor the Grubby", 5000, 100, "Half Elf"),
            new StoreOwner("Derigrin the Honest", 10000, 100, "Hobbit"),
            new StoreOwner("Munk the Barterer", 2000, 100, "Half Ogre"),
            new StoreOwner("Gadrialdur the Fair", 4000, 100, "Half Elf"),
            new StoreOwner("Ninar the Stooped", 5000, 100, "Dwarf"),
            new StoreOwner("Adirath the Unmagical", 10000, 100, "Tcho-Tcho")
        };

        private static readonly StoreOwner[] _templeOwners =
        {
            new StoreOwner("Ludwig the Humble", 10000, 109, "Dwarf"),
            new StoreOwner("Gunnar the Paladin", 15000, 110, "Half Troll"),
            new StoreOwner("Sir Parsival the Pure", 25000, 107, "High Elf"),
            new StoreOwner("Asenath the Holy", 30000, 109, "Human"),
            new StoreOwner("McKinnon", 10000, 109, "Human"),
            new StoreOwner("Mistress Chastity", 15000, 110, "High Elf"),
            new StoreOwner("Hashnik the Druid", 25000, 107, "Hobbit"),
            new StoreOwner("Finak", 30000, 109, "Yeek"), new StoreOwner("Krikkik", 10000, 109, "Klackon"),
            new StoreOwner("Morival the Wild", 15000, 110, "Elf"),
            new StoreOwner("Hoshak the Dark", 25000, 107, "Imp"),
            new StoreOwner("Atal the Wise", 30000, 109, "Human"),
            new StoreOwner("Ibenidd the Chaste", 10000, 109, "Human"),
            new StoreOwner("Eridish", 15000, 110, "Half Troll"),
            new StoreOwner("Vrudush the Shaman", 25000, 107, "Half Ogre"),
            new StoreOwner("Haob the Berserker", 30000, 109, "Tcho-Tcho"),
            new StoreOwner("Proogdish the Youthfull", 10000, 109, "Half Ogre"),
            new StoreOwner("Lumwise the Mad", 15000, 110, "Yeek"),
            new StoreOwner("Muirt the Virtuous", 25000, 107, "Kobold"),
            new StoreOwner("Dardobard the Weak", 30000, 109, "Spectre")
        };

        private static readonly StoreOwner[] _weaponsmithOwners =
        {
            new StoreOwner("Arnold the Beastly", 10000, 115, "Tcho-Tcho"),
            new StoreOwner("Arndal Beast-Slayer", 15000, 110, "Half Elf"),
            new StoreOwner("Edor the Short", 25000, 115, "Hobbit"),
            new StoreOwner("Oglign Dragon-Slayer", 30000, 112, "Dwarf"),
            new StoreOwner("Drew the Skilled", 10000, 115, "Human"),
            new StoreOwner("Orrax Dragonson", 15000, 110, "Draconian"),
            new StoreOwner("Bob", 25000, 115, "Miri Nigri"),
            new StoreOwner("Arkhoth the Stout", 30000, 112, "Dwarf"),
            new StoreOwner("Sarlyas the Rotten", 10000, 115, "Zombie"),
            new StoreOwner("Tuethic Bare-Bones", 15000, 110, "Skeleton"),
            new StoreOwner("Bilious the Toad", 25000, 115, "Miri Nigri"),
            new StoreOwner("Fasgul", 30000, 112, "Zombie"),
            new StoreOwner("Ellefris the Paladin", 10000, 115, "Tcho-Tcho"),
            new StoreOwner("K'trrik'k", 15000, 110, "Klackon"),
            new StoreOwner("Drocus Spiderfriend", 25000, 115, "Dark Elf"),
            new StoreOwner("Fungus Giant-Slayer", 30000, 112, "Dwarf"),
            new StoreOwner("Nadoc the Strong", 10000, 115, "Hobbit"),
            new StoreOwner("Eramog the Weak", 15000, 110, "Kobold"),
            new StoreOwner("Eowilith the Fair", 25000, 115, "Vampire"),
            new StoreOwner("Huimog Balrog-Slayer", 30000, 112, "Half Orc"),
            new StoreOwner("Peadus the Cruel", 10000, 115, "Human"),
            new StoreOwner("Vamog Slayer", 15000, 110, "Half Ogre"),
            new StoreOwner("Hooshnak the Vicious", 25000, 115, "Miri Nigri"),
            new StoreOwner("Balenn War-Dancer", 30000, 112, "Tcho-Tcho")
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
                        new ItemIdentifier(ItemCategory.Food, FoodType.Venison),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Venison),
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
                        new ItemIdentifier(ItemCategory.Digging, DiggerType.Shovel),
                        new ItemIdentifier(ItemCategory.Digging, DiggerType.Pick),
                        new ItemIdentifier(ItemCategory.Cloak, CloakType.Cloak),
                        new ItemIdentifier(ItemCategory.Cloak, CloakType.Cloak),
                        new ItemIdentifier(ItemCategory.Cloak, CloakType.Cloak),
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
                        new ItemIdentifier(ItemCategory.Digging, DiggerType.Shovel)
                    };

                case StoreType.StoreArmoury:
                    return new[]
                    {
                        new ItemIdentifier(ItemCategory.Boots, BootsType.SoftLeather),
                        new ItemIdentifier(ItemCategory.Boots, BootsType.SoftLeather),
                        new ItemIdentifier(ItemCategory.Boots, BootsType.HardLeather),
                        new ItemIdentifier(ItemCategory.Boots, BootsType.HardLeather),
                        new ItemIdentifier(ItemCategory.Helm, HelmType.LeatherCap),
                        new ItemIdentifier(ItemCategory.Helm, HelmType.LeatherCap),
                        new ItemIdentifier(ItemCategory.Helm, HelmType.MetalCap),
                        new ItemIdentifier(ItemCategory.Helm, HelmType.IronHelm),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.Robe),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.Robe),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SoftLeather),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SoftLeather),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.HardLeather),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.HardLeather),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.HardStuddedLeather),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.HardStuddedLeather),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.LeatherScale),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.LeatherScale),
                        new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.MetalScaleMail),
                        new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.ChainMail),
                        new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.ChainMail),
                        new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.AugmentedChainMail),
                        new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.BarChainMail),
                        new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.DoubleChainMail),
                        new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.MetalBrigandine),
                        new ItemIdentifier(ItemCategory.Gloves, GlovesType.LeatherGloves),
                        new ItemIdentifier(ItemCategory.Gloves, GlovesType.LeatherGloves),
                        new ItemIdentifier(ItemCategory.Gloves, GlovesType.Gauntlets),
                        new ItemIdentifier(ItemCategory.Shield, ShieldType.SmallLeather),
                        new ItemIdentifier(ItemCategory.Shield, ShieldType.SmallLeather),
                        new ItemIdentifier(ItemCategory.Shield, ShieldType.LargeLeather),
                        new ItemIdentifier(ItemCategory.Shield, ShieldType.SmallMetal),
                        new ItemIdentifier(ItemCategory.Boots, BootsType.HardLeather),
                        new ItemIdentifier(ItemCategory.Boots, BootsType.HardLeather),
                        new ItemIdentifier(ItemCategory.Helm, HelmType.LeatherCap),
                        new ItemIdentifier(ItemCategory.Helm, HelmType.LeatherCap),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.Robe),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SoftLeather),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SoftLeather),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.HardLeather),
                        new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.LeatherScale),
                        new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.MetalScaleMail),
                        new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.ChainMail),
                        new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.ChainMail),
                        new ItemIdentifier(ItemCategory.Gloves, GlovesType.LeatherGloves),
                        new ItemIdentifier(ItemCategory.Gloves, GlovesType.Gauntlets),
                        new ItemIdentifier(ItemCategory.Shield, ShieldType.SmallLeather),
                        new ItemIdentifier(ItemCategory.Shield, ShieldType.SmallLeather)
                    };

                case StoreType.StoreWeapon:
                    return new[]
                    {
                        new ItemIdentifier(ItemCategory.Sword, SwordType.Dagger),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.MainGauche),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.Rapier),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.SmallSword),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.ShortSword),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.Sabre),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.Cutlass),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.Tulwar),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.BroadSword),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.LongSword),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.Scimitar),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.Katana),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.BastardSword),
                        new ItemIdentifier(ItemCategory.Polearm, PolearmType.Spear),
                        new ItemIdentifier(ItemCategory.Polearm, PolearmType.AwlPike),
                        new ItemIdentifier(ItemCategory.Polearm, PolearmType.Trident),
                        new ItemIdentifier(ItemCategory.Polearm, PolearmType.Pike),
                        new ItemIdentifier(ItemCategory.Polearm, PolearmType.BeakedAxe),
                        new ItemIdentifier(ItemCategory.Polearm, PolearmType.BroadAxe),
                        new ItemIdentifier(ItemCategory.Polearm, PolearmType.Lance),
                        new ItemIdentifier(ItemCategory.Polearm, PolearmType.BattleAxe),
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.Whip),
                        new ItemIdentifier(ItemCategory.Bow, BowType.Sling),
                        new ItemIdentifier(ItemCategory.Bow, BowType.Shortbow),
                        new ItemIdentifier(ItemCategory.Bow, BowType.Longbow),
                        new ItemIdentifier(ItemCategory.Bow, BowType.LightCrossbow),
                        new ItemIdentifier(ItemCategory.Shot, AmmoType.SvAmmoNormal),
                        new ItemIdentifier(ItemCategory.Shot, AmmoType.SvAmmoNormal),
                        new ItemIdentifier(ItemCategory.Arrow, AmmoType.SvAmmoNormal),
                        new ItemIdentifier(ItemCategory.Arrow, AmmoType.SvAmmoNormal),
                        new ItemIdentifier(ItemCategory.Bolt, AmmoType.SvAmmoNormal),
                        new ItemIdentifier(ItemCategory.Bolt, AmmoType.SvAmmoNormal),
                        new ItemIdentifier(ItemCategory.Bow, BowType.Longbow),
                        new ItemIdentifier(ItemCategory.Bow, BowType.LightCrossbow),
                        new ItemIdentifier(ItemCategory.Arrow, AmmoType.SvAmmoNormal),
                        new ItemIdentifier(ItemCategory.Arrow, AmmoType.SvAmmoNormal),
                        new ItemIdentifier(ItemCategory.Bolt, AmmoType.SvAmmoNormal),
                        new ItemIdentifier(ItemCategory.Bolt, AmmoType.SvAmmoNormal),
                        new ItemIdentifier(ItemCategory.Bow, BowType.Shortbow),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.Dagger),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.MainGauche),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.Rapier),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.SmallSword),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.ShortSword),
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.Whip),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.BroadSword),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.LongSword),
                        new ItemIdentifier(ItemCategory.Sword, SwordType.Scimitar)
                    };

                case StoreType.StoreTemple:
                    return new[]
                    {
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.Whip),
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.Quarterstaff),
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.Mace),
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.BallAndChain),
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.WarHammer),
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.LucernHammer),
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.MorningStar),
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.Flail),
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.LeadFilledMace),
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
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.Whip),
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.Mace),
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.BallAndChain),
                        new ItemIdentifier(ItemCategory.Hafted, HaftedType.WarHammer),
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
                        new ItemIdentifier(ItemCategory.Food, FoodType.Venison),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Venison),
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
                        new ItemIdentifier(ItemCategory.Food, FoodType.Venison),
                        new ItemIdentifier(ItemCategory.Food, FoodType.Venison),
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