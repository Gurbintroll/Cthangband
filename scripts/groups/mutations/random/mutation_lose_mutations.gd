# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationLoseMutations
extends Mutation


func initialise() -> void:
	frequency = 3
	gain_message = "You feel strangely normal."
	have_message = "You may be mutated, but you're recovering."
	lose_message = "You feel normally strange."


func on_process_world(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if Rng.roll_die(5000) != 1:
		return
	player.genome.lose_mutation()
