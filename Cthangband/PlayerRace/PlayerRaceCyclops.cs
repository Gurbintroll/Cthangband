// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cthangband.Enumerations;
using Cthangband.PlayerRace.Base;
using Cthangband.Projection;

namespace Cthangband.PlayerRace
{
    [Serializable]
    internal class PlayerRaceCyclops : BasePlayerRace
    {
        private int[] _abilityBonus = { 4, -3, -3, -3, 4, -6 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AgeRange => 24;
        public override int BaseAge => 50;

        public override int BaseDeviceBonus => -5;

        public override int BaseDisarmBonus => -4;

        public override int BaseMeleeAttackBonus => 20;

        public override int BaseRangedAttackBonus => 12;

        public override int BaseSaveBonus => -5;

        public override int BaseSearchBonus => -2;

        public override int BaseSearchFrequency => 5;

        public override int BaseStealthBonus => -2;

        public override string Description2 => "Cyclopes are one eyed giants, often seen as freaks by the";

        public override string Description3 => "other races. They can learn to throw boulders (at lvl 20)";

        public override string Description4 => "and although they have weak eyesight their hearing is very";

        public override string Description5 => "keen and hard to damage, so they are resistant to sound";

        public override string Description6 => "based attacks.";

        public override int ExperienceFactor => 130;

        public override int FemaleBaseHeight => 80;

        public override int FemaleBaseWeight => 235;

        public override int FemaleHeightRange => 8;

        public override int FemaleWeightRange => 60;

        public override int HitDieBonus => 13;

        public override int Infravision => 1;

        public override int MaleBaseHeight => 92;

        public override int MaleBaseWeight => 255;

        public override int MaleHeightRange => 10;

        public override int MaleWeightRange => 60;

        public override string Title => "Cyclops";

        protected override int BackgroundStartingChart => 77;

        public override void ApplyRacialStatus(Player player)
        {
            player.HasSoundResistance = true;
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
                name = _dwarfSyllable1[Program.Rng.RandomLessThan(_dwarfSyllable1.Length)];
                name += _dwarfSyllable2[Program.Rng.RandomLessThan(_dwarfSyllable2.Length)];
                name += _dwarfSyllable3[Program.Rng.RandomLessThan(_dwarfSyllable3.Length)];
            } while (name.Length > 12);
            return name;
        }

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            f2.Set(ItemFlag2.ResSound);
        }

        public override string GetRacialPowerText(Player player)
        {
            return player.Level < 20 ? "throw boulder      (racial, unusable until level 20)" : "throw boulder      (racial, cost 15, dam 3*lvl, STR based)";
        }

        public override List<string> SelfKnowledge(Player player)
        {
            var list = new List<string>();
            if (player.Level > 19)
            {
                list.Add($"You can throw a boulder, dam. {3 * player.Level} (cost 15).");
            }
            return list;
        }

        public override void UseRacialPower(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(20, 15, Ability.Strength, 12))
            {
                int direction;
                TargetEngine targetEngine = new TargetEngine(player, level);
                if (!targetEngine.GetDirectionWithAim(out direction))
                {
                    return;
                }
                Profile.Instance.MsgPrint("You throw a huge boulder.");
                saveGame.SpellEffects.FireBolt(new ProjectMissile(SaveGame.Instance.SpellEffects), direction,
                    3 * player.Level / 2);
            }
            player.RedrawNeeded.Set(RedrawFlag.PrHp | RedrawFlag.PrVis);
        }
    }
}