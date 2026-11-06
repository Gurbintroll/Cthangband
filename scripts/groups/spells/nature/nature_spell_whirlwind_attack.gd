# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name NatureSpellWhirlwindAttack
extends Spell


func _init() -> void:
	default_base_failure = 80
	default_level = 23
	default_power_cost = 23
	first_cast_experience = 50
	name = "Whirlwind Attack"


func cast(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	for dir: int in 10:
		var row: int = player.map_row + dungeon_level.keypad_direction_row_offset[dir]
		var col: int = player.map_col + dungeon_level.keypad_direction_col_offset[dir]
		var tile: GridTile = dungeon_level.get_grid(row, col)
		var monster: Monster = dungeon_level.dungeon_monsters.monsters[tile.monster_index]
		if tile.monster_index != 0 and (monster.is_visible or dungeon_level.grid_passable(row, col)):
			savegame.command_engine.player_attack_monster(row, col)


func comment(player: Player) -> String:
	return ""
