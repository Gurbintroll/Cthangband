using Cthangband.Enumerations;
using Cthangband.Projection;
using Cthangband.StaticData;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Read a scroll from the inventory or floor
    /// </summary>
    /// <param name="itemIndex"> The inventory index of the scroll to be read </param>
    [Serializable]
    internal class ReadScrollCommand : ICommand
    {
        public char Key => 'r';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            int itemIndex = -999;

            int i;
            // Make sure we're in a situation where we can read
            if (player.TimedBlindness != 0)
            {
                Profile.Instance.MsgPrint("You can't see anything.");
                return;
            }
            if (level.NoLight())
            {
                Profile.Instance.MsgPrint("You have no light to read by.");
                return;
            }
            if (player.TimedConfusion != 0)
            {
                Profile.Instance.MsgPrint("You are too confused!");
                return;
            }
            // If we weren't passed in an item, prompt for one
            Inventory.ItemFilterCategory = ItemCategory.Scroll;
            if (itemIndex == -999)
            {
                if (!SaveGame.Instance.GetItem(out itemIndex, "Read which scroll? ", true, true, true))
                {
                    if (itemIndex == -2)
                    {
                        Profile.Instance.MsgPrint("You have no scrolls to read.");
                    }
                    return;
                }
            }
            Item item = itemIndex >= 0 ? player.Inventory[itemIndex] : level.Items[0 - itemIndex];
            // Make sure the item is actually a scroll
            Inventory.ItemFilterCategory = ItemCategory.Scroll;
            if (!player.Inventory.ItemMatchesFilter(item))
            {
                Profile.Instance.MsgPrint("That is not a scroll!");
                Inventory.ItemFilterCategory = 0;
                return;
            }
            Inventory.ItemFilterCategory = 0;
            // Scrolls use a full turn
            SaveGame.Instance.EnergyUse = 100;
            bool identified = false;
            int itemLevel = item.ItemType.Level;
            bool usedUp = true;
            // Most types of scroll are obvious
            switch (item.ItemSubCategory)
            {
                case ScrollType.Darkness:
                    {
                        if (!player.HasBlindnessResistance && !player.HasDarkResistance)
                        {
                            player.SetTimedBlindness(player.TimedBlindness + 3 + Program.Rng.DieRoll(5));
                        }
                        if (SaveGame.Instance.SpellEffects.UnlightArea(10, 3))
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.AggravateMonster:
                    {
                        Profile.Instance.MsgPrint("There is a high pitched humming noise.");
                        SaveGame.Instance.SpellEffects.AggravateMonsters(1);
                        identified = true;
                        break;
                    }
                case ScrollType.CurseArmour:
                    {
                        if (SaveGame.Instance.CommandEngine.CurseArmour())
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.CurseWeapon:
                    {
                        if (SaveGame.Instance.CommandEngine.CurseWeapon())
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.SummonMonster:
                    {
                        for (i = 0; i < Program.Rng.DieRoll(3); i++)
                        {
                            if (level.Monsters.SummonSpecific(player.MapY, player.MapX, SaveGame.Instance.Difficulty, 0))
                            {
                                identified = true;
                            }
                        }
                        break;
                    }
                case ScrollType.SummonUndead:
                    {
                        for (i = 0; i < Program.Rng.DieRoll(3); i++)
                        {
                            if (level.Monsters.SummonSpecific(player.MapY, player.MapX, SaveGame.Instance.Difficulty,
                                Constants.SummonUndead))
                            {
                                identified = true;
                            }
                        }
                        break;
                    }
                case ScrollType.TrapCreation:
                    {
                        if (SaveGame.Instance.SpellEffects.TrapCreation())
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.PhaseDoor:
                    {
                        SaveGame.Instance.SpellEffects.TeleportPlayer(10);
                        identified = true;
                        break;
                    }
                case ScrollType.Teleport:
                    {
                        SaveGame.Instance.SpellEffects.TeleportPlayer(100);
                        identified = true;
                        break;
                    }
                case ScrollType.TeleportLevel:
                    {
                        SaveGame.Instance.SpellEffects.TeleportPlayerLevel();
                        identified = true;
                        break;
                    }
                case ScrollType.WordOfRecall:
                    {
                        player.ToggleRecall();
                        identified = true;
                        break;
                    }
                case ScrollType.Identify:
                    {
                        identified = true;
                        if (!SaveGame.Instance.SpellEffects.IdentifyItem())
                        {
                            usedUp = false;
                        }
                        break;
                    }
                case ScrollType.StarIdentify:
                    {
                        identified = true;
                        if (!SaveGame.Instance.SpellEffects.IdentifyFully())
                        {
                            usedUp = false;
                        }
                        break;
                    }
                case ScrollType.RemoveCurse:
                    {
                        if (SaveGame.Instance.SpellEffects.RemoveCurse())
                        {
                            Profile.Instance.MsgPrint("You feel as if someone is watching over you.");
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.StarRemoveCurse:
                    {
                        SaveGame.Instance.SpellEffects.RemoveAllCurse();
                        identified = true;
                        break;
                    }
                case ScrollType.EnchantArmor:
                    {
                        identified = true;
                        if (!SaveGame.Instance.SpellEffects.EnchantSpell(0, 0, 1))
                        {
                            usedUp = false;
                        }
                        break;
                    }
                case ScrollType.EnchantWeaponToHit:
                    {
                        if (!SaveGame.Instance.SpellEffects.EnchantSpell(1, 0, 0))
                        {
                            usedUp = false;
                        }
                        identified = true;
                        break;
                    }
                case ScrollType.EnchantWeaponToDam:
                    {
                        if (!SaveGame.Instance.SpellEffects.EnchantSpell(0, 1, 0))
                        {
                            usedUp = false;
                        }
                        identified = true;
                        break;
                    }
                case ScrollType.StarEnchantArmor:
                    {
                        if (!SaveGame.Instance.SpellEffects.EnchantSpell(0, 0, Program.Rng.DieRoll(3) + 2))
                        {
                            usedUp = false;
                        }
                        identified = true;
                        break;
                    }
                case ScrollType.StarEnchantWeapon:
                    {
                        if (!SaveGame.Instance.SpellEffects.EnchantSpell(Program.Rng.DieRoll(3), Program.Rng.DieRoll(3), 0))
                        {
                            usedUp = false;
                        }
                        identified = true;
                        break;
                    }
                case ScrollType.Recharging:
                    {
                        if (!SaveGame.Instance.SpellEffects.Recharge(60))
                        {
                            usedUp = false;
                        }
                        identified = true;
                        break;
                    }
                case ScrollType.Light:
                    {
                        if (SaveGame.Instance.SpellEffects.LightArea(Program.Rng.DiceRoll(2, 8), 2))
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.Mapping:
                    {
                        level.MapArea();
                        identified = true;
                        break;
                    }
                case ScrollType.DetectGold:
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
                case ScrollType.DetectItem:
                    {
                        if (SaveGame.Instance.SpellEffects.DetectObjectsNormal())
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.DetectTrap:
                    {
                        if (SaveGame.Instance.SpellEffects.DetectTraps())
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.DetectDoor:
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
                case ScrollType.DetectInvis:
                    {
                        if (SaveGame.Instance.SpellEffects.DetectMonstersInvis())
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.SatisfyHunger:
                    {
                        if (player.SetFood(Constants.PyFoodMax - 1))
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.Blessing:
                    {
                        if (player.SetTimedBlessing(player.TimedBlessing + Program.Rng.DieRoll(12) + 6))
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.HolyChant:
                    {
                        if (player.SetTimedBlessing(player.TimedBlessing + Program.Rng.DieRoll(24) + 12))
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.HolyPrayer:
                    {
                        if (player.SetTimedBlessing(player.TimedBlessing + Program.Rng.DieRoll(48) + 24))
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.MonsterConfusion:
                    {
                        if (!player.HasConfusingTouch)
                        {
                            Profile.Instance.MsgPrint("Your hands begin to glow.");
                            player.HasConfusingTouch = true;
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.ProtectionFromEvil:
                    {
                        i = 3 * player.Level;
                        if (player.SetTimedProtectionFromEvil(player.TimedProtectionFromEvil + Program.Rng.DieRoll(25) + i))
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.RuneOfProtection:
                    {
                        SaveGame.Instance.SpellEffects.ElderSign();
                        identified = true;
                        break;
                    }
                case ScrollType.TrapDoorDestruction:
                    {
                        if (SaveGame.Instance.SpellEffects.DestroyDoorsTouch())
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.StarDestruction:
                    {
                        SaveGame.Instance.SpellEffects.DestroyArea(player.MapY, player.MapX, 15);
                        identified = true;
                        break;
                    }
                case ScrollType.DispelUndead:
                    {
                        if (SaveGame.Instance.SpellEffects.DispelUndead(60))
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.Carnage:
                    {
                        SaveGame.Instance.SpellEffects.Carnage(true);
                        identified = true;
                        break;
                    }
                case ScrollType.MassCarnage:
                    {
                        SaveGame.Instance.SpellEffects.MassCarnage(true);
                        identified = true;
                        break;
                    }
                case ScrollType.Acquirement:
                    {
                        SaveGame.Instance.Level.Acquirement(player.MapY, player.MapX, 1, true);
                        identified = true;
                        break;
                    }
                case ScrollType.StarAcquirement:
                    {
                        SaveGame.Instance.Level.Acquirement(player.MapY, player.MapX, Program.Rng.DieRoll(2) + 1, true);
                        identified = true;
                        break;
                    }
                case ScrollType.Fire:
                    {
                        SaveGame.Instance.SpellEffects.FireBall(new ProjectFire(SaveGame.Instance.SpellEffects), 0, 150, 4);
                        if (!(player.TimedFireResistance != 0 || player.HasFireResistance || player.HasFireImmunity))
                        {
                            player.TakeHit(50 + Program.Rng.DieRoll(50), "a Scroll of Fire");
                        }
                        identified = true;
                        break;
                    }
                case ScrollType.Ice:
                    {
                        SaveGame.Instance.SpellEffects.FireBall(new ProjectIce(SaveGame.Instance.SpellEffects), 0, 175, 4);
                        if (!(player.TimedColdResistance != 0 || player.HasColdResistance || player.HasColdImmunity))
                        {
                            player.TakeHit(100 + Program.Rng.DieRoll(100), "a Scroll of Ice");
                        }
                        identified = true;
                        break;
                    }
                case ScrollType.Chaos:
                    {
                        SaveGame.Instance.SpellEffects.FireBall(new ProjectChaos(SaveGame.Instance.SpellEffects), 0, 222, 4);
                        if (!player.HasChaosResistance)
                        {
                            player.TakeHit(111 + Program.Rng.DieRoll(111), "a Scroll of Chaos");
                        }
                        identified = true;
                        break;
                    }
                case ScrollType.Rumor:
                    {
                        Profile.Instance.MsgPrint("There is message on the scroll. It says:");
                        Profile.Instance.MsgPrint(null);
                        SaveGame.Instance.CommandEngine.GetRumour();
                        identified = true;
                        break;
                    }
                case ScrollType.Invocation:
                    {
                        var patron = SaveGame.Instance.PatronList[Program.Rng.DieRoll(SaveGame.Instance.PatronList.Length) - 1];
                        Profile.Instance.MsgPrint($"You invoke the secret name of {patron.LongName}.");
                        patron.GetReward(player, SaveGame.Instance.Level, SaveGame.Instance);
                        identified = true;
                        break;
                    }
                case ScrollType.Artifact:
                    {
                        SaveGame.Instance.SpellEffects.ArtifactScroll();
                        identified = true;
                        break;
                    }
            }
            player.NoticeFlags |= Constants.PnCombine | Constants.PnReorder;
            // We might have just identified the scroll
            item.ObjectTried();
            if (identified && !item.IsFlavourAware())
            {
                item.BecomeFlavourAware();
                player.GainExperience((itemLevel + (player.Level >> 1)) / player.Level);
            }
            bool channeled = false;
            // Channelers can use mana instead of the scroll being used up
            if (player.Spellcasting.Type == CastingType.Channeling)
            {
                channeled = SaveGame.Instance.CommandEngine.DoCmdChannel(item);
            }
            if (!channeled)
            {
                if (!usedUp)
                {
                    return;
                }
                // If it wasn't used up then decrease the amount in the stack
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
