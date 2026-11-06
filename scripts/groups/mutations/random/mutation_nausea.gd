# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationNausea
extends Mutation


func initialise() -> void:
	frequency = 1
	gain_message = "Your stomach starts to roil nauseously."
	have_message = "You have a seriously upset stomach."
	lose_message = "Your stomach stops roiling."


func on_process_world(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if player.has_slow_digestion or Rng.roll_die(9000) != 1:
		return
	savegame.disturb(false)
	Profile.instance.message_print("Your stomach roils, and you lose your lunch!")
	Profile.instance.message_print("")
	player.set_food(Constants.FOOD_WEAK)
