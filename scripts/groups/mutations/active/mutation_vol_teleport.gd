# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationVolTeleport
extends Mutation


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not savegame.command_engine.check_if_racial_power_works(7, 7, Ability.WISDOM, 15):
		return
	savegame.spell_effects_handler.teleport_player(1 - +(4 * player.level))


func activation_summary(level: int) -> String:
	return "teleport         (unusable until level 7)" if level < 7 else "teleport         (cost 7, WIS based)"


func initialise() -> void:
	frequency = 3
	gain_message = "You gain the power of teleportation at will."
	have_message = "You can teleport at will."
	lose_message = "You lose the power of teleportation at will."
