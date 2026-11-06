# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationPowerToHealth
extends Mutation


func initialise() -> void:
	frequency = 2
	gain_message = "You are subject to fits of magical healing."
	have_message = "Your blood sometimes rushes to your muscles."
	lose_message = "You are no longer subject to fits of magical healing."


func on_process_world(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if Rng.roll_die(2000) != 1:
		return
	var wounds: int = player.max_health - player.health
	if wounds <= 0:
		return
	var healing: int = player.power
	if healing > wounds:
		healing = wounds
	player.restore_health(healing)
	player.power -= healing
