using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationSwapPos : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.CheckIfRacialPowerWorks(15, 12, Ability.Dexterity, 16))
            {
                return;
            }
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.TeleportSwap(dir);
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 15 ? "swap position    (unusable until level 15)" : "swap position    (cost 12, DEX based)";
        }

        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You feel like walking a mile in someone else's shoes.";
            HaveMessage = "You can switch locations with another being.";
            LoseMessage = "You feel like staying in your own shoes.";
        }
    }
}