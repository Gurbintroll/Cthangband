// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.StaticData;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationSterility : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.CheckIfRacialPowerWorks(20, 40, Ability.Charisma, 18))
            {
                return;
            }
            Profile.Instance.MsgPrint("You suddenly have a headache!");
            player.TakeHit(Program.Rng.DieRoll(30) + 30, "the strain of forcing abstinence");
            level.Monsters.NumRepro += Constants.MaxRepro;
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 20 ? "sterilize        (unusable until level 20)" : "sterilize        (cost 40, CHA based)";
        }

        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "You can give everything around you a headache.";
            HaveMessage = "You can cause mass impotence.";
            LoseMessage = "You hear a massed sigh of relief.";
        }
    }
}