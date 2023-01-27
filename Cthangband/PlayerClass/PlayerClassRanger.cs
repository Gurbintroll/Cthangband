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
    [Serializable]
    internal class PlayerClassRanger : BasePlayerClass
    {
        private int[] _abilityBonus = { 2, 2, 0, 1, 1, 1 };
        public override int[] AbilityBonus => _abilityBonus;

        public override int AttacksPerTurnMax => 5;

        public override int AttacksPerTurnMinWeaponWeight => 35;

        public override int AttacksPerTurnWeightMultiplier => 4;

        public override int BaseDeviceBonus => 32;

        public override int BaseDisarmBonus => 30;

        public override int BaseMeleeAttackBonus => 56;

        public override int BaseRangedAttackBonus => 72;

        public override int BaseSaveBonus => 28;

        public override int BaseSearchBonus => 24;

        public override int BaseSearchFrequency => 16;

        public override int BaseStealthBonus => 3;

        public override CastingType CastingType => CastingType.Divine;

        public override string DescriptionLine2 => "Masters of ranged combat, especiallly using bows. Rangers";

        public override string DescriptionLine3 => "supplement their shooting and stealth with INT based spell";

        public override string DescriptionLine4 => "casting from the Nature realm plus another realm of their";

        public override string DescriptionLine5 => "choice from Death, Corporeal, Tarot, Chaos, and Folk.";

        public override int DeviceBonusPerLevel => 10;

        public override int DisarmBonusPerLevel => 8;

        public override int ExperienceFactor => 30;

        public override Realm FirstRealmChoice => Realm.Nature;

        public override bool HasDetailedSenseInventory => true;
        public override bool HasSpells => true;

        public override int HitDieBonus => 4;

        public override int LegendaryItemBias => Enumerations.LegendaryItemBias.Ranger;

        public override int MeleeAttackBonusPerLevel => 30;

        public override int PrimeAbilityScore => Ability.Intelligence;

        public override int RangedAttackBonusPerLevel => 45;

        public override int SaveBonusPerLevel => 10;

        public override int SearchBonusPerLevel => 0;

        public override int SearchFrequencyPerLevel => 0;

        public override Realm SecondRealmChoice => Realm.Chaos | Realm.Death | Realm.Tarot | Realm.Folk | Realm.Corporeal;

        public override int SpellWeightLimit => 400;

        public override ItemIdentifier[] StartingItems => new[]
            {
                new ItemIdentifier(ItemCategory.NatureBook, 0), new ItemIdentifier(ItemCategory.Sword, SwordType.BroadSword),
                new ItemIdentifier(ItemCategory.DeathBook, 0)
            };

        public override int StealthBonusPerLevel => 0;

        public override string Title => "Ranger";

        public override int WarriorArtifactBias => 30;

        public override void ApplyBonusMissileAttacks(Player player)
        {
            if (player.AmmunitionItemCategory == ItemCategory.Arrow)
            {
                if (player.Level >= 20)
                {
                    player.MissileAttacksPerRound++;
                }
                if (player.Level >= 40)
                {
                    player.MissileAttacksPerRound++;
                }
            }
        }

        public override double SpellBaseFailureMultiplier(Realm realm)
        {
            return realm == Realm.Nature ? 1 : 1.2;
        }

        public override double SpellLevelMultiplier(Realm realm)
        {
            return realm == Realm.Nature ? 1 : 1.3;
        }

        public override double SpellVisCostMultiplier(Realm realm)
        {
            return realm == Realm.Nature ? 1 : 1.3;
        }

        public override bool TrySenseInventory(int playerLevel)
        {
            return Program.Rng.RandomLessThan(95000 / ((playerLevel * playerLevel) + 40)) == 0;
        }
    }
}