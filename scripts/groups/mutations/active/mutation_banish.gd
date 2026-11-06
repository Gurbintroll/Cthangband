# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationBanish
extends Mutation


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not savegame.command_engine.check_if_racial_power_works(25, 25, Ability.WISDOM, 18):
		return
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var dir: int = target_engine.get_direction_no_aim()
	if dir == -1:
		return
	var row: int = player.map_row + dungeon_level.keypad_direction_row_offset[dir]
	var col: int = player.map_col + dungeon_level.keypad_direction_col_offset[dir]
	var tile: GridTile = dungeon_level.get_grid(row, col)
	if tile.monster_index == 0:
		Profile.instance.message_print("You sense no evil there!")
		return
	var monster: Monster = dungeon_level.dungeon_monsters.monsters[tile.monster_index]
	var monster_type: MonsterType = monster.monster_type
	if monster_type.flags_3.is_set(MonsterFlags3.EVIL):
		dungeon_level.dungeon_monsters.delete_monster_by_index(tile.monster_index, true)
		Profile.instance.message_print("The evil creature vanishes in a puff of sulfurous smoke!")
	else:
		Profile.instance.message_print("Your invocation is ineffectual!")


func activation_summary(level: int) -> String:
	return "banish evil      (unusable until level 25)" if level < 25 else "banish evil      (cost 25, WIS based)"


func initialise() -> void:
	frequency = 1
	gain_message = "You feel a holy wrath fill you."
	have_message = "You can send evil creatures directly to Hell."
	lose_message = "You no longer feel a holy wrath."
