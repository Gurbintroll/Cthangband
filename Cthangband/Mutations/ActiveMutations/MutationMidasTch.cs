﻿// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
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
    internal class MutationMidasTch : BaseMutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(10, 5, Ability.Intelligence, 12))
            {
                saveGame.SpellEffects.Alchemy();
            }
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 10 ? "midas touch      (unusable until level 10)" : "midas touch      (cost 5, INT based)";
        }

        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You gain the Midas touch.";
            HaveMessage = "You can turn ordinary items to gold.";
            LoseMessage = "You lose the Midas touch.";
        }
    }
}