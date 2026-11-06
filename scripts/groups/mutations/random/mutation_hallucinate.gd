# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationHallucinate
extends Mutation


func initialise() -> void:
	frequency = 1
	gain_message = "You are afflicted by a hallucinatory insanity!"
	have_message = "You have a hallucinatory insanity!"
	lose_message = "You are no longer afflicted by a hallucinatory insanity!"


func on_process_world(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if Rng.roll_die(6400) != 42:
		return
	if player.has_chaos_resistance:
		return
	savegame.disturb(false)
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_EXTRA)
	player.set_timed_hallucinations(player.timed_hallucinations + Rng.roll_die(50) + 20)
