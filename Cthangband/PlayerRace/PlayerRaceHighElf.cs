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
    internal class PlayerRaceHighElf : BasePlayerRace
    {
        private int[] _abilityBonus = { 1, 3, 2, 3, 1, 5 };
        public override int[] AbilityBonus => _abilityBonus;

        public override int AgeRange => 30;

        public override int BaseAge => 100;

        public override int BaseDeviceBonus => 20;

        public override int BaseDisarmBonus => 4;

        public override int BaseMeleeAttackBonus => 10;

        public override int BaseRangedAttackBonus => 25;

        public override int BaseSaveBonus => 20;

        public override int BaseSearchBonus => 3;

        public override int BaseSearchFrequency => 14;

        public override int BaseStealthBonus => 4;

        public override uint Choice => 0xBF5F;

        public override string Description2 => "High-Elves are the leaders of the elven race. They are";

        public override string Description3 => "more magical than their lesser cousins, but retain their";

        public override string Description4 => "affinity with nature. High-elves resist light based attacks";

        public override string Description5 => "and their acute senses are able to see invisible creatures.";

        public override int ExperienceFactor => 200;

        public override int FemaleBaseHeight => 82;

        public override int FemaleBaseWeight => 180;

        public override int FemaleHeightRange => 10;

        public override int FemaleWeightRange => 15;

        public override int HitDieBonus => 10;

        public override int Infravision => 4;

        public override int MaleBaseHeight => 90;

        public override int MaleBaseWeight => 190;

        public override int MaleHeightRange => 10;

        public override int MaleWeightRange => 20;

        public override string Title => "High Elf";

        protected override int BackgroundStartingChart => 7;

        public override void ApplyRacialStatus(Player player)
        {
            player.HasSeeInvisibility = true;
            player.HasLightResistance = true;
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
                name = _elfSyllable1[Program.Rng.RandomLessThan(_elfSyllable1.Length)];
                name += _elfSyllable2[Program.Rng.RandomLessThan(_elfSyllable2.Length)];
                name += _elfSyllable3[Program.Rng.RandomLessThan(_elfSyllable3.Length)];
            } while (name.Length > 12);
            return name;
        }

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            f2.Set(ItemFlag2.ResLight);
            f3.Set(ItemFlag3.SeeInvis);
        }
    }
}