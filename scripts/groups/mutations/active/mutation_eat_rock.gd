# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationEatRock
extends Mutation


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not savegame.command_engine.check_if_racial_power_works(8, 12, Ability.CONSTITUTION, 18):
		return
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var dir: int = target_engine.get_direction_no_aim()
	if dir == -1:
		return
	var row: int = player.map_row + dungeon_level.keypad_direction_row_offset[dir]
	var col: int = player.map_col + dungeon_level.keypad_direction_col_offset[dir]
	var tile: GridTile = dungeon_level.get_grid(row, col)
	if dungeon_level.grid_passable(row, col):
		Profile.instance.message_print("You bite into thin air!")
		return
	if tile.feature_type.is_permanent:
		Profile.instance.message_print("Ouch!")
		Profile.instance.message_print("This wall is harder than your teeth!")
		return
	if tile.monster_index != 0:
		Profile.instance.message_print("There is something in the way!")
		return
	if tile.feature_type.category == FloorTileTypeCategory.TREE or tile.feature_type.is_closed_door:
		Profile.instance.message_print("You don't like the woody taste!")
		player.set_food(player.food + 1000)
	elif tile.feature_type.category == FloorTileTypeCategory.SECRET_DOOR or tile.feature_type.category == FloorTileTypeCategory.RUBBLE:
		player.set_food(player.food + 3000)
	elif tile.feature_type.category == FloorTileTypeCategory.VEIN:
		player.set_food(player.food + 5000)
	else:
		Profile.instance.message_print("This granite is very filling!")
		player.set_food(player.food + 10000)
	savegame.spell_effects_handler.wall_to_mud(dir)
	var old_row: int = player.map_row
	var old_col: int = player.map_col
	player.map_row = row
	player.map_col = col
	dungeon_level.redraw_single_location(row, col)
	dungeon_level.redraw_single_location(old_row, old_col)
	target_engine.recenter_screen_around_player()
	player.updates_needed.set_bits(UpdateFlags.UPDATE_VIEW | UpdateFlags.UPDATE_LIGHT | UpdateFlags.UPDATE_SCENT)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_DISTANCES)


func activation_summary(level: int) -> String:
	return "eat rock         (unusable until level 8)" if level < 8 else "eat rock         (cost 12, CON based)"


func initialise() -> void:
	frequency = 2
	gain_message = "The walls look delicious."
	have_message = "You can consume solid rock."
	lose_message = "The walls look unappetizing."
