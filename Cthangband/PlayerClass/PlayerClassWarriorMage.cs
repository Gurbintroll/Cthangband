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
    internal class PlayerClassWarriorMage : BasePlayerClass
    {
        private int[] _abilityBonus = { 2, 2, 0, 1, 0, 1 };
        public override int[] AbilityBonus => _abilityBonus;

        public override int AttacksPerTurnMax => 5;

        public override int AttacksPerTurnMinWeaponWeight => 35;

        public override int AttacksPerTurnWeightMultiplier => 3;

        public override int BaseDeviceBonus => 30;

        public override int BaseDisarmBonus => 30;

        public override int BaseMeleeAttackBonus => 50;

        public override int BaseRangedAttackBonus => 26;

        public override int BaseSaveBonus => 28;

        public override int BaseSearchBonus => 18;

        public override int BaseSearchFrequency => 16;

        public override int BaseStealthBonus => 2;

        public override CastingType CastingType => CastingType.Arcane;

        public override string DescriptionLine2 => "A blend of both warrior and mage, getting the abilities of";

        public override string DescriptionLine3 => "both but not being the best at either. They use INT based";

        public override string DescriptionLine4 => "spell casting, getting access to the Folk realm plus a";

        public override string DescriptionLine5 => "second realm of their choice. They pay for their extreme";

        public override string DescriptionLine6 => "flexibility by increasing in level only slowly.";

        public override int DeviceBonusPerLevel => 10;

        public override int DisarmBonusPerLevel => 7;

        public override int ExperienceFactor => 50;

        public override Realm FirstRealmChoice => Realm.Life | Realm.Sorcery | Realm.Nature | Realm.Chaos | Realm.Death | Realm.Tarot | Realm.Folk | Realm.Corporeal;

        public override bool HasSpells => true;

        public override int HitDieBonus => 4;

        public override int LegendaryItemBias => Enumerations.LegendaryItemBias.Mage;

        public override int MeleeAttackBonusPerLevel => 20;

        public override int PrimeAbilityScore => Ability.Intelligence;

        public override int RangedAttackBonusPerLevel => 20;

        public override int SaveBonusPerLevel => 9;

        public override int SearchBonusPerLevel => 0;

        public override int SearchFrequencyPerLevel => 0;

        public override Realm SecondRealmChoice => Realm.Life | Realm.Sorcery | Realm.Nature | Realm.Chaos | Realm.Death | Realm.Tarot | Realm.Folk | Realm.Corporeal;

        public override int SpellWeightLimit => 350;

        public override ItemIdentifier[] StartingItems => new[]
            {
                new ItemIdentifier(ItemCategory.SorceryBook, 0), new ItemIdentifier(ItemCategory.Sword, SwordType.ShortSword),
                new ItemIdentifier(ItemCategory.DeathBook, 0)
            };

        public override int StealthBonusPerLevel => 0;

        public override string Title => "Warrior-Mage";

        public override int WarriorArtifactBias => 40;

        public override double SpellBaseFailureMultiplier(Realm realm)
        {
            return 1;
        }

        public override double SpellLevelMultiplier(Realm realm)
        {
            return 1.1;
        }

        public override double SpellVisCostMultiplier(Realm realm)
        {
            return 1.1;
        }

        public override bool TrySenseInventory(int playerLevel)
        {
            return Program.Rng.RandomLessThan(75000 / ((playerLevel * playerLevel) + 40)) == 0;
        }
    }
}