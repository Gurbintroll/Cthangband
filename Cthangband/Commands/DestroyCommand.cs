using Cthangband.Enumerations;
using Cthangband.StaticData;
using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Destroy a single item
    /// </summary>
    [Serializable]
    internal class DestroyCommand : ICommand, IStoreCommand
    {
        public char Key => 'k';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public bool RequiresRerendering => false;

        public void Execute(Player player)
        {
            // TODO: This is a deviation because the browse command didn't have the store in mind when designed.
            // We need to inject the level but ideally, the functionality can be tweaked to remove this
            // unoptimized code.
            Execute(player, SaveGame.Instance.Level);
        }

        public void Execute(Player player, Level level)
        {
            int amount = 1;
            bool force = Gui.CommandArgument > 0;
            // Get an item to destroy
            if (!SaveGame.Instance.GetItem(out int itemIndex, "Destroy which item? ", false, true, true))
            {
                if (itemIndex == -2)
                {
                    Profile.Instance.MsgPrint("You have nothing to destroy.");
                }
                return;
            }
            Item item = itemIndex >= 0 ? player.Inventory[itemIndex] : level.Items[0 - itemIndex];
            // If we have more than one we might not want to destroy all of them
            if (item.Count > 1)
            {
                amount = Gui.GetQuantity(null, item.Count, true);
                if (amount <= 0)
                {
                    return;
                }
            }
            int oldNumber = item.Count;
            item.Count = amount;
            string itemName = item.Description(true, 3);
            item.Count = oldNumber;
            //Only confirm if it's not a worthless item
            if (!force)
            {
                if (!item.Stompable())
                {
                    string outVal = $"Really destroy {itemName}? ";
                    if (!Gui.GetCheck(outVal))
                    {
                        return;
                    }
                    // If it was something we might want to destroy again, ask
                    if (!item.ItemType.HasQuality() && item.ItemType.Category != ItemCategory.Chest)
                    {
                        if (item.IsKnown())
                        {
                            if (Gui.GetCheck($"Always destroy {itemName}?"))
                            {
                                item.ItemType.Stompable[0] = true;
                            }
                        }
                    }
                }
            }
            // Destroying something takes a turn
            SaveGame.Instance.EnergyUse = 100;
            // Can't destroy an artifact artifact
            if (item.IsFixedArtifact() || !string.IsNullOrEmpty(item.RandartName))
            {
                string feel = "special";
                SaveGame.Instance.EnergyUse = 0;
                Profile.Instance.MsgPrint($"You cannot destroy {itemName}.");
                if (item.IsCursed() || item.IsBroken())
                {
                    feel = "terrible";
                }
                item.Inscription = feel;
                item.IdentifyFlags.Set(Constants.IdentSense);
                player.NoticeFlags |= Constants.PnCombine;
                player.RedrawNeeded.Set(RedrawFlag.PrEquippy);
                return;
            }
            Profile.Instance.MsgPrint($"You destroy {itemName}.");
            // Warriors and paladins get experience for destroying magic books
            if (SaveGame.Instance.CommandEngine.ItemFilterHighLevelBook(item))
            {
                bool gainExpr = false;
                if (player.ProfessionIndex == CharacterClass.Warrior)
                {
                    gainExpr = true;
                }
                else if (player.ProfessionIndex == CharacterClass.Paladin)
                {
                    if (player.Realm1 == Realm.Life)
                    {
                        if (item.Category == ItemCategory.DeathBook)
                        {
                            gainExpr = true;
                        }
                    }
                    else
                    {
                        if (item.Category == ItemCategory.LifeBook)
                        {
                            gainExpr = true;
                        }
                    }
                }
                if (gainExpr && player.ExperiencePoints < Constants.PyMaxExp)
                {
                    int testerExp = player.MaxExperienceGained / 20;
                    if (testerExp > 10000)
                    {
                        testerExp = 10000;
                    }
                    if (item.ItemSubCategory < 3)
                    {
                        testerExp /= 4;
                    }
                    if (testerExp < 1)
                    {
                        testerExp = 1;
                    }
                    Profile.Instance.MsgPrint("You feel more experienced.");
                    player.GainExperience(testerExp * amount);
                }
            }
            // Tidy up the player's inventory
            if (itemIndex >= 0)
            {
                player.Inventory.InvenItemIncrease(itemIndex, -amount);
                player.Inventory.InvenItemDescribe(itemIndex);
                player.Inventory.InvenItemOptimize(itemIndex);
            }
            else
            {
                SaveGame.Instance.Level.FloorItemIncrease(0 - itemIndex, -amount);
                SaveGame.Instance.Level.FloorItemDescribe(0 - itemIndex);
                SaveGame.Instance.Level.FloorItemOptimize(0 - itemIndex);
            }
        }
    }
}
