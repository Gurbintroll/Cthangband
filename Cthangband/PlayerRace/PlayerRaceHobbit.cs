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
    internal class PlayerRaceHobbit : BasePlayerRace
    {
        private int[] _abilityBonus = { -2, 2, 1, 3, 2, 1 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AgeRange => 12;
        public override int BaseAge => 21;

        public override int BaseDeviceBonus => 18;

        public override int BaseDisarmBonus => 15;

        public override int BaseMeleeAttackBonus => -10;

        public override int BaseRangedAttackBonus => 20;

        public override int BaseSaveBonus => 18;

        public override int BaseSearchBonus => 12;

        public override int BaseSearchFrequency => 15;

        public override int BaseStealthBonus => 5;

        public override string Description2 => "Hobbits are small and surprisingly dextrous given their";

        public override string Description3 => "propensity for plumpness. They make excellent burglars";

        public override string Description4 => "and are adept at spell casting too. Hobbits can't have";

        public override string Description5 => "their dexterity reduced, and they can learn to put together";

        public override string Description6 => "nourishing meals from the barest scraps (at lvl 15).";

        public override int ExperienceFactor => 110;

        public override int FemaleBaseHeight => 33;

        public override int FemaleBaseWeight => 50;

        public override int FemaleHeightRange => 3;

        public override int FemaleWeightRange => 3;

        public override int HitDieBonus => 7;

        public override int Infravision => 4;

        public override int MaleBaseHeight => 36;

        public override int MaleBaseWeight => 60;

        public override int MaleHeightRange => 3;

        public override int MaleWeightRange => 3;

        public override string Title => "Hobbit";

        protected override int BackgroundStartingChart => 10;

        public override void ApplyRacialStatus(Player player)
        {
            player.HasSustainDexterity = true;
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
                name = _hobbitSyllable1[Program.Rng.RandomLessThan(_hobbitSyllable1.Length)];
                name += _hobbitSyllable2[Program.Rng.RandomLessThan(_hobbitSyllable2.Length)];
                name += _hobbitSyllable3[Program.Rng.RandomLessThan(_hobbitSyllable3.Length)];
            } while (name.Length > 12);
            return name;
        }

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            f2.Set(ItemFlag2.SustDex);
        }

        public override string GetRacialPowerText(Player player)
        {
            return player.Level < 15 ? "create food        (racial, unusable until level 15)" : "create food        (racial, cost 10, INT based)";
        }

        public override List<string> SelfKnowledge(Player player)
        {
            var list = new List<string>();
            if (player.Level > 14)
            {
                list.Add("You can produce food (cost 10).");
            }
            return list;
        }

        public override void UseRacialPower(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(15, 10, Ability.Intelligence, 10))
            {
                var item = new Item();
                item.AssignItemType(Profile.Instance.ItemTypes.LookupKind(ItemCategory.Food, FoodType.Ration));
                saveGame.Level.DropNear(item, -1, player.MapY, player.MapX);
                Profile.Instance.MsgPrint("You cook some food.");
            }
            player.RedrawNeeded.Set(RedrawFlag.PrHp | RedrawFlag.PrVis);
        }
    }
}