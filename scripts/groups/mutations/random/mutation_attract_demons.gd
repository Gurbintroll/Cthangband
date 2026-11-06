# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationAttractDemons
extends Mutation


func initialise() -> void:
	frequency = 2
	gain_message = "You start attracting demons."
	have_message = "You attract demons."
	lose_message = "You stop attracting demons."


func on_process_world(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if player.has_anti_magic or Rng.roll_die(6666) != 666:
		return
	var worked: bool = false
	if Rng.roll_die(6) == 1:
		worked = dungeon_level.dungeon_monsters.summon_specific_friendly(player.map_row, player.map_col, savegame.total_difficulty(), SummonType.SUMMON_DEMON, true)
	else:
		worked = dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, savegame.total_difficulty(), SummonType.SUMMON_DEMON)
	if not worked:
		return
	Profile.instance.message_print("You have attracted a demon!")
	savegame.disturb(false)
