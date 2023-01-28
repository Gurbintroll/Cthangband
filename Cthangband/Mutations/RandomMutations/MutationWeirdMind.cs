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
    internal class MutationWeirdMind : BaseMutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "Your thoughts suddenly take off in strange directions.";
            HaveMessage = "Your mind randomly expands and contracts.";
            LoseMessage = "Your thoughts return to boring paths.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (player.HasAntiMagic || Program.Rng.DieRoll(3000) != 1)
            {
                return;
            }
            if (player.TimedTelepathy > 0)
            {
                Profile.Instance.MsgPrint("Your mind feels cloudy!");
                player.SetTimedTelepathy(0);
            }
            else
            {
                Profile.Instance.MsgPrint("Your mind expands!");
                player.SetTimedTelepathy(player.Level);
            }
        }
    }
}