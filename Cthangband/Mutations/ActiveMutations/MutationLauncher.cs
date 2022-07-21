// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Commands;
using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationLauncher : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.CheckIfRacialPowerWorks(1, player.Level, Ability.Strength, 6))
            {
                return;
            }
            ThrowCommand.DoCmdThrow(player, level, 2 + (player.Level / 16));
        }

        public override string ActivationSummary(int lvl)
        {
            return "throw object     (cost lev, STR based)";
        }

        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "Your throwing arm feels much stronger.";
            HaveMessage = "You can hurl objects with great force.";
            LoseMessage = "Your throwing arm feels much weaker.";
        }
    }
}