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
    internal class PlayerRaceMiriNigri : BasePlayerRace
    {
        private int[] _abilityBonus = { 2, -2, -1, -1, 2, -4 };
        public override int[] AbilityBonus => _abilityBonus;

        public override int AgeRange => 6;

        public override int BaseAge => 14;

        public override int BaseDeviceBonus => -2;

        public override int BaseDisarmBonus => -5;

        public override int BaseMeleeAttackBonus => 12;

        public override int BaseRangedAttackBonus => 5;

        public override int BaseSaveBonus => -1;

        public override int BaseSearchBonus => -1;

        public override int BaseSearchFrequency => 5;

        public override int BaseStealthBonus => -1;

        public override string Description1 => "Miri-Nigri are squat, toad-like chaos beasts. Their";

        public override string Description2 => "close ties to chaos render them resistant to sound and";

        public override string Description3 => "immune to confusion. However, their chaotic nature also";

        public override string Description4 => "makes them prone to random mutation. Also, the outer gods";

        public override string Description5 => "pay special attention to miri-nigri servants and they";

        public override string Description6 => "are more likely to interfere with them for good or ill.";

        public override int ExperienceFactor => 140;

        public override int FemaleBaseHeight => 61;

        public override int FemaleBaseWeight => 120;

        public override int FemaleHeightRange => 6;

        public override int FemaleWeightRange => 15;

        public override int HitDieBonus => 11;

        public override int Infravision => 0;

        public override int MaleBaseHeight => 65;

        public override int MaleBaseWeight => 150;

        public override int MaleHeightRange => 6;

        public override int MaleWeightRange => 20;

        public override bool Mutates => true;

        public override bool SanityImmune => true;

        public override string Title => "Miri Nigri";

        protected override int BackgroundStartingChart => 129;

        public override void ApplyRacialStatus(Player player)
        {
            player.HasConfusionResistance = true;
            player.HasSoundResistance = true;
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
            f2.Set(ItemFlag2.ResSound);
            f2.Set(ItemFlag2.ResConf);
        }
    }
}