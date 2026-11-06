# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name Player
extends Resource
## The player character

const REGEN_FAINT: int = 33
const REGEN_HP_BASE: int = 1442
const REGEN_PP_BASE: int = 524
const REGEN_NORMAL: int = 197
const REGEN_WEAK: int = 98
const PENETRATE_INVULNERABILITY: int = 13

@export var ability_scores: Array[AbilityScore]
@export var genome: Genome
@export var history: Array[String]
@export var inventory: Inventory
@export var max_dlv: Array[int]
@export var player_hp: Array[int]
@export var age: int
@export var ammunition_item_category: int
@export var armour_class_bonus: int
@export var attack_bonus: int
@export var base_armour_class: int
@export var birth_ancestry: String
@export var current_profession: String
@export var current_ancestry: String
@export var damage_bonus: int
@export var displayed_armour_class_bonus: int
@export var displayed_attack_bonus: int
@export var displayed_base_armour_class: int
@export var displayed_damage_bonus: int
@export var energy: int
@export var experience_multiplier: int
@export var experience_points: int
@export var food: int
@export var fractional_experience_points: int
@export var fractional_health: int
@export var fractional_power: int
@export var game_time: GameTime
@export var gender: Gender
@export var gender_index: int
@export var generation: int
@export var get_first_level_mutation: bool
@export var gold: int
@export var goo_patron: Patron
@export var has_acid_immunity: bool
@export var has_acid_resistance: bool
@export var has_aggravation: bool
@export var has_anti_magic: bool
@export var has_anti_teleport: bool
@export var has_anti_theft: bool
@export var has_blessed_blade: bool
@export var has_blindness_resistance: bool
@export var has_chaos_resistance: bool
@export var has_cold_immunity: bool
@export var has_cold_resistance: bool
@export var has_confusing_touch: bool
@export var has_confusion_resistance: bool
@export var has_dark_resistance: bool
@export var has_disenchant_resistance: bool
@export var has_elemental_vulnerability: bool
@export var has_experience_drain: bool
@export var has_extra_might: bool
@export var has_fear_resistance: bool
@export var has_feather_fall: bool
@export var has_fire_immunity: bool
@export var has_fire_resistance: bool
@export var has_fire_shield: bool
@export var has_free_action: bool
@export var has_glow: bool
@export var has_heavy_bow: bool
@export var has_heavy_weapon: bool
@export var has_heavy_armour: bool
@export var has_hold_life: bool
@export var has_lightning_immunity: bool
@export var has_lightning_resistance: bool
@export var has_lightning_shield: bool
@export var has_light_resistance: bool
@export var has_nether_resistance: bool
@export var has_nexus_resistance: bool
@export var has_poison_resistance: bool
@export var has_quake_weapon: bool
@export var has_random_teleport: bool
@export var has_reflection: bool
@export var has_regeneration: bool
@export var has_restricting_armour: bool
@export var has_restricting_gloves: bool
@export var has_see_invisibility: bool
@export var has_shard_resistance: bool
@export var has_slow_digestion: bool
@export var has_sound_resistance: bool
@export var has_sustain_charisma: bool
@export var has_sustain_constitution: bool
@export var has_sustain_dexterity: bool
@export var has_sustain_intelligence: bool
@export var has_sustain_strength: bool
@export var has_sustain_wisdom: bool
@export var has_telepathy: bool
@export var has_time_resistance: bool
@export var has_unpriestly_weapon: bool
@export var health: int
@export var height: int
@export var hit_die: int
@export var infravision_range: int
@export var is_dead: bool
@export var is_searching: bool
@export var is_winner: bool
@export var is_wizard: bool
@export var level: int
@export var light_level: int
@export var map_row: int
@export var map_col: int
@export var max_experience_gained: int
@export var max_health: int
@export var max_level_gained: int
@export var max_power: int
@export var melee_attacks_per_round: int
@export var missile_attacks_per_round: int
@export var name: String
@export var notice_flags: FlagSet
@export var old_heavy_bow: bool
@export var old_heavy_weapon: bool
@export var old_light_level: int
@export var old_restricting_armour: bool
@export var old_restricting_gloves: bool
@export var old_spare_spell_slots: int
@export var old_unpriestly_weapon: bool
@export var old_heavy_armour: bool
@export var profession: Profession
@export var ancestry: Ancestry
@export var realm_1: int
@export var realm_2: int
@export var redraw_needed: FlagSet
@export var religion: Religion
@export var skill_digging: int
@export var skill_disarm_traps: int
@export var skill_melee: int
@export var skill_ranged: int
@export var skill_saving_throw: int
@export var skill_search_frequency: int
@export var skill_searching: int
@export var skill_stealth: int
@export var skill_throwing: int
@export var skill_use_device: int
@export var social_class: int
@export var spare_spell_slots: int
@export var speed: int
@export var spellcasting: Spellcasting
@export var timed_acid_resistance: int
@export var timed_bleeding: int
@export var timed_blessing: int
@export var timed_blindness: int
@export var timed_cold_resistance: int
@export var timed_confusion: int
@export var timed_etherealness: int
@export var timed_fear: int
@export var timed_fire_resistance: int
@export var timed_hallucinations: int
@export var timed_haste: int
@export var timed_heroism: int
@export var timed_infravision: int
@export var timed_invulnerability: int
@export var timed_lightning_resistance: int
@export var timed_paralysis: int
@export var timed_poison: int
@export var timed_poison_resistance: int
@export var timed_protection_from_evil: int
@export var timed_see_invisibility: int
@export var timed_slow: int
@export var timed_stoneskin: int
@export var timed_stun: int
@export var timed_super_heroism: int
@export var timed_telepathy: int
@export var town_with_house: int
@export var updates_needed: FlagSet
@export var power: int
@export var weight: int
@export var weight_carried: int
@export var wilderness_row: int
@export var wilderness_col: int
@export var word_of_recall_delay: int


static func new_player() -> Player:
	var player: Player = Player.new()
	player.updates_needed = FlagSet.new()
	player.redraw_needed = FlagSet.new()
	player.notice_flags = FlagSet.new()
	player.genome = Genome.new()
	for i: int in 6:
		player.ability_scores.append(AbilityScore.new())
	player.inventory = Inventory.new()
	player.inventory.player = player
	player.religion = Religion.new()
	player.food = Constants.FOOD_FULL - 1
	player.is_wizard = false
	player.is_winner = false
	player.town_with_house = -1
	player.generation = 1
	return player


func lose_experience(amount: int) -> void:
	if amount > experience_points:
		amount = experience_points
	experience_points -= amount
	check_experience()


func polymorph_self() -> void:
	var more_effects: bool = true
	Profile.instance.message_print("You feel a change coming over you...")
	for effect: int in Rng.roll_die(2):
		if not more_effects:
			break
		match Rng.roll_die(12):
			1, 2, 3:
				polymorph_wounds()
			4, 5, 6:
				genome.gain_mutation()
			7:
				var new_ancestry: String = current_ancestry
				var list: Array[String] = Ancestry.sorted_ancestry_names()
				while new_ancestry == current_ancestry:
					var choice: int = Rng.roll_die(list.size()) - 1
					new_ancestry = list[choice]
				var an: String = Ancestry.named_ancestry(new_ancestry).article
				Profile.instance.message_print(
					"You turn into %s %s!" % [an, Ancestry.named_ancestry(new_ancestry).title]
				)
				change_ancestry(new_ancestry)
				SaveGame.instance.dungeon_level.redraw_single_location(map_row, map_col)
				more_effects = false
			8:
				Profile.instance.message_print("You polymorph into an abomination!")
				for tmp: int in 6:
					decrease_ability_score(tmp, Rng.roll_die(6), Rng.roll_die(3) == 1)
					tmp += 1
				if Rng.roll_die(6) == 1:
					Profile.instance.message_print(
						"You find living difficult in your present form!"
					)
					take_hit(Rng.roll_dice(Rng.roll_die(level), level), "a lethal mutation")
				shuffle_ability_scores()
			_:
				shuffle_ability_scores()


func polymorph_wounds() -> void:
	var wounds: int = timed_bleeding
	var hit_points: int = max_health - health
	var change: int = Rng.roll_dice(level, 5)
	var nastyEffect: bool = Rng.roll_die(5) == 1
	if not (wounds != 0 or hit_points != 0 or nastyEffect):
		return
	if nastyEffect:
		Profile.instance.message_print("A new wound was created!")
		take_hit(change, "a polymorphed wound")
		set_timed_bleeding(change)
	else:
		Profile.instance.message_print("Your wounds are polymorphed into less serious ones.")
		restore_health(change)
		set_timed_bleeding(timed_bleeding - (change / 2))


func reroll_hit_points() -> void:
	player_hp[0] = hit_die
	var last_roll: int = hit_die
	for i: int in Constants.MAX_EXPERIENCE_LEVEL:
		player_hp[i] = last_roll
		last_roll -= 1
		if last_roll < 1:
			last_roll = hit_die
	for i: int in Constants.MAX_EXPERIENCE_LEVEL:
		var j: int = Rng.roll_die(Constants.MAX_EXPERIENCE_LEVEL - 1)
		last_roll = player_hp[i]
		player_hp[i] = player_hp[j]
		player_hp[j] = last_roll
	for i: int in Constants.MAX_EXPERIENCE_LEVEL:
		player_hp[i] = player_hp[i - 1] + player_hp[i]
	updates_needed.set_bits(UpdateFlags.UPDATE_HEALTH)
	redraw_needed.set_bits(RedrawFlags.REDRAW_HP)
	SaveGame.instance.notice_update_redraw()


func curse_equipment(chance: int, heavy_chance: int) -> void:
	var curse_changed: bool = false
	var o_1: FlagSet = FlagSet.new()
	var o_2: FlagSet = FlagSet.new()
	var o_3: FlagSet = FlagSet.new()
	var item: Item = inventory.items[InventorySlot.MELEE_WEAPON - 1 + Rng.roll_die(12)]
	if Rng.roll_die(100) > chance:
		return
	if item.item_type == null:
		return
	item.get_merged_flags(o_1, o_2, o_3)
	if o_3.is_set(ItemFlags3.BLESSED) && Rng.roll_die(888) > chance:
		var item_name: String = item.description(false, 0)
		var s: String = "" if item.count > 1 else "s"
		Profile.instance.message_print("Your %s resist%s cursing!" % [item_name, s])
		return
	if (
		Rng.roll_die(100) <= heavy_chance
		and (item.artifact_index != 0 or item.rare_index != 0 or item.is_legendary())
	):
		if o_3.is_clear(ItemFlags3.HEAVY_CURSE):
			curse_changed = true
		item.legendary_flags_3.set_bits(ItemFlags3.HEAVY_CURSE)
		item.legendary_flags_3.set_bits(ItemFlags3.CURSED)
		item.identify_flags.set_bits(IdentifyFlags.CURSED)
	else:
		if item.identify_flags.is_clear(IdentifyFlags.CURSED):
			curse_changed = true
		item.legendary_flags_3.set_bits(ItemFlags3.CURSED)
		item.identify_flags.set_bits(IdentifyFlags.CURSED)
	if curse_changed:
		Profile.instance.message_print("There is a malignant black aura surrounding you...")
	if item.inscription == "uncursed":
		item.inscription = ""


func change_ancestry(new_ancestry: String) -> void:
	current_ancestry = new_ancestry
	ancestry = Ancestry.named_ancestry(current_ancestry)
	experience_multiplier = ancestry.experience_factor + profession.experience_factor
	if gender_index == GenderType.MALE:
		weight = Rng.random_normal(ancestry.male_base_weight, ancestry.male_weight_range)
		height = Rng.random_normal(ancestry.male_base_height, ancestry.male_height_range)
	elif gender_index == GenderType.FEMALE:
		weight = Rng.random_normal(ancestry.female_base_weight, ancestry.female_weight_range)
		height = Rng.random_normal(ancestry.female_base_height, ancestry.female_height_range)
	else:
		if Rng.roll_die(2) == 1:
			weight = Rng.random_normal(ancestry.male_base_weight, ancestry.male_weight_range)
			height = Rng.random_normal(ancestry.male_base_height, ancestry.male_height_range)
		else:
			weight = Rng.random_normal(ancestry.female_base_weight, ancestry.female_weight_range)
			height = Rng.random_normal(ancestry.female_base_height, ancestry.female_height_range)
	check_experience()
	max_level_gained = level
	redraw_needed.set_bits(RedrawFlags.REDRAW_BASIC)
	updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
	SaveGame.instance.notice_update_redraw()


func get_abilities_as_item_flags(f1: FlagSet, f2: FlagSet, f3: FlagSet) -> void:
	f1.clear_all()
	f2.clear_all()
	f3.clear_all()
	profession.get_abilities_as_item_flags(self, f1, f2, f3)
	ancestry.get_abilities_as_item_flags(self, f1, f2, f3)
	if genome.regen:
		f3.set_bits(ItemFlags3.REGEN)
	if genome.suppress_regen:
		f3.clear_bits(ItemFlags3.REGEN)
	if genome.speed_bonus != 0:
		f1.set_bits(ItemFlags1.SPEED)
	if genome.elec_hit:
		f3.set_bits(ItemFlags3.SH_ELEC)
	if genome.fire_hit:
		f3.set_bits(ItemFlags3.SH_FIRE)
		f3.set_bits(ItemFlags3.LIGHT_SOURCE)
	if genome.feather_fall:
		f3.set_bits(ItemFlags3.FEATHER)
	if genome.res_fear:
		f2.set_bits(ItemFlags2.RES_FEAR)
	if genome.esp:
		f3.set_bits(ItemFlags3.TELEPATHY)
	if genome.free_action:
		f2.set_bits(ItemFlags2.FREE_ACT)
	if genome.sustain_all:
		f2.set_bits(ItemFlags2.SUST_CON)
		if level > 9:
			f2.set_bits(ItemFlags2.SUST_STR)
		if level > 19:
			f2.set_bits(ItemFlags2.SUST_DEX)
		if level > 29:
			f2.set_bits(ItemFlags2.SUST_WIS)
		if level > 39:
			f2.set_bits(ItemFlags2.SUST_INT)
		if level > 49:
			f2.set_bits(ItemFlags2.SUST_CHA)


func input_player_name() -> void:
	Console.clear_from(42)
	Console.print_centred(
		43,
		"[Enter your player's name above, or hit ESCAPE to leave it unchanged]",
		Ink.ORANGE,
	)
	var input: Dictionary[bool, String] = User.get_input(2, 15, name, 12)
	if input.has(true):
		if name.strip_edges() != input[true].strip_edges():
			generation = 1
		name = input[true].rpad(12)
	Console.print_at(2, 15, name, Ink.BROWN)


func sense_inventory() -> void:
	if timed_confusion != 0:
		return
	if not profession.try_sense_inventory(level):
		return
	var detailed: bool = profession.has_detailed_sense_inventory
	for i: int in InventorySlot.TOTAL:
		var okay: bool = false
		var item: Item = inventory.items[i]
		if item.item_type == null:
			continue
		match item.category:
			ItemCategory.SHOT, \
					ItemCategory.ARROW, \
					ItemCategory.BOLT, \
					ItemCategory.BOW, \
					ItemCategory.DIGGING, \
					ItemCategory.HAFTED, \
					ItemCategory.POLEARM, \
					ItemCategory.SWORD, \
					ItemCategory.BOOTS, \
					ItemCategory.GLOVES, \
					ItemCategory.HELM, \
					ItemCategory.CROWN, \
					ItemCategory.SHIELD, \
					ItemCategory.CLOAK, \
					ItemCategory.SOFT_ARMOUR, \
					ItemCategory.HARD_ARMOUR, \
					ItemCategory.DRAGON_ARMOUR:
				okay = true
			ItemCategory.LIGHT: # Only orbs
				if item.sub_category == LightType.ORB:
					okay = true
		if not okay:
			continue
		if item.identify_flags.is_set(IdentifyFlags.SENSED):
			continue
		if item.is_known():
			continue
		if i < InventorySlot.MELEE_WEAPON and Rng.roll_die(5) != 1:
			continue
		var feel: String = item.get_detailed_feeling() if detailed else item.get_vague_feeling()
		if feel == "":
			continue
		var item_name: String = item.description(false, 0)
		var is_are: String = "is" if item.count == 1 else "are"
		if i >= InventorySlot.MELEE_WEAPON:
			Profile.instance.message_print(
				"You feel the %s (%s) you are %s %s %s..."
				% [item_name, Util.index_to_label(i), describe_wield_location(i), is_are, feel],
			)
		else:
			Profile.instance.message_print(
				"You feel the %s (%s) in your pack %s %s..."
				% [item_name, Util.index_to_label(i), is_are, feel],
			)
		item.identify_flags.set_bits(IdentifyFlags.SENSED)
		if item.inscription == "":
			item.inscription = feel
		notice_flags.set_bits(NoticeFlags.NOTICE_COMBINE | NoticeFlags.NOTICE_REORDER)


func describe_wield_location(index: int) -> String:
	var p: String
	match index:
		InventorySlot.MELEE_WEAPON:
			p = "attacking monsters with"
		InventorySlot.RANGED_WEAPON:
			p = "shooting missiles with"
		InventorySlot.LEFT_HAND:
			p = "wearing on your left hand"
		InventorySlot.RIGHT_HAND:
			p = "wearing on your right hand"
		InventorySlot.NECK:
			p = "wearing around your neck"
		InventorySlot.LIGHT_SOURCE:
			p = "using to light the way"
		InventorySlot.BODY:
			p = "wearing on your body"
		InventorySlot.CLOAK:
			p = "wearing on your back"
		InventorySlot.ARM:
			p = "wearing on your arm"
		InventorySlot.HEAD:
			p = "wearing on your head"
		InventorySlot.HANDS:
			p = "wearing on your hands"
		InventorySlot.FEET:
			p = "wearing on your feet"
		_:
			p = "carrying in your pack"
	if index == InventorySlot.MELEE_WEAPON:
		var item: Item = inventory.items[index]
		if ability_scores[Ability.STRENGTH].str_max_weapon_weight() < item.weight / 10:
			p = "just lifting"
	if index == InventorySlot.RANGED_WEAPON:
		var item: Item = inventory.items[index]
		if ability_scores[Ability.STRENGTH].str_max_weapon_weight() < item.weight / 10:
			p = "just holding"
	return p


func set_food(new_food: int) -> bool:
	var old_level: int
	var new_level: int
	var notice: bool
	new_food = clampi(new_food, 0, 20000)
	if food < Constants.FOOD_FAINT:
		old_level = 0
	elif food < Constants.FOOD_WEAK:
		old_level = 1
	elif food < Constants.FOOD_ALERT:
		old_level = 2
	elif food < Constants.FOOD_FULL:
		old_level = 3
	elif food < Constants.FOOD_GORGED:
		old_level = 4
	else:
		old_level = 5
	if new_food < Constants.FOOD_FAINT:
		new_level = 0
	elif new_food < Constants.FOOD_WEAK:
		new_level = 1
	elif new_food < Constants.FOOD_ALERT:
		new_level = 2
	elif new_food < Constants.FOOD_FULL:
		new_level = 3
	elif new_food < Constants.FOOD_GORGED:
		new_level = 4
	else:
		new_level = 5
	if new_level > old_level:
		match new_level:
			1:
				Profile.instance.message_print("You are still weak.")
			2:
				Profile.instance.message_print("You are still hungry.")
			3:
				Profile.instance.message_print("You are no longer hungry.")
			4:
				Profile.instance.message_print("You are full!")
			5:
				Profile.instance.message_print("You have gorged yourself!")
		notice = true
	elif new_level < old_level:
		match new_level:
			0:
				Profile.instance.message_print("You are getting faint from hunger!")
			1:
				Profile.instance.message_print("You are getting weak from hunger!")
			2:
				Profile.instance.message_print("You are getting hungry.")
			3:
				Profile.instance.message_print("You are no longer full.")
			4:
				Profile.instance.message_print("You are no longer gorged.")
		notice = true
	food = new_food
	if not notice:
		return false
	SaveGame.instance.disturb(false)
	updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
	redraw_needed.set_bits(RedrawFlags.REDRAW_HUNGER)
	SaveGame.instance.notice_update_redraw()
	return true


## Return the player's current (or possibly posthumous) score
func get_score(savegame: SaveGame) -> int:
	# 100pts per level
	var score: int = (max_level_gained - 1) * 100
	# 10pts per dungeon difficulty reached (per dungeon)
	var dungeons: Array[Dungeon] = savegame.dungeons
	for i: int in dungeons.size():
		if max_dlv[i] > 0:
			score += ((max_dlv[i] + dungeons[i].offset) * 10)
	# 100pts per quest completed
	for quest: Quest in savegame.quests.quests:
		if quest.depth == 0:
			score += 100
	# Bonus 1,000pts for completing all quests
	if is_winner:
		score += 1000
	# 0-99pts for percentage of way to next level
	if max_level_gained < 50:
		var prev: int = 0
		if max_level_gained > 1:
			prev = Tables.BASE_LEVEL_THRESHOLDS[max_level_gained - 2] * experience_multiplier / 100
		var next: int = Tables.BASE_LEVEL_THRESHOLDS[max_level_gained - 1] * experience_multiplier / 100
		var numerator: int = max_experience_gained - prev
		var denominator: int = next - prev
		var fraction: int = 100 * numerator / denominator
		score += fraction
	# Return final score (from 0-23,680pts)
	return score


func gain_experience(amount: int) -> void:
	experience_points += amount
	if experience_points < max_experience_gained:
		max_experience_gained += amount / 5
	check_experience()


func check_experience() -> void:
	var level_reward: bool = false
	var level_mutation: bool = false
	var increased_level: bool = false
	experience_points = clampi(experience_points, 0, Constants.MAX_EXPERIENCE)
	max_experience_gained = clampi(max_experience_gained, 0, Constants.MAX_EXPERIENCE)
	max_experience_gained = maxi(experience_points, max_experience_gained)
	redraw_needed.set_bits(RedrawFlags.REDRAW_EXP)
	SaveGame.instance.notice_update_redraw()
	while (
		level > 1
		and experience_points
		< Tables.BASE_LEVEL_THRESHOLDS[level - 2] * experience_multiplier / 100
	):
		level -= 1
		SaveGame.instance.dungeon_level.redraw_single_location(map_row, map_col)
		updates_needed.set_bits(
			UpdateFlags.UPDATE_BONUSES | UpdateFlags.UPDATE_HEALTH
			| UpdateFlags.UPDATE_POWER | UpdateFlags.UPDATE_SPELLS
		)
		redraw_needed.set_bits(RedrawFlags.REDRAW_LEV | RedrawFlags.REDRAW_TITLE)
		SaveGame.instance.notice_update_redraw()
	while (
		level < Constants.MAX_EXPERIENCE_LEVEL
		and experience_points
		>= Tables.BASE_LEVEL_THRESHOLDS[level - 1] * experience_multiplier / 100
	):
		level += 1
		if not increased_level:
			Console.play_sound("LevelUp")
			increased_level = true
		SaveGame.instance.dungeon_level.redraw_single_location(map_row, map_col)
		if level > max_level_gained:
			max_level_gained = level
			if profession.has_goo_patron:
				level_reward = true
			if genome.chaos_gift:
				level_reward = true
			if ancestry.mutates:
				if Rng.roll_die(5) == 1:
					level_mutation = true
		Profile.instance.message_print("Welcome to level %d." % [level])
		updates_needed.set_bits(
			UpdateFlags.UPDATE_BONUSES | UpdateFlags.UPDATE_HEALTH
			| UpdateFlags.UPDATE_POWER | UpdateFlags.UPDATE_SPELLS
		)
		redraw_needed.set_bits(
			RedrawFlags.REDRAW_EXP | RedrawFlags.REDRAW_LEV | RedrawFlags.REDRAW_TITLE
		)
		SaveGame.instance.notice_update_redraw()
		if level_reward:
			gain_level_reward()
			level_reward = false
		if level_mutation:
			Profile.instance.message_print("You feel different...")
			genome.gain_mutation()
			level_mutation = false


func gain_level_reward() -> void:
	goo_patron.get_reward(SaveGame.instance, self, SaveGame.instance.dungeon_level)


func regenerate_health(percent: int) -> void:
	var old_health: int = health
	var new_health: int = (max_health * percent) + REGEN_HP_BASE
	health += new_health >> 16
	if health < 0 and old_health > 0:
		health = Constants.MAX_INT - 1
	var new_fractional_health: int = (new_health & 0xFFFF) + fractional_health
	if new_fractional_health >= 0x10000:
		fractional_health = new_fractional_health - 0x10000
		health += 1
	else:
		fractional_health = new_fractional_health
	if health >= max_health:
		health = max_health
		fractional_health = 0
	if old_health != health:
		redraw_needed.set_bits(RedrawFlags.REDRAW_HP)


func regenerate_power(percent: int) -> void:
	var old_power: int = power
	var new_power: int = (max_power * percent) + REGEN_PP_BASE
	power += new_power >> 16
	if power < 0 and old_power > 0:
		power = Constants.MAX_INT - 1
	var new_fractional_power: int = (new_power & 0xFFFF) + fractional_power
	if new_fractional_power >= 0x10000:
		fractional_power = new_fractional_power - 0x10000
		power += 1
	else:
		fractional_power = new_fractional_power
	if power >= max_power:
		power = max_power
		fractional_power = 0
	if old_power != power:
		redraw_needed.set_bits(RedrawFlags.REDRAW_POWER)


func take_hit(damage: int, hit_from: String) -> void:
	var pen_invuln: bool = false
	var warning: int = max_health * Constants.HIT_POINT_WARNING / 10
	if is_dead:
		return
	SaveGame.instance.disturb(true)
	if timed_invulnerability != 0 and damage < 9000:
		if Rng.roll_die(PENETRATE_INVULNERABILITY) == 1:
			pen_invuln = true
		else:
			return
	if timed_etherealness != 0:
		damage /= 10
		if damage == 0 and Rng.roll_die(10) == 1:
			damage = 1
	health -= damage
	redraw_needed.set_bits(RedrawFlags.REDRAW_HP)
	if pen_invuln:
		Profile.instance.message_print("The attack penetrates your shield of invulnerability!")
	if health < 0:
		if Rng.roll_die(10) <= religion.gods[GodName.ZO_KALAR].adjusted_favour():
			Profile.instance.message_print("Zo-Kalar's favour saves you from death!")
			health += damage
		else:
			if is_wizard and not User.get_check("Die? "):
				health += damage
			else:
				Console.play_sound("PlayerDeath")
				Profile.instance.message_print("You die.")
				Profile.instance.message_print()
				SaveGame.instance.died_from = hit_from
				if timed_hallucinations != 0:
					SaveGame.instance.died_from += "(?)"
				is_winner = false
				is_dead = true
				return
	if health < warning:
		Console.play_sound("HealthWarning")
		Profile.instance.message_print("*** LOW HITPOINT WARNING! ***")
		Profile.instance.message_print()


func set_timed_acid_resistance(v: int) -> void:
	var notice: bool = false
	v = clampi(v, 0, 10000)
	if v != 0:
		if timed_acid_resistance == 0:
			Profile.instance.message_print("You feel resistant to acid!")
			notice = true
	else:
		if timed_acid_resistance != 0:
			Profile.instance.message_print("You feel less resistant to acid.")
			notice = true
	timed_acid_resistance = v
	if not notice:
		return
	SaveGame.instance.disturb(false)
	SaveGame.instance.notice_update_redraw()


func set_timed_bleeding(v: int) -> bool:
	var old_value: int
	var new_value: int
	var notice: bool = false
	v = clampi(v, 0, 10000)
	if ancestry.doesnt_bleed(self):
		v = 0
	if timed_bleeding > 1000:
		old_value = 7
	elif timed_bleeding > 200:
		old_value = 6
	elif timed_bleeding > 100:
		old_value = 5
	elif timed_bleeding > 50:
		old_value = 4
	elif timed_bleeding > 25:
		old_value = 3
	elif timed_bleeding > 10:
		old_value = 2
	elif timed_bleeding > 0:
		old_value = 1
	else:
		old_value = 0
	if v > 1000:
		new_value = 7
	elif v > 200:
		new_value = 6
	elif v > 100:
		new_value = 5
	elif v > 50:
		new_value = 4
	elif v > 25:
		new_value = 3
	elif v > 10:
		new_value = 2
	elif v > 0:
		new_value = 1
	else:
		new_value = 0
	if new_value > old_value:
		match new_value:
			1:
				Profile.instance.message_print("You have been given a graze.")
			2:
				Profile.instance.message_print("You have been given a light cut.")
			3:
				Profile.instance.message_print("You have been given a bad cut.")
			4:
				Profile.instance.message_print("You have been given a nasty cut.")
			5:
				Profile.instance.message_print("You have been given a severe cut.")
			6:
				Profile.instance.message_print("You have been given a deep gash.")
			7:
				Profile.instance.message_print("You have been given a mortal wound.")
		notice = true
		if Rng.roll_die(1000) < v or Rng.roll_die(16) == 1:
			if not has_sustain_charisma:
				Profile.instance.message_print("You have been horribly scarred.")
				try_decreasing_ability_score(Ability.CHARISMA)
	elif new_value < old_value:
		match new_value:
			0:
				Profile.instance.message_print("You are no longer bleeding.")
				SaveGame.instance.disturb(false)
		notice = true
	timed_bleeding = v
	if not notice:
		return false
	SaveGame.instance.disturb(false)
	updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
	redraw_needed.set_bits(RedrawFlags.REDRAW_CUT)
	SaveGame.instance.notice_update_redraw()
	return true


func set_timed_blessing(v: int) -> bool:
	var notice: bool = false
	v = clampi(v, 0, 10000)
	if v != 0:
		if timed_blessing == 0:
			Profile.instance.message_print("You feel righteous!")
			notice = true
	else:
		if timed_blessing != 0:
			Profile.instance.message_print("The prayer has expired.")
			notice = true
	timed_blessing = v
	if not notice:
		return false
	SaveGame.instance.disturb(false)
	updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
	SaveGame.instance.notice_update_redraw()
	return true


func set_timed_blindness(v: int) -> bool:
	var notice: bool = false
	v = clampi(v, 0, 10000)
	if v != 0:
		if timed_blindness == 0:
			Profile.instance.message_print("You are blind!")
			notice = true
	else:
		if timed_blindness != 0:
			Profile.instance.message_print("You can see again.")
			notice = true
	timed_blindness = v
	if not notice:
		return false
	SaveGame.instance.disturb(false)
	updates_needed.set_bits(UpdateFlags.UPDATE_REMOVE_VIEW | UpdateFlags.UPDATE_REMOVE_LIGHT)
	updates_needed.set_bits(UpdateFlags.UPDATE_VIEW | UpdateFlags.UPDATE_LIGHT)
	updates_needed.set_bits(UpdateFlags.UPDATE_MONSTERS)
	redraw_needed.set_bits(RedrawFlags.REDRAW_MAP)
	redraw_needed.set_bits(RedrawFlags.REDRAW_BLIND)
	SaveGame.instance.notice_update_redraw()
	return true


func set_timed_cold_resistance(v: int) -> bool:
	var notice: bool = false
	v = clampi(v, 0, 10000)
	if v != 0:
		if timed_cold_resistance == 0:
			Profile.instance.message_print("You feel resistant to cold!")
			notice = true
	else:
		if timed_cold_resistance != 0:
			Profile.instance.message_print("You feel less resistant to cold.")
			notice = true
	timed_cold_resistance = v
	if not notice:
		return false
	SaveGame.instance.disturb(false)
	SaveGame.instance.notice_update_redraw()
	return true


func set_timed_confusion(v: int) -> bool:
	var notice: bool = false
	v = clampi(v, 0, 10000)
	if v != 0:
		if timed_confusion == 0:
			Profile.instance.message_print("You are confused!")
			notice = true
	else:
		if timed_confusion != 0:
			Profile.instance.message_print("You feel less confused now.")
			notice = true
	timed_confusion = v
	if not notice:
		return false
	SaveGame.instance.disturb(false)
	redraw_needed.set_bits(RedrawFlags.REDRAW_CONFUSED)
	SaveGame.instance.notice_update_redraw()
	return true


func set_timed_etherealness(v: int) -> void:
	var notice: bool = false
	v = clampi(v, 0, 10000)
	if v != 0:
		if timed_etherealness == 0:
			Profile.instance.message_print(
				"You leave the physical world and turn into a wraith-being!"
			)
			notice = true
			redraw_needed.set_bits(RedrawFlags.REDRAW_MAP)
			updates_needed.set_bits(UpdateFlags.UPDATE_MONSTERS)
	else:
		if timed_etherealness != 0:
			Profile.instance.message_print("You feel solid once more.")
			notice = true
			redraw_needed.set_bits(RedrawFlags.REDRAW_MAP)
			updates_needed.set_bits(UpdateFlags.UPDATE_MONSTERS)
	timed_etherealness = v
	if not notice:
		return
	SaveGame.instance.disturb(false)
	updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
	SaveGame.instance.notice_update_redraw()


func set_timed_fear(v: int) -> bool:
	var notice: bool = false
	v = clampi(v, 0, 10000)
	if v != 0:
		if timed_fear == 0:
			Profile.instance.message_print("You are terrified!")
			notice = true
	else:
		if timed_fear != 0:
			Profile.instance.message_print("You feel bolder now.")
			notice = true
	timed_fear = v
	if not notice:
		return false
	SaveGame.instance.disturb(false)
	redraw_needed.set_bits(RedrawFlags.REDRAW_AFRAID)
	SaveGame.instance.notice_update_redraw()
	return true


func set_timed_fire_resistance(v: int) -> bool:
	var notice: bool = false
	v = clampi(v, 0, 10000)
	if v != 0:
		if timed_fire_resistance == 0:
			Profile.instance.message_print("You feel resistant to fire!")
			notice = true
	else:
		if timed_fire_resistance != 0:
			Profile.instance.message_print("You feel less resistant to fire.")
			notice = true
	timed_fire_resistance = v
	if not notice:
		return false
	SaveGame.instance.disturb(false)
	SaveGame.instance.notice_update_redraw()
	return true


func set_timed_hallucinations(v: int) -> bool:
	var notice: bool = false
	v = clampi(v, 0, 10000)
	if v != 0:
		if timed_hallucinations == 0:
			Profile.instance.message_print("Oh, wow! Everything looks so cosmic now!")
			notice = true
	else:
		if timed_hallucinations != 0:
			Profile.instance.message_print("You can see clearly again.")
			notice = true
	timed_hallucinations = v
	if not notice:
		return false
	SaveGame.instance.disturb(false)
	redraw_needed.set_bits(RedrawFlags.REDRAW_MAP)
	updates_needed.set_bits(UpdateFlags.UPDATE_MONSTERS)
	SaveGame.instance.notice_update_redraw()
	return true


func set_timed_haste(v: int) -> bool:
	var notice: bool = false
	v = clampi(v, 0, 10000)
	if v != 0:
		if timed_haste == 0:
			Profile.instance.message_print("You feel yourself moving faster!")
			notice = true
	else:
		if timed_haste != 0:
			Profile.instance.message_print("You feel yourself slow down.")
			notice = true
	timed_haste = v
	if not notice:
		return false
	SaveGame.instance.disturb(false)
	updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
	SaveGame.instance.notice_update_redraw()
	return true


func set_timed_heroism(v: int) -> bool:
	var notice: bool = false
	v = clampi(v, 0, 10000)
	if v != 0:
		if timed_heroism == 0:
			Profile.instance.message_print("You feel like a hero!")
			notice = true
	else:
		if timed_heroism != 0:
			Profile.instance.message_print("The heroism wears off.")
			notice = true
	timed_heroism = v
	if not notice:
		return false
	SaveGame.instance.disturb(false)
	updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
	updates_needed.set_bits(UpdateFlags.UPDATE_HEALTH)
	SaveGame.instance.notice_update_redraw()
	return true


func set_timed_infravision(v: int) -> bool:
	var notice: bool = false
	v = clampi(v, 0, 10000)
	if v != 0:
		if timed_infravision == 0:
			Profile.instance.message_print("Your eyes begin to tingle!")
			notice = true
	else:
		if timed_infravision != 0:
			Profile.instance.message_print("Your eyes stop tingling.")
			notice = true
	timed_infravision = v
	if not notice:
		return false
	SaveGame.instance.disturb(false)
	updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
	updates_needed.set_bits(UpdateFlags.UPDATE_MONSTERS)
	SaveGame.instance.notice_update_redraw()
	return true


func set_timed_invulnerability(v: int) -> void:
	var notice: bool = false
	v = clampi(v, 0, 10000)
	if v != 0:
		if timed_invulnerability == 0:
			Profile.instance.message_print("Invulnerability!")
			notice = true
			redraw_needed.set_bits(RedrawFlags.REDRAW_MAP)
			updates_needed.set_bits(UpdateFlags.UPDATE_MONSTERS)
	else:
		if timed_invulnerability != 0:
			Profile.instance.message_print("The invulnerability wears off.")
			notice = true
			redraw_needed.set_bits(RedrawFlags.REDRAW_MAP)
			updates_needed.set_bits(UpdateFlags.UPDATE_MONSTERS)
	timed_invulnerability = v
	if not notice:
		return
	SaveGame.instance.disturb(false)
	updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
	SaveGame.instance.notice_update_redraw()


func set_timed_lightning_resistance(v: int) -> void:
	var notice: bool = false
	v = clampi(v, 0, 10000)
	if v != 0:
		if timed_lightning_resistance == 0:
			Profile.instance.message_print("You feel resistant to electricity!")
			notice = true
	else:
		if timed_lightning_resistance != 0:
			Profile.instance.message_print("You feel less resistant to electricity.")
			notice = true
	timed_lightning_resistance = v
	if not notice:
		return
	SaveGame.instance.disturb(false)
	SaveGame.instance.notice_update_redraw()


func set_timed_paralysis(v: int) -> bool:
	var notice: bool = false
	v = clampi(v, 0, 10000)
	if v != 0:
		if timed_paralysis == 0:
			Profile.instance.message_print("You are paralyzed!")
			notice = true
	else:
		if timed_paralysis != 0:
			Profile.instance.message_print("You can move again.")
			notice = true
	timed_paralysis = v
	if not notice:
		return false
	SaveGame.instance.disturb(false)
	redraw_needed.set_bits(RedrawFlags.REDRAW_STATE)
	SaveGame.instance.notice_update_redraw()
	return true


func set_timed_poison(v: int) -> bool:
	var notice: bool = false
	v = clampi(v, 0, 10000)
	if v != 0:
		if timed_poison == 0:
			Profile.instance.message_print("You are poisoned!")
			notice = true
	else:
		if timed_poison != 0:
			Profile.instance.message_print("You are no longer poisoned.")
			notice = true
	timed_poison = v
	if not notice:
		return false
	SaveGame.instance.disturb(false)
	redraw_needed.set_bits(RedrawFlags.REDRAW_POISONED)
	SaveGame.instance.notice_update_redraw()
	return true


func set_timed_poison_resistance(v: int) -> void:
	var notice: bool = false
	v = clampi(v, 0, 10000)
	if v != 0:
		if timed_poison_resistance == 0:
			Profile.instance.message_print("You feel resistant to poison!")
			notice = true
	else:
		if timed_poison_resistance != 0:
			Profile.instance.message_print("You feel less resistant to poison.")
			notice = true
	timed_poison_resistance = v
	if not notice:
		return
	SaveGame.instance.disturb(false)
	SaveGame.instance.notice_update_redraw()


func set_timed_protection_from_evil(v: int) -> bool:
	var notice: bool = false
	v = clampi(v, 0, 10000)
	if v != 0:
		if timed_protection_from_evil == 0:
			Profile.instance.message_print("You feel safe from evil!")
			notice = true
	else:
		if timed_protection_from_evil != 0:
			Profile.instance.message_print("You no longer feel safe from evil.")
			notice = true
	timed_protection_from_evil = v
	if not notice:
		return false
	SaveGame.instance.disturb(false)
	SaveGame.instance.notice_update_redraw()
	return true


func set_timed_see_invisibility(v: int) -> bool:
	var notice: bool = false
	v = clampi(v, 0, 10000)
	if v != 0:
		if timed_see_invisibility == 0:
			Profile.instance.message_print("Your eyes feel very sensitive!")
			notice = true
	else:
		if timed_see_invisibility != 0:
			Profile.instance.message_print("Your eyes feel less sensitive.")
			notice = true
	timed_see_invisibility = v
	if not notice:
		return false
	SaveGame.instance.disturb(false)
	updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
	updates_needed.set_bits(UpdateFlags.UPDATE_MONSTERS)
	SaveGame.instance.notice_update_redraw()
	return true


func set_timed_slow(v: int) -> bool:
	var notice: bool = false
	v = clampi(v, 0, 10000)
	if v != 0:
		if timed_slow == 0:
			Profile.instance.message_print("You feel yourself moving slower!")
			notice = true
	else:
		if timed_slow != 0:
			Profile.instance.message_print("You feel yourself speed up.")
			notice = true
	timed_slow = v
	if not notice:
		return false
	SaveGame.instance.disturb(false)
	updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
	SaveGame.instance.notice_update_redraw()
	return true


func set_timed_stoneskin(v: int) -> void:
	var notice: bool = false
	v = clampi(v, 0, 10000)
	if v != 0:
		if timed_stoneskin == 0:
			Profile.instance.message_print("Your skin turns to stone.")
			notice = true
	else:
		if timed_stoneskin != 0:
			Profile.instance.message_print("Your skin returns to normal.")
			notice = true
	timed_stoneskin = v
	if not notice:
		return
	SaveGame.instance.disturb(false)
	updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
	SaveGame.instance.notice_update_redraw()


func set_timed_stun(v: int) -> bool:
	var old_value: int
	var new_value: int
	var notice: bool = false
	v = clampi(v, 0, 10000)
	if ancestry.doesnt_stun(self):
		v = 0
	if timed_stun > 100:
		old_value = 3
	elif timed_stun > 50:
		old_value = 2
	elif timed_stun > 0:
		old_value = 1
	else:
		old_value = 0
	if v > 100:
		new_value = 3
	elif v > 50:
		new_value = 2
	elif v > 0:
		new_value = 1
	else:
		new_value = 0
	if new_value > old_value:
		match new_value:
			1:
				Profile.instance.message_print("You have been stunned.")
			2:
				Profile.instance.message_print("You have been heavily stunned.")
			3:
				Profile.instance.message_print("You have been knocked out.")
		if Rng.roll_die(1000) < v or Rng.roll_die(16) == 1:
			Profile.instance.message_print("A vicious Attack hits your head.")
			if Rng.roll_die(3) == 1:
				if not has_sustain_intelligence:
					try_decreasing_ability_score(Ability.INTELLIGENCE)
				if not has_sustain_wisdom:
					try_decreasing_ability_score(Ability.WISDOM)
			elif Rng.roll_die(2) == 1:
				if not has_sustain_intelligence:
					try_decreasing_ability_score(Ability.INTELLIGENCE)
			else:
				if not has_sustain_wisdom:
					try_decreasing_ability_score(Ability.WISDOM)
		notice = true
	elif new_value < old_value:
		match new_value:
			0:
				Profile.instance.message_print("You are no longer stunned.")
				SaveGame.instance.disturb(false)
		notice = true
	timed_stun = v
	if not notice:
		return false
	SaveGame.instance.disturb(false)
	updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
	redraw_needed.set_bits(RedrawFlags.REDRAW_STUN)
	SaveGame.instance.notice_update_redraw()
	return true


func set_timed_superheroism(v: int) -> bool:
	var notice: bool = false
	v = clampi(v, 0, 10000)
	if v != 0:
		if timed_super_heroism == 0:
			Profile.instance.message_print("You feel like a killing machine!")
			notice = true
	else:
		if timed_super_heroism != 0:
			Profile.instance.message_print("You feel less Berserk.")
			notice = true
	timed_super_heroism = v
	if not notice:
		return false
	SaveGame.instance.disturb(false)
	updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
	updates_needed.set_bits(UpdateFlags.UPDATE_HEALTH)
	SaveGame.instance.notice_update_redraw()
	return true


func set_timed_telepathy(v: int) -> void:
	var notice: bool = false
	v = clampi(v, 0, 10000)
	if v != 0:
		if timed_telepathy == 0:
			Profile.instance.message_print("You feel your consciousness expand!")
			notice = true
	else:
		if timed_telepathy != 0:
			Profile.instance.message_print("Your consciousness contracts again.")
			notice = true
	timed_telepathy = v
	if not notice:
		return
	SaveGame.instance.disturb(false)
	updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
	updates_needed.set_bits(UpdateFlags.UPDATE_MONSTERS)
	SaveGame.instance.notice_update_redraw()


func try_decreasing_ability_score(stat: int) -> bool:
	var sust: bool = false
	match stat:
		Ability.STRENGTH:
			if has_sustain_strength:
				sust = true
		Ability.INTELLIGENCE:
			if has_sustain_intelligence:
				sust = true
		Ability.WISDOM:
			if has_sustain_wisdom:
				sust = true
		Ability.DEXTERITY:
			if has_sustain_dexterity:
				sust = true
		Ability.CONSTITUTION:
			if has_sustain_constitution:
				sust = true
		Ability.CHARISMA:
			if has_sustain_charisma:
				sust = true
	if sust:
		Profile.instance.message_print(
			"You feel %s for a moment, but the feeling passes." % [Tables.ABILITY_REDUCTIONS[stat]]
		)
		return true
	if Rng.roll_die(10) <= religion.gods[GodName.LOBON].adjusted_favour():
		Profile.instance.message_print(
			"You feel %s for a moment, but Lobon's favour protects you."
			% [Tables.ABILITY_REDUCTIONS[stat]]
		)
		return true
	if decrease_ability_score(stat, 10, false):
		Profile.instance.message_print("You feel very %s." % [Tables.ABILITY_REDUCTIONS[stat]])
		return true
	return false


func try_increasing_ability_score(stat: int) -> bool:
	var res: bool = restore_ability_score(stat)
	if increase_ability_score(stat):
		Profile.instance.message_print("Wow!  You feel very %s!" % [Tables.ABILITY_INCREASES[stat]])
		return true
	if res:
		Profile.instance.message_print("You feel less %s." % [Tables.ABILITY_REDUCTIONS[stat]])
		return true
	return false


func try_restoring_ability_score(stat: int) -> bool:
	if restore_ability_score(stat):
		Profile.instance.message_print("You feel less %s." % [Tables.ABILITY_REDUCTIONS[stat]])
		return true
	return false


func increase_ability_score(which: int) -> bool:
	var value: int = ability_scores[which].innate
	if value < 18 + 100:
		var gain: int = 0
		if value < 18:
			gain = 1 if Rng.roll_die(100) <= 75 else 2
			value += gain
		elif value < 18 + 98:
			gain = (((18 + 100 - value) / 2) + 3) / 2
			if gain < 1:
				gain = 1
			value += Rng.roll_die(gain) + (gain / 2)
			if value > 18 + 99:
				value = 18 + 99
		else:
			value += 1
		ability_scores[which].innate = value
		if value > ability_scores[which].innate_max:
			ability_scores[which].innate_max = value
		updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
		return true
	return false


func decrease_ability_score(stat: int, amount: int, permanent: bool) -> bool:
	var loss: int = 0
	var res: bool = false
	var cur: int = ability_scores[stat].innate
	var maxx: int = ability_scores[stat].innate_max
	var same: bool = cur == maxx
	if cur > 3:
		if cur <= 18:
			if amount > 90:
				cur -= 1
			if amount > 50:
				cur -= 1
			if amount > 20:
				cur -= 1
			cur -= 1
		else:
			loss = ((((cur - 18) / 2) + 1) / 2) + 1
			if loss < 1:
				loss = 1
			loss = (Rng.roll_die(loss) + loss) * amount / 100
			if loss < amount / 2:
				loss = amount / 2
			cur -= loss
			if cur < 18:
				cur = 18 if amount <= 20 else 17
		if cur < 3:
			cur = 3
		if cur != ability_scores[stat].innate:
			res = true
	if permanent and maxx > 3:
		if maxx <= 18:
			if amount > 90:
				maxx -= 1
			if amount > 50:
				maxx -= 1
			if amount > 20:
				maxx -= 1
			maxx -= 1
		else:
			loss = ((((maxx - 18) / 2) + 1) / 2) + 1
			loss = (Rng.roll_die(loss) + loss) * amount / 100
			if loss < amount / 2:
				loss = amount / 2
			maxx -= loss
			if maxx < 18:
				maxx = 18 if amount <= 20 else 17
		if same or maxx < cur:
			maxx = cur
		if maxx != ability_scores[stat].innate_max:
			res = true
	if res:
		ability_scores[stat].innate = cur
		ability_scores[stat].innate_max = maxx
		updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
	return res


func restore_ability_score(stat: int) -> bool:
	if ability_scores[stat].innate != ability_scores[stat].innate_max:
		ability_scores[stat].innate = ability_scores[stat].innate_max
		updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)
		return true
	return false


func shuffle_ability_scores() -> void:
	var ii: int = Rng.roll_die(6) - 1
	var jj: int = ii
	while jj == ii:
		jj = Rng.roll_die(6) - 1
	var innate_max: int = ability_scores[ii].innate_max
	var innate: int = ability_scores[ii].innate
	var innate_max_2: int = ability_scores[jj].innate_max
	var innate_2: int = ability_scores[jj].innate
	ability_scores[ii].innate_max = innate_max_2
	ability_scores[ii].innate = innate_2
	ability_scores[jj].innate_max = innate_max
	ability_scores[jj].innate = innate
	updates_needed.set_bits(UpdateFlags.UPDATE_BONUSES)


func toggle_recall() -> void:
	if word_of_recall_delay == 0:
		word_of_recall_delay = Rng.roll_die(20) + 15
		Profile.instance.message_print("The air about you becomes charged...")
	else:
		word_of_recall_delay = 0
		Profile.instance.message_print("A tension leaves the air around you...")


func restore_health(num: int) -> bool:
	if health < max_health:
		health += num
		if health >= max_health:
			health = max_health
			fractional_health = 0
		redraw_needed.set_bits(RedrawFlags.REDRAW_HP)
		if num < 5:
			Profile.instance.message_print("You feel a little better.")
		elif num < 15:
			Profile.instance.message_print("You feel better.")
		elif num < 35:
			Profile.instance.message_print("You feel much better.")
		else:
			Profile.instance.message_print("You feel very good.")
		return true
	return false


func restore_level() -> bool:
	if experience_points < max_experience_gained:
		Profile.instance.message_print("You feel your life energies returning.")
		experience_points = max_experience_gained
		check_experience()
		return true
	return false


func print_spells(spells: Array[int], num: int, row: int, col: int, realm: int) -> void:
	var spell_set: int = 0 if realm == realm_1 else 1
	Console.clear_to_row_end(row, col)
	Console.print_at(row, col + 5, "Name", Ink.WHITE)
	Console.print_at(row, col + 35, "Lvl Power Fail Info", Ink.WHITE)
	for i: int in num:
		var spell_index: int = spells[i]
		var spell: Spell = spellcasting.spells[spell_set].spells[spell_index]
		Console.clear_to_row_end(row + i + 1, col)
		Console.print_at(
			row + i + 1,
			col,
			"%s) %s" % [Util.index_to_label(i), spell.summary_line(self)],
			Ink.WHITE,
		)
	Console.clear_to_row_end(row + num + 1, col)


func spell_okay(spell_index: int, known: bool, use_realm_2: bool) -> bool:
	var spell_set: int = 1 if use_realm_2 else 0
	var spell: Spell = spellcasting.spells[spell_set].spells[spell_index % 32]
	if spell.level > level:
		return false
	if spell.forgotten:
		return false
	if spell.learned:
		return known
	return not known
