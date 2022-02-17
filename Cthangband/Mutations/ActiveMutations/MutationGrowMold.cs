using Cthangband.Enumerations;
using Cthangband.StaticData;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationGrowMold : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.RacialAux(1, 6, Ability.Constitution, 14))
            {
                return;
            }
            for (int i = 0; i < 8; i++)
            {
                level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, player.Level, Constants.SummonBizarre1,
                    false);
            }
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 1 ? "grow mold        (unusable until level 1)" : "grow mold        (cost 6, CON based)";
        }

        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "You feel a sudden affinity for mold.";
            HaveMessage = "You can cause mold to grow near you.";
            LoseMessage = "You feel a sudden dislike for mold.";
        }
    }
}