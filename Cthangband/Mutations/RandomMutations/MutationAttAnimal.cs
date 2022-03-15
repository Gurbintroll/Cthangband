// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.StaticData;
using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationAttAnimal : Mutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "You start attracting animals.";
            HaveMessage = "You attract animals.";
            LoseMessage = "You stop attracting animals.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (player.HasAntiMagic || Program.Rng.DieRoll(7000) != 1)
            {
                return;
            }
            bool aSummon;
            if (Program.Rng.DieRoll(3) == 1)
            {
                aSummon = level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, saveGame.Difficulty,
                    Constants.SummonAnimal, true);
            }
            else
            {
                aSummon = level.Monsters.SummonSpecific(player.MapY, player.MapX, saveGame.Difficulty,
                    Constants.SummonAnimal);
            }
            if (!aSummon)
            {
                return;
            }
            Profile.Instance.MsgPrint("You have attracted an animal!");
            saveGame.Disturb(false);
        }
    }
}