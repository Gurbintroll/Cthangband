using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationVteleport : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.RacialAux(7, 7, Ability.Wisdom, 15))
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