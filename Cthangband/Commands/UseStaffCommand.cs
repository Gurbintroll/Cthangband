using Cthangband.Enumerations;
using Cthangband.StaticData;
using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Use a staff from the inventory or floor
    /// </summary>
    /// <param name="itemIndex"> The inventory index of the item to use </param>
    [Serializable]
    internal class UseStaffCommand : ICommand
    {
        public char Key => 'u';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            int itemIndex = -999;

            // Get an item if we weren't passed one
            Inventory.ItemFilterCategory = ItemCategory.Staff;
            if (itemIndex == -999)
            {
                if (!SaveGame.Instance.GetItem(out itemIndex, "Use which staff? ", false, true, true))
                {
                    if (itemIndex == -2)
                    {
                        Profile.Instance.MsgPrint("You have no staff to use.");
                    }
                    return;
                }
            }
            Item item = itemIndex >= 0 ? player.Inventory[itemIndex] : level.Items[0 - itemIndex];
            // Make sure the item is actually a staff
            Inventory.ItemFilterCategory = ItemCategory.Staff;
            if (!player.Inventory.ItemMatchesFilter(item))
            {
                Profile.Instance.MsgPrint("That is not a staff!");
                Inventory.ItemFilterCategory = 0;
                return;
            }
            Inventory.ItemFilterCategory = 0;
            // We can't use a staff from the floor
            if (itemIndex < 0 && item.Count > 1)
            {
                Profile.Instance.MsgPrint("You must first pick up the staffs.");
                return;
            }
            // Using a staff costs a full turn
            SaveGame.Instance.EnergyUse = 100;
            bool identified = false;
            int itemLevel = item.ItemType.Level;
            // We have a chance of the device working equal to skill (halved if confused) - item
            // level (capped at 50)
            int chance = player.SkillUseDevice;
            if (player.TimedConfusion != 0)
            {
                chance /= 2;
            }
            chance -= itemLevel > 50 ? 50 : itemLevel;
            // Always a small chance of it working
            if (chance < Constants.UseDevice && Program.Rng.RandomLessThan(Constants.UseDevice - chance + 1) == 0)
            {
                chance = Constants.UseDevice;
            }
            // Check to see if we use it properly
            if (chance < Constants.UseDevice || Program.Rng.DieRoll(chance) < Constants.UseDevice)
            {
                Profile.Instance.MsgPrint("You failed to use the staff properly.");
                return;
            }
            // Make sure it has charges left
            if (item.TypeSpecificValue <= 0)
            {
                Profile.Instance.MsgPrint("The staff has no charges left.");
                item.IdentifyFlags.Set(Constants.IdentEmpty);
                return;
            }
            Gui.PlaySound(SoundEffect.UseStaff);
            int k;
            bool useCharge = true;
            // Do the specific effect for the type of staff
            switch (item.ItemSubCategory)
            {
                case StaffType.Darkness:
                    {
                        if (!player.HasBlindnessResistance && !player.HasDarkResistance)
                        {
                            if (player.SetTimedBlindness(player.TimedBlindness + 3 + Program.Rng.DieRoll(5)))
                            {
                                identified = true;
                            }
                        }
                        if (SaveGame.Instance.SpellEffects.UnlightArea(10, 3))
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.Slowness:
                    {
                        if (player.SetTimedSlow(player.TimedSlow + Program.Rng.DieRoll(30) + 15))
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.HasteMonsters:
                    {
                        if (SaveGame.Instance.SpellEffects.HasteMonsters())
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.Summoning:
                    {
                        for (k = 0; k < Program.Rng.DieRoll(4); k++)
                        {
                            if (level.Monsters.SummonSpecific(player.MapY, player.MapX, SaveGame.Instance.Difficulty, 0))
                            {
                                identified = true;
                            }
                        }
                        break;
                    }
                case StaffType.Teleportation:
                    {
                        SaveGame.Instance.SpellEffects.TeleportPlayer(100);
                        identified = true;
                        break;
                    }
                case StaffType.Identify:
                    {
                        if (!SaveGame.Instance.SpellEffects.IdentifyItem())
                        {
                            useCharge = false;
                        }
                        identified = true;
                        break;
                    }
                case StaffType.RemoveCurse:
                    {
                        if (SaveGame.Instance.SpellEffects.RemoveCurse())
                        {
                            if (player.TimedBlindness == 0)
                            {
                                Profile.Instance.MsgPrint("The staff glows blue for a moment...");
                            }
                            identified = true;
                        }
                        break;
                    }
                case StaffType.Starlight:
                    {
                        if (player.TimedBlindness == 0)
                        {
                            Profile.Instance.MsgPrint("The end of the staff glows brightly...");
                        }
                        for (k = 0; k < 8; k++)
                        {
                            SaveGame.Instance.SpellEffects.LightLine(level.OrderedDirection[k]);
                        }
                        identified = true;
                        break;
                    }
                case StaffType.Light:
                    {
                        if (SaveGame.Instance.SpellEffects.LightArea(Program.Rng.DiceRoll(2, 8), 2))
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.Mapping:
                    {
                        level.MapArea();
                        identified = true;
                        break;
                    }
                case StaffType.DetectGold:
                    {
                        if (SaveGame.Instance.SpellEffects.DetectTreasure())
                        {
                            identified = true;
                        }
                        if (SaveGame.Instance.SpellEffects.DetectObjectsGold())
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.DetectItem:
                    {
                        if (SaveGame.Instance.SpellEffects.DetectObjectsNormal())
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.DetectTrap:
                    {
                        if (SaveGame.Instance.SpellEffects.DetectTraps())
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.DetectDoor:
                    {
                        if (SaveGame.Instance.SpellEffects.DetectDoors())
                        {
                            identified = true;
                        }
                        if (SaveGame.Instance.SpellEffects.DetectStairs())
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.DetectInvis:
                    {
                        if (SaveGame.Instance.SpellEffects.DetectMonstersInvis())
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.DetectEvil:
                    {
                        if (SaveGame.Instance.SpellEffects.DetectMonstersEvil())
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.CureLight:
                    {
                        if (player.RestoreHealth(Program.Rng.DieRoll(8)))
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.Curing:
                    {
                        if (player.SetTimedBlindness(0))
                        {
                            identified = true;
                        }
                        if (player.SetTimedPoison(0))
                        {
                            identified = true;
                        }
                        if (player.SetTimedConfusion(0))
                        {
                            identified = true;
                        }
                        if (player.SetTimedStun(0))
                        {
                            identified = true;
                        }
                        if (player.SetTimedBleeding(0))
                        {
                            identified = true;
                        }
                        if (player.SetTimedHallucinations(0))
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.Healing:
                    {
                        if (player.RestoreHealth(300))
                        {
                            identified = true;
                        }
                        if (player.SetTimedStun(0))
                        {
                            identified = true;
                        }
                        if (player.SetTimedBleeding(0))
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.TheMagi:
                    {
                        if (player.TryRestoringAbilityScore(Ability.Intelligence))
                        {
                            identified = true;
                        }
                        if (player.Mana < player.MaxMana)
                        {
                            player.Mana = player.MaxMana;
                            player.FractionalMana = 0;
                            identified = true;
                            Profile.Instance.MsgPrint("Your feel your head clear.");
                            player.RedrawNeeded.Set(RedrawFlag.PrMana);
                        }
                        break;
                    }
                case StaffType.SleepMonsters:
                    {
                        if (SaveGame.Instance.SpellEffects.SleepMonsters())
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.SlowMonsters:
                    {
                        if (SaveGame.Instance.SpellEffects.SlowMonsters())
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.Speed:
                    {
                        if (player.TimedHaste == 0)
                        {
                            if (player.SetTimedHaste(Program.Rng.DieRoll(30) + 15))
                            {
                                identified = true;
                            }
                        }
                        else
                        {
                            player.SetTimedHaste(player.TimedHaste + 5);
                        }
                        break;
                    }
                case StaffType.Probing:
                    {
                        SaveGame.Instance.SpellEffects.Probing();
                        identified = true;
                        break;
                    }
                case StaffType.DispelEvil:
                    {
                        if (SaveGame.Instance.SpellEffects.DispelEvil(60))
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.Power:
                    {
                        if (SaveGame.Instance.SpellEffects.DispelMonsters(120))
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.Holiness:
                    {
                        if (SaveGame.Instance.SpellEffects.DispelEvil(120))
                        {
                            identified = true;
                        }
                        k = 3 * player.Level;
                        if (player.SetTimedProtectionFromEvil(player.TimedProtectionFromEvil + Program.Rng.DieRoll(25) + k))
                        {
                            identified = true;
                        }
                        if (player.SetTimedPoison(0))
                        {
                            identified = true;
                        }
                        if (player.SetTimedFear(0))
                        {
                            identified = true;
                        }
                        if (player.RestoreHealth(50))
                        {
                            identified = true;
                        }
                        if (player.SetTimedStun(0))
                        {
                            identified = true;
                        }
                        if (player.SetTimedBleeding(0))
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.Carnage:
                    {
                        SaveGame.Instance.SpellEffects.Carnage(true);
                        identified = true;
                        break;
                    }
                case StaffType.Earthquakes:
                    {
                        SaveGame.Instance.SpellEffects.Earthquake(player.MapY, player.MapX, 10);
                        identified = true;
                        break;
                    }
                case StaffType.Destruction:
                    {
                        SaveGame.Instance.SpellEffects.DestroyArea(player.MapY, player.MapX, 15);
                        identified = true;
                        break;
                    }
            }
            player.NoticeFlags |= Constants.PnCombine | Constants.PnReorder;
            // We might now know what the staff does
            item.ObjectTried();
            if (identified && !item.IsFlavourAware())
            {
                item.BecomeFlavourAware();
                player.GainExperience((itemLevel + (player.Level >> 1)) / player.Level);
            }
            // We may not have used up a charge
            if (!useCharge)
            {
                return;
            }
            // Channelers can use mana instead of a charge
            bool channeled = false;
            if (player.Spellcasting.Type == CastingType.Channeling)
            {
                channeled = SaveGame.Instance.CommandEngine.DoCmdChannel(item);
            }
            if (!channeled)
            {
                // Use the actual charge
                item.TypeSpecificValue--;
                // If the staff was part of a stack, separate it from the rest
                if (itemIndex >= 0 && item.Count > 1)
                {
                    Item singleStaff = new Item(item) { Count = 1 };
                    item.TypeSpecificValue++;
                    item.Count--;
                    player.WeightCarried -= singleStaff.Weight;
                    itemIndex = player.Inventory.InvenCarry(singleStaff, false);
                    Profile.Instance.MsgPrint("You unstack your staff.");
                }
                // Let the player know what happened
                if (itemIndex >= 0)
                {
                    player.Inventory.ReportChargeUsageFromInventory(itemIndex);
                }
                else
                {
                    SaveGame.Instance.Level.ReportChargeUsageFromFloor(0 - itemIndex);
                }
            }
        }

    }
}
