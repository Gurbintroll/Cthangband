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