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
    internal class PlayerRaceMindFlayer : BasePlayerRace
    {
        private int[] _abilityBonus = { -3, 4, 4, 0, -2, -5 };
        public override int[] AbilityBonus => _abilityBonus;

        public override int AgeRange => 25;

        public override int BaseAge => 100;

        public override int BaseDeviceBonus => 25;

        public override int BaseDisarmBonus => 10;

        public override int BaseMeleeAttackBonus => -10;

        public override int BaseRangedAttackBonus => -5;

        public override int BaseSaveBonus => 15;

        public override int BaseSearchBonus => 5;

        public override int BaseSearchFrequency => 12;

        public override int BaseStealthBonus => 2;

        public override uint Choice => 0xD746;

        public override string Description2 => "Mind-Flayers are slimy humanoids with squid-like tentacles";

        public override string Description3 => "around their mouths. They are all psychic, and neither";

        public override string Description4 => "their intelligence nor their wisdom can be reduced. They";

        public override string Description5 => "can learn to see invisible (at lvl 15), blast people's";

        public override string Description6 => "minds (at lvl 15), and gain telepathy (at lvl 30).";

        public override int ExperienceFactor => 140;

        public override int FemaleBaseHeight => 63;

        public override int FemaleBaseWeight => 112;

        public override int FemaleHeightRange => 6;

        public override int FemaleWeightRange => 10;

        public override int HitDieBonus => 9;

        public override int Infravision => 4;

        public override int MaleBaseHeight => 68;

        public override int MaleBaseWeight => 142;

        public override int MaleHeightRange => 6;

        public override int MaleWeightRange => 15;

        public override bool SanityImmune => true;

        public override string Title => "Mind Flayer";

        protected override int BackgroundStartingChart => 92;

        public override void ApplyRacialStatus(Player player)
        {
            player.HasSustainIntelligence = true;
            player.HasSustainWisdom = true;
            if (player.Level > 14)
            {
                player.HasSeeInvisibility = true;
            }
            if (player.Level > 29)
            {
                player.HasTelepathy = true;
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
                name = _cthuloidSyllable1[Program.Rng.RandomLessThan(_cthuloidSyllable1.Length)];
                name += _cthuloidSyllable2[Program.Rng.RandomLessThan(_cthuloidSyllable2.Length)];
                name += _cthuloidSyllable3[Program.Rng.RandomLessThan(_cthuloidSyllable3.Length)];
            } while (name.Length > 12);
            return name;
        }

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            f2.Set(ItemFlag2.SustInt);
            f2.Set(ItemFlag2.SustWis);
            if (player.Level > 14)
            {
                f3.Set(ItemFlag3.SeeInvis);
            }
            if (player.Level > 29)
            {
                f3.Set(ItemFlag3.Telepathy);
            }
        }

        public override string GetRacialPowerText(Player player)
        {
            return player.Level < 15 ? "mind blast         (racial, unusable until level 15)" : "mind blast         (racial, cost 12, dam lvl, INT based)";
        }

        public override List<string> SelfKnowledge(Player player)
        {
            var list = new List<string>();
            if (player.Level > 14)
            {
                list.Add($"You can mind blast your enemies, dam {player.Level} (cost 12).");
            }
            return list;
        }

        public override void UseRacialPower(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(15, 12, Ability.Intelligence, 14))
            {
                int direction;
                TargetEngine targetEngine = new TargetEngine(player, level);
                if (!targetEngine.GetDirectionWithAim(out direction))
                {
                    return;
                }
                Profile.Instance.MsgPrint("You concentrate and your eyes glow red...");
                saveGame.SpellEffects.FireBolt(new ProjectPsi(SaveGame.Instance.SpellEffects), direction, player.Level);
            }
            player.RedrawNeeded.Set(RedrawFlag.PrHp | RedrawFlag.PrVis);
        }
    }
}