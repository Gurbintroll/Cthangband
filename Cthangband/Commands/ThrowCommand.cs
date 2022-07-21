using Cthangband.Enumerations;
using Cthangband.StaticData;
using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Throw an item
    /// </summary>
    [Serializable]
    internal class ThrowCommand : ICommand
    {
        public char Key => 'v';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            DoCmdThrow(player, level, 1);
        }

        public static void DoCmdThrow(Player player, Level level, int damageMultiplier)
        {
            // Get an item to throw
            if (!SaveGame.Instance.GetItem(out int itemIndex, "Throw which item? ", false, true, true))
            {
                if (itemIndex == -2)
                {
                    Profile.Instance.MsgPrint("You have nothing to throw.");
                }
                return;
            }
            Item item = itemIndex >= 0 ? player.Inventory[itemIndex] : level.Items[0 - itemIndex];
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            // Copy a single item from the item stack as the thrown item
            Item missile = new Item(item) { Count = 1 };
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
            string missileName = missile.Description(false, 3);
            Colour missileColour = missile.ItemType.Colour;
            char missileCharacter = missile.ItemType.Character;
            // Thrown distance is based on the weight of the missile
            int multiplier = 10 + (2 * (damageMultiplier - 1));
            int divider = missile.Weight > 10 ? missile.Weight : 10;
            int throwDistance = (player.AbilityScores[Ability.Strength].StrAttackSpeedComponent + 20) * multiplier / divider;
            if (throwDistance > 10)
            {
                throwDistance = 10;
            }
            // Work out the damage done
            int damage = Program.Rng.DiceRoll(missile.DamageDice, missile.DamageDiceSides) + missile.BonusDamage;
            damage *= damageMultiplier;
            int chance = player.SkillThrowing + (player.AttackBonus * Constants.BthPlusAdj);
            // Throwing something always uses a full turn, even if you can make multiple missile attacks
            SaveGame.Instance.EnergyUse = 100;
            int y = player.MapY;
            int x = player.MapX;
            int targetX = player.MapX + (99 * level.KeypadDirectionXOffset[dir]);
            int targetY = player.MapY + (99 * level.KeypadDirectionYOffset[dir]);
            if (dir == 5 && targetEngine.TargetOkay())
            {
                targetX = SaveGame.Instance.TargetCol;
                targetY = SaveGame.Instance.TargetRow;
            }
            SaveGame.Instance.HandleStuff();
            int newY = player.MapY;
            int newX = player.MapX;
            bool hitBody = false;
            // Send the thrown object in the right direction one square at a time
            for (int curDis = 0; curDis <= throwDistance;)
            {
                // If we reach our limit, stop the object moving
                if (y == targetY && x == targetX)
                {
                    break;
                }
                level.MoveOneStepTowards(out newY, out newX, y, x, player.MapY, player.MapX, targetY, targetX);
                // If we hit a wall or something stop moving
                if (!level.GridPassable(newY, newX))
                {
                    break;
                }
                curDis++;
                x = newX;
                y = newY;
                const int msec = GlobalData.DelayFactor * GlobalData.DelayFactor * GlobalData.DelayFactor;
                // If we can see, display the thrown item with a suitable delay
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
                    // Delay even if we don't see it, so it doesn't look weird when it passes behind something
                    Gui.Pause(msec);
                }
                // If there's a monster in the way, we might hit it regardless of whether or not it
                // is our intended target
                if (level.Grid[y][x].MonsterIndex != 0)
                {
                    GridTile tile = level.Grid[y][x];
                    Monster monster = level.Monsters[tile.MonsterIndex];
                    MonsterRace race = monster.Race;
                    bool visible = monster.IsVisible;
                    hitBody = true;
                    // See if it actually hit the monster
                    if (SaveGame.Instance.CommandEngine.PlayerCheckRangedHitOnMonster(chance - curDis, race.ArmourClass, monster.IsVisible))
                    {
                        string noteDies = " dies.";
                        if ((race.Flags3 & MonsterFlag3.Demon) != 0 || (race.Flags3 & MonsterFlag3.Undead) != 0 ||
                            (race.Flags3 & MonsterFlag3.Cthuloid) != 0 || (race.Flags2 & MonsterFlag2.Stupid) != 0 ||
                            "Evg".Contains(race.Character.ToString()))
                        {
                            noteDies = " is destroyed.";
                        }
                        // Let the player know what happened
                        if (!visible)
                        {
                            Profile.Instance.MsgPrint($"The {missileName} finds a mark.");
                        }
                        else
                        {
                            string mName = monster.MonsterDesc(0);
                            Profile.Instance.MsgPrint($"The {missileName} hits {mName}.");
                            if (monster.IsVisible)
                            {
                                SaveGame.Instance.HealthTrack(tile.MonsterIndex);
                            }
                        }
                        // Adjust the damage for the particular monster type
                        damage = missile.AdjustDamageForMonsterType(damage, monster);
                        damage = SaveGame.Instance.CommandEngine.PlayerCriticalRanged(missile.Weight, missile.BonusToHit, damage);
                        if (damage < 0)
                        {
                            damage = 0;
                        }
                        if (level.Monsters.DamageMonster(tile.MonsterIndex, damage, out bool fear, noteDies))
                        {
                            // The monster is dead, so don't add further statuses or messages
                        }
                        else
                        {
                            // Let the player know what happens to the monster
                            level.Monsters.MessagePain(tile.MonsterIndex, damage);
                            if ((monster.Mind & Constants.SmFriendly) != 0 &&
                                missile.ItemType.Category != ItemCategory.Potion)
                            {
                                string mName = monster.MonsterDesc(0);
                                Profile.Instance.MsgPrint($"{mName} gets angry!");
                                monster.Mind &= ~Constants.SmFriendly;
                            }
                            if (fear && monster.IsVisible)
                            {
                                Gui.PlaySound(SoundEffect.MonsterFlees);
                                string mName = monster.MonsterDesc(0);
                                Profile.Instance.MsgPrint($"{mName} flees in terror!");
                            }
                        }
                    }
                    break;
                }
            }
            // There's a chance of breakage if we hit a creature
            int chanceToBreak = hitBody ? missile.BreakageChance() : 0;
            // If we hit with a potion, the potion might affect the creature
            if (missile.ItemType.Category == ItemCategory.Potion)
            {
                if (hitBody || !level.GridPassable(newY, newX) || Program.Rng.DieRoll(100) < chanceToBreak)
                {
                    Profile.Instance.MsgPrint($"The {missileName} shatters!");
                    if (SaveGame.Instance.SpellEffects.PotionSmashEffect(1, y, x, missile.ItemSubCategory))
                    {
                        if (level.Grid[y][x].MonsterIndex != 0 &&
                            (level.Monsters[level.Grid[y][x].MonsterIndex].Mind & Constants.SmFriendly) != 0)
                        {
                            string mName = level.Monsters[level.Grid[y][x].MonsterIndex].MonsterDesc(0);
                            Profile.Instance.MsgPrint($"{mName} gets angry!");
                            level.Monsters[level.Grid[y][x].MonsterIndex].Mind &= ~Constants.SmFriendly;
                        }
                    }
                    return;
                }
                chanceToBreak = 0;
            }
            // Drop the item on the floor
            SaveGame.Instance.Level.DropNear(missile, chanceToBreak, y, x);
        }
    }
}
