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

namespace Cthangband.PlayerRace
{
    [Serializable]
    internal class PlayerRaceElf : BasePlayerRace
    {
        private int[] _abilityBonus = { -1, 2, 2, 1, -2, 2 };
        public override int[] AbilityBonus => _abilityBonus;

        public override int AgeRange => 75;

        public override string Article => "an";

        public override int BaseAge => 75;

        public override int BaseDeviceBonus => 6;

        public override int BaseDisarmBonus => 5;

        public override int BaseMeleeAttackBonus => -5;

        public override int BaseRangedAttackBonus => 15;

        public override int BaseSaveBonus => 6;

        public override int BaseSearchBonus => 8;

        public override int BaseSearchFrequency => 12;

        public override int BaseStealthBonus => 2;

        public override string Description2 => "Elves are creatures of the woods, and cultivate a symbiotic";

        public override string Description3 => "relationship with trees. While not the sturdiest of races,";

        public override string Description4 => "they are dextrous and have excellent mental faculties.";

        public override string Description5 => "Because they are partially photosynthetic, elves are able";

        public override string Description6 => "to resist light based attacks.";

        public override int ExperienceFactor => 120;

        public override int FemaleBaseHeight => 54;

        public override int FemaleBaseWeight => 80;

        public override int FemaleHeightRange => 4;

        public override int FemaleWeightRange => 6;

        public override int HitDieBonus => 8;

        public override int Infravision => 3;

        public override int MaleBaseHeight => 60;

        public override int MaleBaseWeight => 100;

        public override int MaleHeightRange => 4;

        public override int MaleWeightRange => 6;

        public override string Title => "Elf";

        protected override int BackgroundStartingChart => 7;

        public override void ApplyRacialStatus(Player player)
        {
            player.HasLightResistance = true;
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
                name = _elfSyllable1[Program.Rng.RandomLessThan(_elfSyllable1.Length)];
                name += _elfSyllable2[Program.Rng.RandomLessThan(_elfSyllable2.Length)];
                name += _elfSyllable3[Program.Rng.RandomLessThan(_elfSyllable3.Length)];
            } while (name.Length > 12);
            return name;
        }

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            f2.Set(ItemFlag2.ResLight);
        }
    }
}