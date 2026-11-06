# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationFlatulence
extends Mutation


func initialise() -> void:
	frequency = 1
	gain_message = "You become subject to uncontrollable flatulence."
	have_message = "You are subject to uncontrollable flatulence."
	lose_message = "You are no longer subject to uncontrollable flatulence."


func on_process_world(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if Rng.roll_die(3000) == 13:
		savegame.disturb(false)
		Profile.instance.message_print("Thrrrrrp! Oops.")
		Profile.instance.message_print("")
		savegame.spell_effects_handler.fire_ball(ProjectilePoison.new(savegame.spell_effects_handler), 0, player.level, 3)
