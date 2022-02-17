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