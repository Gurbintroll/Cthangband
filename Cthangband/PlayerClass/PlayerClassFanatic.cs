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
    internal class PlayerClassFanatic : BasePlayerClass
    {
        private int[] _abilityBonus = { 2, 1, 0, 1, 2, -2 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AttacksPerTurnMax => 5;
        public override int AttacksPerTurnMinWeaponWeight => 30;
        public override int AttacksPerTurnWeightMultiplier => 4;
        public override int BaseDeviceBonus => 24;
        public override int BaseDisarmBonus => 20;
        public override int BaseMeleeAttackBonus => 66;
        public override int BaseRangedAttackBonus => 40;
        public override int BaseSaveBonus => 30;
        public override int BaseSearchBonus => 14;
        public override int BaseSearchFrequency => 12;
        public override int BaseStealthBonus => 1;
        public override CastingType CastingType => CastingType.Divine;
        public override string DescriptionLine2 => "Warriors who dabble in CHA based Chaos magic. Have a cult";
        public override string DescriptionLine3 => "patron who will randomly give them rewards or punishments";
        public override string DescriptionLine4 => "as they increase in level. Learn to resist chaos";
        public override string DescriptionLine5 => "(at lvl 30) and fear (at lvl 40).";
        public override int DeviceBonusPerLevel => 11;
        public override int DisarmBonusPerLevel => 7;
        public override int ExperienceFactor => 35;
        public override Realm FirstRealmChoice => Realm.Chaos;
        public override bool HasDetailedSenseInventory => true;
        public override bool HasPatron => true;

        public override bool HasSpells => true;

        public override int HitDieBonus => 6;

        public override int LegendaryItemBias => Enumerations.LegendaryItemBias.Chaos;

        public override int MeleeAttackBonusPerLevel => 35;

        public override int PrimeAbilityScore => Ability.Intelligence;

        public override int RangedAttackBonusPerLevel => 30;

        public override int SaveBonusPerLevel => 10;

        public override int SearchBonusPerLevel => 0;

        public override int SearchFrequencyPerLevel => 0;

        public override Realm SecondRealmChoice => Realm.None;

        public override int SpellWeightLimit => 400;

        public override ItemIdentifier[] StartingItems => new[]
            {
                new ItemIdentifier(ItemCategory.SorceryBook, 0), new ItemIdentifier(ItemCategory.Sword, SwordType.BroadSword),
                new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.MetalScaleMail)
            };

        public override int StealthBonusPerLevel => 0;

        public override string Title => "Fanatic";

        public override int WarriorArtifactBias => 40;

        public override void ApplyClassStatus(Player player)
        {
            if (player.Level > 29)
            {
                player.HasChaosResistance = true;
            }
            if (player.Level > 39)
            {
                player.HasFearResistance = true;
            }
        }

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            if (player.Level > 39)
            {
                f2.Set(ItemFlag2.ResFear);
            }
            if (player.Level > 29)
            {
                f2.Set(ItemFlag2.ResChaos);
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
            return Program.Rng.RandomLessThan(80000 / ((playerLevel * playerLevel) + 40)) == 0;
        }
    }
}