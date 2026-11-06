# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationInvuln
extends Mutation


func initialise() -> void:
	frequency = 1
	gain_message = "You are blessed with fits of invulnerability."
	have_message = "You occasionally feel invincible."
	lose_message = "You are no longer blessed with fits of invulnerability."


func on_process_world(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not player.has_anti_magic and Rng.roll_die(5000) != 1:
		savegame.disturb(false)
		Profile.instance.message_print("You feel invincible!")
		Profile.instance.message_print("")
		player.set_timed_invulnerability(player.timed_invulnerability + Rng.roll_die(8) + 8)
