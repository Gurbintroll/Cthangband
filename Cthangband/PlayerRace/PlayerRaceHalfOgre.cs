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
    internal class PlayerRaceHalfOgre : BasePlayerRace
    {
        private int[] _abilityBonus = { 3, -1, -1, -1, 3, -3 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AgeRange => 10;
        public override int BaseAge => 40;

        public override int BaseDeviceBonus => -5;

        public override int BaseDisarmBonus => -3;

        public override int BaseMeleeAttackBonus => 20;

        public override int BaseRangedAttackBonus => 0;

        public override int BaseSaveBonus => -5;

        public override int BaseSearchBonus => -1;

        public override int BaseSearchFrequency => 5;

        public override int BaseStealthBonus => -2;

        public override string Description2 => "Half-Ogres are both strong and naturally magical, although";

        public override string Description3 => "they don't usually have the intelligence to make the most";

        public override string Description4 => "of their magic. They resist darkness and can't have their";

        public override string Description5 => "strength reduced. They can also can enter a berserk";

        public override string Description6 => "rage (at lvl 8).";

        public override int ExperienceFactor => 130;

        public override int FemaleBaseHeight => 80;

        public override int FemaleBaseWeight => 235;

        public override int FemaleHeightRange => 8;

        public override int FemaleWeightRange => 60;

        public override int HitDieBonus => 12;

        public override int Infravision => 3;

        public override int MaleBaseHeight => 92;

        public override int MaleBaseWeight => 255;

        public override int MaleHeightRange => 10;

        public override int MaleWeightRange => 60;

        public override string Title => "Half Ogre";

        protected override int BackgroundStartingChart => 74;

        public override void ApplyRacialStatus(Player player)
        {
            player.HasDarkResistance = true;
            player.HasSustainStrength = true;
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
                name = _orcSyllable1[Program.Rng.RandomLessThan(_orcSyllable1.Length)];
                name += _orcSyllable2[Program.Rng.RandomLessThan(_orcSyllable2.Length)];
                name += _orcSyllable3[Program.Rng.RandomLessThan(_orcSyllable3.Length)];
            } while (name.Length > 12);
            return name;
        }

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            f2.Set(ItemFlag2.SustStr);
            f2.Set(ItemFlag2.ResDark);
        }

        public override string GetRacialPowerText(Player player)
        {
            return player.Level < 8 ? "berserk            (racial, unusable until level 8)" : "berserk            (racial, cost 10, WIS based)";
        }

        public override List<string> SelfKnowledge(Player player)
        {
            var list = new List<string>();
            if (player.Level > 7)
            {
                list.Add("You can enter berserk fury (cost 10).");
            }
            return list;
        }

        public override void UseRacialPower(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(8, 10, Ability.Wisdom, player.PlayerClass.PrimeAbilityScore == Ability.Strength ? 6 : 12))
            {
                Profile.Instance.MsgPrint("Raaagh!");
                player.SetTimedFear(0);
                player.SetTimedSuperheroism(player.TimedSuperheroism + 10 + Program.Rng.DieRoll(player.Level));
                player.RestoreHealth(30);
            }
            player.RedrawNeeded.Set(RedrawFlag.PrHp | RedrawFlag.PrVis);
        }
    }
}