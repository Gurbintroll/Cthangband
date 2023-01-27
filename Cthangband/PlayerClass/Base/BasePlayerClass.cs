// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cthangband.PlayerClass.Base
{
    [Serializable]
    internal abstract class BasePlayerClass : IPlayerClass
    {
        public abstract int[] AbilityBonus { get; }
        public abstract int AttacksPerTurnMax { get; }
        public abstract int AttacksPerTurnMinWeaponWeight { get; }
        public abstract int AttacksPerTurnWeightMultiplier { get; }
        public abstract int BaseDeviceBonus { get; }

        public abstract int BaseDisarmBonus { get; }

        public abstract int BaseMeleeAttackBonus { get; }

        public abstract int BaseRangedAttackBonus { get; }

        public abstract int BaseSaveBonus { get; }

        public abstract int BaseSearchBonus { get; }

        public abstract int BaseSearchFrequency { get; }

        public abstract int BaseStealthBonus { get; }

        public virtual bool BluntWeaponsOnly => false;
        public abstract CastingType CastingType { get; }
        public virtual bool ChaosWeaponsOnly => false;
        public virtual string DescriptionLine1 => string.Empty;
        public virtual string DescriptionLine2 => string.Empty;
        public virtual string DescriptionLine3 => string.Empty;
        public virtual string DescriptionLine4 => string.Empty;
        public virtual string DescriptionLine5 => string.Empty;
        public virtual string DescriptionLine6 => string.Empty;
        public abstract int DeviceBonusPerLevel { get; }

        public abstract int DisarmBonusPerLevel { get; }

        public abstract int ExperienceFactor { get; }

        public abstract Realm FirstRealmChoice { get; }
        public virtual bool Glows => false;
        public virtual bool HasBackstab => false;
        public virtual bool HasDeity => false;
        public virtual bool HasDetailedSenseInventory => false;
        public virtual bool HasMinimumSpellFailure => true;
        public virtual bool HasPatron => false;
        public virtual bool HasSpellFailureChance => true;

        public virtual bool HasSpells => false;

        public abstract int HitDieBonus { get; }

        public virtual bool IsMartialArtist => false;
        public abstract int LegendaryItemBias { get; }
        public abstract int MeleeAttackBonusPerLevel { get; }

        public virtual int PetUpkeepDivider => 20;
        public abstract int PrimeAbilityScore { get; }

        public abstract int RangedAttackBonusPerLevel { get; }

        public abstract int SaveBonusPerLevel { get; }

        public abstract int SearchBonusPerLevel { get; }

        public abstract int SearchFrequencyPerLevel { get; }

        public abstract Realm SecondRealmChoice { get; }
        public virtual int SpellBallSizeFactor => 4;
        public virtual int SpellWeightLimit => 0;
        public abstract ItemIdentifier[] StartingItems { get; }
        public abstract int StealthBonusPerLevel { get; }

        public abstract string Title { get; }
        public virtual int WarriorArtifactBias => 0;

        public virtual void ApplyAttackAndDamageBonus(Player player)
        {
            // By default do nothing
        }

        public virtual void ApplyBonusMeleeAttacks(Player player)
        {
            // By default do nothing
        }

        public virtual void ApplyBonusMissileAttacks(Player player)
        {
            // By default do nothing
        }

        public virtual void ApplyClassStatus(Player player)
        {
            // By default do nothing
        }

        public virtual void ApplyMartialArtistArmourClass(Player player)
        {
            // By default do nothing
        }

        public virtual int ApplyVisBonus(int msp)
        {
            return msp;
        }

        public Realm ChooseRealmRandomly(Realm choices, Player player)
        {
            if (choices == Realm.None)
            {
                return choices;
            }

            Realm[] picks = new Realm[Constants.MaxRealm];
            int n = 0;
            if ((choices & Realm.Chaos) != 0 && player.Realm1 != Realm.Chaos)
            {
                picks[n] = Realm.Chaos;
                n++;
            }
            if ((choices & Realm.Corporeal) != 0 && player.Realm1 != Realm.Corporeal)
            {
                picks[n] = Realm.Corporeal;
                n++;
            }
            if ((choices & Realm.Death) != 0 && player.Realm1 != Realm.Death)
            {
                picks[n] = Realm.Death;
                n++;
            }
            if ((choices & Realm.Folk) != 0 && player.Realm1 != Realm.Folk)
            {
                picks[n] = Realm.Folk;
                n++;
            }
            if ((choices & Realm.Life) != 0 && player.Realm1 != Realm.Life)
            {
                picks[n] = Realm.Life;
                n++;
            }
            if ((choices & Realm.Nature) != 0 && player.Realm1 != Realm.Nature)
            {
                picks[n] = Realm.Nature;
                n++;
            }
            if ((choices & Realm.Tarot) != 0 && player.Realm1 != Realm.Tarot)
            {
                picks[n] = Realm.Tarot;
                n++;
            }
            if ((choices & Realm.Sorcery) != 0 && player.Realm1 != Realm.Sorcery)
            {
                picks[n] = Realm.Sorcery;
                n++;
            }
            int k = Program.Rng.RandomLessThan(n);
            return picks[k];
        }

        public virtual string ClassSubName(Realm realm)
        {
            return Title;
        }

        public virtual bool ExperienceForDestroying(Player player, ItemCategory category)
        {
            return false;
        }

        public virtual void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            // By default do nothing
        }

        public virtual bool HasSpeedBonus(Player player)
        {
            return false;
        }

        public virtual bool MartialArtistHeavyArmour(Player player)
        {
            return false;
        }

        public virtual double SpellBaseFailureMultiplier(Realm realm)
        {
            // By default no spells
            return 0;
        }

        public virtual int SpellBeamChance(int level)
        {
            return level / 2;
        }

        public virtual double SpellLevelMultiplier(Realm realm)
        {
            // By default no spells
            return 99;
        }

        public virtual double SpellVisCostMultiplier(Realm realm)
        {
            // By default no spells
            return 0;
        }

        public abstract bool TrySenseInventory(int playerLevel);
    }
}