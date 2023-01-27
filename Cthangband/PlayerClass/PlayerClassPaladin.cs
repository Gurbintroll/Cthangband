// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.PlayerClass.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cthangband.PlayerClass
{
    internal class PlayerClassPaladin : BasePlayerClass
    {
        private int[] _abilityBonus = { 3, -3, 1, 0, 2, 2 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AttacksPerTurnMax => 5;
        public override int AttacksPerTurnMinWeaponWeight => 30;
        public override int AttacksPerTurnWeightMultiplier => 4;
        public override int BaseDeviceBonus => 24;
        public override int BaseDisarmBonus => 20;
        public override int BaseMeleeAttackBonus => 68;
        public override int BaseRangedAttackBonus => 40;
        public override int BaseSaveBonus => 26;
        public override int BaseSearchBonus => 12;
        public override int BaseSearchFrequency => 2;
        public override int BaseStealthBonus => 1;
        public override CastingType CastingType => CastingType.Divine;
        public override string DescriptionLine2 => "Holy warriors who use WIS based spell casting to supplement";
        public override string DescriptionLine3 => "their fighting skills. Paladins can specialise in either";
        public override string DescriptionLine4 => "Life or Death magic, but their spell casting is weak in";
        public override string DescriptionLine5 => "comparison to a full priest. Paladins learn to resist fear";
        public override string DescriptionLine6 => "(at lvl 40).";
        public override int DeviceBonusPerLevel => 10;
        public override int DisarmBonusPerLevel => 7;
        public override int ExperienceFactor => 35;
        public override Realm FirstRealmChoice => Realm.Life | Realm.Death;
        public override bool HasDetailedSenseInventory => true;
        public override bool HasSpells => true;

        public override int HitDieBonus => 6;

        public override int LegendaryItemBias => Enumerations.LegendaryItemBias.Priestly;

        public override int MeleeAttackBonusPerLevel => 35;

        public override int PrimeAbilityScore => Ability.Wisdom;

        public override int RangedAttackBonusPerLevel => 30;

        public override int SaveBonusPerLevel => 11;

        public override int SearchBonusPerLevel => 0;

        public override int SearchFrequencyPerLevel => 0;

        public override Realm SecondRealmChoice => Realm.None;

        public override int SpellWeightLimit => 400;

        public override ItemIdentifier[] StartingItems => new[]
            {
                new ItemIdentifier(ItemCategory.SorceryBook, 0), new ItemIdentifier(ItemCategory.Sword, SwordType.BroadSword),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.ProtectionFromEvil)
            };

        public override int StealthBonusPerLevel => 0;

        public override string Title => "Paladin";

        public override int WarriorArtifactBias => 40;

        public override void ApplyClassStatus(Player player)
        {
            if (player.Level > 39)
            {
                player.HasFearResistance = true;
            }
        }

        public override string ClassSubName(Realm realm)
        {
            return realm == Realm.Death ? "Death Knight" : "Paladin";
        }

        public override bool ExperienceForDestroying(Player player, ItemCategory category)
        {
            return player.Realm1 == Realm.Life ? category == ItemCategory.DeathBook : category == ItemCategory.LifeBook;
        }

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            if (player.Level > 39)
            {
                f2.Set(ItemFlag2.ResFear);
            }
        }

        public override double SpellBaseFailureMultiplier(Realm realm)
        {
            return 1;
        }

        public override double SpellLevelMultiplier(Realm realm)
        {
            return 1;
        }

        public override double SpellVisCostMultiplier(Realm realm)
        {
            return 1.1;
        }

        public override bool TrySenseInventory(int playerLevel)
        {
            return Program.Rng.RandomLessThan(77777 / ((playerLevel * playerLevel) + 40)) == 0;
        }
    }
}