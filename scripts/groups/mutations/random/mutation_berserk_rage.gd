# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationBerserkRage
extends Mutation


func initialise() -> void:
	frequency = 1
	gain_message = "You become subject to fits of berserk rage!"
	have_message = "You are subject to berserker fits."
	lose_message = "You are no longer subject to fits of berserk rage!"


func on_process_world(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if Rng.roll_die(3000) != 1:
		return
	savegame.disturb(false)
	Profile.instance.message_print("You feel a fit of rage coming over you!")
	Profile.instance.message_print("WAAAAAAAGH!")
	player.set_timed_superheroism(player.timed_super_heroism + Rng.roll_die(player.level) + 10)
