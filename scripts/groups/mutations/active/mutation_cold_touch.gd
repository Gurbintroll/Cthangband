# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationColdTouch
extends Mutation


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not savegame.command_engine.check_if_racial_power_works(2, 2, Ability.CONSTITUTION, 11):
		return
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var dir: int = target_engine.get_direction_no_aim()
	if dir == -1:
		return
	var row: int = player.map_row + dungeon_level.keypad_direction_row_offset[dir]
	var col: int = player.map_col + dungeon_level.keypad_direction_col_offset[dir]
	var tile: GridTile = dungeon_level.get_grid(row, col)
	if tile.monster_index == 0:
		Profile.instance.message_print("You wave your hands in the air.")
		return
	savegame.spell_effects_handler.fire_bolt(ProjectileCold.new(savegame.spell_effects_handler), dir, 2 * player.level)


func activation_summary(level: int) -> String:
	return "cold touch       (unusable until level 2)" if level < 2 else "cold touch       (cost 2, CON based)"


func initialise() -> void:
	frequency = 2
	gain_message = "Your hands get very cold."
	have_message = "You can freeze things with a touch."
	lose_message = "Your hands warm up."
