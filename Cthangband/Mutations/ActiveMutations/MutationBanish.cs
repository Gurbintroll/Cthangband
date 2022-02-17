using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationBanish : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.RacialAux(25, 25, Ability.Wisdom, 18))
            {
                return;
            }
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetRepDir(out int dir))
            {
                return;
            }
            int y = player.MapY + level.KeypadDirectionYOffset[dir];
            int x = player.MapX + level.KeypadDirectionXOffset[dir];
            GridTile cPtr = level.Grid[y][x];
            if (cPtr.Monster == 0)
            {
                Profile.Instance.MsgPrint("You sense no evil there!");
                return;
            }
            Monster mPtr = level.Monsters[cPtr.Monster];
            MonsterRace rPtr = mPtr.Race;
            if ((rPtr.Flags3 & MonsterFlag3.Evil) != 0)
            {
                level.Monsters.DeleteMonsterByIndex(cPtr.Monster, true);
                Profile.Instance.MsgPrint("The evil creature vanishes in a puff of sulfurous smoke!");
            }
            else
            {
                Profile.Instance.MsgPrint("Your invocation is ineffectual!");
            }
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 25 ? "banish evil      (unusable until level 25)" : "banish evil      (cost 25, WIS based)";
        }

        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "You feel a holy wrath fill you.";
            HaveMessage = "You can send evil creatures directly to Hell.";
            LoseMessage = "You no longer feel a holy wrath.";
        }
    }
}