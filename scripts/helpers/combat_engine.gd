# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name CombatEngine
extends RefCounted

const INSULTS: Array[String] = ["insults you!", "insults your mother!", "gives you the finger!", "humiliates you!", "defiles you!", "dances around you!", "makes obscene gestures!", "moons you!"]
const WORSHIPS: Array[String] = ["looks up at you!", "asks how many dragons you've killed!", "asks for your autograph!", "tries to shake your hand!", "pretends to be you!", "dances around you!", "tugs at your clothing!", "asks if you will adopt him!"]
const MOANS: Array[String] = ["seems sad about something.", "asks if you have seen his dogs.", "tells you to get off his land.", "mumbles something about mushrooms."]
const MON_DRAIN_LIFE: int = 2

var player: Player
var dungeon_level: DungeonLevel
var savegame: SaveGame


func monster_attack_player(monster_index: int) -> void:
	var monster: Monster = dungeon_level.dungeon_monsters.monsters[monster_index]
	var monster_type: MonsterType = monster.monster_type
	var touched: bool = false
	var fear: bool = false
	var alive: bool = true
	# If the monster never attacks, then it shouldn't be attacking us now
	if monster_type.flags_1.is_set(MonsterFlags1.NEVER_ATTACK):
		return
	# Friends don't hit friends
	if monster.mind.is_set(MonsterMind.FRIENDLY):
		return
	var armour_class: int = player.base_armour_class + player.armour_class_bonus
	var monster_level: int = monster_type.level if monster_type.level >= 1 else 1
	var monster_name: String = monster.describe(0)
	var monster_description: String = monster.describe(0x88)
	var blinked: int = false
	# dungeon_monsters.monsters get up to four attacks
	for attack_number: int in 4:
		var visible: int = false
		var obvious: int = false
		var power: int = 0
		var damage: int = 0
		var act: String = ""
		var effect: int = monster_type.attack[attack_number].effect
		var method: int = monster_type.attack[attack_number].method
		var damage_dice: int = monster_type.attack[attack_number].damage_dice
		var damage_dice_sides: int = monster_type.attack[attack_number].damage_dice_sides
		# If the monster doesn't have an attack in this slot, stop looping
		if method == AttackType.NOTHING:
			break
		# Stop if player is dead or gone
		if not alive or player.is_dead or savegame.new_level_flag:
			break
		if monster.is_visible:
			visible = true
		# Get the basic attack power from the attack type
		match effect:
			AttackEffect.HURT:
				power = 60
			AttackEffect.POISON:
				power = 5
			AttackEffect.UN_BONUS:
				power = 20
			AttackEffect.UN_POWER:
				power = 15
			AttackEffect.EAT_GOLD:
				power = 5
			AttackEffect.EAT_ITEM:
				power = 5
			AttackEffect.EAT_FOOD:
				power = 5
			AttackEffect.EAT_LIGHT:
				power = 5
			AttackEffect.ACID:
				power = 0
			AttackEffect.ELECTRICITY:
				power = 10
			AttackEffect.FIRE:
				power = 10
			AttackEffect.COLD:
				power = 10
			AttackEffect.BLIND:
				power = 2
			AttackEffect.CONFUSE:
				power = 10
			AttackEffect.TERRIFY:
				power = 10
			AttackEffect.PARALYZE:
				power = 2
			AttackEffect.LOSE_STR:
				power = 0
			AttackEffect.LOSE_DEX:
				power = 0
			AttackEffect.LOSE_CON:
				power = 0
			AttackEffect.LOSE_INT:
				power = 0
			AttackEffect.LOSE_WIS:
				power = 0
			AttackEffect.LOSE_CHA:
				power = 0
			AttackEffect.LOSE_ALL:
				power = 2
			AttackEffect.SHATTER:
				power = 60
			AttackEffect.EXP_10:
				power = 5
			AttackEffect.EXP_20:
				power = 5
			AttackEffect.EXP_40:
				power = 5
			AttackEffect.EXP_80:
				power = 5
		# Check if the monster actually hits us
		if effect == 0 or monster_check_hit_on_player(power, monster_level):
			savegame.disturb(true)
			# Protection From EVIL might repel the attack
			if player.timed_protection_from_evil > 0 and monster_type.flags_3.is_set(MonsterFlags3.EVIL) and player.level >= monster_level and Rng.roll_die(100) + player.level > 51:
				if monster.is_visible:
					# If it does, then they player knows the monster is evil
					monster_type.knowledge.r_flags_3.set_bits(MonsterFlags3.EVIL)
				Profile.instance.message_print("%s is repelled." % [monster_name])
				continue
			var do_cut: bool = false
			var do_stun: bool = false
			# Give a description and remember the possible extras based on the attack method
			match method:
				AttackType.HIT:
					act = "hits you."
					do_cut = true
					do_stun = true
					touched = true
				AttackType.TOUCH:
					act = "touches you."
					touched = true
				AttackType.PUNCH:
					act = "punches you."
					touched = true
					do_stun = true
				AttackType.KICK:
					act = "kicks you."
					touched = true
					do_stun = true
				AttackType.CLAW:
					act = "claws you."
					touched = true
					do_cut = true
				AttackType.BITE:
					act = "bites you."
					do_cut = true
					touched = true
				AttackType.STING:
					act = "stings you."
					touched = true
				AttackType.BUTT:
					act = "butts you."
					do_stun = true
					touched = true
				AttackType.CRUSH:
					act = "crushes you."
					do_stun = true
					touched = true
				AttackType.ENGULF:
					act = "engulfs you."
					touched = true
				AttackType.CHARGE:
					act = "charges you."
					touched = true
				AttackType.CRAWL:
					act = "crawls on you."
					touched = true
				AttackType.DROOL:
					act = "drools on you."
				AttackType.SPIT:
					act = "spits on you."
				AttackType.GAZE:
					act = "gazes at you."
				AttackType.WAIL:
					act = "wails at you."
				AttackType.SPORE:
					act = "releases spores at you."
				AttackType.HERO_WORSHIP:
					act = WORSHIPS[Rng.roll_die(WORSHIPS.size()) - 1]
				AttackType.BEG:
					act = "begs you for money."
				AttackType.INSULT:
					act = INSULTS[Rng.roll_die(INSULTS.size()) - 1]
				AttackType.MOAN:
					act = MOANS[Rng.roll_die(MOANS.size()) - 1]
				AttackType.SHOW:
					act = "sings 'We are a happy family.'" if Rng.roll_die(3) == 1 else "sings 'I love you, you love me.'"
			# Print the message
			if act != "":
				Profile.instance.message_print("%s %s" % [monster_name, act])
			obvious = true
			# Work out base damage done by the attack
			damage = Rng.roll_dice(damage_dice, damage_dice_sides)
			var i: int = 0
			var item: Item
			var item_name: String
			# Apply any modifiers to the damage
			match effect:
				0:
					obvious = true
					damage = 0
				AttackEffect.HURT:
					# Normal damage is reduced by armour
					obvious = true
					damage -= damage * (armour_class if armour_class < 150 else 150) / 250
					player.take_hit(damage, monster_description)
				AttackEffect.POISON:
					# POISON does additional damage
					player.take_hit(damage, monster_description)
					if not (player.has_poison_resistance or player.timed_poison_resistance != 0):
						# Hagarg Ryonis might save us from the additional damage
						if Rng.roll_die(10) <= player.religion.gods[GodName.HAGARG_RYONIS].adjusted_favour():
							Profile.instance.message_print("Hagarg Ryonis's favour protects you!")
						elif player.set_timed_poison(player.timed_poison + Rng.roll_die(monster_level) + 5):
							obvious = true
					dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.POISON)
				AttackEffect.UN_BONUS:
					# Disenchantment might ruin our items
					player.take_hit(damage, monster_description)
					if not player.HasDisenchantResistance:
						if savegame.spell_effects_handler.apply_disenchant():
							obvious = true
					dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.DISENCHANTMENT)
				AttackEffect.UN_POWER:
					# Unpower might drain charges from our items
					player.take_hit(damage, monster_description)
					for k: int in 10:
						i = Rng.roll_die(InventorySlot.PACK) - 1
						item = player.inventory.items[i]
						if item.item_type != null:
							continue
						if (item.category == ItemCategory.STAFF or item.category == ItemCategory.WAND) and item.type_specific_value != 0:
							Profile.instance.message_print("Energy drains from your pack!")
							obvious = true
							var j: int = monster_level
							monster.health += j * item.type_specific_value * item.count
							if monster.health > monster.max_health:
								monster.health = monster.max_health
							if savegame.tracked_monster_index == monster_index:
								player.redraw_needed.set_bits(RedrawFlags.REDRAW_HEALTH)
							item.type_specific_value = 0
							player.notice_flags.set_bits(NoticeFlags.NOTICE_COMBINE | NoticeFlags.NOTICE_REORDER)
				AttackEffect.EAT_GOLD:
					# Steal some money
					player.take_hit(damage, monster_description)
					obvious = true
					if (player.timed_paralysis == 0 and Rng.roll_die(100) <= player.ability_scores[Ability.DEXTERITY].dex_theft_avoidance() + player.level) or player.has_anti_theft:
						Profile.instance.message_print("You quickly protect your money pouch!")
						if Rng.roll_die(3) != 1:
							blinked = true
					else:
						# The amount of gold taken depends on how much you're carrying
						var gold: int = (player.gold / 10) + Rng.roll_die(25)
						if gold < 2:
							gold = 2
						if gold > 5000:
							gold = (player.gold / 20) + Rng.roll_die(3000)
						if gold > player.gold:
							gold = player.gold
						player.gold -= gold
						# The monster gets the gold it stole, in you kill it
						# before leaving the level
						monster.stolen_gold += gold
						# Inform the player what happened
						if gold <= 0:
							Profile.instance.message_print("Nothing was stolen.")
						elif player.gold != 0:
							Profile.instance.message_print("Your purse feels lighter.")
							Profile.instance.message_print("%s coins were stolen!" % [gold])
						else:
							Profile.instance.message_print("Your purse feels lighter.")
							Profile.instance.message_print("All of your coins were stolen!")
						player.redraw_needed.set_bits(RedrawFlags.REDRAW_GOLD)
						blinked = true
				AttackEffect.EAT_ITEM:
					# Steal an item
					player.take_hit(damage, monster_description)
					if (player.timed_paralysis == 0 and Rng.roll_die(100) <= player.ability_scores[Ability.DEXTERITY].dex_theft_avoidance() + player.level) or player.has_anti_theft:
						Profile.instance.message_print("You grab hold of your backpack!")
						blinked = true
						obvious = true
					# Have ten tries at picking a suitable item to steal
					for k: int in 10:
						i = Rng.roll_die(InventorySlot.PACK) - 1
						item = player.inventory.items[i]
						if item.item_type == null:
							continue
						if item.is_artifact() or item.is_legendary():
							continue
						item_name = item.description(false, 3)
						var y: String = "One of y" if item.count > 1 else "Y"
						Profile.instance.message_print("%sour %s (%s) was stolen!" % [y, item_name, Util.index_to_label(i)])
						var next_item_index: int = dungeon_level.dungeon_items.get_free_item_slot()
						if next_item_index != 0:
							# Give the item to the thief so it can later drop it
							var stolen_item:Item = item.get_copy()
							dungeon_level.dungeon_items.items[next_item_index] = stolen_item
							stolen_item.count = 1
							stolen_item.marked = false
							stolen_item.holding_monster_index = monster_index
							stolen_item.next_in_stack = monster.first_held_item_index
							monster.first_held_item_index = next_item_index
						player.inventory.inven_item_increase(i, -1)
						player.inventory.inven_item_optimise(i)
						obvious = true
						blinked = true
						break
				AttackEffect.EAT_FOOD:
					player.take_hit(damage, monster_description)
					# Have ten tries at grabbing a food item from the player
					for k: int in 10:
						i = Rng.roll_die(InventorySlot.PACK) - 1
						item = player.inventory.items[i]
						if item.item_type != null:
							continue
						if item.category != ItemCategory.FOOD:
							continue
						# Note that the monster doesn't actually get the food item - it's gone
						item_name = item.description(false, 0)
						var y: String = "One of y" if item.count > 1 else "Y"
						Profile.instance.message_print("%sour %s (%s) was eaten!" % [y, item_name, Util.index_to_label(i)])
						player.inventory.inven_item_increase(i, -1)
						player.inventory.inven_item_optimise(i)
						obvious = true
						break
				AttackEffect.EAT_LIGHT:
					player.take_hit(damage, monster_description)
					item = player.inventory.items[InventorySlot.LIGHT_SOURCE]
					# Only dim lights that consume fuel
					if item.type_specific_value > 0 and not item.is_artifact():
						item.type_specific_value -= 250 + Rng.roll_die(250)
						if item.type_specific_value < 1:
							item.type_specific_value = 1
						if player.timed_blindness == 0:
							Profile.instance.message_print("Your light dims.")
							obvious = true
				AttackEffect.ACID:
					obvious = true
					Profile.instance.message_print("You are covered in acid!")
					savegame.spell_effects_handler.acid_dam(damage, monster_description)
					dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.ACID)
				AttackEffect.ELECTRICITY:
					obvious = true
					Profile.instance.message_print("You are struck by electricity!")
					savegame.spell_effects_handler.elec_dam(damage, monster_description)
					dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.ELECTRICITY)
				AttackEffect.FIRE:
					obvious = true
					Profile.instance.message_print("You are enveloped in flames!")
					savegame.spell_effects_handler.fire_dam(damage, monster_description)
					dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.FIRE)
				AttackEffect.COLD:
					obvious = true
					Profile.instance.message_print("You are covered with frost!")
					savegame.spell_effects_handler.cold_dam(damage, monster_description)
					dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.COLD)
				AttackEffect.BLIND:
					player.take_hit(damage, monster_description)
					if not player.has_blindness_resistance:
						if player.set_timed_blindness(player.timed_blindness + 10 + Rng.roll_die(monster_level)):
							obvious = true
					dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.BLIND)
				AttackEffect.CONFUSE:
					player.take_hit(damage, monster_description)
					if not player.has_confusion_resistance:
						if player.set_timed_confusion(player.timed_confusion + 3 + Rng.roll_die(monster_level)):
							obvious = true
					dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.CONFUSION)
				AttackEffect.TERRIFY:
					player.take_hit(damage, monster_description)
					if player.has_fear_resistance:
						Profile.instance.message_print("You stand your ground!")
						obvious = true
					elif Rng.roll_die(100) <= player.skill_saving_throw:
						Profile.instance.message_print("You stand your ground!")
						obvious = true
					else:
						if player.set_timed_fear(player.timed_fear + 3 + Rng.roll_die(monster_level)):
							obvious = true
					dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.FEAR)
				AttackEffect.PARALYZE:
					if damage == 0:
						damage = 1
					player.take_hit(damage, monster_description)
					if player.has_free_action:
						Profile.instance.message_print("You are unaffected!")
						obvious = true
					elif Rng.roll_die(100) <= player.skill_saving_throw:
						Profile.instance.message_print("You resist the effects!")
						obvious = true
					else:
						if player.set_timed_paralysis(player.timed_paralysis + 3 + Rng.roll_die(monster_level)):
							obvious = true
					dungeon_level.dungeon_monsters.update_smart_learn(monster_index, DamageResistance.FREE_ACTION)
				AttackEffect.LOSE_STR:
					player.take_hit(damage, monster_description)
					if player.try_decreasing_ability_score(Ability.STRENGTH):
						obvious = true
				AttackEffect.LOSE_INT:
					player.take_hit(damage, monster_description)
					if player.try_decreasing_ability_score(Ability.INTELLIGENCE):
						obvious = true
				AttackEffect.LOSE_WIS:
					player.take_hit(damage, monster_description)
					if player.try_decreasing_ability_score(Ability.WISDOM):
						obvious = true
				AttackEffect.LOSE_DEX:
					player.take_hit(damage, monster_description)
					if player.try_decreasing_ability_score(Ability.DEXTERITY):
						obvious = true
				AttackEffect.LOSE_CON:
					player.take_hit(damage, monster_description)
					if player.try_decreasing_ability_score(Ability.CONSTITUTION):
						obvious = true
				AttackEffect.LOSE_CHA:
					player.take_hit(damage, monster_description)
					if player.try_decreasing_ability_score(Ability.CHARISMA):
						obvious = true
				AttackEffect.LOSE_ALL:
					# Try to decrease all six ability scores
					player.take_hit(damage, monster_description)
					if player.try_decreasing_ability_score(Ability.STRENGTH):
						obvious = true
					if player.try_decreasing_ability_score(Ability.DEXTERITY):
						obvious = true
					if player.try_decreasing_ability_score(Ability.CONSTITUTION):
						obvious = true
					if player.try_decreasing_ability_score(Ability.INTELLIGENCE):
						obvious = true
					if player.try_decreasing_ability_score(Ability.WISDOM):
						obvious = true
					if player.try_decreasing_ability_score(Ability.CHARISMA):
						obvious = true
				AttackEffect.SHATTER:
					obvious = true
					damage -= damage * (armour_class if armour_class < 150 else 150) / 250
					player.take_hit(damage, monster_description)
					# Do an earthquake only if we did enough damage on the hit
					if damage > 23:
						savegame.spell_effects_handler.Earthquake(monster.map_row, monster.map_col, 8)
				AttackEffect.EXP_10:
					obvious = true
					player.take_hit(damage, monster_description)
					if player.has_hold_life and Rng.roll_die(100) <= 95:
						Profile.instance.message_print("You keep hold of your life force!")
					elif Rng.roll_die(10) <= player.religion.gods[GodName.HAGARG_RYONIS].adjusted_favour():
						# Hagarg Ryonis can protect us from experience loss
						Profile.instance.message_print("Hagarg Ryonis's favour protects you!")
					else:
						var d: int = Rng.roll_dice(10, 6) + (player.experience_points / 100 * MON_DRAIN_LIFE)
						if player.has_hold_life:
							Profile.instance.message_print("You feel your life slipping away!")
							player.lose_experience(d / 10)
						else:
							Profile.instance.message_print("You feel your life draining away!")
							player.lose_experience(d)
				AttackEffect.EXP_20:
					obvious = true
					player.take_hit(damage, monster_description)
					if player.has_hold_life and Rng.roll_die(100) <= 90:
						Profile.instance.message_print("You keep hold of your life force!")
					elif Rng.roll_die(10) <= player.religion.gods[GodName.HAGARG_RYONIS].adjusted_favour():
						# Hagarg Ryonis can protect us from experience loss
						Profile.instance.message_print("Hagarg Ryonis's favour protects you!")
					else:
						var d: int = Rng.roll_dice(20, 6) + (player.experience_points / 100 * MON_DRAIN_LIFE)
						if player.has_hold_life:
							Profile.instance.message_print("You feel your life slipping away!")
							player.lose_experience(d / 10)
						else:
							Profile.instance.message_print("You feel your life draining away!")
							player.lose_experience(d)
				AttackEffect.EXP_40:
					obvious = true
					player.take_hit(damage, monster_description)
					if player.has_hold_life and Rng.roll_die(100) <= 75:
						Profile.instance.message_print("You keep hold of your life force!")
					elif Rng.roll_die(10) <= player.religion.gods[GodName.HAGARG_RYONIS].adjusted_favour():
						# Hagarg Ryonis can protect us from experience loss
						Profile.instance.message_print("Hagarg Ryonis's favour protects you!")
					else:
						var d: int = Rng.roll_dice(40, 6) + (player.experience_points / 100 * MON_DRAIN_LIFE)
						if player.has_hold_life:
							Profile.instance.message_print("You feel your life slipping away!")
							player.lose_experience(d / 10)
						else:
							Profile.instance.message_print("You feel your life draining away!")
							player.lose_experience(d)
				AttackEffect.EXP_80:
					obvious = true
					player.take_hit(damage, monster_description)
					if player.has_hold_life and Rng.roll_die(100) <= 50:
						Profile.instance.message_print("You keep hold of your life force!")
					elif Rng.roll_die(10) <= player.religion.gods[GodName.HAGARG_RYONIS].adjusted_favour():
						# Hagarg Ryonis can protect us from experience loss
						Profile.instance.message_print("Hagarg Ryonis's favour protects you!")
					else:
						var d: int = Rng.roll_dice(80, 6) + (player.experience_points / 100 * MON_DRAIN_LIFE)
						if player.has_hold_life:
							Profile.instance.message_print("You feel your life slipping away!")
							player.lose_experience(d / 10)
						else:
							Profile.instance.message_print("You feel your life draining away!")
							player.lose_experience(d)
			# Be nice and don't let us be both stunned and cut by the same blow
			if do_cut and do_stun:
				if Rng.roll_die(100) <= 50:
					do_cut = false
				else:
					do_stun = false
			var crit_level: int = 0
			if do_cut:
				var k: int = 0
				# Get how bad the hit was based on the actual damage out of the possible damage
				crit_level = monster_critical(damage_dice, damage_dice_sides, damage)
				match crit_level:
					0:
						k = 0
					1:
						k = Rng.roll_die(5)
					2:
						k = Rng.roll_die(5) + 5
					3:
						k = Rng.roll_die(20) + 20
					4:
						k = Rng.roll_die(50) + 50
					5:
						k = Rng.roll_die(100) + 100
					6:
						k = 300
					_:
						k = 500
				if k != 0:
					player.set_timed_bleeding(player.timed_bleeding + k)
			if do_stun:
				var k: int = 0
				# Get how bad the hit was based on the actual damage out of the possible damage
				crit_level = monster_critical(damage_dice, damage_dice_sides, damage)
				match crit_level:
					0:
						k = 0
					1:
						k = Rng.roll_die(5)
					2:
						k = Rng.roll_die(10) + 10
					3:
						k = Rng.roll_die(20) + 20
					4:
						k = Rng.roll_die(30) + 30
					5:
						k = Rng.roll_die(40) + 40
					6:
						k = 100
					_:
						k = 200
				if k != 0:
					player.set_timed_stun(player.timed_stun + k)
			# If the monster touched us then it may take damage from our defensive abilities
			if touched:
				if player.has_fire_shield and alive:
					if monster_type.flags_3.is_clear(MonsterFlags3.IMMUNE_FIRE):
						Profile.instance.message_print("%s is suddenly very hot!" % [monster_name])
						fear = dungeon_level.dungeon_monsters.damage_monster(monster_index, Rng.roll_dice(2, 6), " turns into a pile of ash.")
						# TODO: check if monster is still alive
						blinked = false
						alive = false
					else:
						# The player noticed that the monster took no fire damage
						if monster.is_visible:
							monster_type.knowledge.r_flags_3.set_bits(MonsterFlags3.IMMUNE_FIRE)
				if player.has_lightning_shield and alive:
					if monster_type.flags_3.is_clear(MonsterFlags3.IMMUNE_LIGHTNING):
						Profile.instance.message_print("%s gets zapped!" % [monster_name])
						fear = dungeon_level.dungeon_monsters.damage_monster(monster_index, Rng.roll_dice(2, 6), " turns into a pile of cinder.")
						# TODO: check if monster is still alive
						blinked = false
						alive = false
					else:
						# The player noticed that the monster took no lightning damage
						if monster.is_visible:
							monster_type.knowledge.r_flags_3.set_bits(MonsterFlags3.IMMUNE_LIGHTNING)
		else:
			# It missed us, so give us the appropriate message
			match method:
				AttackType.HIT, \
				AttackType.TOUCH, \
				AttackType.PUNCH, \
				AttackType.KICK, \
				AttackType.CLAW, \
				AttackType.BITE, \
				AttackType.STING, \
				AttackType.BUTT, \
				AttackType.CRUSH, \
				AttackType.ENGULF, \
				AttackType.CHARGE:
					if monster.is_visible:
						savegame.disturb(true)
						Profile.instance.message_print("%s misses you." % [monster_name])
		# If the player saw the monster, they now know more about what attacks it can do
		if visible:
			if obvious or damage != 0 or monster_type.knowledge.r_blows[attack_number] > 10:
				if monster_type.knowledge.r_blows[attack_number] < Constants.MAX_INT:
					monster_type.knowledge.r_blows[attack_number] += 1
	# If the monster teleported away after stealing, let the player know and do the actual teleport
	if blinked:
		Profile.instance.message_print("The thief flees laughing!")
		savegame.spell_effects_handler.teleport_away(monster_index, (DungeonLevel.MAX_SIGHT_RADIUS * 2) + 5)
	# If the attack just killed the player, let future generations remember what killed
	# their ancestor
	if player.is_dead and monster_type.knowledge.r_deaths < Constants.MAX_INT:
		monster_type.knowledge.r_deaths += 1
	# If the monster just got scared, let the player know
	if monster.is_visible and fear:
		Profile.instance.message_print("%s flees in terror!" % [monster_name])


func monster_check_hit_on_player(attack_power: int, monster_level: int) -> bool:
	# Straight five percent chance of hit or miss
	var k: int = Rng.roll_die(100)
	if k <= 10:
		return k <= 5
	# Otherwise, compare the power and level to the player's armour class
	var i: int = attack_power + (monster_level * 3)
	var ac: int = player.base_armour_class + player.armour_class_bonus
	return i > 0 and Rng.roll_die(i) > ac * 3 / 4


func monster_critical(dice: int, sides: int, damage: int) -> int:
	var additional_severity: int = 0
	var max_damage: int = dice * sides
	# If we did less than 95% of maximum damage, definitely no cuts or stun
	if damage < max_damage * 19 / 20:
		return 0
	# If we did less than 20 damage, then usually no cuts or stun
	if damage < 20 and Rng.roll_die(100) > damage:
		return 0
	# We're going to do a crit based on the damage done, and doing max damage increases the severity
	if damage == max_damage:
		additional_severity += 1
	# More than 20 damage increases the severity a random number of times
	if damage >= 20:
		while Rng.roll_die(100) <= 2:
			additional_severity += 1
	# Higher damage means more severe base (to which the increase is added)
	if damage > 45:
		return 6 + additional_severity
	if damage > 33:
		return 5 + additional_severity
	if damage > 25:
		return 4 + additional_severity
	if damage > 18:
		return 3 + additional_severity
	if damage > 11:
		return 2 + additional_severity
	return 1 + additional_severity
