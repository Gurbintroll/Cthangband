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
    internal class MutationHypnGaze : BaseMutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.CheckIfRacialPowerWorks(12, 12, Ability.Charisma, 18))
            {
                return;
            }
            TargetEngine targetEngine = new TargetEngine(player, level);
            Profile.Instance.MsgPrint("Your eyes look mesmerizing...");
            if (targetEngine.GetDirectionWithAim(out int dir))
            {
                saveGame.SpellEffects.CharmMonster(dir, player.Level);
            }
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 20 ? "hypnotic gaze    (unusable until level 12)" : "hypnotic gaze    (cost 12, CHA based)";
        }

        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "Your eyes look mesmerizing...";
            HaveMessage = "Your gaze is hypnotic.";
            LoseMessage = "Your eyes look uninteresting.";
        }
    }
}