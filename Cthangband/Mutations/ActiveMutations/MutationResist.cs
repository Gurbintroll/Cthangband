using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationResist : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.RacialAux(10, 12, Ability.Constitution, 12))
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