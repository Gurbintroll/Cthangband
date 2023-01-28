// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
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

namespace Cthangband.PlayerRace
{
    [Serializable]
    internal class PlayerRaceZombie : BasePlayerRace
    {
        private int[] _abilityBonus = { 2, -6, -6, 1, 4, -5 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AgeRange => 30;
        public override int BaseAge => 100;
        public override int BaseDeviceBonus => -5;

        public override int BaseDisarmBonus => -5;

        public override int BaseMeleeAttackBonus => 15;

        public override int BaseRangedAttackBonus => 0;

        public override int BaseSaveBonus => 8;

        public override int BaseSearchBonus => -1;

        public override int BaseSearchFrequency => 5;

        public override int BaseStealthBonus => -1;

        public override string Description2 => "Zombies are undead creatures. Their decayed flesh resists";

        public override string Description3 => "nether and poison, and having their life force drained.";

        public override string Description4 => "Zombies digest food slowly, and can see invisible monsters.";

        public override string Description5 => "They can learn to restore their life force (at lvl 30).";

        public override bool DoesntEat => true;

        public override int ExperienceFactor => 135;

        public override int FemaleBaseHeight => 66;

        public override int FemaleBaseWeight => 100;

        public override int FemaleHeightRange => 4;

        public override int FemaleWeightRange => 20;

        public override int HitDieBonus => 13;

        public override int Infravision => 2;

        public override bool IsNocturnal => true;

        public override int MaleBaseHeight => 72;

        public override int MaleBaseWeight => 100;

        public override int MaleHeightRange => 6;

        public override int MaleWeightRange => 25;

        public override bool SanityResistant => true;

        public override string Title => "Zombie";

        protected override int BackgroundStartingChart => 107;

        public override void ApplyRacialStatus(Player player)
        {
            player.HasNetherResistance = true;
            player.HasHoldLife = true;
            player.HasSeeInvisibility = true;
            player.HasPoisonResistance = true;
            player.HasSlowDigestion = true;
            if (player.Level > 4)
            {
                player.HasColdResistance = true;
            }
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
            var name = "";
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
            return (player.Level > 11);
        }

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            f3.Set(ItemFlag3.SeeInvis);
            f2.Set(ItemFlag2.HoldLife);
            f2.Set(ItemFlag2.ResNether);
            f2.Set(ItemFlag2.ResPois);
            f3.Set(ItemFlag3.SlowDigest);
            if (player.Level > 4)
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