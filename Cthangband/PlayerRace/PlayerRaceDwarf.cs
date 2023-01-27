// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.PlayerRace.Base;
using Cthangband.Projection.Base;
using Cthangband.Projection;
using Cthangband.StaticData;
using Cthangband.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cthangband.PlayerRace
{
    [Serializable]
    internal class PlayerRaceDwarf : BasePlayerRace
    {
        private int[] _abilityBonus = { 2, -2, 2, -2, 2, -3 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AgeRange => 15;
        public override int BaseAge => 35;

        public override int BaseDeviceBonus => 9;

        public override int BaseDisarmBonus => 2;

        public override int BaseMeleeAttackBonus => 15;

        public override int BaseRangedAttackBonus => 0;

        public override int BaseSaveBonus => 10;

        public override int BaseSearchBonus => 7;

        public override int BaseSearchFrequency => 10;

        public override int BaseStealthBonus => -1;

        public override string Description1 => "Dwarves are short and stocky, and although not noted for";

        public override string Description2 => "their intelligence or subtlety they are generally very";

        public override string Description3 => "pious. They are also rather resistant to spells. As natural";

        public override string Description4 => "miners, used to feeling their way around in the dark,";

        public override string Description5 => "dwarves are immune to all forms of blindness and can learn";

        public override string Description6 => "to detect secret doors and traps (at lvl 5).";

        public override int ExperienceFactor => 125;

        public override int FemaleBaseHeight => 46;

        public override int FemaleBaseWeight => 120;

        public override int FemaleHeightRange => 3;

        public override int FemaleWeightRange => 10;

        public override int HitDieBonus => 11;

        public override int Infravision => 5;

        public override int MaleBaseHeight => 48;

        public override int MaleBaseWeight => 150;

        public override int MaleHeightRange => 3;

        public override int MaleWeightRange => 10;

        public override string Title => "Dwarf";

        protected override int BackgroundStartingChart => 16;

        public override void ApplyRacialStatus(Player player)
        {
            player.HasBlindnessResistance = true;
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

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            f2.Set(ItemFlag2.ResBlind);
        }

        public override string GetRacialPowerText(Player player)
        {
            return player.Level < 5 ? "detect doors+traps (racial, unusable until level 5)" : "detect doors+traps (racial, cost 5, WIS based)";
        }

        public override List<string> SelfKnowledge(Player player)
        {
            var list = new List<string>();
            if (player.Level > 4)
            {
                list.Add("You can find traps, doors and stairs (cost 5).");
            }
            return list;
        }

        /// <summary>
        /// Use the player's racial power, if they have one
        /// </summary>
        public override void UseRacialPower(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(5, 5, Ability.Wisdom, 12))
            {
                Profile.Instance.MsgPrint("You examine your surroundings.");
                saveGame.SpellEffects.DetectTraps();
                saveGame.SpellEffects.DetectDoors();
                saveGame.SpellEffects.DetectStairs();
            }
            player.RedrawNeeded.Set(RedrawFlag.PrHp | RedrawFlag.PrVis);
        }
    }
}