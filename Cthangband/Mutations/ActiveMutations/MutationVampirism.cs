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
    internal class MutationVampirism : BaseMutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.CheckIfRacialPowerWorks(13, player.Level, Ability.Constitution, 14))
            {
                return;
            }
            var targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out var dir))
            {
                return;
            }
            if (SaveGame.Instance.SpellEffects.DrainLife(dir, player.Level * 2))
            {
                player.RestoreHealth(player.Level + Program.Rng.DieRoll(player.Level));
            }
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 13
                ? "vampiric drain   (unusable until level 13)"
                : $"vampiric drain   (cost {lvl}, CON based)";
        }

        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You become vampiric.";
            HaveMessage = "You can drain life from a foe like a vampire.";
            LoseMessage = "You are no longer vampiric.";
        }
    }
}