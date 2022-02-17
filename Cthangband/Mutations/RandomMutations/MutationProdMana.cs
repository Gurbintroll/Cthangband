using Cthangband.Projection;
using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationProdMana : Mutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "You start producing magical energy uncontrollably.";
            HaveMessage = "You are producing magical energy uncontrollably.";
            LoseMessage = "You stop producing magical energy uncontrollably.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (player.HasAntiMagic || Program.Rng.DieRoll(9000) != 1)
            {
                return;
            }
            TargetEngine targetEngine = new TargetEngine(player, level);
            saveGame.Disturb(false);
            Profile.Instance.MsgPrint("Magical energy flows through you! You must release it!");
            Profile.Instance.MsgPrint(null);
            targetEngine.GetHackDir(out int dire);
            saveGame.SpellEffects.FireBall(new ProjectMana(SaveGame.Instance.SpellEffects), dire, player.Level * 2, 3);
        }
    }
}