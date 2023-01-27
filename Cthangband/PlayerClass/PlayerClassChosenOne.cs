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
    internal class PlayerClassChosenOne : BasePlayerClass
    {
        private int[] _abilityBonus = { 3, -2, -2, 2, 2, -1 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AttacksPerTurnMax => 5;
        public override int AttacksPerTurnMinWeaponWeight => 35;
        public override int AttacksPerTurnWeightMultiplier => 3;
        public override int BaseDeviceBonus => 18;
        public override int BaseDisarmBonus => 25;
        public override int BaseMeleeAttackBonus => 50;
        public override int BaseRangedAttackBonus => 32;
        public override int BaseSaveBonus => 20;
        public override int BaseSearchBonus => 16;
        public override int BaseSearchFrequency => 4;
        public override int BaseStealthBonus => 1;
        public override CastingType CastingType => CastingType.None;
        public override string DescriptionLine2 => "Warriors of fate, who have no spell casting abilities but";
        public override string DescriptionLine3 => "gain a large number of passive magical abilities (too long";
        public override string DescriptionLine4 => "to list here) as they increase in level.";
        public override int DeviceBonusPerLevel => 7;
        public override int DisarmBonusPerLevel => 12;
        public override int ExperienceFactor => 20;
        public override Realm FirstRealmChoice => Realm.None;
        public override bool Glows => true;
        public override bool HasDetailedSenseInventory => true;
        public override int HitDieBonus => 4;
        public override int LegendaryItemBias => Enumerations.LegendaryItemBias.Warrior;
        public override int MeleeAttackBonusPerLevel => 20;
        public override int PrimeAbilityScore => Ability.Strength;
        public override int RangedAttackBonusPerLevel => 20;
        public override int SaveBonusPerLevel => 10;
        public override int SearchBonusPerLevel => 0;
        public override int SearchFrequencyPerLevel => 0;
        public override Realm SecondRealmChoice => Realm.None;

        public override ItemIdentifier[] StartingItems => new[]
            {
                new ItemIdentifier(ItemCategory.Sword, SwordType.SmallSword),
                new ItemIdentifier(ItemCategory.Potion, PotionType.Healing),
                new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SoftLeather)
            };

        public override int StealthBonusPerLevel => 0;
        public override string Title => "Chosen One";

        public override void ApplyClassStatus(Player player)
        {
            player.HasGlow = true;
            if (player.Level >= 2)
            {
                player.HasConfusionResistance = true;
            }
            if (player.Level >= 4)
            {
                player.HasFearResistance = true;
            }
            if (player.Level >= 6)
            {
                player.HasBlindnessResistance = true;
            }
            if (player.Level >= 8)
            {
                player.HasFeatherFall = true;
            }
            if (player.Level >= 10)
            {
                player.HasSeeInvisibility = true;
            }
            if (player.Level >= 12)
            {
                player.HasSlowDigestion = true;
            }
            if (player.Level >= 14)
            {
                player.HasSustainConstitution = true;
            }
            if (player.Level >= 16)
            {
                player.HasPoisonResistance = true;
            }
            if (player.Level >= 18)
            {
                player.HasSustainDexterity = true;
            }
            if (player.Level >= 20)
            {
                player.HasSustainStrength = true;
            }
            if (player.Level >= 22)
            {
                player.HasHoldLife = true;
            }
            if (player.Level >= 24)
            {
                player.HasFreeAction = true;
            }
            if (player.Level >= 26)
            {
                player.HasTelepathy = true;
            }
            if (player.Level >= 28)
            {
                player.HasDarkResistance = true;
            }
            if (player.Level >= 30)
            {
                player.HasLightResistance = true;
            }
            if (player.Level >= 32)
            {
                player.HasSustainCharisma = true;
            }
            if (player.Level >= 34)
            {
                player.HasSoundResistance = true;
            }
            if (player.Level >= 36)
            {
                player.HasDisenchantResistance = true;
            }
            if (player.Level >= 38)
            {
                player.HasRegeneration = true;
            }
            if (player.Level >= 40)
            {
                player.HasSustainIntelligence = true;
            }
            if (player.Level >= 42)
            {
                player.HasChaosResistance = true;
            }
            if (player.Level >= 44)
            {
                player.HasSustainWisdom = true;
            }
            if (player.Level >= 46)
            {
                player.HasNexusResistance = true;
            }
            if (player.Level >= 48)
            {
                player.HasShardResistance = true;
            }
            if (player.Level >= 50)
            {
                player.HasNetherResistance = true;
            }
        }

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            f3.Set(ItemFlag3.Lightsource);
            if (player.Level >= 2)
            {
                f2.Set(ItemFlag2.ResConf);
            }
            if (player.Level >= 4)
            {
                f2.Set(ItemFlag2.ResFear);
            }
            if (player.Level >= 6)
            {
                f2.Set(ItemFlag2.ResBlind);
            }
            if (player.Level >= 8)
            {
                f3.Set(ItemFlag3.Feather);
            }
            if (player.Level >= 10)
            {
                f3.Set(ItemFlag3.SeeInvis);
            }
            if (player.Level >= 12)
            {
                f3.Set(ItemFlag3.SlowDigest);
            }
            if (player.Level >= 14)
            {
                f2.Set(ItemFlag2.SustCon);
            }
            if (player.Level >= 16)
            {
                f2.Set(ItemFlag2.ResPois);
            }
            if (player.Level >= 18)
            {
                f2.Set(ItemFlag2.SustDex);
            }
            if (player.Level >= 20)
            {
                f2.Set(ItemFlag2.SustStr);
            }
            if (player.Level >= 22)
            {
                f2.Set(ItemFlag2.HoldLife);
            }
            if (player.Level >= 24)
            {
                f2.Set(ItemFlag2.FreeAct);
            }
            if (player.Level >= 26)
            {
                f3.Set(ItemFlag3.Telepathy);
            }
            if (player.Level >= 28)
            {
                f2.Set(ItemFlag2.ResDark);
            }
            if (player.Level >= 30)
            {
                f2.Set(ItemFlag2.ResLight);
            }
            if (player.Level >= 32)
            {
                f2.Set(ItemFlag2.SustCha);
            }
            if (player.Level >= 34)
            {
                f2.Set(ItemFlag2.ResSound);
            }
            if (player.Level >= 36)
            {
                f2.Set(ItemFlag2.ResDisen);
            }
            if (player.Level >= 38)
            {
                f3.Set(ItemFlag3.Regen);
            }
            if (player.Level >= 40)
            {
                f2.Set(ItemFlag2.SustInt);
            }
            if (player.Level >= 42)
            {
                f2.Set(ItemFlag2.ResChaos);
            }
            if (player.Level >= 44)
            {
                f2.Set(ItemFlag2.SustWis);
            }
            if (player.Level >= 46)
            {
                f2.Set(ItemFlag2.ResNexus);
            }
            if (player.Level >= 48)
            {
                f2.Set(ItemFlag2.ResShards);
            }
            if (player.Level >= 50)
            {
                f2.Set(ItemFlag2.ResNether);
            }
        }

        public override bool TrySenseInventory(int playerLevel)
        {
            return Program.Rng.RandomLessThan(9000 / ((playerLevel * playerLevel) + 40)) == 0;
        }
    }
}