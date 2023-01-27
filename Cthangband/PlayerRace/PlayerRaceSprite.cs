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
    internal class PlayerRaceSprite : BasePlayerRace
    {
        private int[] _abilityBonus = { -4, 3, 3, 3, -2, 2 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AgeRange => 25;
        public override int BaseAge => 50;

        public override int BaseDeviceBonus => 10;

        public override int BaseDisarmBonus => 10;

        public override int BaseMeleeAttackBonus => -12;

        public override int BaseRangedAttackBonus => 0;

        public override int BaseSaveBonus => 10;

        public override int BaseSearchBonus => 10;

        public override int BaseSearchFrequency => 10;

        public override int BaseStealthBonus => 4;

        public override string Description1 => "Sprites are tiny fairies, distantly related to elves. They";

        public override string Description2 => "share their relatives' resistance to light based attacks,";

        public override string Description3 => "and their wings both protect them from falling damage and";

        public override string Description4 => "allow them to move progressively faster if unencumbered.";

        public override string Description5 => "Sprites glow in the dark and can learn to throw fairy dust";

        public override string Description6 => "to send their enemies to sleep (at lvl 12).";

        public override int ExperienceFactor => 175;

        public override int FemaleBaseHeight => 29;

        public override int FemaleBaseWeight => 65;

        public override int FemaleHeightRange => 2;

        public override int FemaleWeightRange => 2;

        public override bool Glows => true;

        public override bool HasSpeedBonus => true;

        public override int HitDieBonus => 7;

        public override int Infravision => 4;

        public override int MaleBaseHeight => 32;

        public override int MaleBaseWeight => 75;

        public override int MaleHeightRange => 2;

        public override int MaleWeightRange => 2;

        public override string Title => "Sprite";

        protected override int BackgroundStartingChart => 124;

        public override void ApplyRacialStatus(Player player)
        {
            player.HasFeatherFall = true;
            player.HasGlow = true;
            player.HasLightResistance = true;
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
                name = _elfSyllable1[Program.Rng.RandomLessThan(_elfSyllable1.Length)];
                name += _elfSyllable2[Program.Rng.RandomLessThan(_elfSyllable2.Length)];
                name += _elfSyllable3[Program.Rng.RandomLessThan(_elfSyllable3.Length)];
            } while (name.Length > 12);
            return name;
        }

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            f2.Set(ItemFlag2.ResLight);
            f3.Set(ItemFlag3.Feather);
            if (player.Level > 9)
            {
                f1.Set(ItemFlag1.Speed);
            }
        }

        public override string GetRacialPowerText(Player player)
        {
            return player.Level < 12 ? "sleeping dust      (racial, unusable until level 12)" : "sleeping dust      (racial, cost 12, INT based)";
        }

        public override List<string> SelfKnowledge(Player player)
        {
            var list = new List<string>();
            if (player.Level > 11)
            {
                list.Add("You can throw magic dust which induces sleep (cost 12).");
            }
            return list;
        }

        public override void UseRacialPower(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(12, 12, Ability.Intelligence, 15))
            {
                Profile.Instance.MsgPrint("You throw some magic dust...");
                if (player.Level < 25)
                {
                    saveGame.SpellEffects.SleepMonstersTouch();
                }
                else
                {
                    saveGame.SpellEffects.SleepMonsters();
                }
            }
            player.RedrawNeeded.Set(RedrawFlag.PrHp | RedrawFlag.PrVis);
        }
    }
}