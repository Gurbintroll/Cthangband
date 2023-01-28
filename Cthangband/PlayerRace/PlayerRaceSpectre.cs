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
    internal class PlayerRaceSpectre : BasePlayerRace
    {
        private int[] _abilityBonus = { -5, 4, 4, 2, -3, -6 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AgeRange => 30;
        public override int BaseAge => 100;
        public override int BaseDeviceBonus => 25;
        public override int BaseDisarmBonus => 10;
        public override int BaseMeleeAttackBonus => -15;
        public override int BaseRangedAttackBonus => -5;
        public override int BaseSaveBonus => 20;
        public override int BaseSearchBonus => 5;
        public override int BaseSearchFrequency => 14;
        public override int BaseStealthBonus => 5;
        public override string Description1 => "Spectres are ethereal and they can pass through walls and";
        public override string Description2 => "other obstacles. They resist nether, attacks, poison, and";
        public override string Description3 => "cold; and they need little food. They also resist having";
        public override string Description4 => "their life force drained and can see invisible creatures.";
        public override string Description5 => "Finally, they glow with their own light, can learn to";
        public override string Description6 => "scare monsters (at lvl 4) and gain telepathy (at lvl 35).";
        public override bool DoesntEat => true;
        public override int ExperienceFactor => 180;
        public override bool FeedsOnNether => true;
        public override int FemaleBaseHeight => 66;
        public override int FemaleBaseWeight => 100;
        public override int FemaleHeightRange => 4;
        public override int FemaleWeightRange => 20;
        public override bool Glows => true;
        public override int HitDieBonus => 7;
        public override int Infravision => 5;
        public override bool IsIncorporeal => true;
        public override bool IsNocturnal => true;

        public override int MaleBaseHeight => 72;
        public override int MaleBaseWeight => 100;

        public override int MaleHeightRange => 6;

        public override int MaleWeightRange => 25;

        public override bool SanityResistant => true;

        public override string Title => "Spectre";

        protected override int BackgroundStartingChart => 118;

        public override void ApplyRacialStatus(Player player)
        {
            player.HasFeatherFall = true;
            player.HasNetherResistance = true;
            player.HasHoldLife = true;
            player.HasSeeInvisibility = true;
            player.HasPoisonResistance = true;
            player.HasSlowDigestion = true;
            player.HasColdResistance = true;
            player.HasGlow = true;
            if (player.Level > 34)
            {
                player.HasTelepathy = true;
            }
        }

        public override void ConsumeFood(Player player, Item item)
        {
            Profile.Instance.MsgPrint("The food of mortals is poor sustenance for you.");
            player.SetFood(player.Food + (item.TypeSpecificValue / 20));
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

        public override bool DoesntBleed(Player player)
        {
            return true;
        }

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            f2.Set(ItemFlag2.ResCold);
            f3.Set(ItemFlag3.SeeInvis);
            f2.Set(ItemFlag2.HoldLife);
            f2.Set(ItemFlag2.ResNether);
            f2.Set(ItemFlag2.ResPois);
            f3.Set(ItemFlag3.SlowDigest);
            f3.Set(ItemFlag3.Lightsource);
            if (player.Level > 34)
            {
                f3.Set(ItemFlag3.Telepathy);
            }
        }

        public override string GetRacialPowerText(Player player)
        {
            return player.Level < 4 ? "scare monster      (racial, unusable until level 4)" : "scare monster      (racial, cost 3, INT based)";
        }

        public override List<string> SelfKnowledge(Player player)
        {
            var list = new List<string>();
            if (player.Level > 3)
            {
                list.Add("You can wail to terrify your enemies (cost 3).");
            }
            return list;
        }

        public override void UseRacialPower(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(4, 6, Ability.Intelligence, 3))
            {
                Profile.Instance.MsgPrint("You emit an eldritch howl!");
                int direction;
                var targetEngine = new TargetEngine(player, level);
                if (!targetEngine.GetDirectionWithAim(out direction))
                {
                    return;
                }
                saveGame.SpellEffects.FearMonster(direction, player.Level);
            }
            player.RedrawNeeded.Set(RedrawFlag.PrHp | RedrawFlag.PrVis);
        }
    }
}