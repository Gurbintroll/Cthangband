// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Projection;
using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationRawChaos : Mutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "You feel the universe is less stable around you.";
            HaveMessage = "You occasionally are surrounded with raw chaos.";
            LoseMessage = "You feel the universe is more stable around you.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (player.HasAntiMagic || Program.Rng.DieRoll(8000) != 1)
            {
                return;
            }
            saveGame.Disturb(false);
            Profile.Instance.MsgPrint("You feel the world warping around you!");
            Profile.Instance.MsgPrint(null);
            saveGame.SpellEffects.FireBall(new ProjectChaos(SaveGame.Instance.SpellEffects), 0, player.Level, 8);
        }
    }
}