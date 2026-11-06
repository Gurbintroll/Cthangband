# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationPanicHit
extends Mutation


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not savegame.command_engine.check_if_racial_power_works(10, 12, Ability.DEXTERITY, 14):
		return
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var dir: int = target_engine.get_direction_no_aim()
	if dir == -1:
		return
	var row: int = player.map_row + dungeon_level.keypad_direction_row_offset[dir]
	var col: int = player.map_col + dungeon_level.keypad_direction_col_offset[dir]
	var tile: GridTile = dungeon_level.get_grid(row, col)
	if tile.monster_index == 0:
		savegame.command_engine.player_attack_monster(row, col)
		savegame.spell_effects_handler.teleport_player(30)
	else:
		Profile.instance.message_print("You don't see any monster in this direction.")


func activation_summary(level: int) -> String:
	return "panic hit        (unusable until level 10)" if level < 10 else "panic hit        (cost 12, DEX based)"


func initialise() -> void:
	frequency = 2
	gain_message = "You suddenly understand how thieves feel."
	have_message = "You can run for your life after hitting something."
	lose_message = "You no longer feel jumpy."
