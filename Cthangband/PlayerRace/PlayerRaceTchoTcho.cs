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
    internal class PlayerRaceTchoTcho : BasePlayerRace
    {
        private int[] _abilityBonus = { 3, -2, -1, 1, 2, -2 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AgeRange => 8;

        public override int BaseAge => 14;

        public override int BaseDeviceBonus => -10;

        public override int BaseDisarmBonus => -2;

        public override int BaseMeleeAttackBonus => 12;

        public override int BaseRangedAttackBonus => 10;

        public override int BaseSaveBonus => 2;

        public override int BaseSearchBonus => 1;

        public override int BaseSearchFrequency => 7;

        public override int BaseStealthBonus => -1;

        public override string Description2 => "Tcho-Tchos are hairless cannibalistic near-humans who dwell";

        public override string Description3 => "in isolated parts of the world away from more civilised";

        public override string Description4 => "places where their dark rituals and sacrifices go unseen.";

        public override string Description5 => "Tcho-Tchos are immune to fear, and can also learn to create";

        public override string Description6 => "The Yellow Sign (at lvl 25).";

        public override int ExperienceFactor => 120;

        public override int FemaleBaseHeight => 78;

        public override int FemaleBaseWeight => 190;

        public override int FemaleHeightRange => 6;

        public override int FemaleWeightRange => 15;

        public override int HitDieBonus => 11;

        public override int Infravision => 0;

        public override int MaleBaseHeight => 82;

        public override int MaleBaseWeight => 200;

        public override int MaleHeightRange => 5;

        public override int MaleWeightRange => 20;

        public override bool ResistsFear => true;

        public override bool SanityResistant => true;

        public override string Title => "Tcho-Tcho";

        protected override int BackgroundStartingChart => 138;

        public override void ApplyRacialStatus(Player player)
        {
            player.HasFearResistance = true;
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
            f2.Set(ItemFlag2.ResFear);
        }

        public override string GetRacialPowerText(Player player)
        {
            return player.Level < 25 ? "Yellow Sign     (racial, unusable until level 25)" : "Yellow Sign     (racial, cost 35, INT based)";
        }

        public override List<string> SelfKnowledge(Player player)
        {
            var list = new List<string>();
            if (player.Level > 24)
            {
                list.Add("You can set an Yellow Sign (cost 35).");
            }
            return list;
        }

        public override void UseRacialPower(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(25, 35, Ability.Intelligence, 15))
            {
                Profile.Instance.MsgPrint("You carefully draw The Yellow Sign...");
                saveGame.SpellEffects.YellowSign();
            }
            player.RedrawNeeded.Set(RedrawFlag.PrHp | RedrawFlag.PrVis);
        }
    }
}