// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.PlayerRace.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cthangband.PlayerRace
{
    [Serializable]
    internal class PlayerRaceHuman : BasePlayerRace
    {
        private int[] _abilityBonus = { 0, 0, 0, 0, 0, 0 };
        public override int[] AbilityBonus => _abilityBonus;

        public override int AgeRange => 6;
        public override int BaseAge => 14;
        public override int BaseDeviceBonus => 0;

        public override int BaseDisarmBonus => 0;

        public override int BaseMeleeAttackBonus => 0;
        public override int BaseRangedAttackBonus => 0;
        public override int BaseSaveBonus => 0;
        public override int BaseSearchBonus => 0;
        public override int BaseSearchFrequency => 10;
        public override int BaseStealthBonus => 0;
        public override string Description2 => "Hopefully you know all about humans already because you";
        public override string Description3 => "are one! In game terms, humans are the average around which";
        public override string Description4 => "the other races are measured. As such, humans get no";
        public override string Description5 => "special abilities, but they increase in level quicker than";
        public override string Description6 => "any other race. Humans are recommended for new players.";
        public override int ExperienceFactor => 100;
        public override int FemaleBaseHeight => 66;
        public override int FemaleBaseWeight => 150;
        public override int FemaleHeightRange => 4;
        public override int FemaleWeightRange => 20;
        public override int HitDieBonus => 10;
        public override int Infravision => 0;
        public override int MaleBaseHeight => 72;
        public override int MaleBaseWeight => 180;
        public override int MaleHeightRange => 6;
        public override int MaleWeightRange => 25;
        public override string Title => "Human";

        protected override int BackgroundStartingChart => 1;

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
    }
}