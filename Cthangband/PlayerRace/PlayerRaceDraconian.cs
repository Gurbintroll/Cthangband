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
using Cthangband.Projection.Base;
using System;
using System.Collections.Generic;

namespace Cthangband.PlayerRace
{
    [Serializable]
    internal class PlayerRaceDraconian : BasePlayerRace
    {
        private int[] _abilityBonus = { 2, 1, 1, 1, 2, -3 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AgeRange => 33;

        public override int BaseAge => 75;

        public override int BaseDeviceBonus => 5;
        public override int BaseDisarmBonus => -2;

        public override int BaseMeleeAttackBonus => 5;

        public override int BaseRangedAttackBonus => 5;

        public override int BaseSaveBonus => 3;

        public override int BaseSearchBonus => 1;

        public override int BaseSearchFrequency => 10;

        public override int BaseStealthBonus => 0;

        public override string Description1 => "Draconians are related to dragons and this shows both in";

        public override string Description2 => "their physical superiority and their legendary arrogance.";

        public override string Description3 => "As well as having a breath weapon, their wings let them";

        public override string Description4 => "avoid falling damage, and they can learn to resist fire";

        public override string Description5 => "(at lvl 5), cold (at lvl 10), acid (at lvl 15), lightning";

        public override string Description6 => "(at lvl 20), and poison (at lvl 35).";

        public override int ExperienceFactor => 250;

        public override int FemaleBaseHeight => 72;

        public override int FemaleBaseWeight => 130;

        public override int FemaleHeightRange => 1;

        public override int FemaleWeightRange => 5;

        public override int HitDieBonus => 11;

        public override int Infravision => 2;

        public override int MaleBaseHeight => 76;

        public override int MaleBaseWeight => 160;

        public override int MaleHeightRange => 1;

        public override int MaleWeightRange => 5;

        public override string Title => "Draconian";

        protected override int BackgroundStartingChart => 89;

        public override void ApplyRacialStatus(Player player)
        {
            player.HasFeatherFall = true;
            if (player.Level > 4)
            {
                player.HasFireResistance = true;
            }
            if (player.Level > 9)
            {
                player.HasColdResistance = true;
            }
            if (player.Level > 14)
            {
                player.HasAcidResistance = true;
            }
            if (player.Level > 19)
            {
                player.HasLightningResistance = true;
            }
            if (player.Level > 34)
            {
                player.HasPoisonResistance = true;
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
                name = _gnomeSyllable1[Program.Rng.RandomLessThan(_gnomeSyllable1.Length)];
                name += _gnomeSyllable2[Program.Rng.RandomLessThan(_gnomeSyllable2.Length)];
                name += _gnomeSyllable3[Program.Rng.RandomLessThan(_gnomeSyllable3.Length)];
            } while (name.Length > 12);
            return name;
        }

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            f3.Set(ItemFlag3.Feather);
            if (player.Level > 4)
            {
                f2.Set(ItemFlag2.ResFire);
            }
            if (player.Level > 9)
            {
                f2.Set(ItemFlag2.ResCold);
            }
            if (player.Level > 14)
            {
                f2.Set(ItemFlag2.ResAcid);
            }
            if (player.Level > 19)
            {
                f2.Set(ItemFlag2.ResElec);
            }
            if (player.Level > 34)
            {
                f2.Set(ItemFlag2.ResPois);
            }
        }

        public override string GetRacialPowerText(Player player)
        {
            return "breath weapon      (racial, cost lvl, dam 2*lvl, CON based)";
        }

        public override List<string> SelfKnowledge(Player player)
        {
            var list = new List<string>();
            list.Add($"You can breathe, dam. {2 * player.Level} (cost {player.Level}).");
            return list;
        }

        public override void UseRacialPower(SaveGame saveGame, Player player, Level level)
        {
            IProjection projectile;
            string projectileDescription;
            // Default to being randomly fire (66% chance) or cold (33% chance)
            if (Program.Rng.DieRoll(3) == 1)
            {
                projectile = new ProjectCold(SaveGame.Instance.SpellEffects);
                projectileDescription = "cold";
            }
            else
            {
                projectile = new ProjectFire(SaveGame.Instance.SpellEffects);
                projectileDescription = "fire";
            }
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(1, player.Level, Ability.Constitution, 12))
            {
                int direction;
                var targetEngine = new TargetEngine(player, level);
                if (!targetEngine.GetDirectionWithAim(out direction))
                {
                    return;
                }
                Profile.Instance.MsgPrint($"You breathe {projectileDescription}.");
                saveGame.SpellEffects.FireBall(projectile, direction, player.Level * 2, -(player.Level / 15) + 1);
            }
            player.RedrawNeeded.Set(RedrawFlag.PrHp | RedrawFlag.PrVis);
        }
    }
}