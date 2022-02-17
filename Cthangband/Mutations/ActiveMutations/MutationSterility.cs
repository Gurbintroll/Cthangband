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
            if (!saveGame.CommandEngine.RacialAux(20, 40, Ability.Charisma, 18))
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