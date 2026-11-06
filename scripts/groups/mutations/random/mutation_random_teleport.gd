# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationRandomTeleport
extends Mutation


func initialise() -> void:
	frequency = 1
	gain_message = "Your position seems very uncertain..."
	have_message = "You are teleporting randomly."
	lose_message = "Your position seems more certain."


func on_process_world(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if Rng.roll_die(5000) != 88:
		return
	if player.has_anti_teleport or player.has_nexus_resistance:
		return
	savegame.disturb(false)
	Profile.instance.message_print("Your position suddenly seems very uncertain...")
	Profile.instance.message_print("")
	savegame.spell_effects_handler.teleport_player(40)
