// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Mutations.Base;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationResist : BaseMutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(10, 12, Ability.Constitution, 12))
            {
                int num = player.Level / 10;
                int dur = Program.Rng.DieRoll(20) + 20;
                if (Program.Rng.RandomLessThan(5) < num)
                {
                    player.SetTimedAcidResistance(player.TimedAcidResistance + dur);
                    num--;
                }
                if (Program.Rng.RandomLessThan(4) < num)
                {
                    player.SetTimedLightningResistance(player.TimedLightningResistance + dur);
                    num--;
                }
                if (Program.Rng.RandomLessThan(3) < num)
                {
                    player.SetTimedFireResistance(player.TimedFireResistance + dur);
                    num--;
                }
                if (Program.Rng.RandomLessThan(2) < num)
                {
                    player.SetTimedColdResistance(player.TimedColdResistance + dur);
                    num--;
                }
                if (num != 0)
                {
                    player.SetTimedPoisonResistance(player.TimedPoisonResistance + dur);
                }
            }
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 10 ? "resist elements  (unusable until level 10)" : "resist elements  (cost 12, CON based)";
        }

        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "You feel like you can protect yourself.";
            HaveMessage = "You can harden yourself to the ravages of the elements.";
            LoseMessage = "You feel like you might be vulnerable.";
        }
    }
}