# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name TargetEngine
extends RefCounted

const TARGET_KILL: int = 0x01
const TARGET_LOOK: int = 0x02
const TARGET_XTRA: int = 0x04
const TARGET_GRID: int = 0x08

var player: Player
var dungeon_level: DungeonLevel
var savegame: SaveGame


static func new_target_engine(the_player: Player, the_level: DungeonLevel, the_savegame: SaveGame) -> TargetEngine:
	var target_engine: TargetEngine = TargetEngine.new()
	target_engine.player = the_player
	target_engine.dungeon_level = the_level
	target_engine.savegame = the_savegame
	return target_engine


func panel_bounds() -> void:
	dungeon_level.panel_row_min = dungeon_level.panel_row * (Constants.SCREEN_HEIGHT / 2)
	dungeon_level.panel_row_max = dungeon_level.panel_row_min + Constants.SCREEN_HEIGHT - 1
	dungeon_level.panel_row_print = dungeon_level.panel_row_min - 1
	dungeon_level.panel_col_min = dungeon_level.panel_col * (Constants.SCREEN_WIDTH / 2)
	dungeon_level.panel_col_max = dungeon_level.panel_col_min + Constants.SCREEN_WIDTH - 1
	dungeon_level.panel_col_print = dungeon_level.panel_col_min - 13


func panel_bounds_center() -> void:
	dungeon_level.panel_row = dungeon_level.panel_row_min / (Constants.SCREEN_HEIGHT / 2)
	dungeon_level.panel_row_max = dungeon_level.panel_row_min + Constants.SCREEN_HEIGHT - 1
	dungeon_level.panel_row_print = dungeon_level.panel_row_min - 1
	dungeon_level.panel_col = dungeon_level.panel_col_min / (Constants.SCREEN_WIDTH / 2)
	dungeon_level.panel_col_max = dungeon_level.panel_col_min + Constants.SCREEN_WIDTH - 1
	dungeon_level.panel_col_print = dungeon_level.panel_col_min - 13


func recenter_screen_around_player() -> void:
	var row: int = player.map_row
	var col: int = player.map_col
	var max_p_row_min: int = dungeon_level.max_panel_rows * (Constants.SCREEN_HEIGHT / 2)
	var max_p_col_min: int = dungeon_level.max_panel_cols * (Constants.SCREEN_WIDTH / 2)
	var p_row_min: int = row - (Constants.SCREEN_HEIGHT / 2)
	p_row_min = clampi(p_row_min, 0, max_p_row_min)
	var p_col_min: int = col - (Constants.SCREEN_WIDTH / 2)
	p_col_min = clampi(p_col_min, 0, max_p_col_min)
	if p_row_min == dungeon_level.panel_row_min and p_col_min == dungeon_level.panel_col_min:
		return
	dungeon_level.panel_row_min = p_row_min
	dungeon_level.panel_col_min = p_col_min
	panel_bounds_center()
	player.updates_needed.set_bits(UpdateFlags.UPDATE_MONSTERS)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_MAP)


## Get and return allied keypad direction of the user's choosing, or return -1 if no direction chosen
func get_direction_no_aim() -> int:
	var dir: int = User.command_direction
	# If we don't already have allied direction, get one
	while dir == 0:
		var choice: String = User.get_simple_choice("Direction (Escape to cancel)? ")
		if choice == "":
			break
		dir = User.key_to_keypad_direction(choice)
	# If we would stand still or we still don't have allied direction, quit
	if dir == 5:
		dir = 0
	if dir == 0:
		return -1
	# Set the direction we chose
	User.command_direction = dir
	# If we're confused, possibly change the direction randomly
	if player.timed_confusion != 0:
		if Rng.roll_die(4) != 4:
			dir = dungeon_level.ordered_direction[Rng.roll_die(8) - 1]
	if User.command_direction != dir:
		Profile.instance.message_print("You are confused.")
	return dir


func get_direction_with_aim() -> int:
	var dir: int = User.command_direction
	if target_okay():
		dir = 5
	while dir == 0:
		var p: String = "Direction ('*' to choose a target, Escape to cancel)? " if not target_okay() else "Direction ('5' for target, '*' to re-target, Escape to cancel)? "
		var command: String = User.get_simple_choice(p)
		if command == "":
			break
		match command:
			"T", "t", ".", "5", "0":
				dir = 5
			"*":
				if target_set(TARGET_KILL):
					dir = 5
			_:
				dir = User.key_to_keypad_direction(command)
				break
		if dir == 5 and not target_okay():
			dir = 0
	if dir == 0:
		return -1
	User.command_direction = dir
	if player.timed_confusion != 0:
		dir = dungeon_level.ordered_direction[Rng.roll_die(8) - 1]
	if User.command_direction != dir:
		Profile.instance.message_print("You are confused.")
	return dir


func target_okay() -> bool:
	if savegame.target_who < 0:
		return true
	if savegame.target_who <= 0:
		return false
	if not target_able(savegame.target_who):
		return false
	var monster: Monster = dungeon_level.dungeon_monsters.monsters[savegame.target_who]
	savegame.target_row = monster.map_row
	savegame.target_col = monster.map_col
	return true


func target_set(mode: int) -> bool:
	var row: int = player.map_row
	var col: int = player.map_col
	var done: bool = false
	savegame.target_who = 0
	target_set_prepare(mode)
	var m: int = 0
	if dungeon_level.locations_for_redraw_count != 0:
		row = dungeon_level.locations_for_redraw[m].row
		col = dungeon_level.locations_for_redraw[m].col
	while not done:
		var tile: GridTile = dungeon_level.get_grid(row, col)
		var info: String = "t,T,*" if target_able(tile.monster_index) else "T,*"
		var query: String = target_set_aux(row, col, mode | TARGET_LOOK, info)
		match query:
			"Escape":
				done = true
			"t":
				if target_able(tile.monster_index):
					savegame.health_track(tile.monster_index)
					savegame.target_who = tile.monster_index
					savegame.target_row = row
					savegame.target_col = col
					done = true
			"T":
				savegame.target_who = -1
				savegame.target_row = row
				savegame.target_col = col
				done = true
			"*":
				if row == dungeon_level.locations_for_redraw[m].row and col == dungeon_level.locations_for_redraw[m].col:
					m += 1
					if m >= dungeon_level.locations_for_redraw_count:
						m = 0
						done = true
					else:
						row = dungeon_level.locations_for_redraw[m].row
						col = dungeon_level.locations_for_redraw[m].col
				else:
					row = dungeon_level.locations_for_redraw[m].row
					col = dungeon_level.locations_for_redraw[m].col
			_:
				var d: int = User.key_to_keypad_direction(query)
				if d != 0:
					col += dungeon_level.keypad_direction_col_offset[d]
					row += dungeon_level.keypad_direction_row_offset[d]
					if col >= dungeon_level.cur_wid - 1 or col > dungeon_level.panel_col_max:
						col -= 1
					elif col <= 0 or col < dungeon_level.panel_col_min:
						col += 1
					if row >= dungeon_level.cur_hgt - 1 or row > dungeon_level.panel_row_max:
						row -= 1
					elif row <= 0 or row < dungeon_level.panel_row_min:
						row += 1
	dungeon_level.locations_for_redraw_count = 0
	Console.hide_cursor()
	Console.clear_row(0)
	return savegame.target_who != 0


func target_able(monster_index: int) -> bool:
	var monster: Monster = dungeon_level.dungeon_monsters.monsters[monster_index]
	if monster.monster_type == null:
		return false
	if not monster.is_visible:
		return false
	if not dungeon_level.projectable(player.map_row, player.map_col, monster.map_row, monster.map_col):
		return false
	if player.timed_hallucinations != 0:
		return false
	return true


func target_set_prepare(mode: int) -> void:
	dungeon_level.locations_for_redraw_count = 0
	for row: int in range(dungeon_level.panel_row_min, dungeon_level.panel_row_max + 1):
		for col: int in range(dungeon_level.panel_col_min, dungeon_level.panel_col_max + 1):
			var tile: GridTile = dungeon_level.get_grid(row, col)
			if not dungeon_level.player_has_los_bold(row, col):
				continue
			if not target_set_accept(row, col):
				continue
			if (mode & TARGET_KILL) != 0 and not target_able(tile.monster_index):
				continue
			dungeon_level.locations_for_redraw[dungeon_level.locations_for_redraw_count].row = row
			dungeon_level.locations_for_redraw[dungeon_level.locations_for_redraw_count].col = col
			dungeon_level.locations_for_redraw_count += 1
	var list: Dictionary[int, Location]
	for i: int in dungeon_level.locations_for_redraw_count:
		var location: Location = Location.new()
		location.row = dungeon_level.locations_for_redraw[i].row
		location.col = dungeon_level.locations_for_redraw[i].col
		var key: int = dungeon_level.distance(location.row, location.col, player.map_row, player.map_col) * 1000 + i
		list.get_or_add(key, location)
	var keys: Array[int] = list.keys()
	keys.sort()
	for i: int in dungeon_level.locations_for_redraw_count:
		dungeon_level.locations_for_redraw[i].row = list[keys[i]].row
		dungeon_level.locations_for_redraw[i].col = list[keys[i]].col


func target_set_accept(row: int, col: int) -> bool:
	if row == player.map_row and col == player.map_col:
		return true
	if player.timed_hallucinations != 0:
		return false
	var tile: GridTile = dungeon_level.get_grid(row, col)
	if tile.monster_index != 0:
		var monster: Monster = dungeon_level.dungeon_monsters.monsters[tile.monster_index]
		if monster.is_visible:
			return true
	var item_index: int = tile.item_index
	while item_index != 0:
		var item: Item = dungeon_level.dungeon_items.items[item_index]
		if item.marked:
			return true
		item_index = item.next_in_stack
	if tile.tile_flags.is_set(GridTile.PLAYER_MEMORISED):
		return tile.feature_type.is_interesting
	return false


func target_set_aux(row: int, col: int, mode: int, info: String) -> String:
	var tile: GridTile = dungeon_level.get_grid(row, col)
	var query: String = "Enter"
	var item_index: int
	while query == "Enter":
		query = " "
		var boring: bool = true
		var partial_1: String = "You see "
		var partial_2: String = ""
		var partial_3: String = ""
		if row == player.map_row and col == player.map_col:
			partial_1 = "You are "
			partial_2 = "on "
		var full_text: String
		if player.timed_hallucinations != 0:
			const name: String = "something strange"
			full_text = "%s%s%s%s [%s]" % [partial_1, partial_2, partial_3, name, info]
			Console.print_line_at(0, 0, full_text, Ink.WHITE)
			dungeon_level.show_cursor_relative(row, col)
			query = User.get_key()
			if query != "Enter":
				break
			continue
		if tile.monster_index != 0:
			var monster: Monster = dungeon_level.dungeon_monsters.monsters[tile.monster_index]
			var monster_type: MonsterType = monster.monster_type
			if monster.is_visible:
				var recall: bool = false
				boring = false
				var monster_name: String = monster.describe(0x08)
				savegame.health_track(tile.monster_index)
				savegame.notice_update_redraw()
				while true:
					if recall:
						Console.save_screen()
						monster_type.knowledge.display(monster_type)
						Console.print_following("  [r,%s]" % [info], Ink.WHITE)
						query = User.get_key()
						Console.restore_screen()
					else:
						var cloned: String = " (clone)" if monster.mind.is_set(MonsterMind.CLONED) else ""
						var allied: String = " (allied) " if monster.mind.is_set(MonsterMind.FRIENDLY) else ""
						full_text = "%s%s%s%s (%s)%s%s[r,%s]" % [partial_1, partial_2, partial_3, monster_name, look_mon_desc(tile.monster_index), cloned, allied, info]
						Console.print_line_at(0, 0, full_text, Ink.WHITE)
						dungeon_level.show_cursor_relative(row, col)
						query = User.get_key()
					if query != "r":
						break
					recall = not recall
				if query != "Enter" and query != " ":
					break
				if query == " " and (mode & TARGET_LOOK) == 0:
					break
				partial_1 = "It is "
				if monster_type.flags_1.is_set(MonsterFlags1.FEMALE):
					partial_1 = "She is "
				elif monster_type.flags_1.is_set(MonsterFlags1.MALE):
					partial_1 = "He is "
				partial_2 = "carrying "
				item_index = monster.first_held_item_index
				while item_index != 0:
					var item: Item = dungeon_level.dungeon_items.items[item_index]
					var item_name: String = item.description(true, 3)
					full_text = "%s%s%s%s [%s]" % [partial_1, partial_2, partial_3, item_name, info]
					Console.print_line_at(0, 0, full_text, Ink.WHITE)
					dungeon_level.show_cursor_relative(row, col)
					query = User.get_key()
					if query != "Enter" and query != " ":
						break
					if query == " " and (mode & TARGET_LOOK) == 0:
						break
					partial_2 = "also carrying "
					item_index = item.next_in_stack
				if item_index != 0:
					break
				partial_2 = "on "
		item_index = tile.item_index
		while item_index != 0:
			var item: Item = dungeon_level.dungeon_items.items[item_index]
			if item.marked:
				boring = false
				var item_name: String = item.description(true, 3)
				full_text = "%s%s%s%s [%s]" % [partial_1, partial_2, partial_3, item_name, info]
				Console.print_line_at(0, 0, full_text, Ink.WHITE)
				dungeon_level.show_cursor_relative(row, col)
				query = User.get_key()
				if query != "Enter" and query != " ":
					break
				if query == " " and (mode & TARGET_LOOK) == 0:
					break
				partial_1 = "It is "
				if item.count != 1:
					partial_1 = "They are "
				partial_2 = "on "
				item_index = item.next_in_stack
		if item_index != 0:
			break
		var tile_types: Dictionary[String, FloorTileType] = StaticData.tile_types
		var feat: String = tile_types[tile.background_feature.appear_as].name if tile.feature_type.appear_as == "" else tile_types[tile.feature_type.appear_as].name
		if tile.tile_flags.is_clear(GridTile.PLAYER_MEMORISED) and not dungeon_level.player_can_see_bold(row, col):
			feat = ""
		if boring or not tile.feature_type.is_open_floor:
			var name: String = "unknown grid"
			if feat != "":
				name = tile_types[feat].description
				if partial_2 != "" and tile.feature_type.blocks_los:
					partial_2 = "in "
			partial_3 = "an " if Util.is_vowel(name[0]) else "a "
			if tile.feature_type.is_shop:
				partial_3 = "the entrance to an " if Util.is_vowel(name[0]) else "the entrance to a "
			full_text = "%s%s%s%s [%s]" % [partial_1, partial_2, partial_3, name, info]
			Console.print_line_at(0, 0, full_text, Ink.WHITE)
			dungeon_level.show_cursor_relative(row, col)
			query = User.get_key()
			if query != "Enter" and query != " ":
				break
	return query


func look_mon_desc(monster_index: int) -> String:
	var monster: Monster = dungeon_level.dungeon_monsters.monsters[monster_index]
	var monster_type: MonsterType = monster.monster_type
	var living: bool = monster_type.flags_3.is_set(MonsterFlags3.UNDEAD)
	if monster_type.flags_3.is_set(MonsterFlags3.DEMON):
		living = false
	if monster_type.flags_3.is_set(MonsterFlags3.CTHULOID):
		living = false
	if monster_type.flags_3.is_set(MonsterFlags3.NON_LIVING):
		living = false
	if "Egv".contains(monster_type.character):
		living = false
	if monster.health >= monster.max_health:
		return "unhurt" if living else "undamaged"
	var perc: int = 100 * monster.health / monster.max_health
	if perc >= 60:
		return "somewhat wounded" if living else "somewhat damaged"
	if perc >= 25:
		return "wounded" if living else "damaged"
	if perc >= 10:
		return "badly wounded" if living else "badly damaged"
	return "almost dead" if living else "almost destroyed"


func target_point(location: Location) -> bool:
	var success: bool = false
	location.row = player.map_row
	location.col = player.map_col
	Profile.instance.message_print("Select a point and press space.")
	var ch: String
	while ch != "Escape" and ch != " ":
		dungeon_level.show_cursor_relative(location.row, location.col)
		ch = User.get_key()
		ch = User.strip_movement_keys(ch)
		match ch:
			"Escape":
				break
			" ":
				success = true
				break
			_:
				var d: int = User.key_to_keypad_direction(ch)
				if d == 0:
					continue
				location.row += dungeon_level.keypad_direction_row_offset[d]
				location.col += dungeon_level.keypad_direction_col_offset[d]
				if location.col >= dungeon_level.cur_wid - 1 or location.col >= dungeon_level.panel_col_min + Console.CONSOLE_WIDTH:
					location.col -= 1
				elif location.col <= 0 or location.col <= dungeon_level.panel_col_min:
					location.col += 1
				if location.row >= dungeon_level.cur_hgt - 1 or location.row >= dungeon_level.panel_row_min + Console.CONSOLE_HEIGHT:
					location.row -= 1
				elif location.row <= 0 or location.row <= dungeon_level.panel_row_min:
					location.row += 1
				continue
	Console.hide_cursor()
	return success
