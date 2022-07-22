using Cthangband.Enumerations;
using System;

namespace Cthangband.Stores
{
    [Serializable]
    internal class MagicStore : Store
    {
        public MagicStore() : base(StoreType.StoreMagic)
        {
        }

        public override string FeatureType => "MagicShop";

        protected override ItemIdentifier[] GetStoreTable()
        {
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
        }

        protected override bool StoreWillBuy(Item item)
        {
            switch (item.Category)
            {
                case ItemCategory.SorceryBook:
                case ItemCategory.NatureBook:
                case ItemCategory.ChaosBook:
                case ItemCategory.DeathBook:
                case ItemCategory.TarotBook:
                case ItemCategory.FolkBook:
                case ItemCategory.CorporealBook:
                case ItemCategory.Amulet:
                case ItemCategory.Ring:
                case ItemCategory.Staff:
                case ItemCategory.Wand:
                case ItemCategory.Rod:
                case ItemCategory.Scroll:
                case ItemCategory.Potion:
                    return item.Value() > 0;
                default:
                    return false;
            }
        }
    }
}
