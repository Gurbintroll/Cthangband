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
    internal class PlayerRaceKobold : BasePlayerRace
    {
        private int[] _abilityBonus = { 1, -1, 0, 1, 0, -4 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AgeRange => 3;

        public override int BaseAge => 11;

        public override int BaseDeviceBonus => -3;

        public override int BaseDisarmBonus => -2;

        public override int BaseMeleeAttackBonus => 10;

        public override int BaseRangedAttackBonus => -8;

        public override int BaseSaveBonus => -2;

        public override int BaseSearchBonus => 1;

        public override int BaseSearchFrequency => 8;

        public override int BaseStealthBonus => -1;

        public override uint Choice => 0xC009;

        public override string Description2 => "Kobolds are small reptillian creatures whose claims to be";

        public override string Description3 => "related to dragons are generally not taken seriously. They";

        public override string Description4 => "are resistant to poison, and can learn to throw poison";

        public override string Description5 => "darts (at lvl 9).";

        public override int ExperienceFactor => 125;

        public override int FemaleBaseHeight => 55;

        public override int FemaleBaseWeight => 100;

        public override int FemaleHeightRange => 1;

        public override int FemaleWeightRange => 5;

        public override int HitDieBonus => 9;

        public override int Infravision => 3;

        public override int MaleBaseHeight => 60;

        public override int MaleBaseWeight => 130;

        public override int MaleHeightRange => 1;

        public override int MaleWeightRange => 5;

        public override string Title => "Kobold";

        protected override int BackgroundStartingChart => 82;

        public override void ApplyRacialStatus(Player player)
        {
            player.HasPoisonResistance = true;
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
                name = _hobbitSyllable1[Program.Rng.RandomLessThan(_hobbitSyllable1.Length)];
                name += _hobbitSyllable2[Program.Rng.RandomLessThan(_hobbitSyllable2.Length)];
                name += _hobbitSyllable3[Program.Rng.RandomLessThan(_hobbitSyllable3.Length)];
            } while (name.Length > 12);
            return name;
        }

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            f2.Set(ItemFlag2.ResPois);
        }

        public override string GetRacialPowerText(Player player)
        {
            return player.Level < 12 ? "poison dart        (racial, unusable until level 12)" : "poison dart        (racial, cost 8, dam lvl, DEX based)";
        }

        public override List<string> SelfKnowledge(Player player)
        {
            var list = new List<string>();
            if (player.Level > 11)
            {
                list.Add($"You can throw a dart of poison, dam. {player.Level} (cost 8).");
            }
            return list;
        }

        public override void UseRacialPower(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(12, 8, Ability.Dexterity, 14))
            {
                int direction;
                TargetEngine targetEngine = new TargetEngine(player, level);
                if (!targetEngine.GetDirectionWithAim(out direction))
                {
                    return;
                }
                Profile.Instance.MsgPrint("You throw a dart of poison.");
                saveGame.SpellEffects.FireBolt(new ProjectPoison(SaveGame.Instance.SpellEffects), direction, player.Level);
            }
            player.RedrawNeeded.Set(RedrawFlag.PrHp | RedrawFlag.PrVis);
        }
    }
}