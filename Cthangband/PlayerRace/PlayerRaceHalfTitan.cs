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
    internal class PlayerRaceHalfTitan : BasePlayerRace
    {
        private int[] _abilityBonus = { 5, 1, 1, -2, 3, 1 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AgeRange => 30;
        public override int BaseAge => 100;

        public override int BaseDeviceBonus => 5;

        public override int BaseDisarmBonus => -5;

        public override int BaseMeleeAttackBonus => 25;

        public override int BaseRangedAttackBonus => 0;

        public override int BaseSaveBonus => 2;

        public override int BaseSearchBonus => 1;

        public override int BaseSearchFrequency => 8;

        public override int BaseStealthBonus => -2;

        public override uint Choice => 0x1F27;

        public override string Description2 => "Half-Titans are massively strong, being descended from the";

        public override string Description3 => "predecessors of the gods that grew from primal chaos. This";

        public override string Description4 => "legacy lets them resist damage from chaos, and half-titans";

        public override string Description5 => "can learn to magically probe their foes to find out their";

        public override string Description6 => "strengths and weaknesses (at lvl 35).";

        public override int ExperienceFactor => 255;

        public override int FemaleBaseHeight => 99;

        public override int FemaleBaseWeight => 250;

        public override int FemaleHeightRange => 11;

        public override int FemaleWeightRange => 86;

        public override int HitDieBonus => 14;

        public override int Infravision => 0;

        public override int MaleBaseHeight => 111;

        public override int MaleBaseWeight => 255;

        public override int MaleHeightRange => 11;

        public override int MaleWeightRange => 86;

        public override string Title => "Half Titan";

        protected override int BackgroundStartingChart => 76;

        public override void ApplyRacialStatus(Player player)
        {
            player.HasChaosResistance = true;
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

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            f2.Set(ItemFlag2.ResChaos);
        }

        public override string GetRacialPowerText(Player player)
        {
            return player.Level < 35 ? "probing            (racial, unusable until level 35)" : "probing            (racial, cost 20, INT based)";
        }

        public override List<string> SelfKnowledge(Player player)
        {
            var list = new List<string>();
            if (player.Level > 34)
            {
                list.Add("You can probe monsters (cost 20).");
            }
            return list;
        }

        public override void UseRacialPower(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(35, 20, Ability.Intelligence, 12))
            {
                Profile.Instance.MsgPrint("You examine your foes...");
                saveGame.SpellEffects.Probing();
            }
            player.RedrawNeeded.Set(RedrawFlag.PrHp | RedrawFlag.PrVis);
        }
    }
}