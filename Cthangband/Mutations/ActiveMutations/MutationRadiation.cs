using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationRadiation : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.RacialAux(15, 15, Ability.Constitution, 14))
            {
                return;
            }
            Profile.Instance.MsgPrint("Radiation flows from your body!");
            saveGame.SpellEffects.FireBall(new ProjectNuke(SaveGame.Instance.SpellEffects), 0, player.Level * 2, 3 + (player.Level / 20));
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 15
                ? "produce radiation   (unusable until level 15)"
                : "produce radiation   (cost 15, CON based)";
        }

        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You start emitting hard radiation.";
            HaveMessage = "You can emit hard radiation at will.";
            LoseMessage = "You stop emitting hard radiation.";
        }
    }
}