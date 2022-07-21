using Cthangband.Enumerations;
using Cthangband.Projection;
using Cthangband.StaticData;
using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Use a rod from the inventory or ground
    /// </summary>
    /// <param name="itemIndex"> The inventory index of the item to be used </param>
    [Serializable]
    internal class ZapRodCommand : ICommand
    {
        public char Key => 'z';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            int itemIndex = -999;

            // Get the item if we weren't passed it
            Inventory.ItemFilterCategory = ItemCategory.Rod;
            if (itemIndex == -999)
            {
                if (!SaveGame.Instance.GetItem(out itemIndex, "Zap which rod? ", false, true, true))
                {
                    if (itemIndex == -2)
                    {
                        Profile.Instance.MsgPrint("You have no rod to zap.");
                    }
                    return;
                }
            }
            Item item = itemIndex >= 0 ? player.Inventory[itemIndex] : level.Items[0 - itemIndex];
            // Make sure the item is actually a rod
            Inventory.ItemFilterCategory = ItemCategory.Rod;
            if (!player.Inventory.ItemMatchesFilter(item))
            {
                Profile.Instance.MsgPrint("That is not a rod!");
                Inventory.ItemFilterCategory = 0;
                return;
            }
            Inventory.ItemFilterCategory = 0;
            // Rods can't be used from the floor
            if (itemIndex < 0 && item.Count > 1)
            {
                Profile.Instance.MsgPrint("You must first pick up the rods.");
                return;
            }
            // We may need to aim the rod
            int dir = 5;
            if ((item.ItemSubCategory >= RodType.MinimumAimed && item.ItemSubCategory != RodType.Havoc) ||
                !item.IsFlavourAware())
            {
                TargetEngine targetEngine = new TargetEngine(player, level);
                if (!targetEngine.GetDirectionWithAim(out dir))
                {
                    return;
                }
            }
            // Using a rod takes a whole turn
            SaveGame.Instance.EnergyUse = 100;
            bool identified = false;
            int itemLevel = item.ItemType.Level;
            // Chance to successfully use it is skill (halved if confused) - rod level (capped at 50)
            int chance = player.SkillUseDevice;
            if (player.TimedConfusion != 0)
            {
                chance /= 2;
            }
            chance -= itemLevel > 50 ? 50 : itemLevel;
            // There's always a small chance of success
            if (chance < Constants.UseDevice && Program.Rng.RandomLessThan(Constants.UseDevice - chance + 1) == 0)
            {
                chance = Constants.UseDevice;
            }
            // Do the actual check
            if (chance < Constants.UseDevice || Program.Rng.DieRoll(chance) < Constants.UseDevice)
            {
                Profile.Instance.MsgPrint("You failed to use the rod properly.");
                return;
            }
            // Rods only have a single charge but recharge over time
            if (item.TypeSpecificValue != 0)
            {
                Profile.Instance.MsgPrint("The rod is still charging.");
                return;
            }
            Gui.PlaySound(SoundEffect.ZapRod);
            // Do the rod-specific effect
            bool useCharge = true;
            switch (item.ItemSubCategory)
            {
                case RodType.DetectTrap:
                    {
                        if (SaveGame.Instance.SpellEffects.DetectTraps())
                        {
                            identified = true;
                        }
                        item.TypeSpecificValue = 10 + Program.Rng.DieRoll(10);
                        break;
                    }
                case RodType.DetectDoor:
                    {
                        if (SaveGame.Instance.SpellEffects.DetectDoors())
                        {
                            identified = true;
                        }
                        if (SaveGame.Instance.SpellEffects.DetectStairs())
                        {
                            identified = true;
                        }
                        item.TypeSpecificValue = 70;
                        break;
                    }
                case RodType.Identify:
                    {
                        identified = true;
                        if (!SaveGame.Instance.SpellEffects.IdentifyItem())
                        {
                            useCharge = false;
                        }
                        item.TypeSpecificValue = 10;
                        break;
                    }
                case RodType.Recall:
                    {
                        player.ToggleRecall();
                        identified = true;
                        item.TypeSpecificValue = 60;
                        break;
                    }
                case RodType.Illumination:
                    {
                        if (SaveGame.Instance.SpellEffects.LightArea(Program.Rng.DiceRoll(2, 8), 2))
                        {
                            identified = true;
                        }
                        item.TypeSpecificValue = 10 + Program.Rng.DieRoll(11);
                        break;
                    }
                case RodType.Mapping:
                    {
                        level.MapArea();
                        identified = true;
                        item.TypeSpecificValue = 99;
                        break;
                    }
                case RodType.Detection:
                    {
                        SaveGame.Instance.SpellEffects.DetectAll();
                        identified = true;
                        item.TypeSpecificValue = 99;
                        break;
                    }
                case RodType.Probing:
                    {
                        SaveGame.Instance.SpellEffects.Probing();
                        identified = true;
                        item.TypeSpecificValue = 50;
                        break;
                    }
                case RodType.Curing:
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
                        item.TypeSpecificValue = 999;
                        break;
                    }
                case RodType.Healing:
                    {
                        if (player.RestoreHealth(500))
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
                        item.TypeSpecificValue = 999;
                        break;
                    }
                case RodType.Restoration:
                    {
                        if (player.RestoreLevel())
                        {
                            identified = true;
                        }
                        if (player.TryRestoringAbilityScore(Ability.Strength))
                        {
                            identified = true;
                        }
                        if (player.TryRestoringAbilityScore(Ability.Intelligence))
                        {
                            identified = true;
                        }
                        if (player.TryRestoringAbilityScore(Ability.Wisdom))
                        {
                            identified = true;
                        }
                        if (player.TryRestoringAbilityScore(Ability.Dexterity))
                        {
                            identified = true;
                        }
                        if (player.TryRestoringAbilityScore(Ability.Constitution))
                        {
                            identified = true;
                        }
                        if (player.TryRestoringAbilityScore(Ability.Charisma))
                        {
                            identified = true;
                        }
                        item.TypeSpecificValue = 999;
                        break;
                    }
                case RodType.Speed:
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
                        item.TypeSpecificValue = 99;
                        break;
                    }
                case RodType.TeleportAway:
                    {
                        if (SaveGame.Instance.SpellEffects.TeleportMonster(dir))
                        {
                            identified = true;
                        }
                        item.TypeSpecificValue = 25;
                        break;
                    }
                case RodType.Disarming:
                    {
                        if (SaveGame.Instance.SpellEffects.DisarmTrap(dir))
                        {
                            identified = true;
                        }
                        item.TypeSpecificValue = 15 + Program.Rng.DieRoll(15);
                        break;
                    }
                case RodType.Light:
                    {
                        Profile.Instance.MsgPrint("A line of blue shimmering light appears.");
                        SaveGame.Instance.SpellEffects.LightLine(dir);
                        identified = true;
                        item.TypeSpecificValue = 9;
                        break;
                    }
                case RodType.SleepMonster:
                    {
                        if (SaveGame.Instance.SpellEffects.SleepMonster(dir))
                        {
                            identified = true;
                        }
                        item.TypeSpecificValue = 18;
                        break;
                    }
                case RodType.SlowMonster:
                    {
                        if (SaveGame.Instance.SpellEffects.SlowMonster(dir))
                        {
                            identified = true;
                        }
                        item.TypeSpecificValue = 20;
                        break;
                    }
                case RodType.DrainLife:
                    {
                        if (SaveGame.Instance.SpellEffects.DrainLife(dir, 75))
                        {
                            identified = true;
                        }
                        item.TypeSpecificValue = 23;
                        break;
                    }
                case RodType.Polymorph:
                    {
                        if (SaveGame.Instance.SpellEffects.PolyMonster(dir))
                        {
                            identified = true;
                        }
                        item.TypeSpecificValue = 25;
                        break;
                    }
                case RodType.AcidBolt:
                    {
                        SaveGame.Instance.SpellEffects.FireBoltOrBeam(10, new ProjectAcid(SaveGame.Instance.SpellEffects), dir,
                            Program.Rng.DiceRoll(6, 8));
                        identified = true;
                        item.TypeSpecificValue = 12;
                        break;
                    }
                case RodType.ElecBolt:
                    {
                        SaveGame.Instance.SpellEffects.FireBoltOrBeam(10, new ProjectElec(SaveGame.Instance.SpellEffects), dir,
                            Program.Rng.DiceRoll(3, 8));
                        identified = true;
                        item.TypeSpecificValue = 11;
                        break;
                    }
                case RodType.FireBolt:
                    {
                        SaveGame.Instance.SpellEffects.FireBoltOrBeam(10, new ProjectFire(SaveGame.Instance.SpellEffects), dir,
                            Program.Rng.DiceRoll(8, 8));
                        identified = true;
                        item.TypeSpecificValue = 15;
                        break;
                    }
                case RodType.ColdBolt:
                    {
                        SaveGame.Instance.SpellEffects.FireBoltOrBeam(10, new ProjectCold(SaveGame.Instance.SpellEffects), dir,
                            Program.Rng.DiceRoll(5, 8));
                        identified = true;
                        item.TypeSpecificValue = 13;
                        break;
                    }
                case RodType.AcidBall:
                    {
                        SaveGame.Instance.SpellEffects.FireBall(new ProjectAcid(SaveGame.Instance.SpellEffects), dir, 60, 2);
                        identified = true;
                        item.TypeSpecificValue = 27;
                        break;
                    }
                case RodType.ElecBall:
                    {
                        SaveGame.Instance.SpellEffects.FireBall(new ProjectElec(SaveGame.Instance.SpellEffects), dir, 32, 2);
                        identified = true;
                        item.TypeSpecificValue = 23;
                        break;
                    }
                case RodType.FireBall:
                    {
                        SaveGame.Instance.SpellEffects.FireBall(new ProjectFire(SaveGame.Instance.SpellEffects), dir, 72, 2);
                        identified = true;
                        item.TypeSpecificValue = 30;
                        break;
                    }
                case RodType.ColdBall:
                    {
                        SaveGame.Instance.SpellEffects.FireBall(new ProjectCold(SaveGame.Instance.SpellEffects), dir, 48, 2);
                        identified = true;
                        item.TypeSpecificValue = 25;
                        break;
                    }
                case RodType.Havoc:
                    {
                        SaveGame.Instance.SpellEffects.CallChaos();
                        identified = true;
                        item.TypeSpecificValue = 250;
                        break;
                    }
            }
            player.NoticeFlags |= Constants.PnCombine | Constants.PnReorder;
            // We may have just discovered what the rod does
            item.ObjectTried();
            if (identified && !item.IsFlavourAware())
            {
                item.BecomeFlavourAware();
                player.GainExperience((itemLevel + (player.Level >> 1)) / player.Level);
            }
            // We may not have actually used a charge
            if (!useCharge)
            {
                item.TypeSpecificValue = 0;
                return;
            }
            // Channelers can spend mana instead of a charge
            bool channeled = false;
            if (player.Spellcasting.Type == CastingType.Channeling)
            {
                channeled = SaveGame.Instance.CommandEngine.DoCmdChannel(item);
                if (channeled)
                {
                    item.TypeSpecificValue = 0;
                }
            }
            if (!channeled)
            {
                // If the rod was part of a stack, remove it
                if (itemIndex >= 0 && item.Count > 1)
                {
                    Item singleRod = new Item(item) { Count = 1 };
                    item.TypeSpecificValue = 0;
                    item.Count--;
                    player.WeightCarried -= singleRod.Weight;
                    player.Inventory.InvenCarry(singleRod, false);
                    Profile.Instance.MsgPrint("You unstack your rod.");
                }
            }
        }

    }
}
