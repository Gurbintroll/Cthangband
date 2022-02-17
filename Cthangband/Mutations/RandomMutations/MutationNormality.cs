using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationNormality : Mutation
    {
        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "You feel strangely normal.";
            HaveMessage = "You may be chaotic, but you're recovering.";
            LoseMessage = "You feel normally strange.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (Program.Rng.DieRoll(5000) == 1)
            {
                player.Dna.LoseMutation();
            }
        }
    }
}