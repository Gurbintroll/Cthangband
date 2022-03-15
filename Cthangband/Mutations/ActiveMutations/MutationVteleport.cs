// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationVteleport : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.CheckIfRacialPowerWorks(7, 7, Ability.Wisdom, 15))
            {
                return;
            }
            Profile.Instance.MsgPrint("You concentrate...");
            saveGame.SpellEffects.TeleportPlayer(10 + (4 * player.Level));
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 7 ? "teleport         (unusable until level 7)" : "teleport         (cost 7, WIS based)";
        }

        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "You gain the power of teleportation at will.";
            HaveMessage = "You can teleport at will.";
            LoseMessage = "You lose the power of teleportation at will.";
        }
    }
}