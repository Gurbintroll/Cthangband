// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Mutations.Base;
using Cthangband.Projection;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationLaserEye : BaseMutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.CheckIfRacialPowerWorks(7, 10, Ability.Wisdom, 9))
            {
                return;
            }
            var targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out var dir))
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