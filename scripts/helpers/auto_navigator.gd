# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AutoNavigator
extends RefCounted

const CYCLIC_ENTRY_POINT: Array[int] = [0, 8, 9, 10, 7, 0, 11, 6, 5, 4]
const DIRECTION_CYCLE: Array[int] = [1, 2, 3, 6, 9, 8, 7, 4, 1, 2, 3, 6, 9, 8, 7, 4, 1]

var current_run_direction: int
var _find_break_left: bool
var _find_break_right: bool
var _find_open_area: bool
var _previous_run_direction: int
var _player: Player
var _savegame: SaveGame


func _init() -> void:
	current_run_direction = 5
	_previous_run_direction = 5
	_find_open_area = true
	_find_break_left = false
	_find_break_right = false
	_savegame = SaveGame.instance
	_player = _savegame.player


func set_direction(direction: int) -> void:
	current_run_direction = direction
	_previous_run_direction = direction
	_find_open_area = true
	_find_break_right = false
	_find_break_left = false
	var wall_double_ahead_left: bool = false
	var wall_double_ahead_right: bool = false
	var wall_ahead_right: bool = false
	var wall_ahead_left: bool = false
	# Get the row and column of the first step in the run
	var row: int = _player.map_row + _savegame.dungeon_level.keypad_direction_row_offset[direction]
	var col: int = _player.map_col + _savegame.dungeon_level.keypad_direction_col_offset[direction]
	# Get the index of our run direction in the cycle
	var cycle_index: int = CYCLIC_ENTRY_POINT[direction]
	# If there's a wall ahead-left of us, remember that
	if see_wall(DIRECTION_CYCLE[cycle_index + 1], _player.map_row, _player.map_col):
		_find_break_left = true
		wall_ahead_left = true
	# Else check if there's a wall ahead-left of our first step, and remember that
	elif see_wall(DIRECTION_CYCLE[cycle_index + 1], row, col):
		_find_break_left = true
		wall_double_ahead_left = true
	# If there's a wall ahead-right of us, remember that
	if see_wall(DIRECTION_CYCLE[cycle_index - 1], _player.map_row, _player.map_col):
		_find_break_right = true
		wall_ahead_right = true
	# Else check if there's a wall ahead-right of our first step, and remember that
	elif see_wall(DIRECTION_CYCLE[cycle_index - 1], row, col):
		_find_break_right = true
		wall_double_ahead_right = true
	# If we're looking for breaks on either side, we're not looking for an open area
	if _find_break_left and _find_break_right:
		_find_open_area = false
		# If we're moving orthogonally and we have a wall double-ahead on either side,
		# nudge our assumed previous direction away from it
		if (direction & 0x01) != 0:
			if wall_double_ahead_left and not wall_double_ahead_right:
				_previous_run_direction = DIRECTION_CYCLE[cycle_index - 1]
			elif wall_double_ahead_right and not wall_double_ahead_left:
				_previous_run_direction = DIRECTION_CYCLE[cycle_index + 1]
		# If there's a wall directly ahead but not diagonally ahead, nudge our assumed
		# previous direction away from it
		elif see_wall(DIRECTION_CYCLE[cycle_index], row, col):
			if wall_ahead_left and not wall_ahead_right:
				_previous_run_direction = DIRECTION_CYCLE[cycle_index - 2]
			elif wall_ahead_right and not wall_ahead_left:
				_previous_run_direction = DIRECTION_CYCLE[cycle_index + 2]


func navigate_next_step() -> bool:
	var new_direction: int
	var check_direction: int = 0
	var option_1: int = 0
	var option_2: int = 0
	var previous_direction: int = _previous_run_direction
	var level: DungeonLevel = _savegame.dungeon_level
	var row: int
	var col: int
	var tile: GridTile
	# Set our search width to 1 if we're moving diagonally, or two if we're moving orthogonally
	var search_width: int = (previous_direction & 0x01) + 1
	# Search to either side from right to left with a width equal to the search width
	for i: int in range(-search_width, search_width + 1):
		# Pick up the tile 0-2 rotations from the direction we previously moved
		new_direction = DIRECTION_CYCLE[CYCLIC_ENTRY_POINT[previous_direction] + i]
		row = _player.map_row + level.keypad_direction_row_offset[new_direction]
		col = _player.map_col + level.keypad_direction_col_offset[new_direction]
		tile = level.get_grid(row, col)
		# If there's a monster there we must stop moving
		if tile.monster_index != 0:
			var monster: Monster = level.dungeon_monsters.monsters[tile.monster_index]
			if monster.is_visible:
				return true
		# If there's an item there we weren't previously aware of then we must stop moving
		var item_index: int = tile.item_index
		while item_index != 0:
			var item: Item = level.dungeon_items.items[item_index]
			item_index = item.next_in_stack
			if item.marked:
				return true
		var tile_unseen: bool = true
		# If the tile is something we should not run past then we must stop moving
		if tile.tile_flags.is_set(GridTile.PLAYER_MEMORISED):
			var notice: bool = not tile.feature_type.run_past
			if notice:
				return true
			tile_unseen = false
		# We can enter the tile or it's unseen
		if tile_unseen or level.grid_passable(row, col):
			if _find_open_area:
				# Ignore the open area
				pass
			elif option_1 == 0:
				# It's an option_1 for changing direction to
				option_1 = new_direction
			# We shouldn't be finding open areas, but we did find one so we have to stop
			elif option_2 != 0:
				return true
			# We've previously found an option_1 and it isn't just a diagonal corner-cut from
			# this one so we have to stop
			elif option_1 != DIRECTION_CYCLE[CYCLIC_ENTRY_POINT[previous_direction] + i - 1]:
				return true
			# If we're running diagonally then we can have a second option_1 two away
			elif (new_direction & 0x01) != 0:
				check_direction = DIRECTION_CYCLE[CYCLIC_ENTRY_POINT[previous_direction] + i - 2]
				option_2 = new_direction
			# We're running orthogonally so we can have a second option_1 one away
			else:
				check_direction = DIRECTION_CYCLE[CYCLIC_ENTRY_POINT[previous_direction] + i + 1]
				option_2 = option_1
				option_1 = new_direction
		else:
			# If we were looking for an open area we're now just looking for a left or right
			if _find_open_area:
				if i < 0:
					_find_break_right = true
				elif i > 0:
					_find_break_left = true
	# If we're looking for an open area, search both directions
	if _find_open_area:
		# Look left first
		for i: int in range(-search_width, 0):
			new_direction = DIRECTION_CYCLE[CYCLIC_ENTRY_POINT[previous_direction] + i]
			row = _player.map_row + level.keypad_direction_row_offset[new_direction]
			col = _player.map_col + level.keypad_direction_col_offset[new_direction]
			tile = level.get_grid(row, col)
			if tile.tile_flags.is_clear(GridTile.PLAYER_MEMORISED) or not tile.feature_type.is_wall:
				if _find_break_right:
					return true
			else:
				if _find_break_left:
					return true
		# Then look left
		for i: int in range(search_width, 0, -1):
			new_direction = DIRECTION_CYCLE[CYCLIC_ENTRY_POINT[previous_direction] + i]
			row = _player.map_row + level.keypad_direction_row_offset[new_direction]
			col = _player.map_col + level.keypad_direction_col_offset[new_direction]
			tile = level.get_grid(row, col)
			if tile.tile_flags.is_clear(GridTile.PLAYER_MEMORISED) or not tile.feature_type.is_wall:
				if _find_break_left:
					return true
			else:
				if _find_break_right:
					return true
	# Not looking for an open area
	else:
		# If we have nowhere else to run, we must stop
		if option_1 == 0:
			return true
		# If we only have one option_1, take it
		if option_2 == 0:
			current_run_direction = option_1
			_previous_run_direction = option_1
		# if we don't see a wall in one of our two options, take that one
		else:
			row = _player.map_row + level.keypad_direction_row_offset[option_1]
			col = _player.map_col + level.keypad_direction_col_offset[option_1]
			if not see_wall(option_1, row, col) or not see_wall(check_direction, row, col):
				if see_nothing(option_1, row, col) and see_nothing(option_2, row, col):
					current_run_direction = option_1
					_previous_run_direction = option_2
				else:
					return true
			else:
				current_run_direction = option_2
				_previous_run_direction = option_2
	# No options, so just return whether or not we can move forward
	return see_wall(current_run_direction, _player.map_row, _player.map_col)


func see_wall(direction: int, row: int, col: int) -> bool:
	var level: DungeonLevel = _savegame.dungeon_level
	row += level.keypad_direction_row_offset[direction]
	col += level.keypad_direction_col_offset[direction]
	# Out of bounds is not a wall
	if not level.in_outer_bounds(row, col):
		return false
	# Any passable grid is okay
	if level.grid_passable(row, col):
		return false
	# If we don't know what's there it's okay
	if level.get_grid(row, col).tile_flags.is_clear(GridTile.PLAYER_MEMORISED):
		return false
	# It was impassable and we knew it
	return true


func see_nothing(direction: int, row: int, col: int) -> bool:
	var level:DungeonLevel = _savegame.dungeon_level
	row += level.keypad_direction_row_offset[direction]
	col += level.keypad_direction_col_offset[direction]
	# Out of bounds is empty
	if not level.in_outer_bounds(row, col):
		return true
	# Unknown tiles are not empty
	if level.get_grid(row, col).tile_flags.is_set(GridTile.PLAYER_MEMORISED):
		return false
	# Anything we can walk through is empty
	if not level.grid_passable(row, col):
		return true
	# It's empty if we can't see it
	return not level.player_can_see_bold(row, col)
