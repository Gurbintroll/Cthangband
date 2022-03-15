// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
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
    internal class MutationPolymorph : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(18, 20, Ability.Constitution, 18))
            {
                player.PolymorphSelf();
            }
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 18 ? "polymorph        (unusable until level 18)" : "polymorph        (cost 20, CON based)";
        }

        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "Your body seems mutable.";
            HaveMessage = "You can polymorph yourself at will.";
            LoseMessage = "Your body seems stable.";
        }
    }
}