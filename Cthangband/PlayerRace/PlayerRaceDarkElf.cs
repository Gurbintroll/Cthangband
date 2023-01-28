// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.PlayerRace.Base;
using Cthangband.Projection;
using System;
using System.Collections.Generic;

namespace Cthangband.PlayerRace
{
    [Serializable]
    internal class PlayerRaceDarkElf : BasePlayerRace
    {
        private int[] _abilityBonus = { -1, 3, 2, 2, -2, 1 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AgeRange => 75;
        public override int BaseAge => 75;

        public override int BaseDeviceBonus => 15;

        public override int BaseDisarmBonus => 5;

        public override int BaseMeleeAttackBonus => -5;

        public override int BaseRangedAttackBonus => 10;

        public override int BaseSaveBonus => 20;

        public override int BaseSearchBonus => 8;

        public override int BaseSearchFrequency => 12;

        public override int BaseStealthBonus => 3;

        public override string Description1 => "Dark elves are underground elves who have a kinship with";

        public override string Description2 => "fungi the way that surface elves have a kinship with trees.";

        public override string Description3 => "The innately magical nature of dark elves lets them learn";

        public override string Description4 => "to fire magical missiles at their opponents (at lvl 2).";

        public override string Description5 => "They also resist dark-based attacks and can learn to see";

        public override string Description6 => "invisible creatures (at lvl 20).";

        public override int ExperienceFactor => 150;

        public override int FemaleBaseHeight => 54;

        public override int FemaleBaseWeight => 80;

        public override int FemaleHeightRange => 4;

        public override int FemaleWeightRange => 6;

        public override int HitDieBonus => 9;

        public override int Infravision => 5;

        public override int MaleBaseHeight => 60;

        public override int MaleBaseWeight => 100;

        public override int MaleHeightRange => 4;

        public override int MaleWeightRange => 6;

        public override string Title => "Dark Elf";

        protected override int BackgroundStartingChart => 69;

        public override void ApplyRacialStatus(Player player)
        {
            player.HasDarkResistance = true;
            if (player.Level > 19)
            {
                player.HasSeeInvisibility = true;
            }
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
            f2.Set(ItemFlag2.ResDark);
            if (player.Level > 19)
            {
                f3.Set(ItemFlag3.SeeInvis);
            }
        }

        public override string GetRacialPowerText(Player player)
        {
            return player.Level < 2 ? "magic missile      (racial, unusable until level 2)" : "magic missile      (racial, cost 2, INT based)";
        }

        public override List<string> SelfKnowledge(Player player)
        {
            var list = new List<string>();
            if (player.Level > 1)
            {
                list.Add($"You can cast a Magic Missile, dam {3 + ((player.Level - 1) / 5)} (cost 2).");
            }
            return list;
        }

        public override void UseRacialPower(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(2, 2, Ability.Intelligence, 9))
            {
                int direction;
                var targetEngine = new TargetEngine(player, level);
                if (!targetEngine.GetDirectionWithAim(out direction))
                {
                    return;
                }
                Profile.Instance.MsgPrint("You cast a magic missile.");
                saveGame.SpellEffects.FireBoltOrBeam(10, new ProjectMissile(SaveGame.Instance.SpellEffects),
                    direction, Program.Rng.DiceRoll(3 + ((player.Level - 1) / 5), 4));
            }
            player.RedrawNeeded.Set(RedrawFlag.PrHp | RedrawFlag.PrVis);
        }
    }
}