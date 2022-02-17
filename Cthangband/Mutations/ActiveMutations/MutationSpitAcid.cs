using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationSpitAcid : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.RacialAux(9, 9, Ability.Dexterity, 15))
            {
                return;
            }
            TargetEngine targetEngine = new TargetEngine(player, level);
            Profile.Instance.MsgPrint("You spit acid...");
            if (targetEngine.GetAimDir(out int dir))
            {
                saveGame.SpellEffects.FireBall(new ProjectAcid(SaveGame.Instance.SpellEffects), dir, player.Level, 1 + (player.Level / 30));
            }
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 9
                ? "spit acid        (unusable until level 9)"
                : $"spit acid        (cost 9, dam {lvl}, DEX based)";
        }

        public override void Initialise()
        {
            Frequency = 4;
            GainMessage = "You gain the ability to spit acid.";
            HaveMessage = "You can spit acid (dam lvl).";
            LoseMessage = "You lose the ability to spit acid.";
        }
    }
}