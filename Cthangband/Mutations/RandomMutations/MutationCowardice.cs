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
    internal class MutationCowardice : Mutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "You become an incredible coward!";
            HaveMessage = "You are subject to cowardice.";
            LoseMessage = "You are no longer an incredible coward!";
            Group = MutationGroup.Bravery;
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (Program.Rng.DieRoll(3000) != 13)
            {
                return;
            }
            if (player.HasFearResistance || player.TimedHeroism != 0 || player.TimedSuperheroism != 0)
            {
                return;
            }
            saveGame.Disturb(false);
            Profile.Instance.MsgPrint("It's so dark... so scary!");
            player.RedrawNeeded.Set(RedrawFlag.PrAfraid);
            player.TimedFear = player.TimedFear + 13 + Program.Rng.DieRoll(26);
        }
    }
}