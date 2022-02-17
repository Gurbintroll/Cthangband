using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationPolyWound : Mutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "You feel forces of chaos entering your old scars.";
            HaveMessage = "Your health is subject to chaotic forces.";
            LoseMessage = "You feel forces of chaos departing your old scars.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (Program.Rng.DieRoll(3000) == 1)
            {
                player.PolymorphWounds();
            }
        }
    }
}