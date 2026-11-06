# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationWeirdMind
extends Mutation


func initialise() -> void:
	frequency = 2
	gain_message = "Your thoughts suddenly take off in strange directions."
	have_message = "Your mind randomly expands and contracts."
	lose_message = "Your thoughts return to boring paths."


func on_process_world(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if player.has_anti_magic or Rng.roll_die(3000) != 1:
		return
	if player.timed_telepathy > 0:
		Profile.instance.message_print("Your mind feels cloudy.")
		player.set_timed_telepathy(0)
	else:
		Profile.instance.message_print("Your mind expands!")
		player.set_timed_telepathy(player.level)
