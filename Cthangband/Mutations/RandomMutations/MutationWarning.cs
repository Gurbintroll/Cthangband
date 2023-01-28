// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
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
    internal class MutationWarning : BaseMutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You suddenly feel paranoid.";
            HaveMessage = "You receive warnings about your foes.";
            LoseMessage = "You no longer feel paranoid.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (Program.Rng.DieRoll(1000) != 1)
            {
                return;
            }
            var dangerAmount = 0;
            for (var monster = 0; monster < level.MMax; monster++)
            {
                var mPtr = level.Monsters[monster];
                var rPtr = mPtr.Race;
                if (mPtr.Race == null)
                {
                    continue;
                }
                if (rPtr.Level >= player.Level)
                {
                    dangerAmount += rPtr.Level - player.Level + 1;
                }
            }
            if (dangerAmount > 100)
            {
                Profile.Instance.MsgPrint("You feel utterly terrified!");
            }
            else if (dangerAmount > 50)
            {
                Profile.Instance.MsgPrint("You feel terrified!");
            }
            else if (dangerAmount > 20)
            {
                Profile.Instance.MsgPrint("You feel very worried!");
            }
            else if (dangerAmount > 10)
            {
                Profile.Instance.MsgPrint("You feel paranoid!");
            }
            else if (dangerAmount > 5)
            {
                Profile.Instance.MsgPrint("You feel almost safe.");
            }
            else
            {
                Profile.Instance.MsgPrint("You feel lonely.");
            }
        }
    }
}