// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.PlayerRace.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cthangband.PlayerRace
{
    [Serializable]
    internal class PlayerRaceSkeleton : BasePlayerRace
    {
        private int[] _abilityBonus = { 0, -2, -2, 0, 1, -4 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AgeRange => 35;
        public override int BaseAge => 100;
        public override int BaseDeviceBonus => -5;
        public override int BaseDisarmBonus => -5;
        public override int BaseMeleeAttackBonus => 10;
        public override int BaseRangedAttackBonus => 0;
        public override int BaseSaveBonus => 5;
        public override int BaseSearchBonus => -1;
        public override int BaseSearchFrequency => 8;
        public override int BaseStealthBonus => -1;
        public override uint Choice => 0x5F0F;
        public override string Description2 => "Skeletons are undead creatures. Being without eyes, they";
        public override string Description3 => "use magical sight which can see invisible creatures. Their";
        public override string Description4 => "lack of flesh means that they resist poison and shards, and";
        public override string Description5 => "their life force is hard to drain. They can learn to resist";
        public override string Description6 => "cold (at lvl 10), and restore their life force (at lvl 30).";
        public override bool DoesntEat => true;
        public override int ExperienceFactor => 145;
        public override int FemaleBaseHeight => 66;
        public override int FemaleBaseWeight => 50;
        public override int FemaleHeightRange => 4;
        public override int FemaleWeightRange => 5;
        public override int HitDieBonus => 10;
        public override int Infravision => 2;
        public override bool IsNocturnal => true;
        public override int MaleBaseHeight => 72;
        public override int MaleBaseWeight => 50;
        public override int MaleHeightRange => 6;
        public override int MaleWeightRange => 5;
        public override bool SanityResistant => true;
        public override bool SpillsPotions => true;
        public override string Title => "Skeleton";

        protected override int BackgroundStartingChart => 102;

        public override void ApplyRacialStatus(Player player)
        {
            player.HasShardResistance = true;
            player.HasHoldLife = true;
            player.HasSeeInvisibility = true;
            player.HasPoisonResistance = true;
            if (player.Level > 9)
            {
                player.HasColdResistance = true;
            }
        }

        public override void ConsumeFood(Player player, Item item)
        {
            if (!(item.ItemSubCategory == FoodType.Waybread || item.ItemSubCategory == FoodType.Warpstone ||
                  item.ItemSubCategory < FoodType.Biscuit))
            {
                // Spawn a new food item on the floor to make up for the one that will be destroyed
                Item floorItem = new Item();
                Profile.Instance.MsgPrint("The food falls through your jaws!");
                floorItem.AssignItemType(
                    Profile.Instance.ItemTypes.LookupKind(item.Category, item.ItemSubCategory));
                SaveGame.Instance.Level.DropNear(floorItem, -1, player.MapY, player.MapX);
            }
            else
            {
                // But some magical types work anyway and then vanish
                Profile.Instance.MsgPrint("The food falls through your jaws and vanishes!");
            }
        }

        /// <summary>
        /// Create a random name for a character based on their race.
        /// </summary>
        /// <returns> The random name </returns>
        public override string CreateRandomName()
        {
            string name = "";
            do
            {
                name = _humanSyllable1[Program.Rng.RandomLessThan(_humanSyllable1.Length)];
                name += _humanSyllable2[Program.Rng.RandomLessThan(_humanSyllable2.Length)];
                name += _humanSyllable3[Program.Rng.RandomLessThan(_humanSyllable3.Length)];
            } while (name.Length > 12);
            return name;
        }

        public override bool DoesntBleed(Player player)
        {
            return true;
        }

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            f3.Set(ItemFlag3.SeeInvis);
            f2.Set(ItemFlag2.ResShards);
            f2.Set(ItemFlag2.HoldLife);
            f2.Set(ItemFlag2.ResPois);
            if (player.Level > 9)
            {
                f2.Set(ItemFlag2.ResCold);
            }
        }

        public override string GetRacialPowerText(Player player)
        {
            return player.Level < 30 ? "restore life       (racial, unusable until level 30)" : "restore life       (racial, cost 30, WIS based)";
        }

        public override List<string> SelfKnowledge(Player player)
        {
            var list = new List<string>();
            if (player.Level > 29)
            {
                list.Add("You can restore lost life forces (cost 30).");
            }
            return list;
        }

        public override void UseRacialPower(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(30, 30, Ability.Wisdom, 18))
            {
                Profile.Instance.MsgPrint("You attempt to restore your lost energies.");
                player.RestoreLevel();
            }
            player.RedrawNeeded.Set(RedrawFlag.PrHp | RedrawFlag.PrVis);
        }
    }
}