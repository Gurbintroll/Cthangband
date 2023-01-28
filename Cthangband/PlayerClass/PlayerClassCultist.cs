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
    internal class PlayerClassCultist : BasePlayerClass
    {
        private int[] _abilityBonus = { -5, 4, 0, 1, -2, -2 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AttacksPerTurnMax => 4;
        public override int AttacksPerTurnMinWeaponWeight => 40;
        public override int AttacksPerTurnWeightMultiplier => 2;
        public override int BaseDeviceBonus => 36;
        public override int BaseDisarmBonus => 30;
        public override int BaseMeleeAttackBonus => 30;
        public override int BaseRangedAttackBonus => 20;
        public override int BaseSaveBonus => 32;
        public override int BaseSearchBonus => 16;
        public override int BaseSearchFrequency => 18;
        public override int BaseStealthBonus => 1;
        public override CastingType CastingType => CastingType.Arcane;
        public override bool ChaosWeaponsOnly => true;
        public override string DescriptionLine2 => "CHA based spell casters, who use Chaos and another realm";
        public override string DescriptionLine3 => "of their choice. Can't wield weapons except for powerful";
        public override string DescriptionLine4 => "chaos blades. Learn to resist chaos (at lvl 20). Have a";
        public override string DescriptionLine5 => "cult patron who will randomly give them rewards or";
        public override string DescriptionLine6 => "punishments as they increase in level.";
        public override int DeviceBonusPerLevel => 13;
        public override int DisarmBonusPerLevel => 7;
        public override int ExperienceFactor => 30;
        public override Realm FirstRealmChoice => Realm.Chaos;
        public override bool HasMinimumSpellFailure => false;
        public override bool HasPatron => true;

        public override bool HasSpells => true;

        public override int HitDieBonus => 0;

        public override int LegendaryItemBias => Enumerations.LegendaryItemBias.Mage;

        public override int MeleeAttackBonusPerLevel => 15;

        public override int PrimeAbilityScore => Ability.Charisma;

        public override int RangedAttackBonusPerLevel => 15;

        public override int SaveBonusPerLevel => 10;

        public override int SearchBonusPerLevel => 0;

        public override int SearchFrequencyPerLevel => 0;

        public override Realm SecondRealmChoice => Realm.Life | Realm.Sorcery | Realm.Nature | Realm.Death | Realm.Tarot | Realm.Folk | Realm.Corporeal;

        public override int SpellWeightLimit => 300;

        public override ItemIdentifier[] StartingItems => new[]
            {
                new ItemIdentifier(ItemCategory.SorceryBook, 0), new ItemIdentifier(ItemCategory.Ring, RingType.SustainInt),
                new ItemIdentifier(ItemCategory.DeathBook, 0)
            };

        public override int StealthBonusPerLevel => 0;

        public override string Title => "Cultist";

        public override void ApplyClassStatus(Player player)
        {
            if (player.Level > 19)
            {
                player.HasChaosResistance = true;
            }
        }

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            if (player.Level > 19)
            {
                f2.Set(ItemFlag2.ResChaos);
            }
        }

        public override double SpellBaseFailureMultiplier(Realm realm)
        {
            return realm == Realm.Chaos ? 0.9 : 1.1;
        }

        public override double SpellLevelMultiplier(Realm realm)
        {
            return realm == Realm.Chaos ? 0.9 : 1;
        }

        public override double SpellVisCostMultiplier(Realm realm)
        {
            return realm == Realm.Chaos ? 0.9 : 1.2;
        }

        public override bool TrySenseInventory(int playerLevel)
        {
            return Program.Rng.RandomLessThan(240000 / (playerLevel + 5)) == 0;
        }
    }
}