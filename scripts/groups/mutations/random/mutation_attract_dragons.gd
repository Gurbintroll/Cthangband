# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationAttractDragons
extends Mutation


func initialise() -> void:
	frequency = 1
	gain_message = "You start attracting dragons."
	have_message = "You attract dragons."
	lose_message = "You stop attracting dragons."


func on_process_world(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if player.has_anti_magic or Rng.roll_die(3000) != 13:
		return
	var worked: bool = false
	if Rng.roll_die(5) == 1:
		worked = dungeon_level.dungeon_monsters.summon_specific_friendly(player.map_row, player.map_col, savegame.total_difficulty(), SummonType.SUMMON_DRAGON, true)
	else:
		worked = dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, savegame.total_difficulty(), SummonType.SUMMON_DRAGON)
	if not worked:
		return
	Profile.instance.message_print("You have attracted a dragon!")
	savegame.disturb(false)
