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
using System.Threading;
using System.Threading.Tasks;

namespace Cthangband.PlayerClass
{
    [Serializable]
    internal class PlayerClassHighMage : BasePlayerClass
    {
        private int[] _abilityBonus = { -5, 4, 0, 0, -2, 1 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AttacksPerTurnMax => 4;
        public override int AttacksPerTurnMinWeaponWeight => 40;
        public override int AttacksPerTurnWeightMultiplier => 2;
        public override int BaseDeviceBonus => 38;
        public override int BaseDisarmBonus => 30;
        public override int BaseMeleeAttackBonus => 34;
        public override int BaseRangedAttackBonus => 20;
        public override int BaseSaveBonus => 30;
        public override int BaseSearchBonus => 16;
        public override int BaseSearchFrequency => 20;
        public override int BaseStealthBonus => 2;
        public override CastingType CastingType => CastingType.Arcane;
        public override string DescriptionLine2 => "INT based spell casters who specialise in a single realm";
        public override string DescriptionLine3 => "of magic. They may choose any realm, and are better at";
        public override string DescriptionLine4 => "casting spells from that realm than a normal mage. High";
        public override string DescriptionLine5 => "mages also get more vis than other spell casters do.";
        public override string DescriptionLine6 => "Wearing too much armour disrupts their casting.";
        public override int DeviceBonusPerLevel => 13;
        public override int DisarmBonusPerLevel => 7;
        public override int ExperienceFactor => 30;
        public override Realm FirstRealmChoice => Realm.Life | Realm.Sorcery | Realm.Nature | Realm.Chaos | Realm.Death | Realm.Tarot | Realm.Folk | Realm.Corporeal;
        public override bool HasMinimumSpellFailure => false;
        public override bool HasSpells => true;
        public override int HitDieBonus => 0;
        public override int LegendaryItemBias => Enumerations.LegendaryItemBias.Mage;
        public override int MeleeAttackBonusPerLevel => 15;
        public override int PetUpkeepDivider => 12;
        public override int PrimeAbilityScore => Ability.Intelligence;
        public override int RangedAttackBonusPerLevel => 15;
        public override int SaveBonusPerLevel => 9;
        public override int SearchBonusPerLevel => 0;
        public override int SearchFrequencyPerLevel => 0;
        public override Realm SecondRealmChoice => Realm.None;
        public override int SpellBallSizeFactor => 2;
        public override int SpellWeightLimit => 300;

        public override ItemIdentifier[] StartingItems => new[]
            {
                new ItemIdentifier(ItemCategory.SorceryBook, 0), new ItemIdentifier(ItemCategory.Sword, SwordType.Dagger),
                new ItemIdentifier(ItemCategory.Ring, RingType.SustainInt)
            };

        public override int StealthBonusPerLevel => 0;

        public override string Title => "High-Mage";

        public override int ApplyVisBonus(int msp)
        {
            return msp + msp / 4;
        }

        public override string ClassSubName(Realm realm)
        {
            switch (realm)
            {
                case Realm.Life:
                    return "Vivimancer";

                case Realm.Sorcery:
                    return "Sorcerer";

                case Realm.Nature:
                    return "Naturist";

                case Realm.Chaos:
                    return "Warlock";

                case Realm.Death:
                    return "Necromancer";

                case Realm.Tarot:
                    return "Summoner";

                case Realm.Folk:
                    return "Hedge Wizard";

                case Realm.Corporeal:
                    return "Zen Master";

                default:
                    return "High Mage";
            }
        }

        public override double SpellBaseFailureMultiplier(Realm realm)
        {
            return 0.8;
        }

        public override int SpellBeamChance(int level)
        {
            return level + 10;
        }

        public override double SpellLevelMultiplier(Realm realm)
        {
            return 0.8;
        }

        public override double SpellVisCostMultiplier(Realm realm)
        {
            return 0.8;
        }

        public override bool TrySenseInventory(int playerLevel)
        {
            return Program.Rng.RandomLessThan(240000 / (playerLevel + 5)) == 0;
        }
    }
}