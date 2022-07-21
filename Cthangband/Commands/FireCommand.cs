using Cthangband.Enumerations;
using Cthangband.StaticData;
using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Fire the missile weapon we have in our hand
    /// </summary>
    [Serializable]
    internal class FireCommand : ICommand
    {
        public char Key => 'f';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            // Check that we're actually wielding a ranged weapon
            Item missileWeapon = player.Inventory[InventorySlot.RangedWeapon];
            if (missileWeapon.Category == 0)
            {
                Profile.Instance.MsgPrint("You have nothing to fire with.");
                return;
            }
            // Get the ammunition to fire
            Inventory.ItemFilterCategory = player.AmmunitionItemCategory;
            if (!SaveGame.Instance.GetItem(out int itemIndex, "Fire which item? ", false, true, true))
            {
                if (itemIndex == -2)
                {
                    Profile.Instance.MsgPrint("You have nothing to fire.");
                }
                return;
            }
            Item ammunitionStack = itemIndex >= 0 ? player.Inventory[itemIndex] : level.Items[0 - itemIndex];
            TargetEngine targetEngine = new TargetEngine(player, level);
            // Find out where we're aiming at
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            // Copy an ammunition piece from the stack...
            Item individualAmmunition = new Item(ammunitionStack) { Count = 1 };
            // ...and reduced the amount in the stack
            if (itemIndex >= 0)
            {
                player.Inventory.InvenItemIncrease(itemIndex, -1);
                player.Inventory.InvenItemDescribe(itemIndex);
                player.Inventory.InvenItemOptimize(itemIndex);
            }
            else
            {
                SaveGame.Instance.Level.FloorItemIncrease(0 - itemIndex, -1);
                SaveGame.Instance.Level.FloorItemOptimize(0 - itemIndex);
            }
            Gui.PlaySound(SoundEffect.Shoot);
            // Get the details of the shot
            string missileName = individualAmmunition.Description(false, 3);
            Colour missileColour = individualAmmunition.ItemType.Colour;
            char missileCharacter = individualAmmunition.ItemType.Character;
            int shotSpeed = player.MissileAttacksPerRound;
            int shotDamage = Program.Rng.DiceRoll(individualAmmunition.DamageDice, individualAmmunition.DamageDiceSides) + individualAmmunition.BonusDamage +
                       missileWeapon.BonusDamage;
            int attackBonus = player.AttackBonus + individualAmmunition.BonusToHit + missileWeapon.BonusToHit;
            int chanceToHit = player.SkillRanged + (attackBonus * Constants.BthPlusAdj);
            // Damage multiplier depends on weapon
            int damageMultiplier = 1;
            switch (missileWeapon.ItemSubCategory)
            {
                case BowType.SvSling:
                    {
                        damageMultiplier = 2;
                        break;
                    }
                case BowType.SvShortBow:
                    {
                        damageMultiplier = 2;
                        break;
                    }
                case BowType.SvLongBow:
                    {
                        damageMultiplier = 3;
                        break;
                    }
                case BowType.SvLightXbow:
                    {
                        damageMultiplier = 3;
                        break;
                    }
                case BowType.SvHeavyXbow:
                    {
                        damageMultiplier = 4;
                        break;
                    }
            }
            // Extra might gives us an increased multiplier
            if (player.HasExtraMight)
            {
                damageMultiplier++;
            }
            shotDamage *= damageMultiplier;
            // We're actually going to track the shot and draw it square by square
            int shotDistance = 10 + (5 * damageMultiplier);
            // Divide by our shot speed to give the equivalent of x shots per turn
            SaveGame.Instance.EnergyUse = 100 / shotSpeed;
            int y = player.MapY;
            int x = player.MapX;
            int targetX = player.MapX + (99 * level.KeypadDirectionXOffset[dir]);
            int targetY = player.MapY + (99 * level.KeypadDirectionYOffset[dir]);
            // Special case for if we're hitting our own square
            if (dir == 5 && targetEngine.TargetOkay())
            {
                targetX = SaveGame.Instance.TargetCol;
                targetY = SaveGame.Instance.TargetRow;
            }
            SaveGame.Instance.HandleStuff();
            bool hitBody = false;
            // Loop until we've reached our distance or hit something
            for (int curDis = 0; curDis <= shotDistance;)
            {
                if (y == targetY && x == targetX)
                {
                    break;
                }
                // Move a step towards the target
                level.MoveOneStepTowards(out int newY, out int newX, y, x, player.MapY, player.MapX, targetY, targetX);
                // If we were blocked by a wall or something then stop short
                if (!level.GridPassable(newY, newX))
                {
                    break;
                }
                curDis++;
                x = newX;
                y = newY;
                int msec = GlobalData.DelayFactor * GlobalData.DelayFactor * GlobalData.DelayFactor;
                // If we can see the current projectile location, show it briefly
                if (level.PanelContains(y, x) && level.PlayerCanSeeBold(y, x))
                {
                    level.PrintCharacterAtMapLocation(missileCharacter, missileColour, y, x);
                    level.MoveCursorRelative(y, x);
                    Gui.Refresh();
                    Gui.Pause(msec);
                    level.RedrawSingleLocation(y, x);
                    Gui.Refresh();
                }
                else
                {
                    // Pause even if we can't see it so it doesn't look weird if it goes in and out
                    // of sight
                    Gui.Pause(msec);
                }
                // Check if we might hit a monster (not necessarily the one we were aiming at)
                if (level.Grid[y][x].MonsterIndex != 0)
                {
                    GridTile tile = level.Grid[y][x];
                    Monster monster = level.Monsters[tile.MonsterIndex];
                    MonsterRace race = monster.Race;
                    bool visible = monster.IsVisible;
                    hitBody = true;
                    // Check if we actually hit it
                    if (SaveGame.Instance.CommandEngine.PlayerCheckRangedHitOnMonster(chanceToHit - curDis, race.ArmourClass, monster.IsVisible))
                    {
                        string noteDies = " dies.";
                        if ((race.Flags3 & MonsterFlag3.Demon) != 0 || (race.Flags3 & MonsterFlag3.Undead) != 0 ||
                            (race.Flags3 & MonsterFlag3.Cthuloid) != 0 || (race.Flags2 & MonsterFlag2.Stupid) != 0 ||
                            "Evg".Contains(race.Character.ToString()))
                        {
                            noteDies = " is destroyed.";
                        }
                        if (!visible)
                        {
                            Profile.Instance.MsgPrint($"The {missileName} finds a mark.");
                        }
                        else
                        {
                            string monsterName = monster.MonsterDesc(0);
                            Profile.Instance.MsgPrint($"The {missileName} hits {monsterName}.");
                            if (monster.IsVisible)
                            {
                                SaveGame.Instance.HealthTrack(tile.MonsterIndex);
                            }
                            // Note that pets only get angry if they see us and we see them
                            if ((monster.Mind & Constants.SmFriendly) != 0)
                            {
                                monsterName = monster.MonsterDesc(0);
                                Profile.Instance.MsgPrint($"{monsterName} gets angry!");
                                monster.Mind &= ~Constants.SmFriendly;
                            }
                        }
                        // Work out the damage done
                        shotDamage = individualAmmunition.AdjustDamageForMonsterType(shotDamage, monster);
                        shotDamage = SaveGame.Instance.CommandEngine.PlayerCriticalRanged(individualAmmunition.Weight, individualAmmunition.BonusToHit, shotDamage);
                        if (shotDamage < 0)
                        {
                            shotDamage = 0;
                        }
                        if (level.Monsters.DamageMonster(tile.MonsterIndex, shotDamage, out bool fear, noteDies))
                        {
                            // The monster is dead, so don't add further statuses or messages
                        }
                        else
                        {
                            level.Monsters.MessagePain(tile.MonsterIndex, shotDamage);
                            if (fear && monster.IsVisible)
                            {
                                Gui.PlaySound(SoundEffect.MonsterFlees);
                                string mName = monster.MonsterDesc(0);
                                Profile.Instance.MsgPrint($"{mName} flees in terror!");
                            }
                        }
                    }
                    // Stop the ammo's travel since we hit something
                    break;
                }
            }
            // If we hit something we have a chance to break the ammo, otherwise it just drops at
            // the end of its travel
            int j = hitBody ? individualAmmunition.BreakageChance() : 0;
            SaveGame.Instance.Level.DropNear(individualAmmunition, j, y, x);
        }
    }
}
