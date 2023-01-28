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
    internal class MutationIllumine : BaseMutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.CommandEngine.CheckIfRacialPowerWorks(3, 2, Ability.Intelligence, 10))
            {
                saveGame.SpellEffects.LightArea(Program.Rng.DiceRoll(2, player.Level / 2), (player.Level / 10) + 1);
            }
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 3 ? "illuminate       (unusable until level 3)" : "illuminate       (cost 2, INT based)";
        }

        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "You can light up rooms with your presence.";
            HaveMessage = "You can emit bright light.";
            LoseMessage = "You can no longer light up rooms with your presence.";
        }
    }
}