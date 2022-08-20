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
    internal class MutationDazzle : BaseMutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.CheckIfRacialPowerWorks(7, 15, Ability.Charisma, 8))
            {
                return;
            }
            saveGame.SpellEffects.StunMonsters(player.Level * 4);
            saveGame.SpellEffects.ConfuseMonsters(player.Level * 4);
            saveGame.SpellEffects.TurnMonsters(player.Level * 4);
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 7 ? "dazzle           (unusable until level 7)" : "dazzle           (cost 15, CHA based)";
        }

        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "You gain the ability to emit dazzling lights.";
            HaveMessage = "You can emit confusing, blinding radiation.";
            LoseMessage = "You lose the ability to emit dazzling lights.";
        }
    }
}