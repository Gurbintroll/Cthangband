# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name DungeonLevel
extends Resource

const MAX_HGT: int = 126
const MAX_WID: int = 198
const MAX_DEPTH: int = 128
const MONSTER_CHARACTERS: String = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"
const ITEM_CHARACTERS: String = "?/|\\\"!$()_-=[]{},~"
const MAX_SPELL_RANGE: int = 18
const MAX_SIGHT_RADIUS: int = 20
const OVER_SIGHT_RADIUS: int = 30 # Always MAX_SIGHT_RADIUS * 1.5
const VIEW_MAX: int = 1536
const LIGHT_MAX: int = 128
const SCENT_MAX: int = 1536
const SCENT_LIMIT: int = 32
const ordered_direction: Array[int] = [2, 8, 6, 4, 3, 1, 9, 7, 5]
const ordered_direction_col_offset: Array[int] = [0, 0, 1, -1, 1, -1, 1, -1, 0]
const ordered_direction_row_offset: Array[int] = [1, -1, 0, 0, 1, 1, -1, -1, 0]
const keypad_direction_row_offset: Array[int] = [0, 1, 1, 1, 0, 0, 0, -1, -1, -1]
const keypad_direction_col_offset: Array[int] = [0, -1, 0, 1, -1, 0, 1, -1, 0, 1]

@export var cur_hgt: int
@export var cur_wid: int
@export var item_count: int
@export var item_count_max: int
@export var monster_count: int
@export var monster_count_max: int
@export var opening_chest: bool
@export var panel_col: int
@export var panel_col_min: int
@export var panel_col_max: int
@export var panel_col_print: int
@export var max_panel_cols: int
@export var panel_row: int
@export var panel_row_min: int
@export var panel_row_max: int
@export var panel_row_print: int
@export var max_panel_rows: int
@export var inner_grid: Array[GridTile]
@export var dungeon_items: DungeonItems
@export var dungeon_monsters: DungeonMonsters
@export var treasure_feeling: int
@export var treasure_rating: int
@export var special_treasure: bool
@export var danger_feeling: int
@export var danger_rating: int
@export var special_danger: bool
@export var monster_level: int
@export var item_level: int
@export var player: Player
# Remember the locations we can see so we can clear that visibility when we move
@export var visible_locations: Array[Location]
@export var visible_locations_count: int
@export var locations_for_redraw: Array[Location]
@export var locations_for_redraw_count: int
# Remember the location that are lit by the player, so we can clear that when we move
@export var lit_locations: Array[Location]
@export var lit_locations_count: int
@export var time_since_scent_update: int
@export var last_scent_index: int
@export var first_scent_index: int


static func new_dungeon_level() -> DungeonLevel:
	var dungeon_level: DungeonLevel = DungeonLevel.new()
	dungeon_level.player = SaveGame.instance.player
	dungeon_level.dungeon_monsters = DungeonMonsters.new_dungeon_monsters(dungeon_level)
	dungeon_level.dungeon_items = DungeonItems.new_dungeon_items(dungeon_level)
	# We have fixed arrays that we re-use to avoid lots of memory allocation
	for i: int in VIEW_MAX:
		dungeon_level.visible_locations.append(Location.new())
		dungeon_level.locations_for_redraw.append(Location.new())
	for i: int in LIGHT_MAX:
		dungeon_level.lit_locations.append(Location.new())
	return dungeon_level


# Formerly InBounds2
func in_outer_bounds(row: int, col: int) -> bool:
	return (row >= 0 and row < cur_hgt and col >= 0 and col < cur_wid)


# Formerly InBounds
func in_inner_bounds(row: int, col: int) -> bool:
	return (row >= 1 and row < cur_hgt - 1 and col >= 1 and col < cur_wid - 1)


func acquirement(row:int, col:int, num:int, great:bool)->void:
	for i:int in num:
		var item:Item = Item.new()
		if not item.make_item(true, great):
			continue
		dungeon_items.drop_near(item, -1, row, col)


func forget_light() -> void:
	if lit_locations_count == 0:
		return
	for index: int in lit_locations_count:
		var location: Location = lit_locations[index]
		get_grid(location.row, location.col).tile_flags.clear_bits(GridTile.PLAYER_LIT)
		redraw_single_location(location.row, location.col)
	lit_locations_count = 0


func forget_view() -> void:
	if visible_locations_count == 0:
		return
	for index: int in visible_locations_count:
		var location: Location = visible_locations[index]
		get_grid(location.row, location.col).tile_flags.clear_bits(GridTile.IS_VISIBLE)
		redraw_single_location(location.row, location.col)
	visible_locations_count = 0


func floor_item_describe(item_index:int)->void:
	var item:Item = dungeon_items.items[item_index]
	var item_name: String = item.description(true, 3)
	Profile.instance.message_print("You see %s." % [item_name])


func floor_item_increase(item_index:int, num:int)->void:
	var item:Item = dungeon_items.items[item_index]
	num += item.count
	if num > 255:
		num = 255
	elif num < 0:
		num = 0
	num -= item.count
	item.count += num

func floor_item_optimise(item_index:int)->void:
	var item:Item = dungeon_items.items[item_index]
	if item.item_type == null:
		return
	if item.count != 0:
		return
	delete_item_by_index(item_index)



func print_character_at_map_location(character: String, ink: int, row: int, col: int) -> void:
	if panel_contains(row, col):
		if player.timed_invulnerability != 0:
			ink = Ink.WHITE
		elif player.timed_etherealness != 0:
			ink = Ink.BLACK
		Console.print_at(row - panel_row_print, col - panel_col_print, character, ink)


func update_light() -> void:
	# If we have no light, it's easy
	if player.light_level == 0:
		forget_light()
		redraw_single_location(player.map_row, player.map_col)
	# Wipe out our previous lit locations, remembering them for redraw
	for i: int in lit_locations_count:
		var location: Location = lit_locations[i]
		get_grid(location.row, location.col).tile_flags.clear_bits(GridTile.PLAYER_LIT)
		get_grid(location.row, location.col).tile_flags.set_bits(GridTile.NEEDS_REDRAW)
		locations_for_redraw[locations_for_redraw_count].row = location.row
		locations_for_redraw[locations_for_redraw_count].col = location.col
		locations_for_redraw_count += 1
	lit_locations_count = 0
	set_lit(player.map_row, player.map_col)
	# We can brute-force the first couple of radii
	if player.light_level >= 1:
		set_lit(player.map_row + 1, player.map_col)
		set_lit(player.map_row - 1, player.map_col)
		set_lit(player.map_row, player.map_col + 1)
		set_lit(player.map_row, player.map_col - 1)
		set_lit(player.map_row + 1, player.map_col + 1)
		set_lit(player.map_row + 1, player.map_col - 1)
		set_lit(player.map_row - 1, player.map_col + 1)
		set_lit(player.map_row - 1, player.map_col - 1)
	if player.light_level >= 2:
		if grid_passable(player.map_row + 1, player.map_col):
			set_lit(player.map_row + 2, player.map_col)
			set_lit(player.map_row + 2, player.map_col + 1)
			set_lit(player.map_row + 2, player.map_col - 1)
		if grid_passable(player.map_row - 1, player.map_col):
			set_lit(player.map_row - 2, player.map_col)
			set_lit(player.map_row - 2, player.map_col + 1)
			set_lit(player.map_row - 2, player.map_col - 1)
		if grid_passable(player.map_row, player.map_col + 1):
			set_lit(player.map_row - 1, player.map_col + 2)
			set_lit(player.map_row, player.map_col + 2)
			set_lit(player.map_row + 1, player.map_col + 2)
		if grid_passable(player.map_row, player.map_col - 1):
			set_lit(player.map_row - 1, player.map_col - 2)
			set_lit(player.map_row, player.map_col - 2)
			set_lit(player.map_row + 1, player.map_col - 2)
	# With a radius of 3 or more we can brute force the initial corners
	if player.light_level >= 3:
		var p: int = mini(player.light_level, 5)
		if grid_passable(player.map_row + 1, player.map_col + 1):
			set_lit(player.map_row + 2, player.map_col + 2)
		if grid_passable(player.map_row + 1, player.map_col - 1):
			set_lit(player.map_row + 2, player.map_col - 2)
		if grid_passable(player.map_row - 1, player.map_col + 1):
			set_lit(player.map_row - 2, player.map_col + 2)
		if grid_passable(player.map_row - 1, player.map_col - 1):
			set_lit(player.map_row - 2, player.map_col - 2)
		# The rest must be calculated
		var min_row: int = clampi(player.map_row - p, 0, cur_hgt - 1)
		var max_row: int = clampi(player.map_row + p, 0, cur_hgt - 1)
		var min_col: int = clampi(player.map_col - p, 0, cur_wid - 1)
		var max_col: int = clampi(player.map_col + p, 0, cur_wid - 1)
		for row: int in range(min_row, max_row + 1):
			for col: int in range(min_col, max_col + 1):
				var d_row: int = absi(player.map_row - row)
				var d_col: int = absi(player.map_col - col)
				if d_row <= 2 and d_col <= 2:
					continue
				var d: int = (d_row + floor(d_col / 2)) if d_row > d_col else (d_col + floor(d_row / 2))
				if d > p:
					continue
				if player_has_los_bold(row, col):
					set_lit(row, col)
	# Now perform the actual changes
	for i: int in lit_locations_count:
		var location: Location = lit_locations[i]
		if get_grid(location.row, location.col).tile_flags.is_set(GridTile.NEEDS_REDRAW):
			continue
		note_spot(location.row, location.col)
		redraw_single_location(location.row, location.col)
	for i: int in locations_for_redraw_count:
		var location: Location = locations_for_redraw[i]
		get_grid(location.row, location.col).tile_flags.clear_bits(GridTile.NEEDS_REDRAW)
		if get_grid(location.row, location.col).tile_flags.is_set(GridTile.PLAYER_LIT):
			continue
		redraw_single_location(location.row, location.col)
	locations_for_redraw_count = 0


# Update scent trail on locations that need redrawing
func update_scent_trail() -> void:
	# We'll be using the locations_for_redraw array for scent, so make sure it exists
	if locations_for_redraw_count != 0:
		return
	# If it's been a while since the last scent update, age all the scent records and reset the timer
	if time_since_scent_update == 255:
		for row: int in cur_hgt:
			for col: int in cur_wid:
				var w: int = get_grid(row, col).scent_age
				get_grid(row, col).scent_age = w - 128 if w > 128 else 0
		time_since_scent_update = 127
	# It's been another turn since scent update
	time_since_scent_update += 1
	# Start at the beginning of the redraw array
	last_scent_index = 0
	first_scent_index = 0
	# Update the scent on the player's location, which will also add adjacent locations to the list
	update_scent_on_grid(player.map_row, player.map_col, 0)
	# If we have had more locations added, process them too
	while last_scent_index != first_scent_index:
		# Get the location to process
		var row: int = locations_for_redraw[first_scent_index].row
		var col: int = locations_for_redraw[first_scent_index].col
		# Move to the next location in the array, looping back to the beginning if necessary
		first_scent_index += 1
		if first_scent_index == SCENT_MAX:
			first_scent_index = 0
		# Update the scent on the locations around the one we're procesing (which may add more to the list)
		for d: int in 8:
			update_scent_on_grid(row + ordered_direction_row_offset[d], col + ordered_direction_col_offset[d], get_grid(row, col).scent_strength + 1)
	last_scent_index = 0
	first_scent_index = 0


# Update the scent at a single location
func update_scent_on_grid(row: int, col: int, scent_strength: int) -> void:
	var old_last_scent_index: int = last_scent_index
	# Get the tile we should be updating
	var tile: GridTile = get_grid(row, col)
	# If we've already updated the tile this pass, exit
	if tile.scent_age == time_since_scent_update:
		return
	# If the tile isn't open, exit
	if tile.feature_type.blocks_los and tile.feature_type.name != "SecretDoor":
		return
	# Update the scent on this tile and mark it as having been updated this pass
	tile.scent_age = time_since_scent_update
	tile.scent_strength = scent_strength
	# If we're at our scent strength limit (higher = weaker scent) then exit
	if scent_strength == SCENT_LIMIT:
		return
	# The scent will spread from here, so add the location to the array
	locations_for_redraw[last_scent_index].row = row
	locations_for_redraw[last_scent_index].col = col
	# We've added the location, so adjust the index, looping if necessary
	last_scent_index += 1
	if last_scent_index == SCENT_MAX:
		last_scent_index = 0
	# If we've spread so far that we've caught up with ourselves, stop don't spread any further
	if last_scent_index == first_scent_index:
		last_scent_index = old_last_scent_index


func update_monsters(update_distances: bool) -> void:
	for i: int in dungeon_monsters.monsters.size():
		var monster: Monster = dungeon_monsters.monsters[i]
		if monster.monster_type == null:
			continue
		dungeon_monsters.update_monster_visibility(i, update_distances)


func get_grid(row: int, col: int) -> GridTile:
	return inner_grid[row * MAX_WID + col]


func reset_grid(background_feature: String) -> void:
	if inner_grid == []:
		for index: int in MAX_HGT * MAX_WID:
			inner_grid.append(GridTile.new())
	for index: int in MAX_HGT * MAX_WID:
		inner_grid[index].set_background_feature(background_feature)
		inner_grid[index].set_feature("Nothing")
		inner_grid[index].tile_flags.clear_all()
		inner_grid[index].item_index = 0
		inner_grid[index].monster_index = 0
		inner_grid[index].scent_strength = 0
		inner_grid[index].scent_age = 0


func grid_open_no_item_or_creature(row: int, col: int) -> bool:
	var grid: GridTile = get_grid(row, col)
	return grid.feature_type.is_open_floor and grid.item_index == 0 and grid.monster_index == 0


func grid_open_no_item(row: int, col: int) -> bool:
	var grid: GridTile = get_grid(row, col)
	return grid.feature_type.is_open_floor and grid.item_index == 0


func grid_passable(row: int, col: int) -> bool:
	return get_grid(row, col).feature_type.is_passable


func grid_passable_no_creature(row: int, col: int) -> bool:
	var grid: GridTile = get_grid(row, col)
	return grid.feature_type.is_passable and grid.monster_index == 0


func player_has_los_bold(row: int, col: int) -> bool:
	return get_grid(row, col).tile_flags.is_set(GridTile.IS_VISIBLE)


func print_map() -> void:
	for row: int in range(panel_row_min, panel_row_max + 1):
		for col: int in range(panel_col_min, panel_col_max + 1):
			var visual: Visual = map_info(row, col)
			if player.timed_invulnerability > 0:
				visual.ink = Ink.WHITE
			if player.timed_etherealness > 0:
				visual.ink = Ink.BLACK
			Console.print_visual_at(row - panel_row_print, col - panel_col_print, visual)


func panel_contains(row: int, col: int) -> bool:
	return row >= panel_row_min and row <= panel_row_max and col >= panel_col_min and col <= panel_col_max


func redraw_single_location(row: int, col: int) -> void:
	if panel_contains(row, col):
		var visual: Visual = map_info(row, col)
		if player.timed_invulnerability != 0:
			visual.ink = Ink.WHITE
		if player.timed_etherealness != 0:
			visual.ink = Ink.BLACK
		Console.print_visual_at(row - panel_row_print, col - panel_col_print, visual)


func map_info(row: int, col: int) -> Visual:
	var visual: Visual = Visual.new()
	var grid: GridTile = get_grid(row, col)
	# First, deal with the base feature of the tile
	var feature: FloorTileType = grid.feature_type
	if feature.is_open_floor:
		# We need to a) have memorised it or b) it needs to be lit and visible and we need to not be blind
		if grid.tile_flags.is_set(GridTile.PLAYER_MEMORISED) or (grid.tile_flags.is_set(GridTile.PLAYER_LIT) or grid.tile_flags.is_set(GridTile.SELF_LIT) and grid.tile_flags.is_set(GridTile.IS_VISIBLE)) and player.timed_blindness == 0:
			# Start by simply assuming the floor's character and colour
			visual.character = feature.character
			visual.ink = feature.colour
			# If it dims outside line of sight, the deal with its status
			if feature.dims_outside_los:
				# If we're blind, then it's black
				if player.timed_blindness != 0:
					visual.ink = Ink.BLACK
				# If it's in our light radius and it yellows, then yellow it
				elif grid.tile_flags.is_set(GridTile.PLAYER_LIT):
					if feature.yellow_in_torch_light:
						visual.ink = Ink.BRIGHT_YELLOW
				# If it's not self-lit, then it's black
				elif grid.tile_flags.is_clear(GridTile.SELF_LIT):
					visual.ink = Ink.BLACK
				# If it's not visible, then dim its colour
				elif grid.tile_flags.is_clear(GridTile.IS_VISIBLE):
					visual.ink = Ink.dim_colour(visual.ink)
				# If it's on the edge of our Detect Traps radius, then override the colour
				if grid.tile_flags.is_set(GridTile.TRAPS_DETECTED):
					var count: int = 0
					if get_grid(row + 1, col).tile_flags.is_set(GridTile.TRAPS_DETECTED):
						count += 1
					if get_grid(row - 1, col).tile_flags.is_set(GridTile.TRAPS_DETECTED):
						count += 1
					if get_grid(row, col + 1).tile_flags.is_set(GridTile.TRAPS_DETECTED):
						count += 1
					if get_grid(row, col - 1).tile_flags.is_set(GridTile.TRAPS_DETECTED):
						count += 1
					if count != 4:
						visual.ink = Ink.BRIGHT_CHARTREUSE
		else:
			# It's open floor but not visible or memorised
			visual.character = StaticData.tile_types["Nothing"].character
			visual.ink = StaticData.tile_types["Nothing"].colour
	else:
		# It's not an open floor
		if grid.tile_flags.is_set(GridTile.PLAYER_MEMORISED):
			# If we know the tile, see what it appears as (e.g., a trap might appear as something else)
			var appear_as: String = feature.appear_as if feature.appear_as != "" else grid.background_feature.appear_as
			feature = StaticData.tile_types[appear_as]
			# Start by simply assuming the floor's character and colour
			visual.character = feature.character
			visual.ink = feature.colour
			# If it dims outside line of sight, the deal with its status
			if feature.dims_outside_los:
				# If we're blind, then it's black
				if player.timed_blindness != 0:
					visual.ink = Ink.BLACK
				# If it's in our light radius and it yellows, then yellow it
				elif grid.tile_flags.is_set(GridTile.PLAYER_LIT):
					if feature.yellow_in_torch_light:
						visual.ink = Ink.BRIGHT_YELLOW
				else:
					if grid.tile_flags.is_clear(GridTile.IS_VISIBLE):
						visual.ink = Ink.dim_colour(visual.ink)
					elif grid.tile_flags.is_clear(GridTile.SELF_LIT):
						visual.ink = Ink.dim_colour(visual.ink)
					else:
						# Dim the tile if the tile one nearer to the player is not self-lit
						var test_row: int = (row + 1) if row < player.map_row else (row - 1) if row > player.map_row else row
						var test_col: int = (col + 1) if col < player.map_col else (col - 1) if col > player.map_col else col
						if get_grid(test_row, test_col).tile_flags.is_clear(GridTile.SELF_LIT):
							visual.ink = Ink.dim_colour(visual.ink)
		else:
			# It's not open floor but not visible or memorised
			visual.character = StaticData.tile_types["Nothing"].character
			visual.ink = StaticData.tile_types["Nothing"].colour
	# We now have the basic feature in the correct colour based on lighting and visibility
	if player.timed_hallucinations != 0 and Rng.roll_die(256) == 1 and not grid.feature_type.is_wall:
		# If we're hallucinating and it's not a wall, have a small chance of being random
		visual = hallucinate_something(visual)
	# We need to check if there is an items on the tile, because if so then it will
	# override the tiles basic feature
	var item_index: int = grid.item_index
	# Run through the stack of items on the tile looking for one the player has seen
	while item_index != 0:
		var item: Item = dungeon_items.items[item_index]
		var next_item_index: int = item.next_in_stack
		if item.marked:
			visual.character = item.item_type.character
			visual.ink = item.item_type.colour
			# If we're hallucinating, make it random
			if player.timed_hallucinations != 0:
				visual = hallucinate_item(visual)
			break
		item_index = next_item_index
	# Monsters override items and basic tiles
	if grid.monster_index != 0:
		var monster: Monster = dungeon_monsters.monsters[grid.monster_index]
		# Store the monster visual in a separate variable because we might not be using it
		var monster_visual: Visual = Visual.new()
		if monster.is_visible:
			var monster_type: MonsterType = monster.monster_type
			monster_visual.character = monster_type.character
			monster_visual.ink = monster_type.colour
			# If multi-hued, change the colour
			if monster_type.flags_1.is_set(MonsterFlags1.ATTR_MULTI):
				# Shapechanger is only valid with multi-hued
				if monster_type.flags_2.is_set(MonsterFlags2.SHAPECHANGER):
					if Rng.roll_die(25) == 11:
						monster_visual.character = ITEM_CHARACTERS[Rng.roll_die(ITEM_CHARACTERS.length()) - 1]
					else:
						monster_visual.character = MONSTER_CHARACTERS[Rng.roll_die(MONSTER_CHARACTERS.length()) - 1]
				else:
					visual.character = monster_visual.character
				# Some monsters can be any colour except for background
				if monster_type.flags_2.is_set(MonsterFlags2.ATTR_ANY):
					visual.ink = Rng.roll_die(31)
				# Other multi-hued are just basic dragon/element colours
				else:
					match Rng.roll_die(7):
						1:
							visual.ink = Ink.RED
						2:
							visual.ink = Ink.BRIGHT_RED
						3:
							visual.ink = Ink.WHITE
						4:
							visual.ink = Ink.BRIGHT_GREEN
						5:
							visual.ink = Ink.BLUE
						6:
							visual.ink = Ink.BLACK
						7:
							visual.ink = Ink.GREEN
			elif monster_type.flags_1.is_clear(MonsterFlags1.CHAR_CLEAR | MonsterFlags1.ATTR_CLEAR):
				visual.character = monster_visual.character
				visual.ink = monster_visual.ink
			else:
				# Override character or colour for clear monsters
				if monster_type.flags_1.is_clear(MonsterFlags1.CHAR_CLEAR):
					visual.character = monster_visual.character
				if monster_type.flags_1.is_clear(MonsterFlags1.ATTR_CLEAR):
					visual.ink = monster_visual.ink
			# If we're hallucinating, make it random
			if player.timed_hallucinations != 0:
				visual = hallucinate_item(visual)
	# Finally, the player overrides anything else in the location
	if row == player.map_row and col == player.map_col:
		var monster_type: MonsterType = Profile.instance.monster_types.types[0]
		visual.character = monster_type.character
		visual.ink = monster_type.colour
	# Return the visual we created
	return visual


func map_area()->void:
	var row_1: int = panel_row_min - Rng.roll_die(10)
	var row_2: int = panel_row_max + Rng.roll_die(10)
	var col_1: int = panel_col_min - Rng.roll_die(20)
	var col_2: int = panel_col_max + Rng.roll_die(20)
	row_1 = clampi(row_1, 1, cur_hgt-2)
	row_2 = clampi(row_2, 1, cur_hgt-2)
	col_1 = clampi(col_1, 1, cur_wid-2)
	col_2 = clampi(col_2, 1, cur_wid-2)
	for row: int in range(row_1, row_2+1):
		for col: int in range(col_1, col_2+1):
			var tile: GridTile = get_grid(row,col)
			if not tile.feature_type.is_wall:
				if not tile.feature_type.is_open_floor:
					tile.tile_flags.set_bits(GridTile.PLAYER_MEMORISED)
				for i:int in 8:
					tile = get_grid(row + ordered_direction_row_offset[i], col + ordered_direction_col_offset[i])
					if tile.feature_type.is_wall:
						tile.tile_flags.set_bits(GridTile.PLAYER_MEMORISED)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_MAP)



func hallucinate_monster(visual: Visual) -> Visual:
	visual.character = Profile.instance.monster_types.types[Rng.roll_die(Profile.instance.monster_types.types.size() - 2)].character
	visual.ink = Profile.instance.monster_types.types[Rng.roll_die(Profile.instance.monster_types.types.size() - 2)].colour
	return visual


func hallucinate_item(visual: Visual) -> Visual:
	visual.character = SaveGame.instance.item_types.types[Rng.roll_die(SaveGame.instance.item_types.types.size() - 2)].character
	visual.ink = SaveGame.instance.item_types.types[Rng.roll_die(SaveGame.instance.item_types.types.size() - 2)].colour
	return visual


func hallucinate_something(visual: Visual) -> Visual:
	if Rng.roll_die(100) < 75:
		return hallucinate_monster(visual)
	else:
		return hallucinate_item(visual)


func set_visible(grid: GridTile, row: int, col: int) -> void:
	grid.tile_flags.set_bits(GridTile.IS_VISIBLE)
	visible_locations[visible_locations_count].row = row
	visible_locations[visible_locations_count].col = col
	visible_locations_count += 1


func set_lit(row: int, col: int) -> void:
	get_grid(row, col).tile_flags.set_bits(GridTile.PLAYER_LIT)
	lit_locations[lit_locations_count].row = row
	lit_locations[lit_locations_count].col = col
	lit_locations_count += 1


func update_view() -> void:
	var grid: GridTile
	var row_max: int = cur_hgt - 1
	var col_max: int = cur_wid - 1
	var east_north_limit: int
	var east_south_limit: int
	var west_north_limit: int
	var west_south_limit: int
	var north_east_limit: int
	var north_west_limit: int
	var south_east_limit: int
	var south_west_limit: int
	# First, clear all the previously visible locations
	for n: int in visible_locations_count:
		var location: Location = visible_locations[n]
		get_grid(location.row, location.col).tile_flags.clear_bits(GridTile.IS_VISIBLE)
		get_grid(location.row, location.col).tile_flags.set_bits(GridTile.NEEDS_REDRAW)
		locations_for_redraw[locations_for_redraw_count].row = location.row
		locations_for_redraw[locations_for_redraw_count].col = location.col
		locations_for_redraw_count += 1
	visible_locations_count = 0
	# Start working out what is now visible
	# Step 1) We can always see our own location
	var row: int = player.map_row
	var col: int = player.map_col
	grid = get_grid(row, col)
	grid.tile_flags.set_bits(GridTile.EASY_VISIBILITY)
	set_visible(grid, row, col)
	# Step 2) Diagonals are easy
	var z: int = MAX_SIGHT_RADIUS * 2 / 3
	for d: int in range(1, z + 1):
		grid = get_grid(row + d, col + d)
		grid.tile_flags.set_bits(GridTile.EASY_VISIBILITY)
		set_visible(grid, row + d, col + d)
		if grid.feature_type.blocks_los:
			break
		if not in_inner_bounds(row + d, col + d):
			break
	for d: int in range(1, z + 1):
		grid = get_grid(row + d, col - d)
		grid.tile_flags.set_bits(GridTile.EASY_VISIBILITY)
		set_visible(grid, row + d, col - d)
		if grid.feature_type.blocks_los:
			break
		if not in_inner_bounds(row + d, col - d):
			break
	for d: int in range(1, z + 1):
		grid = get_grid(row - d, col + d)
		grid.tile_flags.set_bits(GridTile.EASY_VISIBILITY)
		set_visible(grid, row - d, col + d)
		if grid.feature_type.blocks_los:
			break
		if not in_inner_bounds(row - d, col + d):
			break
	for d: int in range(1, z + 1):
		grid = get_grid(row - d, col - d)
		grid.tile_flags.set_bits(GridTile.EASY_VISIBILITY)
		set_visible(grid, row - d, col - d)
		if grid.feature_type.blocks_los:
			break
		if not in_inner_bounds(row - d, col - d):
			break
	# Step 3) Orthogonals are also easy
	for d: int in range(1, MAX_SIGHT_RADIUS + 1):
		grid = get_grid(row - d, col)
		grid.tile_flags.set_bits(GridTile.EASY_VISIBILITY)
		set_visible(grid, row - d, col)
		north_east_limit = d
		north_west_limit = d
		if grid.feature_type.blocks_los:
			break
		if not in_inner_bounds(row - d, col):
			break
	for d: int in range(1, MAX_SIGHT_RADIUS + 1):
		grid = get_grid(row + d, col)
		grid.tile_flags.set_bits(GridTile.EASY_VISIBILITY)
		set_visible(grid, row + d, col)
		south_east_limit = d
		south_west_limit = d
		if grid.feature_type.blocks_los:
			break
		if not in_inner_bounds(row + d, col):
			break
	for d: int in range(1, MAX_SIGHT_RADIUS + 1):
		grid = get_grid(row, col - d)
		grid.tile_flags.set_bits(GridTile.EASY_VISIBILITY)
		set_visible(grid, row, col - d)
		west_north_limit = d
		west_south_limit = d
		if grid.feature_type.blocks_los:
			break
		if not in_inner_bounds(row, col - d):
			break
	for d: int in range(1, MAX_SIGHT_RADIUS + 1):
		grid = get_grid(row, col + d)
		grid.tile_flags.set_bits(GridTile.EASY_VISIBILITY)
		set_visible(grid, row, col + d)
		east_north_limit = d
		east_south_limit = d
		if grid.feature_type.blocks_los:
			break
		if not in_inner_bounds(row, col + d):
			break
	# Now the eight fiddly bits between the orthogonals and diagonals
	for n: int in OVER_SIGHT_RADIUS / 2 + 1:
		z = OVER_SIGHT_RADIUS - 2 * n
		if z > MAX_SIGHT_RADIUS - n - n / 2:
			z = MAX_SIGHT_RADIUS - n - n / 2
		var row_plus_n: int = row + n
		var row_minus_n: int = row - n
		var col_plus_n: int = col + n
		var col_minus_n: int = col - n
		if row_plus_n < row_max:
			var m: int = min(z, row_max - row_plus_n)
			# east side of south orthogonal
			if col <= col_max and n < south_east_limit:
				var k: int = n
				for d: int in range(1, m + 1):
					if (test_visibility_and_blocking(row_plus_n + d, col_plus_n, row_plus_n + d - 1, col_plus_n - 1, row_plus_n + d - 1, col_plus_n)):
						if n + d > south_east_limit:
							break
					else:
						k = n + d
				south_east_limit = k + 1
			# west side of south orthogonal
			if col >= 0 and n < south_west_limit:
				var k: int = n
				for d: int in range(1, m + 1):
					if test_visibility_and_blocking(row_plus_n + d, col_minus_n, row_plus_n + d - 1, col_minus_n - 1, row_plus_n + d - 1, col_minus_n):
						if n + d > south_west_limit:
							break
					else:
						k = n + d
				south_west_limit = k + 1
		if row_minus_n > 0:
			var m: int = min(z, row_minus_n)
			# east side of north orthogonal
			if col_plus_n <= col_max and n < north_east_limit:
				var k: int = n
				for d: int in range(1, m + 1):
					if test_visibility_and_blocking(row_minus_n - d, col_plus_n, row_minus_n - d + 1, col_plus_n - 1, row_minus_n - d + 1, col_plus_n):
						if n + d >= north_east_limit:
							break
					else:
						k = n + d
				north_east_limit = k + 1
			# west side of north orthogonal
			if col_minus_n >= 0 and n < north_west_limit:
				var k: int = n
				for d: int in range(1, m + 1):
					if test_visibility_and_blocking(row_minus_n - d, col_minus_n, row_minus_n - d + 1, col_minus_n + 1, row_minus_n - d + 1, col_minus_n):
						if n + d >= north_west_limit:
							break
					else:
						k = n + d
				north_west_limit = k + 1
		if col_plus_n < col_max:
			var m: int = min(z, col_max - col_plus_n)
			# south side of east orthogonal
			if row_plus_n <= row_max and n < east_south_limit:
				var k: int = n
				for d: int in range(1, m + 1):
					if test_visibility_and_blocking(row_plus_n, col_plus_n + d, row_plus_n - 1, col_plus_n + d - 1, row_plus_n, col_plus_n + d - 1):
						if n + d > east_south_limit:
							break
					else:
						k = n + d
				east_south_limit = k + 1
			# north side of east orthogonal
			if row_minus_n >= 0 and n < east_north_limit:
				var k: int = n
				for d: int in range(1, m + 1):
					if test_visibility_and_blocking(row_minus_n, col_plus_n + d, row_minus_n + 1, col_plus_n + d - 1, row_minus_n, col_plus_n + d - 1):
						if n + d > east_north_limit:
							break
					else:
						k = n + d
				east_north_limit = k + 1
		if col_minus_n > 0:
			var m: int = min(z, col_minus_n)
			# south side of west orthogonal
			if row_plus_n <= row_max and n < west_south_limit:
				var k: int = n
				for d: int in range(1, m + 1):
					if test_visibility_and_blocking(row_plus_n, col_minus_n - d, row_plus_n - 1, col_minus_n - d + 1, row_plus_n, col_minus_n - d + 1):
						if n + d >= west_south_limit:
							break
					else:
						k = n + d
				west_south_limit = k + 1
			# north side of west orthogonal
			if row_minus_n >= 0 and n < west_north_limit:
				var k: int = n
				for d: int in range(1, m + 1):
					if test_visibility_and_blocking(row_minus_n, col_minus_n - d, row_minus_n + 1, col_minus_n - d + 1, row_minus_n, col_minus_n - d + 1):
						if n + d > west_north_limit:
							break
					else:
						k = n + d
				west_north_limit = k + 1
	# We've done all our calculations, so actually show the visible grids
	for n: int in visible_locations_count:
		row = visible_locations[n].row
		col = visible_locations[n].col
		get_grid(row, col).tile_flags.clear_bits(GridTile.EASY_VISIBILITY)
		if get_grid(row, col).tile_flags.is_set(GridTile.NEEDS_REDRAW):
			continue
		note_spot(row, col)
		redraw_single_location(row, col)
	for n: int in locations_for_redraw_count:
		row = locations_for_redraw[n].row
		col = locations_for_redraw[n].col
		get_grid(row, col).tile_flags.clear_bits(GridTile.NEEDS_REDRAW)
		if get_grid(row, col).tile_flags.is_set(GridTile.IS_VISIBLE):
			continue
		redraw_single_location(row, col)
	locations_for_redraw_count = 0


func test_visibility_and_blocking(row: int, col: int, row_1: int, col_1: int, row_2: int, col_2: int) -> bool:
	# Get the two grids in the way of our target grid
	var grid_1: GridTile = get_grid(row_1, col_1)
	var grid_2: GridTile = get_grid(row_2, col_2)
	# if neither grid is see-through then we can't be seen
	var through_1: bool = not grid_1.feature_type.blocks_los
	var through_2: bool = not grid_2.feature_type.blocks_los
	if not through_1 and not through_2:
		return true
	# If neither grid is visible and see-through then we can't be seen
	var vis_1: bool = through_1 and grid_1.tile_flags.is_set(GridTile.IS_VISIBLE)
	var vis_2: bool = through_2 and grid_2.tile_flags.is_set(GridTile.IS_VISIBLE)
	if not vis_1 and not vis_2:
		return true
	# If both grids are visible with easy visibility, then we can be seen with easy visibility but we might block
	var grid: GridTile = get_grid(row, col)
	var grid_blocks: bool = grid.feature_type.blocks_los
	var z_1: bool = vis_1 and grid_1.tile_flags.is_set(GridTile.EASY_VISIBILITY)
	var z_2: bool = vis_2 and grid_2.tile_flags.is_set(GridTile.EASY_VISIBILITY)
	if z_1 and z_2:
		grid.tile_flags.set_bits(GridTile.EASY_VISIBILITY)
		set_visible(grid, row, col)
		return grid_blocks
	# If both grids are visible, then we can be seen but we might block
	if vis_1 and vis_2:
		set_visible(grid, row, col)
		return grid_blocks
	# If we block, we can be seen but we still block further checks
	if grid_blocks:
		set_visible(grid, row, col)
		return true
	# If all else fails, do a line-of-sight check to the grid
	if line_of_sight(player.map_row, player.map_col, row, col):
		set_visible(grid, row, col)
		return false
	# We can't see the grid, so just return that it blocks
	return true


func note_spot(row: int, col: int) -> void:
	var grid: GridTile = get_grid(row, col)
	# If we're blind we see nothing
	if player.timed_blindness != 0:
		return
	# If we're not lit and visible we see nothing
	if grid.tile_flags.is_clear(GridTile.PLAYER_LIT):
		if grid.tile_flags.is_clear(GridTile.IS_VISIBLE):
			return
		if grid.tile_flags.is_clear(GridTile.SELF_LIT):
			return
	# Mark all items on the grid as having been seen
	var item_index: int = grid.item_index
	while item_index != 0:
		var item: Item = dungeon_items.items[item_index]
		item.marked = true
		item_index = item.next_in_stack
	# If we haven't already memorised the tile then do so
	if grid.tile_flags.is_clear(GridTile.PLAYER_MEMORISED):
		if grid.feature_type.is_open_floor:
			grid.tile_flags.set_bits(GridTile.PLAYER_MEMORISED)
		elif grid.feature_type.is_passable:
			grid.tile_flags.set_bits(GridTile.PLAYER_MEMORISED)
		elif grid.tile_flags.is_set(GridTile.PLAYER_LIT):
			grid.tile_flags.set_bits(GridTile.PLAYER_MEMORISED)
		else:
			var row_behind: int = (row + 1) if row < player.map_row else (row - 1) if row > player.map_row else row
			var col_behind: int = (col + 1) if col < player.map_col else (col - 1) if col > player.map_col else col
			if get_grid(row_behind, col_behind).tile_flags.is_set(GridTile.SELF_LIT):
				grid.tile_flags.set_bits(GridTile.PLAYER_MEMORISED)


func line_of_sight(row_1: int, col_1: int, row_2: int, col_2: int) -> bool:
	var d_row: int = row_2 - row_1
	var d_col: int = col_2 - col_1
	var abs_row: int = absi(d_row)
	var abs_col: int = absi(d_col)
	# First degenerate case - we are on or next to the target
	if abs_row < 2 and abs_col < 2:
		return true
	# Second degenerate case - straight vertical line
	if d_col == 0:
		if d_row > 0:
			for test_row: int in range(row_1 + 1, row_2):
				if get_grid(test_row, col_1).feature_type.blocks_los:
					return false
		else:
			for test_row: int in range(row_1 - 1, row_2, -1):
				if get_grid(test_row, col_1).feature_type.blocks_los:
					return false
		return true
	# Third degenerate case - straight horizontal line
	if d_row == 0:
		if d_col > 0:
			for test_col: int in range(col_1 + 1, col_2):
				if get_grid(row_1, test_col).feature_type.blocks_los:
					return false
		else:
			for test_col: int in range(col_1 - 1, col_2, -1):
				if get_grid(row_1, test_col).feature_type.blocks_los:
					return false
		return true
	# Fourth degenerate case - it's a knight's move away
	var sign_row: int = -1 if d_row < 0 else 1
	var sign_col: int = -1 if d_col < 0 else 1
	if abs_col == 1:
		if abs_row == 2:
			if not get_grid(row_1 + sign_row, col_1).feature_type.blocks_los:
				return true
	elif abs_row == 1:
		if abs_col == 2:
			if not get_grid(row_1, col_1 + sign_col).feature_type.blocks_los:
				return true
	# None of the degenerate cases applied, so we're going to have to work it out properly
	var f_2: int = abs_col * abs_row
	var f_1: int = f_2 * 2
	if abs_col >= abs_row:
		var q_row: int = abs_row * abs_row
		var m: int = q_row * 2
		var test_col: int = col_1 + sign_col
		var test_row: int
		if q_row == f_2:
			test_row = row_1 + sign_row
			q_row -= f_1
		else:
			test_row = row_1
		while test_col != col_2:
			if get_grid(test_row, test_col).feature_type.blocks_los:
				return false
			q_row += m
			if q_row < f_2:
				test_col += sign_col
			elif q_row > f_2:
				test_row += sign_row
				if get_grid(test_row, test_col).feature_type.blocks_los:
					return false
				q_row -= f_1
				test_col += sign_col
			else:
				test_row += sign_row
				q_row -= f_1
				test_col += sign_col
	else:
		var q_col: int = abs_col * abs_col
		var m: int = q_col * 2
		var test_row: int = row_1 + sign_row
		var test_col: int
		if q_col == f_2:
			test_col = col_1 + sign_col
			q_col -= f_1
		else:
			test_col = col_1
		while test_row != row_2:
			if get_grid(test_row, test_col).feature_type.blocks_los:
				return false
			q_col += m
			if q_col < f_2:
				test_row += sign_row
			elif q_col > f_2:
				test_col += sign_col
				if get_grid(test_row, test_col).feature_type.blocks_los:
					return false
				q_col -= f_1
				test_row += sign_row
			else:
				test_col += sign_col
				q_col -= f_1
				test_row += sign_row
	return true


func distance(row_1: int, col_1: int, row_2: int, col_2: int) -> int:
	var d_row: int = absi(row_2 - row_1)
	var d_col: int = absi(col_2 - col_1)
	return (d_row + d_col / 2) if d_row > d_col else (d_col + d_row / 2)


func replace_pets(row: int, col: int, pets: Array[Monster]) -> void:
	for monster: Monster in pets:
		dungeon_monsters.replace_pet(row, col, monster)

func report_charge_usage_from_floor (item_index: int)->void:
	var item: Item = dungeon_items.items[item_index]
	if item.category != ItemCategory.STAFF and item.category != ItemCategory.WAND:
		return
	if not item.is_known():
		return
	var is_are: String = "is" if item.type_specific_value == 1 else "are"
	var s: String = "" if item.type_specific_value == 1 else "s"
	Profile.instance.message_print("There %s %d charge%s remaining." % [is_are, item.type_specific_value, s])


func scatter(row: int, col: int, dist: int) -> Location:
	var new_row: int = 0
	var new_col: int = 0
	var location: Location = Location.new()
	location.row = row
	location.col = col
	var attempts_left: int = 5000
	while attempts_left != 0:
		new_row = Rng.random_around(row, dist)
		new_col = Rng.random_around(col, dist)
		if not in_inner_bounds(row, col):
			continue
		if dist > 1 and distance(row, col, new_row, new_col) > dist:
			continue
		if line_of_sight(row, col, new_row, new_col):
			break
		attempts_left -= 1
	if attempts_left > 0:
		location.row = new_row
		location.col = new_col
	return location


func cave_valid_bold(row: int, col: int) -> bool:
	var tile: GridTile = get_grid(row, col)
	if tile.feature_type.is_permanent:
		return false
	var item_index: int = tile.item_index
	while item_index != 0:
		var item: Item = dungeon_items.items[item_index]
		item_index = item.next_in_stack
		if item.is_legendary() or item.is_artifact():
			return false
	return true


func cave_set_feat(row: int, col: int, feat: String) -> void:
	get_grid(row, col).set_feature(feat)
	note_spot(row, col)
	redraw_single_location(row, col)


func delete_item_at(row: int, col: int) -> void:
	if not in_outer_bounds(row, col):
		return
	var tile: GridTile = get_grid(row, col)
	var item_index: int = tile.item_index
	while item_index != 0:
		var item: Item = dungeon_items.items[item_index]
		dungeon_items.items[item_index] = Item.new()
		item_index = item.next_in_stack
		item_count -= 1
	tile.item_index = 0
	redraw_single_location(row, col)


func delete_monster_at (row: int, col: int) -> void:
	if not in_outer_bounds(row, col):
		return
	var tile: GridTile = get_grid(row, col)
	if tile.monster_index != 0:
		dungeon_monsters.delete_monster_by_index(tile.monster_index, true)


func place_trap(row: int, col: int) -> void:
	if not in_inner_bounds(row, col):
		return
	if not grid_open_no_item_or_creature(row, col):
		return
	cave_set_feat(row, col, "Invis")


func place_gold(row: int, col: int) -> void:
	if not in_inner_bounds(row, col):
		return
	if not grid_open_no_item(row, col):
		return
	var item: Item = Item.new()
	if not item.make_gold():
		return
	var index: int = dungeon_items.get_free_item_slot()
	if index != 0:
		dungeon_items.items[index] = item
		item.map_row = row
		item.map_col = col
		var tile: GridTile = get_grid(row, col)
		item.next_in_stack = tile.item_index
		tile.item_index = index
		note_spot(row, col)
		redraw_single_location(row, col)


func place_item(row: int, col: int, good: bool, great: bool) -> void:
	if not in_inner_bounds(row, col):
		return
	if not grid_open_no_item(row, col):
		return
	var item: Item = Item.new()
	if not item.make_item(good, great):
		return
	var index: int = dungeon_items.get_free_item_slot()
	if index != 0:
		dungeon_items.items[index] = item
		item.map_row = row
		item.map_col = col
		var tile: GridTile = get_grid(row, col)
		item.next_in_stack = tile.item_index
		tile.item_index = index
		note_spot(row, col)
		redraw_single_location(row, col)
	else:
		if item.artifact_index != 0:
			SaveGame.instance.artifacts[item.artifact_index].current_number = 0


func wiz_light() -> void:
	for i: int in item_count_max:
		var item: Item = dungeon_items.items[i]
		if item.item_type != null:
			continue
		if item.holding_monster_index != 0:
			continue
		item.marked = true
	for row: int in range(1, cur_hgt - 1):
		for col: int in range(1, cur_wid - 1):
			var tile: GridTile = get_grid(row, col)
			if not tile.feature_type.is_wall:
				for i: int in 9:
					var test_row: int = row + ordered_direction_row_offset[i]
					var test_col: int = col + ordered_direction_col_offset[i]
					tile = get_grid(test_row, test_col)
					tile.tile_flags.set_bits(GridTile.SELF_LIT)
					tile.tile_flags.set_bits(GridTile.PLAYER_MEMORISED)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_MONSTERS)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_MAP)


func wiz_dark() -> void:
	for row: int in cur_hgt:
		for col: int in cur_wid:
			var tile : GridTile = get_grid(row, col)
			tile.tile_flags.clear_bits(GridTile.PLAYER_MEMORISED)
	for item: Item in dungeon_items.items:
		if item.item_type == null:
			continue
		if item.holding_monster_index != 0:
			continue
		item.marked = false
	player.updates_needed.set_bits(UpdateFlags.UPDATE_REMOVE_VIEW | UpdateFlags.UPDATE_REMOVE_LIGHT)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_VIEW | UpdateFlags.UPDATE_LIGHT)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_MONSTERS)
	player.updates_needed.set_bits(RedrawFlags.REDRAW_MAP)


func clear_item_index(index: int) -> void:
	var this_index: int
	var next_index: int
	var prev_index: int
	var item: Item = dungeon_items.items[index]
	if item.holding_monster_index != 0:
		var monster: Monster = dungeon_monsters.monsters[item.holding_monster_index]
		this_index = monster.first_held_item_index
		while this_index != 0:
			var held_item: Item = dungeon_items.items[this_index]
			next_index = held_item.next_in_stack
			if this_index == index:
				if prev_index == 0:
					monster.first_held_item_index = next_index
				else:
					var prev_item: Item = dungeon_items.items[prev_index]
					prev_item.next_in_stack = next_index
				held_item.next_in_stack = 0
				break
			prev_index = this_index
			this_index = next_index
	else:
		var row: int = item.map_row
		var col: int = item.map_col
		var tile: GridTile = get_grid(row, col)
		this_index = tile.item_index
		while this_index != 0:
			var floor_item: Item = dungeon_items.items[this_index]
			next_index = floor_item.next_in_stack
			if this_index == index:
				if prev_index == 0:
					tile.item_index = next_index
				else:
					var prev_item: Item = dungeon_items.items[prev_index]
					prev_item.next_in_stack = next_index
				floor_item.next_in_stack = 0
				break
			prev_index = this_index
			this_index = next_index


func delete_item_by_index(item_index: int) -> void:
	clear_item_index(item_index)
	var item: Item = dungeon_items.items[item_index]
	if item.holding_monster_index == 0:
		var row: int = item.map_row
		var col: int = item.map_col
		redraw_single_location(row, col)
		dungeon_items.items[item_index] = Item.new()
		item_count -= 1


func put_quest_monster(index: int) -> void:
	if Profile.instance.monster_types.types[index].max_num == 0:
		Profile.instance.monster_types.types[index].max_num += 1
		Profile.instance.message_print("Resurrecting guardian to fix corrupted savefile...")
	var row: int
	var col: int
	var done: bool = false
	while not done:
		while true:
			row = Rng.roll_die(MAX_HGT - 2) - 1
			col = Rng.roll_die(MAX_HGT - 2) - 1
			if not grid_open_no_item_or_creature(row, col):
				continue
			if distance(row, col, SaveGame.instance.player.map_row, SaveGame.instance.player.map_col) > 15:
				break
		done = dungeon_monsters.place_monster_by_index(row, col, index, false, false, false)


func display_map() -> Location:
	var map_ink: Array = []
	for row: int in cur_hgt + 2:
		var map_row: Array[int] = []
		for col: int in cur_wid + 2:
			map_row.append(Ink.WHITE)
		map_ink.append(map_row)
	var map_character: Array = []
	for row: int in cur_hgt + 2:
		var map_row: Array[String] = []
		for col: int in cur_wid + 2:
			map_row.append(" ")
		map_character.append(map_row)
	var map_priority: Array = []
	for row: int in cur_hgt + 2:
		var map_row: Array[int] = []
		for col: int in cur_wid + 2:
			map_row.append(0)
		map_priority.append(map_row)
	var max_row: int = 0
	var max_col: int = 0
	for dungeon_row: int in cur_hgt:
		for dungeon_col: int in cur_wid:
			var map_row: int = (dungeon_row / 3) + 1
			var map_col: int = (dungeon_col / 3) + 1
			if map_row > max_row:
				max_row = map_row
			if map_col > max_col:
				max_col = map_col
			var visual: Visual = map_info(dungeon_row, dungeon_col)
			var priority: int = get_grid(dungeon_row, dungeon_col).feature_type.map_priority
			if visual.ink == Ink.BACKGROUND:
				priority = 0
			if map_priority[map_row][map_col] < priority:
				map_character[map_row][map_col] = visual.character
				map_ink[map_row][map_col] = visual.ink
				map_priority[map_row][map_col] = priority
	var border_row: int = max_row + 1
	var border_col: int = max_col + 1
	var col_offset: int = (80 - border_col) / 2
	var row_offset: int = (44 - border_row) / 2
	Console.set_mask (row_offset + 1,col_offset + 1,max_row,max_col)
	map_character[0][0] = "+"
	map_ink[0][0] = Ink.PURPLE
	map_character[0][border_col] = "+"
	map_ink[0][border_col] = Ink.PURPLE
	map_character[border_row][0] = "+"
	map_ink[border_row][0] = Ink.PURPLE
	map_character[border_row][border_col] = "+"
	map_ink[border_row][border_col] = Ink.PURPLE
	for col: int in range(1, max_col + 1):
		map_character[0][col] = "-"
		map_ink[0][col] = Ink.PURPLE
		map_character[max_row + 1][col] = "-"
		map_ink[max_row + 1][col] = Ink.PURPLE
	for row: int in range(1, max_row + 1):
		map_character[row][0] = "|"
		map_ink[row][0] = Ink.PURPLE
		map_character[row][max_col + 1] = "|"
		map_ink[row][max_col + 1] = Ink.PURPLE
	for row: int in max_row + 2:
		for col: int in max_col + 2:
			var ink: int = map_ink[row][col]
			var character: String = map_character[row][col]
			if player.timed_invulnerability != 0:
				ink = Ink.WHITE
			elif player.timed_etherealness != 0:
				ink = Ink.BLACK
			Console.print_at(row + row_offset, col + col_offset, character, ink)
	var location: Location = Location.new()
	location.row = row_offset + (player.map_row / 3) + 1
	location.col = col_offset + (player.map_col / 3) + 1
	return location


func replace_secret_door(row: int, col: int) -> void:
	var tmp: int = Rng.roll_die(400)
	if tmp <= 300:
		cave_set_feat(row, col, "LockedDoor0")
	elif tmp <= 999:
		cave_set_feat(row, col, "LockedDoor%s" % [Rng.roll_die(7)])
	else:
		cave_set_feat(row, col, "JammedDoor%s" % [Rng.roll_die(8) - 1])


func no_light() -> bool:
	return not player_can_see_bold(player.map_row, player.map_col)


func player_can_see_bold(row: int, col: int) -> bool:
	if player.timed_blindness != 0:
		return false
	var tile: GridTile = get_grid(row, col)
	if tile.tile_flags.is_set(GridTile.PLAYER_LIT):
		return true
	if not player_has_los_bold(row, col):
		return false
	if tile.tile_flags.is_clear(GridTile.SELF_LIT):
		return false
	if grid_passable(row, col):
		return true
	var row_1: int = row + 1 if row < player.map_row else row - 1 if row > player.map_row else row
	var col_1: int = col + 1 if col < player.map_col else col - 1 if col > player.map_col else col
	return get_grid(row_1, col_1).tile_flags.is_set(GridTile.SELF_LIT)


func pick_trap(row: int, col: int) -> void:
	var feat: String
	var tile: GridTile = get_grid(row, col)
	if tile.feature_type.name != "Invis":
		return
	var trap_type: int = Rng.roll_die(16)
	if SaveGame.instance.quests.is_quest_level(SaveGame.instance.current_depth):
		trap_type = Rng.roll_die(15)
	if SaveGame.instance.current_depth >= SaveGame.instance.current_dungeon.max_level:
		trap_type = Rng.roll_die(15)
	match trap_type:
		1:
			feat = "AcidTrap"
		2:
			feat = "FireTrap"
		3:
			feat = "StrDart"
		4:
			feat = "ConDart"
		5:
			feat = "DexDart"
		6:
			feat = "SlowDart"
		7:
			feat = "PoisonGas"
		8:
			feat = "ConfuseGas"
		9:
			feat = "SleepGas"
		10:
			feat = "BlindGas"
		11:
			feat = "SummonRune"
		12:
			feat = "TeleportRune"
		13:
			feat = "Pit"
		14:
			feat = "SpikedPit"
		15:
			feat = "PoisonPit"
		_:
			feat = "TrapDoor"
	cave_set_feat(row, col, feat)


func chest_check(row: int, col: int) -> int:
	var tile: GridTile = get_grid(row, col)
	var item_index: int = tile.item_index
	while item_index != 0:
		var item: Item = dungeon_items.items[item_index]
		if item.category == ItemCategory.CHEST:
			return item_index
		item_index = item.next_in_stack
	return 0


func coords_to_dir(row: int, col: int) -> int:
	var d: Array[int] = [7, 8, 9, 4, 5, 6, 1, 2, 3]
	var d_row: int = row - player.map_row
	var d_col: int = col - player.map_col
	if absi(d_row) > 1 or absi(d_col) > 1:
		return 0
	return d[(d_row + 1) * 3 + d_col + 1]


func show_cursor_relative(row: int, col: int) -> void:
	row -= panel_row_print
	col -= panel_col_print
	Console.show_cursor(row, col)


func projectable(row_from: int, col_from: int, row_to: int, col_to: int) -> bool:
	var row: int = row_from
	var col: int = col_from
	for dist: int in MAX_SPELL_RANGE:
		if col == col_to and row == row_to:
			return true
		if dist != 0 and not grid_passable(row, col) and get_grid(row, col).feature_type.name != "YellowSign":
			break
		var location: Location = move_one_step_towards(row, col, row_from, col_from, row_to, col_to)
		row = location.row
		col = location.col
	return false


func move_one_step_towards(row: int, col: int, row_from: int, col_from: int, row_to: int, col_to: int) -> Location:
	var location: Location = Location.new()
	location.row = row
	location.col = col
	var shift: int
	var row_disp: int = row_from - location.row if location.row < row_from else location.row - row_from
	var col_disp: int = col_from - location.col if location.col < col_from else location.col - col_from
	var max_dist: int = row_disp if row_disp > col_disp else col_disp
	max_dist += 1
	row_disp = row_from - row_to if row_to < row_from else row_to - row_from
	col_disp = col_from - col_to if col_to < col_from else col_to - col_from
	if row_disp == 0 and col_disp == 0:
		return location
	if row_disp > col_disp:
		shift = ((max_dist * col_disp) + ((row_disp - 1) / 2)) / row_disp
		location.col = col_from - shift if col_to < col_from else col_from + shift
		location.row = row_from - max_dist if row_to < row_from else row_from + max_dist
	else:
		shift = ((max_dist * row_disp) + ((col_disp - 1) / 2)) / col_disp
		location.row = row_from - shift if row_to < row_from else row_from + shift
		location.col = col_from - max_dist if col_to < col_from else col_from + max_dist
	return location


func revert_tile_to_background(row: int, col: int) -> void:
	var tile: GridTile = get_grid(row, col)
	tile.revert_to_background()
	note_spot(row, col)
	redraw_single_location(row, col)
