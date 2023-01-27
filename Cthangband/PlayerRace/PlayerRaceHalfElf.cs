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
    internal class PlayerRaceHalfElf : BasePlayerRace
    {
        private int[] _abilityBonus = { -1, 1, 1, 1, -1, 1 };
        public override int[] AbilityBonus => _abilityBonus;

        public override int AgeRange => 16;

        public override int BaseAge => 24;

        public override int BaseDeviceBonus => 3;

        public override int BaseDisarmBonus => 2;

        public override int BaseMeleeAttackBonus => -1;

        public override int BaseRangedAttackBonus => 5;

        public override int BaseSaveBonus => 3;

        public override int BaseSearchBonus => 6;

        public override int BaseSearchFrequency => 11;

        public override int BaseStealthBonus => 1;

        public override string Description2 => "Half-Elves inherit better ability scores and skills from";
        public override string Description3 => "their elven parent, but none of that parent's special";
        public override string Description4 => "abilities. However, a half elf will advance in level more";
        public override string Description5 => "quickly than a full elf.";
        public override int ExperienceFactor => 110;

        public override int FemaleBaseHeight => 62;

        public override int FemaleBaseWeight => 100;

        public override int FemaleHeightRange => 6;

        public override int FemaleWeightRange => 10;

        public override int HitDieBonus => 9;

        public override int Infravision => 2;

        public override int MaleBaseHeight => 66;

        public override int MaleBaseWeight => 130;

        public override int MaleHeightRange => 6;

        public override int MaleWeightRange => 15;

        public override string Title => "Half Elf";

        protected override int BackgroundStartingChart => 4;

        /// <summary>
        /// Create a random name for a character based on their race.
        /// </summary>
        /// <returns> The random name </returns>
        public override string CreateRandomName()
        {
            string name = "";
            do
            {
                name = _elfSyllable1[Program.Rng.RandomLessThan(_elfSyllable1.Length)];
                name += _elfSyllable2[Program.Rng.RandomLessThan(_elfSyllable2.Length)];
                name += _elfSyllable3[Program.Rng.RandomLessThan(_elfSyllable3.Length)];
            } while (name.Length > 12);
            return name;
        }
    }
}