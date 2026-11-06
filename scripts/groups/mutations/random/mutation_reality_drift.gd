# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationRealityDrift
extends Mutation


func initialise() -> void:
	frequency = 1
	gain_message = "You feel like reality is as thin as paper."
	have_message = "You occasionally stumble into other realities."
	lose_message = "You feel like you're trapped in reality."


func on_process_world(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if Rng.roll_die(12000) != 1:
		return
	savegame.spell_effects_handler.alter_reality()
