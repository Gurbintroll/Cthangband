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
    internal class PlayerClassDruid : BasePlayerClass
    {
        private int[] _abilityBonus = { -1, -3, 4, -2, 0, 3 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AttacksPerTurnMax => 5;
        public override int AttacksPerTurnMinWeaponWeight => 35;
        public override int AttacksPerTurnWeightMultiplier => 3;
        public override int BaseDeviceBonus => 30;
        public override int BaseDisarmBonus => 30;
        public override int BaseMeleeAttackBonus => 48;
        public override int BaseRangedAttackBonus => 36;
        public override int BaseSaveBonus => 32;
        public override int BaseSearchBonus => 20;
        public override int BaseSearchFrequency => 8;
        public override int BaseStealthBonus => 3;
        public override bool BluntWeaponsOnly => true;
        public override CastingType CastingType => CastingType.Divine;
        public override string DescriptionLine2 => "Nature priests who use WIS based spell casting and who are";
        public override string DescriptionLine3 => "limited to the Nature realm. As priests, they can't use";
        public override string DescriptionLine4 => "edged weapons unless those weapons are holy; but they can";
        public override string DescriptionLine5 => "wear heavy armour without it disrupting their casting.";
        public override int DeviceBonusPerLevel => 10;
        public override int DisarmBonusPerLevel => 8;
        public override int ExperienceFactor => 20;
        public override Realm FirstRealmChoice => Realm.Nature;
        public override bool HasMinimumSpellFailure => false;
        public override bool HasSpells => true;
        public override int HitDieBonus => 3;
        public override int LegendaryItemBias => Enumerations.LegendaryItemBias.Priestly;
        public override int MeleeAttackBonusPerLevel => 20;
        public override int PetUpkeepDivider => 10;
        public override int PrimeAbilityScore => Ability.Wisdom;

        public override int RangedAttackBonusPerLevel => 20;

        public override int SaveBonusPerLevel => 12;

        public override int SearchBonusPerLevel => 0;

        public override int SearchFrequencyPerLevel => 0;

        public override Realm SecondRealmChoice => Realm.None;

        public override int SpellWeightLimit => 350;

        public override ItemIdentifier[] StartingItems => new[]
            {
                new ItemIdentifier(ItemCategory.SorceryBook, 0), new ItemIdentifier(ItemCategory.Hafted, HaftedType.Quarterstaff),
                new ItemIdentifier(ItemCategory.Ring, RingType.SustainWis)
            };

        public override int StealthBonusPerLevel => 0;

        public override string Title => "Druid";

        public override double SpellBaseFailureMultiplier(Realm realm)
        {
            return 0.8;
        }

        public override double SpellLevelMultiplier(Realm realm)
        {
            return 0.7;
        }

        public override double SpellVisCostMultiplier(Realm realm)
        {
            return 0.8;
        }

        public override bool TrySenseInventory(int playerLevel)
        {
            return Program.Rng.RandomLessThan(10000 / ((playerLevel * playerLevel) + 40)) == 0;
        }
    }
}