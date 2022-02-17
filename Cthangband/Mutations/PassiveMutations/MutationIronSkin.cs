using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.PassiveMutations
{
    [Serializable]
    internal class MutationIronSkin : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "Your skin turns to steel!";
            HaveMessage = "Your skin is made of steel (-1 DEX, +25 AC).";
            LoseMessage = "Your skin reverts to flesh!";
            Group = MutationGroup.Skin;
        }

        public override void OnGain(Genome genome)
        {
            genome.DexterityBonus -= 1;
            genome.ArmourClassBonus += 25;
        }

        public override void OnLose(Genome genome)
        {
            genome.DexterityBonus += 1;
            genome.ArmourClassBonus -= 25;
        }
    }
}