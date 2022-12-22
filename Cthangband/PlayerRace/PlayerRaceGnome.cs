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
    internal class PlayerRaceGnome : BasePlayerRace
    {
        private int[] _abilityBonus = { -1, 2, 0, 2, 1, -2 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AgeRange => 40;
        public override int BaseAge => 50;

        public override int BaseDeviceBonus => 12;

        public override int BaseDisarmBonus => 10;

        public override int BaseMeleeAttackBonus => -8;

        public override int BaseRangedAttackBonus => 12;

        public override int BaseSaveBonus => 12;

        public override int BaseSearchBonus => 6;

        public override int BaseSearchFrequency => 13;

        public override int BaseStealthBonus => 3;

        public override uint Choice => 0x1E0F;

        public override string Description1 => "Gnomes are small, playful, and talented at magic. However,";

        public override string Description2 => "they are almost chronically incapable of taking anything";

        public override string Description3 => "seriously. Gnomes are constantly fidgeting and always on";

        public override string Description4 => "the move, and this makes them impossible to paralyse or";

        public override string Description5 => "magically slow. Gnomes are even able to learn how to ";

        public override string Description6 => "teleport short distances (at lvl 5).";

        public override int ExperienceFactor => 135;

        public override int FemaleBaseHeight => 39;

        public override int FemaleBaseWeight => 75;

        public override int FemaleHeightRange => 3;

        public override int FemaleWeightRange => 3;

        public override int HitDieBonus => 8;

        public override int Infravision => 4;

        public override int MaleBaseHeight => 42;

        public override int MaleBaseWeight => 90;

        public override int MaleHeightRange => 3;

        public override int MaleWeightRange => 6;

        public override string Title => "Gnome";

        protected override int BackgroundStartingChart => 13;

        public override void ApplyRacialStatus(Player player)
        {
            player.HasFreeAction = true;
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
                name = _gnomeSyllable1[Program.Rng.RandomLessThan(_gnomeSyllable1.Length)];
                name += _gnomeSyllable2[Program.Rng.RandomLessThan(_gnomeSyllable2.Length)];
                name += _gnomeSyllable3[Program.Rng.RandomLessThan(_gnomeSyllable3.Length)];
            } while (name.Length > 12);
            return name;
        }

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            f2.Set(ItemFlag2.FreeAct);
        }

        public override string GetRacialPowerText(Player player)
        {
            return player.Level < 5 ? "teleport           (racial, unusable until level 5)" : "teleport           (racial, cost 5 + lvl/5, INT based)";
        }

        public override List<string> SelfKnowledge(Player player)
        {
            var list = new List<string>();
            if (player.Level > 4)
            {
                list.Add($"You can teleport, range {1 + player.Level} (cost {5 + (player.Level / 5)}).");
            }
            return list;
        }

        public override void UseRacialPower(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(5, 5 + (player.Level / 5), Ability.Intelligence, 12))
            {
                Profile.Instance.MsgPrint("Blink!");
                saveGame.SpellEffects.TeleportPlayer(10 + player.Level);
            }
            player.RedrawNeeded.Set(RedrawFlag.PrHp | RedrawFlag.PrVis);
        }
    }
}