// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
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
    internal class MutationShriek : BaseMutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.CheckIfRacialPowerWorks(4, 4, Ability.Constitution, 6))
            {
                return;
            }
            saveGame.SpellEffects.FireBall(new ProjectSound(SaveGame.Instance.SpellEffects), 0, 4 * player.Level, 8);
            saveGame.SpellEffects.AggravateMonsters(0);
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 4 ? "shriek           (unusable until level 4)" : "shriek           (cost 4, CON based)";
        }

        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "Your vocal cords get much tougher.";
            HaveMessage = "You can emit a horrible shriek.";
            LoseMessage = "Your vocal cords get much weaker.";
        }
    }
}