// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationWalkShad : Mutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "You feel like reality is as thin as paper.";
            HaveMessage = "You occasionally stumble into other shadows.";
            LoseMessage = "You feel like you're trapped in reality.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (!player.HasAntiMagic && Program.Rng.DieRoll(12000) == 1)
            {
                saveGame.SpellEffects.AlterReality();
            }
        }
    }
}