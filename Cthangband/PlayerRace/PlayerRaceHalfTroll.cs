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
    internal class PlayerRaceHalfTroll : BasePlayerRace
    {
        private int[] _abilityBonus = { 4, -4, -2, -4, 3, -6 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AgeRange => 10;
        public override int BaseAge => 20;

        public override int BaseDeviceBonus => -8;

        public override int BaseDisarmBonus => -5;

        public override int BaseMeleeAttackBonus => 20;

        public override int BaseRangedAttackBonus => -10;

        public override int BaseSaveBonus => -8;

        public override int BaseSearchBonus => -1;

        public override int BaseSearchFrequency => 5;

        public override int BaseStealthBonus => -2;

        public override uint Choice => 0x0805;

        public override string Description1 => "Half-Trolls make up for their stupidity by being almost";

        public override string Description2 => "pure muscle, as strong as creatures much larger than they.";

        public override string Description3 => "They can't have their strength reduced, and as they grow";

        public override string Description4 => "stronger they can go into a berserk rage (at lvl 10),";

        public override string Description5 => "regenerate wounds (at lvl 15), and survive on less food";

        public override string Description6 => "(at lvl 15).";

        public override int ExperienceFactor => 137;

        public override int FemaleBaseHeight => 84;

        public override int FemaleBaseWeight => 225;

        public override int FemaleHeightRange => 8;

        public override int FemaleWeightRange => 40;

        public override int HitDieBonus => 12;

        public override int Infravision => 3;

        public override int MaleBaseHeight => 96;

        public override int MaleBaseWeight => 250;

        public override int MaleHeightRange => 10;

        public override int MaleWeightRange => 50;

        public override string Title => "Half Troll";

        protected override int BackgroundStartingChart => 22;

        public override void ApplyRacialStatus(Player player)
        {
            player.HasSustainStrength = true;
            if (player.Level > 14)
            {
                player.HasRegeneration = true;
                player.HasSlowDigestion = true;
            }
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
            f2.Set(ItemFlag2.SustStr);
            if (player.Level > 14)
            {
                f3.Set(ItemFlag3.Regen);
                f3.Set(ItemFlag3.SlowDigest);
            }
        }

        public override string GetRacialPowerText(Player player)
        {
            return player.Level < 10 ? "berserk            (racial, unusable until level 10)" : "berserk            (racial, cost 12, WIS based)";
        }

        public override List<string> SelfKnowledge(Player player)
        {
            var list = new List<string>();
            if (player.Level > 9)
            {
                list.Add("You enter berserk fury (cost 12).");
            }
            return list;
        }

        public override void UseRacialPower(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(10, 12, Ability.Wisdom, player.ProfessionIndex == CharacterClass.Warrior ? 6 : 12))
            {
                Profile.Instance.MsgPrint("RAAAGH!");
                player.SetTimedFear(0);
                player.SetTimedSuperheroism(player.TimedSuperheroism + 10 + Program.Rng.DieRoll(player.Level));
                player.RestoreHealth(30);
            }
            player.RedrawNeeded.Set(RedrawFlag.PrHp | RedrawFlag.PrVis);
        }
    }
}