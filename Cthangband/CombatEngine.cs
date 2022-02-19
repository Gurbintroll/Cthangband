using Cthangband.Enumerations;
using Cthangband.StaticData;
using Cthangband.UI;
using System;

namespace Cthangband
{
    /// <summary>
    /// A class to handle attacks
    /// </summary>
    [Serializable]
    internal class CombatEngine
    {
        private readonly SaveGame _saveGame;

        public CombatEngine(SaveGame saveGame)
        {
            _saveGame = saveGame;
        }

        /// <summary>
        /// Have a monster make an attack on the player
        /// </summary>
        /// <param name="monsterIndex"> The index of the monster making the attack </param>
        public void MonsterAttackPlayer(int monsterIndex)
        {
            Player player = _saveGame.Player;
            Level level = _saveGame.Level;
            Monster monster = level.Monsters[monsterIndex];
            MonsterRace race = monster.Race;
            int attackNumber;
            bool touched = false;
            bool fear = false;
            bool alive = true;
            // If the monster never attacks, then it shouldn't be attacking us now
            if ((race.Flags1 & MonsterFlag1.NeverAttack) != 0)
            {
                return;
            }
            // Friends don't hit friends
            if ((monster.Mind & Constants.SmFriendly) != 0)
            {
                return;
            }

            int armourClass = player.BaseArmourClass + player.ArmourClassBonus;
            int monsterLevel = race.Level >= 1 ? race.Level : 1;
            string monsterName = monster.MonsterDesc(0);
            string monsterDescription = monster.MonsterDesc(0x88);
            bool blinked = false;
            // Monsters get up to four attacks
            for (attackNumber = 0; attackNumber < 4; attackNumber++)
            {
                bool visible = false;
                bool obvious = false;
                int power = 0;
                int damage = 0;
                string act = null;
                AttackEffect effect = race.Attack[attackNumber].Effect;
                AttackType method = race.Attack[attackNumber].Method;
                int damageDice = race.Attack[attackNumber].DDice;
                int damageSides = race.Attack[attackNumber].DSide;
                // If the monster doesn't have an attack in this slot, stop looping
                if (method == AttackType.Nothing)
                {
                    break;
                }
                // Stop if player is dead or gone
                if (!alive || player.IsDead || SaveGame.Instance.NewLevelFlag)
                {
                    break;
                }
                if (monster.IsVisible)
                {
                    visible = true;
                }
                // Get the basic attack power from the attack type
                switch (effect)
                {
                    case AttackEffect.Hurt:
                        power = 60;
                        break;

                    case AttackEffect.Poison:
                        power = 5;
                        break;

                    case AttackEffect.UnBonus:
                        power = 20;
                        break;

                    case AttackEffect.UnPower:
                        power = 15;
                        break;

                    case AttackEffect.EatGold:
                        power = 5;
                        break;

                    case AttackEffect.EatItem:
                        power = 5;
                        break;

                    case AttackEffect.EatFood:
                        power = 5;
                        break;

                    case AttackEffect.EatLight:
                        power = 5;
                        break;

                    case AttackEffect.Acid:
                        power = 0;
                        break;

                    case AttackEffect.Electricity:
                        power = 10;
                        break;

                    case AttackEffect.Fire:
                        power = 10;
                        break;

                    case AttackEffect.Cold:
                        power = 10;
                        break;

                    case AttackEffect.Blind:
                        power = 2;
                        break;

                    case AttackEffect.Confuse:
                        power = 10;
                        break;

                    case AttackEffect.Terrify:
                        power = 10;
                        break;

                    case AttackEffect.Paralyze:
                        power = 2;
                        break;

                    case AttackEffect.LoseStr:
                        power = 0;
                        break;

                    case AttackEffect.LoseDex:
                        power = 0;
                        break;

                    case AttackEffect.LoseCon:
                        power = 0;
                        break;

                    case AttackEffect.LoseInt:
                        power = 0;
                        break;

                    case AttackEffect.LoseWis:
                        power = 0;
                        break;

                    case AttackEffect.LoseCha:
                        power = 0;
                        break;

                    case AttackEffect.LoseAll:
                        power = 2;
                        break;

                    case AttackEffect.Shatter:
                        power = 60;
                        break;

                    case AttackEffect.Exp10:
                        power = 5;
                        break;

                    case AttackEffect.Exp20:
                        power = 5;
                        break;

                    case AttackEffect.Exp40:
                        power = 5;
                        break;

                    case AttackEffect.Exp80:
                        power = 5;
                        break;
                }
                // Check if the monster actually hits us
                if (effect == 0 || MonsterCheckHitOnPlayer(power, monsterLevel))
                {
                    _saveGame.Disturb(true);
                    // Protection From Evil might repel the attack
                    if (player.TimedProtectionFromEvil > 0 && (race.Flags3 & MonsterFlag3.Evil) != 0 && player.Level >= monsterLevel &&
                        Program.Rng.RandomLessThan(100) + player.Level > 50)
                    {
                        if (monster.IsVisible)
                        {
                            // If it does, then they player knows the monster is evil
                            race.Knowledge.RFlags3 |= MonsterFlag3.Evil;
                        }
                        Profile.Instance.MsgPrint($"{monsterName} is repelled.");
                        continue;
                    }
                    bool doCut = false;
                    bool doStun = false;
                    // Give a description and remember the possible extras based on the attack method
                    switch (method)
                    {
                        case AttackType.Hit:
                            {
                                act = "hits you.";
                                doCut = true;
                                doStun = true;
                                touched = true;
                                break;
                            }
                        case AttackType.Touch:
                            {
                                act = "touches you.";
                                touched = true;
                                break;
                            }
                        case AttackType.Punch:
                            {
                                act = "punches you.";
                                touched = true;
                                doStun = true;
                                break;
                            }
                        case AttackType.Kick:
                            {
                                act = "kicks you.";
                                touched = true;
                                doStun = true;
                                break;
                            }
                        case AttackType.Claw:
                            {
                                act = "claws you.";
                                touched = true;
                                doCut = true;
                                break;
                            }
                        case AttackType.Bite:
                            {
                                act = "bites you.";
                                doCut = true;
                                touched = true;
                                break;
                            }
                        case AttackType.Sting:
                            {
                                act = "stings you.";
                                touched = true;
                                break;
                            }
                        case AttackType.Butt:
                            {
                                act = "butts you.";
                                doStun = true;
                                touched = true;
                                break;
                            }
                        case AttackType.Crush:
                            {
                                act = "crushes you.";
                                doStun = true;
                                touched = true;
                                break;
                            }
                        case AttackType.Engulf:
                            {
                                act = "engulfs you.";
                                touched = true;
                                break;
                            }
                        case AttackType.Charge:
                            {
                                act = "charges you.";
                                touched = true;
                                break;
                            }
                        case AttackType.Crawl:
                            {
                                act = "crawls on you.";
                                touched = true;
                                break;
                            }
                        case AttackType.Drool:
                            {
                                act = "drools on you.";
                                break;
                            }
                        case AttackType.Spit:
                            {
                                act = "spits on you.";
                                break;
                            }
                        case AttackType.Gaze:
                            {
                                act = "gazes at you.";
                                break;
                            }
                        case AttackType.Wail:
                            {
                                act = "wails at you.";
                                break;
                            }
                        case AttackType.Spore:
                            {
                                act = "releases spores at you.";
                                break;
                            }
                        case AttackType.Worship:
                            {
                                string[] worships = { "looks up at you!", "asks how many dragons you've killed!", "asks for your autograph!", "tries to shake your hand!", "pretends to be you!", "dances around you!", "tugs at your clothing!", "asks if you will adopt him!" };
                                act = worships[Program.Rng.RandomLessThan(8)];
                                break;
                            }
                        case AttackType.Beg:
                            {
                                act = "begs you for money.";
                                break;
                            }
                        case AttackType.Insult:
                            {
                                string[] insults = { "insults you!", "insults your mother!", "gives you the finger!", "humiliates you!", "defiles you!", "dances around you!", "makes obscene gestures!", "moons you!" };
                                act = insults[Program.Rng.RandomLessThan(8)];
                                break;
                            }
                        case AttackType.Moan:
                            {
                                string[] moans = { "seems sad about something.", "asks if you have seen his dogs.", "tells you to get off his land.", "mumbles something about mushrooms." };
                                act = moans[Program.Rng.RandomLessThan(4)];
                                break;
                            }
                        case AttackType.Show:
                            {
                                act = Program.Rng.DieRoll(3) == 1
                                    ? "sings 'We are a happy family.'"
                                    : "sings 'I love you, you love me.'";
                                break;
                            }
                    }
                    // Print the message
                    if (!string.IsNullOrEmpty(act))
                    {
                        Profile.Instance.MsgPrint($"{monsterName} {act}");
                    }
                    obvious = true;
                    // Work out base damage done by the attack
                    damage = Program.Rng.DiceRoll(damageDice, damageSides);
                    int i;
                    int k;
                    Item item;
                    string itemName;
                    // Apply any modifiers to the damage
                    switch (effect)
                    {
                        case 0:
                            {
                                obvious = true;
                                damage = 0;
                                break;
                            }
                        case AttackEffect.Hurt:
                            {
                                // Normal damage is reduced by armour
                                obvious = true;
                                damage -= damage * (armourClass < 150 ? armourClass : 150) / 250;
                                player.TakeHit(damage, monsterDescription);
                                break;
                            }
                        case AttackEffect.Poison:
                            {
                                // Poison does additional damage
                                player.TakeHit(damage, monsterDescription);
                                if (!(player.HasPoisonResistance || player.TimedPoisonResistance != 0))
                                {
                                    // Hagarg Ryonis might save us from the additional damage
                                    if (Program.Rng.DieRoll(10) <= player.Religion.GetNamedDeity(Pantheon.GodName.Hagarg_Ryonis).AdjustedFavour)
                                    {
                                        Profile.Instance.MsgPrint("Hagarg Ryonis's favour protects you!");
                                    }
                                    else if (player.SetTimedPoison(player.TimedPoison + Program.Rng.DieRoll(monsterLevel) + 5))
                                    {
                                        obvious = true;
                                    }
                                }
                                level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsPois);
                                break;
                            }
                        case AttackEffect.UnBonus:
                            {
                                // Disenchantment might ruin our items
                                player.TakeHit(damage, monsterDescription);
                                if (!player.HasDisenchantResistance)
                                {
                                    if (_saveGame.SpellEffects.ApplyDisenchant())
                                    {
                                        obvious = true;
                                    }
                                }
                                level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsDisen);
                                break;
                            }
                        case AttackEffect.UnPower:
                            {
                                // Unpower might drain charges from our items
                                player.TakeHit(damage, monsterDescription);
                                for (k = 0; k < 10; k++)
                                {
                                    i = Program.Rng.RandomLessThan(InventorySlot.Pack);
                                    item = player.Inventory[i];
                                    if (item.ItemType != null)
                                    {
                                        continue;
                                    }
                                    if ((item.Category == ItemCategory.Staff ||
                                         item.Category == ItemCategory.Wand) && item.TypeSpecificValue != 0)
                                    {
                                        Profile.Instance.MsgPrint("Energy drains from your pack!");
                                        obvious = true;
                                        int j = monsterLevel;
                                        monster.Health += j * item.TypeSpecificValue * item.Count;
                                        if (monster.Health > monster.MaxHealth)
                                        {
                                            monster.Health = monster.MaxHealth;
                                        }
                                        if (_saveGame.TrackedMonsterIndex == monsterIndex)
                                        {
                                            player.RedrawFlags |= RedrawFlag.PrHealth;
                                        }
                                        item.TypeSpecificValue = 0;
                                        player.NoticeFlags |= Constants.PnCombine | Constants.PnReorder;
                                        break;
                                    }
                                }
                                break;
                            }
                        case AttackEffect.EatGold:
                            {
                                // Steal some money
                                player.TakeHit(damage, monsterDescription);
                                obvious = true;
                                if ((player.TimedParalysis == 0 && Program.Rng.RandomLessThan(100) <
                                    player.AbilityScores[Ability.Dexterity].DexTheftAvoidance + player.Level) || player.HasAntiTheft)
                                {
                                    Profile.Instance.MsgPrint("You quickly protect your money pouch!");
                                    if (Program.Rng.RandomLessThan(3) != 0)
                                    {
                                        blinked = true;
                                    }
                                }
                                else
                                {
                                    // The amount of gold taken depends on how much you're carrying
                                    int gold = (player.Gold / 10) + Program.Rng.DieRoll(25);
                                    if (gold < 2)
                                    {
                                        gold = 2;
                                    }
                                    if (gold > 5000)
                                    {
                                        gold = (player.Gold / 20) + Program.Rng.DieRoll(3000);
                                    }
                                    if (gold > player.Gold)
                                    {
                                        gold = player.Gold;
                                    }
                                    player.Gold -= gold;
                                    // The monster gets the gold it stole, in case you kill it
                                    // before leaving the level
                                    monster.StolenGold += gold;
                                    // Inform the player what happened
                                    if (gold <= 0)
                                    {
                                        Profile.Instance.MsgPrint("Nothing was stolen.");
                                    }
                                    else if (player.Gold != 0)
                                    {
                                        Profile.Instance.MsgPrint("Your purse feels lighter.");
                                        Profile.Instance.MsgPrint($"{gold} coins were stolen!");
                                    }
                                    else
                                    {
                                        Profile.Instance.MsgPrint("Your purse feels lighter.");
                                        Profile.Instance.MsgPrint("All of your coins were stolen!");
                                    }
                                    player.RedrawFlags |= RedrawFlag.PrGold;
                                    blinked = true;
                                }
                                break;
                            }
                        case AttackEffect.EatItem:
                            {
                                // Steal an item
                                player.TakeHit(damage, monsterDescription);
                                if ((player.TimedParalysis == 0 && Program.Rng.RandomLessThan(100) <
                                    player.AbilityScores[Ability.Dexterity].DexTheftAvoidance + player.Level) || player.HasAntiTheft)
                                {
                                    Profile.Instance.MsgPrint("You grab hold of your backpack!");
                                    blinked = true;
                                    obvious = true;
                                    break;
                                }
                                // Have ten tries at picking a suitable item to steal
                                for (k = 0; k < 10; k++)
                                {
                                    i = Program.Rng.RandomLessThan(InventorySlot.Pack);
                                    item = player.Inventory[i];
                                    if (item.ItemType == null)
                                    {
                                        continue;
                                    }
                                    if (item.IsFixedArtifact() || !string.IsNullOrEmpty(item.RandartName))
                                    {
                                        continue;
                                    }
                                    itemName = item.Description(false, 3);
                                    string y = item.Count > 1 ? "One of y" : "Y";
                                    Profile.Instance.MsgPrint($"{y}our {itemName} ({i.IndexToLabel()}) was stolen!");
                                    int nextObjectIndex = _saveGame.Level.OPop();
                                    if (nextObjectIndex != 0)
                                    {
                                        // Give the item to the thief so it can later drop it
                                        Item stolenItem = new Item(item);
                                        level.Items[nextObjectIndex] = stolenItem;
                                        stolenItem.Count = 1;
                                        stolenItem.Marked = false;
                                        stolenItem.HoldingMonsterIndex = monsterIndex;
                                        stolenItem.NextInStack = monster.FirstHeldItemIndex;
                                        monster.FirstHeldItemIndex = nextObjectIndex;
                                    }
                                    player.Inventory.InvenItemIncrease(i, -1);
                                    player.Inventory.InvenItemOptimize(i);
                                    obvious = true;
                                    blinked = true;
                                    break;
                                }
                                break;
                            }
                        case AttackEffect.EatFood:
                            {
                                player.TakeHit(damage, monsterDescription);
                                // Have ten tries at grabbing a food item from the player
                                for (k = 0; k < 10; k++)
                                {
                                    i = Program.Rng.RandomLessThan(InventorySlot.Pack);
                                    item = player.Inventory[i];
                                    if (item.ItemType != null)
                                    {
                                        continue;
                                    }
                                    if (item.Category != ItemCategory.Food)
                                    {
                                        continue;
                                    }
                                    // Note that the monster doesn't actually get the food item -
                                    // it's gone
                                    itemName = item.Description(false, 0);
                                    string y = item.Count > 1 ? "One of y" : "Y";
                                    Profile.Instance.MsgPrint($"{y}our {itemName} ({i.IndexToLabel()}) was eaten!");
                                    player.Inventory.InvenItemIncrease(i, -1);
                                    player.Inventory.InvenItemOptimize(i);
                                    obvious = true;
                                    break;
                                }
                                break;
                            }
                        case AttackEffect.EatLight:
                            {
                                player.TakeHit(damage, monsterDescription);
                                item = player.Inventory[InventorySlot.Lightsource];
                                // Only dim lights that consume fuel
                                if (item.TypeSpecificValue > 0 && !item.IsFixedArtifact())
                                {
                                    item.TypeSpecificValue -= 250 + Program.Rng.DieRoll(250);
                                    if (item.TypeSpecificValue < 1)
                                    {
                                        item.TypeSpecificValue = 1;
                                    }
                                    if (player.TimedBlindness == 0)
                                    {
                                        Profile.Instance.MsgPrint("Your light dims.");
                                        obvious = true;
                                    }
                                }
                                break;
                            }
                        case AttackEffect.Acid:
                            {
                                obvious = true;
                                Profile.Instance.MsgPrint("You are covered in acid!");
                                _saveGame.SpellEffects.AcidDam(damage, monsterDescription);
                                level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsAcid);
                                break;
                            }
                        case AttackEffect.Electricity:
                            {
                                obvious = true;
                                Profile.Instance.MsgPrint("You are struck by electricity!");
                                _saveGame.SpellEffects.ElecDam(damage, monsterDescription);
                                level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsElec);
                                break;
                            }
                        case AttackEffect.Fire:
                            {
                                obvious = true;
                                Profile.Instance.MsgPrint("You are enveloped in flames!");
                                _saveGame.SpellEffects.FireDam(damage, monsterDescription);
                                level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsFire);
                                break;
                            }
                        case AttackEffect.Cold:
                            {
                                obvious = true;
                                Profile.Instance.MsgPrint("You are covered with frost!");
                                _saveGame.SpellEffects.ColdDam(damage, monsterDescription);
                                level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsCold);
                                break;
                            }
                        case AttackEffect.Blind:
                            {
                                player.TakeHit(damage, monsterDescription);
                                if (!player.HasBlindnessResistance)
                                {
                                    if (player.SetTimedBlindness(player.TimedBlindness + 10 + Program.Rng.DieRoll(monsterLevel)))
                                    {
                                        obvious = true;
                                    }
                                }
                                level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsBlind);
                                break;
                            }
                        case AttackEffect.Confuse:
                            {
                                player.TakeHit(damage, monsterDescription);
                                if (!player.HasConfusionResistance)
                                {
                                    if (player.SetTimedConfusion(player.TimedConfusion + 3 + Program.Rng.DieRoll(monsterLevel)))
                                    {
                                        obvious = true;
                                    }
                                }
                                level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsConf);
                                break;
                            }
                        case AttackEffect.Terrify:
                            {
                                player.TakeHit(damage, monsterDescription);
                                if (player.HasFearResistance)
                                {
                                    Profile.Instance.MsgPrint("You stand your ground!");
                                    obvious = true;
                                }
                                else if (Program.Rng.RandomLessThan(100) < player.SkillSavingThrow)
                                {
                                    Profile.Instance.MsgPrint("You stand your ground!");
                                    obvious = true;
                                }
                                else
                                {
                                    if (player.SetTimedFear(player.TimedFear + 3 + Program.Rng.DieRoll(monsterLevel)))
                                    {
                                        obvious = true;
                                    }
                                }
                                level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsFear);
                                break;
                            }
                        case AttackEffect.Paralyze:
                            {
                                if (damage == 0)
                                {
                                    damage = 1;
                                }
                                player.TakeHit(damage, monsterDescription);
                                if (player.HasFreeAction)
                                {
                                    Profile.Instance.MsgPrint("You are unaffected!");
                                    obvious = true;
                                }
                                else if (Program.Rng.RandomLessThan(100) < player.SkillSavingThrow)
                                {
                                    Profile.Instance.MsgPrint("You resist the effects!");
                                    obvious = true;
                                }
                                else
                                {
                                    if (player.SetTimedParalysis(player.TimedParalysis + 3 + Program.Rng.DieRoll(monsterLevel)))
                                    {
                                        obvious = true;
                                    }
                                }
                                level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsFree);
                                break;
                            }
                        case AttackEffect.LoseStr:
                            {
                                player.TakeHit(damage, monsterDescription);
                                if (player.TryDecreasingAbilityScore(Ability.Strength))
                                {
                                    obvious = true;
                                }
                                break;
                            }
                        case AttackEffect.LoseInt:
                            {
                                player.TakeHit(damage, monsterDescription);
                                if (player.TryDecreasingAbilityScore(Ability.Intelligence))
                                {
                                    obvious = true;
                                }
                                break;
                            }
                        case AttackEffect.LoseWis:
                            {
                                player.TakeHit(damage, monsterDescription);
                                if (player.TryDecreasingAbilityScore(Ability.Wisdom))
                                {
                                    obvious = true;
                                }
                                break;
                            }
                        case AttackEffect.LoseDex:
                            {
                                player.TakeHit(damage, monsterDescription);
                                if (player.TryDecreasingAbilityScore(Ability.Dexterity))
                                {
                                    obvious = true;
                                }
                                break;
                            }
                        case AttackEffect.LoseCon:
                            {
                                player.TakeHit(damage, monsterDescription);
                                if (player.TryDecreasingAbilityScore(Ability.Constitution))
                                {
                                    obvious = true;
                                }
                                break;
                            }
                        case AttackEffect.LoseCha:
                            {
                                player.TakeHit(damage, monsterDescription);
                                if (player.TryDecreasingAbilityScore(Ability.Charisma))
                                {
                                    obvious = true;
                                }
                                break;
                            }
                        case AttackEffect.LoseAll:
                            {
                                // Try to decrease all six ability scores
                                player.TakeHit(damage, monsterDescription);
                                if (player.TryDecreasingAbilityScore(Ability.Strength))
                                {
                                    obvious = true;
                                }
                                if (player.TryDecreasingAbilityScore(Ability.Dexterity))
                                {
                                    obvious = true;
                                }
                                if (player.TryDecreasingAbilityScore(Ability.Constitution))
                                {
                                    obvious = true;
                                }
                                if (player.TryDecreasingAbilityScore(Ability.Intelligence))
                                {
                                    obvious = true;
                                }
                                if (player.TryDecreasingAbilityScore(Ability.Wisdom))
                                {
                                    obvious = true;
                                }
                                if (player.TryDecreasingAbilityScore(Ability.Charisma))
                                {
                                    obvious = true;
                                }
                                break;
                            }
                        case AttackEffect.Shatter:
                            {
                                obvious = true;
                                damage -= damage * (armourClass < 150 ? armourClass : 150) / 250;
                                player.TakeHit(damage, monsterDescription);
                                // Do an earthquake only if we did enough damage on the hit
                                if (damage > 23)
                                {
                                    _saveGame.SpellEffects.Earthquake(monster.MapY, monster.MapX, 8);
                                }
                                break;
                            }
                        case AttackEffect.Exp10:
                            {
                                obvious = true;
                                player.TakeHit(damage, monsterDescription);
                                if (player.HasHoldLife && Program.Rng.RandomLessThan(100) < 95)
                                {
                                    Profile.Instance.MsgPrint("You keep hold of your life force!");
                                }
                                else if (Program.Rng.DieRoll(10) <= player.Religion.GetNamedDeity(Pantheon.GodName.Hagarg_Ryonis).AdjustedFavour)
                                {
                                    // Hagarg Ryonis can protect us from experience loss
                                    Profile.Instance.MsgPrint("Hagarg Ryonis's favour protects you!");
                                }
                                else
                                {
                                    int d = Program.Rng.DiceRoll(10, 6) + (player.ExperiencePoints / 100 * Constants.MonDrainLife);
                                    if (player.HasHoldLife)
                                    {
                                        Profile.Instance.MsgPrint("You feel your life slipping away!");
                                        player.LoseExperience(d / 10);
                                    }
                                    else
                                    {
                                        Profile.Instance.MsgPrint("You feel your life draining away!");
                                        player.LoseExperience(d);
                                    }
                                }
                                break;
                            }
                        case AttackEffect.Exp20:
                            {
                                obvious = true;
                                player.TakeHit(damage, monsterDescription);
                                if (player.HasHoldLife && Program.Rng.RandomLessThan(100) < 90)
                                {
                                    Profile.Instance.MsgPrint("You keep hold of your life force!");
                                }
                                else if (Program.Rng.DieRoll(10) <= player.Religion.GetNamedDeity(Pantheon.GodName.Hagarg_Ryonis).AdjustedFavour)
                                {
                                    // Hagarg Ryonis can protect us from experience loss
                                    Profile.Instance.MsgPrint("Hagarg Ryonis's favour protects you!");
                                }
                                else
                                {
                                    int d = Program.Rng.DiceRoll(20, 6) + (player.ExperiencePoints / 100 * Constants.MonDrainLife);
                                    if (player.HasHoldLife)
                                    {
                                        Profile.Instance.MsgPrint("You feel your life slipping away!");
                                        player.LoseExperience(d / 10);
                                    }
                                    else
                                    {
                                        Profile.Instance.MsgPrint("You feel your life draining away!");
                                        player.LoseExperience(d);
                                    }
                                }
                                break;
                            }
                        case AttackEffect.Exp40:
                            {
                                obvious = true;
                                player.TakeHit(damage, monsterDescription);
                                if (player.HasHoldLife && Program.Rng.RandomLessThan(100) < 75)
                                {
                                    Profile.Instance.MsgPrint("You keep hold of your life force!");
                                }
                                else if (Program.Rng.DieRoll(10) <= player.Religion.GetNamedDeity(Pantheon.GodName.Hagarg_Ryonis).AdjustedFavour)
                                {
                                    // Hagarg Ryonis can protect us from experience loss
                                    Profile.Instance.MsgPrint("Hagarg Ryonis's favour protects you!");
                                }
                                else
                                {
                                    int d = Program.Rng.DiceRoll(40, 6) + (player.ExperiencePoints / 100 * Constants.MonDrainLife);
                                    if (player.HasHoldLife)
                                    {
                                        Profile.Instance.MsgPrint("You feel your life slipping away!");
                                        player.LoseExperience(d / 10);
                                    }
                                    else
                                    {
                                        Profile.Instance.MsgPrint("You feel your life draining away!");
                                        player.LoseExperience(d);
                                    }
                                }
                                break;
                            }
                        case AttackEffect.Exp80:
                            {
                                obvious = true;
                                player.TakeHit(damage, monsterDescription);
                                if (player.HasHoldLife && Program.Rng.RandomLessThan(100) < 50)
                                {
                                    Profile.Instance.MsgPrint("You keep hold of your life force!");
                                }
                                else if (Program.Rng.DieRoll(10) <= player.Religion.GetNamedDeity(Pantheon.GodName.Hagarg_Ryonis).AdjustedFavour)
                                {
                                    // Hagarg Ryonis can protect us from experience loss
                                    Profile.Instance.MsgPrint("Hagarg Ryonis's favour protects you!");
                                }
                                else
                                {
                                    int d = Program.Rng.DiceRoll(80, 6) + (player.ExperiencePoints / 100 * Constants.MonDrainLife);
                                    if (player.HasHoldLife)
                                    {
                                        Profile.Instance.MsgPrint("You feel your life slipping away!");
                                        player.LoseExperience(d / 10);
                                    }
                                    else
                                    {
                                        Profile.Instance.MsgPrint("You feel your life draining away!");
                                        player.LoseExperience(d);
                                    }
                                }
                                break;
                            }
                    }
                    // Be nice and don't let us be both stunned and cut by the same blow
                    if (doCut && doStun)
                    {
                        if (Program.Rng.RandomLessThan(100) < 50)
                        {
                            doCut = false;
                        }
                        else
                        {
                            doStun = false;
                        }
                    }
                    int critLevel;
                    if (doCut)
                    {
                        // Get how bad the hit was based on the actual damage out of the possible damage
                        critLevel = MonsterCritical(damageDice, damageSides, damage);
                        switch (critLevel)
                        {
                            case 0:
                                k = 0;
                                break;

                            case 1:
                                k = Program.Rng.DieRoll(5);
                                break;

                            case 2:
                                k = Program.Rng.DieRoll(5) + 5;
                                break;

                            case 3:
                                k = Program.Rng.DieRoll(20) + 20;
                                break;

                            case 4:
                                k = Program.Rng.DieRoll(50) + 50;
                                break;

                            case 5:
                                k = Program.Rng.DieRoll(100) + 100;
                                break;

                            case 6:
                                k = 300;
                                break;

                            default:
                                k = 500;
                                break;
                        }
                        if (k != 0)
                        {
                            player.SetTimedBleeding(player.TimedBleeding + k);
                        }
                    }
                    if (doStun)
                    {
                        // Get how bad the hit was based on the actual damage out of the possible damage
                        critLevel = MonsterCritical(damageDice, damageSides, damage);
                        switch (critLevel)
                        {
                            case 0:
                                k = 0;
                                break;

                            case 1:
                                k = Program.Rng.DieRoll(5);
                                break;

                            case 2:
                                k = Program.Rng.DieRoll(10) + 10;
                                break;

                            case 3:
                                k = Program.Rng.DieRoll(20) + 20;
                                break;

                            case 4:
                                k = Program.Rng.DieRoll(30) + 30;
                                break;

                            case 5:
                                k = Program.Rng.DieRoll(40) + 40;
                                break;

                            case 6:
                                k = 100;
                                break;

                            default:
                                k = 200;
                                break;
                        }
                        if (k != 0)
                        {
                            player.SetTimedStun(player.TimedStun + k);
                        }
                    }
                    // If the monster touched us then it may take damage from our defensive abilities
                    if (touched)
                    {
                        if (player.HasFireShield && alive)
                        {
                            if ((race.Flags3 & MonsterFlag3.ImmuneFire) == 0)
                            {
                                Profile.Instance.MsgPrint($"{monsterName} is suddenly very hot!");
                                if (level.Monsters.DamageMonster(monsterIndex, Program.Rng.DiceRoll(2, 6), out fear,
                                    " turns into a pile of ash."))
                                {
                                    blinked = false;
                                    alive = false;
                                }
                            }
                            else
                            {
                                // The player noticed that the monster took no fire damage
                                if (monster.IsVisible)
                                {
                                    race.Knowledge.RFlags3 |= MonsterFlag3.ImmuneFire;
                                }
                            }
                        }
                        if (player.HasLightningShield && alive)
                        {
                            if ((race.Flags3 & MonsterFlag3.ImmuneLightning) == 0)
                            {
                                Profile.Instance.MsgPrint($"{monsterName} gets zapped!");
                                if (level.Monsters.DamageMonster(monsterIndex, Program.Rng.DiceRoll(2, 6), out fear,
                                    " turns into a pile of cinder."))
                                {
                                    blinked = false;
                                    alive = false;
                                }
                            }
                            else
                            {
                                // The player noticed that the monster took no lightning damage
                                if (monster.IsVisible)
                                {
                                    race.Knowledge.RFlags3 |= MonsterFlag3.ImmuneLightning;
                                }
                            }
                        }
                    }
                }
                else
                {
                    // It missed us, so give us the appropriate message
                    switch (method)
                    {
                        case AttackType.Hit:
                        case AttackType.Touch:
                        case AttackType.Punch:
                        case AttackType.Kick:
                        case AttackType.Claw:
                        case AttackType.Bite:
                        case AttackType.Sting:
                        case AttackType.Butt:
                        case AttackType.Crush:
                        case AttackType.Engulf:
                        case AttackType.Charge:
                            if (monster.IsVisible)
                            {
                                _saveGame.Disturb(true);
                                Profile.Instance.MsgPrint($"{monsterName} misses you.");
                            }
                            break;
                    }
                }
                // If the player saw the monster, they now know more about what attacks it can do
                if (visible)
                {
                    if (obvious || damage != 0 || race.Knowledge.RBlows[attackNumber] > 10)
                    {
                        if (race.Knowledge.RBlows[attackNumber] < Constants.MaxUchar)
                        {
                            race.Knowledge.RBlows[attackNumber]++;
                        }
                    }
                }
            }
            // If the monster teleported away after stealing, let the player know and do the actual teleport
            if (blinked)
            {
                Profile.Instance.MsgPrint("The thief flees laughing!");
                _saveGame.SpellEffects.TeleportAway(monsterIndex, (Constants.MaxSight * 2) + 5);
            }
            // If the attack just killed the player, let future generations remember what killed
            // their ancestor
            if (player.IsDead && race.Knowledge.RDeaths < Constants.MaxShort)
            {
                race.Knowledge.RDeaths++;
            }
            // If the monster just got scared, let the player know
            if (monster.IsVisible && fear)
            {
                Gui.PlaySound(SoundEffect.MonsterFlees);
                Profile.Instance.MsgPrint($"{monsterName} flees in terror!");
            }
        }

        /// <summary>
        /// Check whether an attack hits the player.
        /// </summary>
        /// <param name="attackPower"> The power of the attack </param>
        /// <param name="monsterLevel"> The level of the monster making the attack </param>
        /// <returns> True if the attack hit, false if not </returns>
        private bool MonsterCheckHitOnPlayer(int attackPower, int monsterLevel)
        {
            // Straight five percent chance of hit or miss
            int k = Program.Rng.RandomLessThan(100);
            if (k < 10)
            {
                return k < 5;
            }
            // Otherwise, compare the power and level to the player's armour class
            int i = attackPower + (monsterLevel * 3);
            int ac = _saveGame.Player.BaseArmourClass + _saveGame.Player.ArmourClassBonus;
            return i > 0 && Program.Rng.DieRoll(i) > ac * 3 / 4;
        }

        /// <summary>
        /// Work out if the monster hit the player hard enough to cause a critical hit (a cut or a stun)
        /// </summary>
        /// <param name="dice"> The number of dice of damage caused </param>
        /// <param name="sides"> The number of sides on each damage dice </param>
        /// <param name="damage"> The actual damage the attack caused </param>
        /// <returns> </returns>
        private int MonsterCritical(int dice, int sides, int damage)
        {
            int additionalSeverity = 0;
            int maxDamage = dice * sides;
            // If we did less than 95% of maximum damage, definitely no cuts or stun
            if (damage < maxDamage * 19 / 20)
            {
                return 0;
            }
            // If we did less than 20 damage, then usually no cuts or stun
            if (damage < 20 && Program.Rng.RandomLessThan(100) >= damage)
            {
                return 0;
            }
            // We're going to do a crit based on the damage done, and doing max damage increases the severity
            if (damage == maxDamage)
            {
                additionalSeverity++;
            }
            // More than 20 damage increases the severity a random number of times
            if (damage >= 20)
            {
                while (Program.Rng.RandomLessThan(100) < 2)
                {
                    additionalSeverity++;
                }
            }
            // Higher damage means more severe base (to which the increase is added)
            if (damage > 45)
            {
                return 6 + additionalSeverity;
            }
            if (damage > 33)
            {
                return 5 + additionalSeverity;
            }
            if (damage > 25)
            {
                return 4 + additionalSeverity;
            }
            if (damage > 18)
            {
                return 3 + additionalSeverity;
            }
            if (damage > 11)
            {
                return 2 + additionalSeverity;
            }
            return 1 + additionalSeverity;
        }
    }
}