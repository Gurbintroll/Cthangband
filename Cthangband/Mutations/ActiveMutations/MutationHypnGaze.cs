using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationHypnGaze : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.CheckIfRacialPowerWorks(12, 12, Ability.Charisma, 18))
            {
                return;
            }
            TargetEngine targetEngine = new TargetEngine(player, level);
            Profile.Instance.MsgPrint("Your eyes look mesmerizing...");
            if (targetEngine.GetAimDir(out int dir))
            {
                saveGame.SpellEffects.CharmMonster(dir, player.Level);
            }
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 20 ? "hypnotic gaze    (unusable until level 12)" : "hypnotic gaze    (cost 12, CHA based)";
        }

        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "Your eyes look mesmerizing...";
            HaveMessage = "Your gaze is hypnotic.";
            LoseMessage = "Your eyes look uninteresting.";
        }
    }
}