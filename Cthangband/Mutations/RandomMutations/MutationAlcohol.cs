using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationAlcohol : Mutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "Your body starts producing alcohol!";
            HaveMessage = "Your body produces alcohol.";
            LoseMessage = "Your body stops producing alcohol!";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (Program.Rng.DieRoll(6400) != 321)
            {
                return;
            }
            if (player.HasChaosResistance && player.HasConfusionResistance)
            {
                return;
            }
            saveGame.Disturb(false);
            player.RedrawNeeded.Set(RedrawFlag.PrExtra);
            Profile.Instance.MsgPrint("You feel a SSSCHtupor cOmINg over yOu... *HIC*!");
            if (Program.Rng.DieRoll(20) == 1)
            {
                Profile.Instance.MsgPrint(null);
                if (Program.Rng.DieRoll(3) == 1)
                {
                    saveGame.SpellEffects.LoseAllInfo();
                }
                else
                {
                    level.WizDark();
                }
                saveGame.SpellEffects.TeleportPlayer(100);
                level.WizDark();
                Profile.Instance.MsgPrint("You wake up somewhere with a sore head...");
                Profile.Instance.MsgPrint("You can't remember a thing, or how you got here!");
            }
            else
            {
                if (!player.HasConfusionResistance)
                {
                    player.SetTimedConfusion(player.TimedConfusion + Program.Rng.RandomLessThan(20) + 15);
                }
                if (Program.Rng.DieRoll(3) == 1 && !player.HasChaosResistance)
                {
                    Profile.Instance.MsgPrint("Thishcischs GooDSChtuff!");
                    player.SetTimedHallucinations(player.TimedHallucinations + Program.Rng.RandomLessThan(150) + 150);
                }
            }
        }
    }
}