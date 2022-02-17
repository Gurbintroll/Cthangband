using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationWarning : Mutation
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
            int dangerAmount = 0;
            for (int monster = 0; monster < level.MMax; monster++)
            {
                Monster mPtr = level.Monsters[monster];
                MonsterRace rPtr = mPtr.Race;
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