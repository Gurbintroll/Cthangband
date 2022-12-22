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
    internal class PlayerRaceHalfOrc : BasePlayerRace
    {
        private int[] _abilityBonus = { 2, -1, 0, 0, 1, -4 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AgeRange => 4;
        public override int BaseAge => 11;

        public override int BaseDeviceBonus => -3;

        public override int BaseDisarmBonus => -3;

        public override int BaseMeleeAttackBonus => 12;

        public override int BaseRangedAttackBonus => -5;

        public override int BaseSaveBonus => -3;

        public override int BaseSearchBonus => 0;

        public override int BaseSearchFrequency => 7;

        public override int BaseStealthBonus => -1;

        public override uint Choice => 0x898D;

        public override string Description2 => "Half-Orcs are stronger than humans, and less dimwitted";

        public override string Description3 => "their orcish parentage would lead you to assume.";

        public override string Description4 => "Half-Orcs are born of darkness and are resistant to that";

        public override string Description5 => "form of attack. They are also able to learn to shrug off";

        public override string Description6 => "magical fear (at lvl 5).";

        public override int ExperienceFactor => 110;

        public override int FemaleBaseHeight => 62;

        public override int FemaleBaseWeight => 120;

        public override int FemaleHeightRange => 1;

        public override int FemaleWeightRange => 5;

        public override int HitDieBonus => 10;

        public override int Infravision => 3;

        public override int MaleBaseHeight => 66;

        public override int MaleBaseWeight => 150;

        public override int MaleHeightRange => 1;

        public override int MaleWeightRange => 5;

        public override string Title => "Half Orc";

        protected override int BackgroundStartingChart => 19;

        public override void ApplyRacialStatus(Player player)
        {
            player.HasDarkResistance = true;
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
                name = _orcSyllable1[Program.Rng.RandomLessThan(_orcSyllable1.Length)];
                name += _orcSyllable2[Program.Rng.RandomLessThan(_orcSyllable2.Length)];
                name += _orcSyllable3[Program.Rng.RandomLessThan(_orcSyllable3.Length)];
            } while (name.Length > 12);
            return name;
        }

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            f2.Set(ItemFlag2.ResDark);
        }

        public override string GetRacialPowerText(Player player)
        {
            return player.Level < 3 ? "remove fear        (racial, unusable until level 3)" : "remove fear        (racial, cost 5, WIS based)";
        }

        public override List<string> SelfKnowledge(Player player)
        {
            var list = new List<string>();
            if (player.Level > 2)
            {
                list.Add("You can remove fear (cost 5).");
            }
            return list;
        }

        public override void UseRacialPower(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(3, 5, Ability.Wisdom, player.CharacterClassIndex == CharacterClassId.Warrior ? 5 : 10))
            {
                Profile.Instance.MsgPrint("You play tough.");
                player.SetTimedFear(0);
            }
            player.RedrawNeeded.Set(RedrawFlag.PrHp | RedrawFlag.PrVis);
        }
    }
}