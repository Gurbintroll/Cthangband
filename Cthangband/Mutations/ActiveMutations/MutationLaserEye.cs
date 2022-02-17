using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationLaserEye : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.RacialAux(7, 10, Ability.Wisdom, 9))
            {
                return;
            }
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBeam(new ProjectLight(SaveGame.Instance.SpellEffects), dir, 2 * player.Level);
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 7 ? "laser eyes        (unusable until level 7)" : "laser eyes        (cost 10, WIS based)";
        }

        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "Your eyes burn for a moment.";
            HaveMessage = "Your eyes can fire laser beams.";
            LoseMessage = "Your eyes burn for a moment, then feel soothed.";
        }
    }
}