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
    internal class PlayerRaceNibelung : BasePlayerRace
    {
        private int[] _abilityBonus = { 1, -1, 2, 0, 2, -4 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AgeRange => 12;
        public override int BaseAge => 40;

        public override int BaseDeviceBonus => 5;

        public override int BaseDisarmBonus => 3;

        public override int BaseMeleeAttackBonus => 9;

        public override int BaseRangedAttackBonus => 0;

        public override int BaseSaveBonus => 10;

        public override int BaseSearchBonus => 5;

        public override int BaseSearchFrequency => 10;

        public override int BaseStealthBonus => 1;

        public override string Description2 => "Nibelungen are also known as dark dwarves and are famous";

        public override string Description3 => "as the makers of (often cursed) magical items. They can";

        public override string Description4 => "resist darkness and protect the items they are carrying";

        public override string Description5 => "from disenchantment. They can also learn to detect traps,";

        public override string Description6 => "stairs, and secret doors (at lvl 5).";

        public override int ExperienceFactor => 135;

        public override int FemaleBaseHeight => 40;

        public override int FemaleBaseWeight => 78;

        public override int FemaleHeightRange => 3;

        public override int FemaleWeightRange => 3;

        public override int HitDieBonus => 11;

        public override int Infravision => 5;

        public override int MaleBaseHeight => 43;

        public override int MaleBaseWeight => 92;

        public override int MaleHeightRange => 3;

        public override int MaleWeightRange => 6;

        public override string Title => "Niebelung";

        protected override int BackgroundStartingChart => 87;

        public override void ApplyRacialStatus(Player player)
        {
            player.HasDisenchantResistance = true;
            player.HasDarkResistance = true;
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
                name = _dwarfSyllable1[Program.Rng.RandomLessThan(_dwarfSyllable1.Length)];
                name += _dwarfSyllable2[Program.Rng.RandomLessThan(_dwarfSyllable2.Length)];
                name += _dwarfSyllable3[Program.Rng.RandomLessThan(_dwarfSyllable3.Length)];
            } while (name.Length > 12);
            return name;
        }

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            f2.Set(ItemFlag2.ResDisen);
            f2.Set(ItemFlag2.ResDark);
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

        public override void UseRacialPower(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(5, 5, Ability.Wisdom, 10))
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