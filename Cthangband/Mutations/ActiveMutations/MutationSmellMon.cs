// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
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
    internal class MutationSmellMon : BaseMutation
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