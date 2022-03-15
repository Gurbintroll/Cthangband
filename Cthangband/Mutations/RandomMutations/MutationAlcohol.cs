// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
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