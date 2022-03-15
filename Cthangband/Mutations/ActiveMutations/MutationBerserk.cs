﻿// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationBerserk : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.CheckIfRacialPowerWorks(8, 8, Ability.Strength, 14))
            {
                return;
            }
            player.SetTimedSuperheroism(player.TimedSuperheroism + Program.Rng.DieRoll(25) + 25);
            player.RestoreHealth(30);
            player.SetTimedFear(0);
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 8 ? "berserk          (unusable until level 8)" : "berserk          (cost 8, STR based)";
        }

        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "You feel a controlled rage.";
            HaveMessage = "You can drive yourself into a berserk frenzy.";
            LoseMessage = "You no longer feel a controlled rage.";
        }
    }
}