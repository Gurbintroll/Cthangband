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
    internal class PlayerClassPriest : BasePlayerClass
    {
        private int[] _abilityBonus = { -1, -3, 3, -1, 0, 2 };
        public override int[] AbilityBonus => _abilityBonus;

        public override int AttacksPerTurnMax => 5;
        public override int AttacksPerTurnMinWeaponWeight => 35;
        public override int AttacksPerTurnWeightMultiplier => 3;
        public override int BaseDeviceBonus => 30;
        public override int BaseDisarmBonus => 25;
        public override int BaseMeleeAttackBonus => 48;
        public override int BaseRangedAttackBonus => 36;
        public override int BaseSaveBonus => 32;
        public override int BaseSearchBonus => 16;
        public override int BaseSearchFrequency => 8;
        public override int BaseStealthBonus => 2;
        public override bool BluntWeaponsOnly => true;
        public override CastingType CastingType => CastingType.Divine;
        public override string DescriptionLine2 => "Devout followers of the Great Ones, Priests use WIS based";
        public override string DescriptionLine3 => "spell casting. They may choose either Life or Death magic,";
        public override string DescriptionLine4 => "and another realm of their choice. Priests can't use edged";
        public override string DescriptionLine5 => "weapons unless they are blessed, but can use any armour.";
        public override int DeviceBonusPerLevel => 10;
        public override int DisarmBonusPerLevel => 7;
        public override int ExperienceFactor => 20;
        public override Realm FirstRealmChoice => Realm.Life | Realm.Death;
        public override bool HasDeity => true;
        public override bool HasMinimumSpellFailure => false;
        public override bool HasSpells => true;

        public override int HitDieBonus => 2;

        public override int LegendaryItemBias => Enumerations.LegendaryItemBias.Priestly;

        public override int MeleeAttackBonusPerLevel => 20;

        public override int PrimeAbilityScore => Ability.Wisdom;

        public override int RangedAttackBonusPerLevel => 20;

        public override int SaveBonusPerLevel => 12;

        public override int SearchBonusPerLevel => 0;

        public override int SearchFrequencyPerLevel => 0;

        public override Realm SecondRealmChoice => Realm.Nature | Realm.Chaos | Realm.Tarot | Realm.Folk | Realm.Corporeal;

        public override int SpellWeightLimit => 350;

        public override ItemIdentifier[] StartingItems => new[]
            {
                new ItemIdentifier(ItemCategory.SorceryBook, 0), new ItemIdentifier(ItemCategory.Hafted, HaftedType.Mace),
                new ItemIdentifier(ItemCategory.DeathBook, 0)
            };

        public override int StealthBonusPerLevel => 0;

        public override string Title => "Priest";

        public override string ClassSubName(Realm realm)
        {
            return realm == Realm.Death ? "Exorcist" : "Priest";
        }

        public override double SpellBaseFailureMultiplier(Realm realm)
        {
            return (realm == Realm.Life || realm == Realm.Death) ? 0.8 : 1.0;
        }

        public override double SpellLevelMultiplier(Realm realm)
        {
            return (realm == Realm.Life || realm == Realm.Death) ? 0.7 : 1.1;
        }

        public override double SpellVisCostMultiplier(Realm realm)
        {
            return (realm == Realm.Life || realm == Realm.Death) ? 0.8 : 1.1;
        }

        public override bool TrySenseInventory(int playerLevel)
        {
            return Program.Rng.RandomLessThan(10000 / ((playerLevel * playerLevel) + 40)) == 0;
        }
    }
}