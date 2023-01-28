// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Mutations.Base;
using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationDisarm : BaseMutation
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
            var oPtr = player.Inventory[InventorySlot.MeleeWeapon];
            if (oPtr.ItemType == null)
            {
                return;
            }
            Profile.Instance.MsgPrint("You drop your weapon!");
            player.Inventory.InvenDrop(InventorySlot.MeleeWeapon, 1);
        }
    }
}