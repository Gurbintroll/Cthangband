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
    internal class PlayerClassWarrior : BasePlayerClass
    {
        private int[] _abilityBonus = { 5, -2, -2, 2, 2, -1 };
        public override int[] AbilityBonus => _abilityBonus;

        public override int AttacksPerTurnMax => 6;

        public override int AttacksPerTurnMinWeaponWeight => 30;

        public override int AttacksPerTurnWeightMultiplier => 5;

        public override int BaseDeviceBonus => 18;

        public override int BaseDisarmBonus => 25;

        public override int BaseMeleeAttackBonus => 70;

        public override int BaseRangedAttackBonus => 60;

        public override int BaseSaveBonus => 18;

        public override int BaseSearchBonus => 14;

        public override int BaseSearchFrequency => 2;

        public override int BaseStealthBonus => 1;

        public override CastingType CastingType => CastingType.None;

        public override string DescriptionLine2 => "Straightforward, no-nonsense fighters. They are the best";

        public override string DescriptionLine3 => "characters at melee combat, and require the least amount";

        public override string DescriptionLine4 => "of experience to increase in level. They can learn to";

        public override string DescriptionLine5 => "resist fear (at lvl 30). The ideal class for novices.";

        public override int DeviceBonusPerLevel => 7;

        public override int DisarmBonusPerLevel => 12;

        public override int ExperienceFactor => 0;

        public override Realm FirstRealmChoice => Realm.None;

        public override bool HasDetailedSenseInventory => true;

        public override int HitDieBonus => 9;

        public override int LegendaryItemBias => Enumerations.LegendaryItemBias.Warrior;

        public override int MeleeAttackBonusPerLevel => 45;

        public override int PrimeAbilityScore => Ability.Strength;

        public override int RangedAttackBonusPerLevel => 45;

        public override int SaveBonusPerLevel => 10;

        public override int SearchBonusPerLevel => 0;

        public override int SearchFrequencyPerLevel => 0;

        public override Realm SecondRealmChoice => Realm.None;

        public override ItemIdentifier[] StartingItems => new[]
            {
                new ItemIdentifier(ItemCategory.Ring, RingType.ResFear), new ItemIdentifier(ItemCategory.Sword, SwordType.BroadSword),
                new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.ChainMail)
            };

        public override int StealthBonusPerLevel => 0;

        public override string Title => "Warrior";

        public override void ApplyAttackAndDamageBonus(Player player)
        {
            player.AttackBonus += player.Level / 5;
            player.DamageBonus += player.Level / 5;
            player.DisplayedAttackBonus += player.Level / 5;
            player.DisplayedDamageBonus += player.Level / 5;
        }

        public override void ApplyBonusMeleeAttacks(Player player)
        {
            player.MeleeAttacksPerRound += player.Level / 15;
        }

        public override void ApplyBonusMissileAttacks(Player player)
        {
            if (player.AmmunitionItemCategory <= ItemCategory.Bolt && player.AmmunitionItemCategory >= ItemCategory.Shot)
            {
                if (player.Level >= 25)
                {
                    player.MissileAttacksPerRound++;
                }
                if (player.Level >= 50)
                {
                    player.MissileAttacksPerRound++;
                }
            }
        }

        public override void ApplyClassStatus(Player player)
        {
            if (player.Level > 29)
            {
                player.HasFearResistance = true;
            }
        }

        public override bool ExperienceForDestroying(Player player, ItemCategory category)
        {
            return true;
        }

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            if (player.Level > 29)
            {
                f2.Set(ItemFlag2.ResFear);
            }
        }

        public override bool TrySenseInventory(int playerLevel)
        {
            return Program.Rng.RandomLessThan(9000 / ((playerLevel * playerLevel) + 40)) == 0;
        }
    }
}