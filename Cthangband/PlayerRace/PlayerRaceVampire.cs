// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.PlayerRace.Base;
using Cthangband.StaticData;
using System;
using System.Collections.Generic;

namespace Cthangband.PlayerRace
{
    [Serializable]
    internal class PlayerRaceVampire : BasePlayerRace
    {
        private int[] _abilityBonus = { 3, 3, -1, -1, 1, 2 };

        public override int[] AbilityBonus => _abilityBonus;

        public override int AgeRange => 30;

        public override int BaseAge => 100;

        public override int BaseDeviceBonus => 10;

        public override int BaseDisarmBonus => 4;

        public override int BaseMeleeAttackBonus => 5;

        public override int BaseRangedAttackBonus => 0;

        public override int BaseSaveBonus => 10;

        public override int BaseSearchBonus => 1;

        public override int BaseSearchFrequency => 8;

        public override int BaseStealthBonus => 4;

        public override string Description2 => "Vampires are powerful undead. They resist darkness, nether,";

        public override string Description3 => "cold, poison, and having their life force drained. Vampires";

        public override string Description4 => "produce their own ethereal light in the dark, but are hurt";

        public override string Description5 => "by direct sunlight. They can learn to drain the life force";

        public override string Description6 => "from their foes (at lvl 2).";

        public override bool DoesntEat => true;

        public override int ExperienceFactor => 200;

        public override int FemaleBaseHeight => 66;

        public override int FemaleBaseWeight => 150;

        public override int FemaleHeightRange => 4;

        public override int FemaleWeightRange => 20;

        public override bool Glows => true;

        public override int HitDieBonus => 11;

        public override int Infravision => 5;

        public override bool IsNocturnal => true;

        public override bool IsSunlightSensitive => true;

        public override int MaleBaseHeight => 72;

        public override int MaleBaseWeight => 180;

        public override int MaleHeightRange => 6;

        public override int MaleWeightRange => 25;

        public override bool SanityResistant => true;

        public override string Title => "Vampire";

        protected override int BackgroundStartingChart => 113;

        public override void ApplyRacialStatus(Player player)
        {
            player.HasDarkResistance = true;
            player.HasHoldLife = true;
            player.HasNetherResistance = true;
            player.HasColdResistance = true;
            player.HasPoisonResistance = true;
            player.HasGlow = true;
        }

        public override void ConsumeFood(Player player, Item item)
        {
            _ = player.SetFood(player.Food + (item.TypeSpecificValue / 10));
            Profile.Instance.MsgPrint("Mere victuals hold scant sustenance for a being such as yourself.");
            if (player.Food < Constants.PyFoodAlert)
            {
                Profile.Instance.MsgPrint("Your hunger can only be satisfied with fresh blood!");
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
                name = _humanSyllable1[Program.Rng.RandomLessThan(_humanSyllable1.Length)];
                name += _humanSyllable2[Program.Rng.RandomLessThan(_humanSyllable2.Length)];
                name += _humanSyllable3[Program.Rng.RandomLessThan(_humanSyllable3.Length)];
            } while (name.Length > 12);
            return name;
        }

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            f2.Set(ItemFlag2.HoldLife);
            f2.Set(ItemFlag2.ResDark);
            f2.Set(ItemFlag2.ResNether);
            f3.Set(ItemFlag3.Lightsource);
            f2.Set(ItemFlag2.ResPois);
            f2.Set(ItemFlag2.ResCold);
        }

        public override string GetRacialPowerText(Player player)
        {
            return player.Level < 2 ? "drain life         (racial, unusable until level 2)" : "drain life         (racial, cost 1 + lvl/3, based)";
        }

        public override List<string> SelfKnowledge(Player player)
        {
            var list = new List<string>();
            if (player.Level > 1)
            {
                list.Add($"You can steal life from a foe, dam. {player.Level + Math.Max(1, player.Level / 10)}-{player.Level + (player.Level * Math.Max(1, player.Level / 10))} (cost {1 + (player.Level / 3)}).");
            }
            return list;
        }

        public override void UseRacialPower(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(2, 1 + (player.Level / 3), Ability.Constitution, 9))
            {
                int direction;
                var targetEngine = new TargetEngine(player, level);
                if (!targetEngine.GetDirectionNoAim(out direction))
                {
                    return;
                }
                var y = player.MapY + level.KeypadDirectionYOffset[direction];
                var x = player.MapX + level.KeypadDirectionXOffset[direction];
                var tile = level.Grid[y][x];
                if (tile.MonsterIndex == 0)
                {
                    Profile.Instance.MsgPrint("You bite into thin air!");
                    return;
                }
                Profile.Instance.MsgPrint("You grin and bare your fangs...");
                var dummy = player.Level + (Program.Rng.DieRoll(player.Level) * Math.Max(1, player.Level / 10));
                if (saveGame.SpellEffects.DrainLife(direction, dummy))
                {
                    if (player.Food < Constants.PyFoodFull)
                    {
                        player.RestoreHealth(dummy);
                    }
                    else
                    {
                        Profile.Instance.MsgPrint("You were not hungry.");
                    }
                    dummy = player.Food + Math.Min(5000, 100 * dummy);
                    if (player.Food < Constants.PyFoodMax)
                    {
                        player.SetFood(dummy >= Constants.PyFoodMax ? Constants.PyFoodMax - 1 : dummy);
                    }
                }
                else
                {
                    Profile.Instance.MsgPrint("Yechh. That tastes foul.");
                }
            }
            player.RedrawNeeded.Set(RedrawFlag.PrHp | RedrawFlag.PrVis);
        }
    }
}