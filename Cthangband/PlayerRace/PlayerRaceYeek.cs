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
    internal class PlayerRaceYeek : BasePlayerRace
    {
        private int[] _abilityBonus = { -2, 1, 1, 1, -2, -7 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AgeRange => 3;
        public override int BaseAge => 14;

        public override int BaseDeviceBonus => 4;

        public override int BaseDisarmBonus => 2;

        public override int BaseMeleeAttackBonus => -5;

        public override int BaseRangedAttackBonus => -5;

        public override int BaseSaveBonus => 10;

        public override int BaseSearchBonus => 5;

        public override int BaseSearchFrequency => 15;

        public override int BaseStealthBonus => 3;

        public override string Description2 => "Yeeks are long-eared furry creatures that look vaguely";

        public override string Description3 => "like humanoid rabbits. Although physically weak, they make";

        public override string Description4 => "passable spell casters. They are resistant to acid, and can";

        public override string Description5 => "learn to scream to terrify their foes (at lvl 15) and";

        public override string Description6 => "become completely immune to acid (at lvl 20).";

        public override int ExperienceFactor => 100;

        public override int FemaleBaseHeight => 50;

        public override int FemaleBaseWeight => 75;

        public override int FemaleHeightRange => 3;

        public override int FemaleWeightRange => 3;

        public override int HitDieBonus => 7;

        public override int Infravision => 2;

        public override int MaleBaseHeight => 50;

        public override int MaleBaseWeight => 90;

        public override int MaleHeightRange => 3;

        public override int MaleWeightRange => 6;

        public override string Title => "Yeek";

        protected override int BackgroundStartingChart => 78;

        public override void ApplyRacialStatus(Player player)
        {
            player.HasAcidResistance = true;
            if (player.Level > 19)
            {
                player.HasAcidImmunity = true;
            }
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
                name = _yeekSyllable1[Program.Rng.RandomLessThan(_yeekSyllable1.Length)];
                name += _yeekSyllable2[Program.Rng.RandomLessThan(_yeekSyllable2.Length)];
                name += _yeekSyllable3[Program.Rng.RandomLessThan(_yeekSyllable3.Length)];
            } while (name.Length > 12);
            return name;
        }

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            f2.Set(ItemFlag2.ResAcid);
            if (player.Level > 19)
            {
                f2.Set(ItemFlag2.ImAcid);
            }
        }

        public override string GetRacialPowerText(Player player)
        {
            return player.Level < 15 ? "scare monster      (racial, unusable until level 15)" : "scare monster      (racial, cost 15, WIS based)";
        }

        public override List<string> SelfKnowledge(Player player)
        {
            var list = new List<string>();
            if (player.Level > 14)
            {
                list.Add("You can make a terrifying scream (cost 15).");
            }
            return list;
        }

        public override void UseRacialPower(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(15, 15, Ability.Wisdom, 10))
            {
                int direction;
                var targetEngine = new TargetEngine(player, level);
                if (!targetEngine.GetDirectionWithAim(out direction))
                {
                    return;
                }
                Profile.Instance.MsgPrint("You make a horrible scream!");
                saveGame.SpellEffects.FearMonster(direction, player.Level);
            }
            player.RedrawNeeded.Set(RedrawFlag.PrHp | RedrawFlag.PrVis);
        }
    }
}