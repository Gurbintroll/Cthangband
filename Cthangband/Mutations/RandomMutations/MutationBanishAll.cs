// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Mutations.Base;
using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationBanishAll : BaseMutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You feel a terrifying power lurking behind you.";
            HaveMessage = "You sometimes cause nearby creatures to vanish.";
            LoseMessage = "You no longer feel a terrifying power lurking behind you.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (Program.Rng.DieRoll(9000) != 1)
            {
                return;
            }
            saveGame.Disturb(false);
            Profile.Instance.MsgPrint("You suddenly feel almost lonely.");
            saveGame.SpellEffects.BanishMonsters(100);
            Profile.Instance.MsgPrint(null);
        }
    }
}