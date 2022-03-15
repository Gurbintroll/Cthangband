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
    internal class MutationHpToSp : Mutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "You are subject to fits of painful clarity.";
            HaveMessage = "Your blood sometimes rushes to your head.";
            LoseMessage = "You are no longer subject to fits of painful clarity.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (player.HasAntiMagic || Program.Rng.DieRoll(4000) != 1)
            {
                return;
            }
            int wounds = player.MaxMana - player.Mana;
            if (wounds <= 0)
            {
                return;
            }
            int healing = player.Health;
            if (healing > wounds)
            {
                healing = wounds;
            }
            player.Mana += healing;
            player.TakeHit(healing, "blood rushing to the head");
        }
    }
}