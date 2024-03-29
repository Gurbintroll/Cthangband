﻿// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Mutations.Base;
using Cthangband.StaticData;
using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationNausea : BaseMutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "Your stomach starts to roil nauseously.";
            HaveMessage = "You have a seriously upset stomach.";
            LoseMessage = "Your stomach stops roiling.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (player.HasSlowDigestion || Program.Rng.DieRoll(9000) != 1)
            {
                return;
            }
            saveGame.Disturb(false);
            Profile.Instance.MsgPrint("Your stomach roils, and you lose your lunch!");
            Profile.Instance.MsgPrint(null);
            player.SetFood(Constants.PyFoodWeak);
        }
    }
}