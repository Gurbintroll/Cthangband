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
    internal class PlayerRaceHalfGiant : BasePlayerRace
    {
        private int[] _abilityBonus = { 4, -2, -2, -2, 3, -3 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AgeRange => 10;

        public override int BaseAge => 40;

        public override int BaseDeviceBonus => -8;

        public override int BaseDisarmBonus => -6;
        public override int BaseMeleeAttackBonus => 25;

        public override int BaseRangedAttackBonus => 5;

        public override int BaseSaveBonus => -6;

        public override int BaseSearchBonus => -1;

        public override int BaseSearchFrequency => 5;

        public override int BaseStealthBonus => -2;

        public override string Description2 => "Half-Giants are immensely strong and tough, and their skin";

        public override string Description3 => "is stony. They can't have their strength reduced, and they";

        public override string Description4 => "resist damage from explosions that throw out shards of";

        public override string Description5 => "stone and metal. They can learn to soften rock into mud";

        public override string Description6 => "(at lvl 20).";

        public override int ExperienceFactor => 150;

        public override int FemaleBaseHeight => 80;

        public override int FemaleBaseWeight => 240;

        public override int FemaleHeightRange => 10;

        public override int FemaleWeightRange => 64;

        public override int HitDieBonus => 13;

        public override int Infravision => 3;

        public override int MaleBaseHeight => 100;

        public override int MaleBaseWeight => 255;

        public override int MaleHeightRange => 10;

        public override int MaleWeightRange => 65;

        public override string Title => "Half Giant";

        protected override int BackgroundStartingChart => 75;

        public override void ApplyRacialStatus(Player player)
        {
            player.HasSustainStrength = true;
            player.HasShardResistance = true;
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
                name = _dwarfSyllable1[Program.Rng.RandomLessThan(_dwarfSyllable1.Length)];
                name += _dwarfSyllable2[Program.Rng.RandomLessThan(_dwarfSyllable2.Length)];
                name += _dwarfSyllable3[Program.Rng.RandomLessThan(_dwarfSyllable3.Length)];
            } while (name.Length > 12);
            return name;
        }

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            f2.Set(ItemFlag2.ResShards);
            f2.Set(ItemFlag2.SustStr);
        }

        public override string GetRacialPowerText(Player player)
        {
            return player.Level < 20 ? "stone to mud       (racial, unusable until level 20)" : "stone to mud       (racial, cost 10, STR based)";
        }

        public override List<string> SelfKnowledge(Player player)
        {
            var list = new List<string>();
            if (player.Level > 19)
            {
                list.Add("You can break stone walls (cost 10).");
            }
            return list;
        }

        public override void UseRacialPower(SaveGame saveGame, Player player, Level level)
        {
            int direction;
            var targetEngine = new TargetEngine(player, level);
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(20, 10, Ability.Strength, 12))
            {
                if (!targetEngine.GetDirectionWithAim(out direction))
                {
                    return;
                }
                Profile.Instance.MsgPrint("You bash at a stone wall.");
                saveGame.SpellEffects.WallToMud(direction);
            }
            player.RedrawNeeded.Set(RedrawFlag.PrHp | RedrawFlag.PrVis);
        }
    }
}