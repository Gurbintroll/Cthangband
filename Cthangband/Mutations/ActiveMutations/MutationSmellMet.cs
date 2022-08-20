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
    internal class MutationSmellMet : BaseMutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(3, 2, Ability.Intelligence, 12))
            {
                saveGame.SpellEffects.DetectTreasure();
            }
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 3 ? "smell metal      (unusable until level 3)" : "smell metal      (cost 2, INT based)";
        }

        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "You smell a metallic odor.";
            HaveMessage = "You can smell nearby precious metal.";
            LoseMessage = "You no longer smell a metallic odor.";
        }
    }
}