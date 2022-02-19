using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationMindBlst : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.CheckIfRacialPowerWorks(5, 3, Ability.Wisdom, 15))
            {
                return;
            }
            TargetEngine targetEngine = new TargetEngine(player, level);
            Profile.Instance.MsgPrint("You concentrate...");
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBolt(new ProjectPsi(SaveGame.Instance.SpellEffects), dir,
                Program.Rng.DiceRoll(3 + ((player.Level - 1) / 5), 3));
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 5 ? "mind blast       (unusable until level 5)" : "mind blast       (cost 3, WIS based)";
        }

        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You gain the power of Mind Blast.";
            HaveMessage = "You can Mind Blast your enemies.";
            LoseMessage = "You lose the power of Mind Blast.";
        }
    }
}