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
    internal class MutationSpToHp : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You are subject to fits of magical healing.";
            HaveMessage = "Your blood sometimes rushes to your muscles.";
            LoseMessage = "You are no longer subject to fits of magical healing.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (Program.Rng.DieRoll(2000) != 1)
            {
                return;
            }
            int wounds = player.MaxHealth - player.Health;
            if (wounds <= 0)
            {
                return;
            }
            int healing = player.Mana;
            if (healing > wounds)
            {
                healing = wounds;
            }
            player.RestoreHealth(healing);
            player.Mana -= healing;
        }
    }
}