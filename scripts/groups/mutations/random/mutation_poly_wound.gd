# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationPolyWound
extends Mutation


func initialise() -> void:
	frequency = 1
	gain_message = "You feel forces of chaos entering your old scars."
	have_message = "Your health is subject to chaotic forces."
	lose_message = "You feel forces of chaos departing your old scars."


func on_process_world(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if Rng.roll_die(3000) != 1:
		return
	player.polymorph_wounds()
