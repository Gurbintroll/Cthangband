# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationProduceChaos
extends Mutation


func initialise() -> void:
	frequency = 1
	gain_message = "You feel the universe is less stable around you."
	have_message = "You are occasionally surrounded with raw chaos."
	lose_message = "You feel the universe is more stable around you."


func on_process_world(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if player.has_anti_magic or Rng.roll_die(8000) != 1:
		return
	savegame.disturb(false)
	Profile.instance.message_print("You feel the world warping around you!")
	Profile.instance.message_print("")
	savegame.spell_effects_handler.fire_ball(ProjectileChaos.new(savegame.spell_effects_handler), 0, player.level, 8)
