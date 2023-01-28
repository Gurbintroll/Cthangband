// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Mutations.Base;
using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationWraith : BaseMutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "You start to fade in and out of the physical world.";
            HaveMessage = "You fade in and out of physical reality.";
            LoseMessage = "You are firmly in the physical world.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (player.HasAntiMagic || Program.Rng.DieRoll(3000) != 13)
            {
                return;
            }
            saveGame.Disturb(false);
            Profile.Instance.MsgPrint("You feel insubstantial!");
            Profile.Instance.MsgPrint(null);
            player.SetTimedEtherealness(player.TimedEtherealness + Program.Rng.DieRoll(player.Level / 2) + player.Level / 2);
        }
    }
}