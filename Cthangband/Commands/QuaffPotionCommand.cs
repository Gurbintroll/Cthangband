using Cthangband.Enumerations;
using Cthangband.StaticData;
using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Quaff a potion from the inventory or the ground
    /// </summary>
    /// <param name="itemIndex"> The inventory index of the potion to quaff </param>
    [Serializable]
    internal class QuaffPotionCommand : ICommand
    {
        public char Key => 'q';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            int itemIndex = -999;

            // Get an item if we didn't already have one
            Inventory.ItemFilterCategory = ItemCategory.Potion;
            if (itemIndex == -999)
            {
                if (!SaveGame.Instance.GetItem(out itemIndex, "Quaff which potion? ", true, true, true))
                {
                    if (itemIndex == -2)
                    {
                        Profile.Instance.MsgPrint("You have no potions to quaff.");
                    }
                    return;
                }
            }
            Item item = itemIndex >= 0 ? player.Inventory[itemIndex] : level.Items[0 - itemIndex];
            // Make sure the item is a potion
            Inventory.ItemFilterCategory = ItemCategory.Potion;
            if (!player.Inventory.ItemMatchesFilter(item))
            {
                Profile.Instance.MsgPrint("That is not a potion!");
                return;
            }
            Inventory.ItemFilterCategory = 0;
            Gui.PlaySound(SoundEffect.Quaff);
            // Drinking a potion costs a whole turn
            SaveGame.Instance.EnergyUse = 100;
            int itemLevel = item.ItemType.Level;
            // Do the actual potion effect
            bool identified = SaveGame.Instance.CommandEngine.PotionEffect(item.ItemSubCategory);
            // Skeletons are messy drinkers
            if (player.RaceIndex == RaceId.Skeleton && Program.Rng.DieRoll(12) == 1)
            {
                Profile.Instance.MsgPrint("Some of the fluid falls through your jaws!");
                SaveGame.Instance.SpellEffects.PotionSmashEffect(0, player.MapY, player.MapX, item.ItemSubCategory);
            }
            player.NoticeFlags |= Constants.PnCombine | Constants.PnReorder;
            // We may now know the potion's type
            item.ObjectTried();
            if (identified && !item.IsFlavourAware())
            {
                item.BecomeFlavourAware();
                player.GainExperience((itemLevel + (player.Level >> 1)) / player.Level);
            }
            // Most potions give us a bit of food too
            player.SetFood(player.Food + item.TypeSpecificValue);
            bool channeled = false;
            // If we're a channeler, we might be able to spend mana instead of using it up
            if (player.Spellcasting.Type == CastingType.Channeling)
            {
                channeled = SaveGame.Instance.CommandEngine.DoCmdChannel(item);
            }
            if (!channeled)
            {
                // We didn't channel it, so use up one potion from the stack
                if (itemIndex >= 0)
                {
                    player.Inventory.InvenItemIncrease(itemIndex, -1);
                    player.Inventory.InvenItemDescribe(itemIndex);
                    player.Inventory.InvenItemOptimize(itemIndex);
                }
                else
                {
                    SaveGame.Instance.Level.FloorItemIncrease(0 - itemIndex, -1);
                    SaveGame.Instance.Level.FloorItemDescribe(0 - itemIndex);
                    SaveGame.Instance.Level.FloorItemOptimize(0 - itemIndex);
                }
            }
        }
    }
}
