using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationDisarm : Mutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "Your feet grow to four times their former size.";
            HaveMessage = "You occasionally stumble and drop things.";
            LoseMessage = "Your feet shrink to their former size.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (Program.Rng.DieRoll(10000) != 1)
            {
                return;
            }
            saveGame.Disturb(false);
            Profile.Instance.MsgPrint("You trip over your own feet!");
            player.TakeHit(Program.Rng.DieRoll(player.Weight / 6), "tripping");
            Profile.Instance.MsgPrint(null);
            Item oPtr = player.Inventory[InventorySlot.MeleeWeapon];
            if (oPtr.ItemType == null)
            {
                return;
            }
            Profile.Instance.MsgPrint("You drop your weapon!");
            player.Inventory.InvenDrop(InventorySlot.MeleeWeapon, 1);
        }
    }
}