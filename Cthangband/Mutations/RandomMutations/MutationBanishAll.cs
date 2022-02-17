using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationBanishAll : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "You feel a terrifying power lurking behind you.";
            HaveMessage = "You sometimes cause nearby creatures to vanish.";
            LoseMessage = "You no longer feel a terrifying power lurking behind you.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (Program.Rng.DieRoll(9000) != 1)
            {
                return;
            }
            saveGame.Disturb(false);
            Profile.Instance.MsgPrint("You suddenly feel almost lonely.");
            saveGame.SpellEffects.BanishMonsters(100);
            Profile.Instance.MsgPrint(null);
        }
    }
}