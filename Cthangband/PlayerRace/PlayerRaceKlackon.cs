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
    internal class PlayerRaceKlackon : BasePlayerRace
    {
        private int[] _abilityBonus = { 2, -1, -1, 1, 2, -2 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AgeRange => 3;

        public override int BaseAge => 20;

        public override int BaseDeviceBonus => 5;

        public override int BaseDisarmBonus => 10;

        public override int BaseMeleeAttackBonus => 5;

        public override int BaseRangedAttackBonus => 5;

        public override int BaseSaveBonus => 5;

        public override int BaseSearchBonus => -1;

        public override int BaseSearchFrequency => 10;

        public override int BaseStealthBonus => 0;

        public override string Description1 => "Klackons are humanoid insects. Although most stay safe in";

        public override string Description2 => "their hive cities, a small number venture forth in search";

        public override string Description3 => "of adventure. The chitin of a klackon resists acid, and";

        public override string Description4 => "their ordered minds cannot be confused. They can learn to";

        public override string Description5 => "spit acid (at lvl 9) and they get progressively faster if";

        public override string Description6 => "unencumbered by armour.";

        public override int ExperienceFactor => 135;

        public override int FemaleBaseHeight => 54;

        public override int FemaleBaseWeight => 70;

        public override int FemaleHeightRange => 3;

        public override int FemaleWeightRange => 4;

        public override bool HasSpeedBonus => true;

        public override int HitDieBonus => 12;

        public override int Infravision => 2;

        public override int MaleBaseHeight => 60;

        public override int MaleBaseWeight => 80;

        public override int MaleHeightRange => 3;

        public override int MaleWeightRange => 4;

        public override string Title => "Klackon";

        protected override int BackgroundStartingChart => 84;

        public override void ApplyRacialStatus(Player player)
        {
            player.HasConfusionResistance = true;
            player.HasAcidResistance = true;
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
                name = _klackonSyllable1[Program.Rng.RandomLessThan(_klackonSyllable1.Length)];
                name += _klackonSyllable2[Program.Rng.RandomLessThan(_klackonSyllable2.Length)];
                name += _klackonSyllable3[Program.Rng.RandomLessThan(_klackonSyllable3.Length)];
            } while (name.Length > 12);
            return name;
        }

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            if (player.Level > 9)
            {
                f1.Set(ItemFlag1.Speed);
            }
            f2.Set(ItemFlag2.ResConf);
            f2.Set(ItemFlag2.ResAcid);
        }

        public override string GetRacialPowerText(Player player)
        {
            return player.Level < 9 ? "spit acid          (racial, unusable until level 9)" : "spit acid          (racial, cost 9, dam lvl, DEX based)";
        }

        public override List<string> SelfKnowledge(Player player)
        {
            var list = new List<string>();
            if (player.Level > 8)
            {
                list.Add($"You can spit acid, dam. {player.Level} (cost 9).");
            }
            return list;
        }

        public override void UseRacialPower(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(9, 9, Ability.Dexterity, 14))
            {
                int direction;
                var targetEngine = new TargetEngine(player, level);
                if (!targetEngine.GetDirectionWithAim(out direction))
                {
                    return;
                }
                Profile.Instance.MsgPrint("You spit acid.");
                if (player.Level < 25)
                {
                    saveGame.SpellEffects.FireBolt(new ProjectAcid(SaveGame.Instance.SpellEffects), direction, player.Level);
                }
                else
                {
                    saveGame.SpellEffects.FireBall(new ProjectAcid(SaveGame.Instance.SpellEffects), direction, player.Level,
                        2);
                }
            }
            player.RedrawNeeded.Set(RedrawFlag.PrHp | RedrawFlag.PrVis);
        }
    }
}