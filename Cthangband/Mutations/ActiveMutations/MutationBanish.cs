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
    internal class MutationBanish : BaseMutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.CheckIfRacialPowerWorks(25, 25, Ability.Wisdom, 18))
            {
                return;
            }
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionNoAim(out int dir))
            {
                return;
            }
            int y = player.MapY + level.KeypadDirectionYOffset[dir];
            int x = player.MapX + level.KeypadDirectionXOffset[dir];
            GridTile cPtr = level.Grid[y][x];
            if (cPtr.MonsterIndex == 0)
            {
                Profile.Instance.MsgPrint("You sense no evil there!");
                return;
            }
            Monster mPtr = level.Monsters[cPtr.MonsterIndex];
            MonsterRace rPtr = mPtr.Race;
            if ((rPtr.Flags3 & MonsterFlag3.Evil) != 0)
            {
                level.Monsters.DeleteMonsterByIndex(cPtr.MonsterIndex, true);
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