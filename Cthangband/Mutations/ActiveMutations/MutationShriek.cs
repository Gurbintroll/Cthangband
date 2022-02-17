using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationShriek : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.RacialAux(4, 4, Ability.Constitution, 6))
            {
                return;
            }
            saveGame.SpellEffects.FireBall(new ProjectSound(SaveGame.Instance.SpellEffects), 0, 4 * player.Level, 8);
            saveGame.SpellEffects.AggravateMonsters(0);
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 4 ? "shriek           (unusable until level 4)" : "shriek           (cost 4, CON based)";
        }

        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "Your vocal cords get much tougher.";
            HaveMessage = "You can emit a horrible shriek.";
            LoseMessage = "Your vocal cords get much weaker.";
        }
    }
}