# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name SaveGameFactory
extends RefCounted

var _savegame: SaveGame


func _init(game: SaveGame) -> void:
	_savegame = game


func initialise_savegame() -> void:
	# Set up a seed for our flavours
	_savegame.seed_flavour = Rng.big_number()
	flavour_init()
	apply_flavour_visuals()
	# Set up the monster and item tables
	set_up_monster_and_item_tables()
	# Set up the dungeons, towns, and wilderness
	_savegame.dungeons = Dungeon.new_dungeon_list()
	_savegame.island = Island.new_island()
	_savegame.towns = Town.new_town_list()
	for town: Town in _savegame.towns:
		for store: Store in town.stores:
			store.store_init()
			store.store_maint()
	create_world()
	_savegame.quests = QuestList.create_new_quest_list()
	_savegame.quests.initialise_quests()
	# Reset which items we're suppressing
	_savegame.item_types.reset_stompability()
	# We start on the surface, in a random town
	_savegame.current_depth = 0
	_savegame.current_town = _savegame.towns[Rng.roll_die(_savegame.towns.size()) - 1]
	while _savegame.current_town.character == "K" or _savegame.current_town.character == "N":
		_savegame.current_town = _savegame.towns[Rng.roll_die(_savegame.towns.size()) - 1]
	_savegame.current_dungeon = _savegame.dungeons[_savegame.current_town.index]
	_savegame.player.wilderness_row = _savegame.current_town.wild_row
	_savegame.player.wilderness_col = _savegame.current_town.wild_col
	_savegame.recall_dungeon = _savegame.current_dungeon.index
	_savegame.dungeon_difficulty = _savegame.current_dungeon.offset
	_savegame.came_from = LevelStart.START_RANDOM
	_savegame.dungeon_level = null
	_savegame.player.max_dlv[_savegame.current_town.index] = 1

func create_world() -> void:
	# Initialise the regions, dungeons and towns
	for row: int in 12:
		for col: int in 12:
			var region: WildernessRegion = _savegame.island.get_region(row, col)
			region.random_seed = Rng.big_number()
			region.dungeon = null
			region.town = null
			region.road_map = 0
	for i: int in _savegame.towns.size():
		_savegame.towns[i].random_seed = Rng.big_number()
		_savegame.towns[i].visited = false
		_savegame.towns[i].wild_row = 0
		_savegame.towns[i].wild_col = 0
	for i: int in _savegame.dungeons.size():
		_savegame.dungeons[i].wild_row = 0
		_savegame.dungeons[i].wild_col = 0
	# Place the dungeons randomly on the island
	for i: int in _savegame.dungeons.size():
		_savegame.dungeons[i].visited = false
		_savegame.dungeons[i].known_depth = false
		_savegame.dungeons[i].known_offset = false
		var dungeon_row: int
		var dungeon_col: int
		if i < _savegame.towns.size():
			# If it's a town dungeon, it can't be next to another town or dungeon
			var placed: bool = false
			while not placed:
				placed = true
				dungeon_row = Rng.roll_die(8) + 1
				dungeon_col = Rng.roll_die(8) + 1
				for row_offset: int in range(-1, 2):
					for col_offset: int in range(-1, 2):
						if _savegame.island.get_region(dungeon_row + row_offset, dungeon_col + col_offset).dungeon != null:
							placed = false
		else:
			# If it's not a town dungeon, it can't be on top of another town or dungeon
			var placed: bool = false
			while not placed:
				placed = true
				dungeon_row = Rng.roll_die(8) + 1
				dungeon_col = Rng.roll_die(8) + 1
				if _savegame.island.get_region(dungeon_row, dungeon_col).dungeon != null:
					placed = false
		# Place the dungeon
		_savegame.island.get_region(dungeon_row, dungeon_col).dungeon = _savegame.dungeons[i]
		_savegame.dungeons[i].wild_row = dungeon_row
		_savegame.dungeons[i].wild_col = dungeon_col
		# If it is a town dungeon, place the town in the same place
		if i < _savegame.towns.size():
			_savegame.island.get_region(dungeon_row, dungeon_col).town = _savegame.towns[i]
			_savegame.towns[i].wild_row = dungeon_row
			_savegame.towns[i].wild_col = dungeon_col
	# Create roads between the towns
	for i: int in (_savegame.towns.size() - 1):
		var cur_row: int = _savegame.towns[i].wild_row
		var cur_col: int = _savegame.towns[i].wild_col
		var dest_row: int = _savegame.towns[i + 1].wild_row
		var dest_col: int = _savegame.towns[i + 1].wild_col
		var finished: bool = false
		while not finished:
			# Work out how far we are from the next town
			var row_disp: int = dest_row - cur_row
			var row_sign: int = signi(row_disp)
			row_disp = absi(row_disp)
			var col_disp: int = dest_col - cur_col
			var col_sign: int = signi(col_disp)
			col_disp = absi(col_disp)
			if row_disp == 0 and col_disp == 0:
				# We're at the next town
				finished = true
			else:
				# We're not there yet
				var cur_dir: int
				var next_dir: int
				if (row_disp == col_disp) and col_sign == 1 and row_sign == -1:
					cur_dir = RoadDirection.ROAD_UP
					next_dir = RoadDirection.ROAD_DOWN
				elif col_sign == 1 and col_disp >= row_disp:
					cur_dir = RoadDirection.ROAD_RIGHT
					next_dir = RoadDirection.ROAD_LEFT
				elif row_sign == 1 and row_disp >= col_disp:
					cur_dir = RoadDirection.ROAD_DOWN
					next_dir = RoadDirection.ROAD_UP
				elif col_sign == -1 and col_disp >= row_disp:
					cur_dir = RoadDirection.ROAD_LEFT
					next_dir = RoadDirection.ROAD_RIGHT
				else:
					cur_dir = RoadDirection.ROAD_UP
					next_dir = RoadDirection.ROAD_DOWN
				_savegame.island.get_region(cur_row, cur_col).road_map |= cur_dir
				match cur_dir:
					RoadDirection.ROAD_UP:
						cur_row -= 1
					RoadDirection.ROAD_DOWN:
						cur_row += 1
					RoadDirection.ROAD_LEFT:
						cur_col -= 1
					RoadDirection.ROAD_RIGHT:
						cur_col += 1
				_savegame.island.get_region(cur_row, cur_col).road_map |= next_dir


func flavour_init() -> void:
	Rng.use_fixed_seed(_savegame.seed_flavour)
	var temp_flavours: Array[Flavour]
	# All amulet flavours get shuffled
	_savegame.amulet_flavours = []
	for name: String in StaticData.amulet_flavours:
		temp_flavours.append(StaticData.amulet_flavours[name])
	while temp_flavours.size() > 0:
		var index: int = Rng.roll_die(temp_flavours.size()) - 1
		_savegame.amulet_flavours.append(temp_flavours[index])
		temp_flavours.remove_at(index)
	# All mushroom flavours get shuffled
	_savegame.mushroom_flavours = []
	for name: String in StaticData.mushroom_flavours:
		temp_flavours.append(StaticData.mushroom_flavours[name])
	while temp_flavours.size() > 0:
		var index: int = Rng.roll_die(temp_flavours.size()) - 1
		_savegame.mushroom_flavours.append(temp_flavours[index])
		temp_flavours.remove_at(index)
	# The first three potions (water, apple juice, and slime mould juice, are always the same
	_savegame.potion_flavours = []
	_savegame.potion_flavours.append(StaticData.potion_flavours["Clear"])
	_savegame.potion_flavours.append(StaticData.potion_flavours["Light Brown"])
	_savegame.potion_flavours.append(StaticData.potion_flavours["Icky Green"])
	# The rest of the potions get shuffled
	for name: String in StaticData.potion_flavours:
		if name == "Clear":
			continue
		if name == "Light Brown":
			continue
		if name == "Icky Green":
			continue
		temp_flavours.append(StaticData.potion_flavours[name])
	while temp_flavours.size() > 0:
		var index: int = Rng.roll_die(temp_flavours.size()) - 1
		_savegame.potion_flavours.append(temp_flavours[index])
		temp_flavours.remove_at(index)
	# All ring flavours get shuffled
	_savegame.ring_flavours = []
	for name: String in StaticData.ring_flavours:
		temp_flavours.append(StaticData.ring_flavours[name])
	while temp_flavours.size() > 0:
		var index: int = Rng.roll_die(temp_flavours.size()) - 1
		_savegame.ring_flavours.append(temp_flavours[index])
		temp_flavours.remove_at(index)
	# All rod flavours get shuffled
	_savegame.rod_flavours = []
	for name: String in StaticData.rod_flavours:
		temp_flavours.append(StaticData.rod_flavours[name])
	while temp_flavours.size() > 0:
		var index: int = Rng.roll_die(temp_flavours.size()) - 1
		_savegame.rod_flavours.append(temp_flavours[index])
		temp_flavours.remove_at(index)
	# All staff flavours get shuffled
	_savegame.staff_flavours = []
	for name: String in StaticData.staff_flavours:
		temp_flavours.append(StaticData.staff_flavours[name])
	while temp_flavours.size() > 0:
		var index: int = Rng.roll_die(temp_flavours.size()) - 1
		_savegame.staff_flavours.append(temp_flavours[index])
		temp_flavours.remove_at(index)
	# All wand flavours get shuffled
	_savegame.wand_flavours = []
	for name: String in StaticData.wand_flavours:
		temp_flavours.append(StaticData.wand_flavours[name])
	while temp_flavours.size() > 0:
		var index: int = Rng.roll_die(temp_flavours.size()) - 1
		_savegame.wand_flavours.append(temp_flavours[index])
		temp_flavours.remove_at(index)
	# Scrolls work differently - there are only two flavours and we need to randomise the names
	_savegame.scroll_flavours = []
	for name: String in StaticData.scroll_flavours:
		temp_flavours.append(StaticData.scroll_flavours[name])
	for scroll_index: int in Constants.MAX_SCROLLS:
		var flavour: Flavour = Flavour.new()
		_savegame.scroll_flavours.append(flavour)
		var index: int = Rng.roll_die(temp_flavours.size()) - 1
		flavour.character = temp_flavours[index].character
		flavour.colour = temp_flavours[index].colour
		while true:
			# Repeat until we get an acceptable name
			var scroll_name: String = ""
			while true:
				# Keep adding words to the name
				var word: String = ""
				var num_syllables: int = 1 if Rng.roll_chance(30) else 2
				for s: int in num_syllables:
					# Word can be one or two random syllables
					word += Flavour.SCROLL_SYLLABLES[Rng.roll_die(Flavour.SCROLL_SYLLABLES.size()) - 1]
				# Quit adding words when it would make the name too long
				if scroll_name.length() + word.length() > 14:
					break
				# Add the word and loop back for another
				scroll_name += " "
				scroll_name += word
			# Assign the new name
			flavour.name = scroll_name.strip_edges()
			var okay: bool = true
			# Check that we don't have a clash with an existing name
			for check: int in scroll_index:
				var start_1: String = _savegame.scroll_flavours[scroll_index].name.substr(0, 4)
				var start_2: String = _savegame.scroll_flavours[check].name.substr(0, 4)
				if start_1 != start_2:
					# No clash yet, so keep checking
					continue
				# We have a clash, so no point checking further - mark it as not okay and break
				okay = false
				break
			# If the name was okay, break, otherwise loop by default back to choosing a new name
			if okay:
				break
	# We no longer need the fixed RNG#
	Rng.use_main_sequence()
	# Initialise flavoured and easy-know items
	for i: int in _savegame.item_types.types.size():
		var type: ItemType = _savegame.item_types.types[i]
		if type.name == "":
			continue
		type.has_flavour = Inventory.item_type_has_flavour(type)
		if not type.has_flavour:
			type.flavour_aware = true
		type.easy_know = Inventory.item_type_is_easy_know(i)


func apply_flavour_visuals() -> void:
	for i: int in _savegame.item_types.types.size():
		var type: ItemType = _savegame.item_types.types[i]
		var flavour: Flavour = object_flavour(type)
		if flavour != null:
			type.character = flavour.character
			type.colour = flavour.colour


func object_flavour(type: ItemType) -> Flavour:
	if type.has_flavour:
		var sub: int = type.sub_category
		match type.item_category:
			ItemCategory.AMULET:
				return _savegame.amulet_flavours[sub]
			ItemCategory.FOOD:
				return _savegame.mushroom_flavours[sub]
			ItemCategory.POTION:
				return _savegame.potion_flavours[sub]
			ItemCategory.RING:
				return _savegame.ring_flavours[sub]
			ItemCategory.ROD:
				return _savegame.rod_flavours[sub]
			ItemCategory.SCROLL:
				return _savegame.scroll_flavours[sub]
			ItemCategory.STAFF:
				return _savegame.staff_flavours[sub]
			ItemCategory.WAND:
				return _savegame.wand_flavours[sub]
	return null


func set_up_monster_and_item_tables() -> void:
	var entries_at_level_total: Array[int]
	var entries_at_level_so_far: Array[int]
	for i: int in DungeonLevel.MAX_DEPTH:
		entries_at_level_total.append(0)
		entries_at_level_so_far.append(0)
	_savegame.random_item_table_size = 0
	# Count the item types that can appear at each level
	for item_type: ItemType in _savegame.item_types.types:
		# Each item has up to four levels it can appear at
		for j: int in 4:
			if item_type.chance[j] != 0:
				_savegame.random_item_table_size += 1
				entries_at_level_total[item_type.locale[j]] += 1
	# Add the item types for each level to get cumulative indices into the allocation table
	for i: int in DungeonLevel.MAX_DEPTH:
		entries_at_level_total[i] = entries_at_level_total[i] + entries_at_level_total[i - 1]
	_savegame.random_item_table = []
	# Build the table to the correct size
	for i: int in _savegame.random_item_table_size:
		_savegame.random_item_table.append(FindChance.new())
	# Run through the item types, adding them to the table
	for index: int in _savegame.item_types.types.size():
		var item_type: ItemType = _savegame.item_types.types[index]
		# Each item type has up to four levels it can appear at
		for j: int in 4:
			if item_type.chance[j] > 0:
				var level: int = item_type.locale[j]
				var probability: int = 100 / item_type.chance[j]
				var table_index: int = 0
				if level > 0:
					table_index = entries_at_level_total[level - 1]
				table_index = table_index + entries_at_level_so_far[level]
				_savegame.random_item_table[table_index].index = index
				_savegame.random_item_table[table_index].level = level
				_savegame.random_item_table[table_index].base_probability = probability
				_savegame.random_item_table[table_index].filtered_probability = probability
				_savegame.random_item_table[table_index].final_probability = probability
				entries_at_level_so_far[level] += 1
	# Monsters are similar to items, but simpler
	for i: int in DungeonLevel.MAX_DEPTH:
		entries_at_level_total[i] = 0
		entries_at_level_so_far[i] = 0
	_savegame.random_monster_table_size = 0
	# Count the monster types that can appear at each level
	for monster_type: MonsterType in Profile.instance.monster_types.types:
		_savegame.random_monster_table_size += 1
		entries_at_level_total[monster_type.level] += 1
	# Add the monster types for each level to get cumulative indices into the allocation table
	for i: int in DungeonLevel.MAX_DEPTH:
		entries_at_level_total[i] = entries_at_level_total[i] + entries_at_level_total[i - 1]
	_savegame.random_monster_table = []
	# Build the table to the correct size
	for i: int in _savegame.random_monster_table_size:
		_savegame.random_monster_table.append(FindChance.new())
	# Run through the monster types, adding them to the table
	for index: int in Profile.instance.monster_types.types.size():
		var monster_type: MonsterType = Profile.instance.monster_types.types[index]
		if monster_type.rarity != 0:
			var level: int = monster_type.level
			var probability: int = 100 / monster_type.rarity
			var table_index: int = 0
			if level > 0:
				table_index = entries_at_level_total[level - 1]
			table_index = table_index + entries_at_level_so_far[level]
			_savegame.random_monster_table[table_index].index = index
			_savegame.random_monster_table[table_index].level = level
			_savegame.random_monster_table[table_index].base_probability = probability
			_savegame.random_monster_table[table_index].filtered_probability = probability
			_savegame.random_monster_table[table_index].final_probability = probability
			entries_at_level_so_far[level] += 1
