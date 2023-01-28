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
    internal class MutationRadiation : BaseMutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.CheckIfRacialPowerWorks(15, 15, Ability.Constitution, 14))
            {
                return;
            }
            Profile.Instance.MsgPrint("Radiation flows from your body!");
            saveGame.SpellEffects.FireBall(new ProjectNuke(SaveGame.Instance.SpellEffects), 0, player.Level * 2, 3 + (player.Level / 20));
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 15
                ? "produce radiation   (unusable until level 15)"
                : "produce radiation   (cost 15, CON based)";
        }

        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You start emitting hard radiation.";
            HaveMessage = "You can emit hard radiation at will.";
            LoseMessage = "You stop emitting hard radiation.";
        }
    }
}