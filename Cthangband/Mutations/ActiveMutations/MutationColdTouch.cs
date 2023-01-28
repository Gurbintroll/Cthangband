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
    internal class MutationColdTouch : BaseMutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.CheckIfRacialPowerWorks(2, 2, Ability.Constitution, 11))
            {
                return;
            }
            var targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionNoAim(out var dir))
            {
                return;
            }
            var y = player.MapY + level.KeypadDirectionYOffset[dir];
            var x = player.MapX + level.KeypadDirectionXOffset[dir];
            var cPtr = level.Grid[y][x];
            if (cPtr.MonsterIndex == 0)
            {
                Profile.Instance.MsgPrint("You wave your hands in the air.");
                return;
            }
            saveGame.SpellEffects.FireBolt(new ProjectCold(SaveGame.Instance.SpellEffects), dir, 2 * player.Level);
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 2 ? "cold touch       (unusable until level 2)" : "cold touch       (cost 2, CON based)";
        }

        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "Your hands get very cold.";
            HaveMessage = "You can freeze things with a touch.";
            LoseMessage = "Your hands warm up.";
        }
    }
}