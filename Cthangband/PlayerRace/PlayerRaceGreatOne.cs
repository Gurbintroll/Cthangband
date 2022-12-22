// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.PlayerRace.Base;
using Cthangband.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cthangband.PlayerRace
{
    [Serializable]
    internal class PlayerRaceGreatOne : BasePlayerRace
    {
        private int[] _abilityBonus = { 1, 2, 2, 2, 3, 2 };
        public override int[] AbilityBonus => _abilityBonus;
        public override int AgeRange => 50;

        public override int BaseAge => 50;
        public override int BaseDeviceBonus => 5;

        public override int BaseDisarmBonus => 4;

        public override int BaseMeleeAttackBonus => 15;

        public override int BaseRangedAttackBonus => 10;

        public override int BaseSaveBonus => 5;

        public override int BaseSearchBonus => 3;

        public override int BaseSearchFrequency => 13;

        public override int BaseStealthBonus => 2;

        public override uint Choice => 0xFFFF;

        public override string Description2 => "Great-Ones are the offspring of the petty gods that rule";

        public override string Description3 => "Dreamlands. As such they are somewhat more than human.";

        public override string Description4 => "Their constitution cannot be reduced, and they heal";

        public override string Description5 => "quickly. They can also learn to travel through dreams";

        public override string Description6 => "(at lvl 30) and restore their health (at lvl 40).";

        public override int ExperienceFactor => 225;

        public override int FemaleBaseHeight => 78;

        public override int FemaleBaseWeight => 180;

        public override int FemaleHeightRange => 6;

        public override int FemaleWeightRange => 15;

        public override int HitDieBonus => 10;

        public override int Infravision => 0;

        public override int MaleBaseHeight => 82;

        public override int MaleBaseWeight => 190;

        public override int MaleHeightRange => 5;

        public override int MaleWeightRange => 20;

        public override string Title => "Great One";

        protected override int BackgroundStartingChart => 67;

        public override void ApplyRacialStatus(Player player)
        {
            player.HasSustainConstitution = true;
            player.HasRegeneration = true;
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
                name = _humanSyllable1[Program.Rng.RandomLessThan(_humanSyllable1.Length)];
                name += _humanSyllable2[Program.Rng.RandomLessThan(_humanSyllable2.Length)];
                name += _humanSyllable3[Program.Rng.RandomLessThan(_humanSyllable3.Length)];
            } while (name.Length > 12);
            return name;
        }

        public override void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3)
        {
            f2.Set(ItemFlag2.SustCon);
            f3.Set(ItemFlag3.Regen);
        }

        public override string GetRacialPowerText(Player player)
        {
            return "dream powers    (unusable until level 30/40)";
        }

        public override List<string> SelfKnowledge(Player player)
        {
            var list = new List<string>();
            if (player.Level > 29)
            {
                list.Add("You can dream travel (cost 50).");
            }
            if (player.Level > 39)
            {
                list.Add("You can dream a better self (cost 75).");
            }
            return list;
        }

        public override void UseRacialPower(SaveGame saveGame, Player player, Level level)
        {
            int dreamPower;
            while (true)
            {
                if (!Gui.GetCom("Use Dream [T]ravel or [D]reaming? ", out char ch))
                {
                    dreamPower = 0;
                    break;
                }
                if (ch == 'D' || ch == 'd')
                {
                    dreamPower = 1;
                    break;
                }
                if (ch == 'T' || ch == 't')
                {
                    dreamPower = 2;
                    break;
                }
            }
            if (dreamPower == 1)
            {
                if (saveGame.CommandEngine.CheckIfRacialPowerWorks(40, 75, Ability.Wisdom, 50))
                {
                    Profile.Instance.MsgPrint("You dream of a time of health and peace...");
                    player.SetTimedPoison(0);
                    player.SetTimedHallucinations(0);
                    player.SetTimedStun(0);
                    player.SetTimedBleeding(0);
                    player.SetTimedBlindness(0);
                    player.SetTimedFear(0);
                    player.TryRestoringAbilityScore(Ability.Strength);
                    player.TryRestoringAbilityScore(Ability.Intelligence);
                    player.TryRestoringAbilityScore(Ability.Wisdom);
                    player.TryRestoringAbilityScore(Ability.Dexterity);
                    player.TryRestoringAbilityScore(Ability.Constitution);
                    player.TryRestoringAbilityScore(Ability.Charisma);
                    player.RestoreLevel();
                }
            }
            else if (dreamPower == 2)
            {
                if (saveGame.CommandEngine.CheckIfRacialPowerWorks(30, 50, Ability.Intelligence, 50))
                {
                    Profile.Instance.MsgPrint("You start walking around. Your surroundings change.");
                    saveGame.IsAutosave = true;
                    saveGame.DoCmdSaveGame();
                    saveGame.IsAutosave = false;
                    saveGame.NewLevelFlag = true;
                    saveGame.CameFrom = LevelStart.StartRandom;
                }
            }
            player.RedrawNeeded.Set(RedrawFlag.PrHp | RedrawFlag.PrVis);
        }
    }
}