using Cthangband.Enumerations;
using Cthangband.StaticData;
using System;

namespace Cthangband.Mutations.ActiveMutations
{
    [Serializable]
    internal class MutationEatMagic : Mutation
    {
        public override void Activate(SaveGame saveGame, Player player, Level level)
        {
            if (!saveGame.CommandEngine.RacialAux(17, 1, Ability.Wisdom, 15))
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
                    player.Mana += 2 * lev;
                    oPtr.TypeSpecificValue = 500;
                }
            }
            else
            {
                if (oPtr.TypeSpecificValue > 0)
                {
                    player.Mana += oPtr.TypeSpecificValue * lev;
                    oPtr.TypeSpecificValue = 0;
                }
                else
                {
                    Profile.Instance.MsgPrint("There's no energy there to absorb!");
                }
                oPtr.IdentifyFlags.Set(Constants.IdentEmpty);
            }
            if (player.Mana > player.MaxMana)
            {
                player.Mana = player.MaxMana;
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