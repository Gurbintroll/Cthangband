# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationHealthToPower
extends Mutation


func initialise() -> void:
	frequency = 1
	gain_message = "You are subject to fits of painful clarity."
	have_message = "Your blood sometimes rushes to your head."
	lose_message = "You are no longer subject to fits of painful clarity."


func on_process_world(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if player.has_anti_magic or Rng.roll_die(4000) != 1:
		return
	var wounds: int = player.max_power - player.power
	if wounds <= 0:
		return
	var healing: int = player.health
	if healing > wounds:
		healing = wounds
	player.power += healing
	player.take_hit(healing, "blood rushing to the head")
