using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationEarthquake : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.CheckIfRacialPowerWorks(12, 12, Ability.Strength, 16))
            {
                return;
            }
            if (!saveGame.Quests.IsQuest(saveGame.DunLevel) && saveGame.DunLevel != 0)
            {
                saveGame.SpellEffects.Earthquake(player.MapY, player.MapX, 10);
            }
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 12 ? "earthquake       (unusable until level 12)" : "earthquake       (cost 12, STR based)";
        }

        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "You gain the ability to wreck the dungeon.";
            HaveMessage = "You can bring down the dungeon around your ears.";
            LoseMessage = "You lose the ability to wreck the dungeon.";
        }
    }
}