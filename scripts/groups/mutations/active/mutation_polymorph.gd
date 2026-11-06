# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name MutationPolymorph
extends Mutation


func activate(savegame: SaveGame, player: Player, dungeon_level: DungeonLevel) -> void:
	if not savegame.command_engine.check_if_racial_power_works(18, 20, Ability.CONSTITUTION, 18):
		return
	player.polymorph_self()


func activation_summary(level: int) -> String:
	return "polymorph        (unusable until level 18)" if level < 18 else "polymorph        (cost 20, CON based)"


func initialise() -> void:
	frequency = 1
	gain_message = "Your body seems mutable."
	have_message = "You can polymorph yourself at will."
	lose_message = "Your body seems stable."
