// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.PlayerRace.Base;
using Cthangband.Projection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cthangband.PlayerRace
{
    [Serializable]
    internal class PlayerRaceGolem : BasePlayerRace
    {
        private int[] _abilityBonus = { 4, -5, -5, 0, 4, -4 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AgeRange => 100;
        public override int BaseAge => 1;

        public override int BaseDeviceBonus => -5;

        public override int BaseDisarmBonus => -5;

        public override int BaseMeleeAttackBonus => 20;

        public override int BaseRangedAttackBonus => 0;

        public override int BaseSaveBonus => 10;

        public override int BaseSearchBonus => -1;

        public override int BaseSearchFrequency => 8;

        public override int BaseStealthBonus => -1;

        public override uint Choice => 0x4001;

        public override string Description1 => "Golems are animated statues. Their inorganic bodies make it";

        public override string Description2 => "hard for them to digest food properly, but they have innate";

        public override string Description3 => "natural armour and can't be stunned or made to bleed. They";

        public override string Description4 => "also resist poison and can see invisible creatures. Golems";

        public override string Description5 => "can learn to use their armour more efficiently (at lvl 20)";

        public override string Description6 => "and avoid having their life force drained (at lvl 35).";

        public override bool DoesntEat => true;

        public override int ExperienceFactor => 200;

        public override int FemaleBaseHeight => 62;

        public override int FemaleBaseWeight => 180;

        public override int FemaleHeightRange => 1;

        public override int FemaleWeightRange => 6;

        public override int HitDieBonus => 12;

        public override int Infravision => 4;

        public override int MaleBaseHeight => 66;

        public override int MaleBaseWeight => 200;

        public override int MaleHeightRange => 1;

        public override int MaleWeightRange => 6;

        public override string Title => "Golem";

        protected override int BackgroundStartingChart => 98;

        public override void ApplyArmourBonus(Player player)
        {
            player.ArmourClassBonus += 20 + (player.Level / 5);
            player.DisplayedArmourClassBonus += 20 + (player.Level / 5);
        }

        public override void ApplyRacialStatus(Player player)
        {
            if (player.Level > 34)
            {
                player.HasHoldLife = true;
            }
            player.HasSlowDigestion = true;
            player.HasFreeAction = true;
            player.HasSeeInvisibility = true;
            player.HasPoisonResistance = true;
        }

        public override void ConsumeFood(Player player, Item item)
        {
            Profile.Instance.MsgPrint("The food of mortals is poor sustenance for you.");
            player.SetFood(player.Food + (item.TypeSpecificValue / 20));
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
                name = _dwarfSyllable1[Program.Rng.RandomLessThan(_dwarfSyllable1.Length)];
                name += _dwarfSyllable2[Program.Rng.RandomLessThan(_dwarfSyllable2.Length)];
                name += _dwarfSyllable3[Program.Rng.RandomLessThan(_dwarfSyllable3.Length)];
            } while (name.Length > 12);
            return name;
        }

        public override bool DoesntBleed(Player player)
        {
            return true;
        }

        public override bool DoesntStun(Player player)
        {
            return true;
        }

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            f3.Set(ItemFlag3.SeeInvis);
            f2.Set(ItemFlag2.FreeAct);
            f2.Set(ItemFlag2.ResPois);
            f3.Set(ItemFlag3.SlowDigest);
            if (player.Level > 34)
            {
                f2.Set(ItemFlag2.HoldLife);
            }
        }

        public override string GetRacialPowerText(Player player)
        {
            return player.Level < 20 ? "stone skin         (racial, unusable until level 20)" : "stone skin         (racial, cost 15, dur 30+d20, CON based)";
        }

        public override List<string> SelfKnowledge(Player player)
        {
            var list = new List<string>();
            if (player.Level > 19)
            {
                list.Add("You can turn your skin to stone, dur d20+30 (cost 15).");
            }
            return list;
        }

        public override void UseRacialPower(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(20, 15, Ability.Constitution, 8))
            {
                player.SetTimedStoneskin(player.TimedStoneskin + Program.Rng.DieRoll(20) + 30);
            }
            player.RedrawNeeded.Set(RedrawFlag.PrHp | RedrawFlag.PrVis);
        }
    }
}