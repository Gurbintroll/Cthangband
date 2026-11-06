# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationSpeedFlux
extends Mutation


func initialise() -> void:
	frequency = 2
	gain_message = "You have become unstuck in time."
	have_message = "You move faster or slower randomly."
	lose_message = "You are firmly anchored in time."


func on_process_world(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if Rng.roll_die(6000) != 1:
		return
	savegame.disturb(false)
	if Rng.roll_die(2) == 1:
		Profile.instance.message_print("Everything around you speeds up.")
		if player.timed_haste != 0:
			player.timed_haste = 0
		else:
			player.set_timed_slow(player.timed_slow + Rng.roll_die(30) + 10)
	else:
		Profile.instance.message_print("Everything around you slows down.")
		if player.timed_slow != 0:
			player.timed_slow = 0
		else:
			player.set_timed_haste(player.timed_haste + Rng.roll_die(30) + 10)
	Profile.instance.message_print("")
