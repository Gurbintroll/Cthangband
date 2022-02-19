using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationSmellMon : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(5, 4, Ability.Intelligence, 15))
            {
                saveGame.SpellEffects.DetectMonstersNormal();
            }
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 5 ? "smell monsters   (unusable until level 5)" : "smell monsters   (cost 4, INT based)";
        }

        public override void Initialise()
        {
            Frequency = 4;
            GainMessage = "You smell filthy monsters.";
            HaveMessage = "You can smell nearby monsters.";
            LoseMessage = "You no longer smell filthy monsters.";
        }
    }
}