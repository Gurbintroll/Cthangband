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

namespace Cthangband.PlayerClass
{
    [Serializable]
    internal class PlayerClassChanneller : BasePlayerClass
    {
        private int[] _abilityBonus = { -1, 0, 2, -1, -1, 3 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AttacksPerTurnMax => 4;
        public override int AttacksPerTurnMinWeaponWeight => 40;
        public override int AttacksPerTurnWeightMultiplier => 2;
        public override int BaseDeviceBonus => 40;
        public override int BaseDisarmBonus => 40;
        public override int BaseMeleeAttackBonus => 40;
        public override int BaseRangedAttackBonus => 30;
        public override int BaseSaveBonus => 30;
        public override int BaseSearchBonus => 16;
        public override int BaseSearchFrequency => 20;
        public override int BaseStealthBonus => 2;
        public override CastingType CastingType => CastingType.Channelling;
        public override string DescriptionLine2 => "Similar to a spell caster, but rather than casting spells";
        public override string DescriptionLine3 => "from a book, they can use their CHA to channel vis into";
        public override string DescriptionLine4 => "most types of item, powering the effects of the items";
        public override string DescriptionLine5 => "without depleting them.";
        public override int DeviceBonusPerLevel => 13;
        public override int DisarmBonusPerLevel => 12;
        public override int ExperienceFactor => 30;
        public override Realm FirstRealmChoice => Realm.None;
        public override bool HasDetailedSenseInventory => true;
        public override int HitDieBonus => 1;
        public override int LegendaryItemBias => Enumerations.LegendaryItemBias.Mage;
        public override int MeleeAttackBonusPerLevel => 15;
        public override int PrimeAbilityScore => Ability.Charisma;
        public override int RangedAttackBonusPerLevel => 15;
        public override int SaveBonusPerLevel => 9;
        public override int SearchBonusPerLevel => 0;
        public override int SearchFrequencyPerLevel => 0;
        public override Realm SecondRealmChoice => Realm.None;
        public override int SpellWeightLimit => 400;

        public override ItemIdentifier[] StartingItems => new[]
            {
                new ItemIdentifier(ItemCategory.Wand, WandType.MagicMissile), new ItemIdentifier(ItemCategory.Sword, SwordType.Dagger),
                new ItemIdentifier(ItemCategory.Ring, RingType.SustainCha)
            };

        public override int StealthBonusPerLevel => 0;
        public override string Title => "Channeller";

        public override bool TrySenseInventory(int playerLevel)
        {
            return Program.Rng.RandomLessThan(9000 / ((playerLevel * playerLevel) + 40)) == 0;
        }
    }
}