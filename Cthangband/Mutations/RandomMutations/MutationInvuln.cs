// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
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
    internal class MutationInvuln : BaseMutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "You are blessed with fits of invulnerability.";
            HaveMessage = "You occasionally feel invincible.";
            LoseMessage = "You are no longer blessed with fits of invulnerability.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (!player.HasAntiMagic && Program.Rng.DieRoll(5000) == 1)
            {
                saveGame.Disturb(false);
                Profile.Instance.MsgPrint("You feel invincible!");
                Profile.Instance.MsgPrint(null);
                player.SetTimedInvulnerability(player.TimedInvulnerability + Program.Rng.DieRoll(8) + 8);
            }
        }
    }
}