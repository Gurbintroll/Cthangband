// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Mutations.Base;
using Cthangband.StaticData;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationEatMagic : BaseMutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.CheckIfRacialPowerWorks(17, 1, Ability.Wisdom, 15))
            {
                return;
            }
            saveGame.ItemFilter = saveGame.SpellEffects.ItemTesterHookRecharge;
            if (!saveGame.GetItem(out int item, "Drain which item? ", false, true, true))
            {
                return;
            }
            Item oPtr = item >= 0 ? player.Inventory[item] : level.Items[0 - item];
            int lev = oPtr.ItemType.Level;
            if (oPtr.Category == ItemCategory.Rod)
            {
                if (oPtr.TypeSpecificValue > 0)
                {
                    Profile.Instance.MsgPrint("You can't absorb energy from a discharged rod.");
                }
                else
                {
                    player.Vril += 2 * lev;
                    oPtr.TypeSpecificValue = 500;
                }
            }
            else
            {
                if (oPtr.TypeSpecificValue > 0)
                {
                    player.Vril += oPtr.TypeSpecificValue * lev;
                    oPtr.TypeSpecificValue = 0;
                }
                else
                {
                    Profile.Instance.MsgPrint("There's no energy there to absorb!");
                }
                oPtr.IdentifyFlags.Set(Constants.IdentEmpty);
            }
            if (player.Vril > player.MaxVril)
            {
                player.Vril = player.MaxVril;
            }
            player.NoticeFlags |= Constants.PnCombine | Constants.PnReorder;
        }

        public override string ActivationSummary(int lvl)
        {
            return lvl < 17 ? "eat magic        (unusable until level 17)" : "eat magic        (cost 1, WIS based)";
        }

        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "Your magic items look delicious.";
            HaveMessage = "You can consume magic energy for your own use.";
            LoseMessage = "Your magic items no longer look delicious.";
        }
    }
}