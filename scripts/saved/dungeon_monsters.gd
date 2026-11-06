# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name DungeonMonsters
extends Resource

const RANDOM_ENCOUNTER_CHANCE: int = 160
const MAX_DUNGEON_MONSTERS: int = 512
const NASTY_MONSTER_CHANCE: int = 50
const GROUP_MAX: int = 32

@export var currently_acting_monster: int
@export var dungeon_bias: int
@export var last_placed_monster: int
@export var last_placed_monster_type_index: int
@export var monsters: Array[Monster]
@export var num_repro: int
@export var repair_monsters: bool
@export var shimmer_monsters: bool
@export var summon_kin_type: String
@export var summon_specific_type: int

var dungeon_level: DungeonLevel:
	set = _set_dungeon_level, get = _get_dungeon_level
var monster_validation_function: Callable
var _dungeon_level: DungeonLevel


static func new_dungeon_monsters(new_dungeon_level: DungeonLevel) -> DungeonMonsters:
	var dungeon_monsters: DungeonMonsters = DungeonMonsters.new()
	dungeon_monsters.dungeon_level = new_dungeon_level
	return dungeon_monsters


func _init() -> void:
	monster_validation_function = Callable()
	monsters = []
	for i: int in MAX_DUNGEON_MONSTERS:
		monsters.append(Monster.new())


func wipe() -> void:
	for i: int in range(dungeon_level.monster_count_max - 1, 0, -1):
		var monster: Monster = monsters[i]
		if monster.monster_type == null:
			continue
		# There is one fewer of this monster type in existence
		monster.monster_type.cur_num -= 1
		# Remove the index from the map
		dungeon_level.get_grid(monster.map_row, monster.map_col).monster_index = 0
		# Replace the monster with a null one
		monsters[i] = Monster.new()
	# No monsters on level
	dungeon_level.monster_count = 0
	dungeon_level.monster_count_max = 1
	# No reproducers on level
	num_repro = 0
	# No-one to target or track
	SaveGame.instance.target_who = 0
	SaveGame.instance.health_track(0)


# Prepare the random monster table based on a validation function or lack of one
func prepare_random_monster_table() -> void:
	for entry: FindChance in SaveGame.instance.random_monster_table:
		if monster_validation_function.is_null() == false:
			# If we have a validation function then monsters that fail validation have zero probability
			if monster_validation_function.call(entry.index):
				entry.filtered_probability = entry.base_probability
			else:
				entry.filtered_probability = 0
		else:
			# If we have no validation function then all monsters have their base probability
			entry.filtered_probability = entry.base_probability


# Select a random monster of an appropriate level
func get_random_monster(level: int) -> int:
	var table: Array[FindChance] = SaveGame.instance.random_monster_table
	if level > 0:
		# If we're not on the town we have a chance of a nasty monster
		if Rng.roll_die(NASTY_MONSTER_CHANCE) == 1:
			level += mini(level / 4 + 2, 5)
		# Check for a nasy monster twice (meaning there's a very small chance of a very nasty monster)
		if Rng.roll_die(NASTY_MONSTER_CHANCE) == 1:
			level += mini(level / 4 + 2, 5)
	var total: int = 0
	# Do a second pass filtering the (possibly pre-filtered) table for unsuitable monsters
	for i: int in SaveGame.instance.random_monster_table_size:
		# If we've gone to far, stop
		if table[i].level > level:
			break
		table[i].final_probability = 0
		# No town inhabitants in the dungeon
		if level > 0 and table[i].level <= 0:
			continue
		# No uniques if they're already here
		var type: MonsterType = Profile.instance.monster_types.types[table[i].index]
		if type.flags_1.is_set(MonsterFlags1.UNIQUE) and type.cur_num >= type.max_num:
			continue
		table[i].final_probability = table[i].filtered_probability
		total += table[i].final_probability
	# If we have no suitable monsters, return
	if total == 0:
		return 0
	# Choose a monster based on the final probability
	var roll: int = Rng.roll_die(total) - 1
	var index: int = 0
	while index < SaveGame.instance.random_monster_table_size:
		if roll < table[index].final_probability:
			break
		roll -= table[index].final_probability
		index += 1
	var p: int = Rng.roll_die(100) - 1
	# 60% chance to reroll and take the higher level monster
	if p < 60:
		var previous: int = index
		roll = Rng.roll_die(total) - 1
		index = 0
		while index < SaveGame.instance.random_monster_table_size:
			if roll < table[index].final_probability:
				break
			roll -= table[index].final_probability
			index += 1
		if table[previous].level > table[index].level:
			index = previous
	# 10% chance to reroll a second time and take the higher level monster
	if p < 10:
		var previous: int = index
		roll = Rng.roll_die(total) - 1
		index = 0
		while index < SaveGame.instance.random_monster_table_size:
			if roll < table[index].final_probability:
				break
			roll -= table[index].final_probability
			index += 1
		if table[previous].level > table[index].level:
			index = previous
	return table[index].index


## Check if a monster is visible, and mark it appropriately
func update_monster_visibility(index: int, update_distance: bool) -> void:
	var player: Player = SaveGame.instance.player
	var monster: Monster = monsters[index]
	var type: MonsterType = monster.monster_type
	if type == null:
		return
	var final_row: int = monster.map_row
	var final_col: int = monster.map_col
	# Update the monster's distance from the player if necessary
	if update_distance:
		var d_row: int = absi(player.map_row - final_row)
		var d_col: int = absi(player.map_col - final_col)
		var d: int = (d_row + d_col / 2) if d_row > d_col else (d_col + d_row / 2)
		monster.distance_from_player = clampi(d, 1, 255)
	var old_visibility: bool = monster.is_visible
	var flag: bool
	var easy: bool
	var hard: bool
	var do_weird_mind: bool
	var do_empty_mind: bool
	var do_invisible: bool
	var do_cold_blood: bool
	if monster.distance_from_player > DungeonLevel.MAX_SIGHT_RADIUS:
		if not monster.is_visible:
			return
		if monster.individual_monster_flags.is_set(IndividualMonsterFlags.MARK):
			flag = true
	elif dungeon_level.panel_contains(final_row, final_col):
		var grid: GridTile = dungeon_level.get_grid(final_row, final_col)
		if grid.tile_flags.is_set(GridTile.IS_VISIBLE) and player.timed_blindness == 0:
			if monster.distance_from_player <= player.infravision_range:
				if type.flags_2.is_set(MonsterFlags2.COLD_BLOOD):
					do_cold_blood = true
				if not do_cold_blood:
					easy = true
					flag = true
			if grid.tile_flags.is_set(GridTile.PLAYER_LIT | GridTile.SELF_LIT):
				if type.flags_2.is_set(MonsterFlags2.INVISIBLE):
					do_invisible = true
				if not do_invisible or player.has_see_invisibility:
					easy = true
					flag = true
		if player.has_telepathy:
			if type.flags_2.is_set(MonsterFlags2.EMPTY_MIND):
				do_empty_mind = true
			elif type.flags_2.is_set(MonsterFlags2.WEIRD_MIND):
				do_weird_mind = true
				if Rng.roll_die(100) <= 10:
					hard = true
					flag = true
			else:
				hard = true
				flag = true
		if monster.individual_monster_flags.is_set(IndividualMonsterFlags.MARK):
			flag = true
		if player.is_wizard:
			flag = true
	if flag:
		if not monster.is_visible:
			monster.is_visible = true
			dungeon_level.redraw_single_location(final_row, final_col)
			if SaveGame.instance.tracked_monster_index == index:
				player.redraw_needed.set_bits(RedrawFlags.REDRAW_HEALTH)
			if type.knowledge.r_sights < 32768:
				type.knowledge.r_sights += 1
		if hard:
			if type.flags_2.is_set(MonsterFlags2.STUPID):
				type.knowledge.r_flags_2.set_bits(MonsterFlags2.STUPID)
			if type.flags_2.is_set(MonsterFlags2.SMART):
				type.knowledge.r_flags_2.set_bits(MonsterFlags2.SMART)
		if do_empty_mind:
			type.knowledge.r_flags_2.set_bits(MonsterFlags2.EMPTY_MIND)
		if do_weird_mind:
			type.knowledge.r_flags_2.set_bits(MonsterFlags2.WEIRD_MIND)
		if do_invisible:
			type.knowledge.r_flags_2.set_bits(MonsterFlags2.INVISIBLE)
		if do_cold_blood:
			type.knowledge.r_flags_2.set_bits(MonsterFlags2.COLD_BLOOD)
	else:
		if monster.is_visible:
			monster.is_visible = false
			dungeon_level.redraw_single_location(final_row, final_col)
			if SaveGame.instance.tracked_monster_index == index:
				player.redraw_needed.set_bits(RedrawFlags.REDRAW_HEALTH)
	if easy:
		if monster.is_visible != old_visibility:
			if type.flags_2.is_set(MonsterFlags2.ELDRITCH_HORROR):
				monster.sanity_blast(false)
		if monster.individual_monster_flags.is_clear(IndividualMonsterFlags.VIEW):
			monster.individual_monster_flags.clear_bits(IndividualMonsterFlags.VIEW)
			if monster.mind.is_clear(MonsterMind.FRIENDLY):
				SaveGame.instance.disturb(true)
	else:
		if monster.individual_monster_flags.is_set(IndividualMonsterFlags.VIEW):
			monster.individual_monster_flags.clear_bits(IndividualMonsterFlags.VIEW)
			if monster.mind.is_clear(MonsterMind.FRIENDLY):
				SaveGame.instance.disturb(true)


## Spawn a random monster somewhere randomly on the level
func spawn_monster(min_distance: int, start_asleep: bool) -> void:
	var attempts_left: int = 10000
	var row: int
	var col: int
	while attempts_left != 0:
		row = Rng.roll_die(dungeon_level.cur_hgt) - 1
		col = Rng.roll_die(dungeon_level.cur_wid) - 1
		if not dungeon_level.grid_open_no_item_or_creature(row, col):
			continue
		if dungeon_level.distance(row, col, SaveGame.instance.player.map_row, SaveGame.instance.player.map_col) > min_distance:
			break
		attempts_left -= 1
	if Rng.roll_die(5000) <= dungeon_level.monster_level:
		spawn_horde(row, col)
	else:
		if dungeon_bias != 0 and Rng.roll_die(10) > 6:
			summon_specific(row, col, dungeon_level.monster_level, dungeon_bias)
		else:
			place_random_monster(row, col, start_asleep, true)


# Spawn a horde of monsters somewhere on the level
func spawn_horde(row: int, col: int) -> bool:
	var type: MonsterType
	var attempts: int = 1000
	while attempts > 0:
		attempts -= 1
		# Find a base monster
		var index: int = get_random_monster(dungeon_level.monster_level)
		# If no suitable monsters, give up
		if index == 0:
			return false
		type = Profile.instance.monster_types.types[index]
	# If we didn't find a suitable monster, give up
	if attempts < 1:
		return false
	attempts = 1000
	while attempts > 0:
		attempts -= 1
		if place_monster_by_type(row, col, type, false, false, false):
			break
	if attempts < 1:
		return false
	# Place some more monsters the same as the previous one
	var monster: Monster = monsters[last_placed_monster]
	summon_kin_type = type.character
	for i: int in Rng.roll_die(10) + 5:
		summon_specific(monster.map_row, monster.map_col, dungeon_level.monster_level, SummonType.SUMMON_KIN)
	return true


## Summon a specific summon-type of monster of a suitable level near the desired location
func summon_specific(base_row: int, base_col: int, monster_level: int, summon_type: int) -> bool:
	var tries: int = 0
	var row: int = base_row
	var col: int = base_col
	var group_ok: bool = true
	while tries < 20:
		# Increase our distance if we're running out of tries
		var distance: int = tries / 15 + 1
		# Pick a spot close to the base row
		var location: Location = dungeon_level.scatter(base_row, base_col, distance)
		# If we can't place a monster here, try again
		if not dungeon_level.grid_passable_no_creature(location.row, location.col):
			tries += 1
			continue
		# We can't place a monster on a sigil
		if dungeon_level.get_grid(location.row, location.col).feature_type.category == FloorTileTypeCategory.SIGIL:
			tries += 1
			continue
		# We successfully found a place
		row = location.row
		col = location.col
		break
	# If we ran out of tries, return a failure to place the monster
	if tries == 20:
		return false
	# Prepare the random monster table to only include what we want to summon
	summon_specific_type = summon_type
	monster_validation_function = validate_summon_specific
	prepare_random_monster_table()
	var index: int = get_random_monster(monster_level)
	monster_validation_function = Callable()
	prepare_random_monster_table()
	if index == 0:
		return false
	var type: MonsterType = Profile.instance.monster_types.types[index]
	# Can't summon groups of Avatars of Nyarlathotep
	if summon_type == SummonType.SUMMON_AVATAR:
		group_ok = false
	return place_monster_by_type(row, col, type, false, group_ok, false)


## Place a specific monster, making it friendly
func summon_specific_friendly(base_row:int, base_col:int, monster_level:int, summon_type:int, group_ok:bool)->bool:
	var tries: int = 0
	var row: int = base_row
	var col: int = base_col
	# Bump up the monster level if it's less than the player's level
	monster_level = maxi(monster_level, SaveGame.instance.player.level)
	# Bump up the monster level if it's less than the average of the player's level and the dungeon level's difficulty
	monster_level = maxi(monster_level, SaveGame.instance.player.level + SaveGame.instance.total_difficulty() / 2)
	while tries < 20:
		# Increase our distance if we're running out of tries
		var distance: int = tries / 15 + 1
		# Pick a spot close to the base row
		var location: Location = dungeon_level.scatter(base_row, base_col, distance)
		# If we can't place a monster here, try again
		if not dungeon_level.grid_passable_no_creature(location.row, location.col):
			tries += 1
			continue
		# We can't place a monster on a sigil
		if dungeon_level.get_grid(location.row, location.col).feature_type.category == FloorTileTypeCategory.SIGIL:
			tries += 1
			continue
		# We successfully found a place
		row = location.row
		col = location.col
		break
	# If we ran out of tries, return a failure to place the monster
	if tries == 20:
		return false
	# Prepare the random monster table to only include what we want to summon
	summon_specific_type = summon_type
	monster_validation_function = validate_summon_specific
	prepare_random_monster_table()
	var index: int = get_random_monster(monster_level)
	monster_validation_function = Callable()
	prepare_random_monster_table()
	if index == 0:
		return false
	var type: MonsterType = Profile.instance.monster_types.types[index]
	# Can't summon groups of Avatars of Nyarlathotep
	if summon_type == SummonType.SUMMON_AVATAR:
		group_ok = false
	return place_monster_by_type(row, col, type, false, group_ok, true)



## Place a random monster in the dungeon, with a possible group
func place_random_monster(row: int, col: int, start_asleep: bool, allow_group: bool) -> bool:
	# Get a random monster type for the level
	var index: int = get_random_monster(dungeon_level.monster_level)
	# We couldn't find any suitable monster types
	if index == 0:
		return false
	return place_monster_by_index(row, col, index, start_asleep, allow_group, false)


## Place a monster in the dungeon by monster index, with a possible group
func place_monster_by_index(row: int, col: int, index: int, start_asleep: bool, allow_group: bool, start_friendly: bool) -> bool:
	return place_monster_by_type(row, col, Profile.instance.monster_types.types[index], start_asleep, allow_group, start_friendly)


## Place a monster in the dungeon my monster type, with a possible group
func place_monster_by_type(row: int, col: int, type: MonsterType, start_asleep: bool, allow_group: bool, start_friendly: bool) -> bool:
	# If we can't place the monster, then simply return
	if not _place_monster_one(row, col, type, start_asleep, start_friendly):
		return false
	# If the monster has an escort, have fifty tries at placing minions
	if type.flags_1.is_set(MonsterFlags1.ESCORTED):
		for i: int in 50:
			var location: Location = dungeon_level.scatter(row, col, 3)
			if not dungeon_level.grid_passable_no_creature(location.row, location.col):
				continue
			# Prepare the random monster table to only valid escorts
			last_placed_monster_type_index = type.index
			monster_validation_function = validate_escort
			prepare_random_monster_table()
			var z: int = get_random_monster(type.level)
			monster_validation_function = Callable()
			prepare_random_monster_table()
			if z == 0:
				break
			var escorter_type: MonsterType = Profile.instance.monster_types.types[z]
			_place_monster_one(location.row, location.col, escorter_type, start_asleep, start_friendly)
			if escorter_type.flags_1.is_set(MonsterFlags1.FRIENDS) or escorter_type.flags_1.is_set(MonsterFlags1.ESCORTS_GROUP):
				place_monster_group(location.row, location.col, z, start_asleep, start_friendly)
	if not allow_group:
		return true
	if type.flags_1.is_set(MonsterFlags1.FRIENDS):
		place_monster_group(row, col, type.index, start_asleep, start_friendly)
	return true


# Validate a monster type as being suitable to escort the most recently placed monster
func validate_escort(monster_index: int) -> bool:
	var master_type: MonsterType = Profile.instance.monster_types.types[last_placed_monster_type_index]
	var type: MonsterType = Profile.instance.monster_types.types[monster_index]
	# Escort must be broadly the same type of monster (using the same character)
	if master_type.character != type.character:
		return false
	# Escort can't be higher level than the master
	if type.level > master_type.level:
		return false
	# Uniques never escort other monsters
	if type.flags_1.is_set(MonsterFlags1.UNIQUE):
		return false
	# Monsters never escort themselves
	if monster_index == last_placed_monster_type_index:
		return false
	# Passed the tests
	return true


## Place a variable number of the same monster together, based on the difficulty
func place_monster_group(row: int, col: int, index: int, start_asleep: bool, start_friendly: bool) -> void:
	var type: MonsterType = Profile.instance.monster_types.types[index]
	var extra: int
	var rows: Array[int]
	var cols: Array[int]
	for i: int in GROUP_MAX:
		rows.append(0)
		cols.append(0)
	# Start with 1d13 monsters
	var total: int = Rng.roll_die(13)
	if type.level > dungeon_level.monster_level:
		# If the monster is higher level than the dungeon level, reduce the number
		extra = -Rng.roll_die(type.level - dungeon_level.monster_level)
	elif type.level < dungeon_level.monster_level:
		# If the monster is lower level than the dungeon level, increase the number
		extra = Rng.roll_die(dungeon_level.monster_level - type.level)
		# No more than 12 extras, regardless of how low level the monster is
		extra = mini(extra, 12)
	# Add the extras, and clamp the total to make sure there aren't too few or too many
	total += extra
	total = clampi(total, 1, GROUP_MAX)
	# Remember the old danger rating, because we don't want to send it through the roof if we're adding monsters higher than its level
	var old_danger: int = dungeon_level.danger_rating
	# Make the monsters
	var placing: int = 1
	rows[0] = row
	cols[0] = col
	var anchor: int = 0
	# We keep trying to place monsters around our anchor monster until we can't place any more,
	# then we update the anchor to the next placed monster and continue
	# We stop when we have placed our total number of monsters or we can't place any more
	while anchor < placing and placing < total:
		var old_row: int = rows[anchor]
		var old_col: int = cols[anchor]
		var dir: int = 0
		while dir < 8 and placing < total:
			var new_row: int = old_row + dungeon_level.ordered_direction_row_offset[dir]
			var new_col: int = old_col + dungeon_level.ordered_direction_col_offset[dir]
			if not dungeon_level.grid_passable_no_creature(new_row, new_col):
				dir += 1
				continue
			if _place_monster_one(new_row, new_col, type, start_asleep, start_friendly):
				rows[placing] = new_row
				cols[placing] = new_col
				placing += 1
			dir += 1
		anchor += 1
	dungeon_level.danger_rating = old_danger


## Return the index of the next free slot in the monster array
func get_free_monster_slot() -> int:
	# Simple scenario - there's a slot at the end of the list
	if dungeon_level.monster_count_max < MAX_DUNGEON_MONSTERS:
		var index: int = dungeon_level.monster_count_max
		dungeon_level.monster_count_max += 1
		dungeon_level.monster_count += 1
		return index
	# More complex scenario - the list is apparently full (but an existing monster may have died)
	for index: int in range(1, dungeon_level.monster_count_max):
		if monsters[index].monster_type != null:
			continue
		dungeon_level.monster_count += 1
		return index
	# The list is full and there were no slots left by dead monsters
	#TODO: console.message_print("Too many monsters!")
	return 0


## Validate a monster type as being suitable based on a summon flag
func validate_summon_specific(monster_index: int) -> bool:
	var type: MonsterType = Profile.instance.monster_types.types[monster_index]
	var okay: bool = false
	# If we have no summoning flag then any monster is acceptable
	if summon_specific_type == SummonType.NO_SUMMON_FLAG:
		return true
	# Each summon type has its own custom restriction
	match summon_specific_type:
		SummonType.SUMMON_ANT:
			okay = type.character == "a" and type.flags_1.is_clear(MonsterFlags1.UNIQUE)
		SummonType.SUMMON_SPIDER:
			okay = type.character == "S" and type.flags_1.is_clear(MonsterFlags1.UNIQUE)
		SummonType.SUMMON_HOUND:
			okay = (type.character == "C" or type.character == "Z") and type.flags_1.is_clear(MonsterFlags1.UNIQUE)
		SummonType.SUMMON_HYDRA:
			okay = type.character == "M" and type.flags_1.is_clear(MonsterFlags1.UNIQUE)
		SummonType.SUMMON_CTHULOID:
			okay = type.flags_3.is_set(MonsterFlags3.CTHULOID) and type.flags_1.is_clear(MonsterFlags1.UNIQUE)
		SummonType.SUMMON_DEMON:
			okay = type.flags_3.is_set(MonsterFlags3.DEMON) and type.flags_1.is_clear(MonsterFlags1.UNIQUE)
		SummonType.SUMMON_UNDEAD:
			okay = type.flags_3.is_set(MonsterFlags3.UNDEAD) and type.flags_1.is_clear(MonsterFlags1.UNIQUE)
		SummonType.SUMMON_DRAGON:
			okay = type.flags_3.is_set(MonsterFlags3.DRAGON) and type.flags_1.is_clear(MonsterFlags1.UNIQUE)
		SummonType.SUMMON_HI_UNDEAD:
			okay = type.character == "L" or type.character == "V" or type.character == "W"
		SummonType.SUMMON_HI_DRAGON:
			okay = type.character == "D"
		SummonType.SUMMON_GOO:
			okay = type.flags_3.is_set(MonsterFlags3.GREAT_OLD_DONE)
		SummonType.SUMMON_UNIQUE:
			okay = type.flags_1.is_set(MonsterFlags1.UNIQUE)
		SummonType.SUMMON_ORC:
			okay = type.character == "o" and type.flags_1.is_clear(MonsterFlags1.UNIQUE)
		SummonType.SUMMON_KOBOLD:
			okay = type.character == "k" and type.flags_1.is_clear(MonsterFlags1.UNIQUE)
		SummonType.SUMMON_YEEK:
			okay = type.character == "y" and type.flags_1.is_clear(MonsterFlags1.UNIQUE)
		SummonType.SUMMON_HUMAN:
			okay = type.character == "p" and type.flags_1.is_clear(MonsterFlags1.UNIQUE)
		SummonType.SUMMON_BIZARRE_1:
			okay = type.character == "m" and type.flags_1.is_clear(MonsterFlags1.UNIQUE)
		SummonType.SUMMON_BIZARRE_2:
			okay = type.character == "b" and type.flags_1.is_clear(MonsterFlags1.UNIQUE)
		SummonType.SUMMON_BIZARRE_3:
			okay = type.character == "Q" and type.flags_1.is_clear(MonsterFlags1.UNIQUE)
		SummonType.SUMMON_BIZARRE_4:
			okay = type.character == "v" and type.flags_1.is_clear(MonsterFlags1.UNIQUE)
		SummonType.SUMMON_BIZARRE_5:
			okay = type.character == "$" and type.flags_1.is_clear(MonsterFlags1.UNIQUE)
		SummonType.SUMMON_BIZARRE_6:
			okay = (type.character == "!" or type.character == "?" or type.character == "=" or type.character == "$" or type.character == "|") and type.flags_1.is_clear(MonsterFlags1.UNIQUE)
		SummonType.SUMMON_REAVER:
			okay = type.name == "Black reaver" and type.flags_1.is_clear(MonsterFlags1.UNIQUE)
		SummonType.SUMMON_KIN:
			okay = type.character == summon_kin_type and type.flags_1.is_clear(MonsterFlags1.UNIQUE)
		SummonType.SUMMON_AVATAR:
			okay = type.Name == "Avatar of Nyarlathotep" and type.flags_1.is_clear(MonsterFlags1.UNIQUE)
		SummonType.SUMMON_ANIMAL:
			okay = type.flags_3.is_set(MonsterFlags3.ANIMAL) and type.flags_1.is_clear(MonsterFlags1.UNIQUE)
		SummonType.SUMMON_ANIMAL_RANGER:
			okay = (type.flags_3.is_set(MonsterFlags3.ANIMAL) and "abcflqrwBCIJKMRS".contains(type.character) and type.flags_3.is_clear(MonsterFlags3.DRAGON)
				and type.flags_3.is_clear(MonsterFlags3.EVIL) and type.flags_3.is_clear(MonsterFlags3.UNDEAD) and type.flags_3.is_clear(MonsterFlags3.DEMON)
				and type.flags_3.is_clear(MonsterFlags3.CTHULOID) and not type.flags_4.is_any_set() and not type.flags_5.is_any_set()
				and not type.flags_6.is_any_set() and type.flags_1.is_clear(MonsterFlags1.UNIQUE) )
		SummonType.SUMMON_HI_UNDEAD_NO_UNIQUES:
			okay = (type.character == "L" or type.character == "V" or type.character == "W") and type.flags_1.is_clear(MonsterFlags1.UNIQUE)
		SummonType.SUMMON_HI_DRAGON_NO_UNIQUES:
			okay = type.character == "D" and type.flags_1.is_clear(MonsterFlags1.UNIQUE)
		SummonType.SUMMON_NO_UNIQUES:
			okay = type.flags_1.is_clear(MonsterFlags1.UNIQUE)
		SummonType.SUMMON_PHANTOM:
			okay = type.name.contains("Phantom") and type.flags_1.is_clear(MonsterFlags1.UNIQUE)
		SummonType.SUMMON_ELEMENTAL:
			okay = type.name.contains("lemental") and type.flags_1.is_clear(MonsterFlags1.UNIQUE)
	return okay


func get_pets() -> Array[Monster]:
	var list: Array[Monster] = []
	for monster: Monster in monsters:
		if monster.mind.is_set(MonsterMind.FRIENDLY):
			list.append(monster)
	return list


func replace_pet(row: int, col: int, monster: Monster) -> void:
	var location: Location
	var attempt: int = 1
	for i: int in 20:
		attempt += 1
		var d: int = (i / 15) + 1
		location = dungeon_level.scatter(row, col, d)
		if not dungeon_level.grid_passable_no_creature(location.row, location.col):
			continue
		if dungeon_level.get_grid(location.row, location.col).feature_type.category == FloorTileTypeCategory.SIGIL:
			continue
		break
	if attempt > 20:
		Profile.instance.message_print("You lose sight of %s." % [monster.monster_desc(0)])
		return
	var tile: GridTile = dungeon_level.get_grid(location.row, location.col)
	tile.monster_index = get_free_monster_slot()
	if tile.monster_index == 0:
		Profile.instance.message_print("You lose sight of %s." % [monster.monster_desc(0)])
		return
	monsters[tile.monster_index] = monster
	monster.map_row = location.row
	monster.map_col = location.col
	var type: MonsterType = monster.monster_type
	if type.flags_2.is_set(MonsterFlags2.MULTIPLY):
		num_repro += 1
	if type.flags_1.is_set(MonsterFlags1.ATTR_MULTI):
		shimmer_monsters = true


func delete_monster_by_index(index: int, visibly: bool) -> void:
	var monster: Monster = monsters[index]
	var type: MonsterType = monster.monster_type
	if type == null:
		return
	var row: int = monster.map_row
	var col: int = monster.map_col
	type.cur_num -= 1
	if type.flags_2.is_set(MonsterFlags2.MULTIPLY):
		num_repro -= 1
	if index == SaveGame.instance.target_who:
		SaveGame.instance.target_who = 0
	if index == SaveGame.instance.tracked_monster_index:
		SaveGame.instance.health_track(0)
	dungeon_level.get_grid(row, col).monster_index = 0
	var item_index: int = monster.first_held_item_index
	while item_index != 0:
		var item: Item = dungeon_level.dungeon_items.items[item_index]
		item.holding_monster_index = 0
		dungeon_level.delete_item_by_index(item_index)
		item_index = item.next_in_stack
	monsters[index] = Monster.new()
	dungeon_level.monster_count -= 1
	if visibly:
		dungeon_level.redraw_single_location(row, col)


func damage_monster(monster_index: int, damage: int, note: String) -> bool:
	CommandEngine.fear = false
	var monster: Monster = monsters[monster_index]
	var monster_type: MonsterType = monster.monster_type
	if SaveGame.instance.tracked_monster_index == monster_index:
		SaveGame.instance.player.redraw_needed.set_bits(RedrawFlags.REDRAW_HEALTH)
	monster.sleep_level = 0
	monster.health -= damage
	if monster.health < 0:
		Console.play_sound("MonsterDeath")
		var monster_name: String = monster.describe(0)
		if monster_type.flags_3.is_set(MonsterFlags3.GREAT_OLD_DONE) and Rng.roll_die(2) == 1:
			Profile.instance.message_print("%s retreats into another dimension!" % [monster_name])
			if Rng.roll_die(5) == 1:
				Profile.instance.message_print("Nyarlathotep puts a terrible curse on you!")
				SaveGame.instance.Player.CurseEquipment(100, 50)
				for curses: int in 1 + Rng.roll_die(3):
					SaveGame.instance.activate_dread_curse()
		if note != "":
			Profile.instance.message_print("%s%s" % [monster_name, note])
		elif not monster.is_visible:
			Profile.instance.message_print("You have killed %s." % [monster_name])
		elif (monster_type.flags_3.is_set(MonsterFlags3.DEMON) or monster_type.flags_3.is_set(MonsterFlags3.UNDEAD) or
			monster_type.flags_3.is_set(MonsterFlags3.CTHULOID) or monster_type.flags_2.is_set(MonsterFlags2.STUPID) or
			monster_type.flags_3.is_set(MonsterFlags3.NON_LIVING) or "Evg".contains(monster_type.character) ):
			Profile.instance.message_print("You have destroyed %s." % [monster_name])
		else:
			Profile.instance.message_print("You have slain %s." % [monster_name])
		var div: int = 10 * SaveGame.instance.player.max_level_gained
		if monster_type.knowledge.r_kills >= 19:
			div *= 2
		if monster_type.knowledge.r_kills == 0:
			div /= 3
		if monster_type.knowledge.r_kills == 1:
			div /= 2
		if monster_type.knowledge.r_kills == 2:
			div /= 2
		if div < 1:
			div = 1
		var new_experience: int = monster_type.experience * monster_type.level * 10 / div
		var new_experience_fraction: int = (monster_type.experience * monster_type.level % div * 0x10000 / div) + SaveGame.instance.player.fractional_experience_points
		if new_experience_fraction >= 0x10000:
			new_experience += 1
			SaveGame.instance.player.fractional_experience_points = new_experience_fraction - 0x10000
		else:
			SaveGame.instance.player.fractional_experience_points = new_experience_fraction
		SaveGame.instance.player.gain_experience(new_experience)
		SaveGame.instance.monster_death(monster_index)
		if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
			monster_type.max_num = 0
		if monster.is_visible or monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
			if monster_type.knowledge.r_kills < Constants.MAX_INT:
				monster_type.knowledge.r_kills += 1
			if monster_type.knowledge.r_total_kills < Constants.MAX_INT:
				monster_type.knowledge.r_total_kills += 1
		delete_monster_by_index(monster_index, true)
		CommandEngine.fear = false
		return true
	if monster.fear_level != 0 and damage > 0:
		var temp: int = Rng.roll_die(damage)
		if temp < monster.fear_level:
			monster.fear_level -= temp
		else:
			monster.fear_level = 0
			CommandEngine.fear = false
	if monster.fear_level == 0 and monster_type.flags_3.is_clear(MonsterFlags3.IMMUNE_FEAR):
		var percentage: int = 100 * monster.health / monster.max_health
		if (percentage <= 10 and Rng.roll_die(10) <= percentage) or (damage >= monster.health and Rng.roll_die(100) <= 80):
			CommandEngine.fear = true
			monster.fear_level = Rng.roll_die(10) + (20 if damage >= monster.health and percentage > 7 else (11 - percentage) * 5)
	return false


func lore_treasure(monster_index: int, item_count: int, gold_count: int) -> void:
	var monster: Monster = monsters[monster_index]
	var monster_type: MonsterType = monster.monster_type
	if item_count > monster_type.knowledge.r_drop_item:
		monster_type.knowledge.r_drop_item = item_count
	if gold_count > monster_type.knowledge.r_drop_gold:
		monster_type.knowledge.r_drop_gold = gold_count
	if monster_type.flags_1.is_set(MonsterFlags1.DROP_GOOD):
		monster_type.knowledge.r_flags_1.set_bits(MonsterFlags1.DROP_GOOD)
	if monster_type.flags_1.is_set(MonsterFlags1.DROP_GREAT):
		monster_type.knowledge.r_flags_1.set_bits(MonsterFlags1.DROP_GREAT)


func multiply_monster(monster_index: int, charm: int, clone: int) -> bool:
	var monster: Monster = monsters[monster_index]
	var result: bool = false
	for i: int in 18:
		var d: int = 1
		var location: Location = _dungeon_level.scatter(monster.map_row, monster.map_col, d)
		if not _dungeon_level.grid_passable_no_creature(location.row, location.col):
			continue
		result = place_monster_by_type(location.row, location.col, monster.monster_type, false, false, charm)
		break
	if clone and result:
		monsters[last_placed_monster].Mind.set_bits(MonsterMind.CLONED)
	monster.generation += 1
	monsters[last_placed_monster].generation = monster.generation
	return result


func message_pain(monster_index: int, dam: int) -> void:
	var monster: Monster = monsters[monster_index]
	var monster_type: MonsterType = monster.monster_type
	var monster_name: String = monster.describe(0)
	if dam == 0:
		Profile.instance.message_print("%s is unharmed." % [monster_name])
		return
	var new_health: int = monster.health
	var old_health: int = new_health + dam
	var percentage: int = new_health * 100 / old_health
	if "jmvQ".contains(monster_type.character):
		if percentage > 95:
			Profile.instance.message_print("%s barely notices." % [monster_name])
		elif percentage > 75:
			Profile.instance.message_print("%s flinches." % [monster_name])
		elif percentage > 50:
			Profile.instance.message_print("%s squelches." % [monster_name])
		elif percentage > 35:
			Profile.instance.message_print("%s quivers in pain." % [monster_name])
		elif percentage > 20:
			Profile.instance.message_print("%s writhes about." % [monster_name])
		elif percentage > 10:
			Profile.instance.message_print("%s writhes in agony." % [monster_name])
		else:
			Profile.instance.message_print("%s jerks limply." % [monster_name])
	elif "CZ".contains(monster_type.character):
		if percentage > 95:
			Profile.instance.message_print("%s shrugs off the attack." % [monster_name])
		elif percentage > 75:
			Profile.instance.message_print("%s snarls with pain." % [monster_name])
		elif percentage > 50:
			Profile.instance.message_print("%s yelps in pain." % [monster_name])
		elif percentage > 35:
			Profile.instance.message_print("%s howls in pain." % [monster_name])
		elif percentage > 20:
			Profile.instance.message_print("%s howls in agony." % [monster_name])
		elif percentage > 10:
			Profile.instance.message_print("%s writhes in agony." % [monster_name])
		else:
			Profile.instance.message_print("%s yelps feebly." % [monster_name])
	elif "FIKMRSXabclqrst".contains(monster_type.character):
		if percentage > 95:
			Profile.instance.message_print("%s ignores the attack." % [monster_name])
		elif percentage > 75:
			Profile.instance.message_print("%s grunts with pain." % [monster_name])
		elif percentage > 50:
			Profile.instance.message_print("%s squeals in pain." % [monster_name])
		elif percentage > 35:
			Profile.instance.message_print("%s shrieks in pain." % [monster_name])
		elif percentage > 20:
			Profile.instance.message_print("%s shrieks in agony." % [monster_name])
		elif percentage > 10:
			Profile.instance.message_print("%s writhes in agony." % [monster_name])
		else:
			Profile.instance.message_print("%s cries out feebly." % [monster_name])
	else:
		if percentage > 95:
			Profile.instance.message_print("%s shrugs off the attack." % [monster_name])
		elif percentage > 75:
			Profile.instance.message_print("%s grunts with pain." % [monster_name])
		elif percentage > 50:
			Profile.instance.message_print("%s cries out in pain." % [monster_name])
		elif percentage > 35:
			Profile.instance.message_print("%s screams in pain." % [monster_name])
		elif percentage > 20:
			Profile.instance.message_print("%s screams in agony." % [monster_name])
		elif percentage > 10:
			Profile.instance.message_print("%s writhes in agony." % [monster_name])
		else:
			Profile.instance.message_print("%s cries out feebly." % [monster_name])


func update_smart_learn(monster_index: int, what: int) -> void:
	var player: Player = SaveGame.instance.player
	var monster: Monster = monsters[monster_index]
	var monster_type: MonsterType = monster.monster_type
	if monster_type == null:
		return
	if monster_type.flags_2.is_set(MonsterFlags2.STUPID):
		return
	if monster_type.flags_2.is_clear(MonsterFlags2.SMART) and Rng.roll_die(100) <= 50:
		return
	match what:
		DamageResistance.ACID:
			if player.has_acid_resistance:
				monster.mind.set_bits(MonsterMind.SEEN_ACID_RES)
			if player.timed_acid_resistance != 0:
				monster.mind.set_bits(MonsterMind.SEEN_TEMP_ACID_RES)
			if player.has_acid_immunity:
				monster.mind.set_bits(MonsterMind.SEEN_ACID_IMMUNITY)
		DamageResistance.ELECTRICITY:
			if player.has_lightning_resistance:
				monster.mind.set_bits(MonsterMind.SEEN_ELECTRIC_RES)
			if player.timed_lightning_resistance != 0:
				monster.mind.set_bits(MonsterMind.SEEN_TEMP_ELECTRIC_RES)
			if player.has_lightning_immunity:
				monster.mind.set_bits(MonsterMind.SEEN_ELECTRIC_IMMUNITY)
		DamageResistance.FIRE:
			if player.has_fire_resistance:
				monster.mind.set_bits(MonsterMind.SEEN_FIRE_RES)
			if player.timed_fire_resistance != 0:
				monster.mind.set_bits(MonsterMind.SEEN_TEMP_FIRE_RES)
			if player.has_fire_immunity:
				monster.mind.set_bits(MonsterMind.SEEN_FIRE_IMMUNITY)
		DamageResistance.COLD:
			if player.has_cold_resistance:
				monster.mind.set_bits(MonsterMind.SEEN_COLD_RES)
			if player.timed_cold_resistance != 0:
				monster.mind.set_bits(MonsterMind.SEEN_TEMP_COLD_RES)
			if player.has_cold_immunity:
				monster.mind.set_bits(MonsterMind.SEEN_COLD_IMMUNITY)
		DamageResistance.POISON:
			if player.has_poison_resistance:
				monster.mind.set_bits(MonsterMind.SEEN_POISON_RES)
			if player.timed_poison_resistance != 0:
				monster.mind.set_bits(MonsterMind.SEEN_TEMP_POISON_RES)
		DamageResistance.NETHER:
			if player.has_nether_resistance:
				monster.mind.set_bits(MonsterMind.SEEN_NETHER_RES)
		DamageResistance.LIGHT:
			if player.has_light_resistance:
				monster.mind.set_bits(MonsterMind.SEEN_LIGHT_RES)
		DamageResistance.DARK:
			if player.has_dark_resistance:
				monster.mind.set_bits(MonsterMind.SEEN_DARK_RES)
		DamageResistance.FEAR:
			if player.has_fear_resistance:
				monster.mind.set_bits(MonsterMind.SEEN_FEAR_RES)
		DamageResistance.CONFUSION:
			if player.has_confusion_resistance:
				monster.mind.set_bits(MonsterMind.SEEN_CONF_RES)
		DamageResistance.CHAOS:
			if player.has_chaos_resistance:
				monster.mind.set_bits(MonsterMind.SEEN_CHAOS_RES)
		DamageResistance.DISENCHANTMENT:
			if player.has_disenchant_resistance:
				monster.mind.set_bits(MonsterMind.SEEN_DISEN_RES)
		DamageResistance.BLIND:
			if player.has_blindness_resistance:
				monster.mind.set_bits(MonsterMind.SEEN_BLIND_RES)
		DamageResistance.NEXUS:
			if player.has_nexus_resistance:
				monster.mind.set_bits(MonsterMind.SEEN_NEXUS_RES)
		DamageResistance.SOUND:
			if player.has_sound_resistance:
				monster.mind.set_bits(MonsterMind.SEEN_SOUND_RES)
		DamageResistance.SHARD:
			if player.has_shard_resistance:
				monster.mind.set_bits(MonsterMind.SEEN_SHARD_RES)
		DamageResistance.FREE_ACTION:
			if player.has_free_action:
				monster.mind.set_bits(MonsterMind.SEEN_FREE_ACTION)
		DamageResistance.POWER:
			if player.max_power == 0:
				monster.mind.set_bits(MonsterMind.SEEN_NO_POWER)
		DamageResistance.REFLECTION:
			if player.has_reflection:
				monster.mind.set_bits(MonsterMind.SEEN_REFLECTION)


func _set_dungeon_level(value: DungeonLevel) -> void:
	_dungeon_level = value


func _get_dungeon_level() -> DungeonLevel:
	if _dungeon_level == null:
		_dungeon_level = SaveGame.instance.dungeon_level
	return _dungeon_level


## Place a single specific monster in the dungeon at a specified location
func _place_monster_one(row: int, col: int, type: MonsterType, start_asleep: bool, start_friendly: bool) -> bool:
	# Do some sanity checks to make sure the monster is valid
	if type == null:
		return false
	var name: String = type.name
	if name == "":
		return false
	if name.begins_with("Player"):
		return false
	# Can't place a monster out of bounds
	if not dungeon_level.in_inner_bounds(row, col):
		return false
	# Even if the monster can walk through walls it can't start in a wall or other impassable feature
	if not dungeon_level.grid_passable_no_creature(row, col):
		return false
	# If the monster is a guardian, only place it on its own level and if there aren't already too many
	if type.flags_1.is_set(MonsterFlags1.ONLY_GUARDIAN) or type.flags_1.is_set(MonsterFlags1.GUARDIAN):
		var quest_index: int = SaveGame.instance.quests.get_quest_number()
		if quest_index < 0:
			return false
		var quest: Quest = SaveGame.instance.quests.quests[quest_index]
		if type.index != quest.monster_type_index:
			return false
		if type.cur_num >= quest.to_kill - quest.killed:
			return false
	# Try to get a slot for the monster in the array
	last_placed_monster = get_free_monster_slot()
	if last_placed_monster == 0:
		return false
	# We're committed to placing the monster now
	dungeon_level.get_grid(row, col).monster_index = last_placed_monster
	# If the monster is out of depth, update the level's danger rating
	if type.level > SaveGame.instance.total_difficulty():
		if type.flags_1.is_set(MonsterFlags1.UNIQUE):
			dungeon_level.danger_rating += (type.level - SaveGame.instance.total_difficulty()) * 2
		else:
			dungeon_level.danger_rating += (type.level - SaveGame.instance.total_difficulty())
	# Set the simple monster properties
	var monster: Monster = monsters[last_placed_monster]
	monster.confusion_level = 0
	monster.distance_from_player = 0
	monster.energy = Rng.roll_die(100) - 1
	monster.fear_level = 0
	monster.first_held_item_index = 0
	monster.generation = 1
	monster.individual_monster_flags.clear_all()
	monster.is_visible = false
	monster.map_row = row
	monster.map_col = col
	monster.max_health = Rng.roll_dice_max(type.hit_dice, type.hit_dice_sides) if type.flags_1.is_set(MonsterFlags1.FORCE_MAX_HP) else Rng.roll_dice(type.hit_dice, type.hit_dice_sides)
	monster.health = monster.max_health
	monster.mind.clear_all()
	monster.monster_type = type
	monster.stun_level = 0
	monster.sleep_level = 0
	monster.speed = type.speed
	monster.stolen_gold = 0
	# If the monster starts friendly, let it know that
	if start_friendly:
		monster.mind.set_bits(MonsterMind.FRIENDLY)
	# If the monster should start asleep, and this is valid for its type, set its sleep level
	if start_asleep and type.sleep:
		monster.sleep_level = type.sleep * 2 + Rng.roll_die(type.sleep * 10)
	# Give the monster's speed random nudge if it's not unique
	if type.flags_1.is_clear(MonsterFlags1.UNIQUE):
		var i: int = Tables.EXTRACT_ENERGY[monster.speed] / 10
		if i != 0:
			monster.speed = Rng.random_around(monster.speed, i)
	# If the monster type should start sleepy, then mark it as needing to play nice in its first turn
	if type.flags_1.is_set(MonsterFlags1.FORCE_SLEEP):
		monster.individual_monster_flags.set_bits(IndividualMonsterFlags.NICE)
		repair_monsters = true
	# If the monster's slot hasn't been activated yet this turn, tell it to not act until next turn
	if last_placed_monster < currently_acting_monster:
		monster.individual_monster_flags.set_bits(IndividualMonsterFlags.BORN)
	# We might see the monster arrive
	update_monster_visibility(last_placed_monster, true)
	# There's one more of this type of monster on the level
	type.cur_num += 1
	# If it's a reproducer, there's one more reproducer on the level
	if type.flags_2.is_set(MonsterFlags2.MULTIPLY):
		num_repro += 1
	# If it's mult-hued, then let us know we need to shimmer monsters
	if type.flags_1.is_set(MonsterFlags1.ATTR_MULTI):
		shimmer_monsters = true
	# Monster placed successfully
	return true


func compact_monsters(size: int) -> void:
	var count: int = 0
	var num: int = 0
	while num < size:
		if count > 1000:
			return
		var level_threshold: int = 5 * count
		var distance_threshold: int = 5 * (20 - count)
		for i: int in range (1, _dungeon_level.monster_count_max):
			var monster: Monster = monsters[i]
			var monster_type: MonsterType = monster.monster_type
			if monster_type == null:
				continue
			if monster_type.level > level_threshold:
				continue
			if distance_threshold > 0 and monster.distance_from_player < distance_threshold:
				continue
			var chance: int = 90
			if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
				chance = 99
			if monster_type.flags_1.is_set(MonsterFlags1.GUARDIAN):
				chance = 100
			if Rng.roll_die(100) <= chance:
				continue
			delete_monster_by_index(i, true)
			num += 1
		for i: int in range (_dungeon_level.monster_count_max-1, 0, -1):
			var monster: Monster = monsters[i]
			if monster.monster_type != null:
				continue
			move_compacted_monster(_dungeon_level.monster_count_max - 1, i)
			_dungeon_level.monster_count_max -= 1


func move_compacted_monster(index_from: int, index_to: int)->void:
	if index_from == index_to:
		return
	var monster: Monster = monsters[index_from]
	var row: int = monster.map_row
	var col: int = monster.map_col
	var tile: GridTile = _dungeon_level.get_grid(row, col)
	tile.monster_index = index_to
	var item_index: int = monster.first_held_item_index
	while item_index != 0:
		var item: Item = _dungeon_level.dungeon_items.items[item_index]
		item.holding_monster_index = index_to
		item_index = item.next_in_stack
	if SaveGame.instance.target_who == index_from:
		SaveGame.instance.target_who = index_to
	if SaveGame.instance.tracked_monster_index == index_from:
		SaveGame.instance.health_track(index_to)
	monsters[index_to] = monsters[index_from]
	monsters[index_from] = Monster.new()
