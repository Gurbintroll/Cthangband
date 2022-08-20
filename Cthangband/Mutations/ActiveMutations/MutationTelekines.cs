// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Mutations.Base;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationTelekines : BaseMutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.CheckIfRacialPowerWorks(9, 9, Ability.Wisdom, 14))
            {
                return;
            }
            TargetEngine targetEngine = new TargetEngine(player, level);
            Profile.Instance.MsgPrint("You concentrate...");
            if (targetEngine.GetDirectionWithAim(out int dir))
            {
                saveGame.CommandEngine.SummonItem(dir, player.Level * 10, true);
            }
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 9 ? "telekinesis      (unusable until level 9)" : "telekinesis      (cost 9, WIS based)";
        }

        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You gain the ability to move objects telekinetically.";
            HaveMessage = "You are telekinetic.";
            LoseMessage = "You lose the ability to move objects telekinetically.";
        }
    }
}