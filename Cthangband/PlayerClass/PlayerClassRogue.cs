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
    internal class PlayerClassRogue : BasePlayerClass
    {
        private int[] _abilityBonus = { 2, 1, -2, 3, 1, -1 };
        public override int[] AbilityBonus => _abilityBonus;

        public override int AttacksPerTurnMax => 5;

        public override int AttacksPerTurnMinWeaponWeight => 30;

        public override int AttacksPerTurnWeightMultiplier => 3;

        public override int BaseDeviceBonus => 32;

        public override int BaseDisarmBonus => 45;

        public override int BaseMeleeAttackBonus => 60;

        public override int BaseRangedAttackBonus => 66;

        public override int BaseSaveBonus => 28;

        public override int BaseSearchBonus => 32;

        public override int BaseSearchFrequency => 24;

        public override int BaseStealthBonus => 5;

        public override CastingType CastingType => CastingType.Arcane;

        public override string DescriptionLine2 => "Stealth based characters who are adept at picking locks,";

        public override string DescriptionLine3 => "searching, and disarming traps. Rogues can use stealth to";

        public override string DescriptionLine4 => "their advantage in order to backstab sleeping or fleeing";

        public override string DescriptionLine5 => "foes. They also dabble in INT based magic, learning spells";

        public override string DescriptionLine6 => "from the Tarot, Sorcery, Death, or Folk realms.";

        public override int DeviceBonusPerLevel => 10;

        public override int DisarmBonusPerLevel => 15;

        public override int ExperienceFactor => 25;

        public override Realm FirstRealmChoice => Realm.Sorcery | Realm.Death | Realm.Tarot | Realm.Folk;

        public override bool HasBackstab => true;

        public override bool HasDetailedSenseInventory => true;
        public override bool HasSpells => true;

        public override int HitDieBonus => 6;

        public override int LegendaryItemBias => Enumerations.LegendaryItemBias.Rogue;

        public override int MeleeAttackBonusPerLevel => 40;

        public override int PrimeAbilityScore => Ability.Dexterity;

        public override int RangedAttackBonusPerLevel => 10;

        public override int SaveBonusPerLevel => 10;

        public override int SearchBonusPerLevel => 0;

        public override int SearchFrequencyPerLevel => 0;

        public override Realm SecondRealmChoice => Realm.None;

        public override int SpellWeightLimit => 350;

        public override ItemIdentifier[] StartingItems => new[]
            {
                new ItemIdentifier(ItemCategory.SorceryBook, 0), new ItemIdentifier(ItemCategory.Sword, SwordType.Dagger),
                new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SoftLeather)
            };

        public override int StealthBonusPerLevel => 0;

        public override string Title => "Rogue";

        public override int WarriorArtifactBias => 25;

        public override string ClassSubName(Realm realm)
        {
            switch (realm)
            {
                case Realm.Sorcery:
                    return "Burglar";

                case Realm.Death:
                    return "Assassin";

                case Realm.Tarot:
                    return "Card Sharp";

                default:
                    return "Thief";
            }
        }

        public override double SpellBaseFailureMultiplier(Realm realm)
        {
            return 1.2;
        }

        public override double SpellLevelMultiplier(Realm realm)
        {
            return 1.4;
        }

        public override double SpellVisCostMultiplier(Realm realm)
        {
            return 1.4;
        }

        public override bool TrySenseInventory(int playerLevel)
        {
            return Program.Rng.RandomLessThan(20000 / ((playerLevel * playerLevel) + 40)) == 0;
        }
    }
}