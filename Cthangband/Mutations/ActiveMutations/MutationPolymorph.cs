using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationPolymorph : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(18, 20, Ability.Constitution, 18))
            {
                player.PolymorphSelf();
            }
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 18 ? "polymorph        (unusable until level 18)" : "polymorph        (cost 20, CON based)";
        }

        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "Your body seems mutable.";
            HaveMessage = "You can polymorph yourself at will.";
            LoseMessage = "Your body seems stable.";
        }
    }
}