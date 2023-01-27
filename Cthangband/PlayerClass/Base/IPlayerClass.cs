// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”

using Cthangband.Enumerations;

namespace Cthangband.PlayerClass.Base
{
    internal interface IPlayerClass
    {
        int[] AbilityBonus { get; }
        int AttacksPerTurnMax { get; }
        int AttacksPerTurnMinWeaponWeight { get; }
        int AttacksPerTurnWeightMultiplier { get; }
        int BaseDeviceBonus { get; }
        int BaseDisarmBonus { get; }
        int BaseMeleeAttackBonus { get; }
        int BaseRangedAttackBonus { get; }
        int BaseSaveBonus { get; }
        int BaseSearchBonus { get; }
        int BaseSearchFrequency { get; }
        int BaseStealthBonus { get; }
        bool BluntWeaponsOnly { get; }
        CastingType CastingType { get; }
        bool ChaosWeaponsOnly { get; }
        string DescriptionLine1 { get; }
        string DescriptionLine2 { get; }
        string DescriptionLine3 { get; }
        string DescriptionLine4 { get; }
        string DescriptionLine5 { get; }
        string DescriptionLine6 { get; }
        int DeviceBonusPerLevel { get; }
        int DisarmBonusPerLevel { get; }
        int ExperienceFactor { get; }
        Realm FirstRealmChoice { get; }
        bool Glows { get; }
        bool HasBackstab { get; }
        bool HasDeity { get; }
        bool HasDetailedSenseInventory { get; }
        bool HasMinimumSpellFailure { get; }
        bool HasPatron { get; }
        bool HasSpellFailureChance { get; }

        bool HasSpells { get; }

        int HitDieBonus { get; }

        bool IsMartialArtist { get; }
        int LegendaryItemBias { get; }
        int MeleeAttackBonusPerLevel { get; }

        int PetUpkeepDivider { get; }
        int PrimeAbilityScore { get; }

        int RangedAttackBonusPerLevel { get; }

        int SaveBonusPerLevel { get; }

        int SearchBonusPerLevel { get; }

        int SearchFrequencyPerLevel { get; }

        Realm SecondRealmChoice { get; }
        int SpellBallSizeFactor { get; }
        int SpellWeightLimit { get; }
        ItemIdentifier[] StartingItems { get; }
        int StealthBonusPerLevel { get; }

        string Title { get; }
        int WarriorArtifactBias { get; }

        void ApplyAttackAndDamageBonus(Player player);

        void ApplyBonusMeleeAttacks(Player player);

        void ApplyBonusMissileAttacks(Player player);

        void ApplyClassStatus(Player player);

        void ApplyMartialArtistArmourClass(Player player);

        int ApplyVisBonus(int msp);

        Realm ChooseRealmRandomly(Realm choice, Player player);

        string ClassSubName(Realm realm);

        bool ExperienceForDestroying(Player player, ItemCategory category);

        void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3);

        bool HasSpeedBonus(Player player);

        bool MartialArtistHeavyArmour(Player player);

        double SpellBaseFailureMultiplier(Realm realm);

        int SpellBeamChance(int level);

        double SpellLevelMultiplier(Realm realm);

        double SpellVisCostMultiplier(Realm realm);

        bool TrySenseInventory(int playerLevel);
    }
}