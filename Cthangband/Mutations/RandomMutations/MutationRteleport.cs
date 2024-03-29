﻿// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
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
    internal class MutationRteleport : BaseMutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "Your position seems very uncertain...";
            HaveMessage = "You are teleporting randomly.";
            LoseMessage = "Your position seems more certain.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (Program.Rng.DieRoll(5000) != 88)
            {
                return;
            }
            if (player.HasNexusResistance || player.HasAntiTeleport)
            {
                return;
            }
            saveGame.Disturb(false);
            Profile.Instance.MsgPrint("Your position suddenly seems very uncertain...");
            Profile.Instance.MsgPrint(null);
            saveGame.SpellEffects.TeleportPlayer(40);
        }
    }
}