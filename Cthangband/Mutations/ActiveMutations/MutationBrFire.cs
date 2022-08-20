// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
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
    internal class MutationBrFire : BaseMutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.CheckIfRacialPowerWorks(20, player.Level, Ability.Constitution, 18))
            {
                return;
            }
            TargetEngine targetEngine = new TargetEngine(player, level);
            Profile.Instance.MsgPrint("You breathe fire...");
            if (targetEngine.GetDirectionWithAim(out int dir))
            {
                saveGame.SpellEffects.FireBall(new ProjectFire(SaveGame.Instance.SpellEffects), dir, player.Level * 2, -(1 + (player.Level / 20)));
            }
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 20
                ? "fire breath      (unusable until level 20)"
                : $"fire breath      (cost {lvl}, dam {lvl * 2}, CON based)";
        }

        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "You gain the ability to breathe fire.";
            HaveMessage = "You can breathe fire (dam lvl * 2).";
            LoseMessage = "You lose the ability to breathe fire.";
        }
    }
}