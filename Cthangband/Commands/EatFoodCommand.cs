using Cthangband.Enumerations;
using Cthangband.StaticData;
using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Eat some food
    /// </summary>
    /// <param name="itemIndex"> The inventory index of the food item </param>
    [Serializable]
    internal class EatFoodCommand : ICommand
    {
        public char Key => 'E';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            int itemIndex = -999;
            // Get a food item from the inventory if one wasn't already specified
            Inventory.ItemFilterCategory = ItemCategory.Food;
            if (itemIndex == -999)
            {
                if (!SaveGame.Instance.GetItem(out itemIndex, "Eat which item? ", false, true, true))
                {
                    if (itemIndex == -2)
                    {
                        Profile.Instance.MsgPrint("You have nothing to eat.");
                    }
                    return;
                }
            }
            Item item = itemIndex >= 0 ? player.Inventory[itemIndex] : level.Items[0 - itemIndex];
            // Make sure the item is edible
            Inventory.ItemFilterCategory = ItemCategory.Food;
            if (!player.Inventory.ItemMatchesFilter(item))
            {
                Profile.Instance.MsgPrint("You can't eat that!");
                Inventory.ItemFilterCategory = 0;
                return;
            }
            Inventory.ItemFilterCategory = 0;
            // We don't actually eat dwarf bread
            if (item.ItemSubCategory != FoodType.Dwarfbread)
            {
                Gui.PlaySound(SoundEffect.Eat);
            }
            // Eating costs 100 energy
            SaveGame.Instance.EnergyUse = 100;
            bool ident = false;
            int itemLevel = item.ItemType.Level;
            switch (item.ItemSubCategory)
            {
                case FoodType.Poison:
                    {
                        if (!(player.HasPoisonResistance || player.TimedPoisonResistance != 0))
                        {
                            // Hagarg Ryonis may protect us from poison
                            if (Program.Rng.DieRoll(10) <= player.Religion.GetNamedDeity(Pantheon.GodName.Hagarg_Ryonis).AdjustedFavour)
                            {
                                Profile.Instance.MsgPrint("Hagarg Ryonis's favour protects you!");
                            }
                            else if (player.SetTimedPoison(player.TimedPoison + Program.Rng.RandomLessThan(10) + 10))
                            {
                                ident = true;
                            }
                        }
                        break;
                    }
                case FoodType.Blindness:
                    {
                        if (!player.HasBlindnessResistance)
                        {
                            if (player.SetTimedBlindness(player.TimedBlindness + Program.Rng.RandomLessThan(200) + 200))
                            {
                                ident = true;
                            }
                        }
                        break;
                    }
                case FoodType.Paranoia:
                    {
                        if (!player.HasFearResistance)
                        {
                            if (player.SetTimedFear(player.TimedFear + Program.Rng.RandomLessThan(10) + 10))
                            {
                                ident = true;
                            }
                        }
                        break;
                    }
                case FoodType.Confusion:
                    {
                        if (!player.HasConfusionResistance)
                        {
                            if (player.SetTimedConfusion(player.TimedConfusion + Program.Rng.RandomLessThan(10) + 10))
                            {
                                ident = true;
                            }
                        }
                        break;
                    }
                case FoodType.Hallucination:
                    {
                        if (!player.HasChaosResistance)
                        {
                            if (player.SetTimedHallucinations(player.TimedHallucinations + Program.Rng.RandomLessThan(250) + 250))
                            {
                                ident = true;
                            }
                        }
                        break;
                    }
                case FoodType.Paralysis:
                    {
                        if (!player.HasFreeAction)
                        {
                            if (player.SetTimedParalysis(player.TimedParalysis + Program.Rng.RandomLessThan(10) + 10))
                            {
                                ident = true;
                            }
                        }
                        break;
                    }
                case FoodType.Weakness:
                    {
                        player.TakeHit(Program.Rng.DiceRoll(6, 6), "poisonous food.");
                        player.TryDecreasingAbilityScore(Ability.Strength);
                        ident = true;
                        break;
                    }
                case FoodType.Sickness:
                    {
                        player.TakeHit(Program.Rng.DiceRoll(6, 6), "poisonous food.");
                        player.TryDecreasingAbilityScore(Ability.Constitution);
                        ident = true;
                        break;
                    }
                case FoodType.Stupidity:
                    {
                        player.TakeHit(Program.Rng.DiceRoll(8, 8), "poisonous food.");
                        player.TryDecreasingAbilityScore(Ability.Intelligence);
                        ident = true;
                        break;
                    }
                case FoodType.Naivety:
                    {
                        player.TakeHit(Program.Rng.DiceRoll(8, 8), "poisonous food.");
                        player.TryDecreasingAbilityScore(Ability.Wisdom);
                        ident = true;
                        break;
                    }
                case FoodType.Unhealth:
                    {
                        player.TakeHit(Program.Rng.DiceRoll(10, 10), "poisonous food.");
                        player.TryDecreasingAbilityScore(Ability.Constitution);
                        ident = true;
                        break;
                    }
                case FoodType.Disease:
                    {
                        player.TakeHit(Program.Rng.DiceRoll(10, 10), "poisonous food.");
                        player.TryDecreasingAbilityScore(Ability.Strength);
                        ident = true;
                        break;
                    }
                case FoodType.CurePoison:
                    {
                        if (player.SetTimedPoison(0))
                        {
                            ident = true;
                        }
                        break;
                    }
                case FoodType.CureBlindness:
                    {
                        if (player.SetTimedBlindness(0))
                        {
                            ident = true;
                        }
                        break;
                    }
                case FoodType.CureParanoia:
                    {
                        if (player.SetTimedFear(0))
                        {
                            ident = true;
                        }
                        break;
                    }
                case FoodType.CureConfusion:
                    {
                        if (player.SetTimedConfusion(0))
                        {
                            ident = true;
                        }
                        break;
                    }
                case FoodType.CureSerious:
                    {
                        if (player.RestoreHealth(Program.Rng.DiceRoll(4, 8)))
                        {
                            ident = true;
                        }
                        break;
                    }
                case FoodType.RestoreStr:
                    {
                        if (player.TryRestoringAbilityScore(Ability.Strength))
                        {
                            ident = true;
                        }
                        break;
                    }
                case FoodType.RestoreCon:
                    {
                        if (player.TryRestoringAbilityScore(Ability.Constitution))
                        {
                            ident = true;
                        }
                        break;
                    }
                case FoodType.Restoring:
                    {
                        if (player.TryRestoringAbilityScore(Ability.Strength))
                        {
                            ident = true;
                        }
                        if (player.TryRestoringAbilityScore(Ability.Intelligence))
                        {
                            ident = true;
                        }
                        if (player.TryRestoringAbilityScore(Ability.Wisdom))
                        {
                            ident = true;
                        }
                        if (player.TryRestoringAbilityScore(Ability.Dexterity))
                        {
                            ident = true;
                        }
                        if (player.TryRestoringAbilityScore(Ability.Constitution))
                        {
                            ident = true;
                        }
                        if (player.TryRestoringAbilityScore(Ability.Charisma))
                        {
                            ident = true;
                        }
                        break;
                    }
                case FoodType.Ration:
                case FoodType.Biscuit:
                case FoodType.Jerky:
                    {
                        Profile.Instance.MsgPrint("That tastes good.");
                        ident = true;
                        break;
                    }
                case FoodType.Dwarfbread:
                    {
                        Profile.Instance.MsgPrint("You look at the dwarf bread, and don't feel quite so hungry anymore.");
                        ident = true;
                        break;
                    }
                case FoodType.SlimeMold:
                    {
                        SaveGame.Instance.CommandEngine.PotionEffect(PotionType.SlimeMold);
                        ident = true;
                        break;
                    }
                case FoodType.Waybread:
                    {
                        Profile.Instance.MsgPrint("That tastes good.");
                        player.SetTimedPoison(0);
                        player.RestoreHealth(Program.Rng.DiceRoll(4, 8));
                        ident = true;
                        break;
                    }
                case FoodType.PintOfAle:
                case FoodType.PintOfWine:
                    {
                        Profile.Instance.MsgPrint("That tastes good.");
                        ident = true;
                        break;
                    }
                case FoodType.Warpstone:
                    {
                        Profile.Instance.MsgPrint("That tastes... funky.");
                        player.Dna.GainMutation();
                        if (Program.Rng.DieRoll(3) == 1)
                        {
                            player.Dna.GainMutation();
                        }
                        if (Program.Rng.DieRoll(3) == 1)
                        {
                            player.Dna.GainMutation();
                        }
                        ident = true;
                        break;
                    }
            }
            player.NoticeFlags |= Constants.PnCombine | Constants.PnReorder;
            // We've tried this type of object
            item.ObjectTried();
            // Learn its flavour if necessary
            if (ident && !item.IsFlavourAware())
            {
                item.BecomeFlavourAware();
                player.GainExperience((itemLevel + (player.Level >> 1)) / player.Level);
            }
            // Dwarf bread isn't actually eaten so reduce our hunger and return early
            if (item.ItemSubCategory == FoodType.Dwarfbread)
            {
                player.SetFood(player.Food + item.TypeSpecificValue);
                return;
            }
            // Vampires only get 1/10th of the food value
            if (player.RaceIndex == RaceId.Vampire)
            {
                _ = player.SetFood(player.Food + (item.TypeSpecificValue / 10));
                Profile.Instance.MsgPrint("Mere victuals hold scant sustenance for a being such as yourself.");
                if (player.Food < Constants.PyFoodAlert)
                {
                    Profile.Instance.MsgPrint("Your hunger can only be satisfied with fresh blood!");
                }
            }
            // Skeletons get no food sustenance
            else if (player.RaceIndex == RaceId.Skeleton)
            {
                if (!(item.ItemSubCategory == FoodType.Waybread || item.ItemSubCategory == FoodType.Warpstone ||
                      item.ItemSubCategory < FoodType.Biscuit))
                {
                    // Spawn a new food item on the floor to make up for the one that will be destroyed
                    Item floorItem = new Item();
                    Profile.Instance.MsgPrint("The food falls through your jaws!");
                    floorItem.AssignItemType(
                        Profile.Instance.ItemTypes.LookupKind(item.Category, item.ItemSubCategory));
                    SaveGame.Instance.Level.DropNear(floorItem, -1, player.MapY, player.MapX);
                }
                else
                {
                    // But some magical types work anyway and then vanish
                    Profile.Instance.MsgPrint("The food falls through your jaws and vanishes!");
                }
            }
            // Golems, zombies, and spectres get only 1/20th of the food value
            else if (player.RaceIndex == RaceId.Golem || player.RaceIndex == RaceId.Zombie || player.RaceIndex == RaceId.Spectre)
            {
                Profile.Instance.MsgPrint("The food of mortals is poor sustenance for you.");
                player.SetFood(player.Food + (item.TypeSpecificValue / 20));
            }
            // Everyone else gets the full value
            else
            {
                player.SetFood(player.Food + item.TypeSpecificValue);
            }
            // Use up the item (if it fell to the floor this will have already been dealt with)
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
