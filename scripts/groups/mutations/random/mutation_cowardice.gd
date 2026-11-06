# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationCowardice
extends Mutation


func initialise() -> void:
	frequency = 1
	gain_message = "You become an incredible coward!"
	have_message = "You are subject to cowardice."
	lose_message = "You are no longer an incredible coward!"
	group = MutationGroup.BRAVERY


func on_process_world(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if Rng.roll_die(3000) != 13:
		return
	if player.has_fear_resistance or player.timed_heroism != 0 or player.timed_super_heroism != 0:
		return
	savegame.disturb(false)
	Profile.instance.message_print("It's so dark... so scary!")
	player.redraw_needed.set_bits(RedrawFlags.REDRAW_AFRAID)
	player.timed_fear = player.timed_fear + Rng.roll_die(26) + 13
