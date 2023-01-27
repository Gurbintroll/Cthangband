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
    internal class PlayerRaceImp : BasePlayerRace
    {
        private int[] _abilityBonus = { -1, -1, -1, 1, 2, -3 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AgeRange => 4;

        public override string Article => "an";

        public override int BaseAge => 13;

        public override int BaseDeviceBonus => 2;

        public override int BaseDisarmBonus => -3;

        public override int BaseMeleeAttackBonus => 5;

        public override int BaseRangedAttackBonus => -5;

        public override int BaseSaveBonus => -1;

        public override int BaseSearchBonus => -1;

        public override int BaseSearchFrequency => 10;

        public override int BaseStealthBonus => 1;

        public override string Description2 => "Imps are minor demons that have escaped their binding and";

        public override string Description3 => "are able to run free in the world. Imps naturally resist";

        public override string Description4 => "fire, and can learn to throw bolt of flame (at lvl 10),";

        public override string Description5 => "see invisible creatures (at lvl 10), become completely";

        public override string Description6 => "immune to fire (at lvl 20), and cast fireballs (at lvl 30).";

        public override int ExperienceFactor => 110;

        public override int FemaleBaseHeight => 64;

        public override int FemaleBaseWeight => 120;

        public override int FemaleHeightRange => 1;

        public override int FemaleWeightRange => 5;

        public override int HitDieBonus => 10;

        public override int Infravision => 3;

        public override int MaleBaseHeight => 68;

        public override int MaleBaseWeight => 150;

        public override int MaleHeightRange => 1;

        public override int MaleWeightRange => 5;

        public override bool SanityImmune => true;

        public override string Title => "Imp";

        protected override int BackgroundStartingChart => 94;

        public override void ApplyRacialStatus(Player player)
        {
            player.HasFireResistance = true;
            if (player.Level > 9)
            {
                player.HasSeeInvisibility = true;
            }
            if (player.Level > 19)
            {
                player.HasFireImmunity = true;
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
                name = _angelSyllable1[Program.Rng.RandomLessThan(_angelSyllable1.Length)];
                name += _angelSyllable2[Program.Rng.RandomLessThan(_angelSyllable2.Length)];
                name += _angelSyllable3[Program.Rng.RandomLessThan(_angelSyllable3.Length)];
            } while (name.Length > 12);
            return name;
        }

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            f2.Set(ItemFlag2.ResFire);
            if (player.Level > 9)
            {
                f3.Set(ItemFlag3.SeeInvis);
            }
            if (player.Level > 19)
            {
                f2.Set(ItemFlag2.ImFire);
            }
        }

        public override string GetRacialPowerText(Player player)
        {
            return player.Level < 9 ? "fire bolt/ball     (racial, unusable until level 9/30)" : "fire bolt/ball(30) (racial, cost 15, dam lvl, WIS based)";
        }

        public override List<string> SelfKnowledge(Player player)
        {
            var list = new List<string>();
            if (player.Level > 29)
            {
                list.Add($"You can cast a Fire Ball, dam. {player.Level} (cost 15).");
            }
            else if (player.Level > 8)
            {
                list.Add($"You can cast a Fire Bolt, dam. {player.Level} (cost 15).");
            }
            return list;
        }

        public override void UseRacialPower(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(9, 15, Ability.Wisdom, 15))
            {
                int direction;
                TargetEngine targetEngine = new TargetEngine(player, level);
                if (!targetEngine.GetDirectionWithAim(out direction))
                {
                    return;
                }
                if (player.Level >= 30)
                {
                    Profile.Instance.MsgPrint("You cast a ball of fire.");
                    saveGame.SpellEffects.FireBall(new ProjectFire(SaveGame.Instance.SpellEffects), direction, player.Level,
                        2);
                }
                else
                {
                    Profile.Instance.MsgPrint("You cast a bolt of fire.");
                    saveGame.SpellEffects.FireBolt(new ProjectFire(SaveGame.Instance.SpellEffects), direction, player.Level);
                }
            }
            player.RedrawNeeded.Set(RedrawFlag.PrHp | RedrawFlag.PrVis);
        }
    }
}