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
    internal class PlayerClassMindcrafter : BasePlayerClass
    {
        private int[] _abilityBonus = { -1, 0, 3, -1, -1, 2 };
        public override int[] AbilityBonus => _abilityBonus;

        public override int AttacksPerTurnMax => 5;

        public override int AttacksPerTurnMinWeaponWeight => 35;

        public override int AttacksPerTurnWeightMultiplier => 3;

        public override int BaseDeviceBonus => 30;

        public override int BaseDisarmBonus => 30;

        public override int BaseMeleeAttackBonus => 50;

        public override int BaseRangedAttackBonus => 40;

        public override int BaseSaveBonus => 30;

        public override int BaseSearchBonus => 22;

        public override int BaseSearchFrequency => 16;

        public override int BaseStealthBonus => 3;

        public override CastingType CastingType => CastingType.Mentalism;

        public override string DescriptionLine2 => "Disciples of the psionic arts, Mindcrafters learn a range";

        public override string DescriptionLine3 => "of mental abilities; which they power using WIS. As well";

        public override string DescriptionLine4 => "as their powers, they learn to resist fear (at lvl 10),";

        public override string DescriptionLine5 => "prevent wis drain (at lvl 20), resist confusion";

        public override string DescriptionLine6 => "(at lvl 30), and gain telepathy (at lvl 40).";

        public override int DeviceBonusPerLevel => 10;

        public override int DisarmBonusPerLevel => 10;

        public override int ExperienceFactor => 25;

        public override Realm FirstRealmChoice => Realm.None;

        public override int HitDieBonus => 2;

        public override int LegendaryItemBias => Enumerations.LegendaryItemBias.Priestly;

        public override int MeleeAttackBonusPerLevel => 20;

        public override int PrimeAbilityScore => Ability.Wisdom;

        public override int RangedAttackBonusPerLevel => 30;

        public override int SaveBonusPerLevel => 10;

        public override int SearchBonusPerLevel => 0;

        public override int SearchFrequencyPerLevel => 0;

        public override Realm SecondRealmChoice => Realm.None;

        public override int SpellWeightLimit => 350;

        public override ItemIdentifier[] StartingItems => new[]
            {
                new ItemIdentifier(ItemCategory.Sword, SwordType.SmallSword),
                new ItemIdentifier(ItemCategory.Potion, PotionType.RestoreVis),
                new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SoftLeather)
            };

        public override int StealthBonusPerLevel => 0;

        public override string Title => "Mindcrafter";

        public override int WarriorArtifactBias => 60;

        public override void ApplyClassStatus(Player player)
        {
            if (player.Level > 9)
            {
                player.HasFearResistance = true;
            }
            if (player.Level > 19)
            {
                player.HasSustainWisdom = true;
            }
            if (player.Level > 29)
            {
                player.HasConfusionResistance = true;
            }
            if (player.Level > 39)
            {
                player.HasTelepathy = true;
            }
        }

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            if (player.Level > 9)
            {
                f2.Set(ItemFlag2.ResFear);
            }
            if (player.Level > 19)
            {
                f2.Set(ItemFlag2.SustWis);
            }
            if (player.Level > 29)
            {
                f2.Set(ItemFlag2.ResConf);
            }
            if (player.Level > 39)
            {
                f3.Set(ItemFlag3.Telepathy);
            }
        }

        public override bool TrySenseInventory(int playerLevel)
        {
            return Program.Rng.RandomLessThan(55000 / ((playerLevel * playerLevel) + 40)) == 0;
        }
    }
}