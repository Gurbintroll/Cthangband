// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.PlayerClass.Base;
using System;

namespace Cthangband.PlayerClass
{
    [Serializable]
    internal class PlayerClassMonk : BasePlayerClass
    {
        private int[] _abilityBonus = { 2, -1, 1, 3, 2, 1 };
        public override int[] AbilityBonus => _abilityBonus;

        public override int AttacksPerTurnMax => 4;

        public override int AttacksPerTurnMinWeaponWeight => 40;

        public override int AttacksPerTurnWeightMultiplier => 3;

        public override int BaseDeviceBonus => 32;

        public override int BaseDisarmBonus => 45;

        public override int BaseMeleeAttackBonus => 64;

        public override int BaseRangedAttackBonus => 50;

        public override int BaseSaveBonus => 28;

        public override int BaseSearchBonus => 32;

        public override int BaseSearchFrequency => 24;

        public override int BaseStealthBonus => 5;

        public override CastingType CastingType => CastingType.Divine;

        public override string DescriptionLine2 => "Masters of unarmed combat. While wearing only light armour";

        public override string DescriptionLine3 => "they can move faster and dodge blows and can learn to";

        public override string DescriptionLine4 => "resist paralysis (at lvl 25). While not wielding a weapon";

        public override string DescriptionLine5 => "they have extra attacks and do increased damage. They are";

        public override string DescriptionLine6 => "WIS based casters using Chaos, Tarot or Corporeal magic.";

        public override int DeviceBonusPerLevel => 12;

        public override int DisarmBonusPerLevel => 15;

        public override int ExperienceFactor => 40;

        public override Realm FirstRealmChoice => Realm.Corporeal | Realm.Tarot | Realm.Chaos;

        public override bool HasSpells => true;

        public override int HitDieBonus => 6;

        public override bool IsMartialArtist => true;

        public override int LegendaryItemBias => Enumerations.LegendaryItemBias.Priestly;

        public override int MeleeAttackBonusPerLevel => 40;

        public override int PrimeAbilityScore => Ability.Dexterity;

        public override int RangedAttackBonusPerLevel => 30;

        public override int SaveBonusPerLevel => 10;

        public override int SearchBonusPerLevel => 0;

        public override int SearchFrequencyPerLevel => 0;

        public override Realm SecondRealmChoice => Realm.None;

        public override int SpellWeightLimit => 300;

        public override ItemIdentifier[] StartingItems => new[]
            {
                new ItemIdentifier(ItemCategory.SorceryBook, 0), new ItemIdentifier(ItemCategory.Potion, PotionType.Healing),
                new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SoftLeather)
            };

        public override int StealthBonusPerLevel => 0;

        public override string Title => "Monk";

        public override void ApplyClassStatus(Player player)
        {
            if (player.Level > 24 && !MartialArtistHeavyArmour(player))
            {
                player.HasFreeAction = true;
            }
        }

        public override void ApplyMartialArtistArmourClass(Player player)
        {
            if (MartialArtistHeavyArmour(player))
            {
                return;
            }
            if (player.Inventory[InventorySlot.Body].ItemType == null)
            {
                player.ArmourClassBonus += player.Level * 3 / 2;
                player.DisplayedArmourClassBonus += player.Level * 3 / 2;
            }
            if (player.Inventory[InventorySlot.Cloak].ItemType == null && player.Level > 15)
            {
                player.ArmourClassBonus += (player.Level - 13) / 3;
                player.DisplayedArmourClassBonus += (player.Level - 13) / 3;
            }
            if (player.Inventory[InventorySlot.Arm].ItemType == null && player.Level > 10)
            {
                player.ArmourClassBonus += (player.Level - 8) / 3;
                player.DisplayedArmourClassBonus += (player.Level - 8) / 3;
            }
            if (player.Inventory[InventorySlot.Head].ItemType == null && player.Level > 4)
            {
                player.ArmourClassBonus += (player.Level - 2) / 3;
                player.DisplayedArmourClassBonus += (player.Level - 2) / 3;
            }
            if (player.Inventory[InventorySlot.Hands].ItemType == null)
            {
                player.ArmourClassBonus += player.Level / 2;
                player.DisplayedArmourClassBonus += player.Level / 2;
            }
            if (player.Inventory[InventorySlot.Feet].ItemType == null)
            {
                player.ArmourClassBonus += player.Level / 3;
                player.DisplayedArmourClassBonus += player.Level / 3;
            }
        }

        public override string ClassSubName(Realm realm)
        {
            switch (realm)
            {
                case Realm.Corporeal:
                    return "Ascetic";

                case Realm.Tarot:
                    return "Ninja";

                case Realm.Chaos:
                    return "Street Fighter";

                default:
                    return "Monk";
            }
        }

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            if (player.Level > 9 && !MartialArtistHeavyArmour(player))
            {
                f1.Set(ItemFlag1.Speed);
            }
            if (player.Level > 24 && !MartialArtistHeavyArmour(player))
            {
                f2.Set(ItemFlag2.FreeAct);
            }
        }

        public override bool HasSpeedBonus(Player player)
        {
            return !MartialArtistHeavyArmour(player);
        }

        public override bool MartialArtistHeavyArmour(Player player)
        {
            var martialArtistArmWgt = 0;
            martialArtistArmWgt += player.Inventory[InventorySlot.Body].Weight;
            martialArtistArmWgt += player.Inventory[InventorySlot.Head].Weight;
            martialArtistArmWgt += player.Inventory[InventorySlot.Arm].Weight;
            martialArtistArmWgt += player.Inventory[InventorySlot.Cloak].Weight;
            martialArtistArmWgt += player.Inventory[InventorySlot.Hands].Weight;
            martialArtistArmWgt += player.Inventory[InventorySlot.Feet].Weight;
            return martialArtistArmWgt > 100 + (player.Level * 4);
        }

        public override double SpellBaseFailureMultiplier(Realm realm)
        {
            return 1.1;
        }

        public override double SpellLevelMultiplier(Realm realm)
        {
            return 1.1;
        }

        public override double SpellVisCostMultiplier(Realm realm)
        {
            return 1.3;
        }

        public override bool TrySenseInventory(int playerLevel)
        {
            return Program.Rng.RandomLessThan(20000 / ((playerLevel * playerLevel) + 40)) == 0;
        }
    }
}