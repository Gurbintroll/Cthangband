# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationWraith
extends Mutation


func initialise() -> void:
	frequency = 1
	gain_message = "You start to fade in and out of the physical world."
	have_message = "You fade in and out of physical reality."
	lose_message = "You are firmly in the physical world."


func on_process_world(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if player.has_anti_magic or Rng.roll_die(3000) != 13:
		return
	savegame.disturb(false)
	Profile.instance.message_print("You feel insubstantial!")
	Profile.instance.message_print("")
	player.set_timed_etherealness(player.timed_etherealness + Rng.roll_die(player.level / 2) + player.level / 2)
