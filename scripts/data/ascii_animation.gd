# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name AsciiAnimation
extends Resource

@export var name: String
@export var colour: int
@export var alternate_colour: int
@export var sequence: String


func animate_cell(dungeon_level: DungeonLevel, row: int, col: int) -> void:
	var drawn: bool = false
	var odd_frame: bool = true
	for character: String in sequence:
		if dungeon_level.player_has_los_bold(row, col) and dungeon_level.panel_contains(row, col):
			var cell_colour: int = colour if odd_frame else alternate_colour
			dungeon_level.print_character_at_map_location(character, cell_colour, row, col)
			drawn = true
		if drawn:
			User.wait_for_animation()
		odd_frame = !odd_frame
	if drawn:
		if dungeon_level.player_has_los_bold(row, col) and dungeon_level.panel_contains(row, col):
			dungeon_level.redraw_single_location(row, col)


func animate_cells(dungeon_level: DungeonLevel, row: Array[int], col: Array[int]) -> void:
	var grids: int = col.size()
	var drawn: bool = false
	var odd_frame: bool = true
	for character: String in sequence:
		for i: int in grids:
			if (
				dungeon_level.player_has_los_bold(row[i], col[i])
				and dungeon_level.panel_contains(row[i], col[i])
			):
				var cell_colour: int = colour if odd_frame else alternate_colour
				dungeon_level.print_character_at_map_location(
					character,
					cell_colour,
					row[i],
					col[i],
				)
				drawn = true
		if drawn:
			User.wait_for_animation()
		odd_frame = !odd_frame
	if drawn:
		for i: int in grids:
			if (
				dungeon_level.player_has_los_bold(row[i], col[i])
				and dungeon_level.panel_contains(row[i], col[i])
			):
				dungeon_level.redraw_single_location(row[i], col[i])
