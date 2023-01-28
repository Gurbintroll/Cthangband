// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Mutations.Base;
using Cthangband.StaticData;
using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationAttDragon : BaseMutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "You start attracting dragons.";
            HaveMessage = "You attract dragons.";
            LoseMessage = "You stop attracting dragons.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (player.HasAntiMagic || Program.Rng.DieRoll(3000) != 13)
            {
                return;
            }
            bool dSummon;
            if (Program.Rng.DieRoll(5) == 1)
            {
                dSummon = level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, saveGame.Difficulty,
                    Constants.SummonDragon, true);
            }
            else
            {
                dSummon = level.Monsters.SummonSpecific(player.MapY, player.MapX, saveGame.Difficulty,
                    Constants.SummonDragon);
            }
            if (!dSummon)
            {
                return;
            }
            Profile.Instance.MsgPrint("You have attracted a dragon!");
            saveGame.Disturb(false);
        }
    }
}