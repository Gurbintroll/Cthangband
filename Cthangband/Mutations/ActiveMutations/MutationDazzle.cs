using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationDazzle : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.CheckIfRacialPowerWorks(7, 15, Ability.Charisma, 8))
            {
                return;
            }
            saveGame.SpellEffects.StunMonsters(player.Level * 4);
            saveGame.SpellEffects.ConfuseMonsters(player.Level * 4);
            saveGame.SpellEffects.TurnMonsters(player.Level * 4);
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 7 ? "dazzle           (unusable until level 7)" : "dazzle           (cost 15, CHA based)";
        }

        public override void Initialise()
        {
            Frequency = 3;
            GainMessage = "You gain the ability to emit dazzling lights.";
            HaveMessage = "You can emit confusing, blinding radiation.";
            LoseMessage = "You lose the ability to emit dazzling lights.";
        }
    }
}